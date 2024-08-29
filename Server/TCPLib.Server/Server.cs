using System;
using System.Threading.Tasks;
using System.Threading;
using TCPLib.Classes;

namespace TCPLib.Server
{
    using TCPLib.Server.Net;
    using TCPLib.Server.Net.Encrypt;
    using TCPLib.Server.SaveFiles;

    public class Server
    {
        private ServerListener _Server;
        private UDPListener _UDP;
        public ushort Port;

        public static Settings settings;

        public delegate Task ServerD();
        public event ServerD Started;
        public event ServerD Starting;
        public event ServerD Stopped;


#if DEBUG
        public static bool TestingMode = false;
#endif
        public Server(IBanListSaver banSaver, ISettingsSaver settingsSaver)
        {
            Ban.saver = banSaver;
            Settings.saver = settingsSaver;
        }
        public void Start()
        {
            var StartTime = DateTime.UtcNow;

            settings = Settings.Load();

            _Server = new ServerListener(settings.port);
            _UDP = new UDPListener(settings.port);
            Port = settings.port;

            Console.Initialize(settings.deleteLogsAfterDays);
            Console.SaveLogs = settings.saveLogs;

            Commands.CommandManager.RegAllCommands(this);
            Ban.ClearInvalidBans();

            Starting?.Invoke();

            Console.Info("Encryption key generation ...");
            Encryptor.GetServerEncryptor();

            Thread tcplistenThread = new Thread(_Server.Initialize) { Priority = ThreadPriority.AboveNormal };
            var udpListenThread = new Thread(_UDP.Initialize) { Priority = ThreadPriority.BelowNormal };
            tcplistenThread.Start();
            udpListenThread.Start();

            while (!_Server.Started && !_UDP.Started) ;

            Console.Info($"The server successfully started in {(DateTime.UtcNow - StartTime).TotalMilliseconds} ms");

#if !NET48
            new Thread(new ParameterizedThreadStart((_) => Started?.Invoke())).Start();
#else
            new Thread(new ThreadStart(() => Started?.Invoke())).Start();
#endif
        }
        public async void Stop()
        {
            Console.Info("Alerting everyone about the shutdown ...");
            while (Client.clients.Count > 0)
            {
                await Client.clients[0].Kick(new KickMessage(ResponseCode.ServerShutdown));
            }
            _Server.Dispose();
            _UDP.Dispose();
            Console.Info("Bye!!!");
            Stopped?.Invoke();
        }
        public void ConsoleRead()
        {
            while (true)
            {
                Commands.CommandManager.HandleLine(System.Console.ReadLine());
            }
        }

    }
}
