using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ExternalAPIs.Encryption
{
    public static class CryptoManagerJanaBank
    {
        public static (byte[] key, byte[] iv) GetAesKey()
        {

            var aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateKey();
            aes.GenerateIV();
            return (aes.Key, aes.IV);

        }

        public static List<byte[]> UniqueIvAndKeyGenerating()
        {
            var newKeys = GetAesKey();

            return new List<byte[]>() { newKeys.key, newKeys.iv };
        }

        public static string AesEncrypt(byte[] key, byte[] iv, string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;
            var cryptTransform = aes.CreateEncryptor();
            var plaintext = Encoding.UTF8.GetBytes(plainText);
            var cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);
            return Convert.ToBase64String(cipherText);
        }

        public static string AesDecrypt(byte[] key, byte[] iv, string encryptedText)
        {
            using Aes aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;
            var cryptTransform = aes.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public static string RsaEncrypt(byte[] data)
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            RSA rsa = RSA.Create();
            var collection = new X509Certificate2Collection();
            collection.ImportFromPemFile(path + @"/TestPublicCert.pem");
            var certificate = collection[0];
            var output = "";
            using RSA csp = (RSA)certificate.PublicKey.Key;
            byte[] bytesEncrypted = csp.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            output = Convert.ToBase64String(bytesEncrypted);
            return output;
        }
        public static bool signatureVerfication(string paylodMsg, string signature)
        {
            bool retVal = false;
            var encoder = new UTF8Encoding();
            byte[] bytesToVerify = encoder.GetBytes(paylodMsg);

            byte[] signedBytes = Convert.FromBase64String(signature);

            try
            {
                using (var rsaProvider = new RSACryptoServiceProvider())
                {

                    string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    RSA rsa = RSA.Create();
                    var collection = new X509Certificate2Collection();
                    collection.ImportFromPemFile(path + @"/TestPublicCert.pem");
                    var certificate = collection[0];

                    using (RSA publicKey = certificate.GetRSAPublicKey())
                    {
                        retVal = publicKey.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
           
            return retVal;
        }

    }
}