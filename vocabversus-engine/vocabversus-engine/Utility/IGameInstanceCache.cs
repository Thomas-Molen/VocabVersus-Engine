using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public interface IGameInstanceCache
    {
        /// <summary>
        /// Registers <see cref="GameInstance"/> in the memorycache with given identifier
        /// </summary>
        /// <param name="data"></param>
        /// <param name="identifier">identifier used to reference the cached data</param>
        /// <returns><see cref="GameInstance"/> object that has been registerd</returns>
        GameInstance Register(GameInstance data, string identifier);

        /// <summary>
        /// Adds user data to a <see cref="GameInstance">
        /// </summary>
        /// <param name="userIdentifier">unique value to reference user</param>
        /// <param name="username">display name for the user</param>
        /// <param name="gameIdentifier">identifier for cache entry to add user data to</param>
        /// <returns><see cref="GameInstance"> the user data has been added to</returns>
        void AddUser(string userIdentifier, string username, string gameIdentifier);

        /// <summary>
        /// Obtain the data stored at the identifier location
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>registered <see cref="GameInstance"/>, or null if nothing found</returns>
        GameInstance? Retrieve(string identifier);

        /// <summary>
        /// Remove the data stored at the identifier location
        /// </summary>
        /// <param name="identifier"></param>
        void Clear(string identifier);

        /// <summary>
        /// Creates a new unique identifier, that is not connected to any entry
        /// </summary>
        /// <returns><see cref="Guid"/> as string, used as key in the cache</returns>
        string GetNewIdentifier();
    }
}