namespace TCPLib.Client.Net;
public interface IProtobufSerializable<T>
{
    public byte[] ToByteArray();
    public abstract static T FromBytes(byte[] bytes);
}
