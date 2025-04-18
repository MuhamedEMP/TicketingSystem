using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class ResponseMapper
    {
        public static ViewResponseDto ToViewDto(this Response response)
        {
            // include the ticket in dto by default
            return response.ToViewDto(includeTicket: true);
        }

        // use includeTicket=false when including ViewResponseDto in ViewTicketDto
        public static ViewResponseDto ToViewDto(this Response response, bool includeTicket)
        {
            var dto = new ViewResponseDto
            {
                Id = response.Id,
                TicketId = response.TicketId,
                UserId = response.UserId,
                UserFullName = response.User?.fullName,
                AttachmentUrls = response.Attachments?.Select(a => a.Path).ToList() ?? new List<string>(),
                Message = response.Message,
                Status = response.Status.ToString(),
                CreatedAt = response.CreatedAt
            };

            if (includeTicket && response.Ticket != null)
            {
                dto.Ticket = response.Ticket.modelToViewDto();
            }

            return dto;
        }
    }
}
