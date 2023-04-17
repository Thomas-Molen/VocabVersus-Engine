namespace vocabversus_engine.Models.Responses
{
    public class GetWordSetResponse
    {
        /// <summary>
        /// Id of word set
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Name of word set
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Words contained within word set
        /// </summary>
        public IEnumerable<string> Words { get; set; } = Array.Empty<string>();
    }
}
