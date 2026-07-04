using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class GetTopicDto
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "The Description field must be a maximum length of 1000 characters.")]
        public string? Description { get; set; }
    }

    public class ActionTopicDto
    {
        [Required]
        public int LessonId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "The Description field must be a maximum length of 1000 characters.")]
        public string? Description { get; set; }
    }
}
