namespace TCPLib.Classes
{
    public enum ResponseCode
    {
        Ok,
        BadResponse,
        ServerIsFull,
        DifferentVersions,
        Timeout,
        ServerError,
        Kicked,
        DisconnectedByUser
    }
}
