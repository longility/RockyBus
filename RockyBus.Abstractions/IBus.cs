using System.Threading.Tasks;

namespace RockyBus.Abstractions
{
    public interface IBus
    {
        Task Start();
        Task Publish<T>(T eventMessage);
        Task Send<T>(T commandMessage);
        Task Stop();
    }
}
