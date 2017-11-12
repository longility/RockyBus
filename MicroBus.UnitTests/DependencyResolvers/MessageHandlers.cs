using System;
using System.Threading.Tasks;
using MicroBus.DemoMessages;

namespace MicroBus.UnitTests.DependencyResolvers
{
    public class AppleCommandHandler : IMessageHandler<AppleCommand>
    {
        public Task Handle(AppleCommand message) => Task.CompletedTask;
    }

    public class BananaCommandHandler : BaseEventHandler<BananaCommand> 
    {
        
    }

    public class RottenAppleCommandHandler : IMessageHandler<AppleCommand>
    {
        public Task Handle(AppleCommand message) => throw new Exception("Rotten Apple");
    }

    public abstract class BaseEventHandler<T> : IMessageHandler<T>
    {
        public Task Handle(T message) => Task.CompletedTask;
    }
}
