using vocabversus_engine.Models.Exceptions;

namespace vocabversus_engine.Models
{
    public class PlayerRecord
    {
        /// <summary>
        /// Display name of user
        /// </summary>
        public string Username { get; set; } = "username";
        /// <summary>
        /// Connection status of user
        /// </summary>
        public bool IsConnected { get; set; } = true;
        /// <summary>
        /// Gameplay status of user
        /// </summary>
        public bool IsReady { get; set; } = false;
        /// <summary>
        /// Total score points of user
        /// </summary>
        public int Points { get; set; } = 0;
    }

    public class PlayerContainer
    {
        /// <summary>
        /// Maximum amount of players allowed in the container
        /// </summary>
        public int MaxPlayers { get; set; } = 4;

        /// <summary>
        /// List of all players in container, Key = playerIdentifier
        /// </summary>
        public Dictionary<string, PlayerRecord> Players { get; private set; } = new();

        public PlayerContainer(int maxPLayers)
        {
            MaxPlayers = maxPLayers;
        }

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
            bool addPlayerResult = Players.TryAdd(playerIdentifier, new PlayerRecord { Username = username});
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

        private void SetPlayerConnection(string playerIdentifier, bool isConnected)
        {
            PlayerRecord player = Players.GetValueOrDefault(playerIdentifier) ?? throw new MissingPlayerException("player could not be found");
            player.IsConnected = isConnected;
        }

        /// <summary>
        /// Changes player to disconnected status
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void DisconnectPlayer(string playerIdentifier)
        {
            SetPlayerConnection(playerIdentifier, false);
        }

        /// <summary>
        /// Changes player to connected status
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void ReconnectPlayer(string playerIdentifier)
        {
            SetPlayerConnection(playerIdentifier, true);
        }

        /// <summary>
        /// Changes player ready status
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="readyState"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void SetPlayerReadyState(string playerIdentifier, bool readyState)
        {
            PlayerRecord player = Players.GetValueOrDefault(playerIdentifier) ?? throw new MissingPlayerException("player could not be found");
            player.IsReady = readyState;
        }

        /// <summary>
        /// Gives player points
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="points"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void GivePlayerPoints(string playerIdentifier, int points)
        {
            PlayerRecord player = Players.GetValueOrDefault(playerIdentifier) ?? throw new MissingPlayerException("player could not be found");
            player.Points += points;
        }

        /// <summary>
        /// Removes player points
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="points"></param>
        /// <exception cref="MissingPlayerException">when user was not found in the game, possibly due to being explicitely removed</exception>
        public void RemovePlayerPoints(string playerIdentifier, int points)
        {
            PlayerRecord player = Players.GetValueOrDefault(playerIdentifier) ?? throw new MissingPlayerException("player could not be found");
            player.Points -= points;
        }
    }
}
