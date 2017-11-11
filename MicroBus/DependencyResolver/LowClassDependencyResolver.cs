using System;
namespace MicroBus
{
    public class LowClassDependencyResolver : IDependencyResolver
    {
        public IResolverScope CreateScope()
        {
            throw new NotImplementedException();
        }

        public string[] ResolvableMessageTypeNames()
        {
            throw new NotImplementedException();
        }
    }
}
