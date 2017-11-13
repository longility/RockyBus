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
        class BlahMessageHandler : IMessageHandler<AppleCommand>
        {
            public Task Handle(AppleCommand message)
            {
                throw new NotImplementedException();
            }
        }
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
                        .AddMessageHandler(() => new BlahMessageHandler()) //IMessageHandler<Message>,new() (cannot have duplicate handlers on a message)

                        .Build();
                });
            var bus = serviceProvider.GetService<IBus>();
            //TECHNICAL ISSUE: May not be able to do same pattern with other dependency injection frameworks.

            await bus.Start();
            await bus.Publish(new CatEvent { Text = "This is event." });
            await bus.Publish(new DogEvent { Text = "This is event 2." });
            await bus.Send(new AppleCommand { Text = "This is command." });
            await bus.Send(new BananaCommand { Text = "This is command 2." });

            //samples
            //Retry policy
            //Test different .net frameworks as consumer
            //GitHub
            //Documentation
            //Deploy to nuget server
            //Error handling /DLQ
        }
    }
}
