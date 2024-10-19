using System;
using System.IO;
using Newtonsoft.Json;
using TCPLib.Server.SaveFiles;

namespace ExampleServer
{
    public struct SettingsSaver : ISettingsSaver
    {
        public Settings? settings;

        public SettingsSaver() { }

        public Settings Load()
        {
            if(settings == null)
                settings = new Settings();

            return settings;
        }

        public void Save(Settings settings)
            => this.settings = settings;
    }
    public struct BanSaver : IBanListSaver
    {
        Ban[] bans = [];

        public BanSaver() { }

        public Ban[] Load()
            => bans;

        public void Save(Ban[] bans)
            => this.bans = bans;
    }
}
