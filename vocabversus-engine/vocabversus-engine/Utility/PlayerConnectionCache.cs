using Microsoft.Extensions.Caching.Memory;
using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public class PlayerConnectionCache : IPlayerConnectionCache
    {
        protected readonly IMemoryCache _memoryCache;
        protected const string _cacheKey = "user-connection";
        private const int _relativeExpirationTimeInMinutes = 30;

        public PlayerConnectionCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public PlayerConnection Register(PlayerConnection data, string connectionId)
        {
            _memoryCache.Set($"{_cacheKey}_{connectionId}", data, TimeSpan.FromMinutes(_relativeExpirationTimeInMinutes));
            return data;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">user was not found in the cache</exception>
        public PlayerConnection Retrieve(string connectionId)
        {
            if (!_memoryCache.TryGetValue($"{_cacheKey}_{connectionId}", out PlayerConnection value))
                throw new ArgumentException("player was not found in the internal cache");
            return value;
        }

        /// <inheritdoc/>
        public void Remove(string connectionId)
        {
            _memoryCache.Remove($"{_cacheKey}_{connectionId}");
        }
    }
}
