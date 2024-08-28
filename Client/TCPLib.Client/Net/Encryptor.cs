// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;
using System.Security.Cryptography;
using System.IO;
using System.Linq;


namespace TCPLib.Client.Net
{
    public class Encryptor
    {
        const int dwKeySize = 4086;

        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

        Aes aes = Aes.Create();

        byte[] AesKey;
        byte[] AesIV;

        public static Encryptor ServerEncryptor;
        private Encryptor() { }

        public static byte[] ExportPrivateRSAKey(RSACryptoServiceProvider rsa)
        {
            var stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                var privateKey = rsa.ExportParameters(true);

                //D
                if (privateKey.D != null)
                {
                    writer.Write(privateKey.D.Length);
                    writer.Write(privateKey.D);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                //DP
                if (privateKey.DP != null)
                {
                    writer.Write(privateKey.DP.Length);
                    writer.Write(privateKey.DP);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                // DQ
                if (privateKey.DQ != null)
                {
                    writer.Write(privateKey.DQ.Length);
                    writer.Write(privateKey.DQ);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                //Exponent
                if (privateKey.Exponent != null)
                {
                    writer.Write(privateKey.Exponent.Length);
                    writer.Write(privateKey.Exponent);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                // InverseQ
                if (privateKey.InverseQ != null)
                {
                    writer.Write(privateKey.InverseQ.Length);
                    writer.Write(privateKey.InverseQ);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                // Modulus
                if (privateKey.Modulus != null)
                {
                    writer.Write(privateKey.Modulus.Length);
                    writer.Write(privateKey.Modulus);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                // P
                if (privateKey.P != null)
                {
                    writer.Write(privateKey.P.Length);
                    writer.Write(privateKey.P);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                // Q
                if (privateKey.Q != null)
                {
                    writer.Write(privateKey.Q.Length);
                    writer.Write(privateKey.Q);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }
            };

            return stream.ToArray();
        }
        public static void ImportRSAPrivateKey(RSACryptoServiceProvider rsa, byte[] data)
        {
            RSAParameters parameters = new RSAParameters();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
            {
                parameters.D = reader.ReadBytes(reader.ReadInt32());
                parameters.DP = reader.ReadBytes(reader.ReadInt32());
                parameters.DQ = reader.ReadBytes(reader.ReadInt32());
                parameters.Exponent = reader.ReadBytes(reader.ReadInt32());
                parameters.InverseQ = reader.ReadBytes(reader.ReadInt32());
                parameters.Modulus = reader.ReadBytes(reader.ReadInt32());
                parameters.P = reader.ReadBytes(reader.ReadInt32());
                parameters.Q = reader.ReadBytes(reader.ReadInt32());
            }
            rsa.ImportParameters(parameters);
        }
        public static void ImportRSAPublicKey(RSACryptoServiceProvider rsa, byte[] data)
        {
            RSAParameters parameters = new RSAParameters();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
            {
                parameters.Exponent = reader.ReadBytes(reader.ReadInt32());
                parameters.Modulus = reader.ReadBytes(reader.ReadInt32());
            }
            rsa.ImportParameters(parameters);
        }
        public static byte[] ExportPublicRSAKey(RSACryptoServiceProvider rsa)
        {
            var stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                var publicKey = rsa.ExportParameters(false);

                //Exponent
                if (publicKey.Exponent != null)
                {
                    writer.Write(publicKey.Exponent.Length);
                    writer.Write(publicKey.Exponent);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }

                // Modulus
                if (publicKey.Modulus != null)
                {
                    writer.Write(publicKey.Modulus.Length);
                    writer.Write(publicKey.Modulus);
                }
                else
                {
                    writer.Write(0);
                    writer.Write(new byte[] { });
                }
            };

            return stream.ToArray();
        }

        public static Encryptor GetEncryptor()
        {
            var enc = new Encryptor();

            enc.AesIV = enc.aes.IV;
            enc.AesKey = enc.aes.Key;

            return enc;
        }
        public Encryptor SetRSAKey(byte[] privatekey)
        {
            ImportRSAPrivateKey(RSA, privatekey);

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
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        byte[] DecryptAes(byte[] encryptedData, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var memoryStream = new MemoryStream(encryptedData))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var resultStream = new MemoryStream())
                            {
                                cryptoStream.CopyTo(resultStream);
                                return resultStream.ToArray();
                            }
                        }
                    }
                }
            }
        }

        public byte[] GetRSAPublicKey() => ExportPublicRSAKey(RSA);

        public byte[] GetRSAPrivateKey() => ExportPrivateRSAKey(RSA);

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

        public AESKey FromBytes(byte[] bytes)
        {
            var aes = Protobuf.AESKey.Parser.ParseFrom(bytes);

            return new AESKey() { Key = aes.Key.ToArray(), IV = aes.IV.ToArray() };
        }

        public byte[] ToByteArray() =>
            new Protobuf.AESKey() { Key = ByteString.CopyFrom(Key), IV = ByteString.CopyFrom(IV) }.ToByteArray();

    }
}
