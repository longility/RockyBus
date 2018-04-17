using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RockyBus.UnitTests
{
    [TestClass]
    public class PublishOptionsTests
    {
        [TestMethod]
        public void ShouldSetOptions()
        {
            var options = new PublishOptions();
            var key = "code";
            var value = "abc"; 

            options.SetHeaders(key, value);

            options.GetHeaders()[key].Should().Be(value);
        }

        [TestMethod]
        public void ShouldGetOptions()
        {
            var options = new PublishOptions();
            var key = "code";
            var value = "abc";
            options.SetHeaders(key, value);

            var header = options.GetHeaders();

            header[key].Should().Be(value);
        }

        [TestMethod]
        public void ShouldThrowIfKeyIsEmpty()
        {
            Action setNullOptions = () => new PublishOptions().SetHeaders(" ", "value");
            setNullOptions.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void ShouldThrowIfKeyIsNull()
        {
            Action setNullOptions = () => new PublishOptions().SetHeaders(null, "value");
            setNullOptions.ShouldThrow<ArgumentException>();
        }
    }
}