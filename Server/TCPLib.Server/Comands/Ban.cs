using TCPLib.Server.Net;
using System.Threading.Tasks;
using System.Linq;
using TCPLib.Classes;
using System.Text;
using System.Collections.Generic;
using TCPLib.Server.SaveFiles;
using TCPLib.Net;

namespace TCPLib.Server.Commands
{
    internal class Ban : ICommand
    {
        public string[] Synonyms { get; }

        public string Name { get; }

        public string Description { get; }

        private Client GetUserByID(string[] args)
        {
            var clientlist = Client.clients.Where(x => x.id.ToString() == args[0]);
            if (clientlist.Count() == 0)
            {
                Console.Error($"The user with id {args[0]} was not found.");
                return null;
            }
            return clientlist.First();
        }
        private Client GetUserByIP(string[] args)
        {
            string ip;
            if (args[0].IndexOf(':') == -1)
            {
                ip = args[0];
            }
            else
            {
                ip = args[0].Split(':')[0];
            }
            foreach (var s in Client.clients)
            {
                if (s.IP.RemovePort() == ip)
                {
                    return s;
                }
            }

            return null;
        }
        private bool CheckBanList(System.Collections.Generic.List<TCPLib.Server.SaveFiles.Ban> list, IP ip)
        {
            foreach (var s in list)
            {
                if (s.IP == ip)
                {
                    Console.Error($"The user with ip {ip} is already blocked.");
                    return true;
                }
            }
            return false;
        }
        private async Task BanWithReason(string[] args, Client client)
        {
            var list = SaveFiles.Ban.Load().ToList();
            StringBuilder reason = new StringBuilder();

            for (int i = 1; i < args.Length; i++)
            {
                reason.Append(args[i] + " ");
            }
            var strReason = reason.ToString();
            var ban = SaveFiles.Ban.CreateBan(client.IP, strReason.Trim(' '));

            list.Add(ban);
            SaveFiles.Ban.Save(list.ToArray());
            if (client != null)
            {
                await client.Kick(new KickMessage(ResponseCode.Kicked, $"You are blocked: \"{strReason}\""));
            }
            Console.Info($"{client.IP.RemovePort()} has been blocked with reason: {reason}");
        }
        private async Task BanWithoutReason(Client client)
        {
            var list = SaveFiles.Ban.Load().ToList();
            var ban = SaveFiles.Ban.CreateBan(client.IP);
            list.Add(ban);
            SaveFiles.Ban.Save(list.ToArray());
            if (client != null)
            {
                await client.Kick(new KickMessage(ResponseCode.Kicked, "You are blocked"));
            }
            Console.Info($"{client.IP.RemovePort()} has been blocked");
        }
        public async Task<bool> Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error("Not enough arguments to know more write: ? ban");
                return false;
            }
            Client client = null;
            if (args[0].IndexOf('.') == -1)
            {
                client = GetUserByID(args);
            }
            else
            {
                client = GetUserByIP(args);
            }
            var ip = client.IP.RemovePort();
            if(!CheckBanList(SaveFiles.Ban.Load().ToList(), ip))
                return false;
            if (args.Length > 1)
            {
                await BanWithReason(args, client);
            }
            else
            {
               await BanWithoutReason(client);
            }
            return true;
        }
        public Ban()
        {
            Synonyms = new [] { "block", "ban" };
            Name = "ban";
            Description = "Blocks a user by ip or id. Usage: ban {ip/id} {reason}";
        }
    }
}