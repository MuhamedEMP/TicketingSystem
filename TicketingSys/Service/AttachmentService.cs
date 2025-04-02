using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.AttachmentDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Mappers;
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

        public async Task<List<TicketAttachment>> CreateAndSaveTicketAttachments(int ticketId, List<NewTicketAttachmentDto> attachmentsDto)
        {
            var attachments = attachmentsDto.Select(a => new TicketAttachment
            {
                TicketId = ticketId,
                Path = a.Path,
                FileName = a.Filename,
                ContentType = a.ContentType
            }).ToList();

            _context.TicketAttachments.AddRange(attachments);
            await _context.SaveChangesAsync();

            return attachments;
        }

        public async Task<List<ResponseAttachment>> CreateAndSaveResponseAttachments(int responseId, List<NewResponseAttachmentDto> dtoList)
        {
            var attachments = dtoList.Select(a => new ResponseAttachment
            {
                ResponseId = responseId,
                Path = a.Path,
                FileName = a.Filename,
                ContentType = a.ContentType
            }).ToList();

            _context.ResponseAttachments.AddRange(attachments);
            await _context.SaveChangesAsync();

            return attachments;
        }


        public async Task SaveResponseAttachments(List<ResponseAttachment> attachments)
        {
            _context.ResponseAttachments.AddRange(attachments);
            await _context.SaveChangesAsync();
        }
    }
}
