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
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService enrollmentService;
        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            this.enrollmentService = enrollmentService;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEnrollment(int pageNum = 1, int pageSize = 10)
        {
            var result = await enrollmentService.GetAllEnrollment(pageNum, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            var result = await enrollmentService.GetEnrollmentById(id);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await enrollmentService.EnrollAsync(userId, courseId);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> IsEnrolled(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await enrollmentService.isEnrol(userId, courseId);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GetInstructorEnrollments(int pageNum = 1, int pageSize = 10, int courseId = 0)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await enrollmentService.GetInstructorEnrollmentsAsync(pageNum, pageSize, instructorId, courseId);
            return Ok(result);
        }
    }
}
