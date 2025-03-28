using TicketingSys.Enums;

namespace TicketingSys.Dtos.ResponseDtos
{
    public class ViewResponseDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public List<string> AttachmentUrls { get; set; }
        public string Message { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
