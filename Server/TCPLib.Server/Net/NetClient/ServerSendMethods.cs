using System.Threading.Tasks;
using System;

namespace TCPLib.Server.Net
{
    public partial class NetClient
    {

        public async Task SendWithoutCryptographyAsync<T>(Package<T> data) where T : IProtobufSerializable<T>, new()
        {
            var bytes = data.Pack();
            var b = BitConverter.GetBytes(bytes.Length);

            await stream.WriteAsync(b, 0, b.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
        public async Task SendWithoutCryptographyAsync<T>(T data) where T : IProtobufSerializable<T>, new()
        {
            var package = new Package<T>(typeof(T).Name, data);
            var bytes = package.Pack();
            var b = BitConverter.GetBytes(bytes.Length);

            await stream.WriteAsync(b, 0, b.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
        public async Task SendAsync<T>(Package<T> data) where T : IProtobufSerializable<T>, new()
        {

            if (EncryptType == EncryptType.AES)
            {
                var bytes = Encryptor.AESEncrypt(data.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                await stream.WriteAsync(b, 0, b.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            else
            {
                var bytes = Encryptor.RSAEncrypt(data.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                await stream.WriteAsync(b, 0, b.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
        public async Task SendAsync<T>(T data) where T : IProtobufSerializable<T>, new()
        {
            var package = new Package<T>(typeof(T).Name, data);
            if (EncryptType == EncryptType.AES)
            {
                var bytes = Encryptor.AESEncrypt(package.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                await stream.WriteAsync(b, 0, b.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            else
            {
                var bytes = Encryptor.RSAEncrypt(package.Pack());
                var b = BitConverter.GetBytes(bytes.Length);

                await stream.WriteAsync(b, 0, b.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}