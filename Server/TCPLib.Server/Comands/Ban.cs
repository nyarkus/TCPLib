using TCPLib.Server.Net;
using System.Threading.Tasks;
using System.Linq;
using TCPLib.Classes;
using System.Text;

namespace TCPLib.Server.Commands
{
    internal class Ban : ICommand
    {
        public string[] Synonyms { get; }

        public string Name { get; }

        public string Description { get; }

        public async Task<bool> Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error("Not enough arguments to know more write: ? ban");
                return false;
            }
            string ip = "";
            Client client = null;
            if (args[0].IndexOf('.') == -1)
            {
                var clientlist = Client.clients.Where(x => x.id.ToString() == args[0]);
                if (clientlist.Count() == 0)
                {
                    Console.Error($"The user with id {args[0]} was not found.");
                    return false;
                }
                ip = clientlist.First().client.Client.RemoteEndPoint.ToString().Split(':')[0];
                client = clientlist.First();
            }
            else
            {
                if (args[0].IndexOf(':') == -1)
                {
                    ip = args[0];
                }
                else
                {
                    ip = args[0].Split(':')[0];
                }
                foreach (var s in Client.clients)
                    if (s.client.Client.RemoteEndPoint.ToString().Split(':')[0] == ip)
                    {
                        client = s;
                    }
            }
            var list = SaveFiles.Ban.Load().ToList();
            foreach (var s in list)
            {
                if (s.IP == ip)
                {
                    Console.Error($"The user with ip {ip} is already blocked.");
                }
            }
            if (args.Length > 1)
            {
                StringBuilder reason = new StringBuilder();
                for (int i = 1; i < args.Length; i++)
                {
                    reason.Append(args[i] + " ");
                }
                var strReason = reason.ToString();
                var ban = SaveFiles.Ban.CreateBan(ip, strReason.Trim(' '));
                list.Add(ban);
                SaveFiles.Ban.Save(list.ToArray());
                if (client != null)
                {
                    await client.Kick(new KickMessage(ResponseCode.Kicked, $"You are blocked: \"{strReason}\""));
                }
                Console.Info($"{ip} has been blocked with reason: {reason}");
            }
            else
            {
                var ban = SaveFiles.Ban.CreateBan(ip);
                list.Add(ban);
                SaveFiles.Ban.Save(list.ToArray());
                if (client != null)
                {
                    await client.Kick(new KickMessage(ResponseCode.Kicked, "You are blocked"));
                }
                Console.Info($"{ip} has been blocked");
            }
            return true;
        }
        public Ban()
        {
            Synonyms = new string[] { "block", "ban" };
            Name = "ban";
            Description = "Blocks a user by ip or id. Usage: ban {ip/id} {reason}";
        }
    }
}