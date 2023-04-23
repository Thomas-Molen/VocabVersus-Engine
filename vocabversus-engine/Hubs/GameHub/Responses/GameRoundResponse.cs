using vocabversus_engine.Models;

namespace vocabversus_engine.Hubs.GameHub.Responses
{
    public class GameRoundResponse
    {
        public char[] RequiredCharacters { get; set; }
        public bool IsCompletedByPlayer { get; set; }
    }
}
