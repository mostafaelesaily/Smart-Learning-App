using Microsoft.AspNetCore.Http;
using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface ILessonService
    {
        Task<PaginatedResultDto<GetLessonDto>> GetAllLessonsAsync(int pageNum, int pageSize);
        Task<GetLessonDto?> GetLessonByTitle(string searchKey);
        Task<PaginatedResultDto<GetLessonDto>> GetCourseLessonsAsync(
         int courseId,
         int pageNum,
         int pageSize);
        Task<ActionLessonDto> AddLessonAsync(ActionLessonDto dto, IEnumerable<IFormFile>? files, string userId);
        Task<ActionLessonDto> UpdateLessonAsync(int lessonId, ActionLessonDto dto, IEnumerable<IFormFile>? files, string userId);
        Task DeleteLessonFileAsync(int fileId, string userId);
        Task DeleteLessonAsync(int lessonId, string userId);
        // Admin endpoints
        Task<ActionLessonDto> AddLessonByAdmin(ActionLessonDto dto, IEnumerable<IFormFile>? files);
        Task<ActionLessonDto> UpdateLessonByAdmin(int lessonId, ActionLessonDto dto, IEnumerable<IFormFile>? files);
        Task DeleteLessonByAdmin(int lessonId);
        Task DeleteLessonFileByAdmin(int fileId);
    }
}