using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class GetLessonDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }

        public int CourseId { get; set; }

        public int OrderIndex { get; set; }
    }

    public class ActionLessonDto
    {
        [Required]
        [StringLength(200, ErrorMessage = "The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int OrderIndex { get; set; }
    }
}
