using Microsoft.AspNetCore.Identity;

namespace Smart_Learning_App.Data.Models
{
    public class AppUser  :  IdentityUser
    {
        public ICollection<Course>  Courses { get; set; } = new List<Course>();
        public ICollection<Enrollments> Enrollments { get; set; } = new List<Enrollments>();
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<InstructorRequest> instructorRequests { get; set; } = new List<InstructorRequest>();

    }
}
