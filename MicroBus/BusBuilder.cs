using System;
using MicroBus.Abstractions;

namespace MicroBus
{
    public class BusBuilder
    {
        private IMessageTransport messageTransport;
        private IDependencyResolver dependencyResolver = new LowClassDependencyResolver();
        private IMessageHandlerResolution messageHandlerResolution;

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

        public BusBuilder UseMessageHandlerResolution(IMessageHandlerResolution messageHandlerResolution) 
        {
            this.messageHandlerResolution = messageHandlerResolution;
            return this;
        }

        public IBus Build()
        {
            if (messageTransport == null) throw new ArgumentException("A message transport is required for a bus.");
            messageTransport.SetMessageHandlerExecutor(new MessageHandlerExecutor(dependencyResolver));
            if (messageHandlerResolution == null) throw new ArgumentException("Message transport requires message handler resolution");
            messageTransport.SetHandlingEventMessageTypes(messageHandlerResolution.ResolvableMessageTypeNames());
            return new Bus(messageTransport);
        }
    }
}
