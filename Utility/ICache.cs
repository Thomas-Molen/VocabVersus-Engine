using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public interface ICache<T> where T : class
    {
        /// <summary>
        /// Registers <see cref="T"/> in the memorycache with given identifier
        /// </summary>
        /// <param name="data"></param>
        /// <param name="identifier">identifier used to reference the cached data</param>
        /// <returns><see cref="T"/> object that has been registerd</returns>
        T Register(T data, string identifier);

        /// <summary>
        /// Obtain the data stored at the identifier location
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>registered <see cref="T"/>, or null if nothing found</returns>
        T? Retrieve(string identifier);

        /// <summary>
        /// Remove the data stored at the identifier location
        /// </summary>
        /// <param name="identifier"></param>
        void Remove(string identifier);
    }
}