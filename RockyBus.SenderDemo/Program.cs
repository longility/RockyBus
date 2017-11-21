﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RockyBus.DemoMessages;
using Microsoft.Extensions.DependencyInjection;

namespace RockyBus.SenderDemo
{
    class Program
    {
        const string SubscriptionId = ""; //Resource group Subscription ID
        const string TenantId = ""; //AAD -> Properties -> Directory ID
        const string ClientId = ""; //Application ID
        const string ClientSecret = ""; //API Access -> Key's Value
        const string ConnectionString = "";
        const string ResourceGroupName = "";
        const string NamespaceName = "";

        static async Task<IBus> JupiterService()
        {
            var bus = new BusBuilder()
                .UseAzureServiceBus(
                    ConnectionString,
                    configuration =>
                    {
                        configuration.SetManagementSettings(SubscriptionId, TenantId, ClientId, ClientSecret, ResourceGroupName, NamespaceName);
                        configuration.PublishAndSendOptions
                                     .AttemptCreateTopicWith(new Microsoft.Azure.Management.ServiceBus.Models.SBTopic { EnablePartitioning = true })
                                     .MapCommandToQueue<AppleCommand>("saturn");
                    })
                .DefineCommandScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Command"))
                .DefineEventScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Event"))
                .Build();
            await bus.Start();

            await bus.Publish(new CatEvent { Text = "Meow" });
            await bus.Send(new AppleCommand { Text = "jupiter to saturn" });

            return bus;
        }

        class AppleCommandHandler : IMessageHandler<AppleCommand>
        {
            public Task Handle(AppleCommand message, IMessageContext messageContext)
            {
                Trace.WriteLine($"AppleCommand received with message '{message.Text}' at {DateTimeOffset.UtcNow} (enqueued at {messageContext.EnqueueTime})");

                return Task.CompletedTask;
            }
        }

        class RottenAppleCommandHandler : IMessageHandler<AppleCommand>
        {
            public Task Handle(AppleCommand message, IMessageContext messageContext)
            {

                Trace.WriteLine($"AppleCommand received with message '{message.Text}' at {DateTimeOffset.UtcNow} (enqueued at {messageContext.EnqueueTime})");
                throw new Exception("Rotten Apple");
            }
        }

        class CatEventHandler : IMessageHandler<CatEvent>
        {
            public Task Handle(CatEvent message, IMessageContext messageContext)
            {
                Trace.WriteLine($"CatEvent received with message '{message.Text}' at {DateTimeOffset.UtcNow} (enqueued at {messageContext.EnqueueTime})");

                return Task.CompletedTask;
            }
        }

        static async Task<IBus> SaturnService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddScoped<IMessageHandler<AppleCommand>, RottenAppleCommandHandler>()
                .AddScoped<IMessageHandler<CatEvent>, CatEventHandler>()
                .AddSingleton(p =>
                    {
                        return new BusBuilder()
                            .UseAzureServiceBus(
                                ConnectionString,
                                configuration =>
                                {
                                    configuration.SetManagementSettings(SubscriptionId, TenantId, ClientId, ClientSecret, ResourceGroupName, NamespaceName);
                                    configuration.ReceiveOptions.QueueName = "saturn";
                                })
                            .UseMicrosoftDependencyInjection(p)
                            .DefineCommandScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Command"))
                            .DefineEventScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Event"))
                        .HandleMessageHandleExceptions(a => { Trace.WriteLine(a.Exception.Message); return Task.CompletedTask; })
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
