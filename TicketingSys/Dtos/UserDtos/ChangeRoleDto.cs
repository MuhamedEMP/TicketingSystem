namespace TicketingSys.Dtos.UserDtos
{
    public class ChangeRoleDto
    {
        public bool? isAdmin { get; set; }
        public List<int>? DepartmentIds { get; set; }
    }
}
