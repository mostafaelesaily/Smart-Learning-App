using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Dtos;
using Smart_Learning_App.Exceptions;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Services.IService;

namespace Smart_Learning_App.Services.MainService
{
    public class InstructorRequestService : IInstructorRequestService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUow uow;
        private readonly ICacheServicecs cacheServicecs;
        private readonly Microsoft.Extensions.Logging.ILogger<InstructorRequestService> logger;

        public InstructorRequestService(
            UserManager<AppUser> _userManager,
            IUow uow,
            ICacheServicecs cacheServicecs,
            Microsoft.Extensions.Logging.ILogger<InstructorRequestService> logger)
        {
            this.uow = uow;
            this._userManager = _userManager;
            this.cacheServicecs = cacheServicecs;
            this.logger = logger;
            this.logger.LogInformation("InstructorRequestService initialized");
        }

        public async Task CreateInstructorRequest(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            logger.LogInformation("Creating instructor request for user {UserId}", userId);

            if (user == null)
            {
                logger.LogWarning("Instructor request creation failed: user {UserId} not found", userId);
                throw new NotFoundException("User not found.");
            }

            if (await _userManager.IsInRoleAsync(user, "Instructor"))
                throw new BadRequestException("User is already an instructor.");

            var pendingRequest = await uow.InstructorRequest
                .FindElmentAsync(r =>
                    r.userId == userId &&
                    r.status == RequestStatus.Pending);

            if (pendingRequest != null)
                throw new BadRequestException("You already have a pending request.");

            var request = new InstructorRequest
            {
                userId = userId,
                status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await uow.InstructorRequest.AddItemAsync(request);
            await uow.SaveChangesAsync();
        }

        public async Task<PaginatedResultDto<GetInstructorRequestDto>> GetAllRequests(int PageNum, int PageSize)
        {
            var cacheKey = $"InstructorRequests_{PageNum}_{PageSize}";

            var TotalCount = await uow.InstructorRequest.CountAsync();

            var InstructorRequestDto = await cacheServicecs.GetOrSetCaheAsync(
                cacheKey,
                async () =>
                {
                    var PaggedRequest = await uow.InstructorRequest
                        .Query()
                        .Include(r => r.user)
                        .Skip((PageNum - 1) * PageSize)
                        .Take(PageSize)
                        .ToListAsync();

                    return PaggedRequest.Select(r => new GetInstructorRequestDto
                    {
                        Id = r.Id,
                        UserId = r.user.Id,
                        UserName = r.user.UserName,
                        Email = r.user.Email,
                        Status = r.status,
                        CreatedAt = r.CreatedAt
                    }).ToList();
                }, TimeSpan.FromMinutes(10), TimeSpan.FromHours(1));

            return new PaginatedResultDto<GetInstructorRequestDto>
            {
                Data = InstructorRequestDto,
                PageNumber = PageNum,
                PageSize = PageSize,
                TotalCount = TotalCount,
            };
        }

        public async Task<GetInstructorRequestDto> GetMyRequest(string userId)
        {
            var request = await uow.InstructorRequest
                .Query()
                .Include(i => i.user)
                .FirstOrDefaultAsync(i => i.userId == userId);

            if (request == null)
                throw new NotFoundException("Request not found.");

            return new GetInstructorRequestDto
            {
                Id = request.Id,
                UserId = request.userId,
                UserName = request.user.UserName!,
                Email = request.user.Email!,
                Status = request.status,
                CreatedAt = request.CreatedAt
            };
        }

        public async Task UpdateRequestStatus(int requestId, RequestStatus status)
        {
            var request = await uow.InstructorRequest.GetByIdAsync(requestId);

            if (request == null)
                throw new Exception("Request not found.");

            if (request.status != RequestStatus.Pending)
                throw new BadRequestException("Request already processed.");

            request.status = status;

            if (status == RequestStatus.Approved)
            {
                var user = await _userManager.FindByIdAsync(request.userId);

                if (user == null)
                    throw new Exception("User not found.");

                if (!await _userManager.IsInRoleAsync(user, "Instructor"))
                {
                    await _userManager.AddToRoleAsync(user, "Instructor");
                }
            }

            await uow.InstructorRequest.UpdateItemAsync(request);
            await uow.SaveChangesAsync();
        }
    }
}