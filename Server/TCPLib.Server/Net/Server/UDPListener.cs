using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCPLib.Server.Net
{
    public class UDPListener : IDisposable
    {
        private UdpClient Listener;
        private int Port;
        private readonly CancellationTokenSource StopToken;
        public bool Started { get; private set; } = false;

        public UDPListener(int port)
        {
            Port = port;
            StopToken = new CancellationTokenSource();
        }

        public void Dispose()
        {
            StopToken.Cancel();
            Listener.Dispose();
        }

        private void Start()
        {
            Console.Debug("Initialisation of the UDP listening...");
            try
            {
                Listener = new UdpClient(Port);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    Console.Error("IP address or port not port cannot be used as it is already in use");
                }
                else if (ex.ErrorCode == 10049)
                {
                    Console.Error("The specified IP address or port does not belong to this computer");
                }
                throw;
            }
        }

        public async void Initialize()
        {
            Start();
            await Listen();
        }

        public virtual async Task Listen()
        {

            byte[] respond = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new ServerInfo()
            {
                Description = Server.settings.description,
                Name = Server.settings.title,
                MaxPlayers = Server.settings.maxPlayers,
                Players = Client.clients.Count
            }));
            while (true)
            {
                try
                {
                    if (StopToken.IsCancellationRequested) return;

                    var result = await Listener.ReceiveAsync();

                    await Listener.SendAsync(respond, respond.Length, result.RemoteEndPoint);
                }
                catch { }
            }
        }
    }
}
