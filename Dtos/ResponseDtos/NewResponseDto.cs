using TicketingSys.Dtos.AttachmentDtos;
using TicketingSys.Enums;

namespace TicketingSys.Dtos.ResponseDtos
{
    public class NewResponseDto
    {
        public int TicketId { get; set; }

        public List<NewResponseAttachmentDto> Attachments { get; set; } = new List<NewResponseAttachmentDto>();

        public string Message { get; set; }

        public TicketStatusEnum Status { get; set; }

    }
}
