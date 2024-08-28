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
public class GameInfo : IProtobufSerializable<GameInfo>
{
    public string Name { get; set; }
    public string Version { get; set; }
    public GameInfo(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public byte[] ToByteArray()
    => new TCPLib.Protobuf.GameInfo() { Name = Name, Version = Version }.ToByteArray();

    public static GameInfo FromBytes(byte[] bytes)
    {
        var gi = TCPLib.Protobuf.GameInfo.Parser.ParseFrom(bytes);

        return new GameInfo(gi.Name, gi.Version);
    }
}