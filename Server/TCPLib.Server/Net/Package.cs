// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.


// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.
using Google.Protobuf;

namespace TCPLib.Server.Net;

public struct Package<T> where T : IProtobufSerializable<T>
{
    public string Type { get; set; }
    public byte[] Data { get; set; }
    public Package(string type, byte[] value)
    {
        Type = type;
        Data = value;
    }
    public Package(string type, IProtobufSerializable<T> value)
    {
        Type = type;
        Data = value.ToByteArray();
    }
    public byte[] Pack()
    => new TCPLib.Protobuf.Package() { Data = ByteString.CopyFrom(Data), Type = Type }.ToByteArray();

    public T Unpack()
    => T.FromBytes(Data);
    public T Value
    {
        get
        {
            return Unpack();
        }
    }

}