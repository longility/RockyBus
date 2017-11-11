using FluentAssertions;
using MicroBus.DemoMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests
{
    [TestClass]
    public class MessageHandlerTypeCreatorTests
    {
        [TestMethod]
        public void create_message_handler_of_type_should_match_message_handler_type()
        {
            var creator = new MessageHandlerTypeCreator();
            var createdType = creator.Create(typeof(AppleCommand));
            createdType.Should().Be(typeof(IMessageHandler<AppleCommand>));
        }

        [TestMethod]
        public void create_message_handler_of_different_type_should_not_match_message_handler_type()
        {
            var creator = new MessageHandlerTypeCreator();
            var createdType = creator.Create(typeof(AppleCommand));
            createdType.Should().NotBe(typeof(IMessageHandler<BananaCommand>));
        }
    }
}
