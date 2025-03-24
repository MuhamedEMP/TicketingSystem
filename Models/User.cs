using Microsoft.AspNetCore.Identity;

namespace TicketingSys.Models
{
    public class User : IdentityUser
    {
        // chatgpt said this is required for azure AD
        public string AzureAdObjectId { get; set; }

        public string OauthToken { get; set; }

        public string OauthRefreshToken { get; set; }
    }
}
