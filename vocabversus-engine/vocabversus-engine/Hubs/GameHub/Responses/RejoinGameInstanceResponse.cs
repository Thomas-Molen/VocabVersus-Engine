using vocabversus_engine.Models;

namespace vocabversus_engine.Hubs.GameHub.Responses
{
    public class ReJoinGameInstanceResponse : JoinGameInstanceResponse
    {
        public string Username { get; set; }
    }
}
