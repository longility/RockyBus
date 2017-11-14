using System;
using System.Threading.Tasks;
namespace MicroBus
{
    public interface IMessageHandler<T>
    {
        Task Handle(T message, IMessageContext messageContext);
    }
}
