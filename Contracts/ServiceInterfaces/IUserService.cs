using Microsoft.AspNetCore.Mvc;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IUserService
    {
        Task<Ticket> newTicket(Ticket ticket);

        Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters);
    }
}