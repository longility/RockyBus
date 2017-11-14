using System;
using Microsoft.Azure.Management.ServiceBus.Models;

namespace RockyBus
{
    public class ReceiveOptions
    {
        public string QueueName { get; set; }
        public SBSubscription SBSubscription { get; set; }
    }
}
