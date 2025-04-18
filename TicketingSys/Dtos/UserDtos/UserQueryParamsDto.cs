namespace TicketingSys.Dtos.UserDtos
{
    public class UserQueryParamsDto
    {
        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string? fullName { get; set; }

        public string? email { get; set; }

        public bool? isAdmin {  get; set; }
        public bool? hasDepartments { get; set; }

    }
}
