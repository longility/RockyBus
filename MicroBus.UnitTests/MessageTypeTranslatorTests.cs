using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests
{
    [TestClass]
    public class MessageTypeTranslatorTests
    {
        class TestMessage { public string Text { get; set; } }

        [TestMethod]
        public void ShouldTranslateFromNameToType()
        {
            var translator = new MessageTypeTranslator();
            var messageType = translator.TranslateFromNameToType(MessageTypeTranslator.TranslateFromTypeToName<TestMessage>());
            messageType.Should().Be(typeof(TestMessage));
        }

        [TestMethod]
        public void ShouldThrowForNotFindingInAssembly()
        {
            var translator = new MessageTypeTranslator();
            Action action = () => translator.TranslateFromNameToType("MicroBus.UnitTests.TestMessage2");
            action.ShouldThrow<NotSupportedException>();
        }
    }
}
