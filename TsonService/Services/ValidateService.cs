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

            try
            {
                new TsonParser().Parse(request.Tson);

                response.IsValid = true;
            }
            catch (TsonParseException e)
            {
                var location = e.ErrorLocation;

                response.Error = new ErrorInfo(e.Message, location.Line, location.Column, location.Offset);
                response.IsValid = false;
            }

            return response;
        }
    }
}

