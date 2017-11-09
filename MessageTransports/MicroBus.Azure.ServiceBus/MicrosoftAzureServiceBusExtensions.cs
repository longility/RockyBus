using System;
using System.Reflection;
using System.Text;
using MicroBus;
using Newtonsoft.Json;

namespace Microsoft.Azure.ServiceBus
{
    internal static class MicrosoftAzureServiceBusExtensions
    {
        private const string MessageTypeKey = "MessageType";
        private static readonly MessageTypeCreator creator = new MessageTypeCreator();
        public static Message SetMessageBody<T>(this Message message, T messageBody)
        {
            var type = typeof(T);

            message.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody));
            message.UserProperties.Add(MessageTypeKey, type.FullName);
            return message;
        }

        public static object GetMessageBody(this Message message)
        {
            var type = creator.Create(message.GetMessageTypeName());
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), type);
        }

        public static string GetMessageTypeName(this Message message) => message.UserProperties[MessageTypeKey].ToString();
    }
}
