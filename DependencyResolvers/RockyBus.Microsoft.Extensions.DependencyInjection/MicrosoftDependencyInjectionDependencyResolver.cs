using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace RockyBus
{
    internal class MicrosoftDependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ICollection<ServiceDescriptor> serviceDescriptors;
        private static readonly string MessageHandlerType = typeof(IMessageHandler<>).Name;
        private readonly Lazy<IEnumerable<Type>> handlingMessageTypes;

        public MicrosoftDependencyInjectionDependencyResolver(IServiceProvider serviceProvider, ICollection<ServiceDescriptor> serviceDescriptors)
        {
            this.serviceProvider = serviceProvider;
            this.serviceDescriptors = serviceDescriptors;
            handlingMessageTypes = new Lazy<IEnumerable<Type>>(() =>
            {
                return serviceDescriptors
                .Where(d => d.ServiceType.Name == MessageHandlerType)
                .Select(d => d.ServiceType.GenericTypeArguments[0]).ToList();
            });
        }

        public IResolverScope CreateScope()
        {
            return new MicrosoftDependencyInjectionScope(serviceProvider.CreateScope());
        }

        public IEnumerable<Type> GetHandlingMessageTypes() => handlingMessageTypes.Value;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="busBuilder"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="serviceDescriptors">ServiceCollection should be passed in</param>
        /// <returns></returns>
        public static BusBuilder UseMicrosoftDependencyInjection(this BusBuilder busBuilder, IServiceProvider serviceProvider, ICollection<ServiceDescriptor> serviceDescriptors)
        {
            var dependencyInjection = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider, serviceDescriptors);
            return busBuilder
                .UseDependencyResolver(dependencyInjection);
        }
    }
}
