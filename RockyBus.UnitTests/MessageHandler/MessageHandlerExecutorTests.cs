using System;
using System.Threading.Tasks;
using FluentAssertions;
using RockyBus.DemoMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace RockyBus.UnitTests.DependencyResolvers
{
    [TestClass]
    public class MessageHandlerExecutorTests
    {
        [TestMethod]
        public void thrown_exception_in_message_handler_should_call_exception_handler_and_throw()
        {
            Exception exception = null;
            Func<MessageHandlingExceptionRaisedEventArgs, Task> exceptionHandler = a =>
            {
                exception = a.Exception;
                return Task.CompletedTask;
            };

            var dependencyResolver = Substitute.For<IDependencyResolver>();
            dependencyResolver.CreateScope().Resolve(Arg.Any<Type>()).ReturnsForAnyArgs(new RottenAppleCommandHandler());
            var executor = new MessageHandlerExecutor(dependencyResolver, exceptionHandler);

            Func<Task> action = () => executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            action.ShouldThrow<Exception>();
            exception.Message.Should().Be("Rotten Apple");
        }

    }
}
