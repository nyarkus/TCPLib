using System;
using System.IO;
using System.Collections.Generic;
using TCPLib.Server.Net;

namespace TCPLib.Server.SaveFiles
{
    public class Ban
    {
        public string IP { get; set; }
        public string Reason { get; set; }
        public DateTime? Until { get; set; }
        public static IBanListSaver saver { get; set; }
        public static Ban CreateBan(Client client, string Reason = "", DateTime? Until = null)
        {
            return new Ban { IP = client.client.Client.RemoteEndPoint.ToString().Split(':')[0], Reason = Reason, Until = Until };
        }
        public static Ban CreateBan(string ip, string Reason = "", DateTime? Until = null)
        {
            return new Ban { IP = ip, Reason = Reason, Until = Until };
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
                if (b.Until is null || b.Until > DateTime.UtcNow) newlist.Add(b);
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