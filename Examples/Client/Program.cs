using TCPLib.Client;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ExampleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Client client = new Client();

            var server = await client.Connect(IPAddress.Parse("127.0.0.1"), 2024);

            while (true)
            {
                string input = Console.ReadLine();

                await server.SendAsync(new Message() { Data = input });
            }
        }
    }
}
