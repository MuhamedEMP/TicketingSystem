using TicketingSys.Models;

namespace TicketingSys.Contracts.RepositoryInterfaces
{
    public interface IAttachmentRepository
    {
        Task SaveAttachments(List<TicketAttachment> attachments);
    }
}