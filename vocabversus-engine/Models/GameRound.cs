namespace vocabversus_engine.Models
{
    public class GameRound
    {
        /// <summary>
        /// Word set used during game round
        /// </summary>
        public Guid WordSetId { get; set; }
        /// <summary>
        /// Required characters for successfull player word submittion
        /// </summary>
        public char[] RequiredCharacters { get; set; } = Array.Empty<char>();
        /// <summary>
        /// List of players who have successfully completed game round
        /// </summary>
        public List<string> PlayersCompleted { get; set; } = new();
    }
}
