using System;
using ServiceStack.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace TsonService
{
    public interface ITsonServiceConfig
    {
        string ServiceUrl { get; }
    }

    public class TsonServiceConfig : ITsonServiceConfig
    {
        public string ServiceUrl { get; private set; }
        public IList<string> CorsAllowedOrigins { get; set; }
        public AppSettings Settings { get; private set; }

        public TsonServiceConfig(string arg0, string arg1)
        {
            Settings = new AppSettings();
            this.ServiceUrl = arg0 ?? Settings.Get("ServiceUrl", "http://*:1337/");
            this.CorsAllowedOrigins = Settings.GetList("CorsAllowedOrigins");
        }

        public override string ToString()
        {
            return string.Format("ServiceUrl={0}, CorsAllowedOrigins={1}", 
                ServiceUrl, String.Join(", ", CorsAllowedOrigins));
        }
    }
}

