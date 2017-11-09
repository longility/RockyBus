using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests.MessageTransports
{
    [TestClass]
    public class AzureServiceBusTests
    {
        class TestMessage { public string Text { get; set; } }

        [TestMethod]
        public void ShouldHaveFullNameWhenSettingBody()
        {
            var message = new Message().SetMessageBody(new TestMessage { Text = "Hello World" });
            message.GetMessageTypeName().Should().Be(typeof(TestMessage).FullName);
        }
    }
}
