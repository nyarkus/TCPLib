using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPLib.Classes;

namespace TCPLib.Server.Net
{
    public partial class Client
    {
        public delegate Task Connetion(ResponseCode code, TcpClient client);
        public delegate Task Connetion2(ResponseCode code, Client client);
        public static Connetion2 SuccessfulConnection;
        public static Connetion FailedConnection;
        public static async Task<Client> HandleConnections(TcpClient client, TimeSpan timeout)
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
                var serverenc = Encrypt.Encryptor.GetServerEncryptor();

                await net.SendAsync(new Key() { Value = serverenc.GetRSAPublicKey(), MaxAESSize = Encrypt.Encryptor.aesKey }, false);
                net.Encryptor = serverenc;

                Console.Debug("Wait a new keys...");
                var NewKeys = net.ReceiveAsync<AESKey>().Result.Unpack();
                if(NewKeys.Key.Length > Encrypt.Encryptor.aesKey)
                {
                    Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {ResponseCode.BadResponse}.");
                    await net.SendAsync(new KickMessage(ResponseCode.BadResponse));
                    client.Close();
                    FailedConnection?.Invoke(ResponseCode.Blocked, client);
                    return null;
                }

                net.Encryptor = net.Encryptor.SetAESKey(NewKeys.Key.ToArray(), NewKeys.IV.ToArray());
                net.EncryptType = EncryptType.AES;

                _clients.Add(net);
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
    }
}
