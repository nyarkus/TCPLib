// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.


// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.
using Google.Protobuf;
using System.Security.Cryptography;

namespace TCPLib.Server.Net;

public class Encryptor
{
#pragma warning disable CS0618
    const string FilePath = @"Certificate.key";

    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
    Aes aes = Aes.Create();
    private Encryptor() { }
    byte[] pub;
    byte[] priv;

    byte[] AesKey;
    byte[] AesIV;

    static Encryptor? ServerEncryptor = null;
    public static Encryptor GetServerEncryptor()
    {
        if (ServerEncryptor is not null)
            return ServerEncryptor;
        try
        {
            var enc = GetEncryptor();
            if (!File.Exists(FilePath))
            {
                Console.Warning("Certificate not found! Generation of a new...");
                using (var fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (BinaryWriter writer = new(fs))
                    {
                        var privateKey = enc.RSA.ExportRSAPrivateKey();
                        var publicKey = enc.RSA.ExportRSAPublicKey();
                        writer.Write(privateKey.Length);
                        writer.Write(privateKey);

                        writer.Write(publicKey.Length);
                        writer.Write(publicKey);
                    }
                }

                enc.priv = enc.RSA.ExportRSAPrivateKey();
                enc.pub = enc.RSA.ExportRSAPublicKey();

                Console.Warning($"New certificate has been generated. DO NOT SEND FILE \"{FilePath}\" to ANYONE!!! ");
                ServerEncryptor = enc;
                return enc;
            }
            using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new(fs))
                {

                    var privateKey = reader.ReadBytes(reader.ReadInt32());
                    var publicKey = reader.ReadBytes(reader.ReadInt32());

                    var e = new Keys() { Private = privateKey, Public = publicKey };
                    enc.priv = e.Private;

                    enc.RSA.ImportRSAPrivateKey(e.Private, out int bytes);
                    Console.Debug($"In private key readed {bytes} bytes");

                    enc.pub = e.Public;

                    enc.RSA.ImportRSAPublicKey(e.Public, out bytes);
                    Console.Debug($"In public key readed {bytes} bytes");
                }
            }
            ServerEncryptor = enc;
            GC.Collect();
            return enc;
        }
        catch (NullReferenceException)
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
            return GetServerEncryptor();
        }

    }
    public static Encryptor GetEncryptor()
    {
        var enc = new Encryptor();
        enc.RSA = new RSACryptoServiceProvider();
        enc.aes = Aes.Create();
        return enc;
    }
    public Encryptor SetRSAKey(byte[] privatekey, byte[] publickey)
    {

        RSA.ImportRSAPrivateKey(privatekey, out int bytes);

        priv = privatekey;
        Console.Debug($"In private key readed {bytes} bytes");

        RSA.ImportRSAPublicKey(publickey, out bytes);

        pub = publickey;
        Console.Debug($"In public key readed {bytes} bytes");

        return this;
    }
    public Encryptor SetAESKey(byte[] key, byte[] IV)
    {
        AesKey = key;
        AesIV = IV;

        return this;
    }
    public Encryptor SetAESKey(AESKey key)
    {
        AesKey = key.Key;
        AesIV = key.IV;

        return this;
    }
    public byte[] AESEncrypt(byte[] input)
    {
        return EncryptStringToBytes_Aes(input, AesKey, AesIV);
    }
    public byte[] AESDecrypt(byte[] s)
    {
        return DecryptStringFromBytes_Aes(s, AesKey, AesIV);
    }
    public byte[] RSAEncrypt(byte[] input)
    {
        RSA.ImportRSAPublicKey(pub, out int bytes);
        return RSA.Encrypt(input, false);
    }
    public byte[] RSADecrypt(byte[] s)
    {
        RSA.ImportRSAPrivateKey(priv, out int bytes);
        return RSA.Decrypt(s, false);
    }
    byte[] EncryptStringToBytes_Aes(byte[] data, byte[] key, byte[] iv)
    {
        aes.Key = key;
        aes.IV = iv;
        aes.Padding = PaddingMode.PKCS7;
        return aes.EncryptCbc(data, iv, PaddingMode.PKCS7);
    }

    byte[] DecryptStringFromBytes_Aes(byte[] encryptedData, byte[] key, byte[] iv)
    {
        aes.Key = key;
        aes.IV = iv;
        aes.Padding = PaddingMode.PKCS7;
        return aes.DecryptCbc(encryptedData, iv, PaddingMode.PKCS7);
    }
    public byte[] GetRSAPublicKey()
    {
        var key = RSA.ExportRSAPublicKey();
        RSA.ImportRSAPublicKey(key, out int b);
        return key;
    }

    public byte[] GetRSAPrivateKey()
    {
        var key = RSA.ExportRSAPrivateKey();
        RSA.ImportRSAPrivateKey(key, out int b);
        return key;
    }

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
public class Keys
{
    public byte[] Public;
    public byte[] Private;
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
