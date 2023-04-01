using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using vocabversus_engine.Hubs.GameHub.Responses;
using vocabversus_engine.Models;
using vocabversus_engine.Models.Exceptions;
using vocabversus_engine.Models.Responses;
using vocabversus_engine.Services;
using vocabversus_engine.Utility;

namespace vocabversus_engine.Hubs.GameHub
{
    public class GameHub : Hub
    {
        private readonly IGameInstanceCache _gameInstanceCache;
        private readonly IPlayerConnectionCache _playerConnectionCache;
        private readonly IGameEventService _gameEventService;
        public GameHub(IGameInstanceCache gameInstanceCache, IPlayerConnectionCache playerConnectionCache, IGameEventService gameEventService)
        {
            _gameInstanceCache = gameInstanceCache;
            _playerConnectionCache = playerConnectionCache;
            _gameEventService = gameEventService;
        }

        // When player connection goes out of scope, notify all relevant games
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            PlayerConnection? playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection is null || playerConnection.GameInstanceIdentifier is null) return;
            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier);
            if (gameInstance is null) return;
            gameInstance.PlayerInformation.DisconnectPlayer(playerConnection.PlayerIdentifier);
            await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("UserLeft", playerConnection.PlayerIdentifier);
        }

        [HubMethodName("CheckGame")]
        public async Task<CheckGameInstanceResponse> CheckGameInstanceAvailability(string gameId, string? userId)
        {
            // Get initialized game instance data if available
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);

            // check if the game already contained the userId, as this means the user can reconnect
            bool canReconnect = gameInstance.PlayerInformation.Players.Any(p => p.Key == userId);

            // Add the player connection to internal cache for future reference
            // this cache is used to handle player actions when connectionId is out of scope and moves game instance referencing responsibility to the server
            // TODO: userID is currently error prone to multiple signalR hub instances running concurrently, as two players could be using the same userID
            //       to solve this use a central database storer to store userID's that have been used, this way actions of that user could also be tracked
            userId ??= Guid.NewGuid().ToString();
            _playerConnectionCache.Register(new PlayerConnection
            {
                ConnectionId = Context.ConnectionId,
                PlayerIdentifier = userId,
                GameInstanceIdentifier = canReconnect ? gameInstance.Identifier : null,
            }, Context.ConnectionId);

            return new CheckGameInstanceResponse
            {
                GameId = gameInstance.Identifier,
                GameState = gameInstance.State,
                PlayerCount = gameInstance.PlayerInformation.Players.Count,
                MaxPlayerCount = gameInstance.PlayerInformation.MaxPlayers,
                PersonalIdentifier = userId,
                CanReconnect = canReconnect
            };
        }

        [HubMethodName("Join")]
        public async Task<JoinGameInstanceResponse> JoinGameInstance(string gameId, string username)
        {
            // Get initialized game instance data, if no game instance was found either no game with given Id has been initialized or the session has expired
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);
            var playerIdentifier = _playerConnectionCache.Retrieve(Context.ConnectionId).PlayerIdentifier;
            try
            {
                gameInstance.PlayerInformation.AddPlayer(playerIdentifier, username);

            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Could not add user, either the game is full or user has already joined game instance", GameHubExceptionCode.UserAddFailed);
            }

            // subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).SendAsync("UserJoined", username, playerIdentifier);

            // add connection instance to the connections cache for reference when the context goes out of scope (e.g. connection disconnects)
            // update the playerConnection to reference the connected game instance
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            playerConnection.GameInstanceIdentifier = gameInstance.Identifier;

            return new JoinGameInstanceResponse
            {
                Players = gameInstance.PlayerInformation.Players
            };
        }

        [HubMethodName("Reconnect")]
        public async Task<ReJoinGameInstanceResponse> JoinGameInstance()
        {
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection.GameInstanceIdentifier is null) throw GameHubException.Create("Could not find a game instance identifier for reconnecting user", GameHubExceptionCode.IdentifierNotFound);
            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier) ?? throw GameHubException.CreateIdentifierError(playerConnection.GameInstanceIdentifier);

            try
            {
                gameInstance.PlayerInformation.ReconnectPlayer(playerConnection.PlayerIdentifier);
            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Could not reconnect user, active players might have explicitely removed this user", GameHubExceptionCode.UserEditFailed);
            }

            var reconnectingPlayer = gameInstance.PlayerInformation.Players.GetValueOrDefault(playerConnection.PlayerIdentifier) ?? throw GameHubException.CreateIdentifierError("No player information was found for reconnecting player identifier");

            // subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameInstance.Identifier);
            await Clients.OthersInGroup(gameInstance.Identifier).SendAsync("UserReconnected", playerConnection.PlayerIdentifier);

            return new ReJoinGameInstanceResponse
            {
                Players = gameInstance.PlayerInformation.Players,
                Username = reconnectingPlayer.username,
            };
        }

        [HubMethodName("Kick")]
        public async Task KickPlayerFromGameInstance(string userIdentifier)
        {
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection.GameInstanceIdentifier is null) throw GameHubException.Create("Could not find a game instance identifier for reconnecting user", GameHubExceptionCode.IdentifierNotFound);

            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier) ?? throw GameHubException.CreateIdentifierError(playerConnection.GameInstanceIdentifier);
            if (gameInstance.PlayerInformation.Players.FirstOrDefault(p => p.Key == userIdentifier).Value.isConnected) throw GameHubException.Create("Active players can not be kicked", GameHubExceptionCode.ActionNotAllowed);
            gameInstance.PlayerInformation.RemovePlayer(userIdentifier);

            await Clients.OthersInGroup(playerConnection.GameInstanceIdentifier).SendAsync("UserRemoved", userIdentifier);
        }

        [HubMethodName("Ready")]
        public async Task SetPlayerReadyState(bool readyState)
        {
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection.GameInstanceIdentifier is null) throw GameHubException.Create("Could not find a game instance identifier for reconnecting user", GameHubExceptionCode.IdentifierNotFound);
            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier) ?? throw GameHubException.CreateIdentifierError(playerConnection.GameInstanceIdentifier);
            try
            {
                gameInstance.PlayerInformation.SetPlayerReadyState(playerConnection.PlayerIdentifier, readyState);
            }
            catch (GameInstanceException)
            {
                throw GameHubException.CreateIdentifierError(playerConnection.GameInstanceIdentifier);
            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Failed to set user ready state", GameHubExceptionCode.UserEditFailed);
            }
            await Clients.OthersInGroup(playerConnection.GameInstanceIdentifier).SendAsync("UserReady", readyState, playerConnection.PlayerIdentifier);

            // If all active players are ready, start game
            if (gameInstance.PlayerInformation.Players.Where(p => p.Value.isConnected).All(p => p.Value.isReady))
            {
                gameInstance.State = GameState.Starting;
                var startTime = DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds();
                await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("GameStateChanged", GameState.Starting);
                await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("GameStarting", startTime);
                await Task.Delay(Convert.ToInt32(startTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())).ContinueWith(async (_) =>
                {
                    gameInstance.State = GameState.Started;
                    await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("GameStateChanged", GameState.Started);
                    GameRound gameRound = await _gameEventService.CreateGameRound(playerConnection.GameInstanceIdentifier, gameInstance.WordSet);
                    await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("StartRound", new GameRoundResponse(gameRound));
                }).Unwrap();
            }
        }
    }
}
