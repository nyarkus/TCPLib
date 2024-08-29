using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPLib.Encrypt
{
    public partial class RSAProvider
    {
        // For public key
        public byte[] SerializePublicKey()
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(Public);

            return publicKeyInfo.GetEncoded();
        }
        public void ImportPublicKey(byte[] der)
        {
            var publicKeyInfo = SubjectPublicKeyInfo.GetInstance(der);

            var key = PublicKeyFactory.CreateKey(publicKeyInfo);

            Public = key;
            forEncrypt = new RsaEngine();
            forEncrypt.Init(true, key);

            forDecrypt = new RsaEngine();
            forDecrypt.Init(false, Private);
        }
        // For private key
        public byte[] SerializePrivateKey()
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(Private);

            return privateKeyInfo.GetEncoded();
        }
        public void ImportPrivateKey(byte[] der)
        {
            var privateKeyInfo = PrivateKeyInfo.GetInstance(der);
            var key = PrivateKeyFactory.CreateKey(privateKeyInfo);

            forEncrypt = new RsaEngine();
            forEncrypt.Init(true, Public);

            Private = key;
            forDecrypt = new RsaEngine();
            forDecrypt.Init(false, key);
        }
        
        // For key pair
        public void ImportKeyPair(byte[] @private, byte[] @public)
        {
            var privateKeyInfo = PrivateKeyInfo.GetInstance(@private);
            var privateKey = PrivateKeyFactory.CreateKey(privateKeyInfo);

            var publicKeyInfo = SubjectPublicKeyInfo.GetInstance(@public);
            var publicKey = PublicKeyFactory.CreateKey(publicKeyInfo);

            Public = publicKey;
            Private = privateKey;

            forEncrypt = new RsaEngine();
            forEncrypt.Init(true, Public);

            forDecrypt = new RsaEngine();
            forDecrypt.Init(false, Private);
        }
    }
}
