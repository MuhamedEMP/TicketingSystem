using Microsoft.AspNetCore.Identity;

namespace TicketingSys.Models
{
    public class User
    {
        public string userId { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }

        public string email { get; set; }

        public List<string> roles { get; set; } = new List<string>();


    }
}
