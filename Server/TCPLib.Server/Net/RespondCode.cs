// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using TCPLib.Server.Net;

namespace TCPLib.Server.net;

public class RespondCode : IProtobufSerializable<RespondCode>
{
    public ResponseCode code;

    public static RespondCode FromBytes(byte[] bytes)
    {
        var rc = Protobuf.RespondCode.Parser.ParseFrom(bytes);

        return new RespondCode((ResponseCode)rc.Code);
    }

    public byte[] ToByteArray()
    => new Protobuf.RespondCode() { Code = (Protobuf.Code)code }.ToByteArray();

    public RespondCode(ResponseCode code)
    { this.code = code; }
}
