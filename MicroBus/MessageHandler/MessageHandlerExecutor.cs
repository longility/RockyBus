using System;
using System.Threading;
using System.Threading.Tasks;

namespace MicroBus
{
    public class MessageHandlerExecutor
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly MessageHandlerTypeCreator creator = new MessageHandlerTypeCreator();
        private readonly Func<MessageHandlingExceptionRaisedEventArgs, Task> messageHandlingExceptionHandler;

        public MessageHandlerExecutor(IDependencyResolver dependencyResolver, Func<MessageHandlingExceptionRaisedEventArgs, Task> messageHandlingExceptionHandler = null)
        {
            this.dependencyResolver = dependencyResolver;
            this.messageHandlingExceptionHandler = messageHandlingExceptionHandler ?? (_ => Task.CompletedTask);
        }

        public async Task Execute(object message, IMessageContext messageContext, CancellationToken cancellationToken)
        {
            using (var scope = dependencyResolver.CreateScope())
            {
                cancellationToken.Register(scope.Dispose);
                var messageHandlerType = creator.Create(message.GetType());
                var messageHandler = scope.Resolve(messageHandlerType);
                if (messageHandler == null) throw new TypeAccessException($"The type {messageHandlerType.Name} is not registered to the container");
                var method = messageHandlerType.GetMethod("Handle");
                try
                {
                    await (method.Invoke(messageHandler, new[] { message, messageContext }) as Task);
                }
                catch(Exception e){
                    await messageHandlingExceptionHandler(new MessageHandlingExceptionRaisedEventArgs
                    {
                        MessageHandlerType = messageHandlerType,
                        Exception = e.InnerException
                    });
                    throw;
                }
            }
        }

    }
}
