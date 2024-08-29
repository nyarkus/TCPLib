// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System.Linq;
using System.IO;
using System;
using TCPLib.Encrypt;
using TCPLib.Net;

namespace TCPLib.Server.Net.Encrypt
{
    public class Encryptor
    {
#pragma warning disable CS0618
        const string FilePath = @"Certificate.key";

        RSAProvider RSA = new RSAProvider();
        AESProvider AES = new AESProvider();


        static Encryptor ServerEncryptor;

        public static Encryptor GetServerEncryptor()
        {
            if (ServerEncryptor != null)
                return ServerEncryptor;
            try
            {
                var enc = new Encryptor();
                if (!File.Exists(FilePath))
                {
                    Console.Warning("Certificate not found! Generation of a new...");
                    using (var fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            var privateKey = enc.RSA.SerializePrivateKey();
                            var publicKey = enc.RSA.SerializePublicKey();



                            writer.Write(privateKey.Length);
                            writer.Write(privateKey);
                        }
                    }

                    Console.Warning($"New certificate has been generated. DO NOT SEND FILE \"{FilePath}\" to ANYONE!!! ");
                    ServerEncryptor = enc;
                    return enc;
                }
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {

                        var privateKey = reader.ReadBytes(reader.ReadInt32());

                        enc.RSA.ImportPrivateKey(privateKey);
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
        public Encryptor SetRSAKey(byte[] privatekey)
        {
            RSA.ImportPrivateKey(privatekey);
            return this;
        }
        public Encryptor SetAESKey(byte[] key, byte[] IV)
        {
            AES.SetKeypair(key, IV);
            return this;
        }
        public Encryptor SetAESKey(AESKey key)
        {
            AES.SetKeypair(key.Key, key.IV);
            return this;
        }
        public byte[] AESEncrypt(byte[] input)
            => AES.Encrypt(input);
        public byte[] AESDecrypt(byte[] input)
            => AES.Decrypt(input);
        public byte[] RSAEncrypt(byte[] input)
            => RSA.Encrypt(input);
        public byte[] RSADecrypt(byte[] input)
            => RSA.Decrypt(input);

        public byte[] GetRSAPublicKey()
            => RSA.SerializePublicKey();
        public byte[] GetRSAPrivateKey()
            => RSA.SerializePrivateKey();

        public AESKey GetAESKey()
            => new AESKey() { Key = AES.GetKey(), IV = AES.GetIV() };
    }

    public struct AESKey : IProtobufSerializable<AESKey>
    {
        public byte[] Key;
        public byte[] IV;

        public AESKey FromBytes(byte[] bytes)
        {
            var aes = Protobuf.AESKey.Parser.ParseFrom(bytes);

            return new AESKey() { Key = aes.Key.ToArray(), IV = aes.IV.ToArray() };
        }

        public byte[] ToByteArray() =>
            new Protobuf.AESKey() { Key = ByteString.CopyFrom(Key), IV = ByteString.CopyFrom(IV) }.ToByteArray();

    }
}
