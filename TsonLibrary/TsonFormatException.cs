using System;

namespace TsonLibrary
{
    public class TsonFormatException : Exception
    {
        public TextLocation ErrorLocation { get { return (TextLocation)this.Data["Location"]; } }

        public TsonFormatException(TsonToken token) : this(token, "")
        {
        }

        public TsonFormatException(TsonToken token, string message) : base(message)
        {
            this.Data["Location"] = token.Location;
        }
    }
}

