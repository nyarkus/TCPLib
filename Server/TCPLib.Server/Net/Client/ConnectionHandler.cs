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
        static async Task _failConnection(TcpClient client, Client net, ResponseCode code)
        {
            Console.Info($"Connection from {client.Client.RemoteEndPoint} rejected because: {code}.");
            await net.SendAsync(new KickMessage(code));
            client.Close();
            FailedConnection?.Invoke(ResponseCode.ServerIsFull, client);
        }
        public static async Task<Client> HandleConnections(TcpClient client, TimeSpan timeout)
        {
            try
            {
                CancellationTokenSource cancellation = new CancellationTokenSource();
                Console.Info($"Connection request from {client.Client.RemoteEndPoint}");
                var net = new Client(client, client.GetStream());
                if (Server.settings.maxPlayers <= clients.Count())
                {
                    await _failConnection(client, net, ResponseCode.ServerIsFull);
                    return null;
                }
                string ip = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                foreach (var ban in SaveFiles.Ban.Load())
                {
                    if (ban.IP == ip && (ban.Until is null || ban.Until > DateTime.UtcNow))
                    {
                        await _failConnection(client, net, ResponseCode.Blocked);
                        return null;
                    }
                }
                var serverenc = Encrypt.Encryptor.GetServerEncryptor();

                await net.SendAsync(new Key() { Value = serverenc.GetRSAPublicKey(), MaxAESSize = Encrypt.Encryptor.aesKey }, false);
                net.Encryptor = serverenc;

                Console.Debug("Wait a new keys...");
                var NewKeys = net.ReceiveAsync<AESKey>(timeout).Result;
                if(!NewKeys.HasValue)
                {
                    await _failConnection(client, net, ResponseCode.Timeout);
                    return null;
                }
                if(NewKeys.Value.Value.Key.Length > Encrypt.Encryptor.aesKey)
                {
                    await _failConnection(client, net, ResponseCode.BadResponse);
                    return null;
                }

                net.Encryptor = net.Encryptor.SetAESKey(NewKeys.Value.Value.Key.ToArray(), NewKeys.Value.Value.IV.ToArray());
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
