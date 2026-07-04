using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class GetCourseDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200,ErrorMessage ="The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "The Description field must be a maximum length of 1000 characters.")]
        public string Description { get; set; }

        public string InstructorId { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
    
    public class ActionCourseDto
    {
        [Required]
        [StringLength(200, ErrorMessage = "The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "The Description field must be a maximum length of 1000 characters.")]
        public string Description { get; set; }
    }

    public class adminActionCourseDto
    {
        [Required]
        [StringLength(200, ErrorMessage = "The Title field must be a maximum length of 200 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "The Description field must be a maximum length of 1000 characters.")]
        public string Description { get; set; }

        public string InstructorId { get; set; }
    }
}
