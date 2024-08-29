// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using TCPLib.Net;
using System.Linq;


namespace TCPLib.Classes
{
    public struct Key : IProtobufSerializable<Key>
    {
        public byte[] Value;

        public Key FromBytes(byte[] bytes)
        {
            var rk = Protobuf.RSAKey.Parser.ParseFrom(bytes);

            return new Key() { Value = rk.Key.ToArray() };
        }

        public byte[] ToByteArray()
        => new Protobuf.RSAKey() { Key = ByteString.CopyFrom(Value) }.ToByteArray();
    }
}
