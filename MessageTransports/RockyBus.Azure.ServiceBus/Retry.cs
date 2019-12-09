using System;
using System.Threading.Tasks;

namespace RockyBus.Azure.ServiceBus
{
    public static class Retry
    {
        public static Task Do(
            Func<Task> action,
            int maxAttemptCount = 3) => Do(action, TimeSpan.FromSeconds(1), maxAttemptCount);

        public static Task Do(
            Func<Task> action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            return Do<object>(async () =>
            {
                await action().ConfigureAwait(false);
                return null;
            }, retryInterval, maxAttemptCount);
        }

        public static Task<T> Do<T>(
            Func<Task<T>> action,
            int maxAttemptCount = 3) => Do(action, TimeSpan.FromSeconds(1), maxAttemptCount);

        public static async Task<T> Do<T>(
        Func<Task<T>> action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3)
        {
            Exception exception = null;

            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0) await Task.Delay(retryInterval).ConfigureAwait(false);
                    return await action().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            throw exception;
        }
    }
}
