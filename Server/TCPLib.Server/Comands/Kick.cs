using TCPLib.Server.Net;
using System.Threading.Tasks;
using System.Linq;

namespace TCPLib.Server.Commands
{
    internal class Kick : ICommand
    {
        public string[] Synonyms { get; }

        public string Name { get; }

        public string Description { get; }

        public async Task<bool> Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error("Not enough arguments to know more write: ? kick");
                return false;
            }
            string ip = "";
            NetClient client;
            if (args[0].IndexOf('.') == -1)
            {
                var clientlist = Client.clients.Where(x => x.id.ToString() == args[0]);
                if (clientlist.Count() == 0)
                {
                    Console.Error($"The user with id {args[0]} was not found.");
                    return false;
                }
                ip = clientlist.FirstOrDefault().client.Client.RemoteEndPoint.ToString().Split(':')[0];
                client = clientlist.First();
            }
            else
            {
                if (args[0].IndexOf(':') == -1)
                    ip = args[0];
                else
                    ip = args[0].Split(':')[0];
                var clientlist = Client.clients.Where(x => x.client.Client.RemoteEndPoint.ToString().Split(':')[0] == args[0]);
                if (clientlist.Count() == 0)
                {
                    Console.Error($"The user with ip {ip} was not found.");
                    return false;
                }
                client = clientlist.First();
            }
            if (args.Length > 1)
            {
                string reason = "";
                for (int i = 1; i < args.Length; i++)
                {
                    reason += args[i] + " ";
                }
                reason = reason.TrimEnd(' ');
                reason = reason.TrimStart(' ');
                await client.Kick(new KickMessage(ResponseCode.Kicked, reason));
                Console.Info($"{ip} has been kicked with reason: {reason}");
            }
            else
            {
                await client.Kick(new KickMessage(ResponseCode.Kicked));
                Console.Info($"{ip} has been kicked");
            }
            return true;
        }
        public Kick()
        {
            Synonyms = new string[] { "kick" };
            Name = "kick";
            Description = "Excludes a user by ip or id. Usage: kick {ip/id} {reason}";
        }
    }
}