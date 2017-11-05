using System;
using System.Threading.Tasks;
using MicroBus.DemoMessages;

namespace MicroBus.ReceiverDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = new Bus(
                new AzureServiceBusTransport(
                    "Endpoint=sb://microbusplatform.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6Gi/JgvRgVn1EgyMguShleD4QdJEvyMK3GbQpUmeuek="
                    , options => { }));
            await bus.StartAsync();
            Console.ReadLine();
        }
    }
}
