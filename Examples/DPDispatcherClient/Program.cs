using System.Runtime.CompilerServices;
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
        private static DateTimeOffset SendTime;
        private static bool Running = true;
        private static async Task Main(string[] args)
        {
            // Creating package handler
            var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));

            // Connecting to the server
            var server = await client.Connect("127.0.0.1:2025");

            // Create a DPDispatcher builder and immediately build it in DPDispatcher
            var dispatcher = new DPDispatcherBuilder(server, stateHandler).Build();

            // Start the dispatcher in a new thread.
            _ = Task.Run(dispatcher.Start);


            Console.WriteLine("Write something");
            var input = Console.ReadLine();
            if(input == null)
            {
                dispatcher.Stop();
                await server.Disconnect();
                return;
            }
            await server.SendAsync(new DataPackageSource("msg", Encoding.UTF8.GetBytes(input)));

            // Use of TimeProvider is optional and is synonymous with DateTimeOffster.UtcNow;
            SendTime = TCPLib.Time.TimeProvider.Now;
            while (Running)
            {
                // That's not the right way to do it 🤓
                await Task.Delay(100);
            }
            dispatcher.Stop();
            await client.ConnectedServer.Disconnect();

            return;
            
        }

        private static async Task OnState(DataPackageSource package)
        {
            await client.ConnectedServer.SendAsync(new State() { Content = $"Ping = {(TCPLib.Time.TimeProvider.Now - SendTime).TotalMilliseconds:F1} ms" });

            Running = false;
        }
    }
}
