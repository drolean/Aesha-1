using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Aesha.Infrastructure
{

    public class CommandDispatcher
    {
        private readonly Dictionary<Type, Action<object>> _commandHandlers = new Dictionary<Type, Action<object>>();
        private readonly ILogger _logger;

        private readonly ConcurrentQueue<ICommand> _commandQueue = new ConcurrentQueue<ICommand>();
        private Task _dispatcher;
        private CancellationTokenSource _dispatcherCancellationTokenSource;

        public CommandDispatcher(ILogger logger)
        {
            _logger = logger;
            SingleThreadedMode = false;
        }

        public bool SingleThreadedMode { get; set; }

        public void Start()
        {
            _dispatcherCancellationTokenSource = new CancellationTokenSource();
            _dispatcher = new Task(DequeueMessages, _dispatcherCancellationTokenSource.Token);
            _dispatcher.Start();
        }

        public void Stop()
        {
            _dispatcherCancellationTokenSource.Cancel();
        }

        private void DequeueMessages()
        {
            while (!_dispatcherCancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var foundCommand = DequeueCommand();

                    Thread.Sleep(foundCommand ? 0 : 500);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred whilst dequeuing messages");
                    throw;
                }
            }
        }

        private bool DequeueCommand()
        {
            ICommand c;
            var foundCommand = _commandQueue.TryDequeue(out c);

            if (!foundCommand)
            {
                return false;
            }

            try
            {
                _commandHandlers[c.GetType()](c);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred whilst executing command '{c.GetType().Name}'");
                return false;
            }
        }
        
        public void SendCommand<TCommand>(TCommand c)
        {
            if (_commandHandlers.ContainsKey(c.GetType()))
            {
                _commandHandlers[c.GetType()](c);
            }
            else
            {
                throw new Exception("No command handler registered for " + c.GetType().Name);
            }
        }

        public void SendCommandAsync<TCommand>(TCommand c)
        {
            if (SingleThreadedMode)
            {
                SendCommand(c);
                return;
            }

            if (_commandHandlers.ContainsKey(c.GetType()))
            {
                _commandQueue.Enqueue((ICommand)c);
            }
            else
            {
                throw new Exception("No command handler registered for " + typeof(TCommand).Name);
            }
        }

        public void AddHandlerFor<TCommand>(IHandleCommand<TCommand> commandHandler)
        {
            if (!_commandHandlers.ContainsKey(typeof(TCommand)))
            {
                _commandHandlers.Add(typeof(TCommand), c =>
                {
                    _logger.Debug($"Command type '{c.GetType().Name}' received");
                    commandHandler.Handle((TCommand) c);
                    _logger.Debug($"Command type '{c.GetType().Name}' handled by command handler '{commandHandler.GetType().Name}'");
                });
            }

            _logger.Debug($"Registered command handler '{commandHandler.GetType().Name}' for command type '{typeof(TCommand).Name}'");
        }

        public void ScanInstance(object instance)
        {
            var handlers = GetImplementationsOfInterface(instance, typeof(IHandleCommand<>));

            var addHandlerForMethodInfo = GetType().GetMethod(nameof(AddHandlerFor));

            var parameters = new[] { instance };

            handlers.ForEach(h => addHandlerForMethodInfo.MakeGenericMethod(h).Invoke(this, parameters));
        }

        private static IEnumerable<Type> GetImplementationsOfInterface(object instance, Type interfaceType)
        {
            return instance.GetType().GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                .Select(i => i.GetGenericArguments()[0]).ToList();
        }

        public void AssertRegistration(Assembly asm, bool throwExceptionOnFailure)
        {
            var types = asm.GetTypes();
            var errors = new StringBuilder();

            foreach (var t in types)
            {
                AssertHandlersRegistration(t, errors);
            }

            if (errors.Length <= 0)
            {
                return;
            }

            var message = errors.ToString();

            if (throwExceptionOnFailure)
            {
                throw new RegistrationFailureException(message);
            }

            _logger.Warning(message);
        }
        
        private void AssertHandlersRegistration(Type t, StringBuilder errors)
        {
            var handlers = t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleCommand<>))
                .Select(y => new { Handler = t, CommandType = y.GetGenericArguments()[0] }).ToList();

            if (handlers.Count > 0)
                handlers.ForEach(h =>
                {
                    var exists = _commandHandlers.ContainsKey(h.CommandType);
                    if (!exists)
                        errors.AppendLine(
                            $"CommandHandler '{h.Handler.Name}' not registered for command '{h.CommandType.Name}'");
                });
        }


    }
}
