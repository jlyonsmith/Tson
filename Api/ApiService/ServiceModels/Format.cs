﻿using System;
using ServiceStack;

namespace TsonService
{
    [Route("/format", "POST")]
    public class Format
    {
        public string Tson { get; set; }
    }

    public class FormatResponse
    {
        public string Tson { get; set; }
        public ErrorInfo Error { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}

