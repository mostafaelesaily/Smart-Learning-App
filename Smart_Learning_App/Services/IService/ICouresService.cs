using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface ICouresService
    {
        public Task<PaginatedResultDto<GetCourseDto>> GetPaggedCouresAsync(int PageNum , int PageSize);
        public Task<GetCourseDto?> GetCourse(string courseName);
        public Task<ActionCourseDto> AddCourse(ActionCourseDto dto, string userId);
        public Task<ActionCourseDto> UpdateCourse(ActionCourseDto dto, int courseId, string userId);
        public Task DeleteCourse(int courseId, string userId);
        public Task<adminActionCourseDto> AddCourseByAdmin(adminActionCourseDto dto);
        public Task<adminActionCourseDto> UpdateCourseByAdmin(int courseId, adminActionCourseDto dto);
        public Task DeleteCourseByAdmin(int courseId);

    }
}
