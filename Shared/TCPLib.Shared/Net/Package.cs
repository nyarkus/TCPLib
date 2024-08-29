// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;

namespace TCPLib.Net
{
    public struct Package<T> where T : IProtobufSerializable<T>, new()
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }
        public Package(string type, byte[] value)
        {
            Type = type;
            Data = value;
        }
        public Package(string type, IProtobufSerializable<T> value)
        {
            Type = type;
            Data = value.ToByteArray();
        }
        public byte[] Pack()
        => new TCPLib.Protobuf.Package() { Data = ByteString.CopyFrom(Data), Type = Type }.ToByteArray();

        public T Unpack()
        {
            T instance = new T();
            return instance.FromBytes(Data);
        }
        public T Value
        {
            get
            {
                return Unpack();
            }
        }

    }
}