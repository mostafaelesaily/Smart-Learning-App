namespace Smart_Learning_App.Data.Models
{
    public class Course
    {
        public int Id {  get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string InstructorId { get; set; }

        public AppUser Instructor { get; set; }

        public ICollection<Enrollments> Enrollments { get; set; } = new List<Enrollments>();

        public ICollection<Lessons> Lessons { get; set; } = new List<Lessons>();

    }
}
