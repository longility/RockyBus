using FluentAssertions;
using MicroBus.DemoMessages.Commands;
using MicroBus.DemoMessages.Events;
using MicroBus.Message;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests.Message
{
    [TestClass]
    public class BusMessagesTests
    {
        private static BusMessages busMessages = new BusMessages(
                new MessageScanRules()
                .DefineEventScanRuleWith(t => t.Namespace == "MicroBus.DemoMessages.Events")
                .DefineCommandScanRuleWith(t => t.Namespace == "MicroBus.DemoMessages.Commands"));
        
        [TestMethod]
        public void scan_bus_messages_should_find_events_and_commands()
        {
            busMessages.EventMessageTypeNames.Should().HaveCount(2);
            busMessages.EventMessageTypeNames.Should().Contain(typeof(Cat).FullName);
            busMessages.EventMessageTypeNames.Should().Contain(typeof(Dog).FullName);

            busMessages.CommandMessageTypeNames.Should().HaveCount(2);
            busMessages.CommandMessageTypeNames.Should().Contain(typeof(Apple).FullName);
            busMessages.CommandMessageTypeNames.Should().Contain(typeof(Banana).FullName);
        }

        [TestMethod]
        public void bus_messages_should_be_able_to_identify_messages()
        {
            busMessages.IsACommand(typeof(Apple)).Should().BeTrue();
            busMessages.IsACommand(typeof(Cat)).Should().BeFalse();
            busMessages.IsAnEvent(typeof(Apple)).Should().BeFalse();
            busMessages.IsAnEvent(typeof(Cat)).Should().BeTrue();
        }

        [TestMethod]
        public void bus_messages_should_translate_message_type_name_to_type()
        {
            var eventType = busMessages.GetTypeByMessageTypeName(typeof(Apple).FullName);
            eventType.Should().Be(typeof(Apple));

            var commandType = busMessages.GetTypeByMessageTypeName(typeof(Cat).FullName);
            commandType.Should().Be(typeof(Cat));
        }

        [TestMethod]
        public void bus_messages_should_translate_type_to_message_type_name()
        {
            var eventTypeName = busMessages.GetMessageTypeNameByType(typeof(Apple));
            eventTypeName.Should().Be(typeof(Apple).FullName);

            var commandTypeName = busMessages.GetMessageTypeNameByType(typeof(Cat));
            commandTypeName.Should().Be(typeof(Cat).FullName);
        }    
    }
}
