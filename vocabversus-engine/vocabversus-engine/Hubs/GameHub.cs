using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using vocabversus_engine.Utility;

namespace vocabversus_engine.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameInstanceCache _gameInstanceCache;
        public GameHub(IGameInstanceCache gameInstanceCache)
        {
            _gameInstanceCache = gameInstanceCache;
        }
        // Trigger on connecting to the Hub
        public override async Task OnConnectedAsync()
        {
        }

        // Trigger on disconnect from the Hub
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
        }

        [HubMethodName("SendMessage")]
        public async Task SendMessage(string message)
            => await Clients.Others.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");

        [HubMethodName("Connect")]
        public async Task JoinGameGroup(string gameId)
        {
            // Get initialized game instance data
            var gameInstance = _gameInstanceCache.Retrieve(gameId);
            // If no game instance was found, either no game with given Id has been initialized or the session has expired
            if (gameInstance is null) throw new HubException($"No game instance found for given identifier: {gameId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        [HubMethodName("Join")]
        public async Task JoinGameInstance(string gameId, string username)
        {
            // Get initialized game instance data
            var gameInstance = _gameInstanceCache.Retrieve(gameId);
            // If no game instance was found, either no game with given Id has been initialized or the session has expired
            if (gameInstance is null) throw new HubException($"No game instance found for given identifier: {gameId}");
            try
            {
                _gameInstanceCache.AddUser(Context.ConnectionId, username, gameId);
            }
            catch (ArgumentException)
            {
                throw new HubException("user could not be added");
            }

            // Send message to group members
            await Clients.Group(gameId).SendAsync("UserJoined", username);
        }

        [HubMethodName("GetPlayerCount")]
        public int GetPlayerCountGameInstance(string gameId)
        {
            // Get initialized game instance data
            var gameInstance = _gameInstanceCache.Retrieve(gameId);
            if (gameInstance is null) throw new HubException($"No game instance found for given identifier: {gameId}");
            // No game instance has been found in the cache
            return gameInstance.PlayerInformation.PlayerCount;
        }
    }
}
