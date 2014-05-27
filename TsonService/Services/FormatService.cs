using System;
using ServiceStack;
using TsonLibrary;

namespace TsonService
{
    public class FormatService : Service
    {
        public ITsonServiceConfig Config { get; set; }

        public FormatResponse Post(Format request)
        {
            FormatResponse response = new FormatResponse();

            response.Tson = Tson.Format(request.Tson);

            return response;
        }
    }
}

