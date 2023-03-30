using Microsoft.AspNetCore.Mvc;
using vocabversus_engine.Models;
using vocabversus_engine.Models.Requests;
using vocabversus_engine.Models.Responses;
using vocabversus_engine.Utility;

namespace vocabversus_engine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HubController : ControllerBase
    {
        private readonly ILogger<HubController> _logger;
        private readonly IGameInstanceCache _gameInstanceCache;

        public HubController(ILogger<HubController> logger, IGameInstanceCache gameInstanceCache)
        {
            _logger = logger;
            _gameInstanceCache = gameInstanceCache;
        }

        [HttpPost("initialize")]
        public IActionResult InitializeHub([FromBody] InitializeHubRequest request)
        {
            string identifier;
            try
            {
                // TODO: Check if wordSet exists, if not return NotFound

                // Create game instance to register in cache, for reference in SignalR Hubs
                identifier = _gameInstanceCache.GetNewIdentifier();
                GameInstance gameInstance = new()
                {
                    Identifier = identifier,
                    WordSet = request.WordSet,
                    PlayerInformation = new PlayerContainer
                    {
                        MaxPlayers = request.MaxPlayerCount
                    },
                    State = GameState.Lobby
                };
                _gameInstanceCache.Register(gameInstance, identifier);
            }
            catch (Exception)
            {
                return BadRequest("Failed to initialize game instance");
            }
            
            return Ok(new InitializeHubResponse
            {
                GameId = identifier
            });
        }
    }
}