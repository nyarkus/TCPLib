// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

namespace TCPLib.Server.Net;
public partial class NetClient
{

    public async Task SendWithoutCryptographyAsync<T>(Package<T> data) where T : IProtobufSerializable<T>
    {
        var bytes = data.Pack();

        await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
        await stream.WriteAsync(bytes);
    }
    public async Task SendWithoutCryptographyAsync<T>(T data) where T : IProtobufSerializable<T>
    {
        var package = new Package<T>(typeof(T).Name, data);
        var bytes = package.Pack();
        await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
        await stream.WriteAsync(bytes);
    }
    public async Task SendAsync<T>(Package<T> data) where T : IProtobufSerializable<T>
    {

        if (EncryptType == EncryptType.AES)
        {
            var bytes = Encryptor.AESEncrypt(data.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
        else
        {
            var bytes = Encryptor.RSAEncrypt(data.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
    }
    public async Task SendAsync<T>(T data) where T : IProtobufSerializable<T>
    {
        var package = new Package<T>(typeof(T).Name, data);
        if (EncryptType == EncryptType.AES)
        {
            var bytes = Encryptor.AESEncrypt(package.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
        else
        {
            var bytes = Encryptor.RSAEncrypt(package.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
    }
}