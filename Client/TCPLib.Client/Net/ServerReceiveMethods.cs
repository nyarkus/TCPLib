using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using System.Linq;

namespace TCPLib.Client.Net
{
    public partial class Server
    {
        public async Task<Package<T>?> ReceiveWithoutCryptographyWithProcessing<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
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
                            Kicked?.Invoke(kick);
                            continue;
                        case ResponseCode.Blocked:
                            Banned?.Invoke(kick);
                            continue;
                        case ResponseCode.ServerShutdown:
                            ServerShutdown?.Invoke(kick);
                            continue;
                    }
                }
                return new Package<T>(result.Type, result.Data);
            }
        }
        public async Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    if (token.IsCancellationRequested)
                        return default;
                    var length = BitConverter.ToInt32(await Read(4, stream), 0);
                    var bytes = await Read(length, stream);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    return new Package<T>(package.Type, package.Data.ToArray());
                }
            }
        }
        public async Task<TCPLib.Client.Net.Classes.PackageSource> ReceiveSourceWithoutCryptographyAsync(CancellationToken token = default)
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    if (token.IsCancellationRequested)
                        return default;
                    var length = BitConverter.ToInt32(await Read(4, stream), 0);
                    var bytes = await Read(length, stream);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    return new TCPLib.Client.Net.Classes.PackageSource(package.Type, package.Data.ToArray());
                }
            }
        }

        public async Task<Package<T>?> ReceiveAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    if (token.IsCancellationRequested)
                        return null;
                    var length = BitConverter.ToInt32(await Read(4, stream), 0);

                    var bytes = await Read(length, stream);
                    if (EncryptType == EncryptType.AES)
                        bytes = encryptor.AESDecrypt(bytes);
                    else
                        bytes = encryptor.RSADecrypt(bytes);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    return new Package<T>(package.Type, package.Data.ToArray());
                }
            }
        }
        public async Task<Classes.PackageSource> ReceiveSourceAsync(CancellationToken token = default)
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    if (token.IsCancellationRequested)
                        return null;
                    var length = BitConverter.ToInt32(await Read(4, stream), 0);

                    var bytes = await Read(length, stream);
                    if (EncryptType == EncryptType.AES)
                        bytes = encryptor.AESDecrypt(bytes);
                    else
                        bytes = encryptor.RSADecrypt(bytes);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    return new Classes.PackageSource(package.Type, package.Data.ToArray());
                }
            }
        }
        public Task<Package<T>?> ReceiveAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
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
        public Task<Classes.PackageSource> ReceiveSourceAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceAsync(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return null;
            }
        }
        public async Task<Package<T>?> ReceiveWithProcessing<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
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
                            Kicked?.Invoke(kick);
                            continue;
                        case ResponseCode.Blocked:
                            Banned?.Invoke(kick);
                            continue;
                        case ResponseCode.ServerShutdown:
                            ServerShutdown?.Invoke(kick);
                            continue;
                    }
                }
                return new Package<T>(result.Type, result.Data);
            }
        }
        public Task<Package<T>?> ReceiveWithProcessing<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithProcessing<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<Package<T>?>(null);
            }
        }
        public Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
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
}