using System.Threading.Tasks;

namespace RockyBus
{
    public interface IBus
    {
        Task Start();
        Task Publish<T>(T eventMessage, PublishOptions options = null);
        Task Send<T>(T commandMessage, SendOptions options = null);
        Task Stop();
    }
}
