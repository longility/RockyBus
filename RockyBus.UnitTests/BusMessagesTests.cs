using System;
using FluentAssertions;
using RockyBus.DemoMessages.Commands;
using RockyBus.DemoMessages.Events;
using RockyBus.Message;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading.Tasks;

namespace RockyBus.UnitTests.Message
{
    [TestClass]
    public class BusMessagesTests
    {
        [TestMethod]
        public void bus_messages_should_send_only_commands_without_handler()
        {
            busMessages.IsSendable(typeof(Apple)).Should().BeTrue();
            busMessages.IsSendable(typeof(Banana)).Should().BeFalse();
            busMessages.IsSendable(typeof(Cat)).Should().BeFalse();
            busMessages.IsSendable(typeof(Dog)).Should().BeFalse();
        }

        [TestMethod]
        public void bus_messages_should_publish_only_events_without_handler()
        {
            busMessages.IsPublishable(typeof(Apple)).Should().BeFalse();
            busMessages.IsPublishable(typeof(Banana)).Should().BeFalse();
            busMessages.IsPublishable(typeof(Cat)).Should().BeTrue();
            busMessages.IsPublishable(typeof(Dog)).Should().BeFalse();
        }

        [TestMethod]
        public void no_dependency_resolver_should_mark_as_either_sendable_or_publishable() {
            var busMessages = new BusMessages(
                new MessageScanRules()
                .DefineEventScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages.Events")
                .DefineCommandScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages.Commands"),
                null);

            busMessages.IsSendable(typeof(Apple)).Should().BeTrue();
            busMessages.IsSendable(typeof(Banana)).Should().BeTrue();
            busMessages.IsSendable(typeof(Cat)).Should().BeFalse();
            busMessages.IsSendable(typeof(Dog)).Should().BeFalse();

            busMessages.IsPublishable(typeof(Apple)).Should().BeFalse();
            busMessages.IsPublishable(typeof(Banana)).Should().BeFalse();
            busMessages.IsPublishable(typeof(Cat)).Should().BeTrue();
            busMessages.IsPublishable(typeof(Dog)).Should().BeTrue();
        }

        [TestMethod]
        public void bus_messages_should_translate_receiving_message_type_name_to_type()
        {
            var eventType = busMessages.GetReceivingTypeByMessageTypeName(typeof(Banana).FullName);
            eventType.Should().Be(typeof(Banana));
        }

        [TestMethod]
        public void bus_messages_should_translate_type_to_publishing_event_type_name()
        {
            var eventTypeName = busMessages.MessageTypeToNamePublishingEventMap[typeof(Cat)];
            eventTypeName.Should().Be(typeof(Cat).FullName);
        }

        [TestMethod]
        public void bus_messages_should_translate_type_to_sending_command_type_name()
        {
            var commandTypeName = busMessages.MessageTypeToNameSendingCommandMap[typeof(Apple)];
            commandTypeName.Should().Be(typeof(Apple).FullName);
        }

        [TestMethod]
        public void no_messages_found_should_throw_exception()
        {
            Action action = () => new BusMessages(
                new MessageScanRules()
                .DefineEventScanRuleWith(t => t.Namespace == "Blah")
                .DefineCommandScanRuleWith(t => t.Namespace == "Blah"), Substitute.For<IDependencyResolver>());
            action.Should().Throw<TypeLoadException>();
        }

        class BananaHandler : IMessageHandler<Banana>
        {
            public Task Handle(Banana message, IMessageContext messageContext)
            {
                throw new NotImplementedException();
            }
        }

        class DogHandler : IMessageHandler<Dog>
        {
            public Task Handle(Dog message, IMessageContext messageContext)
            {
                throw new NotImplementedException();
            }
        }

        private static BusMessages busMessages = new BusMessages(
                new MessageScanRules()
                .DefineEventScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages.Events")
                .DefineCommandScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages.Commands"),
                new PoorMansDependencyInjection().AddMessageHandler(() => new BananaHandler()).AddMessageHandler(() => new DogHandler()));

    }
}
