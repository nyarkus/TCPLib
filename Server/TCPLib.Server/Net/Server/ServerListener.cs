using System;
using System.Threading;
using System.Threading.Tasks;
namespace TCPLib.Server.Net
{
    using System.Net;
    using System.Net.Sockets;

    public class ServerListener : IDisposable
    {
        private readonly TcpListener Listener;
        private readonly int Port;
        private readonly CancellationTokenSource StopToken;
        public bool Started { get; private set; }

        public ServerListener(int port)
        {
            Listener = new TcpListener(IPAddress.Any, port);
            Port = port;
            StopToken = new CancellationTokenSource();
        }
        private void Start()
        {
            Console.Info("Initialisation of the listening...");
            try
            {
                Listener.Start();
            }
            catch (SocketException ex)
            {
                switch(ex.ErrorCode)
                {
                    case 10048:
                        Console.Error("IP address or port not port cannot be used as it is already in use");
                        break;
                    case 10049:
                        Console.Error("The specified IP address or port does not belong to this computer");
                        break;
                }
                throw;
            }

        }
        public async Task Initialize()
        {
            Start();
            await Listen();
        }
        public virtual async Task Listen()
        {
            Console.Info($"Listening is running on {Port}");
            Started = true;
            while (true)
            {
                if (StopToken.IsCancellationRequested) 
                    return;

                Console.Debug("Wait connetction...");

                var client = await Listener.AcceptTcpClientAsync();
                Console.Debug("Connection request! Handle...");

                new Thread(async () =>
                {
                    await Client.HandleConnections(client, TimeSpan.FromSeconds(5));
                }).Start();
            }
        }

        public void Dispose()
        {
            StopToken.Cancel();
            StopToken.Dispose();
#if NET8_0_OR_GREATER
            Listener.Dispose();
#endif
        }
    }
}