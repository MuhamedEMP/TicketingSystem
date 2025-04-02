namespace TicketingSys.Dtos.UserDtos
{
    public class ChangeRoleDto
    {
        public string userId { get; set; }

        public List<string> roles {  get; set; }
    }
}
