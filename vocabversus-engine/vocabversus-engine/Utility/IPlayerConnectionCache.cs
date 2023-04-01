using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public interface IPlayerConnectionCache : ICache<PlayerConnection>
    {
        /// <summary>
        /// Obtain the data stored at the identifier location
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns>registered <see cref=PlayerConnection"/></returns>
        /// <exception cref="ArgumentException">connectionId was not connected to any saved players</exception>
        new PlayerConnection Retrieve(string connectionId);
    }
}