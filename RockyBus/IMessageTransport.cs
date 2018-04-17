using System;
using System.Threading.Tasks;
using RockyBus.Message;
namespace RockyBus
{
    public interface IMessageTransport
    {
        IReceivingMessageTypeNames ReceivingMessageTypeNames { get; set; }
        bool IsPublishAndSendOnly { get; }

        IBus Bus { get; set; }
        Task InitializePublishingEndpoint();
        Task InitializeReceivingEndpoint();

        Task Publish<T>(T message, string messageTypeName, PublishOptions options = null);
        Task Send<T>(T message, string messageTypeName, SendOptions options = null);

        Task StartReceivingMessages(MessageHandlerExecutor messageHandlerExecutor);
        Task StopReceivingMessages();
    }
}
