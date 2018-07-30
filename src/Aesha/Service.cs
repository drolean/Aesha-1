using Aesha.Commands;
using Aesha.Infrastructure;
using Serilog;
using Topshelf;

namespace Aesha
{
    public class Service
    {
        private readonly CommandDispatcher _dispatcher;

        public Service(ILogger logger)
        {
            _dispatcher = new CommandDispatcher(logger);


            _dispatcher.SingleThreadedMode = true;
            //_dispatcher.ScanInstance(new StartRobotHandler());
            _dispatcher.Start();
        }

        public bool Start(HostControl hostControl)
        {
            _dispatcher.Start();
            return true;
        }

        public void Stop()
        {
            _dispatcher.Stop();
        }
    }
}
