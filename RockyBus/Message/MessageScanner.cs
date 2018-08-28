using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RockyBus.Message
{
    internal class MessageScanner
    {
        private readonly MessageScanRules rules;

        public MessageScanner(MessageScanRules rules)
        {
            this.rules = rules;
        }

        public IDictionary<string, Type> EventFullNameToTypeMap { get; } = new Dictionary<string, Type>();
        public IDictionary<Type, string> EventTypeToFullNameMap { get; } = new Dictionary<Type, string>();
        public IDictionary<string, Type> CommandFullNameToTypeMap { get; } = new Dictionary<string, Type>();
        public IDictionary<Type, string> CommandTypeToFullNameMap { get; } = new Dictionary<Type, string>();

        public void Scan()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic).Except(new[] { Assembly.GetExecutingAssembly() }).ToList();
            foreach (var assembly in assemblies)
            {
                var exportedTypes = new Type[0];
                try
                {
                    exportedTypes = assembly.GetExportedTypes();
                }
                catch { }

                foreach (var type in exportedTypes)
                {
                    if (rules.IsAnEvent(type))
                    {
                        EventFullNameToTypeMap.Add(type.FullName, type);
                        EventTypeToFullNameMap.Add(type, type.FullName);
                    }
                    else if (rules.IsACommand(type))
                    {
                        CommandFullNameToTypeMap.Add(type.FullName, type);
                        CommandTypeToFullNameMap.Add(type, type.FullName);
                    }
                }
            }
        }


    }
}
