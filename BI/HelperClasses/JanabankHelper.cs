using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExternalAPIs.BI.HelperClasses
{
    public class aesEncryptResponse
    {
        public byte[] key { get; set; }
        public byte[] IV { get; set; }

        public string keyStr { get; set; }
        public string ivStr { get; set; }

        public string encryptedString { get; set; }

        public string encryptedKey { get; set; }

        public string encryptedIV { get; set; }

        public string RequestToken { get; set; }
        public string encryptedRequestToken { get; set; }
    }

    public class signatureVerification
    {
        public string signature { get; set; }

        public string payload { get; set; }
    }

    public class encryptedPaylod
    {
        public byte[] key { get; set; }
        public byte[] IV { get; set; }
        public string encryptedString { get; set; }
    }
}
