using System;
using System.Collections.Generic;

namespace MicroBus
{
    public class AzureServiceBusConfiguration
    {
        public string SubscriptionId { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string ResourceGroupName { get; set; }
        public string NamespaceName { get; set; }

        public PublishAndSendOptions PublishAndSendOptions { get; private set; } = new PublishAndSendOptions();
        public ReceiveOptions ReceiveOptions { get; private set; } = new ReceiveOptions();
    }
}
