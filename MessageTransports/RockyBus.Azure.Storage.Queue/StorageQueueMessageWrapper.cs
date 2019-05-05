using System;
using Newtonsoft.Json;
using RockyBus.Message;

namespace Microsoft.Azure.Storage.Queue
{
    static class CloudQueueMessageExtensions
    {
        public static CloudQueueMessage WrapAndCreateCloudQueueMessage<T>(
            this T message,
            string messageTypeName)
        {

            var json = JsonConvert.SerializeObject(
                StorageQueueMessageWrapper.WrapMessage(message, messageTypeName));
            return new CloudQueueMessage(json);
        }

        public static object UnwrapAndDeserializeMessage(
            this CloudQueueMessage cloudQueueMessage,
            IReceivingMessageTypeNames messageTypeNames)
        {
            var wrappedMessage = JsonConvert
                .DeserializeObject<StorageQueueMessageWrapper>(cloudQueueMessage.AsString);
            return wrappedMessage.UnwrapMessage(messageTypeNames);
        }

        private class StorageQueueMessageWrapper
        {
            public static StorageQueueMessageWrapper WrapMessage<T>(T message, string messageTypeName) =>
                new StorageQueueMessageWrapper
                {
                    MessageType = messageTypeName,
                    Message = JsonConvert.SerializeObject(message)
                };

            public string MessageType { get; set; }
            public string Message { get; set; }
            public object UnwrapMessage(IReceivingMessageTypeNames messageTypeNames)
            {
                var type = messageTypeNames.GetReceivingTypeByMessageTypeName(MessageType);
                return JsonConvert.DeserializeObject(Message, type);
            }
        }
    }
}
