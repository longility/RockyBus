using System;
using System.Collections.Generic;
using System.Linq;

namespace RockyBus.Message
{
    class BusMessages : IReceivingMessageTypeNames
    {
        private readonly MessageScanner scanner;

        public IDictionary<Type, string> MessageTypeToNamePublishingEventMap { get { return scanner.EventTypeToFullNameMap; } }
        public IDictionary<Type, string> MessageTypeToNameSendingCommandMap { get { return scanner.CommandTypeToFullNameMap; } }

        public IEnumerable<string> ReceivingEventMessageTypeNames { get; }

        public BusMessages(MessageScanRules messageScanRules, IDependencyResolver dependencyResolver)
        {
            scanner = new MessageScanner(messageScanRules);
            scanner.Scan();

            if (!scanner.EventFullNameToTypeMap.Any() && !scanner.CommandFullNameToTypeMap.Any()) throw new TypeLoadException("Unable to find any messages. Properly define the message scan rules so it can be scanned properly.");

            ReceivingEventMessageTypeNames = GetReceivingEventMessageTypeNames(dependencyResolver);
        }

        private IEnumerable<string> GetReceivingEventMessageTypeNames(IDependencyResolver dependencyResolver)
        {
            return dependencyResolver == null ?
                Enumerable.Empty<string>() :
                dependencyResolver.GetHandlingMessageTypes().Intersect(scanner.EventTypeToFullNameMap.Keys).Select(t => t.FullName).ToList();
        }

        internal bool IsPublishable(Type type) => scanner.EventTypeToFullNameMap.ContainsKey(type);
        internal bool IsSendable(Type type) => scanner.CommandTypeToFullNameMap.ContainsKey(type);

        public Type GetReceivingTypeByMessageTypeName(string messageTypeName)
        {
            if (scanner.EventFullNameToTypeMap.TryGetValue(messageTypeName, out Type eventType)) return eventType;
            if (scanner.CommandFullNameToTypeMap.TryGetValue(messageTypeName, out Type commandType)) return commandType;

            throw CreateMessageNotFoundException(messageTypeName);
        }

        internal static Exception CreateMessageNotFoundException(Type type) => CreateMessageNotFoundException(type.AssemblyQualifiedName);
        internal static Exception CreateMessageNotFoundException(string name) => new InvalidOperationException($"{name} is not found. Adjust the scan rules or this is an unexpected behavior");
    }
}
