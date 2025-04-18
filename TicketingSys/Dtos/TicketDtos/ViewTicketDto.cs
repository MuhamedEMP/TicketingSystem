using TicketingSys.Dtos.ResponseDtos;

namespace TicketingSys.Dtos.TicketDtos
{
    public class ViewTicketDto
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }
        public string Urgency { get; set; }

        public string SubmittedById { get; set; }
        public string SubmittedByName { get; set; }

        public string? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public string DepartmentName { get; set; }
        public string CategoryName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<string> AttachmentPaths { get; set; }

        public List<ViewResponseDto>? ViewResponses { get; set; }

        public int? responsesCount { get; set; }
    }
}
