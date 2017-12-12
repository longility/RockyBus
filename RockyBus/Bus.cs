using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RockyBus.Message;

namespace RockyBus
{
    class Bus : IBus
    {
        readonly IMessageTransport busTransport;
        readonly IDependencyResolver dependencyResolver;
        readonly BusMessages busMessages;
        readonly Func<MessageHandlingExceptionRaisedEventArgs, Task> messageHandlingExceptionHandler;
        bool started = false;

        public Bus(IMessageTransport busTransport, IDependencyResolver dependencyResolver, MessageScanRules rules, 
                   Func<MessageHandlingExceptionRaisedEventArgs, Task> messageHandlingExceptionHandler = null)
        {
            this.busTransport = busTransport;
            this.dependencyResolver = dependencyResolver;
            this.busMessages = new BusMessages(rules, dependencyResolver);
            this.messageHandlingExceptionHandler = messageHandlingExceptionHandler;
        }

        public Task Publish<T>(T eventMessage)
        {
            return Publish(eventMessage, null);
        }

        public Task Publish<T>(T eventMessage, PublishOptions options)
        {
            if (!started) throw new InvalidOperationException("The bus has not been started.");
            var type = typeof(T);
            if (!busMessages.IsPublishable(type)) throw BusMessages.CreateMessageNotFoundException(type);
            return options == null
                ? busTransport.Publish(eventMessage, MessageTypeToNamePublishingEventMap[type])
                : busTransport.Publish(eventMessage, MessageTypeToNamePublishingEventMap[type], options);
        }

        public Task Send<T>(T commandMessage)
        {
            return Send(commandMessage, null);
        }

        public Task Send<T>(T commandMessage, SendOptions options)
        {
            if (!started) throw new InvalidOperationException("The bus has not been started.");
            var type = typeof(T);
            if (!busMessages.IsSendable(type)) throw BusMessages.CreateMessageNotFoundException(type);
            return options == null
                ? busTransport.Send(commandMessage, MessageTypeToNameSendingCommandMap[type])
                : busTransport.Send(commandMessage, MessageTypeToNameSendingCommandMap[type], options);
        }

        public async Task Start()
        {
            busTransport.ReceivingMessageTypeNames = busMessages;
            await busTransport.InitializePublishingEndpoint();
            if (!busTransport.IsPublishAndSendOnly)
            {
                if (dependencyResolver == null) throw new InvalidOperationException("Receiving bus requires message handlers. Add message handlers through dependency injection or BusBuilder.");
                await busTransport.InitializeReceivingEndpoint();
                var executor = new MessageHandlerExecutor(dependencyResolver, messageHandlingExceptionHandler);
                await busTransport.StartReceivingMessages(executor);
            }

            started = true;
        }

        public Task Stop()
        {
            return busTransport.StopReceivingMessages();
        }

        internal IDictionary<Type, string> MessageTypeToNamePublishingEventMap => busMessages.MessageTypeToNamePublishingEventMap;
        internal IDictionary<Type, string> MessageTypeToNameSendingCommandMap => busMessages.MessageTypeToNameSendingCommandMap;
    }
}
