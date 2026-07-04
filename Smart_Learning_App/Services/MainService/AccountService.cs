using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using Smart_Learning_App.Exceptions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Smart_Learning_App.Services.MainService
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<AccountService> logger;
        public AccountService(IConfiguration configuration , UserManager<AppUser> userManager, ILogger<AccountService> logger)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<string> GenrateToken(AppUser appUser)
        {
            var claims = new List <Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier , appUser.Id) , 
                new Claim(ClaimTypes.Name , appUser.UserName),
                new Claim(ClaimTypes.Email , appUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
            };
            var roles = await userManager.GetRolesAsync(appUser);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var SignKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: SignKey);

            var _Token = new JwtSecurityTokenHandler().WriteToken(Token);
            return _Token;
        }

        public async Task<RefreshToken> GenrateRefreshToken()
        {
            var RefreshToken = new RefreshToken()
            {
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            };
            return RefreshToken;
        }

        public async Task<AuthResponseDto> HandleRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            if (refreshTokenDto == null) throw new ArgumentNullException(nameof(refreshTokenDto));
            var user = userManager.Users.FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshTokenDto.Token));
            if (user == null) throw new UnauthorizedException("Invalid Refresh Token");

            var refreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshTokenDto.Token);
            if (refreshToken == null || refreshToken.ExpiresOn < DateTime.UtcNow || refreshToken.revokedOn != null)
            {
                throw new UnauthorizedException("Invalid or Expired Refresh Token");
            }
            refreshToken.revokedOn = DateTime.UtcNow;
            var newAccessToken = await GenrateToken(user);
            var newRefreshToken = await GenrateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);
            return new AuthResponseDto
            {
            AccessToken = newAccessToken,     
            RefreshToken = newRefreshToken.Token,
            ExpireOn = DateTime.UtcNow.AddHours(3),
            };
        }

        public async Task<AuthResponseDto> Register(SignUpDto signUpDto)
        {
            logger.LogInformation("Registering new user with email: {Email}",signUpDto.emailAddress);
            if (signUpDto == null) throw new ArgumentNullException(nameof(signUpDto));
            var user = new AppUser()
            {
                UserName = signUpDto.userName ,
                Email = signUpDto.emailAddress ,
                PhoneNumber = signUpDto.phone ,

            };

            var Result = await userManager.CreateAsync(user,signUpDto.password);
            if (!Result.Succeeded)
            {
                logger.LogWarning("User registration failed for email: {Email}. Errors: {Errors}", 
                signUpDto.emailAddress, string.Join(", ", Result.Errors.Select(e => e.Description)));
                var errors = string.Join(", ", Result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new BadRequestException(errors);
            }
            var AccessToken = await GenrateToken(user);
            var RefreshToken = await GenrateRefreshToken();
            user.RefreshTokens.Add(RefreshToken);
            await userManager.UpdateAsync(user);
            logger.LogInformation("User registered successfully with email: {Email}", signUpDto.emailAddress);
            return new AuthResponseDto
            {
                Message = "User Registered Successfully",
                AccessToken = AccessToken,
                RefreshToken = RefreshToken.Token,
                ExpireOn = DateTime.UtcNow.AddHours(3)
            };
        }
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));
            var user = await userManager.FindByEmailAsync(loginDto.emailAddress);
            logger.LogInformation("Attempting login for user with email: {Email}", loginDto.emailAddress);
            if (user == null) throw new UnauthorizedException("Invalid Email or Password");
            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.password);

            if (!isPasswordValid)
            {
                logger.LogWarning("Login failed for user with email: {Email}. " +
                    "Invalid credentials.", loginDto.emailAddress);
                throw new UnauthorizedException("Invalid Email or Password");
            }
            var Accesstoken = await GenrateToken(user);
            var RefreshToken = await GenrateRefreshToken();
            await userManager.UpdateAsync(user);
            logger.LogInformation("User logged in successfully with email: {Email}", loginDto.emailAddress);
            return new AuthResponseDto
            {
                AccessToken =  Accesstoken , 
                RefreshToken = RefreshToken.Token,
                ExpireOn = DateTime.UtcNow.AddHours(3)
            };
        }
        public async Task<string> ChangePassword(ChangePasswordDtos changePasswordDto, string userId)
        {
            logger.LogInformation("Attempting to change password for user with ID: {UserId}", userId);
            var user = userManager.Users.FirstOrDefault(x => x.Id == userId);
            logger.LogWarning("User with ID: {UserId} not found for password change.", userId);
            if (user == null) throw new NotFoundException("User Not Found");
            var result = userManager.ChangePasswordAsync(user, changePasswordDto.oldPassword,
            changePasswordDto.newPassword).Result;
            if (!result.Succeeded)
            {
                logger.LogWarning("Password change failed for user with ID: {UserId}. Errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
                var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new BadRequestException(errors);
            }
            logger.LogInformation("Password changed successfully for user with ID: {UserId}", userId);
            return "Password Changed Successfully";
        }

        public async Task Logout(string userId)
        {
            logger.LogInformation("Logging out user with ID: {UserId}", userId);

            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found for logout.", userId);
                throw new NotFoundException("User not found.");
            }

            foreach (var refreshToken in user.RefreshTokens.Where(t => t.isActive))
            {
                refreshToken.revokedOn = DateTime.UtcNow;
            }

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                logger.LogError("Failed to logout user with ID: {UserId}", userId);

                throw new Exception("Failed to logout user.");
            }

            logger.LogInformation("User with ID: {UserId} logged out successfully.");
        }
    }
}
