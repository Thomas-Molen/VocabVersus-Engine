using Microsoft.Extensions.Caching.Memory;
using vocabversus_engine.Models;

namespace vocabversus_engine.Utility
{
    public class GameInstanceCache : IGameInstanceCache
    {
        protected readonly IMemoryCache _memoryCache;
        protected const string _cacheKey = "game-instance";
        private const int _relativeExpirationTimeInMinutes = 30;

        public GameInstanceCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public GameInstance Register(GameInstance data, string identifier)
        {
            _memoryCache.Set($"{_cacheKey}_{identifier}", data, TimeSpan.FromMinutes(_relativeExpirationTimeInMinutes));
            return data;
        }

        /// <inheritdoc/>
        public GameInstance? Retrieve(string identifier)
        {
            if (!_memoryCache.TryGetValue($"{_cacheKey}_{identifier}", out GameInstance value))
                return null;
            return value;
        }

        /// <inheritdoc/>
        public void Clear(string identifier)
        {
            _memoryCache.Remove($"{_cacheKey}_{identifier}");
        }

        /// <inheritdoc/>
        public string GetNewIdentifier()
        {
            Guid guidIdentifier = Guid.NewGuid();
            while (Retrieve(guidIdentifier.ToString()) is not null)
                guidIdentifier = Guid.NewGuid();
            return guidIdentifier.ToString();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">game instance could not be found</exception>
        /// <exception cref="ArgumentException">player could not be added to the game instance</exception>
        public void AddUser(string userIdentifier, string username, string gameIdentifier)
        {
            if (!_memoryCache.TryGetValue($"{_cacheKey}_{gameIdentifier}", out GameInstance game))
                throw new ArgumentException("no game instance found at identifier");
            game.PlayerInformation.AddPlayer(userIdentifier, username);
            Register(game, gameIdentifier);
        }
    }

}
