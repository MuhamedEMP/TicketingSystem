using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IAttachmentService
    {
        Task SaveAttachments(List<TicketAttachment> attachments);
    }
}
