using System;
using System.Threading.Tasks;
namespace RockyBus
{
    public interface IMessageContext
    {
        int? DeliveryCount { get; }
        DateTimeOffset EnqueueTime { get; }
        Task Publish<T>(T eventMessage);
        Task Send<T>(T commandMessage);
    }
}
