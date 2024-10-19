using Org.BouncyCastle.Security.Certificates;
using System;
using TCPLib.Encrypt;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net;
using TCPLib.Shared.Base;
using System.IO;

namespace TCPLib.Client.Net
{
    public delegate Task ServerKicked(KickMessage response);
    public class Server : NetworkingBase, IDisposable
    {

        public event ServerKicked Kicked;

        public IP IP { get; private set; }

        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }

        #region Networking
        protected override Stream Stream
        {
            get
            {
                return stream;
            }
        }
        protected override async Task<bool> Handle(DataPackageSource package)
        {
            if (package.Type == "KickMessage")
            {
                var kick = new KickMessage().FromBytes(package.Data);
                switch (kick.code)
                {
                    case ResponseCode.Kicked:
                        if (Kicked != null)
                        {
                            await Kicked.Invoke(kick);
                        }
                        return true;
                }
            }
            return false;
        }
        #endregion

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
        private bool disposed;
        public override void Dispose()
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
    
}