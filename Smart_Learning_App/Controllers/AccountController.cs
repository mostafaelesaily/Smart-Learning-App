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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(SignUpDto signUp)
        {
            var Result = await accountService.Register(signUp);
            return Ok(Result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var Result = await accountService.Login(login);
            return Ok(Result);
        }
        [HttpPost("[action]")]

        public async Task<IActionResult> HandleRefreshToken(RefreshTokenDto refreshToken)
        {
         var Result = await accountService.HandleRefreshToken(refreshToken);
            return Ok(Result);
        }
        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDtos changePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Result = await accountService.ChangePassword(changePasswordDto, userId);
            return Ok(Result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await accountService.Logout(userId);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
