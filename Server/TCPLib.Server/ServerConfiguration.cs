using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Server.SaveFiles;

namespace TCPLib.Server
{
    public class ServerConfiguration
    {
        public IBanListSaver banSaver;
        public ISettingsSaver settingsSaver;
        public ServerComponents components;

        public int AESKeySize = 128;
        public int RSAKeyStrength = 2048;

        public ServerConfiguration(IBanListSaver banSaver, ISettingsSaver settingsSaver, ServerComponents components)
        {
            this.banSaver = banSaver;
            this.settingsSaver = settingsSaver;
            this.components = components;
        }
    }
}
