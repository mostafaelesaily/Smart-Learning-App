using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class GetProgressDto
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int LessonId { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class CourseProgressDto
    {
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public float Percentage { get; set; }
    }
}
