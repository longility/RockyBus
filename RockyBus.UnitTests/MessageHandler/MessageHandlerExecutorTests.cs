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
        public void thrown_exception_in_message_handler_with_async_should_call_exception_handler_and_throw()
        {
            given(new AsyncTaskCommandHandler());
            when_executing_handler();
            then_should_call_exception_handler_and_throw();
        } 

        [TestMethod]
        public void thrown_exception_in_message_handler_without_async_should_call_exception_handler_and_throw()
        {
            given(new TaskCommandHandler());
            when_executing_handler();
            then_should_call_exception_handler_and_throw();
        }

        Exception exception;
        MessageHandlerExecutor executor;
        Func<Task> action;
        
        void given(IMessageHandler<AppleCommand> handler)
        {
            Func<MessageHandlingExceptionRaisedEventArgs, Task> exceptionHandler = a =>
            {
                exception = a.Exception;
                return Task.CompletedTask;
            };

            var dependencyResolver = Substitute.For<IDependencyResolver>();
            dependencyResolver.CreateScope().Resolve(Arg.Any<Type>()).ReturnsForAnyArgs(handler);
            executor = new MessageHandlerExecutor(dependencyResolver, exceptionHandler);
        }

        void when_executing_handler()
        {
            action = () => executor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());
        }

        void then_should_call_exception_handler_and_throw()
        {
            action.ShouldThrow<Exception>();
            exception.Message.Should().Be("Rotten Apple");
        }
    }

    public class AsyncTaskCommandHandler : IMessageHandler<AppleCommand>
    {
#pragma warning disable CS1998
        public async Task Handle(AppleCommand message, IMessageContext messageContext) => throw new Exception("Rotten Apple");
    }

    public class TaskCommandHandler : IMessageHandler<AppleCommand>
    {
        public Task Handle(AppleCommand message, IMessageContext messageContext) => throw new Exception("Rotten Apple");
    }
}
