using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketingSys.Models
{
    [Tags("ResponseAttachments")]
    public class ResponseAttachment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Response")]
        public int ResponseId { get; set; }
        public Response Response { get; set; }


        [Required]
        public required string Path { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }


    }
}
