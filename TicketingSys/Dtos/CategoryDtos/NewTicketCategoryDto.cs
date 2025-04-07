using System.ComponentModel.DataAnnotations;

namespace TicketingSys.Dtos.CategoryDtos
{
    public class NewTicketCategoryDto
    {
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
