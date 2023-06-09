using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
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
        private readonly IWordSetService _wordSetService;
        public GameHub(IGameInstanceCache gameInstanceCache, IPlayerConnectionCache playerConnectionCache, IWordSetService wordSetService)
        {
            _gameInstanceCache = gameInstanceCache;
            _playerConnectionCache = playerConnectionCache;
            _wordSetService = wordSetService;
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
        public async ValueTask<CheckGameInstanceResponse> CheckGameInstanceAvailability(string gameId, string? userId)
        {
            // Get initialized game instance data if available
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);

            // Check if the game already contained the userId, as this means the user can reconnect
            bool canReconnect = gameInstance.PlayerInformation.Players.Any(p => p.Key == userId && !p.Value.IsConnected);

            // Add the player connection to internal cache for future reference
            // This cache is used to handle player actions when connectionId is out of scope and moves game instance referencing responsibility to the server
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
                CanReconnect = canReconnect,
                IsPasswordProtected = gameInstance.IsPasswordProtected,
            };
        }

        [HubMethodName("Join")]
        public async Task<JoinGameInstanceResponse> JoinGameInstance(string gameId, string username, string? password = null)
        {
            // Get initialized game instance data, if no game instance was found either no game with given Id has been initialized or the session has expired
            var gameInstance = _gameInstanceCache.Retrieve(gameId) ?? throw GameHubException.CreateIdentifierError(gameId);

            // If game is password protected, authenticate with password
            if (gameInstance.IsPasswordProtected) if (password == null || !gameInstance.VerifyPassword(password)) throw GameHubException.Create("Provided pasword incorrect", GameHubExceptionCode.AuthenticationFailed);

            var playerIdentifier = _playerConnectionCache.Retrieve(Context.ConnectionId).PlayerIdentifier;
            try
            {
                gameInstance.PlayerInformation.AddPlayer(playerIdentifier, username);

            }
            catch (PlayerException)
            {
                throw GameHubException.Create("Could not add user, either the game is full or user has already joined game instance", GameHubExceptionCode.UserAddFailed);
            }

            // Subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).SendAsync("UserJoined", username, playerIdentifier);

            // Add connection instance to the connections cache for reference when the context goes out of scope (e.g. connection disconnects)
            // Update the playerConnection to reference the connected game instance
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            playerConnection.GameInstanceIdentifier = gameInstance.Identifier;

            return new JoinGameInstanceResponse
            {
                Players = gameInstance.PlayerInformation.Players,
                Rounds = gameInstance.RoundInformation.Rounds.Select(r => new GameRoundResponse
                {
                    RequiredCharacters = r.RequiredCharacters,
                    IsCompletedByPlayer = r.PlayersCompleted.Any(p => p == playerConnection.PlayerIdentifier)
                }).ToList(),
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

            // Subscribe player to the game instance via group connection
            await Groups.AddToGroupAsync(Context.ConnectionId, gameInstance.Identifier);
            await Clients.OthersInGroup(gameInstance.Identifier).SendAsync("UserReconnected", playerConnection.PlayerIdentifier);

            return new ReJoinGameInstanceResponse
            {
                Players = gameInstance.PlayerInformation.Players,
                Rounds = gameInstance.RoundInformation.Rounds.Select(r => new GameRoundResponse
                {
                    RequiredCharacters = r.RequiredCharacters,
                    IsCompletedByPlayer = r.PlayersCompleted.Any(p => p == playerConnection.PlayerIdentifier)
                }).ToList(),
                Username = reconnectingPlayer.Username,
            };
        }

        [HubMethodName("Kick")]
        public async Task KickPlayerFromGameInstance(string userIdentifier)
        {
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection.GameInstanceIdentifier is null) throw GameHubException.Create("Could not find a game instance identifier for reconnecting user", GameHubExceptionCode.IdentifierNotFound);

            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier) ?? throw GameHubException.CreateIdentifierError(playerConnection.GameInstanceIdentifier);
            if (gameInstance.PlayerInformation.Players.FirstOrDefault(p => p.Key == userIdentifier).Value.IsConnected) throw GameHubException.Create("Active players can not be kicked", GameHubExceptionCode.ActionNotAllowed);
            gameInstance.PlayerInformation.RemovePlayer(userIdentifier);

            await Clients.OthersInGroup(gameInstance.Identifier).SendAsync("UserRemoved", userIdentifier);
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
            if (gameInstance.PlayerInformation.Players.Where(p => p.Value.IsConnected).All(p => p.Value.IsReady))
            {
                gameInstance.State = GameState.Starting;
                var startTime = DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeMilliseconds();
                await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("GameStateChanged", GameState.Starting);
                await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("GameStarting", startTime);
                await Task.Delay(Convert.ToInt32(startTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())).ContinueWith(async (_) =>
                {
                    await StartGameRound(gameInstance.Identifier);
                    gameInstance.State = GameState.Started;
                    await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("GameStateChanged", GameState.Started);
                }).Unwrap();
            }
        }

        private async Task StartGameRound(string gameIdentifier)
        {
            var gameInstance = _gameInstanceCache.Retrieve(gameIdentifier) ?? throw GameHubException.CreateIdentifierError(gameIdentifier);

            var gameRound = gameInstance.RoundInformation.NewRound();
            await Clients.Group(gameInstance.Identifier).SendAsync("StartRound", new GameRoundResponse
            {
                RequiredCharacters = gameRound.RequiredCharacters,
                IsCompletedByPlayer = false
            });
        }

        [HubMethodName("Submit")]
        public async Task CheckSubmittedWord(string word)
        {
            // Get player data
            var playerConnection = _playerConnectionCache.Retrieve(Context.ConnectionId);
            if (playerConnection.GameInstanceIdentifier is null) throw GameHubException.Create("Could not find a game instance identifier for player", GameHubExceptionCode.IdentifierNotFound);

            // Find game and round information
            var gameInstance = _gameInstanceCache.Retrieve(playerConnection.GameInstanceIdentifier) ?? throw GameHubException.CreateIdentifierError(playerConnection.GameInstanceIdentifier);
            var round = gameInstance.RoundInformation.Rounds.LastOrDefault() ?? throw GameHubException.Create("No round is active to submit words for", GameHubExceptionCode.ActionNotAllowed);
            // Do not allow players who have already completed a round to re-submit
            if (round.PlayersCompleted.Any(p => p == playerConnection.PlayerIdentifier)) throw GameHubException.Create("Player has already completed given round", GameHubExceptionCode.ActionNotAllowed);

            // Check if submitted word contains all required characters
            char[] wordChars = word.ToLower().ToCharArray();
            if (!round.RequiredCharacters.All(c =>
            {
                // If char is not in available chars of word, return false
                if (!wordChars.Contains(c)) return false;
                // Remove found char from available chars of word
                wordChars = wordChars.Where((v, i) => i != Array.IndexOf(wordChars, c)).ToArray();
                return true;
            }))
            {
                // If submittion does not contain all required characters, submittion will be incorrect
                await SubmittionIncorrect();
                return;
            }

            // Evaluate submitted word
            var evaluation = await _wordSetService.EvaluateWord(gameInstance.RoundInformation.WordSetId, word, gameInstance.Settings.IncorrectCharsMargin);
            if (!evaluation.IsSuccess)
            {
                // If evaluation was not a success, submittion is incorrect
                await SubmittionIncorrect();
                return;
            }

            // if user has a valid submition, add the player to the list of completed players, and give points
            // TODO: add logic for calculating and given points to user
            int pointsToGive = word.Length*5;
            int previousPlayersCompleted = round.PlayersCompleted.Count;
            pointsToGive -= previousPlayersCompleted*3;
            if (pointsToGive < 1) pointsToGive = 1;

            gameInstance.PlayerInformation.GivePlayerPoints(playerConnection.PlayerIdentifier, pointsToGive);
            await Clients.Group(gameInstance.Identifier).SendAsync("AddPoints", playerConnection.PlayerIdentifier, pointsToGive);
            round.PlayersCompleted.Add(playerConnection.PlayerIdentifier);

            // Return result to user, do this via a client call so that other pre processes such as new round logic can be done in current process
            await Clients.Caller.SendAsync("SubmitResult", new CheckWordResponse
            {
                isCorrect = true,
            });

            // If first person to complete round, start new round process
            if (previousPlayersCompleted == 0)
            {
                var endTime = DateTimeOffset.UtcNow.AddSeconds(gameInstance.Settings.RoundEndDelay).ToUnixTimeMilliseconds();
                // Send round ending indicator
                await Clients.Group(playerConnection.GameInstanceIdentifier).SendAsync("RoundEnding", endTime);
                // If no game round delay has been added, execute method imediately
                // this avoids inconsistent behavior when handling negative numbers in the Task.Delay
                if (gameInstance.Settings.RoundEndDelay == 0) await StartGameRound(gameInstance.Identifier);
                else
                {
                    await Task.Delay(Convert.ToInt32(endTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())).ContinueWith(async (_) =>
                    {
                        await StartGameRound(gameInstance.Identifier);
                    }).Unwrap();
                }
            }
        }

        private async Task SubmittionIncorrect()
        {
            await Clients.Caller.SendAsync("SubmitResult", new CheckWordResponse
            {
                isCorrect = false,
            });
        }
    }
}
