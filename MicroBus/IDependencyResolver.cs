using System;
namespace MicroBus
{
    public interface IDependencyResolver
    {
        IResolverScope CreateScope();
    }
}
