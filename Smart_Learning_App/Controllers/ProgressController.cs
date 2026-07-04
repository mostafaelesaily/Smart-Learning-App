using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Services.IService;
using System.Security.Claims;

namespace Smart_Learning_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService progressService;
        public ProgressController(IProgressService progressService)
        {
            this.progressService = progressService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> MarkAsComplete(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await progressService.markAsComplete(lessonId, userId);
            return NoContent();
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> IsLessonCompleted(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await progressService.isLessonCompleted(lessonId, userId);
            return Ok(result);
        }

        [HttpGet("course/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetCourseProgress(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await progressService.GetCourseProgressAsync(courseId, userId);
            return Ok(result);
        }
    }
}
