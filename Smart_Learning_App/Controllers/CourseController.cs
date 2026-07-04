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
    public class CourseController : ControllerBase
    {
        private readonly ICouresService courseService;
        public CourseController(ICouresService courseService)
        {
            this.courseService = courseService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPaggedCoures(int pageNum = 1, int pageSize = 10)
        {
            var result = await courseService.GetPaggedCouresAsync(pageNum, pageSize);
            return Ok(result);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> GetCourse([FromQuery] string courseName)
        {
            var result = await courseService.GetCourse(courseName);
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddCourse(ActionCourseDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await courseService.AddCourse(dto, userId);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, ActionCourseDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await courseService.UpdateCourse(dto, id, userId);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await courseService.DeleteCourse(id, userId);

            return NoContent();
        }

        [HttpPost("Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCourseByAdmin(adminActionCourseDto dto)
        {
            var result = await courseService.AddCourseByAdmin(dto);

            return Ok(result);
        }

        [HttpPut("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourseByAdmin(int id, adminActionCourseDto dto)
        {
            var result = await courseService.UpdateCourseByAdmin(id, dto);

            return Ok(result);
        }

        [HttpDelete("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourseByAdmin(int id)
        {
            await courseService.DeleteCourseByAdmin(id);

            return NoContent();
        }
    }
}
