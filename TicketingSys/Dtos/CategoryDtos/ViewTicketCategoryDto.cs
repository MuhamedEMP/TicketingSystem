using TicketingSys.Models;

namespace TicketingSys.Dtos.CategoryDtos
{
    public class ViewTicketCategoryDto
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

}
