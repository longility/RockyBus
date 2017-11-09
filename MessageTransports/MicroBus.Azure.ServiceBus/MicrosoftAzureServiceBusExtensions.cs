using System;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Azure.ServiceBus
{
    internal static class MicrosoftAzureServiceBusExtensions
    {
        private const string MessageTypeKey = "MessageType";
        public static Message SerializeMessage<T>(this T message)
        {
            var type = typeof(T);

            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
            serviceBusMessage.UserProperties.Add(MessageTypeKey, type.FullName);
            return serviceBusMessage;
        }

        public static object DeserializeMessage(this Message message)
        {
            var type = Type.GetType(message.GetMessageTypeName());
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), type);
        }

        public static string GetMessageTypeName(this Message message) => message.UserProperties[MessageTypeKey].ToString();
    }
}
