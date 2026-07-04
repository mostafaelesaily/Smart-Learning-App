namespace Smart_Learning_App.Data.Models
{
    public class InstructorRequest
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public AppUser user { get; set; }
        public RequestStatus status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
