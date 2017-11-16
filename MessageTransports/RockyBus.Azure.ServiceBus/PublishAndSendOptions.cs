using System;
using System.Collections.Generic;
using Microsoft.Azure.Management.ServiceBus.Models;

namespace RockyBus
{
    public class PublishAndSendOptions
    {
        private readonly IDictionary<Type, string> CommandToQueueMap = new Dictionary<Type, string>();
        internal SBTopic SBTopic { get; private set; }

        public PublishAndSendOptions MapCommandToQueue<T>(string queue)
        {
            CommandToQueueMap.Add(typeof(T), queue);
            return this;
        }

        public string GetQueue<T>()
        {
            var type = typeof(T);
            if (!CommandToQueueMap.ContainsKey(type)) throw new InvalidOperationException($"The command {type.FullName} is not mapped to a receiver.");
            return CommandToQueueMap[type];
        }

        /// <summary>
        /// Set this to create if the service bus topic does not exist for publish and sending.
        /// Service Bus topic name, RockyBus, should be created manually,
        /// there should be a single owner to avoid inconsistent creation,
        /// or there should be a shared way to creating the service bus topic. This is still questionable based on fragileness of topic creation.
        /// </summary>
        public PublishAndSendOptions AttemptCreateTopicWith(SBTopic sbTopic)
        {
            SBTopic = sbTopic;
            return this;
        }
    }
}
