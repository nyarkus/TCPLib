namespace Tests;

using System.Net.Sockets;
using System.Threading;
using TCPLib.Server.Net;
using TCPLib.Server.SaveFiles;
#if DEBUG
public class NetTest
{
    [Fact]
    public async Task StartAndConnectionTest()
    {
        var port = 2025;
        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = (ushort)port } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), port);
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        server.Stop();
}

    [Fact]
    public void GetInfoTest()
    {
        var port = 2026;
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = (ushort)port } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        UdpClient client = new UdpClient();
        TCPLib.Client.Net.ServerInfo info = null!;
        for (int i = 0; i < 10; i++)
        {
            info = TCPLib.Client.Net.ServerInfo.GetFrom(new(System.Net.IPAddress.Parse("127.0.0.1"), port), client);
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
    [Fact]
    public async Task TransferMessage()
    {
        var port = 2027;
        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = (ushort)port } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        var sserver = await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), port);
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        Message message = new Message() { Data = "eb" };

        await sserver.SendAsync(message);
        var sclient = Client.clients.First();
        // Добавляем тайм-аут
        var receiveTask = sclient.ReceiveAsync<Message>();
        if (await Task.WhenAny(receiveTask, Task.Delay(5000)) == receiveTask)
        {
            var result = await receiveTask;
            Assert.Equal(result.Unpack().Data, message.Data);
        }
        else
        {
            Assert.Fail("Receiving message timed out.");
        }

        await Client.clients.First().SendAsync(message);

        receiveTask = sserver.ReceiveAsync<Message>();
        if (await Task.WhenAny(receiveTask, Task.Delay(5000)) == receiveTask)
        {
            var result = await receiveTask;
            Assert.Equal(result.Unpack().Data, message.Data);
        }
        else
        {
            Assert.Fail("Receiving message timed out.");
        }

        server.Stop();
    }

    [Fact]
    public async Task Disconnect()
    {
        var port = 2028;
        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = (ushort)port } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), port);
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");
        var cclient = TCPLib.Server.Net.Client.clients.First();

        await client.ConnectedServer.Disconnect();
        await cclient.ReceiveSourceAsync();

        Assert.True(Client.clients.Count == 0);

        server.Stop();
    }
}

public struct SettingsSaver : ISettingsSaver
{
    public Settings settings = new Settings();

    public SettingsSaver() {}

    public Settings Load()
        => settings;

    public void Save(Settings settings)
        => this.settings = settings;
}
public struct BanSaver : IBanListSaver
{
    Ban[] bans = [];

    public BanSaver() {}

    public Ban[] Load()
        => bans;

    public void Save(Ban[] bans)
        => this.bans = bans;
}
#endif