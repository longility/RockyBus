using System;
namespace RockyBus
{
    public static class BusBuilderExtensions
    {
        public static BusBuilder UseAzureStorageQueue(
            this BusBuilder busBuilder, string connectionString, Action<AzureStorageQueueConfiguration> configuration) 
        => busBuilder.UseMessageTransport(new AzureStorageQueueTransport(connectionString, configuration));
    }
}
