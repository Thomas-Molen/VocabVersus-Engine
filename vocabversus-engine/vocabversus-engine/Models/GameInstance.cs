namespace vocabversus_engine.Models
{
    public class GameInstance
    {
        public string Identifier { get; set; }
        public int PlayerCount { get; set; }
        public Guid WordSet { get; set; }
    }
}
