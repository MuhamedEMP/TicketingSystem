using Microsoft.AspNetCore.Mvc;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IUserService
    {
        Task<Ticket> addNewTicket(Ticket ticket);

        Task<User?> getUserById(string id);

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId);

        Task<List<Ticket>?> getAllTicketByUserId(string userId);

        Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters);
    }
}