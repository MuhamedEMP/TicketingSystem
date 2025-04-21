using TicketingSys.Enums;

namespace TicketingSys.Dtos.ResponseDtos
{
    public class ResponseQueryParamsDto
    {
        public string? Search { get; set; }          
        public TicketStatusEnum? Status { get; set; }     
        public string? CategoryName { get; set; }           
        public string? DepartmentName { get; set; }        
        public string? AssignedToName { get; set; }         
        public DateTime? FromDate { get; set; }             
        public DateTime? ToDate { get; set; }
        public bool? hasAttachments { get; set; }
    }
}
