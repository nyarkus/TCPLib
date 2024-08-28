namespace TCPLib.Client.Exceptions;
public class ClientAlredyConnected : Exception
{
    public ClientAlredyConnected(string ip) : base("The connection is already set to " + ip) { }
}