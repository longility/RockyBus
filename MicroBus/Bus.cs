using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroBus.Abstractions;

namespace MicroBus
{
    public class Bus : IBus
    {
        readonly IBusTransport busTransport;

        public Bus(IBusTransport busTransport)
        {
            this.busTransport = busTransport;
        }

        public async Task PublishAsync<T>(T eventMessage)
        {
            await InitializePublishEndpointIfNotInitialized(typeof(T));
            await busTransport.PublishAsync(eventMessage);
        }

        public Task SendAsync<T>(T commandMessage)
        {
            return busTransport.SendAsync(commandMessage);
        }

        public Task StartAsync()
        {
            return busTransport.StartAsync();
        }

        public Task StopAsync()
        {
            return busTransport.StopAsync();
        }

        readonly ICollection<Type> initializedPublishEndpoints = new HashSet<Type>();

        async Task InitializePublishEndpointIfNotInitialized(Type messageType)
        {
            if (initializedPublishEndpoints.Contains(messageType)) return;

            await busTransport.CreateOrUpdatePublishEndpointAsync(messageType);
            initializedPublishEndpoints.Add(messageType);
        }
    }
}
