using TCPLib.Client;
using System.Net;
using System.Threading.Tasks;
using System;
using TCPLib.Net;

namespace ExampleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TCPLib.Client.Client client = new Client();

            var server = await client.Connect(IP.Parse("127.0.0.1:2024"));

            while (true)
            {
                string input = Console.ReadLine();
                if (input == null)
                    return;

                await server.SendAsync(new Message { Data = input });
            }
        }
    }
}
