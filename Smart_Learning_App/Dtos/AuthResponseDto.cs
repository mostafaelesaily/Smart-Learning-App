namespace Smart_Learning_App.Dtos
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireOn { get; set; }
        public string? Message { get; set; }
    }
}
