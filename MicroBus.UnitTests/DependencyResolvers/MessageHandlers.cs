using System;
using System.Threading.Tasks;

namespace MicroBus.UnitTests.DependencyResolvers
{
    public class AppleEventHandler : IMessageHandler<AppleEvent>
    {
        public Task Handle(AppleEvent message) => Task.CompletedTask;
    }

    public class BananaEventHandler : BaseEventHandler<BananaEvent> 
    {
        
    }

    public abstract class BaseEventHandler<T> : IMessageHandler<T>
    {
        public Task Handle(T message) => Task.CompletedTask;
    }
}
