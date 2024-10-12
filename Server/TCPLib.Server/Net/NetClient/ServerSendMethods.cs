using System.Threading.Tasks;
using System;
using TCPLib.Net;
using Newtonsoft.Json.Linq;

namespace TCPLib.Server.Net
{
    public partial class NetClient
    {

        public async Task SendAsync<T>(DataPackage<T> data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var bytes = data.Pack();
            if (UseEncryption)
            {
                if (EncryptType == EncryptType.AES)
                    bytes = Encryptor.AESEncrypt(bytes);
                else
                    bytes = Encryptor.RSAEncrypt(bytes);
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