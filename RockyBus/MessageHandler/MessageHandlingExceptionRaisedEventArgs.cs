using System;
namespace RockyBus
{
    public class MessageHandlingExceptionRaisedEventArgs
    {
        public Type MessageHandlerType { get; internal set; }
        public object Message { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
