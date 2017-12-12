using System.Threading.Tasks;

namespace RockyBus
{
    public interface IBus
    {
        Task Start();
        Task Publish<T>(T eventMessage);
        Task Publish<T>(T eventMessage, PublishOptions options);
        Task Send<T>(T commandMessage);
        Task Send<T>(T commandMessage, SendOptions options);
        Task Stop();
    }
}
