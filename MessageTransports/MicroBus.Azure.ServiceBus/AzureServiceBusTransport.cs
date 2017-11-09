using System;
using System.Threading.Tasks;
using MicroBus.Azure.ServiceBus;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace MicroBus
{
    internal class AzureServiceBusTransport : IMessageTransport
    {
        private const string TopicName = "MicroBus";
        private readonly string connectionString;
        private MessageHandlerExecutor messageHandlerExecutor;
        private readonly AzureServiceBusConfiguration configuration = new AzureServiceBusConfiguration { };
        private TopicClient topicClient;
        private SubscriptionClient subscriptionClient;
        private string[] handlingEventMessageTypes;

        public AzureServiceBusTransport(string connectionString, Action<AzureServiceBusConfiguration> configuration)
        {
            this.connectionString = connectionString;
            configuration(this.configuration);
        }

        public async Task CreateOrUpdatePublishEndpointAsync(Type messageType)
        {
            var context = new AuthenticationContext($"https://login.microsoftonline.com/{configuration.TenantId}");

            var result = await context.AcquireTokenAsync(
                "https://management.core.windows.net/",
                new ClientCredential(configuration.ClientId, configuration.ClientSecret)
            );

            var creds = new TokenCredentials(result.AccessToken);
            var sbClient = new ServiceBusManagementClient(creds)
            {
                SubscriptionId = configuration.SubscriptionId
            };

            var serviceBusTopic = new SBTopic { };

            await sbClient.Topics.CreateOrUpdateAsync(configuration.ResourceGroupName, configuration.NamespaceName, TopicName, serviceBusTopic);
        }

        public Task PublishAsync<T>(T eventMessage) => SendMessageAsync(eventMessage);

        public Task SendAsync<T>(T commandMessage) => SendMessageAsync(
            commandMessage,
            configuration.PublishAndSendOptions.GetQueue<T>());

        private Task SendMessageAsync<T>(T message, string queue = null)
        {
            if (topicClient == null) topicClient = new TopicClient(connectionString, TopicName);


            return topicClient.SendAsync(new Message().SetMessageBody(message).SetDestinationQueue(queue));
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrWhiteSpace(configuration.ReceiveOptions.QueueName)) throw new InvalidOperationException("If this bus does not need to receive, starting and stopping the bus is unnecessary. If this bus needs to receive, the receive options need to be configured.");

            await CreateOrUpdatePublishEndpointAsync(null);
            await CreateOrUpdateReceivingEndpointAsync();
            //only start if there is a consumer
            //if (subscriptionClient == null) subscriptionClient = new SubscriptionClient(connectionString, TopicName, "receivername");
            subscriptionClient.RegisterMessageHandler(
                (message, cancellationToken) => messageHandlerExecutor.Execute(message.GetMessageBody(), cancellationToken),
                    new MessageHandlerOptions((arg) => { return Task.CompletedTask; }) { });
        }

        public Task StopAsync() => subscriptionClient?.CloseAsync() ?? Task.CompletedTask;

        public async Task CreateOrUpdateReceivingEndpointAsync()
        {
            var events = handlingEventMessageTypes;

            var context = new AuthenticationContext($"https://login.microsoftonline.com/{configuration.TenantId}");

            var result = await context.AcquireTokenAsync(
                "https://management.core.windows.net/",
                new ClientCredential(configuration.ClientId, configuration.ClientSecret)
            );

            var creds = new TokenCredentials(result.AccessToken);
            var sbClient = new ServiceBusManagementClient(creds)
            {
                SubscriptionId = configuration.SubscriptionId
            };

            var serviceBusSubscription = new SBSubscription { };

            await sbClient.Subscriptions.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                TopicName,
                configuration.ReceiveOptions.QueueName,
                serviceBusSubscription);

            var eventMessageFilter = new Rule
            {
                SqlFilter = new Microsoft.Azure.Management.ServiceBus.Models.SqlFilter(
                    $"user.{UserProperties.MessageTypeKey} IN ({string.Join(",", events)})")
            };

            await sbClient.Rules.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                TopicName,
                configuration.ReceiveOptions.QueueName,
                nameof(eventMessageFilter),
                eventMessageFilter);

            var commandMessageFilter = new Rule
            {
                SqlFilter = new Microsoft.Azure.Management.ServiceBus.Models.SqlFilter(
                    $"user.{UserProperties.DestinationQueueKey}='{configuration.ReceiveOptions.QueueName}'")
            };
            await sbClient.Rules.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                TopicName,
                configuration.ReceiveOptions.QueueName,
                nameof(commandMessageFilter),
                commandMessageFilter);
        }

        public void SetMessageHandlerExecutor(MessageHandlerExecutor messageHandlerExecutor) => this.messageHandlerExecutor = messageHandlerExecutor;
        public void SetHandlingEventMessageTypes(string[] handlingEventMessageTypes) => this.handlingEventMessageTypes = handlingEventMessageTypes;
    }
}
