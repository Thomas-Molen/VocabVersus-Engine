namespace vocabversus_engine.Hubs.GameHub.Responses
{
    public class JoinGameInstanceResponse
    {
        public Dictionary<string, string> Players { get; set; }
        public string PersonalIdentifier { get; set; }
    }
}
