using System;
using System.Collections.Generic;
using RockyBus.Azure.Storage.Queue;

namespace RockyBus
{
    public class AzureStorageQueueConfiguration
    {
        public PublishAndSendOptions PublishAndSendOptions { get; } = new PublishAndSendOptions();
        public ReceiveOptions ReceiveOptions { get; } = new ReceiveOptions();
    }
}
