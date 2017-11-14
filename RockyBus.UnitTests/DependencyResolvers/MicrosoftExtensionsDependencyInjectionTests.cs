using System;
using System.Threading.Tasks;
using FluentAssertions;
using RockyBus.DemoMessages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace RockyBus.UnitTests.DependencyResolvers
{
    [TestClass]
    public class MicrosoftExtensionsDependencyInjectionTests
    {
        [TestMethod]
        public void executing_message_handler_without_registering_should_throw_an_exception()
        {
            var serviceProvider = new ServiceCollection()
                .BuildServiceProvider();
            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider));

            Func<Task> action = () => executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            action.ShouldThrow<TypeAccessException>();
        }

        [TestMethod]
        public void register_message_handler_as_direct_implementation_should_resolve_and_execute()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IMessageHandler<AppleCommand>, AppleCommandHandler>()
                .BuildServiceProvider();
            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider));

            var task = executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            task.IsCompleted.Should().BeTrue();
        }

        [TestMethod]
        public void register_message_handler_as_an_indirect_implementation_should_resolve_and_execute()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IMessageHandler<BananaCommand>, BananaCommandHandler>()
                .BuildServiceProvider();
            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider));

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
            var serviceProvider = new ServiceCollection()
                .AddScoped<IMessageHandler<AppleCommand>, RottenAppleCommandHandler>()
                .BuildServiceProvider();
            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider), exceptionHandler);

            Func<Task> action = () => executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());

            action.ShouldThrow<Exception>();
            exception.Message.Should().Be("Rotten Apple");
        }

    }
}
