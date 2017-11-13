using System;
using MicroBus.Message;
using MicroBus.Abstractions;
using System.Threading.Tasks;

namespace MicroBus
{
    public class BusBuilder
    {
        private IMessageTransport messageTransport;
        private IDependencyResolver dependencyResolver;
        private PoorMansDependencyInjection poorMansDependencyInjection;
        private readonly MessageScanRules messageScanRules = new MessageScanRules();
        private Func<MessageHandlingExceptionRaisedEventArgs, Task> exceptionHandler;
        public BusBuilder UseMessageTransport(IMessageTransport messageTransport)
        {
            this.messageTransport = messageTransport;
            return this;
        }

        public BusBuilder UseDependencyResolver(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            return this;
        }

        public BusBuilder DefineEventScanRuleWith(Func<Type, bool> isAnEvent)
        {
            messageScanRules.DefineEventScanRuleWith(isAnEvent);
            return this;
        }

        public BusBuilder DefineCommandScanRuleWith(Func<Type, bool> isACommand)
        {
            messageScanRules.DefineCommandScanRuleWith(isACommand);
            return this;
        }

        public BusBuilder HandleMessageHandleExceptions(Func<MessageHandlingExceptionRaisedEventArgs, Task> exceptionHandler)
        {
            this.exceptionHandler = exceptionHandler;
            return this;
        }

        public BusBuilder AddMessageHandler<T>(Func<IMessageHandler<T>> createMessageHandler)
        {
            if (poorMansDependencyInjection == null) poorMansDependencyInjection = new PoorMansDependencyInjection();
            poorMansDependencyInjection.AddMessageHandler(createMessageHandler);
            return this;
        }

        public IBus Build()
        {
            if (messageTransport == null) throw new ArgumentException("A message transport is required for a bus.");
            if (dependencyResolver != null && poorMansDependencyInjection != null) throw new ArgumentException("Use either dependency injection or add message handler, not both.");

            return new Bus(messageTransport, dependencyResolver ?? poorMansDependencyInjection, messageScanRules, exceptionHandler);
        }
    }
}
