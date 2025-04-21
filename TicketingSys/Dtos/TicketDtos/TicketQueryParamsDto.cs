using TicketingSys.Enums;

namespace TicketingSys.Dtos.TicketDtos
{
    public class TicketQueryParamsDto
    {
        public TicketStatusEnum? Status { get; set; }
        public TicketUrgencyEnum? Urgency { get; set; }
        public string? AssignedToName { get; set; }
        public string? CategoryName { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Search { get; set; }
        public bool? isAssigned {  get; set; }
    }
    
}
