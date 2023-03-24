namespace vocabversus_engine.Models.Requests
{
    public class InitializeHubRequest
    {
        public int MaxPlayerCount { get; set; }
        public Guid WordSet { get; set; }
    }
}
