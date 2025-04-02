using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class ResponseMapper
    {
        public static ViewResponseDto ToViewDto(this Response response)
        {
            return new ViewResponseDto
            {
                Id = response.Id,
                TicketId = response.TicketId,
                Ticket = response.Ticket.modelToViewDto(),
                UserId = response.UserId,
                UserFullName = response.User?.fullName, 
                AttachmentUrls = response.Attachments?.Select(a => a.Path).ToList() ?? new List<string>(),
                Message = response.Message,
                Status = response.Status,
                CreatedAt = response.CreatedAt
            };
        }
    }
}
