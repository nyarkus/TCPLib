using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TCPLib.Classes;

namespace TCPLib.Client.Net
{
    public partial class Server
    {
        public EncryptType EncryptType { get; set; } = EncryptType.RSA;

        public delegate Task ServerKicked(KickMessage response);
        public event ServerKicked Kicked;
        public event ServerKicked Banned;
        public event ServerKicked ServerShutdown;

        public IPAddress IP { get; set; }
        public int Port { get; set; }

        public TcpClient client { get; private set; }
        public NetworkStream stream { get; private set; }

        public Encryptor encryptor;

        protected CancellationTokenSource OnKick;

        public Server(IPAddress ip, int port, TcpClient client, NetworkStream stream)
        {
            IP = ip;
            Port = port;
            this.client = client;
            this.stream = stream;

            OnKick = new CancellationTokenSource();
        }

        public async Task Disconnect()
        {
            var kicked = new KickMessage(ResponseCode.DisconnectedByUser);

            await SendAsync(kicked);
            OnKick.Cancel();

            stream.Close();
            stream.Dispose();
            client.Dispose();

            if(Kicked != null)
                await Kicked.Invoke(kicked);
        }

    }
    public enum EncryptType
    {
        AES,
        RSA
    }
}