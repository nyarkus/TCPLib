// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System;
using TCPLib.Net;

namespace TCPLib.Classes
{
    public struct KickMessage : IDataSerializable<KickMessage>, IEquatable<KickMessage>
    {
        public string reason { get; set; }
        public ResponseCode code { get; set; }
        public KickMessage(ResponseCode code, string reason = "")
        {
            this.reason = reason;
            this.code = code;
        }

        public KickMessage FromBytes(byte[] bytes)
        {
            var km = TCPLib.Protobuf.KickMessage.Parser.ParseFrom(bytes);

            return new KickMessage((ResponseCode)km.Code, km.Reason);
        }

        public byte[] ToByteArray()
        {
            return new TCPLib.Protobuf.KickMessage { Code = (TCPLib.Protobuf.Code)code, Reason = reason }.ToByteArray();
        }

        public bool Equals(KickMessage other)
            => reason == other.reason && code == other.code;
    }
}
