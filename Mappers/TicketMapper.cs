using System.Runtime.CompilerServices;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class TicketMapper
    {
        // maps to a model without ticketAttachments because that table needs 
        // Ticket Id, they will be mapped to the Ticket in AttachmentMapper
        public static Ticket NewDtoToModel(this NewTicketDto dto, string createdById)
        {
            return new Ticket
            {
                SubmittedById = createdById,
                CategoryId = dto.CategoryId,
                DepartmentId = dto.DepartmentId,
                Status = Enums.TicketStatusEnum.Open,
                Title = dto.Title,
                Urgency = dto.Urgency,
                CreatedAt = DateTime.UtcNow,
                Description = dto.Description,
                Attachments = new List<TicketAttachment>()
            };
        }
    }
}
