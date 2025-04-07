namespace TicketingSys.Contracts.Misc
{
    public interface IUserUtils
    {
        string? getUserId();

        Task<List<string>?> getUserRoles();

        Task<bool> checkIfCategoryIsValid(int categoryId, int departmentId);
    }
}
