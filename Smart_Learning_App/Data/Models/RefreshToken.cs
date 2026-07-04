using Microsoft.EntityFrameworkCore;

namespace Smart_Learning_App.Data.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime ExpiresOn { get; set; }

        public DateTime? revokedOn { get; set; }

        public bool isExpired => DateTime.UtcNow >= ExpiresOn;

        public bool isActive => revokedOn == null && !isExpired;


    }
}
