using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Dtos.DepartmentDtos;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Enums;
using TicketingSys.Mappers;
using TicketingSys.Models;
using TicketingSys.Settings;
using TicketingSys.Utils;

namespace TicketingSys.Service
{
    public class SharedService :ISharedService
    {

        private readonly ApplicationDbContext _context;

        public SharedService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response?> AddResponse(NewResponseDto dto, string userId)
        {
            var response = new Response
            {
                TicketId = dto.TicketId,
                UserId = userId,
                Message = dto.Message,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };


            await _context.Responses.AddAsync(response);

            var referencedTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == dto.TicketId);
            var departmentName = referencedTicket.Department.Name.ToLower();

            var userHasAccess = await _context.UserDepartmentAccess
                .AnyAsync(a => a.UserId == userId && a.DepartmentId == referencedTicket.DepartmentId);


            if (userHasAccess is false)
                return null;

            if (referencedTicket.Status != dto.Status)
                referencedTicket.Status = dto.Status;

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<List<Ticket>?> getAllTicketByUserId(string userId)
        {
            var tickets = await _context.Tickets
            .Where(t => t.SubmittedById == userId)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.AssignedTo)
            .Include(t => t.SubmittedBy)
            .Include(t => t.Attachments)
            .ToListAsync();

            return tickets.SortByStatusAndUrgency();
        }

        public async Task<List<ViewTicketDto>?> getAllTicketsFromMyDepartment(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.userId == userId);

            var departmentIds = await _context.UserDepartmentAccess
            .Where(a => a.UserId == userId)
            .Select(a => a.DepartmentId)
            .ToListAsync();

   
            var tickets = await _context.Tickets
           .Include(t => t.SubmittedBy)
           .Include(t => t.AssignedTo)
           .Include(t => t.Category)
           .Include(t => t.Department)
           .Include(t => t.Attachments)
           .Where(t => departmentIds.Contains(t.DepartmentId))
           .ToListAsync();

