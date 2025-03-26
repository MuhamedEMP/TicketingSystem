using System.ComponentModel.DataAnnotations;
using TicketingSys.Enums;
using TicketingSys.Models;
using TicketingSys.Dtos;
using TicketingSys.Dtos.AttachmentDtos;

namespace TicketingSys.Dtos.TicketDtos
{
    public class NewTicketDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public List<NewTicketAttachmentDto> Attachments { get; set; } = new List<NewTicketAttachmentDto>();

        [Required]
        public string Title { get; set; } 

        [Required]
        public TicketUrgencyEnum Urgency { get; set; } 

        public string Description { get; set; }


    }
}
