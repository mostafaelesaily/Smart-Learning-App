using Microsoft.AspNetCore.Identity;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Exceptions;

namespace Smart_Learning_App.Services.MainService
{
    public class UserService : IUserService
    {
        private readonly ICacheServicecs cacheServicecs;
        private readonly UserManager<AppUser> userManager;
        private readonly IUow uow;
        private readonly Microsoft.Extensions.Logging.ILogger<UserService> logger;

        public UserService
            (ICacheServicecs cacheServicecs,
            UserManager<AppUser> userManager
            , IUow uow,
            Microsoft.Extensions.Logging.ILogger<UserService> logger)
        {
            this.uow = uow;
            this.cacheServicecs = cacheServicecs;
            this.userManager = userManager;
            this.logger = logger;
            this.logger.LogInformation("UserService initialized");
        }
        public async Task<PaginatedResultDto<GetUserDto>> GetAllUserPagged(int PageNum, int PageSize)
        {
            var CacheKey = $"GeAlluser_{PageNum}_{PageSize}";
            var PaggedUser = await uow.User.GetAllPaged(PageNum, PageSize);
            var UserCount = await uow.User.CountAsync();
            var userDto = await cacheServicecs.GetOrSetCaheAsync
                (
                CacheKey,
                async () =>
                {
                    return PaggedUser.Select(u => new GetUserDto
                    {
                        Id = u.Id,
                        Name = u.UserName ,
                        Email = u.Email , 
                        PhoneNumber = u.PhoneNumber
                    }).ToList();
                },TimeSpan.FromMinutes(10),TimeSpan.FromHours(1));
            var Result = new PaginatedResultDto<GetUserDto> 
            {
             Data = userDto,
             PageNumber = PageNum,
             PageSize = PageSize ,
             TotalCount = UserCount ,   
            };
            return Result;
        }

        public async Task<GetUserDto> GetMyProfile(string UserId)
        {
            var user = await uow.User.GetByIdsStringAsync(UserId);
            if (user == null) throw new NotFoundException("User Not Found");

            var GetUserDto = new GetUserDto
            {
                Id = user.Id,
                Name = user.UserName, 
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };
            return GetUserDto;
        }

        public async Task<UpdateUserDto> UpdateMyProfile(UpdateUserDto userDto, string userId)
        {
           logger.LogInformation("Updating profile for user {UserId}", userId);
           var user =  await uow.User.GetByIdsStringAsync (userId);
           if (user == null) throw new NotFoundException("User Not Found");
            if (user.Email != userDto.Email)
            {
                var existuser = userManager.Users
                .FirstOrDefault(u => u.Email == userDto.Email && u.Id != userId);
                if (existuser != null)
                {
                    throw new ConflictException("Email already exists");
                }
                user.UserName = userDto.Name;
                user.Email = userDto.Email;
                user.PhoneNumber = userDto.PhoneNumber;
            }
            var Result = await userManager.UpdateAsync(user);
            if (!Result.Succeeded) { throw new BadRequestException("Update Failed"); }
            return new UpdateUserDto
            {
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber 
            };
        }

        public async Task DeleteMyAccount(string UserId)
        {
            logger.LogInformation("Deleting account for user {UserId}", UserId);
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null) { throw new NotFoundException("User Not Found"); }
            var Result = await userManager.DeleteAsync(user);
            if (!Result.Succeeded)
                throw new BadRequestException(
           string.Join(", ", Result.Errors.Select(e => e.Description))
       );
        }

        public async Task<GetUserDto> GetUserInfo(string searchKey)
        {
            var user = await uow.User.FindElmentAsync(u =>
                u.Email == searchKey ||
                u.PhoneNumber == searchKey ||
                u.UserName == searchKey);

            if (user == null)
                throw new NotFoundException("User Not Found");


            return new GetUserDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<UpdateUserDto> UpdateUserInfo(string searchKey, UpdateUserDto dto)
        {
            var user = await uow.User.FindElmentAsync(u =>
                u.Email == searchKey ||
                u.PhoneNumber == searchKey ||
                u.UserName == searchKey);

            if (user == null)
                throw new NotFoundException("User Not Found");

            var existUser = userManager.Users.FirstOrDefault(u =>
                u.Email == dto.Email && u.Id != user.Id);

            if (existUser != null)
                throw new ConflictException("Email already exists");

            user.UserName = dto.Name;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException("Update Failed");

            return new UpdateUserDto
            {
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task DeleteUserInfo(string searchKey)
        {
            var user = await uow.User.FindElmentAsync(u =>
                u.Email == searchKey ||
                u.PhoneNumber == searchKey ||
                u.UserName == searchKey);

            if (user == null)
                throw new NotFoundException("User Not Found");

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
        }
    }
}