using System.ComponentModel.DataAnnotations;

namespace TicketingSys.Models
{
    public class TicketCategory
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

    }
}
