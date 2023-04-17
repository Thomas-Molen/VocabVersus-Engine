using vocabversus_engine.Models;
using vocabversus_engine.Models.Responses;

namespace vocabversus_engine.Services
{
    public class WordSetService : IWordSetService
    {
        private readonly HttpClient _httpClient;

        public WordSetService(HttpClient httpCLient)
        {
            _httpClient = httpCLient;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Failed to obtain valid word set data from word set evaluator</exception>
        public async Task<WordSet> GetWordSet(Guid wordSetId)
        {
            // get word set data with ID, Name and Words list
            var wordSetResponse = await _httpClient.GetAsync($"?wordSetId={wordSetId}&fields=Id&fields=Name&fields=Words");
            if (!wordSetResponse.IsSuccessStatusCode) throw new ArgumentException($"Could not obtain word set data from evaluator with ID: {wordSetId}");
            var wordSet = await wordSetResponse.Content.ReadFromJsonAsync<GetWordSetResponse>() ?? throw new ArgumentException("Word set was obtained but value was null");

            return new WordSet
            {
                Id = wordSet.Id,
                Name = wordSet.Name,
                Words = wordSet.Words
            };
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Failed to obtain valid evaluation data from word set evaluator</exception>
        public async Task<WordEvaluationData> EvaluateWord(Guid wordSetId, string word, int incorrectMargin = 0)
        {
            var evaluateResponse = await _httpClient.GetAsync($"WordSet/evaluate?wordSetId={wordSetId}&word={word}&fuzzyChars={incorrectMargin}");
            if (!evaluateResponse.IsSuccessStatusCode) throw new ArgumentException($"Could not evaluate word against word set: {wordSetId}");
            var evaluation = await evaluateResponse.Content.ReadFromJsonAsync<EvaluateWordResponse>() ?? throw new ArgumentException("evaluation succeeded but result was null");

            return new WordEvaluationData
            {
                HasMatch = evaluation.HasMatch
            };
        }
    }
}
