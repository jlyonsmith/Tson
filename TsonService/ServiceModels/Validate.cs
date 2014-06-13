using System;
using ServiceStack;

namespace TsonService
{
    [Route("/validate", "POST")]
    public class Validate
    {
        public string Tson { get; set; }
    }

    public class ErrorInfo
    {
        public ErrorInfo(string message, int line, int column, int offset)
        {
            this.Message = message;
            this.Line = line;
            this.Column = column;
            this.Offset = offset;
        }

        public string Message { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int Offset { get; set; }
    }

    public class ValidateResponse
    {
        public bool IsValid { get; set; }
        public ErrorInfo Error { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}

