// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using TCPLib.Net;
using System.Linq;
using System;


namespace TCPLib.Classes
{
    public struct Key : IDataSerializable<Key>, IEquatable<Key>
    {
        public byte[] Value { get; set; }
        public int MaxAESSize { get; set; }

        public bool Equals(Key other)
            => MaxAESSize == other.MaxAESSize && Value.Equals(other.Value);

        public Key FromBytes(byte[] bytes)
        {
            var rk = Protobuf.Key.Parser.ParseFrom(bytes);

            return new Key { Value = rk.Key_.ToArray(), MaxAESSize = rk.Mas };
        }

        public byte[] ToByteArray()
        => new Protobuf.Key { Key_ = ByteString.CopyFrom(Value), Mas = MaxAESSize }.ToByteArray();
    }
}
