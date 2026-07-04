using Microsoft.AspNetCore.Http;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Exceptions;
using Microsoft.Extensions.Logging;

namespace Smart_Learning_App.Services.MainService
{
    public class LessonService : ILessonService
    {
        ICacheServicecs CacheServicecs;
        IUow uow;
        IFileService fileService;
        private readonly ILogger<LessonService> logger;
        public LessonService(
            ICacheServicecs cacheServicecs,
            IUow uow,
            IFileService fileService,
            ILogger<LessonService> logger)
        {
            this.CacheServicecs = cacheServicecs;
            this.uow = uow;
            this.fileService = fileService;
            this.logger = logger;
            this.logger.LogInformation("LessonService initialized");
        }

        public async Task<PaginatedResultDto<GetLessonDto>> GetAllLessonsAsync(int pageNum, int pageSize)
        {
            var CacheKey = $"GetAll_{pageNum}_{pageSize}";
            var PaggedLessons = await uow.Lessons.GetAllPaged(pageNum, pageSize);
            var TotalCount = await uow.Lessons.CountAsync();
            var LessonDto = await CacheServicecs.GetOrSetCaheAsync(
                CacheKey,
                async () =>
                {
                    return PaggedLessons.Select(l => new GetLessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        CourseId = l.courseId,
                        OrderIndex = l.OrderIndex,
                    }).ToList();
                }, TimeSpan.FromMinutes(10), TimeSpan.FromHours(1)
            );

            return new PaginatedResultDto<GetLessonDto>
            {
                Data = LessonDto,
                PageNumber = pageNum,
                PageSize = pageSize,
                TotalCount = TotalCount
            };
        }

        public async Task<GetLessonDto?> GetLessonByTitle(string searchKey)
        {
            var Lesson = await uow.Lessons.FindElmentAsync(l => l.Title.Contains(searchKey));
            if (Lesson == null) { throw new NotFoundException("Lesson Not Found"); }
            var LessonDto = new GetLessonDto
            {
                Id = Lesson.Id,
                Title = Lesson.Title,
                CourseId = Lesson.courseId,
                OrderIndex = Lesson.OrderIndex,
            };
            return LessonDto;
        }

