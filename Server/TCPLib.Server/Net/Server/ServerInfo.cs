namespace TCPLib.Server.Net
{
    public class ServerInfo
    {
        public int MaxPlayers { get; set; }
        public int Players { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
