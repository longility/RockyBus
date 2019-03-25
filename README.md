[![Build status](https://ci.appveyor.com/api/projects/status/35pu1v3retdxwhw9/branch/master?svg=true)](https://ci.appveyor.com/project/longility/rockybus/branch/master)

# RockyBus
A .NET Standard message bus library that is extensible for simple needs. 

# Getting Started
See demo project for a working example (with missing configuration).

## Messaging
There are two types of message that are supported, events and commands. For details or understanding, NServiceBus does a great job at explaining it here: https://docs.particular.net/nservicebus/messaging/messages-events-commands.

### Publishing an event
```
await bus.Publish(new UserCreated());
```
### Sending a command
```
await bus.Send(new Email());
```

### Handling a message

1. Create a message handler class that implements `IMessageHandler<T>` where T is the command or event class.
2. Register the message handler directly to RockyBus or using a DI framework.
```
//Directly to RockyBus
new BusBuilder().AddMessageHandler(() => new RottenAppleCommandHandler());

//Using Microsoft Dependency Injection
new BusBuilder().UseMicrosoftDependencyInjection(p, serviceCollection);
```
3. Define a unique queue name for the handling service that does not conflict with another purpose handling service.

```
//Azure Service Bus
new BusBuilder()
    .UseAzureServiceBus(
        ConnectionString,
        configuration =>
        {
            configuration.ReceiveOptions.QueueName = "saturn";
        });
```
### Running the bus

Help the bus find the messages and start the bus.

```
var bus = new BusBuilder()
    .DefineCommandScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Command"))
    .DefineEventScanRuleWith(t => t.Namespace == "RockyBus.DemoMessages" && t.Name.EndsWith("Event"))
    .Build();
await bus.Start();
```

## Configuring to allow RockyBus to manage the service bus resource

1. Azure Portal
   1. Create an AAD application.
   2. In the resource group or subscription -> _Access control (IAM)_, add the AAD application as a contributor role.

2. Getting values
   - Subscription ID - can be found on the service bus resource's Overview
   - Resource Group Name - can be found on the service bus resource's Overview
   - Namespace Name - this is the service bus resource name
   - Directory (tenant) ID - can be found on the AAD's application (app that is registered)
   - Application (client) ID - can be found on the AAD's application (app that is registered)
   - Client Secret - can be generated in AAD's application (app that is registered) under _Certificates & secrets_
   - ConnectionString (used for sending and listening to messages) - can use the `RootManageSharedAccessKey` in the service bus resource's _Shared access policy_ menu.

3. Setting values
```
new BusBuilder()
    .UseAzureServiceBus(
        ConnectionString,
        configuration =>
        {
            configuration.SetManagementSettings(SubscriptionId, TenantId, ClientId, ClientSecret, ResourceGroupName, NamespaceName);
        });
```

### Resources
https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal
https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app 

# Why?
- Projects getting into event based architecture may have a harder time justifying NServiceBus and want to get started. Should be easy refactor and transition to NServiceBus when the time is right.
- More simple projects that only requires simple, common messaging patterns.

## Supported Message Transports
- Azure Service Bus - https://github.com/Azure-Samples/service-bus-dotnet-management

## Supported Patterns
- PubSub Pattern
- Command Message

## Supported Dependency Injection Frameworks
- Microsoft.Extensions.DependencyInjection

## Supported .NET Versions
- .NET Core 2.0
- ??

Feel free to reach out to contribute.
