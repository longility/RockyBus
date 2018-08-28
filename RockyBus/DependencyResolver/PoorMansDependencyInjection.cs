using System;
using System.Collections.Generic;

namespace RockyBus
{
    internal class PoorMansDependencyInjection : IDependencyResolver
    {
        private readonly PoorMansResolverScope poorMansResolverScope = new PoorMansResolverScope();
        private readonly IList<Type> handlingMessageTypes = new List<Type>();

        public IResolverScope CreateScope() => poorMansResolverScope;

        public PoorMansDependencyInjection AddMessageHandler<T>(Func<IMessageHandler<T>> createMessageHandler)
        {
            poorMansResolverScope.AddMessageHandler(createMessageHandler);
            handlingMessageTypes.Add(typeof(T));
            return this;
        }

        public IEnumerable<Type> GetHandlingMessageTypes() => handlingMessageTypes;
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
