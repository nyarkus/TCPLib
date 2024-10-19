using System;
using System.Threading.Tasks;
using System.Threading;
using TCPLib.Classes;

namespace TCPLib.Server
{
    using TCPLib.Server.Net;
    using TCPLib.Server.SaveFiles;

    public class Server : IDisposable
    {
        private ServerListener _Server;
        private UDPStateSender _UDP;

        public static Settings settings { get; set; }
        private readonly ServerComponents _components;

        public delegate Task ServerStateChanged();
        public event ServerStateChanged Started;
        public event ServerStateChanged Starting;
        public event ServerStateChanged Stopped;

#if DEBUG
        public static bool TestingMode = false;
#endif
        public Server(IBanListSaver banSaver, ISettingsSaver settingsSaver, ServerComponents components) : this(new ServerConfiguration(banSaver, settingsSaver, components)) { }
        public Server(ServerConfiguration configuration)
        {   
            Settings.DefaultPort = configuration.DefaultPort;

            if (configuration.Components == ServerComponents.All)
            {
                configuration.Components = ServerComponents.BaseCommands | ServerComponents.UDPStateSender;
            }
            _components = configuration.Components;

            ServerEncryptor.RsaKeySize = configuration.RSAKeyStrength;
            ServerEncryptor.AesKeySize = configuration.AESKeySize;

            Ban.saver = configuration.BanSaver;
            Settings.saver = configuration.SettingsSaver;
        }
        public async Task Start()
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

            if(Starting != null)
                await Starting?.Invoke();

            Console.Info("Encryption key generation ...");
            ServerEncryptor.GetServerEncryptor();

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
            if(Started != null)
                new Thread(new ParameterizedThreadStart((_) => Started?.Invoke())).Start();
#else
            if(Started != null)
                new Thread(new ThreadStart(() => Started?.Invoke())).Start();
#endif
        }
        public void Stop()
        {
            if(Stopped != null)
                Stopped?.Invoke();
        }
        public void ConsoleRead(CancellationToken cancellation = default)
        {
            while (!cancellation.IsCancellationRequested)
            {
                Commands.CommandManager.HandleLine(System.Console.ReadLine());
            }
        }

        private bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(disposed) return;

            if (disposing)
            {
                _Server.Dispose();
                if (_UDP != null)
                { _UDP.Dispose(); }

                settings = null;

                Started = null;
                Starting = null;
                Stopped = null;
            }

            disposed = true;
        }
        ~Server() 
        {
            Dispose(false);
        }
    }
}
