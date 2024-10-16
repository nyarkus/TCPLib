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
        private readonly IBanListSaver banSaver;
        private readonly ISettingsSaver settingsSaver;
        private ServerComponents components;

        private int aesKeySize { get; set; } = 128;
        private int rsaKeyStrength { get; set; } = 2048;

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
            internal set { components = value; }
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
