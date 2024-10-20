namespace Tests;

using System.Net.Sockets;
using System.Threading;
using TCPLib.Client.Net;
using TCPLib.Server.Net;
using TCPLib.Server.SaveFiles;
using TCPLib.Net;
#if DEBUG
public class NetTest
{
    public const ushort port = 2025;

    public static TCPLib.Server.Server server;
    static object locker = new object();
    static bool started = false;
    public static void StartServer()
    {
        lock (locker)
        {
            if (started)
                return;

            server = new(new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = port } }, TCPLib.Server.ServerComponents.UDPStateSender) { AESKeySize = 256, RSAKeyStrength = 4096 });
            TCPLib.Server.Server.TestingMode = true;
            server.Start();

            started = true;
        }
    }
    [Fact]
    public async Task StartAndConnectionTest()
    {
        TCPLib.Client.Client client = new();
        StartServer();

        await client.Connect(IP.Parse($"127.0.0.1:{port}"));
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");
}

    [Fact]
    public void GetInfoTest()
    {
        StartServer();

        UdpClient client = new UdpClient();
        TCPLib.Client.Net.ServerInfo info = null!;
        for (int i = 0; i < 10; i++)
        {
            info = TCPLib.Client.Net.ServerInfo.GetFrom(new(System.Net.IPAddress.Parse("127.0.0.1"), port), client);
            if (info != null) break;
        }
        if (info == null)
            Assert.Fail("info is null");

        Assert.True(
        info.MaxPlayers == TCPLib.Server.Server.settings.maxPlayers
        && info.Name == TCPLib.Server.Server.settings.title
        && info.Description == TCPLib.Server.Server.settings.description);

        client.Dispose();
    }
    [Fact]
    public async Task TransferMessage()
    {
        TCPLib.Client.Client client = new();
        StartServer();

        var sserver = await client.Connect(IP.Parse($"127.0.0.1:{port}"));
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        string t = sserver.client.Client.LocalEndPoint!.ToString()!.Split(':')[4];
        var sclient = Client.clients.First(x => x.client.Client.RemoteEndPoint!.ToString()!.Split(':')[1] == t);

        Message message = new Message() { Data = "eb" };

        await sserver.SendAsync(message);
        
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

        await sclient.SendAsync(message);

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
    }

    [Fact]
    public async Task Disconnect()
    {
        TCPLib.Client.Client client = new();
        StartServer();
        int clientsOnStart = Client.clients.Count;

        await client.Connect(IP.Parse($"127.0.0.1:{port}"));
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        string t = client.ConnectedServer.client.Client.LocalEndPoint!.ToString()!.Split(':')[4];
        var sclient = Client.clients.First(x => x.client.Client.RemoteEndPoint!.ToString()!.Split(':')[1] == t);

        await client.ConnectedServer.Disconnect();
        await sclient.ReceiveSourceAsync();

        Assert.True(clientsOnStart - Client.clients.Count <= 0);
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