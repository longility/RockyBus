using System;
using System.Collections.Generic;

namespace RockyBus
{
    public abstract class Options
    {
        readonly Dictionary<string, string> headers = new Dictionary<string, string>();

        public void SetHeaders(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key", "Key cannot be null or empty.");

            headers[key] = value;
        }

        public IReadOnlyDictionary<string, string> GetHeaders()
        {
            return headers;
        }
    }
}