// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using TCPLib.Net;
using System.Linq;
using System;

namespace TCPLib.Classes
{
    public struct AESKey : IDataSerializable<AESKey>, IEquatable<AESKey>
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public bool Equals(AESKey other)
            => Key.Equals(other.Key) && IV.Equals(other.IV);
        

        public AESKey FromBytes(byte[] bytes)
        {
            var aes = Protobuf.AESKey.Parser.ParseFrom(bytes);

            return new AESKey { Key = aes.Key.ToArray(), IV = aes.IV.ToArray() };
        }

        public byte[] ToByteArray() =>
            new Protobuf.AESKey { Key = ByteString.CopyFrom(Key), IV = ByteString.CopyFrom(IV) }.ToByteArray();

    }
}
