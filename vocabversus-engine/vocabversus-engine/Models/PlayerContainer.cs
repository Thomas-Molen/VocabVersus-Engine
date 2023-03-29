using vocabversus_engine.Models.Exceptions;

namespace vocabversus_engine.Models
{
    public class PlayerContainer
    {
        /// <summary>
        /// Maximum amount of players allowed in the container
        /// </summary>
        public int MaxPlayers { get; set; } = 4;

        /// <summary>
        /// List of all players in container, Key = playerIdentifier
        /// </summary>
        public Dictionary<string, GamePlayerRecord> Players { get; private set; } = new();

        /// <summary>
        /// Adds player to the container
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="username"></param>
        /// <exception cref="MaximumPlayerException">when container is full</exception>
        /// <exception cref="DuplicatePlayerException">when given player identifier is already in the game</exception>
        public void AddPlayer(string playerIdentifier, string username)
        {
            if (MaxPlayers == Players.Count) throw new MaximumPlayerException("player container already full");
            bool addPlayerResult = Players.TryAdd(playerIdentifier, new GamePlayerRecord { username = username});
            if (!addPlayerResult) throw new DuplicatePlayerException("player already exists in the container");
        }

        /// <summary>
        /// Removes player from the container
        /// </summary>
        /// <param name="playerIdentifier"></param>
        public void RemovePlayer(string playerIdentifier)
        {
            Players.Remove(playerIdentifier);
        }

        /// <summary>
        /// Changes player to disconnected status
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void DisconnectPlayer(string playerIdentifier)
        {
            GamePlayerRecord player = Players.GetValueOrDefault(playerIdentifier) ?? throw new MissingPlayerException("player could not be found");
            player.isConnected = false;
        }

        /// <summary>
        /// Changes player ready status
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="readyState"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void SetPlayerReadyState(string playerIdentifier, bool readyState)
        {
            GamePlayerRecord player = Players.GetValueOrDefault(playerIdentifier) ?? throw new MissingPlayerException("player could not be found");
            player.isReady = readyState;
        }
    }
}
