using vocabversus_engine.Models;
using vocabversus_engine.Models.Responses;

namespace vocabversus_engine.Services
{
    public class WordSetService : IWordSetService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WordSetService> _logger;

        public WordSetService(HttpClient httpCLient, ILogger<WordSetService> logger)
        {
            _httpClient = httpCLient;
            _logger = logger;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Failed to obtain valid word set data from word set evaluator</exception>
        public async Task<WordSet> GetWordSet(Guid wordSetId)
        {
            // get word set data with ID, Name and Words list
            try
            {
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
            catch (HttpRequestException)
            {
                _logger.LogWarning("Connection with word set evaluator failed when attempting to get word set data");
                throw new ArgumentException($"Could not obtain word set data from evaluator with ID: {wordSetId}");
            }
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
