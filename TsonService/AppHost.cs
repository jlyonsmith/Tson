using System;
using ServiceStack;
using ServiceStack.Logging;
using System.Reflection;
using ServiceStack.Text;
using ServiceStack.Host;
using System.Collections.Generic;
using System.Text;
using System.Net.Mime;

namespace TsonService
{
    public class AppHost : AppHostHttpListenerBase
    {
        private ILog log = LogManager.GetLogger(typeof(AppHost));

        public AppHost() : base("TSON Service", typeof(AppHost).Assembly)
        {
        }

        public override void Configure(Funq.Container container)
        {
            var appConfig = (TsonServiceConfig)this.Container.Resolve<ITsonServiceConfig>();

            JsConfig.EmitCamelCaseNames = true;

            SetConfig(
                new HostConfig
            { 
                EnableFeatures = Feature.All & ~Feature.Soap,
                DefaultContentType = "application/json",
                AppendUtf8CharsetOnContentTypes = new HashSet<string>
                {
                    "application/json", "application/xml"
                },
                #if DEBUG
                DebugMode = true,
                #else
                DebugMode = false,
                #endif
            });

            log.Info(appConfig.ToString());
        }
    }
}

