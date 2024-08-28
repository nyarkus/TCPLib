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
    public async Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>
    {
        while (true)
        {
            while (stream.DataAvailable)
            {
                if (token.IsCancellationRequested)
                    return default;
                var length = BitConverter.ToInt32(await Read(4, stream));
                var bytes = await Read(length, stream);

                var package = Protobuf.Package.Parser.ParseFrom(bytes);

                return new Package<T>(package.Type, package.Data.ToArray());
            }
        }
    }

    public async Task<Package<T>?> ReceiveAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>
    {
        while (true)
        {
            while (stream.DataAvailable)
            {
                if (token.IsCancellationRequested)
                    return null;
                var length = BitConverter.ToInt32(await Read(4, stream));

                var bytes = await Read(length, stream);
                if (EncryptType == EncryptType.AES)
                    bytes = Encryptor.AESDecrypt(bytes);
                else
                    bytes = Encryptor.RSADecrypt(bytes);

                var package = Protobuf.Package.Parser.ParseFrom(bytes);

                return new Package<T>(package.Type, package.Data.ToArray());
            }
        }
    }
    public async Task<Classes.PackageSource?> ReceiveSourceAsync(CancellationToken token = default)
    {
        while (true)
        {
            while (stream.DataAvailable)
            {
                if (token.IsCancellationRequested)
                    return null;
                var length = BitConverter.ToInt32(await Read(4, stream));

                var bytes = await Read(length, stream);
                if (EncryptType == EncryptType.AES)
                    bytes = Encryptor.AESDecrypt(bytes);
                else
                    bytes = Encryptor.RSADecrypt(bytes);

                var package = Protobuf.Package.Parser.ParseFrom(bytes);

                return new(package.Type, package.Data.ToArray());
            }
        }
    }
    public Task<Package<T>?> ReceiveAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>
    {
        var cancel = new CancellationTokenSource();
        token.Register(cancel.Cancel);
        var task = Task.Run(() => ReceiveAsync<T>(cancel.Token));

        if (task.Wait(timeout))
            return task;
        else
        {
            cancel.Cancel();
            return Task.FromResult<Package<T>?>(null);
        }
    }
    public Task<Classes.PackageSource?> ReceiveSourceAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var cancel = new CancellationTokenSource();
        token.Register(cancel.Cancel);
        var task = Task.Run(() => ReceiveSourceAsync(cancel.Token));

        if (task.Wait(timeout))
            return task;
        else
        {
            cancel.Cancel();
            return Task.FromResult<Classes.PackageSource?>(null);
        }
    }
    public Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>
    {
        var cancel = new CancellationTokenSource();
        token.Register(cancel.Cancel);
        var task = Task.Run(() => ReceiveWithoutCryptographyAsync<T>(cancel.Token));

        if (task.Wait(timeout))
            return task;
        else
        {
            cancel.Cancel();
            return Task.FromResult<Package<T>?>(null);
        }
    }

    static async Task<byte[]> Read(int count, Stream stream)
    {
        byte[] buffer = new byte[count];
        int totalRead = 0;

        while (totalRead < count)
        {
            int bytesRead = await stream.ReadAsync(buffer, totalRead, count - totalRead);
            if (bytesRead == 0)
                throw new EndOfStreamException("Reached end of stream before reading expected number of bytes.");
            totalRead += bytesRead;
        }

        return buffer;
    }
}