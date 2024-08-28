namespace TCPLib.Server.Net
{
    public enum ResponseCode
    {
        Ok,
        BadResponse,
        ServerIsFull,
        DifferentVersions,
        Timeout,
        ServerError,
        ServerShutdown,
        Blocked,
        Kicked
    }
}