using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroBus
{
    public class MessageTypeTranslator
    {
        private readonly IDictionary<string, Type> fullNameToMessageTypeMap = new Dictionary<string, Type>();

        private readonly ICollection<Assembly> favoredAssemblies = new List<Assembly>();
        public Type TranslateFromNameToType(string fullName)
        {
            if (fullNameToMessageTypeMap.TryGetValue(fullName, out var messageType)) return messageType;
            foreach (var assembly in favoredAssemblies)
            {
                var type = assembly.GetType(fullName);
                if (type != null)
                {
                    fullNameToMessageTypeMap.Add(fullName, type);
                    return type;
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Except(favoredAssemblies))
            {
                var type = assembly.GetType(fullName);
                if (type != null)
                {
                    favoredAssemblies.Add(assembly);
                    fullNameToMessageTypeMap.Add(fullName, type);
                    return type;
                }
            }

            throw new NotSupportedException($"Cannot find type {fullName} in any assemblies to deserialize.");
        }

        public static string TranslateFromTypeToName<T>() => typeof(T).FullName;
    }
}
