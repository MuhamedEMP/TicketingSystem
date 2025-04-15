namespace TicketingSys.Exceptions
{
    public class CantDeleteDepartmentException : Exception
    {
        const string Default = "Could not delete department.";

        public CantDeleteDepartmentException(string message) : base(message)
        {
            
        }

        public CantDeleteDepartmentException() : base(Default) {
   
        }
    }
}
