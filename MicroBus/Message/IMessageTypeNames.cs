using System;
using System.Collections.Generic;

namespace MicroBus.Message
{
    public interface IMessageTypeNames
    {
        IEnumerable<string> EventMessageTypeNames { get; }
        IEnumerable<string> CommandMessageTypeNames { get; }
        Type GetTypeByMessageTypeName(string messageTypeName);
    }
}
