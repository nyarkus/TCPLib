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
        private UDPStateSender _UDP;

        public static Settings settings;
        private ServerComponents _components;

        public delegate Task ServerStateChanged();
        public event ServerStateChanged Started;
        public event ServerStateChanged Starting;
        public event ServerStateChanged Stopped;

#if DEBUG
        public static bool TestingMode = false;
#endif
        public Server(IBanListSaver banSaver, ISettingsSaver settingsSaver, ServerComponents components)
        {
            Ban.saver = banSaver;
            Settings.saver = settingsSaver;
            if(components == ServerComponents.All)
            {
                components = ServerComponents.BaseCommands | ServerComponents.UDPStateSender;
            }
            _components = components;
        }
        public Server(ServerConfiguration configuration)
        {
            Ban.saver = configuration.BanSaver;
            Settings.saver = configuration.SettingsSaver;
            if (configuration.Components == ServerComponents.All)
            {
                configuration.Components = ServerComponents.BaseCommands | ServerComponents.UDPStateSender;
            }
            _components = configuration.Components;

            Encryptor.rsaKey = configuration.RSAKeyStrength;
            Encryptor.aesKey = configuration.AESKeySize;
        }
        public void Start()
        {
            var StartTime = Time.TimeProvider.Now;

            settings = Settings.Load();

            _Server = new ServerListener(settings.port);
            if((_components & ServerComponents.UDPStateSender) == ServerComponents.UDPStateSender)
            {
                _UDP = new UDPStateSender(settings.port);
            }

            Console.Initialize(settings.deleteLogsAfterDays);
            Console.SaveLogs = settings.saveLogs;

            if((_components & ServerComponents.BaseCommands) == ServerComponents.BaseCommands)
            {
                Commands.CommandManager.RegAllCommands(this);
            }
            Ban.ClearInvalidBans();

            Starting?.Invoke().Wait();

            Console.Info("Encryption key generation ...");
            Encryptor.GetServerEncryptor();

            Thread tcplistenThread = new Thread(_Server.Initialize().Wait) { Priority = ThreadPriority.AboveNormal };
            if ((_components & ServerComponents.UDPStateSender) == ServerComponents.UDPStateSender)
            {
                var udpListenThread = new Thread(_UDP.Initialize().Wait) { Priority = ThreadPriority.BelowNormal };
                udpListenThread.Start();
            }
            tcplistenThread.Start();

            if (_UDP != null)
                { while (!_Server.Started && !_UDP.Started) ; }
            else
                { while (!_Server.Started) ; }

            Console.Info($"The server successfully started in {(Time.TimeProvider.Now - StartTime).TotalMilliseconds} ms");

#if !NET48
            new Thread(new ParameterizedThreadStart((_) => Started?.Invoke())).Start();
#else
            new Thread(new ThreadStart(() => Started?.Invoke())).Start();
#endif
        }
        public void Stop()
        {
            _Server.Dispose();
            if( _UDP != null )
            { _UDP.Dispose(); }
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
