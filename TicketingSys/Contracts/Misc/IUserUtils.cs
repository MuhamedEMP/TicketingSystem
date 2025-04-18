namespace TicketingSys.Contracts.Misc
{
    public interface IUserUtils
    {
        string? getUserIdOr401();

        Task<bool> checkIfCategoryIsValid(int categoryId, int departmentId);
    }
}
