using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Exceptions;
using Microsoft.Extensions.Logging;

namespace Smart_Learning_App.Services.MainService
{
    public class TopicService : ITopicService
    {
        private readonly ICacheServicecs cacheServicecs;
        private readonly IUow uow;
        private readonly ILogger<TopicService> logger;

        public TopicService(ICacheServicecs cacheServicecs, IUow uow, ILogger<TopicService> logger)
        {
            this.uow = uow;
            this.cacheServicecs = cacheServicecs;
            this.logger = logger;
            this.logger.LogInformation("TopicService initialized");
        }

        // ================= PAGINATION =================
        public async Task<PaginatedResultDto<GetTopicDto>> GetAllTopicsPagged(int pageNum, int pageSize)
        {
            var cacheKey = $"topics_{pageNum}_{pageSize}";

            var data = await cacheServicecs.GetOrSetCaheAsync(cacheKey, async () =>
            {
                var topics = await uow.Topics.GetAllPaged(pageNum, pageSize);

                return topics.Select(t => new GetTopicDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    LessonId = t.LessonId
                }).ToList();
            }, TimeSpan.FromMinutes(10), TimeSpan.FromHours(1));

            var totalCount = await uow.Topics.CountAsync();

            return new PaginatedResultDto<GetTopicDto>
            {
                Data = data,
                PageNumber = pageNum,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // ================= GET BY ID =================
        public async Task<GetTopicDto?> GetTopicByname(string searchKey)
        {
            var topic = await uow.Topics.FindElmentAsync(t => t.Title.Contains(searchKey));
            if (topic == null)
                throw new NotFoundException("Topic Not Found");

            return new GetTopicDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                LessonId = topic.LessonId
            };
        }

        // ================= INSTRUCTOR =================
        public async Task<ActionTopicDto> AddTopic(ActionTopicDto dto, string userId)
        {
            var lesson = await uow.Lessons.GetByIdAsync(dto.LessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson Not Found");

            var course = await uow.Course.GetByIdAsync(lesson.courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");

            if (course.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to add topic to this course");

            var topic = new Topics
            {
                Title = dto.Title,
                Description = dto.Description,
                LessonId = dto.LessonId
            };

            await uow.Topics.AddItemAsync(topic);
            await uow.SaveChangesAsync();

            return new ActionTopicDto
            {
                Title = topic.Title,
                Description = topic.Description,
                LessonId = topic.LessonId
            };
        }

        public async Task<ActionTopicDto> UpdateTopic(ActionTopicDto dto, int topicId, string userId)
        {
            var topic = await uow.Topics.GetByIdAsync(topicId);
            if (topic == null)
                throw new NotFoundException("Topic Not Found");

            var lesson = await uow.Lessons.GetByIdAsync(topic.LessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson Not Found");

            var course = await uow.Course.GetByIdAsync(lesson.courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");

            if (course.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to update this topic");

            topic.Title = dto.Title;
            topic.Description = dto.Description;

            await uow.SaveChangesAsync();

            return new ActionTopicDto
            {
                Title = topic.Title,
                Description = topic.Description,
                LessonId = topic.LessonId
            };
        }

        public async Task DeleteTopic(int topicId, string userId)
        {
            var topic = await uow.Topics.GetByIdAsync(topicId);
            if (topic == null)
                throw new NotFoundException("Topic Not Found");

            var lesson = await uow.Lessons.GetByIdAsync(topic.LessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson Not Found");

            var course = await uow.Course.GetByIdAsync(lesson.courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");

            if (course.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to delete this topic");

            await uow.Topics.DeleteItemAsync(topicId);
            await uow.SaveChangesAsync();
        }

        // ================= ADMIN =================
        public async Task<ActionTopicDto> AddTopicByAdmin(ActionTopicDto dto)
        {
            var topic = new Topics
            {
                Title = dto.Title,
                Description = dto.Description,
                LessonId = dto.LessonId
            };

            await uow.Topics.AddItemAsync(topic);
            await uow.SaveChangesAsync();

            return new ActionTopicDto
            {
                Title = topic.Title,
                Description = topic.Description,
                LessonId = topic.LessonId
            };
        }

        public async Task<ActionTopicDto> UpdateTopicByAdmin(int topicId, ActionTopicDto dto)
        {
            var topic = await uow.Topics.GetByIdAsync(topicId);
            if (topic == null)
                throw new NotFoundException("Topic Not Found");

            topic.Title = dto.Title;
            topic.Description = dto.Description;
            topic.LessonId = dto.LessonId;

            await uow.SaveChangesAsync();

            return new ActionTopicDto
            {
                Title = topic.Title,
                Description = topic.Description,
                LessonId = topic.LessonId
            };
        }

        public async Task DeleteTopicByAdmin(int topicId)
        {
            var topic = await uow.Topics.GetByIdAsync(topicId);
            if (topic == null)
                throw new NotFoundException("Topic Not Found");

            await uow.Topics.DeleteItemAsync(topicId);
            await uow.SaveChangesAsync();
        }
    }
}