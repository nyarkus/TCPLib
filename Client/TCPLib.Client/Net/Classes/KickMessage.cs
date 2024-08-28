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

public class KickMessage : IProtobufSerializable<KickMessage>
{
    public string reason;
    public ResponseCode code;
    public KickMessage(ResponseCode code, string reason = "")
    {
        this.reason = reason;
        this.code = code;
    }

    public static KickMessage FromBytes(byte[] bytes)
    {
        var km = TCPLib.Protobuf.KickMessage.Parser.ParseFrom(bytes);

        return new KickMessage((ResponseCode)km.Code, km.Reason);
    }

    public byte[] ToByteArray()
    {
        return new TCPLib.Protobuf.KickMessage() { Code = (TCPLib.Protobuf.Code)code, Reason = reason }.ToByteArray();
    }
}
