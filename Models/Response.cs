using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketingSys.Models
{
    public class Response
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        public int TicketId { get; set; }

        // refrences the user who sent the response not the one who submitted the ticket
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string UserId { get; set; }

        public ICollection<ResponseAttachment>? Attachments { get; set; }

        public string Message { get; set; }

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
