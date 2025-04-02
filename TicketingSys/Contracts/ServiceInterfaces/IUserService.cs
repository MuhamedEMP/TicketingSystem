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

        Task<User?> getUserById(string id); // remvove

        Task<Ticket?> getTicketByUserIdAndTicketId(string userId, int ticketId); 

        Task<List<Ticket>?> getAllTicketByUserId(string userId);

        Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters);

        Task<Response> AddResponse(NewResponseDto dto, string userId); // remove

        Task<Ticket?> getTikcketById(int ticketId);

        Task<List<ViewResponseDto>> getResponsesToUserTickets(string userId);

        Task<List<ViewResponseDto>> getResponsesSentByUser(string userId); // remove

        Task<ViewResponseDto?> getSentResponseByUserIdAndResponseId(string userId, int responseId); // remove

        Task<ViewResponseDto?> GetResponseToMyTicketById(string userId, int responseId);
    }
}