using vocabversus_engine.Models;

namespace vocabversus_engine.Services
{
    public class GameEventService : IGameEventService
    {
        public ValueTask<GameRound> CreateGameRound(string gameId, Guid wordSetId)
        {
            return new ValueTask<GameRound>(new GameRound
            {
                GameId = gameId,
                WordSetId = wordSetId,
                RequiredCharacters = new char[] { 'a', 'b', 'f' }
            });
        }
    }
}
