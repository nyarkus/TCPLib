namespace TCPLib.Server.Net
{
    public interface IProtobufSerializable<T>
    {
        byte[] ToByteArray();
        T FromBytes(byte[] bytes);
    }
}
