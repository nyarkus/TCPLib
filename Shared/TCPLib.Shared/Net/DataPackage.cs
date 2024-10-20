// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System;
using TCPLib.Classes;

namespace TCPLib.Net
{
    public struct DataPackage<T> : IEquatable<DataPackage<T>> where T : IDataSerializable<T>, new()
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }
        public DataPackage(string type, byte[] value)
        {
            Type = type;
            Data = value;
            _cache = default;
            _cached = false;
        }
        public DataPackage(string type, IDataSerializable<T> value)
        {
            Type = type;
            Data = value.ToByteArray();
            _cache = default;
            _cached = false;
        }
        public byte[] Pack()
        => new TCPLib.Protobuf.DataPackage { Data = ByteString.CopyFrom(Data), Type = Type }.ToByteArray();
        public DataPackageSource ToSource()
            => new DataPackageSource(Type, Data);

        public T Unpack()
        {
            T instance = new T();
            return instance.FromBytes(Data);
        }

        public bool Equals(DataPackage<T> other)
        {
            return Data.Equals(other.Data) && other.Type == Type;
        }

        private T _cache;
        bool _cached;
        public T Value
        {
            get
            {
                if(!_cached)
                {
                    _cache = Unpack();
                    _cached = true;
                }
                return _cache;
            }
        }

    }
}