using vocabversus_engine.Models;

namespace vocabversus_engine.Services
{
    public interface IGameEventService
    {
        ValueTask<GameRound> CreateGameRound(string gameId, Guid wordSetId);
    }
}
