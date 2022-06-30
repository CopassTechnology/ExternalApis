using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using ZXing;
using Grpc.Core;
using ZXing.Mobile;
using QRCoder;
using System.Drawing;
using System.IO;

namespace ExternalAPIs.BI
{
    public class QR_Code_genrator
    {
        public class ResponseFields
        {
            public string base64QRCode { get; set; }
        }

        public class ErrorFields
        {
            public string cd { get; set; }
            public string description { get; set; }
        }

        public string SendRequest(string upi_id)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData qrCodeData = _qrCode.CreateQrCode(upi_id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            Byte[] imgdate=BitmapToBytesCode(qrCodeImage);
            string base64string= Convert.ToBase64String(imgdate,0,imgdate.Length);
            return base64string;
        }

      
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }


    }
}




