using Microsoft.AspNetCore.Mvc;
using vocabversus_engine.Models;
using vocabversus_engine.Models.Requests;
using vocabversus_engine.Models.Responses;
using vocabversus_engine.Services;
using vocabversus_engine.Utility;

namespace vocabversus_engine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HubController : ControllerBase
    {
        private readonly ILogger<HubController> _logger;
        private readonly IGameInstanceCache _gameInstanceCache;
        private readonly IWordSetService _wordSetService;

        public HubController(ILogger<HubController> logger, IGameInstanceCache gameInstanceCache, IWordSetService wordSetService)
        {
            _logger = logger;
            _gameInstanceCache = gameInstanceCache;
            _wordSetService = wordSetService;
        }

        /// <summary>
        /// Creates a Game instance with supplied game settings, ready to be joined via SignalR
        /// </summary>
        /// <param name="request">game settings</param>
        /// <returns>identifier used to reference game instance</returns>
        [HttpPost("initialize")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitializeHub([FromBody] InitializeHubRequest request)
        {
            try
            {
                WordSet wordSet;
                try
                {
                    // Get word set info
                    wordSet = await _wordSetService.GetWordSet(request.WordSet);
                }
                catch (ArgumentException)
                {
                    // Wordset was not found
                    return NotFound($"No word set found with ID: {request.WordSet}");
                }

                // Create game instance to register in cache, for reference in SignalR Hubs
                string identifier = _gameInstanceCache.GetNewIdentifier();
                GameInstance gameInstance = new GameInstance(identifier, request.Settings, wordSet, request.MaxPlayerCount);
                _gameInstanceCache.Register(gameInstance, identifier);

                return Ok(new InitializeHubResponse
                {
                    GameId = identifier
                });
            }
            catch (Exception)
            {
                return BadRequest("Failed to initialize game instance");
            }
        }
    }
}