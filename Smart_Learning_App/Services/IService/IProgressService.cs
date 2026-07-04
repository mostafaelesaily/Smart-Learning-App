using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface IProgressService
    {
        Task markAsComplete(int lessonId, string userId);
        Task<bool> isLessonCompleted(int lessonId , string userId);
        Task<CourseProgressDto> GetCourseProgressAsync(int courseId, string userId);
    }
}
