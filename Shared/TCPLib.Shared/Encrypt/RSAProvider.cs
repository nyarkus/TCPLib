using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace TCPLib.Encrypt
{
    public partial class RSAProvider
    {
        AsymmetricKeyParameter Public;
        AsymmetricKeyParameter Private;

        IAsymmetricBlockCipher forEncrypt;
        IAsymmetricBlockCipher forDecrypt;

        public byte[] Encrypt(byte[] input)
        {
            if (input.Length > forEncrypt.GetInputBlockSize())
                throw new ArgumentException($"This byte array cannot be encrypted because its length is greater than the maximum ({forEncrypt.GetInputBlockSize()}).");
            return forEncrypt.ProcessBlock(input, 0, input.Length);
        }
        public byte[] Decrypt(byte[] input)
        {
            if (input.Length > forDecrypt.GetInputBlockSize())
                throw new ArgumentException($"This byte array cannot be decrypted because its length is greater than the maximum ({forDecrypt.GetInputBlockSize()}).");
            return forDecrypt.ProcessBlock(input, 0, input.Length);
        }

        public RSAProvider(int strength = 2048)
        {
            var keyPairGenerator = new RsaKeyPairGenerator();
            var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), strength);
            keyPairGenerator.Init(keyGenerationParameters);
            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();

            Public = keyPair.Public;
            Private = keyPair.Private;

            forEncrypt = new RsaEngine();
            forEncrypt.Init(true, Public);

            forDecrypt = new RsaEngine();
            forDecrypt.Init(false, Private);
        }
    }
}
