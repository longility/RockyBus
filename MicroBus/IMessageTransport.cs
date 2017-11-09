using System;
using System.Threading.Tasks;

namespace MicroBus
{
    public interface IMessageTransport
    {
        Task StartAsync();
        Task PublishAsync<T>(T eventMessage);
        Task CreateOrUpdatePublishEndpointAsync(Type messageType);
        Task SendAsync<T>(T commandMessage);
        Task StopAsync();
        void SetMessageHandlerExecutor(MessageHandlerExecutor messageHandlerExecutor);
    }
}
