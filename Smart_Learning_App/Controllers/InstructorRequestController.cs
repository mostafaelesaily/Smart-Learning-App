using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Services.IService;

namespace Smart_Learning_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorRequestController : ControllerBase
    {
        private readonly IInstructorRequestService requestService;

        public InstructorRequestController(IInstructorRequestService requestService)
        {
            this.requestService = requestService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateInstructorRequest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await requestService.CreateInstructorRequest(userId!);

            return Ok("Instructor request submitted successfully.");
        }

        [Authorize]
        [HttpGet("MyRequest")]
        public async Task<IActionResult> GetMyRequest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await requestService.GetMyRequest(userId!);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllRequests(
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await requestService.GetAllRequests(pageNum, pageSize);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{requestId}")]
        public async Task<IActionResult> UpdateRequestStatus(
            int requestId,
            [FromQuery] RequestStatus status)
        {
            await requestService.UpdateRequestStatus(requestId, status);

            return Ok("Request updated successfully.");
        }
    }
}