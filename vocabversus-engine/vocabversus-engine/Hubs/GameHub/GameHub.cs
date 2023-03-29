using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using vocabversus_engine.Hubs.GameHub.Responses;
using vocabversus_engine.Models;
using vocabversus_engine.Models.Exceptions;
using vocabversus_engine.Utility;

namespace vocabversus_engine.Hubs.GameHub
{
    public class GameHub : Hub
    {
        private readonly IGameInstanceCache _gameInstanceCache;
        private readonly IPlayerConnectionCache _playerConnectionCache;
        public GameHub(IGameInstanceCache gameInstanceCache, IPlayerConnectionCache playerConnectionCache)
        {
            _gameInstanceCache = gameInstanceCache;
            _playerConnectionCache = playerConnectionCache;
        }
        // Trigger on connecting to the Hub
        public override async Task OnConnectedAsync()
        {
        }

        // When player connection goes out of scope, notify all relevant games
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            PlayerConnection? playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection is null) return;
            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier);
            if (gameInstance is null) return;
            gameInstance.PlayerInformation.DisconnectPlayer(playerConnection.PlayerIdentifier);
            await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("UserLeft", Context.ConnectionId);
        }

        [HubMethodName("CheckGame")]
        public async Task CheckGameInstanceAvailability(string gameId)
        {
            // Get initialized game instance data if available
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);
        }

        [HubMethodName("Join")]
        public async Task<JoinGameInstanceResponse> JoinGameInstance(string gameId, string username)
        {
            // Get initialized game instance data, if no game instance was found either no game with given Id has been initialized or the session has expired
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);
            var personalIdentifier = Context.ConnectionId;
            try
            {
                gameInstance.PlayerInformation.AddPlayer(personalIdentifier, username);
                
            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Could not add user, either the game is full or user has already joined game instance", GameHubExceptionCode.UserAddFailed);
            }

            // subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).SendAsync("UserJoined", username, personalIdentifier);

            // add connection instance to the connections cache for reference when the context goes out of scope (e.g. connection disconnects)
            _playerConnectionCache.Register(new PlayerConnection
            {
                Identifier = Context.ConnectionId,
                GameInstanceIdentifier = gameId,
                PlayerIdentifier = personalIdentifier
            }, Context.ConnectionId);

            return new JoinGameInstanceResponse
            {
                PersonalIdentifier = personalIdentifier,
                Players = gameInstance.PlayerInformation.Players
            };
        }

        [HubMethodName("Kick")]
        public async Task KickPlayerFromGameInstance(string gameId, string userIdentifier)
        {
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);
            if (gameInstance.PlayerInformation.Players.FirstOrDefault(p => p.Key == userIdentifier).Value.isConnected) throw GameHubException.Create("Active players can not be kicked", GameHubExceptionCode.ActionNotAllowed);
            gameInstance.PlayerInformation.RemovePlayer(userIdentifier);
            await Clients.OthersInGroup(gameId).SendAsync("UserRemoved", userIdentifier);
        }

        [HubMethodName("Ready")]
        public async Task SetPlayerReadyState(string gameId, bool readyState)
        {
            var personalIdentifier = Context.ConnectionId;
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);
            try
            {
                gameInstance.PlayerInformation.SetPlayerReadyState(personalIdentifier, readyState);
            }
            catch (GameInstanceException)
            {
                throw GameHubException.CreateIdentifierError(gameId);
            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Failed to set user ready state", GameHubExceptionCode.UserEditFailed);
            }
            await Clients.OthersInGroup(gameId).SendAsync("UserReady", readyState, personalIdentifier);
        }
    }
}
