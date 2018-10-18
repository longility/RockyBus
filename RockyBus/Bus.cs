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

        public Bus(IMessageTransport busTransport, IDependencyResolver dependencyResolver, MessageScanRules rules, 
                   Func<MessageHandlingExceptionRaisedEventArgs, Task> messageHandlingExceptionHandler = null)
        {
            busTransport.Bus = this;
            this.busTransport = busTransport;
            this.dependencyResolver = dependencyResolver;
            this.busMessages = new BusMessages(rules, dependencyResolver);
            this.messageHandlingExceptionHandler = messageHandlingExceptionHandler;
        }

        public Task Publish<T>(T eventMessage, PublishOptions options = null)
        {
            var type = typeof(T);
            if (!busMessages.IsPublishable(type)) throw BusMessages.CreateMessageNotFoundException(type);
            return busTransport.Publish(eventMessage, MessageTypeToNamePublishingEventMap[type], options);
        }

        public Task Send<T>(T commandMessage, SendOptions options = null)
        {
            var type = typeof(T);
            if (!busMessages.IsSendable(type)) throw BusMessages.CreateMessageNotFoundException(type);
            return busTransport.Send(commandMessage, MessageTypeToNameSendingCommandMap[type], options);
        }

        public async Task Start()
        {
            await busTransport.InitializePublishingEndpoint().ConfigureAwait(false);
            if (!busTransport.IsPublishAndSendOnly)
            {
                busTransport.ReceivingMessageTypeNames = busMessages;
                if (dependencyResolver == null) throw new InvalidOperationException("Receiving bus requires message handlers. Add message handlers through dependency injection or BusBuilder.");
                await busTransport.InitializeReceivingEndpoint().ConfigureAwait(false);
                var executor = new MessageHandlerExecutor(dependencyResolver, messageHandlingExceptionHandler);
                await busTransport.StartReceivingMessages(executor).ConfigureAwait(false);
            }
        }

        public Task Stop()
        {
            return busTransport.StopReceivingMessages();
        }

        internal IDictionary<Type, string> MessageTypeToNamePublishingEventMap => busMessages.MessageTypeToNamePublishingEventMap;
        internal IDictionary<Type, string> MessageTypeToNameSendingCommandMap => busMessages.MessageTypeToNameSendingCommandMap;
    }
}
