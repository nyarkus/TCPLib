// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System.Security.Cryptography;

namespace TCPLib.Client.Net;

public class Encryptor
{
    const int dwKeySize = 4086;

    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

    Aes aes = Aes.Create();

    byte[] AesKey;
    byte[] AesIV;

    public static Encryptor? ServerEncryptor;
    private Encryptor() { }

    public static Encryptor GetEncryptor()
    {
        var enc = new Encryptor();

        enc.AesIV = enc.aes.IV;
        enc.AesKey = enc.aes.Key;

        return enc;
    }
    public Encryptor SetRSAKey(byte[] privatekey, byte[] publickey)
    {

        RSA.ImportRSAPrivateKey(privatekey, out int bytes);
        RSA.ImportRSAPublicKey(publickey, out bytes);

        return this;
    }
    public Encryptor SetAESKey(byte[] key, byte[] IV)
    {
        AesKey = key;
        AesIV = IV;

        return this;
    }
    public byte[] AESEncrypt(byte[] input)
    {
        return EncryptAes(input, AesKey, AesIV);
    }
    public byte[] AESDecrypt(byte[] s)
    {
        return DecryptAes(s, AesKey, AesIV);
    }
    public static Encryptor GenerateNew()
    {
        return new Encryptor();
    }
    public byte[] RSAEncrypt(byte[] input)
    {
        return RSA.Encrypt(input, false);
    }
    public byte[] RSADecrypt(byte[] s)
    {
        return RSA.Decrypt(s, false);
    }
    byte[] EncryptAes(byte[] data, byte[] key, byte[] iv)
    {
        aes.Key = key;
        aes.IV = iv;
        aes.Padding = PaddingMode.PKCS7;
        return aes.EncryptCbc(data, iv, PaddingMode.PKCS7);
    }

    byte[] DecryptAes(byte[] encryptedData, byte[] key, byte[] iv)
    {
        aes.Key = key;
        aes.IV = iv;
        aes.Padding = PaddingMode.PKCS7;
        return aes.DecryptCbc(encryptedData, iv, PaddingMode.PKCS7);
    }

    public byte[] GetRSAPublicKey() => RSA.ExportRSAPublicKey();

    public byte[] GetRSAPrivateKey() => RSA.ExportRSAPrivateKey();

    public AESKey GetAESKey()
    {
        if (AesKey is null || AesIV is null)
        {
            AesKey = aes.Key;
            AesIV = aes.IV;
        }
        return new AESKey() { Key = AesKey, IV = AesIV };
    }
}
public struct AESKey : IProtobufSerializable<AESKey>
{
    public byte[] Key;
    public byte[] IV;

    public static AESKey FromBytes(byte[] bytes)
    {
        var aes = Protobuf.AESKey.Parser.ParseFrom(bytes);

        return new AESKey() { Key = aes.Key.ToArray(), IV = aes.IV.ToArray() };
    }

    public byte[] ToByteArray() =>
        new Protobuf.AESKey() { Key = ByteString.CopyFrom(Key), IV = ByteString.CopyFrom(IV) }.ToByteArray();

}
