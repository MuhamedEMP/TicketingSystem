using Microsoft.AspNetCore.Mvc;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Contracts.ServiceInterfaces;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Mappers;
using TicketingSys.Models;

namespace TicketingSys.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Ticket> newTicket(Ticket ticket)
        {
            return await _repository.addNewTask(ticket);
        }

        public async Task<List<ViewTicketDto>?> filterTickets(string userId, TicketQueryParamsDto filters)
        {
            var tickets = await _repository.filterTickets(userId, filters);
            return tickets.Select(t => t.modelToViewDto()).ToList();
        }
    }
}