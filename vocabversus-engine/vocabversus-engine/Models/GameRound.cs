namespace vocabversus_engine.Models
{
    public class GameRound
    {
        public string GameId { get; set; }
        public Guid WordSetId { get; set; }
        public char[] RequiredCharacters { get; set; }
    }
}
