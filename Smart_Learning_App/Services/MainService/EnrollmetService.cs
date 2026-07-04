using Microsoft.EntityFrameworkCore;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Microsoft.Extensions.Logging;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Exceptions;

namespace Smart_Learning_App.Services.MainService
{
    public class EnrollmetService : IEnrollmentService
    {
        ICacheServicecs cacheServicecs;
        IUow uow;
        private readonly ILogger<EnrollmetService> logger;
        public EnrollmetService
            (
            ICacheServicecs cacheServicecs,
             IUow uow,
             ILogger<EnrollmetService> logger
            )
        {
            this.cacheServicecs = cacheServicecs;
            this.uow = uow;
            this.logger = logger;
            this.logger.LogInformation("EnrollmetService initialized");
        }

        public async Task<PaginatedResultDto<GetEnrollmentDto>> GetAllEnrollment(int PageNum, int PageSize)
        {
            var CacheKey = $"GetAll_{PageNum}_{PageSize}";
            var PaggedEnroll = await uow.Enrollments.GetAllPaged(PageNum, PageSize);
            var TotalCount = await uow.Enrollments.CountAsync();
            var EnrollDto = await cacheServicecs.GetOrSetCaheAsync
                (
                 CacheKey,
                 async () =>
                 {
                     return PaggedEnroll.Select(e => new GetEnrollmentDto
                     {
                         Id = e.id,
                         CourseId = e.CourseId,
                         UserId = e.UserId,
                         EnrolledAt = e.EnrolledAt,
                     }).ToList();
                 }, TimeSpan.FromMinutes(10), TimeSpan.FromHours(1)
                );
            return new PaginatedResultDto<GetEnrollmentDto>
            {
                Data = EnrollDto,
                PageNumber = PageNum,
                PageSize = PageSize,
                TotalCount = TotalCount
            };
        }

        public async Task<GetEnrollmentDto?> GetEnrollmentById(int EnrollmentId)
        {
            var enroll = await uow.Enrollments.GetByIdAsync(EnrollmentId);
            if (enroll == null) { throw new NotFoundException("User Not Found"); }
            var enrollDto = new GetEnrollmentDto
            {
                Id = enroll.id,
                CourseId = enroll.CourseId,
                UserId = enroll.UserId,
                EnrolledAt = enroll.EnrolledAt,
            };
            return enrollDto;
        }


        public async Task<ActionEnrollmentDto> EnrollAsync(string userId, int courseId)
        {
            logger.LogInformation("Attempting to enroll user {UserId} in course {CourseId}", userId, courseId);
            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
            {
                logger.LogWarning("Enroll failed. Course {CourseId} not found", courseId);
                throw new NotFoundException("Course Not Found");
            }
            var user =  await uow.User.GetByIdsStringAsync(userId);
            if (user == null) throw new NotFoundException("User Not Found");

            var existing = await uow.Enrollments.FindElmentAsync(e => e.UserId == userId
            && e.CourseId == courseId
            );
            if (existing != null) throw new ConflictException("User Already Enrolled");

            var Enrollment = new Enrollments
            {
                CourseId = courseId,
                UserId  = user.Id,
            };
            await uow.Enrollments.AddItemAsync(Enrollment);
            await uow.SaveChangesAsync();
            logger.LogInformation("User {UserId} enrolled in course {CourseId}", userId, courseId);
            return new ActionEnrollmentDto 
            {
            UserId = Enrollment.UserId,
            CourseId = Enrollment.CourseId
            };
        }

        public async Task<bool> isEnrol(string userId, int courseId)
        {
            var existing = await uow.Enrollments.FindElmentAsync
                (
                 e => e.UserId == userId && e.CourseId == courseId
                );
            return existing != null;
        }
        public async Task<PaginatedResultDto<GetEnrollmentDto>>
      GetInstructorEnrollmentsAsync(int PageNum, int PageSize, string InstructorId, int CourseId)
        {
            var cacheKey = $"InstructorEnroll_{InstructorId}_{CourseId}_{PageNum}_{PageSize}";

            var query = uow.Enrollments.Query()
                .Where(e =>
                    e.Course.InstructorId == InstructorId &&
                    e.CourseId == CourseId);

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((PageNum - 1) * PageSize)
                .Take(PageSize)
                .Select(e => new GetEnrollmentDto
                {
                    Id = e.id,
                    CourseId = e.CourseId,
                    UserId = e.UserId,
                    EnrolledAt = e.EnrolledAt
                })
                .ToListAsync();

            var result = await cacheServicecs.GetOrSetCaheAsync(
                cacheKey,
                async () => data,
                TimeSpan.FromMinutes(10), TimeSpan.FromHours(1)
            );

            return new PaginatedResultDto<GetEnrollmentDto>
            {
                Data = result,
                PageNumber = PageNum,
                PageSize = PageSize,
                TotalCount = totalCount
            };
        }
    }
}
