using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace TicketingSys.Models
{
    public class User
    {
        public string userId { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }

        public string email { get; set; }

        public bool IsAdmin { get; set; } = false;

        public List<UserDepartmentAccess> DepartmentAccesses { get; set; }


    }
}
