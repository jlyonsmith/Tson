using System;

namespace TsonLibrary
{
    public class TsonFormatException : Exception
    {
        public TsonFormatException() : base()
        {
        }

        public TsonFormatException(string message) : base(message)
        {
        }

        public TsonFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

