using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketingSys.Models
{
    [Table("TicketAttachments")]
    public class TicketAttachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }


        // path or link to attached file
        public  string Path { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }


    }
}
