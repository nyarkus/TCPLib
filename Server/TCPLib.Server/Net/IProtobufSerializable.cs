namespace TCPLib.Server.Net;
public interface IProtobufSerializable<T>
{
    public byte[] ToByteArray();
    public abstract static T FromBytes(byte[] bytes);
}
