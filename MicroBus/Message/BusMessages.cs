using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroBus.Message
{
    class BusMessages : IMessageTypeNames
    {
        private readonly IDictionary<string, Type> nameToMessageTypeEventMap;
        private readonly IDictionary<Type, string> messageTypeToNameEventMap;
        private readonly IDictionary<string, Type> nameToMessageTypeCommandMap;
        private readonly IDictionary<Type, string> messageTypeToNameCommandMap;

        public BusMessages(MessageScanRules messageScanRules)
        {
            var scanner = new MessageScanner(messageScanRules);
            scanner.Scan();

            var eventTypes = scanner.EventTypes;
            nameToMessageTypeEventMap = eventTypes.ToDictionary(e => e.FullName);
            messageTypeToNameEventMap = eventTypes.ToDictionary(e => e, e => e.FullName);

            var commandTypes = scanner.CommandTypes;
            nameToMessageTypeCommandMap = commandTypes.ToDictionary(e => e.FullName);
            messageTypeToNameCommandMap = commandTypes.ToDictionary(e => e, e => e.FullName);
        }

        public IEnumerable<string> EventMessageTypeNames => nameToMessageTypeEventMap.Keys;
        public IEnumerable<string> CommandMessageTypeNames => nameToMessageTypeCommandMap.Keys;

        internal bool IsAnEvent(Type type) => messageTypeToNameEventMap.ContainsKey(type);
        internal bool IsACommand(Type type) => messageTypeToNameCommandMap.ContainsKey(type);


        public Type GetTypeByMessageTypeName(string messageTypeName)
        {
            if (nameToMessageTypeEventMap.TryGetValue(messageTypeName, out var type)) return type;
            if (nameToMessageTypeCommandMap.TryGetValue(messageTypeName, out type)) return type;
            throw CreateMessageNotFoundException(messageTypeName);
        }

        internal string GetMessageTypeNameByType(Type messageType)
        {
            if (messageTypeToNameEventMap.TryGetValue(messageType, out var name)) return name;
            if (messageTypeToNameCommandMap.TryGetValue(messageType, out name)) return name;
            throw CreateMessageNotFoundException(messageType);
        }

        internal static Exception CreateMessageNotFoundException(Type type) => CreateMessageNotFoundException(type.AssemblyQualifiedName);
        internal static Exception CreateMessageNotFoundException(string name) => new InvalidOperationException($"{name} is not found. Adjust the scan rules or this is an unexpected behavior");
    }
}
