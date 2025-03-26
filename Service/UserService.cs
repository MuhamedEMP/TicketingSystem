using Microsoft.AspNetCore.Mvc;
using TicketingSys.Contracts.RepositoryInterfaces;
using TicketingSys.Contracts.ServiceInterfaces;
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
    }
}