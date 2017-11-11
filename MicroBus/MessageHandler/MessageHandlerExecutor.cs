using System;
using System.Threading;
using System.Threading.Tasks;

namespace MicroBus
{
    public class MessageHandlerExecutor
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly MessageHandlerTypeCreator creator = new MessageHandlerTypeCreator();

        public MessageHandlerExecutor(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public Task Execute(object message, CancellationToken cancellationToken)
        {
            using (var scope = dependencyResolver.CreateScope())
            {
                cancellationToken.Register(scope.Dispose);
                var messageHandlerType = creator.Create(message.GetType());
                var messageHandler = scope.Resolve(messageHandlerType);
                if (messageHandler == null) throw new TypeAccessException($"The type {messageHandlerType.Name} is not registered to the container");
                var method = messageHandlerType.GetMethod("Handle");
                return method.Invoke(messageHandler, new[] { message }) as Task;
            }
        }
    }
}
