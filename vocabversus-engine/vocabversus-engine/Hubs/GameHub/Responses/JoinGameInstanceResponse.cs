using vocabversus_engine.Models;

namespace vocabversus_engine.Hubs.GameHub.Responses
{
    public class JoinGameInstanceResponse
    {
        public Dictionary<string, GamePlayerRecord> Players { get; set; }
        public string PersonalIdentifier { get; set; }
    }
}
