using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketingSys.Enums;

namespace TicketingSys.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        // references the user who submitted the ticket
        public required string SubmittedById { get; set; }
        public User SubmittedBy { get; set; }

        // references the user a ticket is assigned to
        public string? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        // no hard coded categories all will be saved in category table
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public TicketCategory Category { get; set; }

        // list of attached files to ticket - optional
        public List<TicketAttachment> Attachments { get; set; }

        public TicketStatusEnum Status { get; set; }

        public required string Title { get; set; }
        


    }
}
