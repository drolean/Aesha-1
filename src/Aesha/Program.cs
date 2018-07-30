using Aesha.Infrastructure;
using Serilog;
using Topshelf;

namespace Aesha
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                    .WriteTo.Console().MinimumLevel.Verbose()
                    .WriteTo.File("log.txt").MinimumLevel.Verbose()
                .CreateLogger();

            HostFactory.Run(x =>
            {
                x.UseSerilog(logger);
                x.Service<Service>(sc =>
                {
                    sc.ConstructUsing(name => GetService(logger));
                    sc.WhenStarted((service, control) => service.Start(control));
                    sc.WhenStopped(s => s.Stop());
                });
                x.RunAsLocalSystem();

                x.SetServiceName("Aesha");
                x.SetDescription("Aesha Automation");
            });
        }

        private static Service GetService(ILogger logger)
        {
            return new Service(logger);
        }
    }
}
