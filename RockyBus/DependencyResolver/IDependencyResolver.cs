using System;
using System.Collections.Generic;

namespace RockyBus
{
    public interface IDependencyResolver
    {
        IResolverScope CreateScope();
        IEnumerable<Type> GetHandlingMessageTypes();
    }
}
