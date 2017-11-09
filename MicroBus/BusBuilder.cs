using System;
using MicroBus.Abstractions;

namespace MicroBus
{
    public class BusBuilder
    {
        private IMessageTransport messageTransport;
        private IDependencyResolver dependencyResolver = new LowClassDependencyResolver();

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

        public IBus Build()
        {
            if (messageTransport == null) throw new ArgumentException("A message transport is required for a bus.");
            messageTransport.SetMessageHandlerExecutor(new MessageHandlerExecutor(dependencyResolver));
            return new Bus(messageTransport);
        }
    }
}
