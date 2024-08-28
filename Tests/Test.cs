namespace Tests;

using System.Net.Sockets;
using TCPLib.Server.Net;
#if DEBUG
[TestCaseOrderer("Tests.Orders.AlphabeticalOrderer", "Tests")]
public class ServerTest
{
    [Fact]

    public async Task StartAndConnectionTest()
    {
        Thread.Sleep(1000);
        GC.Collect(2, GCCollectionMode.Aggressive);

        TCPLib.Client.Client client = new(new("testgame", "1.0"));
        TCPLib.Server.Server server = new(new("testgame", "1.0"));
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 2024);
        if (Client.clients.Count == 0)
            throw new Exception("The client was unable to connect");

        server.Stop();
    }

    [Fact]
    public void GetInfoTest()
    {
        Thread.Sleep(1000);
        GC.Collect(2, GCCollectionMode.Aggressive);

        TCPLib.Server.Server server = new(new("testgame", "1.0"));
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        UdpClient client = new UdpClient();
        TCPLib.Client.Net.ServerInfo info = null!;
        for (int i = 0; i < 10; i++)
        {
            info = TCPLib.Client.Net.ServerInfo.GetFrom(new(System.Net.IPAddress.Parse("127.0.0.1"), 2024), client);
            if (info != null) break;
        }
        if (info == null)
            Assert.Fail("info is null");

        Assert.True(info.Players == TCPLib.Server.Net.Client.clients.Count
        && info.MaxPlayers == TCPLib.Server.Server.settings.maxPlayers
        && info.Name == TCPLib.Server.Server.settings.title
        && info.Description == TCPLib.Server.Server.settings.description);

        client.Dispose();
        server.Stop();
    }
}
#endif