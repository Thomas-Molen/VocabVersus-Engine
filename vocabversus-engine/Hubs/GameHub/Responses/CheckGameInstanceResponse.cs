using vocabversus_engine.Models;

namespace vocabversus_engine.Hubs.GameHub.Responses
{
    public class CheckGameInstanceResponse
    {
        public string GameId { get; set; }
        public GameState GameState { get; set; }
        public int PlayerCount { get; set; }
        public int MaxPlayerCount { get; set; }
        public string PersonalIdentifier { get; set; }
        public bool CanReconnect { get; set; } = false;
        public bool IsPasswordProtected { get; set; }
    }
}
