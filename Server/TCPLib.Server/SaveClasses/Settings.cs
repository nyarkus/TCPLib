using System;
using System.IO;

namespace TCPLib.Server.SaveFiles
{
    public class Settings
    {
        public string title = "Untitled";
        public string description = "";
        public int deleteLogsAfterDays = -1;
        public bool saveLogs = true;
        public int maxPlayers = 16;
        public ushort port = 2024;

        public static ISettingsSaver saver;
        public void Save()
        => saver.Save(this);
        public static Settings Load()
        => saver.Load();
    }
    public interface ISettingsSaver
    {
        void Save(Settings settings);
        Settings Load();
    }
}