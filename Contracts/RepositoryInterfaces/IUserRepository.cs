using Microsoft.AspNetCore.Mvc;
using TicketingSys.Models;
namespace TicketingSys.Contracts.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<Ticket> addNewTask(Ticket ticket);
    }
}