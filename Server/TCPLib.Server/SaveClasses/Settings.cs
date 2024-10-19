using System;
using System.IO;

namespace TCPLib.Server.SaveFiles
{
    public class Settings
    {
        public string title { get; set; } = "Untitled";
        public string description { get; set; } = "";
        public int deleteLogsAfterDays { get; set; } = -1;
        public bool saveLogs { get; set; } = true;
        public int maxPlayers { get; set; } = 16;
        public ushort port { get; set; } = 2024;
        internal static ushort DefaultPort { get; set; }

        public static ISettingsSaver saver { get; set; }
        public void Save()
        => saver.Save(this);
        public static Settings Load()
        => saver.Load();

        public Settings()
        {
            port = DefaultPort;
        }
    }
    public interface ISettingsSaver
    {
        void Save(Settings settings);
        Settings Load();
    }
}