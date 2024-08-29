namespace TCPLib.Net
{
    public interface IProtobufSerializable<T>
    {
        byte[] ToByteArray();
        T FromBytes(byte[] bytes);
    }
}
