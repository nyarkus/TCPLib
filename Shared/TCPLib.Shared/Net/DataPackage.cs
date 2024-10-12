// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;

namespace TCPLib.Net
{
    public struct DataPackage<T> where T : IDataSerializable<T>, new()
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }
        public DataPackage(string type, byte[] value)
        {
            Type = type;
            Data = value;
        }
        public DataPackage(string type, IDataSerializable<T> value)
        {
            Type = type;
            Data = value.ToByteArray();
        }
        public byte[] Pack()
        => new TCPLib.Protobuf.DataPackage() { Data = ByteString.CopyFrom(Data), Type = Type }.ToByteArray();

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