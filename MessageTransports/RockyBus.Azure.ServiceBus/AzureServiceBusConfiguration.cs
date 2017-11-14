using System;
using System.Collections.Generic;

namespace RockyBus
{
    public class AzureServiceBusConfiguration
    {
        public void SetManagementSettings(string subscriptionId, string tenantId, string clientId, string clientSecret, string resourceGroupName, string namespaceName) 
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ResourceGroupName = resourceGroupName;
            NamespaceName = namespaceName;
        }

        internal string SubscriptionId { get; private set; }
        internal string TenantId { get; private set; }
        internal string ClientId { get; private set; }
        internal string ClientSecret { get; private set; }
		
        internal string ResourceGroupName { get; private set; }
        internal string NamespaceName { get; private set; }
        public PublishAndSendOptions PublishAndSendOptions { get; } = new PublishAndSendOptions();
        public ReceiveOptions ReceiveOptions { get; } = new ReceiveOptions();
    }
}
