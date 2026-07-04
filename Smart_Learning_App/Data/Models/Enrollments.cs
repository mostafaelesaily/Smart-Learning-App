namespace Smart_Learning_App.Data.Models
{
    public class Enrollments
    {
        public int id { get; set; }

        public int CourseId { get; set; } 

        public string UserId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public Course Course { get; set; }

        public AppUser User { get; set; }
    }
}
