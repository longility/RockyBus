using System;
using System.Threading.Tasks;
using MicroBus.DemoMessages;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBus.SenderDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var subscriptionId = "";
            var tenantId = "";
            var clientId = "";
            var clientSecret = "";

            var serviceCollection = new ServiceCollection();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var bus = new BusBuilder()
                .UseAzureServiceBus(
                "",
                configuration => 
            {
                configuration.SubscriptionId = subscriptionId;
                configuration.TenantId = tenantId;
                configuration.ClientId = clientId;
                configuration.ClientSecret = clientSecret;

                configuration.PublishAndSendOptions
                           .MapCommandToQueue<TestCommandMessage>("receiver")
                           .MapCommandToQueue<TestCommand2Message>("randomreceiver");})
                .UseMicrosoftDependencyInjection(serviceProvider)
                .Build();
            await bus.StartAsync();
            await bus.PublishAsync(new TestEventMessage { Text = "This is event." });
            await bus.PublishAsync(new TestEvent2Message { Text = "This is event 2." });
            await bus.SendAsync(new TestCommandMessage { Text = "This is command." });
            await bus.SendAsync(new TestCommand2Message { Text = "This is command 2." });
        }
    }
}
