using Microsoft.AspNetCore.Mvc;
using TicketingSys.Models;
namespace TicketingSys.Contracts.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<Ticket> addNewTask(Ticket ticket);

        Task<User?> getUserById(string id);

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId);

        Task<List<Ticket>?> getAllTicketByUserId(string userId);
    }

}