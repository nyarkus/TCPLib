using System;
using System.Text;
using TCPLib.Net;

namespace ExampleClient
{
    public struct Message : IDataSerializable<Message>, IEquatable<Message>
    {
        public string Data { get; set; }

        public bool Equals(Message other)
        {
            return Data == other.Data;
        }

        public Message FromBytes(byte[] bytes)
        {
            return new Message { Data = Encoding.UTF8.GetString(bytes) };
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(Data);
        }
    }
}