            if (tickets == null || tickets.Count == 0) return null;
            var sorted = tickets.SortByStatusAndUrgency();
            return sorted.modelToViewDtoList();
        }


        public async Task<List<ViewTicketDto>?> queryAlLTicketsFromMyDepartment(string currentUserId, SharedTicketQueryParamsDto query)
        {
            if (string.IsNullOrEmpty(currentUserId))
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId.ToLower() == currentUserId.ToLower());
            if (user == null)
                return null;

            var departmentIds = await _context.UserDepartmentAccess
            .Where(a => a.UserId == currentUserId)
            .Select(a => a.DepartmentId)
            .ToListAsync();

            var queryable = _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .AsQueryable();


            queryable = queryable.Where(t => departmentIds.Contains(t.DepartmentId));

            if (query.isAssigned.HasValue)
            {
                if (query.isAssigned.Value)
                    queryable = queryable.Where(t => t.AssignedToId != null);
                else
                    queryable = queryable.Where(t => t.AssignedToId == null);
            }

            if (!string.IsNullOrWhiteSpace(query.AssignedToName))
            {
                string name = query.AssignedToName.ToLower();
                queryable = queryable.Where(t => t.AssignedTo != null && t.AssignedTo.fullName.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(query.CategoryName))
            {
                string category = query.CategoryName.ToLower();
                queryable = queryable.Where(t => t.Category != null && t.Category.Name.ToLower().Contains(category));
            }

            if (!string.IsNullOrWhiteSpace(query.DepartmentName))
            {
                string department = query.DepartmentName.ToLower();
                queryable = queryable.Where(t => t.Department != null && t.Department.Name.ToLower().Contains(department));
            }


            if (query.Status.HasValue)
                queryable = queryable.Where(t => t.Status == query.Status.Value);

            if (query.Urgency.HasValue)
                queryable = queryable.Where(t => t.Urgency == query.Urgency.Value);

           
            if (!string.IsNullOrEmpty(query.UserId))
                queryable = queryable.Where(t => t.SubmittedById == query.UserId);


            if (query.FromDate.HasValue)
                queryable = queryable.Where(t => t.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                queryable = queryable.Where(t => t.CreatedAt <= query.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
                queryable = queryable.Where(t =>
                    t.Title.ToLower().Contains(query.Search.ToLower()) ||
                    t.Description.ToLower().Contains(query.Search.ToLower())
                );

            var tickets = await queryable.ToListAsync();
            var sorted = tickets.SortByStatusAndUrgency();
            return sorted.Any() ? sorted.modelToViewDtoList() : null;
        }

        public async Task<List<ViewResponseDto>> getResponsesSentByUser(string userId, ResponseQueryParamsDto query)
        {
            var responses = _context.Responses
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Category)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Department)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.AssignedTo)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.SubmittedBy)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Attachments)
                .Include(r => r.User)
                .Include(r => r.Attachments)
                .Where(r => r.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                responses = responses.Where(r => r.Message.ToLower().Contains(query.Search.ToLower()));
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


        // returns responses by the userId of the user who sent them
        public async Task<ViewResponseDto?> getSentResponseByUserIdAndResponseId(string userId, int responseId)
        {
            var response = await _context.Responses
            .Include(r => r.Ticket)
            .Include(r => r.User)
            .Include(r => r.Attachments)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Id == responseId);

            if (response == null)
                return null;

            return response.ToViewDto();
        }

        public async Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId)
        {
            // includes object references by FK
            return await _context.Tickets.Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.SubmittedById == userId);
        }

        public async Task<Ticket?> getTicketById(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .Include(t=> t.Responses)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return null;

            return ticket;
        }

        public async Task<ViewUserDto?> getUserById(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if (user == null) return null;

            var accessibleDepartments = await _context.UserDepartmentAccess
                .Include(ad => ad.Department)
                .Where(ad => ad.UserId == id)
                .ToListAsync();


            return user.userModelToDto(accessibleDepartments);
        }

        public async Task<List<ViewTicketDto>?> getAllTicketsFromUserByDepartment(string userId)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == userId);
            if (user is null) return null;

            if (user.IsAdmin == true)
            {
                var allTickets = await _context.Tickets
                    .Include(t => t.SubmittedBy)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.Category)
                    .Include(t => t.Department)
                    .Include(t => t.Attachments)
                    .Where(t => t.SubmittedById == userId)
                    .ToListAsync();
                var sortedadmin = allTickets.SortByStatusAndUrgency();

                return sortedadmin.modelToViewDtoList();
            }

            var departmentIds = await _context.UserDepartmentAccess
            .Where(a => a.UserId == userId)
            .Select(a => a.DepartmentId)
            .ToListAsync();

            var tickets = await _context.Tickets
                .Include(t => t.SubmittedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Department)
                .Include(t => t.Attachments)
                .Where(t => t.SubmittedById == userId && departmentIds.Contains(t.DepartmentId))
                .ToListAsync();

            var sorted = tickets.SortByStatusAndUrgency();

            if (sorted.Any()) return sorted.modelToViewDtoList();
            return null;
        }

        public async Task<ViewTicketDto?> changeTicketStatus(int ticketId, TicketStatusEnum status, string userId)
        {
            // include object references
            var ticket = await _context.Tickets
            .Include(t => t.Department)
            .Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return null;

            var userHasAccess = await _context.UserDepartmentAccess
                .AnyAsync(a => a.UserId == userId && a.DepartmentId == ticket.DepartmentId);


            if (!userHasAccess) return null;

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ticket.modelToViewDto();
        }

        public async Task<ViewTicketDto?> assignTicketToUser(int ticketId, string userId)
        {
            // include object references
            var ticket = await _context.Tickets
            .Include(t => t.Department)
            .Include(t => t.SubmittedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Category)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return null;

            var departmentName = ticket.Department.Name.ToLower();

            var userHasAccess = await _context.UserDepartmentAccess
            .AnyAsync(a => a.UserId == userId && a.DepartmentId == ticket.DepartmentId);


            if (!userHasAccess) return null;

            ticket.AssignedToId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ticket.modelToViewDto();
        }

        public async Task<List<ViewDepartmentDto>> getAllDepartments()
        {
            var departments = await _context.Departments.ToListAsync();
            return departments.Select(d => new ViewDepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
            }).ToList();
        }


        public async Task<List<ViewDepartmentDto>> getMyAssignedDepartments(string userId)
        {
            var assignedDepts = await _context.UserDepartmentAccess
                .Where(da => da.UserId == userId)
                .Select(da => da.Department) 
                .Select(d => d.modelToViewDto())
                .ToListAsync();

            return assignedDepts;
        }


        public async Task<ViewDepartmentDto?> getDepartmentById(int id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

            if (department is null) return null;
            return department.modelToViewDto();
        }

        public async Task<ViewTicketCategoryDto?> getCategoryById(int id)
        {
            var category = await _context.TicketCategories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return null;
            return category.modelToViewDto();
        }
    }
}
