using System;
namespace MicroBus
{
    public class AzureServiceBusMessageContext : IMessageContext
    {
        public AzureServiceBusMessageContext(Microsoft.Azure.ServiceBus.Message serviceBusMessage)
        {
            DeliveryCount = serviceBusMessage.SystemProperties.DeliveryCount;
            EnqueueTime = new DateTimeOffset(serviceBusMessage.SystemProperties.EnqueuedTimeUtc);
        }

        public int? DeliveryCount { get; }

        public DateTimeOffset EnqueueTime { get; }
    }
}
