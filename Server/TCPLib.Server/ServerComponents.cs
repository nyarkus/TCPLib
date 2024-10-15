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
        None,
        BaseCommands,
        UDPStateSender,
        All,
    }
}
