using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface IAccountService
    {
        public Task<string> GenrateToken(AppUser appUser);
        public Task<RefreshToken> GenrateRefreshToken();
        public Task<AuthResponseDto> HandleRefreshToken(RefreshTokenDto refreshTokenDto);
        public Task<AuthResponseDto> Register(SignUpDto signUpDto);
        public Task<AuthResponseDto> Login(LoginDto loginDto);
        public Task<string> ChangePassword(ChangePasswordDtos changePasswordDto, string userId);
        public Task Logout(string userId); 


    }
}
