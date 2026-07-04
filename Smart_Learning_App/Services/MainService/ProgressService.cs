using Microsoft.EntityFrameworkCore;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Exceptions;
using Microsoft.Extensions.Logging;

namespace Smart_Learning_App.Services.MainService
{
     
    public class ProgressService : IProgressService
    {
        IUow uow;
        private readonly ILogger<ProgressService> logger;

        public ProgressService(IUow uow, ILogger<ProgressService> logger)
        {
            this.uow = uow;
            this.logger = logger;
            this.logger.LogInformation("ProgressService initialized");
        }

       

        public async Task markAsComplete(int lessonId, string userId)
        {
            logger.LogInformation("Marking lesson {LessonId} as complete for user {UserId}", lessonId, userId);
            var lesson = await uow.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                logger.LogWarning("Lesson {LessonId} not found when marking complete", lessonId);
                throw new NotFoundException("Lesson Not Found");
            }

            var user = await uow.User.GetByIdsStringAsync(userId);
            if (user == null)
                throw new NotFoundException("User Not Found");

            var existingProgress = await uow.Progress.FindElmentAsync(
                p => p.UserId == userId && p.LessonId == lessonId);

            if (existingProgress != null)
            {
                existingProgress.IsCompleted = true;
                existingProgress.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                var progress = new Progress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };

                await uow.Progress.AddItemAsync(progress);
            }

            await uow.SaveChangesAsync();
        }
        public async Task<bool> isLessonCompleted(int lessonId, string userId)
        {
            var lesson = await uow.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                logger.LogWarning("Lesson {LessonId} not found when checking completion", lessonId);
                throw new NotFoundException("Lesson Not Found");
            }

            var user = await uow.User.GetByIdsStringAsync(userId);
            if (user == null)
                throw new NotFoundException("User Not Found");
            var existingProgress = await uow.Progress.FindElmentAsync(
             p => p.UserId == userId &&
             p.LessonId == lessonId
             && p.IsCompleted == true
             );
            return existingProgress != null;
        }

        public async Task<CourseProgressDto> GetCourseProgressAsync(int courseId, string userId)
        {
            logger.LogInformation("Getting course progress for course {CourseId} and user {UserId}", courseId, userId);
            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
            {
                logger.LogWarning("Course {CourseId} not found when getting progress", courseId);
                throw new NotFoundException("Course Not Found");
            }

            var user = await uow.User.GetByIdsStringAsync(userId);
            if (user == null)
                throw new NotFoundException("User Not Found");

            var totalLessons = await uow.Lessons.Query()
                .Where(l => l.courseId == courseId)
                .CountAsync();

            var completedCount = await uow.Progress.Query()
                .Where(p =>
                    p.UserId == userId &&
                    p.IsCompleted &&
                    p.Lesson.courseId == courseId)
                .CountAsync();

            var percentage = totalLessons == 0
                ? 0
                : (float)completedCount / totalLessons * 100;

            return new CourseProgressDto
            {
                TotalLessons = totalLessons,
                CompletedLessons = completedCount,
                Percentage = percentage
            };
        }
    }
}
