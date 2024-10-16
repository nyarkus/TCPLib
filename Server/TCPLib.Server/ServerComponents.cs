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
        /// <summary>
        /// These are commands like a ban, stop, kick etc.
        /// </summary>
        BaseCommands = 1 << 0,
        /// <summary>
        /// If the server receives any packet via UDP, it will send information about itself (Name, description, number of players and their maximum number). 
        /// </summary>
        UDPStateSender = 1 << 1,
        All = 1 << 2,
    }
}
