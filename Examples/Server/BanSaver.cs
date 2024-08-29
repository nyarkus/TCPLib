using System;
using System.IO;
using Newtonsoft.Json;
using TCPLib.Server.SaveFiles;

namespace ExampleServer
{
    public class BanSaver : IBanListSaver
    {
        public void Save(Ban[] bans)
        {
            File.WriteAllText("banlist.json", JsonConvert.SerializeObject(bans));
        }
        public Ban[] Load()
        {
            if (!File.Exists("banlist.json")) Save(Array.Empty<Ban>());
            return JsonConvert.DeserializeObject<Ban[]>(File.ReadAllText("banlist.json"));
        }
    }
}
