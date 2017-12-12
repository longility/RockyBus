using System;
using System.Threading.Tasks;
using RockyBus.Message;
namespace RockyBus
{
    public interface IMessageTransport
    {
        IReceivingMessageTypeNames ReceivingMessageTypeNames { get; set; }
        bool IsPublishAndSendOnly { get; }

        Task InitializePublishingEndpoint();
        Task InitializeReceivingEndpoint();

        Task Publish<T>(T message, string messageTypeName);
        Task Publish<T>(T message, string messageTypeName, PublishOptions options);
        Task Send<T>(T message, string messageTypeName);
        Task Send<T>(T message, string messageTypeName, SendOptions options);

        Task StartReceivingMessages(MessageHandlerExecutor messageHandlerExecutor);
        Task StopReceivingMessages();
    }
}
