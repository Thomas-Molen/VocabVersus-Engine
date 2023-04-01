using vocabversus_engine.Models;

namespace vocabversus_engine.Hubs.GameHub.Responses
{
    public class ReJoinGameInstanceResponse
    {
        public Dictionary<string, PlayerRecord> Players { get; set; }
        public string Username { get; set; }
    }
}
