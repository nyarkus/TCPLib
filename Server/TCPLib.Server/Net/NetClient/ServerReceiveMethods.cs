using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using TCPLib.Net;


namespace TCPLib.Server.Net
{
    public partial class NetClient
    {
        public async Task<Package<T>?> ReceiveWithoutCryptographyAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    var length = BitConverter.ToInt32(await Read(4, stream), 0);
                    var bytes = await Read(length, stream);

                    var package = Protobuf.Package.Parser.ParseFrom(bytes);

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    return new Package<T>(package.Type, package.Data.ToArray());
                }
            }
        }

        public async Task<Package<T>?> ReceiveAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    try
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        var length = BitConverter.ToInt32(await Read(4, stream), 0);

                        var bytes = await Read(length, stream);
                        if (EncryptType == EncryptType.AES)
                            bytes = Encryptor.AESDecrypt(bytes);
                        else
                            bytes = Encryptor.RSADecrypt(bytes);

                        var package = Protobuf.Package.Parser.ParseFrom(bytes);

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        return new Package<T>(package.Type, package.Data.ToArray());
                    }
                    catch
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;
                        else throw;
                    }
                }
            }
        }
        public async Task<Classes.PackageSource> ReceiveSourceAsync(CancellationToken token = default)
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    try
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        var length = BitConverter.ToInt32(await Read(4, stream), 0);

                        var bytes = await Read(length, stream);
                        if (EncryptType == EncryptType.AES)
                            bytes = Encryptor.AESDecrypt(bytes);
                        else
                            bytes = Encryptor.RSADecrypt(bytes);

                        var package = Protobuf.Package.Parser.ParseFrom(bytes);

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        return new Classes.PackageSource(package.Type, package.Data.ToArray());
                    }
                    catch
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;
                        else throw;
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
                return Task.FromResult<Classes.PackageSource>(null);
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