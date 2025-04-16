using Microsoft.AspNetCore.Mvc;
using TicketingSys.Dtos.DepartmentDtos;

namespace TicketingSys.Dtos.UserDtos
{
    public class ViewUserDto
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string fullName { get; set; }

        public string email { get; set; }

        public List<AccessibleDepartmentDto>? accessibleDepartmentDtos { get; set; }

        public string? userId { get; set; }

        public bool isAdmin { get; set; }   

    }
}
