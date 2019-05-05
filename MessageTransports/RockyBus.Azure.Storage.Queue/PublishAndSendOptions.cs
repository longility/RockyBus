using System;
using System.Collections.Generic;

namespace RockyBus.Azure.Storage.Queue
{
    public class PublishAndSendOptions
    {
        private readonly IDictionary<Type, string> CommandToQueueMap = new Dictionary<Type, string>();
        private readonly List<string> availablePublishingQueues = new List<string>();
        public PublishAndSendOptions MapCommandToQueue<T>(string queue)
        {
            CommandToQueueMap.Add(typeof(T), queue);
            return this;
        }

        public PublishAndSendOptions AddAvailableQueues(params string[] queues)
        {
            availablePublishingQueues.AddRange(queues);
            return this;
        }

        public string GetQueue<T>()
        {
            var type = typeof(T);
            if (!CommandToQueueMap.ContainsKey(type)) throw new InvalidOperationException($"The command {type.FullName} is not mapped to a receiver.");
            return CommandToQueueMap[type];
        }

        public IEnumerable<string> AvailablePublishingQueues => availablePublishingQueues;
    }
}
