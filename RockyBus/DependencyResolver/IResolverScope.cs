using System;
namespace RockyBus
{
    public interface IResolverScope : IDisposable
    {
        object Resolve(Type objectType);
    }
}
