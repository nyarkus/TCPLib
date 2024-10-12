using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using System.Linq;
using TCPLib.Classes;
using TCPLib.Net;

namespace TCPLib.Client.Net
{
    public partial class Server
    {
        #region ReceiveSource
        public async Task<Classes.PackageSource> ReceiveSourceAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, stream), 0);

                    var bytes = await Read(length, stream);
                    if (EncryptType == EncryptType.AES)
                    {
                        bytes = encryptor.AESDecrypt(bytes);
                    }
                    else
                    {
                        bytes = encryptor.RSADecrypt(bytes);
                    }

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    return new Classes.PackageSource(package.Type, package.Data.ToArray());
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        public Task<Classes.PackageSource> ReceiveSourceAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceAsync(cancel.Token));

            if (task.Wait(timeout))
            {
                cancel.Dispose();
                return task;
            }
            else
            {
                cancel.Cancel();
                return null;
            }
        }
        public async Task<PackageSource> ReceiveSourceWithoutCryptographyAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, stream), 0);
                    var bytes = await Read(length, stream);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    return new PackageSource(package.Type, package.Data.ToArray());
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        public Task<Classes.PackageSource> ReceiveSourceWithoutCryptographyAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithoutCryptographyAsync(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
            else
            {
                cancel.Cancel();
                return null;
            }
        }
        #endregion
        #region ReceiveWithProcessing
        public async Task<Package<T>?> ReceiveWithoutCryptographyWithProcessingAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                var result = await ReceiveSourceWithoutCryptographyAsync();
                if (result.Type == "KickMessage")
                {
                    var kick = new KickMessage().FromBytes(result.Data);
                    switch (kick.code)
                    {
                        case ResponseCode.Kicked:
                            if (Kicked != null)
                            {
                                await Kicked.Invoke(kick);
                            }
                            continue;
                        case ResponseCode.Blocked:
                            if (Banned != null)
                            {
                                await Banned.Invoke(kick);
                            }
                            continue;
                        case ResponseCode.ServerShutdown:
                            if (ServerShutdown != null)
                            {
                                await ServerShutdown.Invoke(kick);
                            }
                            continue;
                    }
                }
                return new Package<T>(result.Type, result.Data);
            }
        }
        public Task<Package<T>?> ReceiveWithoutCryptographyWithProcessingAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithoutCryptographyWithProcessingAsync<T>(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
            else
            {
                cancel.Cancel();
                return Task.FromResult<Package<T>?>(null);
            }
        }
        public async Task<Package<T>?> ReceiveWithProcessingAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                var result = await ReceiveSourceAsync();
                if (result.Type == "KickMessage")
                {
                    var kick = new KickMessage().FromBytes(result.Data);
                    switch (kick.code)
                    {
                        case ResponseCode.Kicked:
                            if (Kicked != null)
                            {
                                await Kicked.Invoke(kick);
                            }
                            continue;
                        case ResponseCode.Blocked:
                            if (Banned != null)
                            {
                                await Banned.Invoke(kick);
                            }
                            continue;
                        case ResponseCode.ServerShutdown:
                            if (ServerShutdown != null)
                            {
                                await ServerShutdown.Invoke(kick);
                            }
                            continue;
                    }
                }
                return new Package<T>(result.Type, result.Data);
            }
        }
        public Task<Package<T>?> ReceiveWithProcessingAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithProcessingAsync<T>(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
            else
            {
                cancel.Cancel();
                return Task.FromResult<Package<T>?>(null);
            }
        }
        #endregion
        #region ReceiveSourceWithProcessing
        public async Task<Classes.PackageSource> ReceiveSourceWithoutCryptographyWithProcessingAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        {
                            return default;
                        }

                        var result = await ReceiveSourceWithoutCryptographyAsync(token);

                        if (result.Type == "KickMessage")
                        {
                            var kick = new KickMessage().FromBytes(result.Data);
                            switch (kick.code)
                            {
                                case ResponseCode.Kicked:
                                    if (Kicked != null)
                                    {
                                        await Kicked.Invoke(kick);
                                    }
                                    continue;
                                case ResponseCode.Blocked:
                                    if (Banned != null)
                                    {
                                        await Banned.Invoke(kick);
                                    }
                                    continue;
                                case ResponseCode.ServerShutdown:
                                    if (ServerShutdown != null)
                                    {
                                        await ServerShutdown.Invoke(kick);
                                    }
                                    continue;
                            }
                        }
                        return result;
                        
                    }
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else
                        throw;
                }
            }
        }
        public Task<Classes.PackageSource> ReceiveSourceWithoutCryptographyWithProcessingAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithoutCryptographyWithProcessingAsync(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
            else
            {
                cancel.Cancel();
                return null;
            }
        }
        public async Task<Classes.PackageSource> ReceiveSourceWithProcessingAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        {
                            return default;
                        }

                        var result = await ReceiveSourceAsync(token);

                        if (result.Type == "KickMessage")
                        {
                            var kick = new KickMessage().FromBytes(result.Data);
                            switch (kick.code)
                            {
                                case ResponseCode.Kicked:
                                    if (Kicked != null)
                                    {
                                        await Kicked.Invoke(kick);
                                    }
                                    continue;
                                case ResponseCode.Blocked:
                                    if (Banned != null)
                                    {
                                        await Banned.Invoke(kick);
                                    }
                                    continue;
                                case ResponseCode.ServerShutdown:
                                    if (ServerShutdown != null)
                                    {
                                        await ServerShutdown.Invoke(kick);
                                    }
                                    continue;
                            }
                        }
                        return result;
                    }
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else
                        throw;
                }
            }
        }
        public Task<Classes.PackageSource> ReceiveSourceWithProcessingAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithProcessingAsync(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
            else
            {
                cancel.Cancel();
                return null;
            }
        }
        #endregion
        public async Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, stream), 0);
                    var bytes = await Read(length, stream);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    return new Package<T>(package.Type, package.Data.ToArray());
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        public Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithoutCryptographyAsync<T>(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
            else
            {
                cancel.Cancel();
                return Task.FromResult<Package<T>?>(null);
            }
        }

        public async Task<Package<T>?> ReceiveAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, stream), 0);

                    var bytes = await Read(length, stream);
                    if (EncryptType == EncryptType.AES)
                    {
                        bytes = encryptor.AESDecrypt(bytes);
                    }
                    else
                    {
                        bytes = encryptor.RSADecrypt(bytes);
                    }

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    return new Package<T>(package.Type, package.Data.ToArray());
                    
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        public Task<Package<T>?> ReceiveAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveAsync<T>(cancel.Token));

            if (task.Wait(timeout))
            {
                return task;
            }
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
}