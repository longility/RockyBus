using System.Threading.Tasks;

namespace MicroBus.Abstractions
{
    public interface IBus
    {
        Task StartAsync();
        Task PublishAsync<T>(T eventMessage);
        Task SendAsync<T>(T commandMessage);
        Task StopAsync();
    }
}
