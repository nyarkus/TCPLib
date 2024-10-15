using System;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Server;
using TCPLib.Server.Net;

namespace ExampleServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TCPLib.Server.Server server = new Server(new BanSaver(), new SettingsSaver(), ServerComponents.None);

            server.Stopped += OnStopped;

            TCPLib.Server.Net.Client.SuccessfulConnection += OnConnected;

            server.Start();
            server.ConsoleRead();
        }

        private static async Task OnConnected(TCPLib.Classes.ResponseCode code, Client client)
        {
            while (true)
            {
                var message = await client.ReceiveSourceAsync();
                TCPLib.Server.Console.Info(UTF8Encoding.UTF8.GetString(message.Data));
            }
        }

        static Task OnStopped()
        {
            Environment.Exit(0);

            return Task.CompletedTask;
        }
    }
}
