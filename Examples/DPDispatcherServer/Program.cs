using ExampleServer;
using TCPLib.Server;
using TCPLib.Server.Net;

namespace DPDispatcherServer
{
    internal static class Program
    {

        private static async Task Main(string[] args)
        {
            var server = new Server(new ServerConfiguration(new BanSaver(), new SettingsSaver(), ServerComponents.BaseCommands) { DefaultPort = 2025 });

            Client.SuccessfulConnection += OnConnection;
            await server.Start();
        }

        private static async Task OnConnection(Client client)
        {
            // Creating custom client class
            var cc = new CustomClient(client);

            // Start the dispatcher. While the dispatcher is running, the thread will be blocked.
            await cc.dispatcher.Start();
        }
    }
}