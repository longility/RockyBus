using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MicroBus
{
    public class MessageHandlerExecutor
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly MessageHandlerTypeCreator creator = new MessageHandlerTypeCreator();
        private readonly IDictionary<Type, Type> MessageTypeToHandlerTypeMap = new Dictionary<Type, Type>();

        public MessageHandlerExecutor(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public Task Execute(object message, CancellationToken cancellationToken)
        {
            using (var scope = dependencyResolver.CreateScope())
            {
                cancellationToken.Register(scope.Dispose);
                var messageHandlerType = ToMessageHandlerType(message.GetType());
                var messageHandler = scope.Resolve(messageHandlerType);
                if (messageHandler == null) throw new TypeAccessException($"The type {messageHandlerType.Name} is not registered to the container");
                var method = messageHandlerType.GetMethod("Handle");
                return method.Invoke(messageHandler, new[] { message }) as Task;
            }
        }

        private Type ToMessageHandlerType(Type messageType)
        {
            if (MessageTypeToHandlerTypeMap.TryGetValue(messageType, out var type)) return type;

            MessageTypeToHandlerTypeMap.Add(messageType, type = creator.Create(messageType));
            return type;
        }
    }
}
