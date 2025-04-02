namespace TicketingSys.Contracts.Misc
{
    public interface IUserUtils
    {
        string? getUserId();

        Task<List<string>?> getUserRoles();
    }
}
