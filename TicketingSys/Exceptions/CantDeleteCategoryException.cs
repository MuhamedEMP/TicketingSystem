namespace TicketingSys.Exceptions
{
    public class CantDeleteCategoryException : Exception
    {
        const string Default = "Could not delete category.";

        public CantDeleteCategoryException(string message) : base(message)
        {

        }

        public CantDeleteCategoryException() : base(Default)
        {

        }
    }
}
