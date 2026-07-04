namespace Smart_Learning_App.Data.Models
{
    public class Progress
    {
        public int id { get; set; }

        public string UserId { get; set; }

        public int LessonId { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; } 

        public AppUser User { get; set; }

        public Lessons Lesson { get; set; }
    }
}
