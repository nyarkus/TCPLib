namespace TCPLib.Net
{
    public interface IDataSerializable<out T>
    {
        byte[] ToByteArray();
        T FromBytes(byte[] bytes);
    }
}
