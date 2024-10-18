using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Client;
using TCPLib.Client.DPDispatcher;
using TCPLib.Net;
using TCPLib.Net.DPDispatcher;

namespace Tests
{
#if DEBUG
    public class DPDispatcher
    {
        internal bool Received = false;
        internal bool ServerReceived = false;
        internal Task OnReceived(DataPackageSource package)
        {
            if(package.Type == "Message")
                Received = true;

            return Task.CompletedTask;
        }
        internal Task OnServerReceived(DataPackageSource package)
        {
            if (package.Type == "Message")
                ServerReceived = true;

            return Task.CompletedTask;
        }
        [Fact]
        public async Task DPDispatcherClient()
        {
            TCPLib.Client.Client client = new();
            NetTest.StartServer();

            var sserver = await client.Connect(IP.Parse($"127.0.0.1:{NetTest.port}"));
            if (TCPLib.Server.Net.Client.clients.Count == 0)
                Assert.Fail("The client was unable to connect");

            string t = sserver.client.Client.LocalEndPoint!.ToString()!.Split(':')[4];
            var sclient = TCPLib.Server.Net.Client.clients.First(x => x.client.Client.RemoteEndPoint!.ToString()!.Split(':')[1] == t);

            var handler = DPHandler.Create(DPFilter.Equals("Message"), new DataPackageReceive(OnReceived));
            var dispatcher = new DPDispatcherBuilder(client.ConnectedServer, handler).Build();
            _ = Task.Run(dispatcher.Start);

            

            await sclient.SendAsync(new Message() { Data = "meow" });
            await Task.Delay(1000);
            Assert.True(Received);

            dispatcher.Stop();
        }
        [Fact]
        public async Task DPDispatcherServer()
        {
            TCPLib.Client.Client client = new();
            NetTest.StartServer();

            var sserver = await client.Connect($"127.0.0.1:{NetTest.port}");
            if (TCPLib.Server.Net.Client.clients.Count == 0)
                Assert.Fail("The client was unable to connect");

            string t = sserver.client.Client.LocalEndPoint!.ToString()!.Split(':')[4];
            var sclient = TCPLib.Server.Net.Client.clients.First(x => x.client.Client.RemoteEndPoint!.ToString()!.Split(':')[1] == t);

            var handler = TCPLib.Server.DPDispatcher.DPHandler.Create(DPFilter.Equals("Message"), new DataPackageReceive(OnServerReceived));
            var dispatcher = new TCPLib.Server.DPDispatcher.DPDispatcherBuilder(sclient, handler).Build();
            _ = Task.Run(dispatcher.Start);

            await sserver.SendAsync(new Message() { Data = "meow" });
            await Task.Delay(1000);
            Assert.True(ServerReceived);

            dispatcher.Stop();
        }
    }
#endif
}
