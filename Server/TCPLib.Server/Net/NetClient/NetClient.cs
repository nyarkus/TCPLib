using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using TCPLib.Server.Net.Encrypt;

namespace TCPLib.Server.Net
{
    public abstract partial class NetClient
    {
        public EncryptType EncryptType { get; set; } = EncryptType.RSA;

        public TcpClient client;
        public NetworkStream stream;
        public uint id { get; private set; }
        public static List<NetClient> clients = new List<NetClient>();
        public Encryptor Encryptor;
        public NetClient(TcpClient client, NetworkStream stream)
        {
            this.client = client;
            if (clients.Count == 0) id = 0;
            else id = clients.Last().id + 1;
            this.stream = stream;
        }
        public async Task Kick(KickMessage message)
        {
            try
            {
                await SendAsync(message);
            }
            catch { }
            client.Close();
            clients.Remove(this);
            GC.Collect();
        }
    }
    public enum EncryptType
    {
        AES,
        RSA
    }
}