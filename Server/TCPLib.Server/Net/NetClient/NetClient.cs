using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using TCPLib.Server.Net.Encrypt;
using TCPLib.Classes;
using System.Threading;

namespace TCPLib.Server.Net
{
    public abstract partial class NetClient : IDisposable
    {
        public EncryptType EncryptType { get; set; } = EncryptType.RSA;

        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }
        public uint id { get; private set; }
        public static IReadOnlyCollection<NetClient> clients
        {
            get
            {
                return _clients;
            }
        }
        protected static List<NetClient> _clients = new List<NetClient>();
        public Encryptor Encryptor { get; set; }

        protected CancellationTokenSource OnKick;
        public NetClient(TcpClient client, NetworkStream stream)
        {
            this.client = client;
            if (clients.Count == 0) id = 0;
            else id = clients.Last().id + 1;
            this.stream = stream;

            OnKick = new CancellationTokenSource();
        }
        public async Task Kick(KickMessage message)
        {
            try
            {
                await SendAsync(message);
            }
            catch { }
            OnDisconnected();
        }
        private void OnDisconnected()
        {
            OnKick.Cancel();

            client.Close();
            _clients.Remove(this);
            GC.Collect();
        }

        public async void Dispose()
        {
            if(stream != null)
            {
                await Kick(new KickMessage(ResponseCode.DisconnectedByUser));
            }
            else
            {
                OnDisconnected();
            }
        }
    }
    public enum EncryptType
    {
        AES,
        RSA
    }
}