using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBus
{
    internal class MicrosoftDependencyInjectionDependencyResolver : IDependencyResolver, IMessageHandlerResolution
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string[] resolvableMessageTypeNames;
        public MicrosoftDependencyInjectionDependencyResolver(IServiceProvider serviceProvider, IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            this.serviceProvider = serviceProvider;
            this.resolvableMessageTypeNames = serviceDescriptors
                .Where(c => c.ServiceType == typeof(IMessageHandler<>))
                .Select(c => MessageTypeTranslator.TranslateFromTypeToName(c.ServiceType.GetGenericArguments()[0])).ToArray();
        }

        public IResolverScope CreateScope()
        {
            return new MicrosoftDependencyInjectionScope(serviceProvider.CreateScope());
        }

        public string[] ResolvableMessageTypeNames() => resolvableMessageTypeNames;
    }

    internal class MicrosoftDependencyInjectionScope : IResolverScope
    {
        private readonly IServiceScope serviceScope;

        public MicrosoftDependencyInjectionScope(IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
        }

        public object Resolve(Type objectType) => serviceScope.ServiceProvider.GetService(objectType);
        public void Dispose() => serviceScope.Dispose();
    }

    public static class DependencyResolverExtensions
    {
        public static BusBuilder UseMicrosoftDependencyInjection(this BusBuilder busBuilder, IServiceProvider serviceProvider, IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            var dependencyInjection = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider, serviceDescriptors);
            return busBuilder
                .UseDependencyResolver(dependencyInjection)
                .UseMessageHandlerResolution(dependencyInjection);
        }
    }
}
