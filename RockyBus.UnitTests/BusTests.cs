using System;
using System.Threading.Tasks;
using FluentAssertions;
using RockyBus.DemoMessages;
using RockyBus.Message;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace RockyBus.UnitTests
{
    [TestClass]
    public class BusTests
    {
        private Bus bus;
        private IMessageTransport messageTransport;

        [TestInitialize]
        public void given_an_initialized_bus() 
        {
            messageTransport = Substitute.For<IMessageTransport>();
            bus = new Bus(messageTransport,
                              Substitute.For<IDependencyResolver>(),
                          new MessageScanRules()
                .DefineCommandScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Command"))
                .DefineEventScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Event")));
        }

        [TestMethod]
        public void bus_should_be_set()
        {
            messageTransport.Bus.Should().NotBeNull();
        }

        [TestMethod]
        public async Task publishing_a_command_should_throw_exception()
        {
            await bus.Start();
            Func<Task> action = () => bus.Publish(new AppleCommand());
            action.Should().Throw<InvalidOperationException>().Where(e => e.Message.Contains("unexpected behavior"));
        }

        [TestMethod]
        public async Task sending_an_event_should_throw_exception()
        {
            await bus.Start();
            Func<Task> action = () => bus.Send(new CatEvent());
            action.Should().Throw<InvalidOperationException>().Where(e => e.Message.Contains("unexpected behavior"));
        }

        [TestMethod]
        public async Task publish_an_event_should_publish_an_event_to_the_message_transport()
        {
            await bus.Start();
            var message = new CatEvent();

            await bus.Publish(message);
            await messageTransport.Received().Publish(message, bus.MessageTypeToNamePublishingEventMap[message.GetType()]);
        }

        [TestMethod]
        public async Task send_a_command_should_send_a_command_to_the_message_transport()
        {
            await bus.Start();
            var message = new AppleCommand();

            await bus.Send(message);
            await messageTransport.Received().Send(message, bus.MessageTypeToNameSendingCommandMap[message.GetType()]);
        }

        [TestMethod]
        public async Task start_a_publish_and_send_only_bus_should_initialize_publishing_but_skip_receiving() {
            messageTransport.IsPublishAndSendOnly.Returns(true);

            await bus.Start();

            await messageTransport.ReceivedWithAnyArgs().InitializePublishingEndpoint();
            await messageTransport.DidNotReceiveWithAnyArgs().InitializeReceivingEndpoint();
            await messageTransport.DidNotReceiveWithAnyArgs().StartReceivingMessages(Arg.Any<MessageHandlerExecutor>());
        }

        [TestMethod]
        public async Task start_a_receiving_bus_should_initialize_publishing_endpoiint_and_initialize_and_start_receiving()
        {
            messageTransport.IsPublishAndSendOnly.Returns(false);

            await bus.Start();

            await messageTransport.ReceivedWithAnyArgs().InitializePublishingEndpoint();
            await messageTransport.ReceivedWithAnyArgs().InitializeReceivingEndpoint();
            await messageTransport.ReceivedWithAnyArgs().StartReceivingMessages(Arg.Any<MessageHandlerExecutor>());
        }

        [TestMethod]
        public async Task send_a_message_with_send_options_should_send_a_command_to_the_message_transport()
        {
            await bus.Start();
            var message = new AppleCommand();
            var options = new SendOptions();

            options.SetHeaders("color", "red");
            options.SetHeaders("dents", "4");

            await bus.Send(message, options);
            await messageTransport.Received().Send(message, bus.MessageTypeToNameSendingCommandMap[message.GetType()], options);
        }

        [TestMethod]
        public async Task publish_a_message_with_publish_options_should_send_a_command_to_the_message_transport()
        {
            await bus.Start();
            var message = new CatEvent();
            var options = new PublishOptions();

            options.SetHeaders("name", "smokey");
            options.SetHeaders("lives", "7");

            await bus.Publish(message, options);
            await messageTransport.Received().Publish(message, bus.MessageTypeToNamePublishingEventMap[message.GetType()], options);
        }
    }
}
