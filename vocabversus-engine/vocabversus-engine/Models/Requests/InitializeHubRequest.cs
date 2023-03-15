namespace vocabversus_engine.Models.Requests
{
    public class InitializeHubRequest
    {
        public int PlayerCount { get; set; }
        public Guid WordSet { get; set; }
    }
}
