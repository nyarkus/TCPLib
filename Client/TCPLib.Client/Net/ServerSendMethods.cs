using System.Threading.Tasks;
using System;
using TCPLib.Net;

namespace TCPLib.Client.Net
{
    public partial class Server
    {
        public async Task SendAsync<T>(DataPackage<T> data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var bytes = data.Pack();
            if(UseEncryption)
            {
                if(EncryptType == EncryptType.AES)
                    bytes = encryptor.AESEncrypt(bytes);
                else
                    bytes = encryptor.RSAEncrypt(bytes);
            }
            try
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
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
        public async Task SendAsync<T>(T data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var package = new DataPackage<T>(nameof(T), data);
            await SendAsync(package, UseEncryption);
        }
    }
}