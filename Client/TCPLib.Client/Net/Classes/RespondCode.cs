// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.


// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.
using Google.Protobuf;

namespace TCPLib.Client.Net;

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
