using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPLib.Client
{
    public class ClientConfiguration
    {
        public int AesKeySize
        {
            get
            {
                return _aesKeySize;
            }
            set
            {
                _aesKeySize = value;
            }
        }
        private int _aesKeySize = 128;
    }
}
