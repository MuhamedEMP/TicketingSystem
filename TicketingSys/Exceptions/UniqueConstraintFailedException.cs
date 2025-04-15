﻿using System.Globalization;

namespace TicketingSys.Exceptions
{
    public class UniqueConstraintFailedException : Exception
    {
        const string Default = "Your db entry violates unique constraint.";


        public UniqueConstraintFailedException() : base(Default) { }

        public UniqueConstraintFailedException(string message) : base(message) { }  
    }
}
