using Microsoft.AspNetCore.Mvc;
using vocabversus_engine.Models.Requests;

namespace vocabversus_engine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HubController : ControllerBase
    {
        private readonly ILogger<HubController> _logger;

        public HubController(ILogger<HubController> logger)
        {
            _logger = logger;
        }

        [HttpPost("initialize")]
        public IActionResult InitializeHub([FromBody] InitializeHubRequest request)
        {
            var hubId = Guid.NewGuid();
            return Ok($"started hub for {request.PlayerCount} players for wordset: {request.WordSet} \n hub Guid: {hubId}");
        }
    }
}