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

            try
            {
                response.Tson = Tson.Format(request.Tson, TsonFormatStyle.Pretty);
            }
            catch (TsonParseException e)
            {
                var location = e.ErrorLocation;

                response.Error = new ErrorInfo(e.Message, location.Line, location.Column, location.Offset);
            }

            return response;
        }
    }
}

