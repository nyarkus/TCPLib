﻿using TCPLib.Client.Net;

namespace TCPLib.Client.Exceptions;

public class ServerConnectionException : Exception
{
    public ServerConnectionException(ResponseCode code) : base($"During the connection, the server responded with an error code: {code}") { }
}
