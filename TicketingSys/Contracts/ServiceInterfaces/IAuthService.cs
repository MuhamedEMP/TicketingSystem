using TicketingSys.Dtos.UserDtos;

namespace TicketingSys.Contracts.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<ViewUserDto> addUserAsync(string userId, string email, List<string> roles, string firstName,
            string lastName, string fullName);

        Task<bool> checkIfUserExists(string userId);    
    }
}
