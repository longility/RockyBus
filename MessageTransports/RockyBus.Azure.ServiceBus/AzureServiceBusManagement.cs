using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace RockyBus.Azure.ServiceBus
{
    class AzureServiceBusManagement
    {
        private IServiceBusManagementClient serviceBusManagementClient;
        private readonly AzureServiceBusConfiguration configuration;

        public AzureServiceBusManagement(AzureServiceBusConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task InitializePublishingEndpoint(string topicName)
        {
            var sbManagementClient = await GetServiceBusManagementClient();

            if (configuration.PublishAndSendOptions.SBTopic == null)
            {
                var topic = await sbManagementClient.Topics.GetAsync(configuration.ResourceGroupName, configuration.NamespaceName, topicName);
                if (topic != null) return;
            }

            await sbManagementClient.Topics.CreateOrUpdateAsync(
                 configuration.ResourceGroupName,
                 configuration.NamespaceName,
                 topicName,
                 configuration.PublishAndSendOptions.SBTopic ?? new SBTopic());
        }

        public async Task InitializeReceivingEndpoint(string topicName, IEnumerable<string> eventMessageTypeNames)
        {
            var sbManagementClient = await GetServiceBusManagementClient();

            await sbManagementClient.Subscriptions.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                topicName,
                configuration.ReceiveOptions.QueueName,
                configuration.ReceiveOptions.SBSubscription ?? new SBSubscription { });

            var eventMessageFilter = new Rule
            {
                SqlFilter = new SqlFilter(
                    $"user.{UserProperties.MessageTypeKey} IN ({string.Join(",", eventMessageTypeNames.Select(n => $"'{n}'"))})")
            };

            var commandMessageFilter = new Rule
            {
                SqlFilter = new SqlFilter(
                    $"user.{UserProperties.DestinationQueueKey}='{configuration.ReceiveOptions.QueueName}'")
            };

            await Task.WhenAll(
                CreateOrUpdateRule(nameof(eventMessageFilter), eventMessageFilter),
                CreateOrUpdateRule(nameof(commandMessageFilter), commandMessageFilter));

            Task CreateOrUpdateRule(string filterName, Rule rule) =>
                sbManagementClient.Rules.CreateOrUpdateAsync(
                configuration.ResourceGroupName,
                configuration.NamespaceName,
                topicName,
                configuration.ReceiveOptions.QueueName,
                filterName,
                rule);
        }

        public async Task<IServiceBusManagementClient> GetServiceBusManagementClient()
        {
            if (serviceBusManagementClient != null) return serviceBusManagementClient;
            var context = new AuthenticationContext($"https://login.microsoftonline.com/{configuration.TenantId}");

            var result = await context.AcquireTokenAsync(
                "https://management.core.windows.net/",
                new ClientCredential(configuration.ClientId, configuration.ClientSecret)
            );

            var creds = new TokenCredentials(result.AccessToken);
            return serviceBusManagementClient = new ServiceBusManagementClient(creds)
            {
                SubscriptionId = configuration.SubscriptionId
            };
        }
    }
}
