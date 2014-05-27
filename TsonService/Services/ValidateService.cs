using System;
using ServiceStack;
using TsonLibrary;

namespace TsonService
{
    public class ValidateService : Service
    {
        public ITsonServiceConfig Config { get; set; }

        public ValidateResponse Post(Validate request)
        {
            var response = new ValidateResponse();

            response.IsValid = Tson.Validate(request.Tson);

            return response;
        }
    }
}

