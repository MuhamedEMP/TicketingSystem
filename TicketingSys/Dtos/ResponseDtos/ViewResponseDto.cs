using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Enums;
using TicketingSys.Models;

namespace TicketingSys.Dtos.ResponseDtos
{
    public class ViewResponseDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public ViewTicketDto Ticket { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public List<string> AttachmentUrls { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } // converted from enum to string in mapper
        public DateTime CreatedAt { get; set; }
    }
}
