using System;
namespace MicroBus
{
    public interface IResolverScope : IDisposable
    {
        object Resolve(Type objectType);
    }
}
