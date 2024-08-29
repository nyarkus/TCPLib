// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System.Net.Sockets;
using TCPLib.Server.net;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using TCPLib.Server.Net.Encrypt;

namespace TCPLib.Server.Net
{
    public class Client : NetClient
    {
        public delegate Task Connetion(ResponseCode code, TcpClient client);
        public delegate Task Connetion2(ResponseCode code, Client client);
        public static Connetion2 SuccessfulConnection;
        public static Connetion FailedConnection;
        public static async Task<NetClient> HandleConnections(TcpClient client, TimeSpan timeout)
        {
            try
            {
                CancellationTokenSource cancellation = new CancellationTokenSource();
                Console.Info($"Connection request from {client.Client.RemoteEndPoint}");
                var net = new Client(client, client.GetStream());
                if (Server.settings.maxPlayers <= clients.Count())
                {
                    Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {ResponseCode.ServerIsFull}.");
                    await net.SendAsync(new KickMessage(ResponseCode.ServerIsFull));
                    client.Close();
                    FailedConnection?.Invoke(ResponseCode.ServerIsFull, client);
                    return null;
                }
                string ip = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                foreach (var ban in SaveFiles.Ban.Load())
                {
                    if (ban.IP == ip && (ban.Until is null || ban.Until > DateTime.UtcNow))
                    {
                        Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {ResponseCode.Blocked}.");
                        await net.SendAsync(new KickMessage(ResponseCode.Blocked));
                        client.Close();
                        FailedConnection?.Invoke(ResponseCode.Blocked, client);
                        return null;
                    }
                }
                var serverenc = Encryptor.GetServerEncryptor();

                await net.SendWithoutCryptographyAsync<Key>(new Key() { Value = serverenc.GetRSAPublicKey() });
                net.Encryptor = serverenc;

                Console.Debug("Wait a new keys...");
                var NewKeys = net.ReceiveAsync<AESKey>().Result.Value.Unpack();

                net.Encryptor = net.Encryptor.SetAESKey(NewKeys.Key.ToArray(), NewKeys.IV.ToArray());
                net.EncryptType = EncryptType.AES;

                Console.Debug("Wait a GameInfo...");
                var version = await net.ReceiveAsync<GameInfo>(timeout);
                Console.Debug("Handle a GameInfo...");
                if (version is null)
                {
                    Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {ResponseCode.Timeout}.");
                    await net.SendAsync(new KickMessage(ResponseCode.Timeout));
                    client.Close();
                    FailedConnection?.Invoke(ResponseCode.Timeout, client);
                    return null;
                }
                if (TCPLib.Server.Server.gameInfo?.Name != version?.Value.Name)
                {
                    Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {ResponseCode.BadResponse}.");
                    await net.SendAsync(new KickMessage(ResponseCode.BadResponse));
                    client.Close();
                    FailedConnection?.Invoke(ResponseCode.BadResponse, client);
                    return null;
                }
                if (TCPLib.Server.Server.gameInfo?.Version != version?.Value.Version)
                {
                    Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {ResponseCode.DifferentVersions}.");
                    await net.SendAsync(new KickMessage(ResponseCode.DifferentVersions));
                    client.Close();
                    FailedConnection?.Invoke(ResponseCode.DifferentVersions, client);
                    return null;
                }
                clients.Add(net);
                await net.SendAsync(new RespondCode(ResponseCode.Ok));
                Console.Info($"Successful connection from {client.Client.RemoteEndPoint}");
                SuccessfulConnection?.Invoke(ResponseCode.Ok, net);
                return net;
            }
            catch (Exception ex)
            {
                Console.Error($"There were unexpected errors when connecting from {client.Client.RemoteEndPoint}.");
                Console.Debug(ex);
                FailedConnection?.Invoke(ResponseCode.ServerError, client);
#if DEBUG
                if (TCPLib.Server.Server.TestingMode) throw;
#endif
                return null;
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
            GC.Collect();
        }
        public Client(TcpClient client, NetworkStream stream) : base(client, stream)
        {

        }
    }
    public struct Key : IProtobufSerializable<Key>
    {
        public byte[] Value;

        public Key FromBytes(byte[] bytes)
        {
            var rk = Protobuf.RSAKey.Parser.ParseFrom(bytes);

            return new Key() { Value = rk.Key.ToArray() };
        }

        public byte[] ToByteArray()
        => new Protobuf.RSAKey() { Key = ByteString.CopyFrom(Value) }.ToByteArray();
    }
}