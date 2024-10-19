using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCPLib.Encrypt;
using System.Threading;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Extentions;
using TCPLib.Net;

namespace TCPLib.Shared.Base
{
    public abstract class NetworkingBase : IDisposable
    {
        protected CancellationTokenSource OnKick;
        protected abstract Stream Stream { get; }
        public Encryptor Encryptor;
        public EncryptType EncryptType = EncryptType.RSA;
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        #region Receive
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
        protected abstract Task<bool> Handle(DataPackageSource package);

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

                    var length = BitConverter.ToInt32(await Read(4, Stream), 0);

                    var bytes = await Read(length, Stream);
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
                    var result = await Handle(source);
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
        public async Task<DataPackage<T>> ReceiveAsync<T>(bool UseDecryption = true, CancellationToken cancellation = default) where T : IDataSerializable<T>, new()
        {
            while (true)
            {
                try
                {
                    if (cancellation.IsCancellationRequested || OnKick.IsCancellationRequested)
                    {
                        return default;
                    }

                    var length = BitConverter.ToInt32(await Read(4, Stream), 0);

                    var bytes = await Read(length, Stream);
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
                    var result = await Handle(source);
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
        public async Task<DataPackage<T>?> ReceiveAsync<T>(TimeSpan timeout, bool UseDecryption = true, CancellationToken cancellation = default) where T : IDataSerializable<T>, new()
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
        #endregion
        #region Send

        public async Task SendAsync(DataPackageSource data, bool UseEncryption = true)
        {
            var bytes = data.Pack();
            if (UseEncryption)
            {
                if (EncryptType == EncryptType.AES)
                    bytes = Encryptor.AESEncrypt(bytes);
                else
                    bytes = Encryptor.RSAEncrypt(bytes);
            }
            var bl = BitConverter.GetBytes(bytes.Length);

            if (OnKick.IsCancellationRequested)
                return;

            await _semaphore.WaitAsync();
            try
            {
                await Stream.WriteAsync(bl, 0, bl.Length);
                await Stream.WriteAsync(bytes, 0, bytes.Length);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <param name="data">The method itself will create a package whose type will be equal to <c>typeof(T).Name</c></param>
        public async Task SendAsync<T>(T data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var package = new DataPackage<T>(typeof(T).Name, data);
            await SendAsync(new DataPackageSource(package.Type, package.Data), UseEncryption);
        }
        public async Task SendAsync<T>(DataPackage<T> data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            await SendAsync(new DataPackageSource(data.Type, data.Data), UseEncryption);
        }

        #endregion

        public abstract void Dispose();
    }
}
