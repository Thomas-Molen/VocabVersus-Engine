namespace vocabversus_engine.Models
{
    public class PlayerContainer
    {
        /// <summary>
        /// Maximum amount of players allowed in the container
        /// </summary>
        public int MaxPlayers { get; set; } = 4;

        private Dictionary<string, string> Players { get; set; } = new();

        /// <summary>
        /// Adds player to the container
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="username"></param>
        /// <exception cref="ArgumentException">when container is full or player is already in the container</exception>
        public void AddPlayer(string playerIdentifier, string username)
        {
            if (MaxPlayers == Players.Count) throw new ArgumentException("player container already full");
            bool addPlayerResult = Players.TryAdd(playerIdentifier, username);
            if (!addPlayerResult) throw new ArgumentException("player already exists in the container");
        }

        public int PlayerCount
        {
            get
            {
                return Players.Count;
            }
        }
    }
}
