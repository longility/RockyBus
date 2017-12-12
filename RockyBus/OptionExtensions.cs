using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RockyBus
{
    public static class OptionExtensions
    {
        public static void SetHeaders(this Options options, string key, string value)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options", "Options cannot be null.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key", "Key cannot be null or empty.");
            }

            options.Headers[key] = value;
        }

        public static IReadOnlyDictionary<string, string> GetHeaders(this Options options)
        {
            return new ReadOnlyDictionary<string, string>(options.Headers);
        }
    }
}