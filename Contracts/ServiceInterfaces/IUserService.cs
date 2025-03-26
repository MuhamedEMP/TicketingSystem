using Microsoft.AspNetCore.Mvc;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IUserService
    {
        Task<Ticket> newTicket(Ticket ticket);
    }
}