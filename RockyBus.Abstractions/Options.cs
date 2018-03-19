using System.Collections.Generic;

namespace RockyBus
{
    public abstract class Options
    {
        protected Options()
        {
            Headers = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Headers { get; }
    }
}