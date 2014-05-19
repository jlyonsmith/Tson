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

        public TsonNode Parse(string input)
        {
            tokenizer = new TsonCleanTokenizer(input);

            var token  = tokenizer.PeekNext();
            TsonNode node;

            if (token.IsLeftCurlyBrace)
            {
                node = ParseObject();
            }
            else if (token.IsLeftSquareBrace)
            {
                node = ParseArray();
            }
            else
            {
                node = ParseRootObject();
            }

            token = tokenizer.Next();

            if (!token.IsEnd)
                throw new TsonParseException(token, "Expected EOF");

            return node;
        }

        private TsonNode ParseRootObject()
        {
            TsonToken token = tokenizer.Next();

            if (token.IsEnd)
                return new TsonObjectNode();

            var keyValues = new List<KeyValuePair<TsonStringNode, TsonNode>>();

            while (true)
            {
                if (!token.IsString)
                    throw new TsonParseException(token, "Expected a string");

                TsonStringNode key = new TsonStringNode(token.Data as string);

                token = tokenizer.Next();

                if (!token.IsColon)
                    throw new TsonParseException(token, "Expected a ':'");

                TsonNode value = ParseValueArrayOrObject();

                keyValues.Add(new KeyValuePair<TsonStringNode, TsonNode>(key, value));

                token = tokenizer.Next();

                if (token.IsComma)
                {
                    token = tokenizer.Next();
                    continue;
                }
                else if (token.IsEnd)
                    break;
                else
                    throw new TsonParseException(token, "Expected a ',' or EOF");
            }

            return new TsonObjectNode(keyValues);
        }

        private TsonNode ParseObject()
        {
            TsonToken token = tokenizer.Next();

            if (!token.IsLeftCurlyBrace)
                throw new TsonParseException(token, "Expected a '{'");

            token = tokenizer.Next();

            if (token.IsRightCurlyBrace)
                return new TsonObjectNode();

            var keyValues = new List<KeyValuePair<TsonStringNode, TsonNode>>();

            while (true)
            {
                if (!token.IsString)
                    throw new TsonParseException(token, "Expected a string");

                TsonStringNode key = new TsonStringNode(token.Data as string);

                token = tokenizer.Next();

                if (!token.IsColon)
                    throw new TsonParseException(token, "Expected a ':'");

                TsonNode value = ParseValueArrayOrObject();

                keyValues.Add(new KeyValuePair<TsonStringNode, TsonNode>(key, value));

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

            return new TsonObjectNode(keyValues);
        }

        private TsonNode ParseArray()
        {
            TsonToken token = tokenizer.Next();

            if (!token.IsLeftSquareBrace)
                throw new TsonParseException(token, "Expected a '['");

            token = tokenizer.PeekNext();

            if (token.IsRightSquareBrace)
            {
                tokenizer.Next();

                return new TsonArrayNode();
            }

            var values = new List<TsonNode>();

            while (true)
            {
                TsonNode value = ParseValueArrayOrObject();

                values.Add(value);

                token = tokenizer.Next();

                if (token.IsComma)
                    continue;
                else if (token.IsRightSquareBrace)
                    break;
                else
                    throw new TsonParseException(token, "Expected a ',' or a ']'");
            }

            return new TsonArrayNode(values);
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

