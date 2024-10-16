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
        private IBanListSaver banSaver;
        private ISettingsSaver settingsSaver;
        private ServerComponents components;

        private int aesKeySize = 128;
        private int rsaKeyStrength = 2048;

        public ServerConfiguration(IBanListSaver banSaver, ISettingsSaver settingsSaver, ServerComponents components)
        {
            this.banSaver = banSaver;
            this.settingsSaver = settingsSaver;
            this.components = components;
        }

        public IBanListSaver BanSaver
        {
            get { return banSaver; }
        }

        public ISettingsSaver SettingsSaver
        {
            get { return settingsSaver; }
        }

        public ServerComponents Components
        {
            get { return components; }
        }

        public int AESKeySize
        {
            get { return aesKeySize; }
            set { aesKeySize = value; }
        }

        public int RSAKeyStrength
        {
            get { return rsaKeyStrength; }
            set { rsaKeyStrength = value; }
        }
    }

}
