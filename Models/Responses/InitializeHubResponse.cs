namespace vocabversus_engine.Models.Responses
{
    public class InitializeHubResponse
    {
        /// <summary>
        /// Reference ID to initialized game instance
        /// </summary>
        public string GameId { get; set; } = Guid.Empty.ToString();
    }
}
