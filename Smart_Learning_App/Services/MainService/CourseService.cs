using Microsoft.Extensions.Logging;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Exceptions;

namespace Smart_Learning_App.Services.MainService
{
    public class CourseService : ICouresService
    {
        public ICacheServicecs cacheServicecs;
        public IUow uow;
        private readonly ILogger<CourseService> logger;

        public CourseService
        (
            ICacheServicecs cacheServicecs,
            IUow uow,
            ILogger<CourseService> logger
        )
        {
            this.uow = uow;
            this.cacheServicecs = cacheServicecs;
            this.logger = logger;
            this.logger.LogInformation("CourseService created");
        }

        public async Task<PaginatedResultDto<GetCourseDto>> GetPaggedCouresAsync(int PageNum, int PageSize)
        {
            logger.LogInformation("Fetching paged courses: page {PageNum}, size {PageSize}", PageNum, PageSize);
            var CacheKey = $"GetAll_{PageNum}_{PageSize}";
            var PaggedCourse = await uow.Course.GetAllPaged(PageNum, PageSize);
            var TotalCount = await uow.Course.CountAsync();
            var CourseDto = await cacheServicecs.GetOrSetCaheAsync
                (
                 CacheKey,
                 async () =>
                 {
                     return PaggedCourse.Select(c => new GetCourseDto
                     {
                         Id = c.Id,
                         Title = c.Title,
                         Description = c.Description,
                         InstructorId = c.InstructorId,
                         CreatedAt = c.CreatedAt,
                     }).ToList();
                 }, TimeSpan.FromMinutes(10), TimeSpan.FromHours(1)
                );
            return new PaginatedResultDto<GetCourseDto>
            {
             Data = CourseDto,
             PageNumber = PageNum,
             PageSize = PageSize,
             TotalCount = TotalCount
            };
        }

        public async Task<GetCourseDto?> GetCourse(string courseName)
        {
            var Course = await uow.Course.FindElmentAsync(c => c.Title.Contains(courseName));
            logger.LogInformation("Fetching course by name: {CourseName}", courseName);
            if (Course == null) { throw new NotFoundException("Course Not Found"); }
            var CourseDto = new GetCourseDto
            {
                Id = Course.Id,
                Title = Course.Title,
                Description = Course.Description,
                InstructorId = Course.InstructorId,
                CreatedAt = Course.CreatedAt,
            };
            return CourseDto;
        }


        public async Task<ActionCourseDto> AddCourse(ActionCourseDto courseDto, string userId)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                InstructorId = userId
            };

            await uow.Course.AddItemAsync(course);
            await uow.SaveChangesAsync();

            return new ActionCourseDto
            {
                Title = course.Title,
                Description = course.Description,
            };
        }

        public async Task<ActionCourseDto> UpdateCourse(ActionCourseDto actionCourse, int courseId, string userId)
        {
            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");
            

            if (course.InstructorId != userId)
                throw new ForbiddenException("You are not allowed to update this course");

            course.Title = actionCourse.Title;
            course.Description = actionCourse.Description;

            await uow.SaveChangesAsync();

            return new ActionCourseDto
            {
                Title = course.Title,
                Description = course.Description,
            };
        }

        public async Task DeleteCourse(int courseId, string userId)
        {
            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");

            if (course.InstructorId != userId)
                throw new ForbiddenException("You are not allowed to delete this course");

            await uow.Course.DeleteItemAsync(courseId);
            await uow.SaveChangesAsync();
        }

        public async Task<adminActionCourseDto> AddCourseByAdmin(adminActionCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                InstructorId = dto.InstructorId
            };

            await uow.Course.AddItemAsync(course);
            await uow.SaveChangesAsync();

            return new adminActionCourseDto
            {
                Title = course.Title,
                Description = course.Description,
                InstructorId = course.InstructorId
            };
        }

        public async Task<adminActionCourseDto> UpdateCourseByAdmin(int courseId, adminActionCourseDto dto)
        {
            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.InstructorId = dto.InstructorId;

            await uow.SaveChangesAsync();

            return new adminActionCourseDto
            {
                Title = course.Title,
                Description = course.Description,
                InstructorId = course.InstructorId
            };
        }

        public async Task DeleteCourseByAdmin(int courseId)
        {

            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException("Course Not Found");

            await uow.Course.DeleteItemAsync(courseId);
            await uow.SaveChangesAsync();
        }
    }
}
