using vocabversus_engine.Models.Exceptions;

namespace vocabversus_engine.Models
{
    public class RoundContainer
    {
        /// <summary>
        /// Rounds executed within container
        /// </summary>
        public List<GameRound> Rounds { get; set; } = new();
        private readonly WordSet _wordSet;
        private readonly Random _random;
        private readonly uint _minWordChars;
        private readonly uint _maxWordChars;

        /// <summary>
        /// Word set reference used during game play rounds
        /// </summary>
        public Guid WordSetId
        {
            get
            {
                return _wordSet.Id;
            }
        }
        public RoundContainer(WordSet wordSet, uint minRequiredChars = 1, uint maxRequiredChars = 1)
        {
            _wordSet = wordSet;
            _random = new Random();
            _minWordChars = minRequiredChars;
            int longestWordChars = wordSet.Words.OrderByDescending(w => w.Length).FirstOrDefault()?.Length ?? 0;
            _maxWordChars = (longestWordChars < maxRequiredChars) ? (uint)longestWordChars : maxRequiredChars;
            if (longestWordChars < 1 || _minWordChars > _maxWordChars) throw new ArgumentException("No valid word exists in word set with given requirements");
        }

        /// <summary>
        /// Generates a new <see cref="GameRound"> to the internal container
        /// </summary>
        /// <returns>newly added <see cref="GameRound"></returns>
        public GameRound NewRound()
        {
            // Get a new random list of characters based on word set
            char[] requiredCharacters = _wordSet.GetRandomWordChars(_random.Next((int)_minWordChars, (int)_maxWordChars+1));

            GameRound newRound = new GameRound
            {
                PlayersCompleted = new(),
                WordSetId = _wordSet.Id,
                RequiredCharacters = requiredCharacters
            };
            Rounds.Add(newRound);
            return newRound;
        }
    }
}
