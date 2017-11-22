using System;
using System.Threading.Tasks;
using FluentAssertions;
using RockyBus.DemoMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace RockyBus.UnitTests.DependencyResolvers
{
    [TestClass]
    public class PoorMansDependencyInjectionTests : BaseDependencyResolverTests
    {
        [TestMethod]
        public void PoorMansDependencyInjection_handling_message_without_registering_should_throw_an_exception()
        {
            given_no_message_handlers();
            when_handling_message();
            then_should_be_unsuccessful();
        }

        [TestMethod]
        public void PoorMansDependencyInjection_handling_message_for_direct_implementation_should_resolve_and_execute()
        {
            given_direct_implementation_of_IMessageHandler();
            when_handling_message();
            then_should_be_successful();
        }

        [TestMethod]
        public void PoorMansDependencyInjection_handling_message_for_indirect_implementation_should_resolve_and_execute()
        {
            given_indirect_implementation_of_IMessageHandler();
            when_handling_message();
            then_should_be_successful();
        }

        protected override void given_no_message_handlers()
        {
            var dependencyInjection = new PoorMansDependencyInjection();
            given_dependency_resolver(dependencyInjection);
        }

        protected override void given_direct_implementation_of_IMessageHandler()
        {
            var dependencyInjection = new PoorMansDependencyInjection();
            dependencyInjection.AddMessageHandler(() => new DirectAppleCommandHandler());
            given_dependency_resolver(dependencyInjection);
        }

        protected override void given_indirect_implementation_of_IMessageHandler()
        {
            var dependencyInjection = new PoorMansDependencyInjection();
            dependencyInjection.AddMessageHandler(() => new IndirectAppleCommandHandler());
            given_dependency_resolver(dependencyInjection);
        }
    }
}
