using vocabversus_engine.Models.Exceptions;

namespace vocabversus_engine.Models
{
    public class PlayerContainer
    {
        /// <summary>
        /// Maximum amount of players allowed in the container
        /// </summary>
        public int MaxPlayers { get; set; } = 4;

        public Dictionary<string, string> Players { get; private set; } = new();

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
            bool addPlayerResult = Players.TryAdd(playerIdentifier, username);
            if (!addPlayerResult) throw new DuplicatePlayerException("player already exists in the container");
        }
    }
}
