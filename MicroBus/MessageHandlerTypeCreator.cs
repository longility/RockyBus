using System;
using System.Collections.Generic;

namespace MicroBus
{
    internal class MessageHandlerTypeCreator
    {
        private readonly IDictionary<Type, Type> MessageTypeToHandlerTypeMap = new Dictionary<Type, Type>();
        private static readonly Type MessageHandlerType = typeof(IMessageHandler<>);
        public Type Create(Type messageType)
        {
            if (MessageTypeToHandlerTypeMap.TryGetValue(messageType, out var handlerType)) return handlerType;

            MessageTypeToHandlerTypeMap.Add(messageType, handlerType = MessageHandlerType.MakeGenericType(messageType));
            return handlerType;
        }
    }
}
