namespace vocabversus_engine.Models
{
    public class PlayerConnection
    {
        /// <summary>
        /// SignalR internal ID
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// Game ID connected to player
        /// </summary>
        public string? GameInstanceIdentifier { get; set; }
        /// <summary>
        /// Internal Player ID
        /// </summary>
        public string PlayerIdentifier { get; set; }
    }
}
