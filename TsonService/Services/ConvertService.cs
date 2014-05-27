using System;
using ServiceStack;
using TsonLibrary;

namespace TsonService
{
    public class ConvertService : Service
    {
        public ITsonServiceConfig Config { get; set; }

        public ConvertResponse Post(Convert request)
        {
            ConvertResponse response = new ConvertResponse();

            FormatType formatType = (FormatType)Enum.Parse(typeof(FormatType), request.ToFormat, ignoreCase: true);

            switch (formatType)
            {
            case FormatType.Json:
                response.Data = Tson.ToJson(request.Tson);
                break;
            case FormatType.Jsv:
                response.Data = Tson.ToJsv(request.Tson);
                break;
            case FormatType.Xml:
                response.Data = Tson.ToXml(request.Tson);
                break;
            }

            return response;
        }
    }
}

