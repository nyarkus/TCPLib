using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Encrypt;

namespace TCPLib.Server.Net
{
    internal static class ServerEncryptor
    {
        private static Encryptor _serverEncryptor;
        internal static int AesKeySize { get; set; } = 128; 
        internal static int RsaKeySize { get; set; } = 2048;
        internal static Encryptor GetServerEncryptor()
        {
            if (_serverEncryptor != null)
                return _serverEncryptor;

            _serverEncryptor = new Encryptor(128, 2048);
            return _serverEncryptor;
        }
    }
}
