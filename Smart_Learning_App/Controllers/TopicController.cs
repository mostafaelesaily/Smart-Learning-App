using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Services.IService;
using System.Security.Claims;

namespace Smart_Learning_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService topicService;

        public TopicController(ITopicService topicService)
        {
            this.topicService = topicService;
        }

        [HttpGet("pagged")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTopics(int pageNum = 1, int pageSize = 10)
        {
            var result = await topicService.GetAllTopicsPagged(pageNum, pageSize);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetTopicByName([FromQuery] string searchKey)
        {
            var result = await topicService.GetTopicByname(searchKey);
            return Ok(result);
        }

        [HttpPost("instructor")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddTopic(ActionTopicDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await topicService.AddTopic(dto, userId);
            return Ok(result);
        }

        [HttpPut("instructor/{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateTopic(int id, ActionTopicDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await topicService.UpdateTopic(dto, id, userId);
            return Ok(result);
        }

        [HttpDelete("instructor/{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await topicService.DeleteTopic(id, userId);
            return NoContent();
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTopicByAdmin(ActionTopicDto dto)
        {
            var result = await topicService.AddTopicByAdmin(dto);
            return Ok(result);
        }

        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTopicByAdmin(int id, ActionTopicDto dto)
        {
            var result = await topicService.UpdateTopicByAdmin(id, dto);
            return Ok(result);
        }

        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTopicByAdmin(int id)
        {
            await topicService.DeleteTopicByAdmin(id);
            return NoContent();
        }
    }
}