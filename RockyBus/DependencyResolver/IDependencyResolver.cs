using System;
namespace RockyBus
{
    public interface IDependencyResolver
    {
        IResolverScope CreateScope();
    }
}
