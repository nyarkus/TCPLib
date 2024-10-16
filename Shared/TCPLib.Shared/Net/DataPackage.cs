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
            _cache = default;
        }
        public DataPackage(string type, IDataSerializable<T> value)
        {
            Type = type;
            Data = value.ToByteArray();
            _cache = default;
        }
        public byte[] Pack()
        => new TCPLib.Protobuf.DataPackage() { Data = ByteString.CopyFrom(Data), Type = Type }.ToByteArray();

        public T Unpack()
        {
            T instance = new T();
            return instance.FromBytes(Data);
        }
        private T _cache;
        public T Value
        {
            get
            {
                if(_cache.Equals(new T()))
                {
                    _cache = Unpack();
                }
                return _cache;
            }
        }

    }
}