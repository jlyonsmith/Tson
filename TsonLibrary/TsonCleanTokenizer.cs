using System;

namespace TsonLibrary
{
    // A tokenizer which cleans out whitespace and comments
    public class TsonCleanTokenizer : TsonTokenizer
    {
        public TsonCleanTokenizer(string input) : base(input)
        {
        }

        public override TsonToken Next()
        {
            TsonToken token;

            do
            {
                token = base.Next();
            }
            while ((token.TokenType == TsonTokenType.Whitespace || token.TokenType == TsonTokenType.Comment) && token.TokenType != TsonTokenType.End);

            return token;
        }

        public override TsonToken PeekNext()
        {
            TsonToken token;

            do
            {
                token = base.PeekNext();
            }
            while ((token.TokenType == TsonTokenType.Whitespace || token.TokenType == TsonTokenType.Comment) && token.TokenType != TsonTokenType.End);

            return token;
        }
    }
}

