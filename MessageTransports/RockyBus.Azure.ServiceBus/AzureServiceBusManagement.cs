using Microsoft.Azure.Management.ServiceBus;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (configuration.PublishAndSendOptions.SBTopic == null) return;

            var sbManagementClient = await GetServiceBusManagementClient().ConfigureAwait(false);

            await Retry.Do(() =>
                 sbManagementClient.Topics.CreateOrUpdateAsync(
                     configuration.ResourceGroupName,
                     configuration.NamespaceName,
                     topicName,
                     configuration.PublishAndSendOptions.SBTopic)).ConfigureAwait(false);
        }

        public async Task InitializeReceivingEndpoint(string topicName, IEnumerable<string> eventMessageTypeNames)
        {
            var sbManagementClient = await GetServiceBusManagementClient().ConfigureAwait(false);

            await new ReceivingEndpoint(configuration, sbManagementClient, topicName, eventMessageTypeNames).Initialize().ConfigureAwait(false);
        }

        public async Task<IServiceBusManagementClient> GetServiceBusManagementClient()
        {
            if (serviceBusManagementClient != null) return serviceBusManagementClient;
            var context = new AuthenticationContext($"https://login.microsoftonline.com/{configuration.TenantId}");

            var result = await context.AcquireTokenAsync(
                "https://management.core.windows.net/",
                new ClientCredential(configuration.ClientId, configuration.ClientSecret)
            ).ConfigureAwait(false);

            var creds = new TokenCredentials(result.AccessToken);
            return serviceBusManagementClient = new ServiceBusManagementClient(creds)
            {
                SubscriptionId = configuration.SubscriptionId
            };
        }
    }
}
