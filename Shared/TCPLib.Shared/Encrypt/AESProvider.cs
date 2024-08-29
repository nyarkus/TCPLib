using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Modes;


namespace TCPLib.Encrypt
{
    public class AESProvider
    {
        byte[] Key;
        byte[] IV;

        IBufferedCipher ForEncrypt;
        IBufferedCipher ForDecrypt;

        public byte[] Encrypt(byte[] data)
            => ForEncrypt.DoFinal(data);
        public byte[] Decrypt(byte[] data)
            => ForDecrypt.DoFinal(data);

        public byte[] GetKey() 
            => Key;
        public byte[] GetIV()
            => IV;
        public void SetKeypair(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;

            ForEncrypt = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            ForEncrypt.Init(true, new ParametersWithIV(new KeyParameter(Key), IV));

            ForDecrypt = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            ForDecrypt.Init(false, new ParametersWithIV(new KeyParameter(Key), IV));
        }


        public AESProvider(int keySize = 256)
        {
            Key = new byte[keySize / 8];
            SecureRandom random = new SecureRandom();
            random.NextBytes(Key);

            IV = new byte[16];
            random.NextBytes(IV);

            ForEncrypt = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            ForEncrypt.Init(true, new ParametersWithIV(new KeyParameter(Key), IV));

            ForDecrypt = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            ForDecrypt.Init(false, new ParametersWithIV(new KeyParameter(Key), IV));
        }
    }
}
