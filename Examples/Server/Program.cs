using System;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Server;
using TCPLib.Server.Net;

namespace ExampleServer
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            TCPLib.Server.Server server = new Server(new BanSaver(), new SettingsSaver(), ServerComponents.BaseCommands);

            server.Stopped += OnStopped;

            TCPLib.Server.Net.Client.SuccessfulConnection += OnConnected;

            await server.Start();
            server.ConsoleRead();
        }

        private static async Task OnConnected(Client client)
        {
            while (client.IsAlive)
            {
                var message = await client.ReceiveSourceAsync();
                TCPLib.Server.Console.Info(Encoding.UTF8.GetString(message.Data));
            }
        }

        static Task OnStopped()
        {
            Environment.Exit(0);

            return Task.CompletedTask;
        }
    }
}
