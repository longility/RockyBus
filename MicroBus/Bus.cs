using System;
using System.Threading.Tasks;
using MicroBus.Abstractions;
using MicroBus.Message;

namespace MicroBus
{
    class Bus : IBus
    {
        readonly IMessageTransport busTransport;
        readonly IDependencyResolver dependencyResolver;
        readonly BusMessages busMessages;
        bool started = false;

        public Bus(IMessageTransport busTransport, IDependencyResolver dependencyResolver, MessageScanRules rules)
        {
            this.busTransport = busTransport;
            this.dependencyResolver = dependencyResolver;
            this.busMessages = new BusMessages(rules);
        }

        public Task Publish<T>(T eventMessage)
        {
            if (!started) throw new InvalidOperationException("The bus has not been started.");
            var type = typeof(T);
            if (!busMessages.IsAnEvent(type)) throw BusMessages.CreateMessageNotFoundException(type);
            return busTransport.Publish(eventMessage, busMessages.GetMessageTypeNameByType(type));
        }

        public Task Send<T>(T commandMessage)
        {
            if (!started) throw new InvalidOperationException("The bus has not been started.");
            var type = typeof(T);
            if (!busMessages.IsACommand(type)) throw BusMessages.CreateMessageNotFoundException(type);
            return busTransport.Send(commandMessage, busMessages.GetMessageTypeNameByType(type));
        }

        public async Task Start()
        {
            busTransport.MessageTypeNames = busMessages;
            await busTransport.InitializePublishingEndpoint();
            if (!busTransport.IsPublishAndSendOnly)
            {
                await busTransport.InitializeReceivingEndpoint();
                await busTransport.StartReceivingMessages(new MessageHandlerExecutor(dependencyResolver));
            }

            started = true;
        }

        public Task Stop()
        {
            return busTransport.StopReceivingMessages();
        }

        internal string GetMessageTypeNameByType(Type type) => busMessages.GetMessageTypeNameByType(type);
        internal Type GetTypeByMessageTypeName(string messageTypeName) => busMessages.GetTypeByMessageTypeName(messageTypeName);
    }
}
