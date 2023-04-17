using vocabversus_engine.Models;

namespace vocabversus_engine.Services
{
    public interface IWordSetService
    {
        /// <summary>
        /// Gets word set info of wordSetId reference
        /// </summary>
        /// <param name="wordSetId">Reference to word set</param>
        /// <returns>Word set data</returns>
        public Task<WordSet> GetWordSet(Guid wordSetId);

        /// <summary>
        /// Gets evaluation results of word against word set
        /// </summary>
        /// <param name="wordSetId">Word set to evaluate against</param>
        /// <param name="word">Word to evaluate</param>
        /// <param name="incorrectMargin">Amount of characters that word can be incorrect while still producing positive evaluation</param>
        /// <returns>Evaluation results</returns>
        public Task<WordEvaluationData> EvaluateWord(Guid wordSetId, string word, int incorrectMargin = 0);
    }
}
