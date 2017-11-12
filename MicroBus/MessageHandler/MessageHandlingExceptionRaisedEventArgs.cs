using System;
namespace MicroBus
{
    public class MessageHandlingExceptionRaisedEventArgs
    {
        public Type MessageHandlerType { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
