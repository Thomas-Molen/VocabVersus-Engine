using vocabversus_engine.Models.Exceptions;

namespace vocabversus_engine.Models
{
    public class RoundContainer
    {
        public List<GameRound> Rounds { get; set; } = new();
        private readonly Guid wordSetId;

        public RoundContainer(Guid wordSet)
        {
            wordSetId = wordSet;
        }

        /// <summary>
        /// Generates a new <see cref="GameRound"> to the internal container
        /// </summary>
        /// <returns>newly added <see cref="GameRound"></returns>
        public GameRound NewRound()
        {
            // TODO: Obtain the required characters from a list of possible characters based on wordSetId
            char[] requiredCharacters = new char[] { 'a', 'b', 'f' };

            GameRound newRound = new GameRound
            {
                PlayersCompleted = new(),
                WordSetId = wordSetId,
                RequiredCharacters = requiredCharacters
            };
            Rounds.Add(newRound);
            return newRound;
        }
    }
}
