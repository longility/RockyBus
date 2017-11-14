using System;
using System.Threading.Tasks;
using FluentAssertions;
using MicroBus.DemoMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MicroBus.UnitTests.DependencyResolvers
{
    [TestClass]
    public class PoorMansDependencyInjectionTests
    {
        [TestMethod]
        public void executing_message_handler_without_registering_should_throw_an_exception()
        {
            var executor = new MessageHandlerExecutor(new PoorMansDependencyInjection());

            Func<Task> action = () => executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            action.ShouldThrow<TypeAccessException>();
        }

        [TestMethod]
        public void register_message_handler_as_direct_implementation_should_resolve_and_execute()
        {
            var dependencyInjection = new PoorMansDependencyInjection();
            dependencyInjection.AddMessageHandler(() => new AppleCommandHandler());
            var executor = new MessageHandlerExecutor(dependencyInjection);

            var task = executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            task.IsCompleted.Should().BeTrue();
        }

        [TestMethod]
        public void register_message_handler_as_an_indirect_implementation_should_resolve_and_execute()
        {
            var dependencyInjection = new PoorMansDependencyInjection();
            dependencyInjection.AddMessageHandler(() => new BananaCommandHandler());
            var executor = new MessageHandlerExecutor(dependencyInjection);

            var task = executor.Execute(new BananaCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            task.IsCompleted.Should().BeTrue();
        }

        [TestMethod]
        public void thrown_exception_in_message_handler_should_call_exception_handler_and_throw()
        {
            Exception exception = null;
            Func<MessageHandlingExceptionRaisedEventArgs, Task> exceptionHandler = a =>
            {
                exception = a.Exception;
                return Task.CompletedTask;
            };
            var dependencyInjection = new PoorMansDependencyInjection();
            dependencyInjection.AddMessageHandler(() => new RottenAppleCommandHandler());
            var executor = new MessageHandlerExecutor(dependencyInjection, exceptionHandler);

            Func<Task> action = () => executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            action.ShouldThrow<Exception>();
            exception.Message.Should().Be("Rotten Apple");
        }

    }
}
