using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public interface IGameInstanceCache : ICache<GameInstance>
    {
        /// <summary>
        /// Creates a new unique identifier, that is not connected to any entry
        /// </summary>
        /// <returns><see cref="Guid"/> as string, used as key in the cache</returns>
        string GetNewIdentifier();
    }
}