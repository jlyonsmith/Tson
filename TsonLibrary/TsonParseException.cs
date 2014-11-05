using System;

namespace TsonLibrary
{
    public class TsonParseException : Exception
    {
        public TextLocation ErrorLocation { get { return (TextLocation)this.Data["Location"]; } }

        public TsonParseException(TsonToken token) : this(token, "")
        {
        }

        public TsonParseException(TsonToken token, string message) : base(message)
        {
            this.Data["Location"] = token.Location;
        }
    }
}

