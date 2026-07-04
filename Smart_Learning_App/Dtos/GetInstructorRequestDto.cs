using Smart_Learning_App.Data.Models;

namespace Smart_Learning_App.Dtos
{
    public class GetInstructorRequestDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
