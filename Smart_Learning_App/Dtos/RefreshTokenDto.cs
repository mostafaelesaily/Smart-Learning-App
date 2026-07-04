using System.ComponentModel.DataAnnotations;

namespace Smart_Learning_App.Dtos
{
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}
