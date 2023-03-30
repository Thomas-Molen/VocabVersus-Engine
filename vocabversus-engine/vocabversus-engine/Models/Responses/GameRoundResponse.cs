namespace vocabversus_engine.Models.Responses
{
    public class GameRoundResponse
    {
        public char[] RequiredCharacters { get; set; }
        public GameRoundResponse(GameRound gameRound)
        {
            RequiredCharacters = gameRound.RequiredCharacters;
        }
    }
}
