namespace TCPLib.Client.Net;
public partial class Server
{

    public async Task SendWithoutCryptographyAsync<T>(Package<T> data) where T : IProtobufSerializable<T>
    {
        var bytes = data.Pack();

        await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
        await stream.WriteAsync(bytes);
    }
    public async Task SendWithoutCryptographyAsync<T>(T data) where T : IProtobufSerializable<T>
    {
        var package = new Package<T>(typeof(T).Name, data);
        var bytes = package.Pack();
        await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
        await stream.WriteAsync(bytes);
    }
    public async Task SendAsync<T>(Package<T> data) where T : IProtobufSerializable<T>
    {

        if (EncryptType == EncryptType.AES)
        {
            var bytes = encryptor.AESEncrypt(data.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
        else
        {
            var bytes = encryptor.RSAEncrypt(data.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
    }
    public async Task SendAsync<T>(T data) where T : IProtobufSerializable<T>
    {
        var package = new Package<T>(typeof(T).Name, data);
        if (EncryptType == EncryptType.AES)
        {
            var bytes = encryptor.AESEncrypt(package.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
        else
        {
            var bytes = encryptor.RSAEncrypt(package.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
    }
}