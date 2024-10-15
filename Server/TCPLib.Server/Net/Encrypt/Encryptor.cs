
using System.IO;
using System;
using TCPLib.Encrypt;
using TCPLib.Classes;

namespace TCPLib.Server.Net.Encrypt
{
    public class Encryptor
    {
        internal static int rsaKey = 2048;
        internal static int aesKey = 128;

        readonly RSAProvider RSA;
        readonly AESProvider AES;
        public Encryptor()
        {
            RSA = new RSAProvider(rsaKey);
            AES = new AESProvider(aesKey);
        }


        static Encryptor ServerEncryptor;

        public static Encryptor GetServerEncryptor()
        {
            if (ServerEncryptor != null)
                return ServerEncryptor;

            ServerEncryptor = new Encryptor();
            return ServerEncryptor;
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
}
