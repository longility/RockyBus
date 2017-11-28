using System;
using System.Threading.Tasks;
using RockyBus.Message;
using RockyBus.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Newtonsoft.Json;

using ServiceBusMessage = Microsoft.Azure.ServiceBus.Message;

namespace RockyBus
{
    internal class AzureServiceBusTransport : IMessageTransport
    {
        private const string TopicName = "RockyBus";
        private readonly string connectionString;
        private readonly AzureServiceBusManagement azureServiceBusManagement;
        private readonly AzureServiceBusConfiguration configuration = new AzureServiceBusConfiguration { };
        private SubscriptionClient subscriptionClient;
        private TopicClient topicClient;

        public IReceivingMessageTypeNames ReceivingMessageTypeNames { get; set; }

        public bool IsPublishAndSendOnly => string.IsNullOrWhiteSpace(configuration.ReceiveOptions.QueueName);

        public AzureServiceBusTransport(string connectionString, Action<AzureServiceBusConfiguration> configuration)
        {
            this.connectionString = connectionString;
            configuration(this.configuration);
            this.azureServiceBusManagement = new AzureServiceBusManagement(this.configuration);
        }

        public async Task InitializePublishingEndpoint()
        {
            await azureServiceBusManagement.InitializePublishingEndpoint(TopicName);

            topicClient = new TopicClient(connectionString, TopicName);
        }

        public Task InitializeReceivingEndpoint() => azureServiceBusManagement.InitializeReceivingEndpoint(TopicName, ReceivingMessageTypeNames.ReceivingEventMessageTypeNames);

        public Task Publish<T>(T message, string messageTypeName) => SendToTopic(message, messageTypeName);
        public Task Send<T>(T message, string messageTypeName) => SendToTopic(message, messageTypeName, true);

        private Task SendToTopic<T>(T message, string messageTypeName, bool isCommand = false)
        {
            var serviceBusMessage = new ServiceBusMessage { Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)) };
            serviceBusMessage.UserProperties.Add(UserProperties.MessageTypeKey, messageTypeName);
            if (isCommand) serviceBusMessage.UserProperties.Add(UserProperties.DestinationQueueKey, configuration.PublishAndSendOptions.GetQueue<T>());
            return topicClient.SendAsync(serviceBusMessage);
        }

        public Task StartReceivingMessages(MessageHandlerExecutor messageHandlerExecutor)
        {
            subscriptionClient = new SubscriptionClient(connectionString, TopicName, configuration.ReceiveOptions.QueueName);
            subscriptionClient.RegisterMessageHandler(
                (message, cancellationToken) => messageHandlerExecutor.Execute(GetMessageBody(message, ReceivingMessageTypeNames), new AzureServiceBusMessageContext(message), cancellationToken),
                new MessageHandlerOptions(_ => Task.CompletedTask)
                { });

            return Task.CompletedTask;

            object GetMessageBody(ServiceBusMessage message, IReceivingMessageTypeNames messageTypeNames)
            {
                var messageTypeName = message.UserProperties[UserProperties.MessageTypeKey].ToString();
                var type = messageTypeNames.GetReceivingTypeByMessageTypeName(messageTypeName);
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), type);
            }
        }

        public Task StopReceivingMessages() => subscriptionClient?.CloseAsync() ?? Task.CompletedTask;
    }
}
