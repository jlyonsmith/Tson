using System;

namespace TsonLibrary
{
    public class TsonFormatException : Exception
    {
        public TextLocation ErrorLocation { get { return (TextLocation)this.Data["Location"]; } }

        public TsonFormatException(TsonToken token) : this(token, "")
        {
        }

        public TsonFormatException(TsonToken token, string message) : this(token, message, null)
        {
        }

        public TsonFormatException(TsonToken token, string message, Exception innerException) : base(message, innerException)
        {
            this.Data["Location"] = token.Location;
        }
    }
}

