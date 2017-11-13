using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MicroBus.Abstractions;
using MicroBus.DemoMessages;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBus.SenderDemo
{
    class Program
    {
        private const string SubscriptionId = "";
        private const string TenantId = "";
        private const string ClientId = "";
        private const string ClientSecret = "";
        private const string ConnectionString = "";

        static async Task<IBus> JupiterService()
        {
            var bus = new BusBuilder()
                .UseAzureServiceBus(
                    ConnectionString,
                    configuration =>
                    {
                        configuration.SubscriptionId = SubscriptionId;
                        configuration.TenantId = TenantId;
                        configuration.ClientId = ClientId;
                        configuration.ClientSecret = ClientSecret;
                        configuration.ResourceGroupName = ResourceGroupName;
                        configuration.NamespaceName = NamespaceName;
                        configuration.PublishAndSendOptions
                                      .MapCommandToQueue<AppleCommand>("saturn");
                    })
                .DefineCommandScanRuleWith(t => t.Namespace == "MicroBus.DemoMessages" && t.Name.EndsWith("Command"))
                .DefineEventScanRuleWith(t => t.Namespace == "MicroBus.DemoMessages" && t.Name.EndsWith("Event"))
                .Build();
            await bus.Start();

            await bus.Publish(new CatEvent { Text = $"Meow published at {DateTimeOffset.UtcNow}" });
            await bus.Send(new AppleCommand { Text = $"jupiter to saturn sent at {DateTimeOffset.UtcNow}" });

            return bus;
        }

        class AppleCommandHandler : IMessageHandler<AppleCommand>
        {
            public Task Handle(AppleCommand message)
            {
                Trace.WriteLine($"AppleCommand received with message '{message.Text}' at {DateTimeOffset.UtcNow}");

                return Task.CompletedTask;
            }
        }

        class RottenAppleCommandHandler : IMessageHandler<AppleCommand>
        {
            public Task Handle(AppleCommand message)
            {

                Trace.WriteLine($"AppleCommand received with message '{message.Text}' at {DateTimeOffset.UtcNow}");
                throw new Exception("Rotten Apple");
            }
        }

        class CatEventHandler : IMessageHandler<CatEvent>
        {
            public Task Handle(CatEvent message)
            {
                Trace.WriteLine($"CatEvent received with message '{message.Text}' at {DateTimeOffset.UtcNow}");

                return Task.CompletedTask;
            }
        }

        static async Task<IBus> SaturnService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddScoped<IMessageHandler<AppleCommand>, AppleCommandHandler>()
                .AddScoped<IMessageHandler<CatEvent>, CatEventHandler>()
                .AddSingleton(p =>
                    {
                        return new BusBuilder()
                            .UseAzureServiceBus(
                                ConnectionString,
                                configuration =>
                                {
                                    configuration.SubscriptionId = SubscriptionId;
                                    configuration.TenantId = TenantId;
                                    configuration.ClientId = ClientId;
                                    configuration.ClientSecret = ClientSecret;
                                    configuration.ResourceGroupName = ResourceGroupName;
                                    configuration.NamespaceName = NamespaceName;
                                    configuration.ReceiveOptions.QueueName = "saturn";
                                })
                            .UseMicrosoftDependencyInjection(p)
                            .DefineCommandScanRuleWith(t => t.Namespace == "MicroBus.DemoMessages" && t.Name.EndsWith("Command"))
                            .DefineEventScanRuleWith(t => t.Namespace == "MicroBus.DemoMessages" && t.Name.EndsWith("Event"))
                            .Build();
                    });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var bus = serviceProvider.GetService<IBus>();

            await bus.Start();

            return bus;
        }

        static async Task Main(string[] args)
        {
            var buses = new[]
            {
                await JupiterService(),
                await SaturnService()
            };

            Console.WriteLine("Press any key to end.");
            Console.Read();

            foreach (var bus in buses)
            {
                await bus.Stop();
            }
        }
    }
}
