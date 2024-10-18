using TCPLib.Server.Net;
using System.Threading.Tasks;
using System.Linq;
using TCPLib.Classes;
using System.Text;

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
            Client client;
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
                StringBuilder reason = new StringBuilder();
                for (int i = 1; i < args.Length; i++)
                {
                    reason.Append(args[i] + " ");
                }
                string strReason = reason.ToString();
                strReason = strReason.Trim(' ');

                await client.Kick(new KickMessage(ResponseCode.Kicked, strReason));
                Console.Info($"{ip} has been kicked with reason: {strReason}");
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
            Synonyms = new [] { "kick" };
            Name = "kick";
            Description = "Excludes a user by ip or id. Usage: kick {ip/id} {reason}";
        }
    }
}