using System;
using System.Threading.Tasks;
namespace MicroBus
{
    public interface IMessageContext
    {
        int? DeliveryCount { get; }
        DateTimeOffset EnqueueTime { get; }
    }
}
