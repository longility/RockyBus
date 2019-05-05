using System;

namespace RockyBus.Azure.Storage.Queue
{
    public class ReceiveOptions
    {
        public string QueueName { get; set; }
        public int MaxDequeueCount { get; set; } = 10;
        public TimeSpan MaxAutoRenewDuration { get; set; } = TimeSpan.FromMinutes(5);
    }
}
