namespace TCPLib.Server.Net;
public partial class NetClient
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
            var bytes = Encryptor.AESEncrypt(data.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
        else
        {
            var bytes = Encryptor.RSAEncrypt(data.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
    }
    public async Task SendAsync<T>(T data) where T : IProtobufSerializable<T>
    {
        var package = new Package<T>(typeof(T).Name, data);
        if (EncryptType == EncryptType.AES)
        {
            var bytes = Encryptor.AESEncrypt(package.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
        else
        {
            var bytes = Encryptor.RSAEncrypt(package.Pack());
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await stream.WriteAsync(bytes);
        }
    }
}