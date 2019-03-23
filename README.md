[![Build status](https://ci.appveyor.com/api/projects/status/35pu1v3retdxwhw9/branch/master?svg=true)](https://ci.appveyor.com/project/longility/rockybus/branch/master)

# RockyBus
A .NET Standard message bus library that is extensible for simple needs. 

# Getting Started
See demo project for now.
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
