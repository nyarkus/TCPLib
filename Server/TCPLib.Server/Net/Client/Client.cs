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
    public partial class Client : IDisposable
    {
        public EncryptType EncryptType { get; set; } = EncryptType.RSA;

        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }
        public uint id { get; private set; }
        public static IReadOnlyCollection<Client> clients
        {
            get
            {
                return _clients;
            }
        }
        protected static List<Client> _clients = new List<Client>();
        public Encryptor Encryptor { get; set; }

        protected CancellationTokenSource OnKick;
        public Client(TcpClient client, NetworkStream stream)
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
        public void Ban(string Reason = "", TimeSpan? time = null)
        {
            var list = SaveFiles.Ban.Load().ToList();
            if (time is null)
                list.Add(SaveFiles.Ban.CreateBan(this, Reason));
            else
                list.Add(SaveFiles.Ban.CreateBan(this, Reason, DateTime.UtcNow + time));
            SaveFiles.Ban.Save(list.ToArray());
        }
    }
    public enum EncryptType
    {
        AES,
        RSA
    }
}