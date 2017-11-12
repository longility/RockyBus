using System;
using MicroBus.Message;
using MicroBus.Abstractions;
using System.Threading.Tasks;

namespace MicroBus
{
    public class BusBuilder
    {
        private IMessageTransport messageTransport;
        private IDependencyResolver dependencyResolver = new LowClassDependencyResolver();
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
        public IBus Build()
        {
            if (messageTransport == null) throw new ArgumentException("A message transport is required for a bus.");

            return new Bus(messageTransport, dependencyResolver, messageScanRules, exceptionHandler);
        }
    }
}
