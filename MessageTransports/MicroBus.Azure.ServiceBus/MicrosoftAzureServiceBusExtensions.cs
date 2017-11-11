using System.Text;
using MicroBus.Azure.ServiceBus;
using MicroBus.Message;
using Newtonsoft.Json;

namespace Microsoft.Azure.ServiceBus
{
    internal static class MicrosoftAzureServiceBusExtensions
    {
        public static object GetMessageBody(this Message message, IMessageTypeNames messageTypeNames)
        {
            var messageTypeName = message.UserProperties[UserProperties.MessageTypeKey].ToString();
            var type = messageTypeNames.MessageTypeNameToType(messageTypeName);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), type);
        }
    }
}
