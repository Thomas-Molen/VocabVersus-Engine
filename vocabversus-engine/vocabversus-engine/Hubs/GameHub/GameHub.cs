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
            _gameInstanceCache.UserDisconnected(playerConnection.PlayerIdentifier, playerConnection.GameInstanceIdentifier);
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
                _gameInstanceCache.UserJoined(personalIdentifier, username, gameId);
            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Could not add user, either the game is full or user has already joined game instance", GameHubExceptionCode.UserAddFailed);
            }

            // subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).SendAsync("UserJoined", username, Context.ConnectionId);

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
    }
}
