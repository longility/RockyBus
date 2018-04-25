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
        private QueueClient queueClient;
        private TopicClient topicClient;

        public IReceivingMessageTypeNames ReceivingMessageTypeNames { get; set; }

        public bool IsPublishAndSendOnly => string.IsNullOrWhiteSpace(configuration.ReceiveOptions.QueueName);

        public IBus Bus { get; set; }

        public AzureServiceBusTransport(string connectionString, Action<AzureServiceBusConfiguration> configuration)
        {
            this.connectionString = connectionString;
            configuration(this.configuration);
            this.azureServiceBusManagement = new AzureServiceBusManagement(this.configuration);
        }

        public async Task InitializePublishingEndpoint()
        {
            await azureServiceBusManagement.InitializePublishingEndpoint(TopicName).ConfigureAwait(false);

            topicClient = new TopicClient(connectionString, TopicName);
        }

        public Task InitializeReceivingEndpoint() => azureServiceBusManagement.InitializeReceivingEndpoint(TopicName, ReceivingMessageTypeNames.ReceivingEventMessageTypeNames);

        public Task Publish<T>(T message, string messageTypeName, PublishOptions publishOptions = null) => SendToTopic(message, messageTypeName, publishOptions);

        public Task Send<T>(T message, string messageTypeName, SendOptions sendOptions = null) => SendToTopic(message, messageTypeName, sendOptions, true);

        private Task SendToTopic<T>(T message, string messageTypeName, Options options = null, bool isCommand = false)
        {
            var serviceBusMessage = new ServiceBusMessage { Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)) };

            serviceBusMessage.UserProperties.Add(UserProperties.MessageTypeKey, messageTypeName);

            if (options != null)
            {
                foreach (var item in options.GetHeaders())
                {
                    serviceBusMessage.UserProperties.Add(item.Key, item.Value);
                }
            }

            if (isCommand)
            {
                serviceBusMessage.UserProperties.Add(UserProperties.DestinationQueueKey, configuration.PublishAndSendOptions.GetQueue<T>());
            }

            return topicClient.SendAsync(serviceBusMessage);
        }

        public Task StartReceivingMessages(MessageHandlerExecutor messageHandlerExecutor)
        {
            queueClient = new QueueClient(connectionString, configuration.ReceiveOptions.QueueName);
            queueClient.RegisterMessageHandler(
                (message, cancellationToken) => messageHandlerExecutor.Execute(GetMessageBody(message, ReceivingMessageTypeNames), new AzureServiceBusMessageContext(Bus, message), cancellationToken),
                new MessageHandlerOptions(_ => Task.CompletedTask)
                { MaxConcurrentCalls = configuration.ReceiveOptions.MaxConcurrentCalls });

            return Task.CompletedTask;

            object GetMessageBody(ServiceBusMessage message, IReceivingMessageTypeNames messageTypeNames)
            {
                var messageTypeName = message.UserProperties[UserProperties.MessageTypeKey].ToString();
                var type = messageTypeNames.GetReceivingTypeByMessageTypeName(messageTypeName);
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), type);
            }
        }

        public Task StopReceivingMessages() => queueClient?.CloseAsync() ?? Task.CompletedTask;
    }
}
