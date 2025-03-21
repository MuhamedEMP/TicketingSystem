using Microsoft.AspNetCore.Identity;

namespace TicketingSys.Models
{
    public class User : IdentityUser
    {
        // chatgpt said this is required for azure AD
        public String? AzureAdObjectId { get; set; }

        public String? OauthToken { get; set; }

        public String? OauthRefreshToken { get; set; }
    }
}
