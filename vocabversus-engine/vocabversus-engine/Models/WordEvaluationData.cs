namespace vocabversus_engine.Models
{
    public class WordEvaluationData
    {
        /// <summary>
        /// If word was found within word set
        /// </summary>
        public bool HasMatch { get; set; } = false;

        /// <summary>
        /// if all word evaluation options have a positive result
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return HasMatch;
            }
        }
    }
}
