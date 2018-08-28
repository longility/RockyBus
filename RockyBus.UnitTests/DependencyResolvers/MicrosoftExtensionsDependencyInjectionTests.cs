using System;
using System.Threading.Tasks;
using FluentAssertions;
using RockyBus.DemoMessages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;

namespace RockyBus.UnitTests.DependencyResolvers
{
    [TestClass]
    public class MicrosoftExtensionsDependencyInjectionTests : BaseDependencyResolverTests
    {
        [TestMethod]
        public void MicrosoftExtensionsDependencyInjection_handling_message_without_registering_should_throw_an_exception()
        {
            given_no_message_handlers();
            when_handling_message();
            then_should_be_unsuccessful();
        }

        [TestMethod]
        public void MicrosoftExtensionsDependencyInjection_handling_message_for_direct_implementation_should_resolve_and_execute()
        {
            given_direct_implementation_of_IMessageHandler();
            when_handling_message();
            then_should_be_successful();
        }

        [TestMethod]
        public void MicrosoftExtensionsDependencyInjection_handling_message_for_indirect_implementation_should_resolve_and_execute()
        {
            given_indirect_implementation_of_IMessageHandler();
            when_handling_message();
            then_should_be_successful();
        }

        [TestMethod]
        public void MicrosoftExtensionsDependencyInjection_get_message_type()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddScoped<IMessageHandler<AppleCommand>, DirectAppleCommandHandler>()
                .BuildServiceProvider();
            var dependencyResolver = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider, serviceCollection);

            var messageTypes = dependencyResolver.GetHandlingMessageTypes();

            messageTypes.Should().HaveCount(1);
            messageTypes.Single().Should().Be(typeof(AppleCommand));
        }

        protected override void given_no_message_handlers()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                .BuildServiceProvider();
            var dependencyResolver = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider, serviceCollection);
            given_dependency_resolver(dependencyResolver);
        }

        protected override void given_direct_implementation_of_IMessageHandler()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddScoped<IMessageHandler<AppleCommand>, DirectAppleCommandHandler>()
                .BuildServiceProvider();
            var dependencyResolver = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider, serviceCollection);
            given_dependency_resolver(dependencyResolver);
        }

        protected override void given_indirect_implementation_of_IMessageHandler()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddScoped<IMessageHandler<AppleCommand>, IndirectAppleCommandHandler>()
                .BuildServiceProvider();
            var dependencyResolver = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider, serviceCollection);
            given_dependency_resolver(dependencyResolver);
        }
    }
}
