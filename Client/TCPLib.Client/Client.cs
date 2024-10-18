using System.Net;
using System.Net.Sockets;
using TCPLib.Client.Net;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net;

namespace TCPLib.Client
{
    public class Client
    {
        public TcpClient tcpClient { get; private set; }
        public Server ConnectedServer { get; private set; }
        public Client() : this(new ClientConfiguration())
        {
        }
        public Client(ClientConfiguration settings)
        {
            tcpClient = new TcpClient();
            Encryptor.AesKeySize = settings.AesKeySize;
        }
        public async Task<Server> Connect(IP ip)
        {
            if (tcpClient.Connected) throw new Exceptions.ClientAlredyConnected($"{ConnectedServer?.IP}");
            tcpClient.Connect(ip.RemovePort(), ip.Port.Value);
            
            Server server = new Server(ip, tcpClient, tcpClient.GetStream());

            var key = await server.ReceiveAsync<Key>(false);

            server.encryptor = new Encryptor();
            server.encryptor.SetPublicRSAKey(key.Value.Value);
            
            var maxAESSize = key.Value.MaxAESSize;
            var aesKey = server.encryptor.GetAESKey();
            if (aesKey.Key.Length > maxAESSize)
            {
                server.encryptor.RegenerateAESKey(maxAESSize);
                aesKey = server.encryptor.GetAESKey();
            }

            await server.SendAsync(aesKey);

            server.encryptor.SetAESKey(aesKey.Key, aesKey.IV);
            server.EncryptType = EncryptType.AES;

            var code = await server.ReceiveAsync<RespondCode>();
            if (code.Value.code != ResponseCode.Ok)
            {
                throw new Exceptions.ServerConnectionException(code.Value.code);
            }
            ConnectedServer = server;
            return server;
        }
    }
}