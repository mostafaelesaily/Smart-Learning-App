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
    public class LessonController : ControllerBase
    {
        private readonly ILessonService lessonService;
        public LessonController(ILessonService lessonService)
        {
            this.lessonService = lessonService;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllLessons(int pageNum = 1, int pageSize = 10)
        {
            var result = await lessonService.GetAllLessonsAsync(pageNum, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetLessonByTitle(string searchKey)
        {
            var result = await lessonService.GetLessonByTitle(searchKey);
            return Ok(result);
        }

        [HttpGet("course/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetCourseLessons(int courseId, int pageNum = 1, int pageSize = 10)
        {
            var result = await lessonService.GetCourseLessonsAsync(courseId, pageNum, pageSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddLesson([FromForm] ActionLessonDto dto, [FromForm] IEnumerable<IFormFile>? files)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await lessonService.AddLessonAsync(dto, files, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateLesson(int id, [FromForm] ActionLessonDto dto, [FromForm] IEnumerable<IFormFile>? files)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await lessonService.UpdateLessonAsync(id, dto, files, userId);
            return Ok(result);
        }

        [HttpDelete("file/instructor/{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteLessonFile(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await lessonService.DeleteLessonFileAsync(id, userId);
            return NoContent();
        }

        [HttpDelete("file/admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLessonFileByAdmin(int id)
        {
            await lessonService.DeleteLessonFileByAdmin(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await lessonService.DeleteLessonAsync(id, userId);
            return NoContent();
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLessonByAdmin([FromForm] ActionLessonDto dto, [FromForm] IEnumerable<IFormFile>? files)
        {
            var result = await lessonService.AddLessonByAdmin(dto, files);
            return Ok(result);
        }

        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLessonByAdmin(int id, [FromForm] ActionLessonDto dto, [FromForm] IEnumerable<IFormFile>? files)
        {
            var result = await lessonService.UpdateLessonByAdmin(id, dto, files);
            return Ok(result);
        }

        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLessonByAdmin(int id)
        {
            await lessonService.DeleteLessonByAdmin(id);
            return NoContent();
        }
    }
}
