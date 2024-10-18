using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using TCPLib.Server.Net.Encrypt;
using TCPLib.Classes;
using System.Threading;
using TCPLib.Net;

namespace TCPLib.Server.Net
{
    public partial class Client : IDisposable
    {
        public EncryptType EncryptType { get; private set; } = EncryptType.RSA;

        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }
        public uint id { get; private set; }
        public IP IP
        {
            get
            {
                return client.Client.RemoteEndPoint.ToString();
            }
        }

        public static IReadOnlyCollection<Client> clients
        {
            get
            {
                return _clients;
            }
        }
        protected static List<Client> _clients { get; set; } = new List<Client>();
        public Encryptor Encryptor { get; set; }
        /// <summary>
        /// If the client disconnects from the server or terminates the connection unexpectedly - the value of this variable will be <c>false</c>
        /// </summary>
        public bool IsAlive { get; private set; }

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

            IsAlive = false;
            client.Close();
            _clients.Remove(this);
            _semaphore.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) 
                return;

            if(disposing)
            {
                if (stream != null)
                {
                    Kick(new KickMessage(ResponseCode.DisconnectedByUser)).Wait();
                }
                else
                {
                    OnDisconnected();
                }
            }

            disposed = true;
        }
        ~Client()
        {
            Dispose(false);
        }
        public void Ban(string Reason = "", TimeSpan? time = null)
        {
            var list = SaveFiles.Ban.Load().ToList();
            if (time is null)
                list.Add(SaveFiles.Ban.CreateBan(this, Reason));
            else
                list.Add(SaveFiles.Ban.CreateBan(this, Reason, Time.TimeProvider.Now + time));
            SaveFiles.Ban.Save(list.ToArray());
        }
    }
    public enum EncryptType
    {
        AES,
        RSA
    }
}