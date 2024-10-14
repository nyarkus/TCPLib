using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Client;
using TCPLib.Client.DPDispatcher;
using TCPLib.Server.SaveFiles;

namespace Tests
{
#if false
    public class DPDispatcher
    {
        internal bool Received = false;
        internal Task OnReceived(DataPackageSource package)
        {
            if(package.Type == "Message")
                Received = true;

            return Task.CompletedTask;
        }
        [Fact]
        public async Task DPDispatcherClient()
        {
            var port = 2024;
            TCPLib.Client.Client client = new();
            TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = (ushort)port } });
            TCPLib.Server.Server.TestingMode = true;
            server.Start();

            await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), port);
            if (TCPLib.Server.Net.Client.clients.Count == 0)
                Assert.Fail("The client was unable to connect");

            var handler = DataPackageHandlerRegistry.Create(DataPackageFilter.Equals("Message"), new DataPackageReceive(OnReceived));
            var dispatcher = new DPDispatcherBuilder(client.ConnectedServer, handler).Build();
            _ = Task.Run(dispatcher.Start);

            await TCPLib.Server.Net.Client.clients.First().SendAsync(new Message() { Data = "meow" });
            await Task.Delay(1000);
            Assert.True(Received);

            dispatcher.Stop();
            server.Stop();
        }
    }
#endif
}
