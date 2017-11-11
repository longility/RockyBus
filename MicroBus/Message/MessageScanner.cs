using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroBus.Message
{
    internal class MessageScanner
    {
        private readonly MessageScanRules rules;
        private readonly ICollection<Type> eventTypes = new List<Type>();
        private readonly ICollection<Type> commandTypes = new List<Type>();

        public MessageScanner(MessageScanRules rules)
        {
            this.rules = rules;
        }

        public IEnumerable<Type> EventTypes => eventTypes;
        public IEnumerable<Type> CommandTypes => commandTypes;

        public void Scan()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Except(new[] { Assembly.GetExecutingAssembly() }))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (rules.IsAnEvent(type)) eventTypes.Add(type);
                    if (rules.IsACommand(type)) commandTypes.Add(type);
                }
            }
        }


    }
}
