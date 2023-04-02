namespace vocabversus_engine.Models
{
    public class GameRound
    {
        public Guid WordSetId { get; set; }
        public char[] RequiredCharacters { get; set; } = Array.Empty<char>();
        public List<string> PlayersCompleted { get; set; } = new();
    }
}
