// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System.Net;
using System.Net.Sockets;
using TCPLib.Client.Net;
using System.Threading.Tasks;
using System.Linq;

namespace TCPLib.Client
{
    public sealed partial class Client
    {
        public TcpClient tcpClient;
        public GameInfo gameInfo;
        public Server ConnectedServer;
        public Client()
        {
            tcpClient = new TcpClient();
            gameInfo = new GameInfo("Untitled", "1");
        }
        public Client(GameInfo gameInfo)
        {
            tcpClient = new TcpClient();
            this.gameInfo = gameInfo;
        }
        public async Task<Server> Connect(IPAddress address, int port)
        {
            if (tcpClient.Connected) throw new Exceptions.ClientAlredyConnected($"{ConnectedServer?.IP}:{ConnectedServer?.Port}");
            tcpClient.Connect(address, port);
            Server server = new Server(address, port, tcpClient, tcpClient.GetStream());

            var key = await server.ReceiveWithoutCryptographyWithProcessing<Key>();
            var encryptor = Encryptor.GenerateNew();

            server.encryptor = Encryptor.GetEncryptor().SetRSAKey(encryptor.GetRSAPrivateKey());

            var aeskey = encryptor.GetAESKey();
            await server.SendAsync(aeskey);

            server.encryptor.SetAESKey(aeskey.Key, aeskey.IV);
            server.EncryptType = EncryptType.AES;

            await server.SendAsync(gameInfo);
            var code = await server.ReceiveAsync<RespondCode>();
            if (code.Value.Value.code != ResponseCode.Ok)
                throw new Exceptions.ServerConnectionException(code.Value.Value.code);
            ConnectedServer = server;
            return server;
        }
    }
    public struct Key : IProtobufSerializable<Key>
    {
        public byte[] Value { get; set; }

        public Key FromBytes(byte[] bytes)
        {
            var rk = Protobuf.RSAKey.Parser.ParseFrom(bytes);

            return new Key() { Value = rk.Key.ToArray() };
        }

        public byte[] ToByteArray()
        => new Protobuf.RSAKey() { Key = ByteString.CopyFrom(Value) }.ToByteArray();
    }
}