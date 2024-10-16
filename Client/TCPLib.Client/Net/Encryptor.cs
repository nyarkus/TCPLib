using TCPLib.Encrypt;
using TCPLib.Classes;


namespace TCPLib.Client.Net
{
    public class Encryptor
    {
        readonly RSAProvider RSA;
        AESProvider AES { get; set; }
        internal static int AesKeySize;
        public Encryptor()
        {
            RSA = new RSAProvider();
            AES = new AESProvider(AesKeySize);
        }

        public Encryptor SetPrivateRSAKey(byte[] privatekey)
        {
            RSA.ImportPrivateKey(privatekey);
            return this;
        }
        public Encryptor SetPublicRSAKey(byte[] publickey)
        {
            RSA.ImportPublicKey(publickey);
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

        public void RegenerateAESKey(int size = 128)
        => AES = new AESProvider(size);

        public AESKey GetAESKey()
            => new AESKey { Key = AES.GetKey(), IV = AES.GetIV() };
    }
}
