namespace Tests;

using Newtonsoft.Json;
using System.Net.Sockets;
using TCPLib.Server.Net;
using TCPLib.Server.SaveFiles;
#if DEBUG
[TestCaseOrderer("Tests.Orders.AlphabeticalOrderer", "Tests")]
public class NetTest
{
    [Fact]
    public async Task StartAndConnectionTest()
    {
        if (File.Exists("Certificate.key"))
            File.Delete("Certificate.key");

        TCPLib.Client.Client client = new();
        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver());
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
        if (File.Exists("Certificate.key"))
            File.Delete("Certificate.key");

        TCPLib.Server.Server server = new(new BanSaver(), new SettingsSaver());
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

public class SettingsSaver : ISettingsSaver
{
    Settings settings = new Settings();
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