using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class GetEnrollmentDto
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string UserId { get; set; }

        public DateTime EnrolledAt { get; set; }
    }

    public class ActionEnrollmentDto
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
