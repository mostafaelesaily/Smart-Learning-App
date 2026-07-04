using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class GetUserDto 
    {
     public string Id { get; set; }
     public string Name { get; set; }
     
     public string Email { get; set; }
     
     public string PhoneNumber { get; set; }
    }
    public class UpdateUserDto 
    {
     public string Name { get; set; }

     public string Email { get; set; }

     public string PhoneNumber { get; set; }
    }
    public class SignUpDto
    {
        [Required]
        [MaxLength(255)]
        public string userName { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? phone { get; set; }

        [EmailAddress]
        [Required]
        [MaxLength(300)]
        public string emailAddress { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(255)]
        public string password { get; set; }
    }

    public class LoginDto
    {
        [EmailAddress]
        [Required]
        [MaxLength(300)]
        public string emailAddress { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(255)]
        public string password { get; set; }

    }
    public class ChangePasswordDtos
    {
        [Required]
        [MaxLength(255)]
        [MinLength(6)]

        public string oldPassword { get; set; }
        [Required]
        [MaxLength(255)]
        [MinLength(6)]
        public string newPassword { get; set; }
    }
}
