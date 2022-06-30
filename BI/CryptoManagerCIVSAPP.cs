using Microsoft.Extensions.Configuration;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExternalAPIs.BI
{
    public class CryptoManagerCIVSAPP
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Mode = CipherMode.ECB;
                //aes.Padding = PaddingMode.None;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Mode = CipherMode.ECB;
                //aes.Padding = PaddingMode.None;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd().Trim();
                        }
                    }
                }
            }
        }


        public static string EncryptUsingCopassCertificateRSA(string Branchkey)
        {
            try
            {
                byte[] byteData = Convert.FromBase64String(Branchkey);
                // byte[] byteData = Encoding.UTF8.GetBytes(encryptText);

                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CoPassInCert4096.pem");
                var collection = new X509Certificate2Collection();
                collection.Import(path);
                var certificate = collection[0];
                var output = "";
                using (RSA csp = (RSA)certificate.PublicKey.Key)
                {
                    byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.OaepSHA1);
                    output = Convert.ToBase64String(bytesEncrypted);
                }
                
                return output;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }



        public static string DecryptUsingCopassCertificateRSA(string encryptText)
        {

            try
            {
                byte[] byteData = Convert.FromBase64String(encryptText);
                // byte[] byteData = Encoding.UTF8.GetBytes(encryptText);

                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CoPassInCert4096.pfx");
                var Password = "Security.123"; //Note This Password is That Password That We Have Put On Generate Keys  
                var collection = new X509Certificate2Collection();
                collection.Import(System.IO.File.ReadAllBytes(path), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                X509Certificate2 certificate = new X509Certificate2();
                certificate = collection[0];
                foreach (var cert in collection)
                {
                    if (cert.FriendlyName.Contains("copass.in"))
                    {
                        certificate = cert;
                    }
                }
                if (certificate.HasPrivateKey)
                {
                    RSA csp = (RSA)certificate.PrivateKey;
                    var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
                    var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));

                    return keys;
                }

                //RSA rsa = (RSA)certificate.PrivateKey;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
     
        }

    }
}
