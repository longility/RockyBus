using System.Text;
using MicroBus;
using MicroBus.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Microsoft.Azure.ServiceBus
{
    internal static class MicrosoftAzureServiceBusExtensions
    {
        private static readonly MessageTypeTranslator translator = new MessageTypeTranslator();
        public static Message SetMessageBody<T>(this Message message, T messageBody)
        {
            message.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody));
            message.UserProperties.Add(UserProperties.MessageTypeKey, MessageTypeTranslator.TranslateFromTypeToName<T>());
            return message;
        }

        public static Message SetDestinationQueue(this Message message, string queue)
        {
            if (string.IsNullOrWhiteSpace(queue)) return message;

            message.UserProperties.Add(UserProperties.DestinationQueueKey, queue);

            return message;
        }

        public static object GetMessageBody(this Message message)
        {
            var type = translator.TranslateFromNameToType(message.GetMessageTypeName());
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), type);
        }

        public static string GetMessageTypeName(this Message message) => message.UserProperties[UserProperties.MessageTypeKey].ToString();
    }
}
