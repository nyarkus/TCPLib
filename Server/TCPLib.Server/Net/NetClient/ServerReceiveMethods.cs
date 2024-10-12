using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using TCPLib.Net;
using TCPLib.Server.Commands;


namespace TCPLib.Server.Net
{
    public partial class NetClient
    {
        #region ReceiveSource
        public async Task<Classes.DataPackageSource> ReceiveSourceWithoutCryptographyAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    while (stream.DataAvailable)
                    {

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        var length = BitConverter.ToInt32(await Read(4, stream), 0);

                        var bytes = await Read(length, stream);

                        var package = Protobuf.DataPackage.Parser.ParseFrom(bytes);

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        return new Classes.DataPackageSource(package.Type, package.Data.ToArray());
                    }

                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else throw;
                }
            }
        }
        public Task<Classes.DataPackageSource> ReceiveSourceWithoutCryptographyAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithoutCryptographyAsync(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<Classes.DataPackageSource>(default);
            }
        }
        public async Task<Classes.DataPackageSource> ReceiveSourceWithoutCryptographyWithProcessingAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        var result = await ReceiveSourceWithoutCryptographyAsync();
                        if (result.Type == "KickMessage")
                        {
                            var kick = new Classes.KickMessage().FromBytes(result.Data);
                            if (kick.code == Classes.ResponseCode.DisconnectedByUser)
                                OnDisconnected();
                        }
                        return new Classes.DataPackageSource(result.Type, result.Data);
                    }
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else throw;
                }
            }
        }
        public Task<Classes.DataPackageSource> ReceiveSourceWithoutCryptographyWithProcessingAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithoutCryptographyWithProcessingAsync(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<Classes.DataPackageSource>(default);
            }
        }
        public async Task<Classes.DataPackageSource> ReceiveSourceAsync(CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    while (stream.DataAvailable)
                    {

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        var length = BitConverter.ToInt32(await Read(4, stream), 0);

                        var bytes = await Read(length, stream);
                        if (EncryptType == EncryptType.AES)
                            bytes = Encryptor.AESDecrypt(bytes);
                        else
                            bytes = Encryptor.RSADecrypt(bytes);

                        var package = Protobuf.DataPackage.Parser.ParseFrom(bytes);

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        return new Classes.DataPackageSource(package.Type, package.Data.ToArray());
                    }

                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else throw;
                }
            }
        }
        public Task<Classes.DataPackageSource> ReceiveSourceAsync(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceAsync(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<Classes.DataPackageSource>(default);
            }
        }
        #endregion
        #region ReceiveWithProcessing
        public async Task<DataPackage<T>?> ReceiveWithoutCryptographyWithProcessingAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    var result = await ReceiveSourceWithoutCryptographyAsync();
                    if (result.Type == "KickMessage")
                    {
                        var kick = new Classes.KickMessage().FromBytes(result.Data);
                        if (kick.code == Classes.ResponseCode.DisconnectedByUser)
                            OnDisconnected();
                    }
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    return new DataPackage<T>(result.Type, result.Data);
                }
            }
            catch
            {
                if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    return default;
                else throw;
            }
        }
        public Task<DataPackage<T>?> ReceiveWithoutCryptographyWithProcessingAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithoutCryptographyWithProcessingAsync<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<DataPackage<T>?>(null);
            }
        }
        public async Task<DataPackage<T>?> ReceiveWithProcessingAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    var result = await ReceiveSourceAsync();
                    if (result.Type == "KickMessage")
                    {
                        var kick = new Classes.KickMessage().FromBytes(result.Data);
                        if (kick.code == Classes.ResponseCode.DisconnectedByUser)
                            OnDisconnected();
                    }

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    return new DataPackage<T>(result.Type, result.Data);
                }
            }
            catch
            {
                if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    return default;
                else throw;
            }
        }
        public Task<DataPackage<T>?> ReceiveWithProcessingAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithProcessingAsync<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<DataPackage<T>?>(null);
            }
        }
        #endregion
        #region ReceiveWithoutCryptography
        public async Task<DataPackage<T>?> ReceiveWithoutCryptographyAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
            {
                try
                {
                    while (stream.DataAvailable)
                    {
                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;
                        var length = BitConverter.ToInt32(await Read(4, stream), 0);
                        var bytes = await Read(length, stream);

                        var package = Protobuf.DataPackage.Parser.ParseFrom(bytes);

                        if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                            return default;

                        return new DataPackage<T>(package.Type, package.Data.ToArray());
                    }
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else throw;
                }
            }
        }
        public Task<DataPackage<T>?> ReceiveWithoutCryptographyAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveWithoutCryptographyAsync<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<DataPackage<T>?>(null);
            }
        }
        #endregion
        #region ReceiveSourceWithProcessing
        public async Task<Classes.DataPackageSource> ReceiveSourceWithProcessingAsync<T>(CancellationToken token = default)
        {
            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    var result = await ReceiveSourceAsync();
                    if (result.Type == "KickMessage")
                    {
                        var kick = new Classes.KickMessage().FromBytes(result.Data);
                        if (kick.code == Classes.ResponseCode.DisconnectedByUser)
                            OnDisconnected();
                    }

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    return result;
                }
            }
            catch
            {
                if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    return default;
                else throw;
            }
        }
        public Task<Classes.DataPackageSource> ReceiveSourceWithProcessingAsync<T>(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithProcessingAsync<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<Classes.DataPackageSource>(default);
            }
        }
        public async Task<Classes.DataPackageSource> ReceiveSourceWithoutCryptographyWithProcessingAsync<T>(CancellationToken token = default)
        {
            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    var result = await ReceiveSourceWithoutCryptographyAsync();
                    if (result.Type == "KickMessage")
                    {
                        var kick = new Classes.KickMessage().FromBytes(result.Data);
                        if (kick.code == Classes.ResponseCode.DisconnectedByUser)
                            OnDisconnected();
                    }

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    return result;
                }
            }
            catch
            {
                if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                    return default;
                else throw;
            }
        }
        public Task<Classes.DataPackageSource> ReceiveSourceWithoutCryptographyWithProcessingAsync<T>(TimeSpan timeout, CancellationToken token = default)
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveSourceWithoutCryptographyWithProcessingAsync<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<Classes.DataPackageSource>(default);
            }
        }
        #endregion
        public async Task<DataPackage<T>?> ReceiveAsync<T>(CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            while (true)
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

                    var package = Protobuf.DataPackage.Parser.ParseFrom(bytes);

                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;

                    return new DataPackage<T>(package.Type, package.Data.ToArray());
                }
                catch
                {
                    if (token.IsCancellationRequested || OnKick.IsCancellationRequested)
                        return default;
                    else throw;
                }
            }
        }
        public Task<DataPackage<T>?> ReceiveAsync<T>(TimeSpan timeout, CancellationToken token = default) where T : IProtobufSerializable<T>, new()
        {
            var cancel = new CancellationTokenSource();
            token.Register(cancel.Cancel);
            var task = Task.Run(() => ReceiveAsync<T>(cancel.Token));

            if (task.Wait(timeout))
                return task;
            else
            {
                cancel.Cancel();
                return Task.FromResult<DataPackage<T>?>(null);
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