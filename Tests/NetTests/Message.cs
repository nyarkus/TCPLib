using System.Text;
using TCPLib.Net;

namespace Tests
{
#nullable disable
    public class Message : IDataSerializable<Message>
    {
        public string Data;

        public Message FromBytes(byte[] bytes)
        {
            return new Message() { Data = Encoding.UTF8.GetString(bytes) };
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(Data);
        }
    }
}
