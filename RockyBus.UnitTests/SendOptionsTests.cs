using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RockyBus.UnitTests
{
    [TestClass]
    public class SendOptionsTests
    {
        [TestMethod]
        public void ShouldSetOptions()
        {
            var sendOptions = new SendOptions();
            var key = "code";
            var value = "abc"; 

            sendOptions.SetHeaders(key, value);

            sendOptions.Headers[key].Should().Be(value);
        }

        [TestMethod]
        public void ShouldGetOptions()
        {
            var sendOptions = new SendOptions();
            var key = "code";
            var value = "abc";
            sendOptions.SetHeaders(key, value);

            var options = sendOptions.GetHeaders();

            options[key].Should().Be(value);
        }

        [TestMethod]
        public void ShouldThrowIfOptionsAreNull()
        {
            Action setNullOptions = () => OptionExtensions.SetHeaders(null, "code", "value");
            setNullOptions.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void ShouldThrowIfKeyIsEmpty()
        {
            Action setNullOptions = () => OptionExtensions.SetHeaders(new SendOptions(), "", "value");
            setNullOptions.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void ShouldThrowIfKeyIsNull()
        {
            Action setNullOptions = () => OptionExtensions.SetHeaders(new SendOptions(), null, "value");
            setNullOptions.ShouldThrow<ArgumentException>();
        }
    }
}