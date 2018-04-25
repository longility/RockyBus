using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockyBus
{
    public class AzureServiceBusMessageContext : IMessageContext
    {
        private readonly IBus bus;
        public AzureServiceBusMessageContext(IBus bus, Microsoft.Azure.ServiceBus.Message serviceBusMessage)
        {
            this.bus = bus;
            DeliveryCount = serviceBusMessage.SystemProperties.DeliveryCount;
            EnqueueTime = new DateTimeOffset(serviceBusMessage.SystemProperties.EnqueuedTimeUtc);
            MessageHeaders = serviceBusMessage.UserProperties.ToDictionary((arg) => arg.Key, (arg) => arg.Value as string);
        }

        public int? DeliveryCount { get; }

        public DateTimeOffset EnqueueTime { get; }

        public IReadOnlyDictionary<string, string> MessageHeaders { get; }

        public Task Publish<T>(T eventMessage) => bus.Publish(eventMessage);

        public Task Send<T>(T commandMessage) => bus.Send(commandMessage);
    }
}
