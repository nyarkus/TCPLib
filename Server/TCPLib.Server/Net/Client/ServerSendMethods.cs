using System.Threading.Tasks;
using System;
using TCPLib.Net;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace TCPLib.Server.Net
{
    public partial class Client
    {

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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

        /// <param name="data">The method itself will create a package whose type will be equal to <c>typeof(T).Name</c></param>
        public async Task SendAsync<T>(T data, bool UseEncryption = true) where T : IDataSerializable<T>, new()
        {
            var package = new DataPackage<T>(typeof(T).Name, data);
            await SendAsync(package, UseEncryption);
        }
    }
}