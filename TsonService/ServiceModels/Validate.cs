using System;
using ServiceStack;

namespace TsonService
{
    [Route("/validate", "POST")]
    public class Validate
    {
        public string Tson { get; set; }
    }

    public class ValidateResponse
    {
        public bool IsValid { get; set; }
    }
}

