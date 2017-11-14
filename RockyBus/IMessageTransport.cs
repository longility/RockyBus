using System;
using System.Threading.Tasks;
using RockyBus.Message;
namespace RockyBus
{
    public interface IMessageTransport
    {
        IMessageTypeNames MessageTypeNames { get; set; }
        bool IsPublishAndSendOnly { get; }

        Task InitializePublishingEndpoint();
        Task InitializeReceivingEndpoint();

        Task Publish<T>(T message, string messageTypeName);
        Task Send<T>(T message, string messageTypeName);

        Task StartReceivingMessages(MessageHandlerExecutor messageHandlerExecutor);
        Task StopReceivingMessages();
    }
}
