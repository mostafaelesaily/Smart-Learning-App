using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Services.IService;
using System.Security.Claims;

namespace Smart_Learning_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserPagged(int pageNum = 1, int pageSize = 10)
        {
            var result = await userService.GetAllUserPagged(pageNum, pageSize);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await userService.GetMyProfile(userId);
            return Ok(result);
        }

        [HttpPut("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile(UpdateUserDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await userService.UpdateMyProfile(dto, userId);
            return Ok(result);
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await userService.DeleteMyAccount(userId);
            return NoContent();
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserInfo(string searchKey)
        {
            var result = await userService.GetUserInfo(searchKey);
            return Ok(result);
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserInfo(string searchKey, UpdateUserDto dto)
        {
            var result = await userService.UpdateUserInfo(searchKey, dto);
            return Ok(result);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserInfo(string searchKey)
        {
            await userService.DeleteUserInfo(searchKey);
            return NoContent();
        }
    }
}