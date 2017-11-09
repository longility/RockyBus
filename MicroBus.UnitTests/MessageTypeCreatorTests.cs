using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests
{
    [TestClass]
    public class MessageTypeCreatorTests
    {
        class TestMessage { public string Text { get; set; } }

        [TestMethod]
        public void ShouldCreateMessageTypeByFullName()
        {
            var creator = new MessageTypeCreator();
            var messageType = creator.Create(typeof(TestMessage).FullName);
            messageType.Should().Be(typeof(TestMessage));
        }

        [TestMethod]
        public void ShouldThrowForNotFindingInAssembly()
        {
            var creator = new MessageTypeCreator();
            Action action = () => creator.Create("MicroBus.UnitTests.TestMessage2");
            action.ShouldThrow<NotSupportedException>();
        }
    }
}
