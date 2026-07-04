using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface IUserService
    {
        // Service For User : 
        public Task<PaginatedResultDto<GetUserDto>> GetAllUserPagged(int PageNum, int PageSize);
        public Task<GetUserDto> GetMyProfile(string UserId);
        public Task<UpdateUserDto> UpdateMyProfile(UpdateUserDto userDto , string userId);
        public Task DeleteMyAccount(string UserId);
        // Service For Admin : 
        public Task<GetUserDto> GetUserInfo(string searchKey);
        Task<UpdateUserDto> UpdateUserInfo(string searchKey, UpdateUserDto dto);
        Task DeleteUserInfo(string searchKey);
    }
}
