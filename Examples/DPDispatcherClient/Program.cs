using System.Text;
using TCPLib.Classes;
using TCPLib.Client;
using TCPLib.Client.DPDispatcher;
using TCPLib.Net.DPDispatcher;

namespace DPDispatcherClient
{
    internal static class Program
    {
        private static Client client = new Client();
        private static async Task Main(string[] args)
        {
            
            var server = await client.Connect("127.0.0.1:2025");

            var messageHandler = DPHandler.Create(DPFilter.Equals("msg"), new DataPackageReceive(OnMessage));
            var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));
        }

        private static Task OnState(DataPackageSource package)
        {
            var state = package.As<State>();

            Console.WriteLine(state.Value.Content);

            return Task.CompletedTask;
        }

        private static async Task OnMessage(DataPackageSource package)
        {
            var content = Encoding.UTF8.GetString(package.Data);

            await client.ConnectedServer.SendAsync(new State() { Content = "I got your message!" });
        }
    }
}
