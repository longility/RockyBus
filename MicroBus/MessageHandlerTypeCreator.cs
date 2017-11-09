using System;
using System.Collections.Generic;

namespace MicroBus
{
    internal class MessageHandlerTypeCreator
    {
        private static readonly Type MessageHandlerType = typeof(IMessageHandler<>);
        public Type Create(Type type) => MessageHandlerType.MakeGenericType(type);
    }
}
