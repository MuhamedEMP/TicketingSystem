using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Repository
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAttachments(List<TicketAttachment> attachments)
        {
            _context.TicketAttachments.AddRange(attachments);
            await _context.SaveChangesAsync();
        }
    }
}