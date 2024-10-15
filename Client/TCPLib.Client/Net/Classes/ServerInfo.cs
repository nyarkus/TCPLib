using System.Net;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;

namespace TCPLib.Client.Net
{
    public class ServerInfo
    {
        public int MaxPlayers { get; private set; }
        public int Players { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TimeSpan Ping { get; private set; }


        /// <summary>
        /// Method receiving information about the server
        /// </summary>
        /// <param name="address">IP address of the server whose information you want to know</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static ServerInfo GetFrom(IPEndPoint address)
        {
            var task = Task.Run(() => _GetFrom(ref address));
            if (task.Wait(TimeSpan.FromSeconds(30)))
            {
                return task.Result;
            }
            else
            {
                throw new TimeoutException();
            }

        }
        private static Task<ServerInfo> _GetFrom(ref IPEndPoint address)
        {
            var client = new UdpClient(address.Port);

            DateTime start = DateTime.UtcNow;

            client.Send(new byte[] { 0 }, 1, address);
            var result = client.Receive(ref address);
            var ping = DateTime.UtcNow - start;

            var jobject = JObject.Parse(System.Text.Encoding.UTF8.GetString(result));

            var res = new ServerInfo()
            {
                MaxPlayers = (int)jobject["MaxPlayers"],
                Players = (int)jobject["Players"],
                Name = (string)jobject["Name"],
                Description = (string)jobject["Description"],
                Ping = ping,
            };
            return Task.FromResult(res);
        }
        /// <summary>
        /// Method receiving information about the server
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public static ServerInfo GetFrom(IPEndPoint address, UdpClient client, TimeSpan timeout)
        {
            var task = Task.Run(() => _GetFrom(ref address, ref client));
            if (task.Wait(timeout))
                return task.Result;
            else
                throw new TimeoutException();
        }
        /// <summary>
        /// Method receiving information about the server
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public static ServerInfo GetFrom(IPEndPoint address, UdpClient client)
        {
            var task = Task.Run(() => _GetFrom(ref address, ref client));
            if (task.Wait(TimeSpan.FromSeconds(30)))
                return task.Result;
            else
                throw new TimeoutException();
        }
        private static Task<ServerInfo> _GetFrom(ref IPEndPoint address, ref UdpClient client)
        {
            try
            {


                DateTime start = DateTime.UtcNow;

                client.Send(new byte[] { 0 }, 1, address);
                var result = client.Receive(ref address);
                DateTime end = DateTime.UtcNow;
                var ping = end - start;
                var jobject = JObject.Parse(System.Text.Encoding.UTF8.GetString(result));

                var res = new ServerInfo()
                {
                    MaxPlayers = (int)jobject["MaxPlayers"],
                    Players = (int)jobject["Players"],
                    Name = (string)jobject["Name"],
                    Description = (string)jobject["Description"],
                    Ping = ping,
                };
                return Task.FromResult(res);
            }
            catch
            {
                return null;
            }
        }
    }
}
