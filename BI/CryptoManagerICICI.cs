 using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThirdParty.BouncyCastle.OpenSsl;

namespace ExternalAPIs.BI
{
    public class CryptoManagerICICI
    {

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string EncryptUsingCertificate(string data, string apitype)
        {

            try
            {
                byte[] byteData = Encoding.UTF8.GetBytes(data);
                string path = "";
                if (apitype=="Live")
                {
                    path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ICICI_cert_prod.pem");
                }
                else if (apitype == "Uat")
                {
                    path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ICICI_cert.pem");
                }
                else if (apitype == "icici_uat")
                {
                    path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "icici_public_key_uat.pem");
                }
                else if (apitype == "icici_uat_upi")
                {
                    path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "icici_uat_upi.pem");
                }
                else if (apitype == "icici_Live_upi")
                {
                    path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "icici_Live_upi.pem");
                }
                var collection = new X509Certificate2Collection();
                collection.Import(path);
                var certificate = collection[0];
                var output = "";
                using (RSA csp = (RSA)certificate.PublicKey.Key)
                {
                    byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.Pkcs1);
                    output = Convert.ToBase64String(bytesEncrypted);
                }
                return output;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string DecryptUsingCertificateRSA(string encryptText)
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
                    var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.Pkcs1));
                    return keys;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        public static string GetKeys(string encryptText)
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string CertPem = File.ReadAllText(path+ @"/copassCertificate.txt");
            string EccPem = File.ReadAllText(path + @"/copassCertPrivateKey.txt");

            var cert = new X509Certificate2(Convert.FromBase64String(CertPem));
           

            // parsing an ECC key from a PEM file. PEM labels have been stripped
            var key = ECDsa.Create();
            key.ImportECPrivateKey(Convert.FromBase64String(EccPem), out _);
          
            var certWithPrivateKey = cert.CopyWithPrivateKey(key);

            return CertPem;
        }
        public static string DecryptUsingCertificate(string encryptText)
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = path + @"\CoPassInCert4096.pfx";
                var password = "Security.123";

                var collection = new X509Certificate2Collection();
                collection.Import(path, password, X509KeyStorageFlags.PersistKeySet);
                var certificate = collection[0];

                if (certificate.HasPrivateKey)
                {
                    RSA rsa = (RSA)certificate.PrivateKey;
                    (certificate.PrivateKey as RSACng).Key.SetProperty(
                        new CngProperty(
                            "Export Policy",
                            BitConverter.GetBytes((int)CngExportPolicies.AllowPlaintextExport),
                            CngPropertyOptions.Persist));

                    RSAParameters RSAParameters = rsa.ExportParameters(true);

                    AsymmetricCipherKeyPair keypair = DotNetUtilities.GetRsaKeyPair(RSAParameters);

                    var bytesData = Convert.FromBase64String(encryptText);
                    var recieverPrivate = ((RsaPrivateCrtKeyParameters)keypair.Private).DQ.ToByteArrayUnsigned();
                    KeyParameter keyparam = ParameterUtilities.CreateKeyParameter("DES", recieverPrivate);
                    IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/ISO7816_4PADDING");
                    cipher.Init(false, keyparam);
                 
                    var dataByte = cipher.DoFinal(bytesData);
                    return Convert.ToBase64String(dataByte);
                }

                //RSA rsa = (RSA)certificate.PrivateKey;
              
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        public static string DecryptResponse(string encryptText)
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = path + @"\CoPassInCert4096.pfx";
                var password = "Security.123";

                var collection = new X509Certificate2Collection();
                collection.Import(path, password, X509KeyStorageFlags.PersistKeySet);
                var certificate = collection[0];

                if (certificate.HasPrivateKey)
                {
                    var rsa = certificate.GetRSAPrivateKey();
                    byte[] binaryData = Convert.FromBase64String(encryptText);
                    //byte[] result = rsa.Decrypt(binaryData, RSAEncryptionPadding.Pkcs1);
                    byte[] result = rsa.Decrypt(binaryData, RSAEncryptionPadding.Pkcs1);
                    return Encoding.UTF8.GetString(result);

                }

                //RSA rsa = (RSA)certificate.PrivateKey;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        public static string DecryptResponseNone(string encryptText)
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = path + @"\CoPassInCert4096.pfx";
                var password = "Security.123";

                var collection = new X509Certificate2Collection();
                collection.Import(path, password, X509KeyStorageFlags.PersistKeySet);
                var certificate = collection[0];

                if (certificate.HasPrivateKey)
                {
                    var rsa = certificate.GetRSAPrivateKey();
                    byte[] binaryData = Convert.FromBase64String(encryptText);
                    byte[] result = rsa.Decrypt(binaryData, RSAEncryptionPadding.Pkcs1);
                    return Encoding.UTF8.GetString(result);

                }

                //RSA rsa = (RSA)certificate.PrivateKey;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        //public static string DecryptUsingCertificateinword(string data, string apitype)
        //{

        //    try
        //    {
        //        byte[] byteData = Encoding.UTF8.GetBytes(data);
        //        string path = "";
        //        if (apitype == "Live")
        //        {
        //            path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ICICI_cert_prod.pem");
        //        }
        //        else if (apitype == "Uat")
        //        {
        //            path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ICICI_cert.pem");
        //        }
        //        var collection = new X509Certificate2Collection();
        //        collection.Import(path);
        //        var certificate = collection[0];
        //        var output = "";
        //        using (RSA csp = (RSA)certificate.PublicKey.Key)
        //        {
        //            byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.Pkcs1);
        //            output = Convert.ToBase64String(bytesEncrypted);
        //        }
        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}
        public static string aesEncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

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
        public static string aesDecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static aesReturn aesEncryptStringReturn(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            aesReturn aesReturn = new aesReturn();

            aesReturn.Key = key;
            aesReturn.iv = Convert.ToBase64String(iv);
            aesReturn.plainText = plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

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
            aesReturn.encryptedText = Convert.ToBase64String(array);
            return aesReturn;
        }

    }

    public class aesReturn
    {
        public string Key { get; set; }
        public string iv { get; set; }
        public string plainText { get; set; }
        public string encryptedText { get; set; }
    }
}

    

