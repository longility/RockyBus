using System;
using System.Threading.Tasks;
using FluentAssertions;
using RockyBus.DemoMessages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace RockyBus.UnitTests.DependencyResolvers
{
    public abstract class BaseDependencyResolverTests
    {
        MessageHandlerExecutor messageHandlerExecutor;
        Func<Task> action;
        protected abstract void given_no_message_handlers();
        protected abstract void given_direct_implementation_of_IMessageHandler();
        protected abstract void given_indirect_implementation_of_IMessageHandler();
        protected void given_dependency_resolver(IDependencyResolver dependencyResolver) => messageHandlerExecutor = new MessageHandlerExecutor(dependencyResolver);
        protected void when_handling_message() => action = () => messageHandlerExecutor.Execute(new AppleCommand(), Substitute.For<IMessageContext>(), new System.Threading.CancellationToken());
        protected void then_should_be_successful() => action().IsCompletedSuccessfully.Should().BeTrue();
        protected void then_should_be_unsuccessful() => action.ShouldThrow<TypeAccessException>();
    }
}
