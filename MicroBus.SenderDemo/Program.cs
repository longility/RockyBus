using System;
using System.Linq;
using System.Threading.Tasks;
using MicroBus.Abstractions;
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
            serviceCollection.AddSingleton(p =>
                {
                    return new BusBuilder()
                        .UseAzureServiceBus(
                            "",
                            configuration =>
                            {
                                configuration.SubscriptionId = subscriptionId;
                                configuration.TenantId = tenantId;
                                configuration.ClientId = clientId;
                                configuration.ClientSecret = clientSecret;

                                configuration.PublishAndSendOptions
                                             .MapCommandToQueue<AppleCommand>("receiver")
                                           .MapCommandToQueue<BananaCommand>("randomreceiver");
                            })
                        .UseMicrosoftDependencyInjection(p)
                        .Build();
                });
            var bus = serviceProvider.GetService<IBus>();
            //TECHNICAL ISSUE: May not be able to do same pattern with other dependency injection frameworks.

            await bus.Start();
            await bus.Publish(new CatEvent { Text = "This is event." });
            await bus.Publish(new DogEvent { Text = "This is event 2." });
            await bus.Send(new AppleCommand { Text = "This is command." });
            await bus.Send(new BananaCommand { Text = "This is command 2." });

            //get default dependency resolver working for simple use case
            //manually add handlers
            //samples
            //Retry policy
            //Unit tests
            //Test different .net frameworks as consumer
            //GitHub
            //Documentation
            //Deploy to nuget server
            //Error handling /DLQ
        }
    }
}
