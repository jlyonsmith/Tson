using System;
using System.Collections.Generic;

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

    public class TsonParser
    {
        private TsonCleanTokenizer tokenizer;

        public TsonParser()
        {
        }

        public TsonObjectNode Parse(string input)
        {
            tokenizer = new TsonCleanTokenizer(input);

            var token  = tokenizer.PeekNext();
            TsonObjectNode node;

            node = ParseRootObject();

            token = tokenizer.Next();

            if (!token.IsEnd)
                throw new TsonParseException(token, "Expected EOF");

            return node;
        }

        private TsonObjectNode ParseRootObject()
        {
            TsonToken token = tokenizer.Next();
            bool hasLeftBrace = false;
            var node = new TsonObjectNode();

            if (token.IsLeftCurlyBrace)
            {
                hasLeftBrace = true;

                token = tokenizer.Next();

                if (token.IsRightCurlyBrace)
                    return node;
            }
            else if (token.IsEnd)
                return node;

            while (true)
            {
                if (!token.IsString)
                    throw new TsonParseException(token, "Expected a string");

                TsonStringNode key = new TsonStringNode(token.Data as string);

                token = tokenizer.Next();

                if (!token.IsColon)
                    throw new TsonParseException(token, "Expected a ':'");

                TsonNode value = ParseValueArrayOrObject();

                node.Add(key, value);

                token = tokenizer.Next();

                if (token.IsComma)
                {
                    token = tokenizer.Next();
                    continue;
                }
                else if (token.IsRightCurlyBrace)
                {
                    if (!hasLeftBrace)
                        throw new TsonParseException(token, "Root object did not have a '{' to match '}'");

                    break;
                }
                else if (token.IsEnd)
                {
                    if (hasLeftBrace)
                        throw new TsonParseException(token, "Expected '}'");

                    break;
                }
                else
                    throw new TsonParseException(token, "Expected ',' or EOF" + (hasLeftBrace ? ", or '}'" : ""));
            }

            return node;
        }

        private TsonObjectNode ParseObject()
        {
            TsonToken token = tokenizer.Next();

            if (!token.IsLeftCurlyBrace)
                throw new TsonParseException(token, "Expected a '{'");

            token = tokenizer.Next();

            var node = new TsonObjectNode();

            if (token.IsRightCurlyBrace)
                return node;

            while (true)
            {
                if (!token.IsString)
                    throw new TsonParseException(token, "Expected a string");

                TsonStringNode key = new TsonStringNode(token.Data as string);

                token = tokenizer.Next();

                if (!token.IsColon)
                    throw new TsonParseException(token, "Expected a ':'");

                TsonNode value = ParseValueArrayOrObject();

                node.Add(key, value);

                token = tokenizer.Next();

                if (token.IsComma)
                {
                    token = tokenizer.Next();
                    continue;
                }
                else if (token.IsRightCurlyBrace)
                    break;
                else
                    throw new TsonParseException(token, "Expected a '}'");
            }

            return node;
        }

        private TsonArrayNode ParseArray()
        {
            TsonToken token = tokenizer.Next();

            if (!token.IsLeftSquareBrace)
                throw new TsonParseException(token, "Expected a '['");

            token = tokenizer.PeekNext();

            var node = new TsonArrayNode();

            if (token.IsRightSquareBrace)
            {
                tokenizer.Next();

                return node;
            }

            while (true)
            {
                TsonNode value = ParseValueArrayOrObject();

                node.Add(value);

                token = tokenizer.Next();

                if (token.IsComma)
                    continue;
                else if (token.IsRightSquareBrace)
                    break;
                else
                    throw new TsonParseException(token, "Expected a ',' or a ']'");
            }

            return node;
        }

        private TsonNode ParseValueArrayOrObject()
        {
            var token = tokenizer.PeekNext();

            if (token.IsLeftCurlyBrace)
                return ParseObject();
            else if (token.IsLeftSquareBrace)
                return ParseArray();
            else 
                return ParseValue();
        }

        private TsonNode ParseValue()
        {
            var token = tokenizer.Next();

            if (!token.IsString)
                throw new TsonParseException(token, "Expected a string");
           
            TsonStringNode stringNode = new TsonStringNode((string)token.Data);

            // See if we can more strongly type the node
            if (stringNode.IsQuoted)
                return stringNode;

            if (stringNode.Value == "null")
                return new TsonNullNode();

            if (stringNode.Value == "true")
                return new TsonBooleanNode(true);

            if (stringNode.Value == "false")
                return new TsonBooleanNode(false);

            double n;

            if (Double.TryParse(stringNode.Value, out n))
                return new TsonNumberNode(n);

            return stringNode;
        }
    }
}

