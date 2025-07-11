﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;
using TicketingSys.Utils;

namespace TicketingSys.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
      

        public async Task<Ticket> addNewTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId)
        {
            // includes object references by FK
            return await _context.Tickets.Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .Include(t => t.Responses)
                    .ThenInclude(r=> r.Attachments) // include response attachments too
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.SubmittedById == userId);
        }


        public async Task<List<Ticket>?> getAllTicketByUserId(string userId)
        {
            var ticket = await _context.Tickets
            .Where(t => t.SubmittedById == userId)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.AssignedTo)
            .Include(t => t.SubmittedBy)
            .Include(t => t.Attachments)
            .Include(t => t.Responses)
            .ToListAsync();

            return ticket.SortByStatusAndUrgency();
        }
        public async Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters)
        {
            var query = _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .Include(t => t.Responses)
                .Where(t => t.SubmittedById == userId)
                .AsQueryable();

            if (filters.Status.HasValue)
                query = query.Where(t => t.Status == filters.Status.Value);

            if (filters.Urgency.HasValue)
                query = query.Where(t => t.Urgency == filters.Urgency.Value);

            if (!string.IsNullOrWhiteSpace(filters.AssignedToName))
            {
                string assignedTo = filters.AssignedToName.ToLower();
                query = query.Where(t => t.AssignedTo != null && t.AssignedTo.fullName.ToLower().Contains(assignedTo));
            }

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
            {
                string category = filters.CategoryName.ToLower();
                query = query.Where(t => t.Category != null && t.Category.Name.ToLower().Contains(category));
            }

            if (!string.IsNullOrWhiteSpace(filters.DepartmentName))
            {
                string department = filters.DepartmentName.ToLower();
                query = query.Where(t => t.Department != null && t.Department.Name.ToLower().Contains(department));
            }

            if (filters.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= filters.FromDate.Value);

            if (filters.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= filters.ToDate.Value);


            if (filters.hasResponses == true)
                query = query.Where(t=> t.Responses!=null && t.Responses.Count()>0);

            if(filters.hasResponses == false)
                query = query.Where(t => t.Responses == null || t.Responses.Count() == 0);

            //if (filters.hasResponses.HasValue)
            //{
            //    if (filters.hasResponses.Value)
            //        query = query.Where(t => (t.Responses?.Count ?? 0) > 0);
            //    else
            //        query = query.Where(t => t.Responses == null || t.Responses.Count == 0);
            //}


            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var search = filters.Search.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(search) ||
                    t.Description.ToLower().Contains(search));
            }

            var tickets = await query.ToListAsync();
            var sorted = tickets.SortByStatusAndUrgency();

            return sorted.Select(t => t.modelToViewDto(includesResponses:true)).ToList();
        }


        public async Task<List<ViewResponseDto>> getResponsesToUserTickets(string userId, ResponseQueryParamsDto query)
        {
            var responses = _context.Responses
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.SubmittedBy)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Category)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Department)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.AssignedTo)
                .Include(r => r.User)
                .Include(r => r.Attachments)
                .Where(r => r.Ticket.SubmittedById == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                responses = responses.Where(r => r.Message.ToLower().Contains(search));
            }

            if (query.Status.HasValue)
            {
                responses = responses.Where(r => r.Status == query.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.CategoryName))
            {
                var cat = query.CategoryName.ToLower();
                responses = responses.Where(r => r.Ticket.Category.Name.ToLower().Contains(cat));
            }

            if (!string.IsNullOrWhiteSpace(query.DepartmentName))
            {
                var dept = query.DepartmentName.ToLower();
                responses = responses.Where(r => r.Ticket.Department.Name.ToLower().Contains(dept));
            }

            if (!string.IsNullOrWhiteSpace(query.AssignedToName))
            {
                var assigned = query.AssignedToName.ToLower();
                responses = responses.Where(r => r.Ticket.AssignedTo != null &&
                                                 r.Ticket.AssignedTo.fullName.ToLower().Contains(assigned));
            }

            if (query.FromDate.HasValue)
            {
                responses = responses.Where(r => r.CreatedAt >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                responses = responses.Where(r => r.CreatedAt <= query.ToDate.Value);
            }


            if (query.hasAttachments.HasValue)
            {
                responses = query.hasAttachments.Value
                    ? responses.Where(r => r.Attachments.Any())
                    : responses.Where(r => !r.Attachments.Any());
            }

            var resultList = await responses.ToListAsync();
            var sorted = resultList.SortByTicketStatusAndUrgency();

            return sorted.Select(r => r.ToViewDto()).ToList();
        }



        public async Task<ViewResponseDto?> GetResponseToMyTicketById(string userId, int responseId)
        {
            var response = await _context.Responses
                .Include(r => r.Ticket)
                .Include(r => r.User)
                .Include(r => r.Attachments)
                .FirstOrDefaultAsync(r => r.Ticket.SubmittedById == userId && r.Id == responseId);

            if (response == null)
                return null;

            return response.ToViewDto();
        }
    }
}