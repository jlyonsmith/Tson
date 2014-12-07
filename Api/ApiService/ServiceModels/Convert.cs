using System;
using ServiceStack;

namespace TsonService
{
    public enum FormatType
    {
        Tson,
        Json,
        Jsv,
        Xml
    }

    [Route("/convert", "POST")]
    public class Convert
    {
        public string Data { get; set; }
        public string FromFormat { get; set; }
        public string ToFormat { get; set; }
    }

    public class ConvertResponse
    {
        public string Data { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}

