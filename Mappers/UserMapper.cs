using TicketingSys.Dtos.UserDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class UserMapper
    {
        public static ViewUserDto userModelToDto(this User userModel)
        {
            return new ViewUserDto
            {
                fullName = userModel.fullName,
                firstName = userModel.firstName,
                lastName = userModel.lastName,
                email = userModel.email,
                roles = userModel.roles,
            };
            
        
        }
    }
}
