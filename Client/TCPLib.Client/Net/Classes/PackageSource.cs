// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

namespace TCPLib.Client.Net.Classes;

public class PackageSource
{
    public string Type;
    public byte[] Data;

    public PackageSource(string type, byte[] data)
    {
        Type = type;
        Data = data;
    }

}
