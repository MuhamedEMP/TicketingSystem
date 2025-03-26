using System.ComponentModel.DataAnnotations;

namespace TicketingSys.Dtos.AttachmentDtos
{
    public class NewTicketAttachmentDto
    {
        
        [Required]
        public string Path { get; set; }

        public string Filename { get; set; }

        public string ContentType { get; set; }
    }
}
