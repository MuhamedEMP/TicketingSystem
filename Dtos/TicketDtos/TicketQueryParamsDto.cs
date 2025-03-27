using TicketingSys.Enums;

namespace TicketingSys.Dtos.TicketDtos
{
    public class TicketQueryParamsDto
    {
        public TicketStatusEnum? Status { get; set; }
        public TicketUrgencyEnum? Urgency { get; set; }
        public string? AssignedToId { get; set; }
        public int? CategoryId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Search { get; set; }
    }

}
