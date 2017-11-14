using System;
using System.Threading.Tasks;
namespace RockyBus
{
    public interface IMessageHandler<T>
    {
        Task Handle(T message, IMessageContext messageContext);
    }
}
