namespace TCPLib.Net
{
    public interface IDataSerializable<T>
    {
        byte[] ToByteArray();
        T FromBytes(byte[] bytes);
    }
}
