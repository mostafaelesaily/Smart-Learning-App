using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface IInstructorRequestService
    {
        Task CreateInstructorRequest(string userId);

        Task<PaginatedResultDto<GetInstructorRequestDto>> GetAllRequests(int PageNum , int PageSize);

        Task<GetInstructorRequestDto> GetMyRequest(string userId);

        Task UpdateRequestStatus(int requestId, RequestStatus status);
    }
}
