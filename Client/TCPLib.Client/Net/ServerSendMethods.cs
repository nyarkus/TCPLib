using System.Threading.Tasks;
using System;
using TCPLib.Net;
using System.Threading;

namespace TCPLib.Client.Net
{
    public partial class Server
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task SendAsync<T>(DataPackage<T> data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var bytes = data.Pack();
            if (UseEncryption)
            {
                if (EncryptType == EncryptType.AES)
                    bytes = encryptor.AESEncrypt(bytes);
                else
                    bytes = encryptor.RSAEncrypt(bytes);
            }
            var bl = BitConverter.GetBytes(bytes.Length);

            if (OnKick.IsCancellationRequested)
                return;

            await _semaphore.WaitAsync();
            try
            {
                await stream.WriteAsync(bl, 0, bl.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SendAsync<T>(T data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var package = new DataPackage<T>(typeof(T).Name, data);
            await SendAsync(package, UseEncryption);
        }
    }
}