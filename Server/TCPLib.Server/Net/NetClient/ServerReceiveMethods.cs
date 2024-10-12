using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using TCPLib.Net;
using TCPLib.Extentions;
using TCPLib.Classes;


namespace TCPLib.Server.Net
{
    public partial class NetClient
    {
        private static async Task<byte[]> Read(int count, Stream stream)
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
        private bool Handle(DataPackageSource package)
        {
            if (package.Type == "KickMessage")
            {
                var kick = new KickMessage().FromBytes(package.Data);
                if (kick.code == ResponseCode.DisconnectedByUser)
                    OnDisconnected();
                return true;
            }
            return false;
        }

        public async Task<DataPackageSource> ReceiveSourceAsync(bool UseDecryption = true, CancellationToken cancellation = default)
        {
            while (true)
            {
                try
                {
                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, stream), 0);

                    var bytes = await Read(length, stream);
                    if (UseDecryption)
                    {
                        if (EncryptType == EncryptType.AES)
                        {
                            bytes = Encryptor.AESDecrypt(bytes);
                        }
                        else
                        {
                            bytes = Encryptor.RSADecrypt(bytes);
                        }
                    }

                    var package = Protobuf.DataPackage.Parser.ParseFrom(bytes);
                    var source = new DataPackageSource(package.Type, package.Data.ToArray());
                    var result = Handle(source);
                    if (result)
                        continue;

                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    return source;
                }
                catch
                {
                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
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
        public async Task<DataPackageSource?> ReceiveSourceAsync(TimeSpan timeout, bool UseDecryption = true, CancellationToken cancellation = default)
        {
            var task = Task.Run(() => ReceiveSourceAsync(UseDecryption, cancellation));

            var result = await task.TimeoutAsync(timeout, cancellation);
            if (result)
            {
                return task.Result;
            }
            else
            {
                return null;
            }
        }
        public async Task<DataPackage<T>> ReceiveAsync<T>(bool UseDecryption = true, CancellationToken cancellation = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                try
                {
                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, stream), 0);

                    var bytes = await Read(length, stream);
                    if (UseDecryption)
                    {
                        if (EncryptType == EncryptType.AES)
                        {
                            bytes = Encryptor.AESDecrypt(bytes);
                        }
                        else
                        {
                            bytes = Encryptor.RSADecrypt(bytes);
                        }
                    }

                    var package = Protobuf.DataPackage.Parser.ParseFrom(bytes);
                    var source = new DataPackageSource(package.Type, package.Data.ToArray());
                    var result = Handle(source);
                    if (result)
                        continue;

                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    return new DataPackage<T>(package.Type, package.Data.ToArray());
                }
                catch
                {
                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
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
        public async Task<DataPackage<T>?> ReceiveAsync<T>(TimeSpan timeout, bool UseDecryption = true, CancellationToken cancellation = default) where T : IProtobufSerializable<T>, new()
        {
            var task = Task.Run(() => ReceiveAsync<T>(UseDecryption, cancellation));

            var result = await task.TimeoutAsync(timeout, cancellation);
            if (result)
            {
                return task.Result;
            }
            else
            {
                return null;
            }
        }
    }
}