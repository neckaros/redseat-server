using System;

namespace RedSeatServer.Exceptions
{
    public class InvalidDateException : Exception
    {
        public InvalidDateException()
        {
        }


        public InvalidDateException(string message, Exception inner)
            : base(message, inner)
        {
        }
        public InvalidDateException(string message) : base(message)
        {
        }
    }
}