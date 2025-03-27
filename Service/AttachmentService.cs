using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Service
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ApplicationDbContext _context;

        public AttachmentService(ApplicationDbContext context)
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
