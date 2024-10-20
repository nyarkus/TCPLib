using System.Text;
using TCPLib.Classes;
using TCPLib.Net.DPDispatcher;
using TCPLib.Server.DPDispatcher;
using TCPLib.Server.Net;

namespace DPDispatcherServer
{
    public class CustomClient
    {
        private Client client;
        public DPDispatcher dispatcher { get; private set; }

        public CustomClient(Client client)
        {
            this.client = client;

            // Creating package handlers
            var messageHandler = DPHandler.Create(DPFilter.Equals("msg"), new DataPackageReceive(OnMessage));
            var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));

            // Create a dispatcher
            dispatcher = new DPDispatcherBuilder(client, messageHandler, stateHandler).Build();
        }

        private Task OnState(DataPackageSource package)
        {
            var state = package.As<State>();

            // Output to the console what the client sent us.
            TCPLib.Server.Console.Info(state.Value.Content);

            // We stop the dispatcher so that we don't create an endless loop.
            dispatcher.Stop();
            
            return Task.CompletedTask;
        }

        private async Task OnMessage(DataPackageSource package)
        {
            var content = Encoding.UTF8.GetString(package.Data);

            await client.SendAsync(new State() { Content = "I got your message!" });
        }
    }
}