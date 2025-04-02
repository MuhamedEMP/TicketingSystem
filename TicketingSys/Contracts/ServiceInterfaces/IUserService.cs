using Microsoft.AspNetCore.Mvc;
using TicketingSys.Dtos.ResponseDtos;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IUserService
    {
        Task<Ticket> addNewTicket(Ticket ticket);

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId); 

        Task<List<Ticket>?> getAllTicketByUserId(string userId);

        Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters);

        Task<List<ViewResponseDto>> getResponsesToUserTickets(string userId);

        Task<ViewResponseDto?> GetResponseToMyTicketById(string userId, int responseId);
    }
}