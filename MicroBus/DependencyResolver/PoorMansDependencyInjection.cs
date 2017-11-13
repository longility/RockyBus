using System;
using System.Collections.Generic;

namespace MicroBus
{
    internal class PoorMansDependencyInjection : IDependencyResolver
    {
        private readonly PoorMansResolverScope poorMansResolverScope = new PoorMansResolverScope();

        public IResolverScope CreateScope() => poorMansResolverScope;

        public void AddMessageHandler<T>(Func<IMessageHandler<T>> createMessageHandler) => poorMansResolverScope.AddMessageHandler(createMessageHandler);
    }

    internal class PoorMansResolverScope : IResolverScope
    {
        public IDictionary<Type, Func<object>> typeToMessageHandlerCreator = new Dictionary<Type, Func<object>>();
        public void Dispose() { }

        public object Resolve(Type objectType)
        {
            return typeToMessageHandlerCreator.TryGetValue(objectType.GenericTypeArguments[0], out var createMessageHandler) ? createMessageHandler() : null;
        }

        public void AddMessageHandler<T>(Func<IMessageHandler<T>> createMessageHandler) => typeToMessageHandlerCreator.Add(typeof(T), createMessageHandler);
    }
}
