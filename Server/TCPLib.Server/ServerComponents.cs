using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPLib.Server
{
    [Flags]
    public enum ServerComponents
    {
        None = 0,
        BaseCommands = 1 << 0,
        UDPStateSender = 1 << 1,
        All = 1 << 2,
    }
}
