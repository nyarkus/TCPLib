using System;

namespace TCPLib.Server
{
    public class ServerIsNotRunning : Exception
    {
        public ServerIsNotRunning() : base("The server is not running") { }
    }
}