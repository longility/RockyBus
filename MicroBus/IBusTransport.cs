using System;
using System.Threading.Tasks;

namespace MicroBus
{
    public interface IBusTransport
    {
        Task StartAsync();
        Task PublishAsync<T>(T eventMessage);
        Task CreateOrUpdatePublishEndpointAsync(Type messageType);
        Task SendAsync<T>(T commandMessage);
        Task StopAsync();
    }
}
