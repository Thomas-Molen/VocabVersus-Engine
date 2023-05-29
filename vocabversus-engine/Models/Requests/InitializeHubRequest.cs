namespace vocabversus_engine.Models.Requests
{
    public class InitializeHubRequest
    {
        /// <summary>
        /// Maximum amount of players allowed to concurrently join game instance
        /// </summary>
        public int MaxPlayerCount { get; set; } = 4;
        /// <summary>
        /// Word set to use during game rounds
        /// </summary>
        public Guid WordSet { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Gameplay settings
        /// </summary>
        public GameInstanceSettings Settings { get; set; } = new();
        /// <summary>
        /// Optional password protection
        /// </summary>
        public string? Password { get; set; } = null;
    }
}
