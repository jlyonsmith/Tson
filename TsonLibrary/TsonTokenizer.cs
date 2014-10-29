using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Globalization;

namespace TsonLibrary
{
    public enum TsonTokenType
    {
        Error,
        Whitespace,
        Comment,
        String,
        Colon,
        Comma,
        LeftSquareBrace,
        RightSquareBrace,
        LeftCurlyBrace,
        RightCurlyBrace,
        End
    }

    public enum TsonValueType
    {
        None = 0,
        Boolean,
        Number,
        String,
        Null,
        Object,
        Array
    }

    public class TsonToken
    {
        public TsonToken(TsonTokenType tokenType, TextLocation location)
        {
            this.TokenType = tokenType;
            this.Location = location;
        }

        public TsonToken(TsonTokenType tokenType, TextLocation location, object data) : this(tokenType, location)
        {
            this.Data = data;
        }

        public static TsonToken End(TextLocation location) { return new TsonToken(TsonTokenType.End, location); }
        public static TsonToken Whitespace(TextLocation location, string whitespace) { return new TsonToken(TsonTokenType.Whitespace, location, whitespace); }
        public static TsonToken Comment(TextLocation location, string comment) { return new TsonToken(TsonTokenType.Comment, location, comment); }
        public static TsonToken Comma(TextLocation location) { return new TsonToken(TsonTokenType.Comma, location); }
        public static TsonToken Colon(TextLocation location) { return new TsonToken(TsonTokenType.Colon, location); }
        public static TsonToken LeftSquareBrace(TextLocation location) { return new TsonToken(TsonTokenType.LeftSquareBrace, location); }
        public static TsonToken RightSquareBrace(TextLocation location) { return new TsonToken(TsonTokenType.RightSquareBrace, location); }
        public static TsonToken LeftCurlyBrace(TextLocation location) { return new TsonToken(TsonTokenType.LeftCurlyBrace, location); }
        public static TsonToken RightCurlyBrace(TextLocation location) { return new TsonToken(TsonTokenType.RightCurlyBrace, location); }
        public static TsonToken Error(TextLocation location) { return new TsonToken(TsonTokenType.Error, location); }
        public static TsonToken String(TextLocation location, string s) { return new TsonToken(TsonTokenType.String, location, s); }

        public static TsonToken End(int offset) { return new TsonToken(TsonTokenType.End, new TextLocation(offset)); }
        public static TsonToken Whitespace(int offset, string whitespace) { return new TsonToken(TsonTokenType.Whitespace, new TextLocation(offset), whitespace); }
        public static TsonToken Comment(int offset, string comment) { return new TsonToken(TsonTokenType.Comment, new TextLocation(offset), comment); }
        public static TsonToken Comma(int offset) { return new TsonToken(TsonTokenType.Comma, new TextLocation(offset)); }
        public static TsonToken Colon(int offset) { return new TsonToken(TsonTokenType.Colon, new TextLocation(offset)); }
        public static TsonToken LeftSquareBrace(int offset) { return new TsonToken(TsonTokenType.LeftSquareBrace, new TextLocation(offset)); }
        public static TsonToken RightSquareBrace(int offset) { return new TsonToken(TsonTokenType.RightSquareBrace, new TextLocation(offset)); }
        public static TsonToken LeftCurlyBrace(int offset) { return new TsonToken(TsonTokenType.LeftCurlyBrace, new TextLocation(offset)); }
        public static TsonToken RightCurlyBrace(int offset) { return new TsonToken(TsonTokenType.RightCurlyBrace, new TextLocation(offset)); }
        public static TsonToken Error(int offset) { return new TsonToken(TsonTokenType.Error, new TextLocation(offset)); }
        public static TsonToken String(int offset, string s) { return new TsonToken(TsonTokenType.String, new TextLocation(offset), s); }

        public bool IsWhitespace { get { return this.TokenType == TsonTokenType.Whitespace; } }
        public bool IsComment { get { return this.TokenType == TsonTokenType.Comment; } }
        public bool IsComma { get { return this.TokenType == TsonTokenType.Comma; } }
        public bool IsColon { get { return this.TokenType == TsonTokenType.Colon; } }
        public bool IsLeftSquareBrace { get { return this.TokenType == TsonTokenType.LeftSquareBrace; } }
        public bool IsRightSquareBrace { get { return this.TokenType == TsonTokenType.RightSquareBrace; } }
        public bool IsLeftCurlyBrace { get { return this.TokenType == TsonTokenType.LeftCurlyBrace; } }
        public bool IsRightCurlyBrace { get { return this.TokenType == TsonTokenType.RightCurlyBrace; } }
        public bool IsError { get { return this.TokenType == TsonTokenType.Error; } }
        public bool IsString { get { return this.TokenType == TsonTokenType.String; } }
        public bool IsEnd { get { return this.TokenType == TsonTokenType.End; } }

        public TsonTokenType TokenType { get; private set; }
        public object Data { get; private set; }
        public TextLocation Location { get; private set; }

        public override string ToString()
        {
            return string.Format("[TsonToken: TokenType={0}, Offset = {1}, Data={2}]", TokenType, Location.Offset, Data);
        }
    }

    public class TsonTokenizer
    {
        private TextLocation currentLocation;
        private string input;
        private TsonToken nextToken;
        private TsonToken endToken;

        public TextLocation CurrentLocation { get { return currentLocation; } }
        public string Input { get { return input; } }

