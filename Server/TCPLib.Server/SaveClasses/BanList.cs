using System;
using System.IO;
using System.Collections.Generic;
using TCPLib.Server.Net;
using TCPLib.Net;

namespace TCPLib.Server.SaveFiles
{
    public class Ban
    {
        public IP IP { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset? Until { get; set; }
        public static IBanListSaver saver { get; set; }
        public static Ban CreateBan(Client client, string Reason = "", DateTimeOffset? Until = null)
        {
            return new Ban { IP = client.IP.RemovePort(), Reason = Reason, Until = Until };
        }
        public static Ban CreateBan(IP ip, string Reason = "", DateTimeOffset? Until = null)
        {
            return new Ban { IP = ip.RemovePort(), Reason = Reason, Until = Until };
        }
        public static Ban[] Load()
            => saver.Load();
        public static void Save(Ban[] bans)
            => saver.Save(bans);
        public static void ClearInvalidBans()
        {
            Console.Info("Clearing invalid bans from the ban list...");
            var list = saver.Load();
            var newlist = new List<Ban>();
            foreach (Ban b in list)
                if (b.Until is null || b.Until > Time.TimeProvider.Now) newlist.Add(b);
            saver.Save(newlist.ToArray());
            GC.Collect();
            Console.Info("The ban list has been cleared!");
        }
    }
    public interface IBanListSaver
    {
        void Save(Ban[] bans);
        Ban[] Load();
    }
}