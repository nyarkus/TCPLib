using Org.BouncyCastle.Security.Certificates;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net;

namespace TCPLib.Client.Net
{
    public delegate Task ServerKicked(KickMessage response);
    public partial class Server : IDisposable
    {
        public EncryptType EncryptType { get; set; } = EncryptType.RSA;

        public event ServerKicked Kicked;

        public IP IP { get; private set; }

        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }

        public Encryptor encryptor { get; set; }

        protected CancellationTokenSource OnKick;

        public Server(IP ip, TcpClient client, NetworkStream stream)
        {
            IP = ip;
            this.client = client;
            this.stream = stream;

            OnKick = new CancellationTokenSource();
        }

        public async Task Disconnect()
        {
            var kicked = new KickMessage(ResponseCode.DisconnectedByUser);
            await SendAsync(kicked);
            OnKick.Cancel();

            stream?.Close();
            stream?.Dispose();
            client?.Dispose();

            if(Kicked != null)
            {
                await Kicked.Invoke(kicked);
            }
        }
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Disconnect().GetAwaiter().GetResult();
                _semaphore.Dispose();
            }

            disposed = true;
        }

        ~Server()
        {
            Dispose(false);
        }
    }
    public enum EncryptType
    {
        AES,
        RSA
    }
}