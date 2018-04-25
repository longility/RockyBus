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
            var options = new SendOptions();
            var key = "code";
            var value = "abc"; 

            options.SetHeaders(key, value);

            options.GetHeaders()[key].Should().Be(value);
        }

        [TestMethod]
        public void ShouldGetOptions()
        {
            var options = new SendOptions();
            var key = "code";
            var value = "abc";
            options.SetHeaders(key, value);

            var headers = options.GetHeaders();

            headers[key].Should().Be(value);
        }

        [TestMethod]
        public void ShouldThrowIfKeyIsEmpty()
        {
            Action setNullOptions = () => new SendOptions().SetHeaders(" ", "value");
            setNullOptions.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void ShouldThrowIfKeyIsNull()
        {
            Action setNullOptions = () => new SendOptions().SetHeaders(null, "value");
            setNullOptions.Should().Throw<ArgumentException>();
        }
    }
}