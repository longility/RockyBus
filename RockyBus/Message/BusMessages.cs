using System;
using System.Collections.Generic;
using System.Linq;

namespace RockyBus.Message
{
    class BusMessages : IReceivingMessageTypeNames
    {
        private readonly IDictionary<string, Type> nameToMessageTypePublishingEventMap = new Dictionary<string, Type>();
        private readonly IDictionary<Type, string> messageTypeToNamePublishingEventMap = new Dictionary<Type, string>();
        private readonly IDictionary<string, Type> nameToMessageTypeSendingCommandMap = new Dictionary<string, Type>();
        private readonly IDictionary<Type, string> messageTypeToNameSendingCommandMap = new Dictionary<Type, string>();
        private readonly IDictionary<string, Type> nameToMessageTypeReceivingEventMap = new Dictionary<string, Type>();
        private readonly IDictionary<Type, string> messageTypeToNameReceivingEventMap = new Dictionary<Type, string>();
        private readonly IDictionary<string, Type> nameToMessageTypeReceivingCommandMap = new Dictionary<string, Type>();
        private readonly IDictionary<Type, string> messageTypeToNameReceivingCommandMap = new Dictionary<Type, string>();

        public IDictionary<Type, string> MessageTypeToNamePublishingEventMap { get { return messageTypeToNamePublishingEventMap; } }
        public IDictionary<Type, string> MessageTypeToNameSendingCommandMap { get { return messageTypeToNameSendingCommandMap; } }
        public BusMessages(MessageScanRules messageScanRules, IDependencyResolver dependencyResolver)
        {
            var scanner = new MessageScanner(messageScanRules);
            scanner.Scan();

            if (!scanner.EventTypes.Any() && !scanner.CommandTypes.Any()) throw new TypeLoadException("Unable to find any messages. Properly define the message scan rules so it can be scanned properly.");

            if (dependencyResolver == null) InitializeMapsWithoutDependencyResolver(scanner);
            else InitializeMapsWithDependencyResolver(dependencyResolver, scanner);
        }

        private void InitializeMapsWithoutDependencyResolver(MessageScanner scanner)
        {
            foreach (var type in scanner.EventTypes)
            {
                nameToMessageTypePublishingEventMap.Add(type.FullName, type);
                messageTypeToNamePublishingEventMap.Add(type, type.FullName);
            }

            foreach (var type in scanner.CommandTypes)
            {
                nameToMessageTypeSendingCommandMap.Add(type.FullName, type);
                messageTypeToNameSendingCommandMap.Add(type, type.FullName);
            }
        }

        private void InitializeMapsWithDependencyResolver(IDependencyResolver dependencyResolver, MessageScanner scanner)
        {
            using (var resolver = dependencyResolver.CreateScope())
            {
                var creator = new MessageHandlerTypeCreator();
                foreach (var type in scanner.EventTypes)
                {
                    var isReceivingType = resolver.Resolve(creator.Create(type)) != null;
                    if (isReceivingType)
                    {
                        nameToMessageTypeReceivingEventMap.Add(type.FullName, type);
                        messageTypeToNameReceivingEventMap.Add(type, type.FullName);
                    }
                    else
                    {
                        nameToMessageTypePublishingEventMap.Add(type.FullName, type);
                        messageTypeToNamePublishingEventMap.Add(type, type.FullName);
                    }
                }

                foreach (var type in scanner.CommandTypes)
                {
                    var isReceivingType = resolver.Resolve(creator.Create(type)) != null;
                    if (isReceivingType)
                    {
                        nameToMessageTypeReceivingCommandMap.Add(type.FullName, type);
                        messageTypeToNameReceivingCommandMap.Add(type, type.FullName);
                    }
                    else
                    {
                        nameToMessageTypeSendingCommandMap.Add(type.FullName, type);
                        messageTypeToNameSendingCommandMap.Add(type, type.FullName);
                    }
                }
            }
        }

        public IEnumerable<string> ReceivingEventMessageTypeNames => nameToMessageTypeReceivingEventMap.Keys;

        internal bool IsPublishable(Type type) => messageTypeToNamePublishingEventMap.ContainsKey(type);
        internal bool IsSendable(Type type) => messageTypeToNameSendingCommandMap.ContainsKey(type);


        public Type GetReceivingTypeByMessageTypeName(string messageTypeName)
        {
            if (nameToMessageTypeReceivingEventMap.TryGetValue(messageTypeName, out var type)) return type;
            if (nameToMessageTypeReceivingCommandMap.TryGetValue(messageTypeName, out type)) return type;
            throw CreateMessageNotFoundException(messageTypeName);
        }

        internal static Exception CreateMessageNotFoundException(Type type) => CreateMessageNotFoundException(type.AssemblyQualifiedName);
        internal static Exception CreateMessageNotFoundException(string name) => new InvalidOperationException($"{name} is not found. Adjust the scan rules or this is an unexpected behavior");
    }
}
