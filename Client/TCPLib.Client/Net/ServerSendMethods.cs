using System.Threading.Tasks;
using System;
using TCPLib.Net;

namespace TCPLib.Client.Net
{
    public partial class Server
    {

        public async Task SendWithoutCryptographyAsync<T>(DataPackage<T> data) where T : IProtobufSerializable<T>, new()
        {
            var bytes = data.Pack();
            var b = BitConverter.GetBytes(bytes.Length);

            try
            {
                await stream.WriteAsync(b, 0, b.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch
            {
                if (OnKick.IsCancellationRequested)
                    return;
                else
                    throw;
            }
        }
        public async Task SendWithoutCryptographyAsync<T>(T data) where T : IProtobufSerializable<T>, new()
        {
            var package = new DataPackage<T>(typeof(T).Name, data);
            var bytes = package.Pack();
            var b = BitConverter.GetBytes(bytes.Length);

            try
            {
                await stream.WriteAsync(b, 0, b.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch
            {
                if (OnKick.IsCancellationRequested)
                    return;
                else
                    throw;
            }
        }
        public async Task SendAsync<T>(DataPackage<T> data) where T : IProtobufSerializable<T>, new()
        {

            if (EncryptType == EncryptType.AES)
            {
                var bytes = encryptor.AESEncrypt(data.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                try
                {
                    await stream.WriteAsync(b, 0, b.Length);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch
                {
                    if (OnKick.IsCancellationRequested)
                        return;
                    else
                        throw;
                }
            }
            else
            {
                var bytes = encryptor.RSAEncrypt(data.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                try
                {
                    await stream.WriteAsync(b, 0, b.Length);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch
                {
                    if (OnKick.IsCancellationRequested)
                        return;
                    else
                        throw;
                }
            }
        }
        public async Task SendAsync<T>(T data) where T : IProtobufSerializable<T>, new()    
        {
            var package = new DataPackage<T>(typeof(T).Name, data);
            if (EncryptType == EncryptType.AES)
            {
                var bytes = encryptor.AESEncrypt(package.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                try
                {
                    await stream.WriteAsync(b, 0, b.Length);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch
                {
                    if (OnKick.IsCancellationRequested)
                        return;
                    else
                        throw;
                }
            }
            else
            {
                var bytes = encryptor.RSAEncrypt(package.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                try
                {
                    await stream.WriteAsync(b, 0, b.Length);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch
                {
                    if (OnKick.IsCancellationRequested)
                        return;
                    else
                        throw;
                }
            }
        }
    }
}