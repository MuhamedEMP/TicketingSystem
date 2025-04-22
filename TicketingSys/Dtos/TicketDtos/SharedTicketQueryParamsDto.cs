using System.Text.Json.Serialization;

namespace TicketingSys.Dtos.TicketDtos
{
    public class SharedTicketQueryParamsDto : TicketQueryParamsDto
    {
        [JsonIgnore]
        public string? UserId { get; set; }

        public string? AssignedToId { get; set; }
    }
}
