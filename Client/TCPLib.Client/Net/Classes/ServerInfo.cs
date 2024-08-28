// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

using System.Net;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;

namespace TCPLib.Client.Net
{
    public class ServerInfo
    {
        public int MaxPlayers;
        public int Players;
        public string Name;
        public string Description;
        public TimeSpan Ping;


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
                return task.Result;
            else
                throw new TimeoutException();

        }
        private static Task<ServerInfo> _GetFrom(ref IPEndPoint address)
        {
            var client = new UdpClient(address.Port);

            DateTime start = DateTime.UtcNow;

            client.Send(new byte[] { 0 }, 1, address);
            var result = client.Receive(ref address);
            DateTime end = DateTime.UtcNow;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerInfo>(System.Text.Encoding.UTF8.GetString(result));
            res.Ping = end - start;
            return Task.FromResult(res);
        }
        /// <summary>
        /// Method receiving information about the server
        /// </summary>
        /// <param name="address">IP address of the server whose information you want to know</param>
        /// <param name="client">client, through which the request for information will be made</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static ServerInfo GetFrom(IPEndPoint address, UdpClient client)
        {
#if DEBUG
            var task = Task.Run(() => _GetFrom(ref address, ref client));
            if (task.Wait(TimeSpan.FromSeconds(5 * 60)))
                return task.Result;
            else
                throw new TimeoutException();
#else
            var task = Task.Run(() => _GetFrom(ref address, ref client));
                    if (task.Wait(TimeSpan.FromSeconds(30)))
                        return task.Result;
                    else
                        throw new TimeoutException();
#endif
        }
        private static Task<ServerInfo> _GetFrom(ref IPEndPoint address, ref UdpClient client)
        {
            try
            {


                DateTime start = DateTime.UtcNow;

                client.Send(new byte[] { 0 }, 1, address);
                var result = client.Receive(ref address);
                DateTime end = DateTime.UtcNow;

                var res = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerInfo>(System.Text.Encoding.UTF8.GetString(result));
                res.Ping = end - start;
                return Task.FromResult(res);
            }
            catch
            {
                return null;
            }
        }
    }
}
