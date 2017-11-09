using System;
using System.Collections.Generic;
using Microsoft.Azure.Management.ServiceBus.Models;

namespace MicroBus
{
    public class PublishAndSendOptions
    {
        private readonly IDictionary<Type, string> CommandToQueueMap = new Dictionary<Type, string>();

        /// <summary>
        /// Set this to create the service bus topic for publish and sending.
        /// Service Bus topic name, MicroBus, should be created manually,
        /// there should be a single owner to avoid inconsistent creation,
        /// or there should be a shared way to creating the service bus topic.
        /// </summary>
        /// <value>The SBTopic.</value>
        public SBTopic SBTopic { get; set; }

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
    }
}
