using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace MicroBus
{
    internal class AzureServiceBusTransport : IMessageTransport
    {
        private const string TopicName = "MicroBus";
        private readonly string connectionString;
        private MessageHandlerExecutor messageHandlerExecutor;
        private readonly AzureServiceBusConfiguration configuration = new AzureServiceBusConfiguration
        {

        };

        private TopicClient topicClient;
        private SubscriptionClient subscriptionClient;
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


            return topicClient.SendAsync(message.SerializeMessage());
        }

        public async Task StartAsync()
        {
            await CreateOrUpdatePublishEndpointAsync(null);
            await CreateOrUpdateReceivingEndpointAsync();
            //only start if there is a consumer
            //if (subscriptionClient == null) subscriptionClient = new SubscriptionClient(connectionString, TopicName, "receivername");
            subscriptionClient.RegisterMessageHandler(
                (message, cancellationToken) => messageHandlerExecutor.Execute(message.DeserializeMessage(), cancellationToken),
                    new MessageHandlerOptions((arg) => { return Task.CompletedTask; }) { });
        }

        public Task StopAsync()
        {
            //only stop if there is a consumer
            throw new NotImplementedException();
        }

        public async Task CreateOrUpdateReceivingEndpointAsync()
        {
            var events = new[] { "MicroBus.DemoMessages.TestEventMessage" }.Select(e => $"'{e}'").ToArray();

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

            var eventFilter = new Rule
            {
                SqlFilter = new Microsoft.Azure.Management.ServiceBus.Models.SqlFilter(
                    $"user.MessageType IN ({string.Join(",", events)})")
            };

            await sbClient.Rules.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                TopicName,
                configuration.ReceiveOptions.QueueName,
                "eventFilter",
                eventFilter);

            var receiverFilter = new Rule
            {
                SqlFilter = new Microsoft.Azure.Management.ServiceBus.Models.SqlFilter(
                    $"user.Queue='{configuration.ReceiveOptions.QueueName}'")
            };
            await sbClient.Rules.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                TopicName,
                configuration.ReceiveOptions.QueueName,
                "receiverFilter",
                receiverFilter);
        }

        public void SetMessageHandlerExecutor(MessageHandlerExecutor messageHandlerExecutor)
        {
            this.messageHandlerExecutor = messageHandlerExecutor;
        }
    }
}
