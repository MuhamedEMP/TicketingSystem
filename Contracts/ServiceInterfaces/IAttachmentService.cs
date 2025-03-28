using TicketingSys.Dtos.AttachmentDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IAttachmentService
    {
        Task SaveResponseAttachments(List<ResponseAttachment> attachments);

        Task<List<TicketAttachment>> CreateAndSaveTicketAttachments(int ticketId, List<NewTicketAttachmentDto> attachmentsDto);

        Task<List<ResponseAttachment>> CreateAndSaveResponseAttachments(int responseId, List<NewResponseAttachmentDto> dtoList);
    }
}
