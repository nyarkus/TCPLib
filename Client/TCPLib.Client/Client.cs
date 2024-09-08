using System.Net;
using System.Net.Sockets;
using TCPLib.Client.Net;
using System.Threading.Tasks;
using TCPLib.Classes;

namespace TCPLib.Client
{
    public sealed partial class Client
    {
        public TcpClient tcpClient;
        public Server ConnectedServer;
        public Client()
        {
            tcpClient = new TcpClient();
        }
        public async Task<Server> Connect(IPAddress address, int port)
        {
            if (tcpClient.Connected) throw new Exceptions.ClientAlredyConnected($"{ConnectedServer?.IP}:{ConnectedServer?.Port}");
            tcpClient.Connect(address, port);
            Server server = new Server(address, port, tcpClient, tcpClient.GetStream());

            var key = await server.ReceiveWithoutCryptographyWithProcessingAsync<Key>();

            server.encryptor = new Encryptor();
            server.encryptor.SetPublicRSAKey(key.Value.Value.Value);

            var aesKey = server.encryptor.GetAESKey();
            await server.SendAsync(aesKey);

            server.encryptor.SetAESKey(aesKey.Key, aesKey.IV);
            server.EncryptType = EncryptType.AES;

            var code = await server.ReceiveAsync<RespondCode>();
            if (code.Value.Value.code != ResponseCode.Ok)
                throw new Exceptions.ServerConnectionException(code.Value.Value.code);
            ConnectedServer = server;
            return server;
        }
    }
}