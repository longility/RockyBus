using System;
using System.Collections.Generic;

namespace RockyBus.Message
{
    public interface IReceivingMessageTypeNames
    {
        IEnumerable<string> ReceivingEventMessageTypeNames { get; }
        Type GetReceivingTypeByMessageTypeName(string receivingMessageTypeName);
    }
}
