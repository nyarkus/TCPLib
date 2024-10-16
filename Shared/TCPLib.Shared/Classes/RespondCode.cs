// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using TCPLib.Net;
using Google.Protobuf;

namespace TCPLib.Classes
{
    public struct RespondCode : IDataSerializable<RespondCode>
    {
        public ResponseCode code { get; set; }

        public RespondCode FromBytes(byte[] bytes)
        {
            var rc = Protobuf.RespondCode.Parser.ParseFrom(bytes);

            return new RespondCode((ResponseCode)rc.Code);
        }

        public byte[] ToByteArray()
        => new Protobuf.RespondCode() { Code = (Protobuf.Code)code }.ToByteArray();

        public RespondCode(ResponseCode code)
        { this.code = code; }

    }
}
