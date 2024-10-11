namespace Tests;

using System.Net.Sockets;
using TCPLib.Server.Net;
using TCPLib.Server.SaveFiles;
#if DEBUG
public class NetTest
{
    [Fact]
    public async Task StartAndConnectionTest()
    {
        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = 2024 } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 2024);
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        server.Stop();
    }

    [Fact]
    public void GetInfoTest()
    {
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = 2025 } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        UdpClient client = new UdpClient();
        TCPLib.Client.Net.ServerInfo info = null!;
        for (int i = 0; i < 10; i++)
        {
            info = TCPLib.Client.Net.ServerInfo.GetFrom(new(System.Net.IPAddress.Parse("127.0.0.1"), 2025), client);
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
        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = 2026 } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 2026);
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        Message message = new Message() { Data = "eb" };

        await client.ConnectedServer.SendAsync(message);
        var result = await Client.clients.First().ReceiveWithProcessingAsync<Message>();

        Assert.Equal(result.Value.Unpack().Data, message.Data);

        await Client.clients.First().SendAsync(message);
        result = await client.ConnectedServer.ReceiveWithProcessingAsync<Message>();

        Assert.Equal(result.Value.Unpack().Data, message.Data);
    }
    [Fact]
    public async Task Disconnect()
    {
        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver() { settings = new Settings() { port = 2027 } });
        TCPLib.Server.Server.TestingMode = true;
        server.Start();

        await client.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 2027);
        if (Client.clients.Count == 0)
            Assert.Fail("The client was unable to connect");

        await Client.clients.First().Kick(new(TCPLib.Classes.ResponseCode.Kicked));
        await client.ConnectedServer.Disconnect();
        await Client.clients.First().ReceiveWithProcessingAsync<Message>(TimeSpan.FromSeconds(5));

        Assert.True(Client.clients.Count == 0);
    }
}

public class SettingsSaver : ISettingsSaver
{
    public Settings settings = new Settings();
    public Settings Load()
        => settings;

    public void Save(Settings settings)
        => this.settings = settings;
}
public class BanSaver : IBanListSaver
{
    Ban[] bans = [];
    public Ban[] Load()
        => bans;

    public void Save(Ban[] bans)
        => this.bans = bans;
}
#endif