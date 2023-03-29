namespace vocabversus_engine.Models
{
    public enum GameState
    {
        Lobby = 0,
        Starting = 1,
        Started = 2,
        Ended = 3,
    }
    public class GameInstance
    {
        public string Identifier { get; set; }
        public Guid WordSet { get; set; }
        public PlayerContainer PlayerInformation { get; set; }
        public GameState State { get; set; }
    }
}