        public TsonTokenizer(string input)
        {
            this.input = input;
            this.currentLocation = new TextLocation(0, 1, 1);
        }

        void MoveToNextChar()
        {
            Char c = CurrentChar();

            currentLocation.Offset++;
            currentLocation.Column++;

            if (c == '\n')
            {
                currentLocation.Line++; 
                currentLocation.Column = 1;
            }
        }

        char CurrentChar()
        {
            if (currentLocation.Offset < input.Length)
                return input[currentLocation.Offset];
            else
                return '\0';
        }

        string SliceInput(ref TextLocation start, ref TextLocation end)
        {
            return input.Substring(start.Offset, end.Offset - start.Offset);
        }

        public static bool IsTsonPunctuation(char c)
        {
            return (c == ',' || c == ':' || c == '[' || c == ']' || c == '{' || c == '}' || c == '"' || c == '#');
        }

        public static bool IsTsonControl(char c)
        {
            return (c == '"' || c == '\\' || c == '/' || c == 'b' || c == 'f' || c == 'n' || c == 'r' || c == 't' || c == 'u');
        }

        public virtual TsonToken Next()
        {
            // Are we all done?
            if (endToken != null)
                return endToken;

            TsonToken token = null;

            // If we already cached a token...
            if (nextToken != null)
            {
                token = nextToken;
                nextToken = null;
                return token;
            }

            TextLocation tokenLocation;
            char c = CurrentChar();

            if (c == '\0')
            {
                tokenLocation = currentLocation;
                MoveToNextChar();

                return (endToken = TsonToken.End(tokenLocation));
            }
            else if (Char.IsWhiteSpace(c))
            {
                tokenLocation = currentLocation;

                do
                {
                    MoveToNextChar();
                }
                while (Char.IsWhiteSpace(CurrentChar()));

                return TsonToken.Whitespace(tokenLocation, SliceInput(ref tokenLocation, ref currentLocation));
            } 
            else if (c == '#')
            {
                tokenLocation = currentLocation;
                MoveToNextChar();

                while ((c = CurrentChar()) != '\0' && c != '\n')
                {
                    MoveToNextChar();
                }

                return TsonToken.Comment(tokenLocation, SliceInput(ref tokenLocation, ref currentLocation));
            }
            else if (c == '"')
            {
                tokenLocation = currentLocation;

                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder(4);

                sb.Append(c);
                MoveToNextChar();

                while ((c = CurrentChar()) != '\0' && c != '"')
                {
                    if (c == '\\')
                    {
                        MoveToNextChar();

                        if ((c = CurrentChar()) == '\0' || !IsTsonControl(c))
                            return TsonToken.Error(currentLocation);

                        switch (c)
                        {
                        case '"':
                        case '\\':
                        case '/':
                            sb.Append(c);
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            sb2.Clear();
                            ushort n;
                            TextLocation digitLocation = currentLocation;

                            for (int i = 0; i < 4; i++)
                            {
                                MoveToNextChar();

                                if ((c = CurrentChar()) == '\0' || c == '"')
                                    return TsonToken.Error(currentLocation);

                                sb2.Append(CurrentChar());
                            }

                            if (!UInt16.TryParse(sb2.ToString(), NumberStyles.AllowHexSpecifier, null, out n))
                                return TsonToken.Error(digitLocation);

                            sb.Append((char)n);
                            break;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    MoveToNextChar();
                }

                if (c == '"')
                {
                    sb.Append(c);
                    MoveToNextChar();

                    return TsonToken.String(tokenLocation, sb.ToString());
                }
                else
                    return TsonToken.Error(currentLocation);
            }

            switch ((char)c)
            {
            case ',':
                token = TsonToken.Comma(currentLocation);
                break;
            case ':':
                token = TsonToken.Colon(currentLocation);
                break;
            case '[':
                token = TsonToken.LeftSquareBrace(currentLocation);
                break;
            case ']':
                token = TsonToken.RightSquareBrace(currentLocation);
                break;
            case '{':
                token = TsonToken.LeftCurlyBrace(currentLocation);
                break;
            case '}':
                token = TsonToken.RightCurlyBrace(currentLocation);
                break;
            }

            if (token != null)
            {
                MoveToNextChar();
                return token;
            }

            tokenLocation = currentLocation;

            for(;;)
            {
                MoveToNextChar();

                if ((c = CurrentChar()) == '\0' || IsTsonPunctuation(c))
                    break;

                if (Char.IsWhiteSpace(c))
                {
                    var whitespaceLocation = currentLocation;

                    do
                    {
                        MoveToNextChar();
                    }
                    while (Char.IsWhiteSpace(CurrentChar()));

                    if ((c = CurrentChar()) == '\0' || IsTsonPunctuation(c))
                    {
                        // This is trailing whitespace; make it appear as the next token
                        this.nextToken = TsonToken.Whitespace(whitespaceLocation, SliceInput(ref whitespaceLocation, ref currentLocation));

                        return TsonToken.String(tokenLocation, SliceInput(ref tokenLocation, ref whitespaceLocation));
                    }

                    // Otherwise, just keep processing the string...
                }
            }

            return TsonToken.String(tokenLocation, SliceInput(ref tokenLocation, ref currentLocation));
        }

        public virtual TsonToken PeekNext()
        {
            if (endToken != null)
                return endToken;

            if (nextToken == null)
            {
                nextToken = Next();
            }

            return nextToken;
        }
    }
}

