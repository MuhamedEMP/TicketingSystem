using Microsoft.AspNetCore.Mvc;

namespace TicketingSys.Dtos.UserDtos
{
    public class ViewUserDto
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }

        public string email { get; set; }

        public List<string> roles { get; set; }

    }
}
