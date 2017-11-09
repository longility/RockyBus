using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroBus.UnitTests.DependencyResolvers
{
    [TestClass]
    public class MicrosoftExtensionsDependencyInjectionTests
    {
        [TestMethod]
        public void ShouldThrowForNoConsumerRegistered()
        {
            var serviceCollection = new ServiceCollection();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider));

            Action action = () => executor.Execute(new AppleEvent(), new System.Threading.CancellationToken());

            action.ShouldThrow<TypeAccessException>();
        }

        [TestMethod]
        public void ShouldResolveAndExecuteDirectImplementingMessageHandler()
        {
            var serviceCollection = new ServiceCollection()
                .AddScoped<IMessageHandler<AppleEvent>, AppleEventHandler>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider));

            var task = executor.Execute(new AppleEvent(), new System.Threading.CancellationToken());

            task.IsCompleted.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldResolveAndExecuteIndirectImplementingMessageHandler()
        {
            var serviceCollection = new ServiceCollection()
                .AddScoped<IMessageHandler<BananaEvent>, BananaEventHandler>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var executor = new MessageHandlerExecutor(new MicrosoftDependencyInjectionDependencyResolver(serviceProvider));

            var task = executor.Execute(new BananaEvent(), new System.Threading.CancellationToken());

            task.IsCompleted.Should().BeTrue();
        }

    }
}
