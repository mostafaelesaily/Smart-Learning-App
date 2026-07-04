using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface IEnrollmentService
    {
        Task<PaginatedResultDto<GetEnrollmentDto>> GetAllEnrollment(int PageNum , int PageSize);
        Task<GetEnrollmentDto?> GetEnrollmentById(int EnrollmentId);
        Task<ActionEnrollmentDto> EnrollAsync (string userId , int courseId);
        Task<bool> isEnrol(string userId , int courseId);
        Task<PaginatedResultDto<GetEnrollmentDto>> GetInstructorEnrollmentsAsync
            (int PageNum , int PageSize ,string InstructorId , int CourseId);
       

    }
}
