using System;
using Microsoft.Azure.Management.ServiceBus.Models;

namespace RockyBus
{
    public class ReceiveOptions
    {
        public string QueueName { get; set; }
        public SBSubscription SBSubscription { get; set; }
        public SBQueue SBQueue { get; set; }
        public int MaxConcurrentCalls { get; set; } = 1;
        public TimeSpan MaxAutoRenewDuration { get; set; } = TimeSpan.FromMinutes(5);
    }
}
