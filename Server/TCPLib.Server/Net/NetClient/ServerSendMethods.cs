using System.Threading.Tasks;
using System;
using TCPLib.Net;
using Newtonsoft.Json.Linq;

namespace TCPLib.Server.Net
{
    public partial class NetClient
    {

        public async Task SendWithoutCryptographyAsync<T>(DataPackage<T> data) where T : IDataSerializable<T>, new()
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
                else throw;
            }
        }
        public async Task SendWithoutCryptographyAsync<T>(T data) where T : IDataSerializable<T>, new()
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
                else throw;
            }
        }
        public async Task SendAsync<T>(DataPackage<T> data) where T : IDataSerializable<T>, new()
        {

            if (EncryptType == EncryptType.AES)
            {
                var bytes = Encryptor.AESEncrypt(data.Pack());
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
                    else throw;
                }
            }
            else
            {
                var bytes = Encryptor.RSAEncrypt(data.Pack());
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
                    else throw;
                }
            }
        }
        public async Task SendAsync<T>(T data) where T : IDataSerializable<T>, new()
        {
            var package = new DataPackage<T>(typeof(T).Name, data);
            if (EncryptType == EncryptType.AES)
            {
                var bytes = Encryptor.AESEncrypt(package.Pack());
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
                    else throw;
                }
            }
            else
            {
                var bytes = Encryptor.RSAEncrypt(package.Pack());
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
                    else throw;
                }
            }
        }
    }
}