using System;
using Microsoft.Extensions.DependencyInjection;

namespace RockyBus
{
    internal class MicrosoftDependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;
        public MicrosoftDependencyInjectionDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IResolverScope CreateScope()
        {
            return new MicrosoftDependencyInjectionScope(serviceProvider.CreateScope());
        }
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
        public static BusBuilder UseMicrosoftDependencyInjection(this BusBuilder busBuilder, IServiceProvider serviceProvider)
        {
            var dependencyInjection = new MicrosoftDependencyInjectionDependencyResolver(serviceProvider);
            return busBuilder
                .UseDependencyResolver(dependencyInjection);
        }
    }
}
