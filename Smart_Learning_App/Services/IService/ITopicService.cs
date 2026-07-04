using Smart_Learning_App.Dtos;

namespace Smart_Learning_App.Services.IService
{
    public interface ITopicService
    {
        Task<PaginatedResultDto<GetTopicDto>> GetAllTopicsPagged(int pageNum, int pageSize);
        Task<GetTopicDto?> GetTopicByname(string searchKey);
        Task<ActionTopicDto> AddTopic(ActionTopicDto dto, string userId);
        Task<ActionTopicDto> UpdateTopic(ActionTopicDto dto, int topicId, string userId);
        Task DeleteTopic(int topicId, string userId);
        Task<ActionTopicDto> AddTopicByAdmin(ActionTopicDto dto);
        Task<ActionTopicDto> UpdateTopicByAdmin(int topicId, ActionTopicDto dto);
        Task DeleteTopicByAdmin(int topicId);
    }
}