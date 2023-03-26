using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public interface IGameInstanceCache : ICache<GameInstance>
    {
        /// <summary>
        /// Adds user data to a <see cref="GameInstance">
        /// </summary>
        /// <param name="userIdentifier">unique value to reference user</param>
        /// <param name="username">display name for the user</param>
        /// <param name="gameIdentifier">identifier for cache entry to add user data to</param>
        /// <returns></returns>
        void UserJoined(string userIdentifier, string username, string gameIdentifier);

        /// <summary>
        /// Changes user status to disconnected, allowing user to reconnect
        /// </summary>
        /// <param name="userIdentifier">unique value to reference user</param>
        /// <param name="gameIdentifier">identifier for cache entry to add user data to</param>
        /// <returns></returns>
        void UserDisconnected(string userIdentifier, string gameIdentifier);

        /// <summary>
        /// Creates a new unique identifier, that is not connected to any entry
        /// </summary>
        /// <returns><see cref="Guid"/> as string, used as key in the cache</returns>
        string GetNewIdentifier();
    }
}