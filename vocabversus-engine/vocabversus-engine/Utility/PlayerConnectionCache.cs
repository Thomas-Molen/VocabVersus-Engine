using Microsoft.Extensions.Caching.Memory;
using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public class PlayerConnectionCache : IPlayerConnectionCache
    {
        protected readonly IMemoryCache _memoryCache;
        protected const string _cacheKey = "user-connection";
        private const int _relativeExpirationTimeInMinutes = 120;

        public PlayerConnectionCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public PlayerConnection Register(PlayerConnection data, string identifier)
        {
            _memoryCache.Set($"{_cacheKey}_{identifier}", data, TimeSpan.FromMinutes(_relativeExpirationTimeInMinutes));
            return data;
        }

        /// <inheritdoc/>
        public PlayerConnection? Retrieve(string identifier)
        {
            if (!_memoryCache.TryGetValue($"{_cacheKey}_{identifier}", out PlayerConnection value))
                return null;
            return value;
        }

        /// <inheritdoc/>
        public void Remove(string identifier)
        {
            _memoryCache.Remove($"{_cacheKey}_{identifier}");
        }
    }

}
