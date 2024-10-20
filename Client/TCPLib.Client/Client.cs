using System.Net;
using System.Net.Sockets;
using TCPLib.Client.Net;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net;
using TCPLib.Encrypt;

namespace TCPLib.Client
{
    public class Client
    {
        public TcpClient tcpClient { get; private set; }
        public Server ConnectedServer { get; private set; }

        private static int _aesKeySize;

        public Client() : this(new ClientConfiguration())
        {
        }
        public Client(ClientConfiguration settings)
        {
            tcpClient = new TcpClient();
            _aesKeySize = settings.AesKeySize;
        }
        public async Task<Server> Connect(IP ip)
        {
            if (tcpClient.Connected) throw new Exceptions.ClientAlredyConnected($"{ConnectedServer?.IP}");
            tcpClient.Connect(ip.RemovePort(), ip.Port.Value);
            
            Server server = new Server(ip, tcpClient, tcpClient.GetStream());

            var key = await server.ReceiveAsync<Key>(false);

            server.Encryptor = new Encryptor(_aesKeySize);
            server.Encryptor.SetPublicRSAKey(key.Value.Value);
            
            var maxAESSize = key.Value.MaxAESSize;
            var aesKey = server.Encryptor.GetAESKey();
            if (aesKey.Key.Length > maxAESSize)
            {
                server.Encryptor.RegenerateAESKey(maxAESSize);
                aesKey = server.Encryptor.GetAESKey();
            }

            await server.SendAsync(aesKey);

            server.Encryptor.SetAESKey(aesKey.Key, aesKey.IV);
            server.setEncryptionType(EncryptType.AES);

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