using System;
using System.Reflection;
using Mono.Unix;
using Mono.Unix.Native;
using Funq;
using System.Text;
using ServiceStack.Text;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;

namespace TsonService
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.LogFactory = new NLogFactory();

            var appHost = new AppHost();
            var appConfig = new TsonServiceConfig(args.Length > 0 ? args[0] : null, args.Length > 1 ? args[1] : null);
            appHost.Container.Register<ITsonServiceConfig>(appConfig);
            appHost.Init();
            appHost.Start(appConfig.ServiceUrl);

            UnixSignal[] signals = new UnixSignal[] { 
                new UnixSignal(Signum.SIGINT), 
                new UnixSignal(Signum.SIGTERM), 
            };

            // Wait for a unix signal
            for (bool exit = false; !exit;)
            {
                int id = UnixSignal.WaitAny(signals);

                if (id >= 0 && id < signals.Length)
                {
                    if (signals[id].IsSet)
                        exit = true;
                }
            }
        }
    }
}