        public async Task<PaginatedResultDto<GetLessonDto>> GetCourseLessonsAsync(int courseId, int pageNum, int pageSize)
        {
            var course = await uow.Course.GetByIdAsync(courseId);
            if (course == null)
                throw new NotFoundException("Course not found");

            var courseLessons = await uow.Lessons.FindAllAsync(l => l.courseId == courseId);

            var totalCount = courseLessons.Count;

            var pagedLessons = courseLessons
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new GetLessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    OrderIndex = l.OrderIndex,
                    CourseId = l.courseId
                })
                .ToList();

            return new PaginatedResultDto<GetLessonDto>
            {
                Data = pagedLessons,
                PageNumber = pageNum,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // ================= INSTRUCTOR =================
        public async Task<ActionLessonDto> AddLessonAsync(ActionLessonDto dto, IEnumerable<IFormFile>? files, string userId)
        {
            // Minimal logging to trace instructor actions
            logger?.LogInformation("AddLessonAsync called by user {UserId} for course {CourseId}", userId, dto.CourseId);
            var course = await uow.Course.GetByIdAsync(dto.CourseId);
            if (course == null)
                throw new NotFoundException("Course not found");

            if (course.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to add lesson to this course");

            var lessons = await uow.Lessons.FindAllAsync(l => l.courseId == dto.CourseId);
            var nextOrder = lessons.Any() ? lessons.Max(l => l.OrderIndex) + 1 : 1;

            var lesson = new Lessons
            {
                Title = dto.Title,
                courseId = dto.CourseId,
                OrderIndex = nextOrder
            };

            await uow.Lessons.AddItemAsync(lesson);
            await uow.SaveChangesAsync();

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var filePath = await fileService.UploadFileAsync(file, "Lessons");
                    var lessonFile = new Files
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        FileType = file.ContentType,
                        LessonId = lesson.Id
                    };
                    await uow.Files.AddItemAsync(lessonFile);
                }
                await uow.SaveChangesAsync();
            }

            return new ActionLessonDto
            {
                Title = lesson.Title,
                CourseId = lesson.courseId,
                OrderIndex = lesson.OrderIndex
            };
        }

        public async Task<ActionLessonDto> UpdateLessonAsync(int lessonId, ActionLessonDto dto, IEnumerable<IFormFile>? files, string userId)
        {
            var lesson = await uow.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson not found");

            var currentCourse = await uow.Course.GetByIdAsync(lesson.courseId);
            if (currentCourse == null)
                throw new NotFoundException("Course not found");

            if (currentCourse.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to update this lesson");

            // If moving to another course, ensure target course exists and user owns it
            if (dto.CourseId != lesson.courseId)
            {
                var targetCourse = await uow.Course.GetByIdAsync(dto.CourseId);
                if (targetCourse == null)
                    throw new NotFoundException("Target course not found");
                if (targetCourse.InstructorId != userId)
                    throw new UnauthorizedAccessException("Not allowed to move lesson to the specified course");
            }

            lesson.Title = dto.Title;
            lesson.courseId = dto.CourseId;

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var filePath = await fileService.UploadFileAsync(file, "Lessons");
                    var lessonFile = new Files
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        FileType = file.ContentType,
                        LessonId = lesson.Id
                    };
                    await uow.Files.AddItemAsync(lessonFile);
                }
            }

            await uow.SaveChangesAsync();

            return new ActionLessonDto
            {
                Title = lesson.Title,
                CourseId = lesson.courseId,
                OrderIndex = lesson.OrderIndex
            };
        }

        public async Task DeleteLessonFileAsync(int fileId, string userId)
        {
            var file = await uow.Files.GetByIdAsync(fileId);
            if (file == null)
                throw new NotFoundException("File not found");

            var lesson = await uow.Lessons.GetByIdAsync(file.LessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson not found");

            var course = await uow.Course.GetByIdAsync(lesson.courseId);
            if (course == null)
                throw new NotFoundException("Course not found");

            if (course.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to delete this file");

            await fileService.DeleteFileAsync(file.FilePath);
            await uow.Files.DeleteItemAsync(fileId);
            await uow.SaveChangesAsync();
        }

        public async Task DeleteLessonFileByAdmin(int fileId)
        {
            var file = await uow.Files.GetByIdAsync(fileId);
            if (file == null)
                throw new NotFoundException("File not found");

            await fileService.DeleteFileAsync(file.FilePath);
            await uow.Files.DeleteItemAsync(fileId);
            await uow.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(int lessonId, string userId)
        {
            var lesson = await uow.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson not found");

            var course = await uow.Course.GetByIdAsync(lesson.courseId);
            if (course == null)
                throw new NotFoundException("Course not found");

            if (course.InstructorId != userId)
                throw new UnauthorizedAccessException("Not allowed to delete this lesson");

            var files = await uow.Files.FindAllAsync(f => f.LessonId == lessonId);
            foreach (var file in files)
            {
                await fileService.DeleteFileAsync(file.FilePath);
                await uow.Files.DeleteItemAsync(file.id);
            }

            await uow.Lessons.DeleteItemAsync(lessonId);
            await uow.SaveChangesAsync();
        }

        // ================= ADMIN =================
        public async Task<ActionLessonDto> AddLessonByAdmin(ActionLessonDto dto, IEnumerable<IFormFile>? files)
        {
            var course = await uow.Course.GetByIdAsync(dto.CourseId);
            if (course == null)
                throw new NotFoundException("Course not found");

            var lessons = await uow.Lessons.FindAllAsync(l => l.courseId == dto.CourseId);
            var nextOrder = lessons.Any() ? lessons.Max(l => l.OrderIndex) + 1 : 1;

            var lesson = new Lessons
            {
                Title = dto.Title,
                courseId = dto.CourseId,
                OrderIndex = nextOrder
            };

            await uow.Lessons.AddItemAsync(lesson);
            await uow.SaveChangesAsync();

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var filePath = await fileService.UploadFileAsync(file, "Lessons");
                    var lessonFile = new Files
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        FileType = file.ContentType,
                        LessonId = lesson.Id
                    };
                    await uow.Files.AddItemAsync(lessonFile);
                }
                await uow.SaveChangesAsync();
            }

            return new ActionLessonDto
            {
                Title = lesson.Title,
                CourseId = lesson.courseId,
                OrderIndex = lesson.OrderIndex
            };
        }

        public async Task<ActionLessonDto> UpdateLessonByAdmin(int lessonId, ActionLessonDto dto, IEnumerable<IFormFile>? files)
        {
            var lesson = await uow.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson not found");

            var course = await uow.Course.GetByIdAsync(dto.CourseId);
            if (course == null)
                throw new NotFoundException("Course not found");

            lesson.Title = dto.Title;
            lesson.courseId = dto.CourseId;

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var filePath = await fileService.UploadFileAsync(file, "Lessons");
                    var lessonFile = new Files
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        FileType = file.ContentType,
                        LessonId = lesson.Id
                    };
                    await uow.Files.AddItemAsync(lessonFile);
                }
                await uow.SaveChangesAsync();
            }

            await uow.SaveChangesAsync();

            return new ActionLessonDto
            {
                Title = lesson.Title,
                CourseId = lesson.courseId,
                OrderIndex = lesson.OrderIndex
            };
        }

        public async Task DeleteLessonByAdmin(int lessonId)
        {
            var lesson = await uow.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new NotFoundException("Lesson not found");

            var files = await uow.Files.FindAllAsync(f => f.LessonId == lessonId);
            foreach (var file in files)
            {
                await fileService.DeleteFileAsync(file.FilePath);
                await uow.Files.DeleteItemAsync(file.id);
            }

            await uow.Lessons.DeleteItemAsync(lessonId);
            await uow.SaveChangesAsync();
        }
    }
}
