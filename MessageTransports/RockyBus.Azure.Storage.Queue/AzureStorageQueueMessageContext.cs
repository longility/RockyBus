using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;

namespace RockyBus
{
    public class AzureStorageQueueMessageContext : IMessageContext
    {
        private readonly IBus bus;
        public AzureStorageQueueMessageContext(IBus bus, CloudQueueMessage cloudQueueMessage)
        {
            this.bus = bus;
            DeliveryCount = cloudQueueMessage.DequeueCount;
            EnqueueTime = cloudQueueMessage.InsertionTime;
            MessageHeaders = new Dictionary<string, string>();
        }

        public int? DeliveryCount { get; }

        public DateTimeOffset? EnqueueTime { get; }

        public IReadOnlyDictionary<string, string> MessageHeaders { get; }

        public Task Publish<T>(T eventMessage) => bus.Publish(eventMessage);

        public Task Send<T>(T commandMessage) => bus.Send(commandMessage);
    }
}
