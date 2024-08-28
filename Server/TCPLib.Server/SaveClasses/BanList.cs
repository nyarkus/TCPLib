using TCPLib.Server.Net;

namespace TCPLib.Server.SaveFiles;

public class Ban
{
    public string? IP;
    public string? Reason;
    public DateTime? Until;
    public static Ban CreateBan(Client client, string Reason = "", DateTime? Until = null)
    {
        return new Ban() { IP = client.client.Client.RemoteEndPoint!.ToString()!.Split(':')[0], Reason = Reason, Until = Until };
    }
    public static Ban CreateBan(NetClient client, string Reason = "", DateTime? Until = null)
    {
        return new Ban() { IP = client.client.Client.RemoteEndPoint!.ToString()!.Split(':')[0], Reason = Reason, Until = Until };
    }
    public static Ban CreateBan(string ip, string Reason = "", DateTime? Until = null)
    {
        return new Ban() { IP = ip, Reason = Reason, Until = Until };
    }
    public static void Save(Ban[] bans)
    {
        File.WriteAllText("banlist.json", Newtonsoft.Json.JsonConvert.SerializeObject(bans));
    }
    public static Ban[] Load()
    {
        if (!File.Exists("banlist.json")) Save(Array.Empty<Ban>());
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Ban[]>(File.ReadAllText("banlist.json"))!;
    }
    public static void ClearInvalidBans()
    {
        Console.Info("Clearing invalid bans from the ban list...");
        var list = Load();
        var newlist = new List<Ban>();
        foreach (Ban b in list)
            if (b.Until is null || b.Until > DateTime.UtcNow) newlist.Add(b);
        Save(newlist.ToArray());
        GC.Collect();
        Console.Info("The ban list has been cleared!");
    }
}