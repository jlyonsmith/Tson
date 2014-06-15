using System;
using ServiceStack;
using TsonLibrary;
using Newtonsoft.Json.Linq;

namespace TsonService
{
    public class ConvertService : Service
    {
        public ITsonServiceConfig Config { get; set; }

        public ConvertResponse Post(Convert request)
        {
            ConvertResponse response = new ConvertResponse();

            if (request.FromFormat == null)
                request.FromFormat = FormatType.Tson.ToString();

            FormatType toFormat = (FormatType)Enum.Parse(typeof(FormatType), request.ToFormat, ignoreCase: true);
            FormatType fromFormat;

            if (String.IsNullOrEmpty(request.FromFormat))
                fromFormat = FormatType.Tson;
            else
                fromFormat = (FormatType)Enum.Parse(typeof(FormatType), request.FromFormat, ignoreCase: true);

            if (fromFormat == FormatType.Tson)
            {
                switch (toFormat)
                {
                case FormatType.Json:
                    response.Data = Tson.ToJson(request.Data);
                    break;
                case FormatType.Jsv:
                    response.Data = Tson.ToJsv(request.Data);
                    break;
                case FormatType.Xml:
                    response.Data = Tson.ToXml(request.Data);
                    break;
                case FormatType.Tson:
                    response.Data = request.Data;
                    break;
                }
            }
            else if (fromFormat == FormatType.Json)
            {
                if (toFormat != FormatType.Tson)
                    throw new NotSupportedException();

                response.Data = new TsonJTokenNodeVisitor(JObject.Parse(request.Data)).ToTson();
            }

            return response;
        }
    }
}

