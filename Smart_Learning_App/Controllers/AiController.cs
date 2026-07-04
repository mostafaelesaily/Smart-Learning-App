using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart_Learning_App.Integrations.Ai;

namespace Smart_Learning_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IAiService aiService;
        public AiController(IAiService aiService)
        {
            this.aiService = aiService;
        }
        [HttpPost("Gemini")]
        [Authorize]
        public async Task<IActionResult> GeminiIntegration([FromBody]string prompt) 
        {
            var Result = await aiService.GenerateResponseAsync(prompt);
            return Ok(Result);
        }
    }
}
