using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using vocabversus_engine.Hubs.GameHub.Responses;
using vocabversus_engine.Models.Exceptions;
using vocabversus_engine.Utility;

namespace vocabversus_engine.Hubs.GameHub
{
    public class GameHub : Hub
    {
        private readonly IGameInstanceCache _gameInstanceCache;
        private readonly UserReferenceCache _userGameReferences;
        public GameHub(IGameInstanceCache gameInstanceCache, UserReferenceCache userGameReferences)
        {
            _gameInstanceCache = gameInstanceCache;
            _userGameReferences = userGameReferences;
        }
        // Trigger on connecting to the Hub
        public override async Task OnConnectedAsync()
        {
        }

        // Trigger on disconnect from the Hub
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            List<string> gamesToNotify = _userGameReferences._userGameReferences.GetValueOrDefault(Context.ConnectionId) ?? new List<string>();
            await Clients.Groups(gamesToNotify).SendAsync("UserLeft", Context.ConnectionId);
        }

        [HubMethodName("SendMessage")]
        public async Task SendMessage(string message)
            => await Clients.Others.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");

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
                _gameInstanceCache.AddUser(personalIdentifier, username, gameId);
            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Could not add user, either the game is full or user has already joined game instance", GameHubExceptionCode.UserAddFailed);
            }

            // subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).SendAsync("UserJoined", username, Context.ConnectionId);

            // TODO: clean up this user game reference
            List<string> gamesOfUser = _userGameReferences._userGameReferences.GetValueOrDefault(Context.ConnectionId) ?? new List<string>();
            gamesOfUser.Add(gameId);
            _userGameReferences._userGameReferences[Context.ConnectionId] = gamesOfUser;

            return new JoinGameInstanceResponse
            {
                PersonalIdentifier = personalIdentifier,
                Players = gameInstance.PlayerInformation.Players
            };
        }
    }
}
