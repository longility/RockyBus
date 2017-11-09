using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests
{
    [TestClass]
    public class MessageHandlerTypeCreatorTests
    {
        class TestMessage { public string Text { get; set; } }
        class TestMessage2 { public string Text { get; set; } }

        [TestMethod]
        public void ShouldMatchNaturalTypeWithCreatedType()
        {
            var naturalType = typeof(IMessageHandler<TestMessage>);
            var creator = new MessageHandlerTypeCreator();
            var createdType = creator.Create(typeof(TestMessage));
            createdType.Should().Be(naturalType);
        }

        [TestMethod]
        public void ShouldNotMatchNaturalTypeWithCreatedType()
        {
            var naturalType = typeof(IMessageHandler<TestMessage2>);
            var creator = new MessageHandlerTypeCreator();
            var createdType = creator.Create(typeof(TestMessage));
            createdType.Should().NotBe(naturalType);
        }
    }
}
