namespace TicketingSys.Contracts.Misc
{
    public interface IUserUtils
    {
        string? getUserIdOr401();

        Task<List<string>?> getUserRoles();

        Task<bool> checkIfCategoryIsValid(int categoryId, int departmentId);
    }
}
