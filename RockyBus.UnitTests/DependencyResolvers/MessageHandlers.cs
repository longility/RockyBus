using System;
using System.Threading.Tasks;
using RockyBus.DemoMessages;

namespace RockyBus.UnitTests.DependencyResolvers
{
    public class AppleCommandHandler : IMessageHandler<AppleCommand>
    {
        public Task Handle(AppleCommand message, IMessageContext messageContext) => Task.CompletedTask;
    }

    public class BananaCommandHandler : BaseEventHandler<BananaCommand> 
    {
        
    }

    public class RottenAppleCommandHandler : IMessageHandler<AppleCommand>
    {
        public Task Handle(AppleCommand message, IMessageContext messageContext) => throw new Exception("Rotten Apple");
    }

    public abstract class BaseEventHandler<T> : IMessageHandler<T>
    {
        public Task Handle(T message, IMessageContext messageContext) => Task.CompletedTask;
    }
}
