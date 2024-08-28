namespace TCPLib.Server.Net.Classes;

public class PackageSource
{
    public string Type;
    public byte[] Data;

    public PackageSource(string type, byte[] data)
    {
        Type = type;
        Data = data;
    }

}
