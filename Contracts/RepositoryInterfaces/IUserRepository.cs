using Microsoft.AspNetCore.Mvc;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Models;
namespace TicketingSys.Contracts.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<Ticket> addNewTask(Ticket ticket);

        Task<User?> getUserById(string id);

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId);

        Task<List<Ticket>?> getAllTicketByUserId(string userId);

        Task<List<Ticket>?> filterTickets(string userId, TicketQueryParamsDto filters);
    }

}