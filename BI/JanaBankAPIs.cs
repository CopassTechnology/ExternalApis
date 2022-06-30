using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.Encryption;
using ExternalAPIs.BI.HelperClasses;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ExternalAPIs.DA;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace ExternalAPIs.BI
{
    public class JanaBankAPIs
    {
        public static string getApiKey(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["OrcaleConnectionlive"];
        }

        public static List<byte[]> getUniqueKeys()
        {
            return CryptoManagerJanaBank.UniqueIvAndKeyGenerating();
        }

        public static aesEncryptResponse aesEncrypt(string sampleText)
        {
            List<byte[]> keys = getUniqueKeys();
            string retData = CryptoManagerJanaBank.AesEncrypt(keys[0], keys[1], sampleText);
            aesEncryptResponse response = new aesEncryptResponse();
            response.key = keys[0];
            response.IV = keys[1];
            response.keyStr = Encoding.UTF8.GetString(response.key);
            response.ivStr = Encoding.UTF8.GetString(response.IV);
            response.encryptedKey = CryptoManagerJanaBank.RsaEncrypt(keys[0]);
            response.encryptedIV = CryptoManagerJanaBank.RsaEncrypt(keys[1]);
            response.RequestToken = DateTime.Now.ToString("MM-dd-yyyy");
            response.encryptedRequestToken = CryptoManagerJanaBank.RsaEncrypt(Encoding.ASCII.GetBytes(response.RequestToken));
            response.encryptedString = retData;
            return response;
        }

        
        public static bool SignVerification(signatureVerification data)
        {
            bool retVal = CryptoManagerJanaBank.signatureVerfication(data.payload, data.signature);
            return retVal;
        }

        
        public static string aesDecryption(encryptedPaylod data)
        {
            string retVal = CryptoManagerJanaBank.AesDecrypt(data.key, data.IV, data.encryptedString);
            return retVal;
        }
        //--------------------Babita Jana Bank Implementation---------------------------//
        public static JObject JanaBankIMPS(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string ReqID = (string)obj["ReqId"];

            Int32 BrId = 0;
            Int32 GLCode = 0;
            Int64 AccNo = 0;
            double Amount = 0;
            string Narration = "";
            string FullName = "";
            string Email = "";
            string MobileNo = "";
            string BankAccNo = "";
            string IFSCCode = "";
            int ErrCode = 0;
            string ErrMsg = "";
            Int64 UniqueReqNo = 0;
            //string API_URL = "";
            string message = "";


            // ------ check record ------  //
            JObject databaseoutput = JanaBankDA.getRequestDetails(ReqID, _config); 


             BrId = Convert.ToInt32(databaseoutput["BRID"]);
             GLCode = Convert.ToInt32(databaseoutput["DRGLCODE"]);
             AccNo = Convert.ToInt64(databaseoutput["DRACCNO"]);
             Amount = Convert.ToDouble(databaseoutput["AMOUNT"]);
             Narration = databaseoutput["REMARK"].ToString();
             FullName = databaseoutput["BENEFNAME"].ToString();
             Email = "abc@gmail.com";
             MobileNo = databaseoutput["MOBILE"].ToString();
             BankAccNo = databaseoutput["BANKACCNO"].ToString();
             IFSCCode = databaseoutput["IFSCCODE"].ToString();

            // ---------- Insert record into aoup_yb_reqres_log ------------ //

          
            JObject dbReqResProc = JanaBankDA.CallReqProc(ReqID, BrId, GLCode, AccNo, BankAccNo, 0, Amount, Narration, "admin", _config);

             ErrCode = Convert.ToInt32(dbReqResProc["ErrCode"]);
             ErrMsg = dbReqResProc["ErrMsg"].ToString();
            UniqueReqNo = Convert.ToInt64(dbReqResProc["UniqueReqNo"]);

            if (ErrCode == -100)
            {
                string StatusCode = "Pending";
                string SubStatusCode = "0300";
                string BankReferenceNo = "";
                string ResponseStr = "";
                string transferType = "";
                string UniqueResponseNo = "";



                JObject dbfinalOutput = JanaBankDA.UpdateResString(UniqueReqNo, ResponseStr, UniqueResponseNo, StatusCode, SubStatusCode, BankReferenceNo, ErrMsg, transferType, _config);

                ErrCode = Convert.ToInt32(dbfinalOutput["ErrCode"]);
                ErrMsg = dbfinalOutput["ErrMsg"].ToString();

                if (ErrCode == -100)
                {
                    message = "Transaction done successfully.";
                }
                else
                {
                    message = "Transaction Unsuccessful.";
                }
            }
            else
            {
                message = "There is problem in Transaction";
            }
  
            JObject responseObj = new JObject(
                new JProperty("message", message)
            );

            return responseObj;
        }

        public static JObject JanaBankReqAPI(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string ReqID = (string)obj["ReqId"];

            Int32 BrId = 0;
            Int32 GLCode = 0;
            Int64 AccNo = 0;
            double Amount = 0;
            string Narration = "";
            string FullName = "";
            string Email = "";
            string MobileNo = "";
            string BankAccNo = "";
            string IFSCCode = "";
            int ErrCode = 0;
            string ErrMsg = "";
            Int64 UniqueReqNo = 0;
            string API_URL = "";
            string message = "";

            // ------ check record ------  //
            JObject databaseoutput = JanaBankDA.getRequestDetailsAPI(ReqID, _config);


            BrId = Convert.ToInt32(databaseoutput["BRID"]);
            GLCode = Convert.ToInt32(databaseoutput["DRGLCODE"]);
            AccNo = Convert.ToInt64(databaseoutput["DRACCNO"]);
            Amount = Convert.ToDouble(databaseoutput["AMOUNT"]);
            Narration = databaseoutput["REMARK"].ToString();
            FullName = databaseoutput["BENEFNAME"].ToString();
            Email = "abc@gmail.com";
            MobileNo = databaseoutput["MOBILE"].ToString();
            BankAccNo = databaseoutput["BANKACCNO"].ToString();
            IFSCCode = databaseoutput["IFSCCODE"].ToString();
            ErrCode = Convert.ToInt32(databaseoutput["errorcode"]);

            // ---------- Insert record into aoup_yb_reqres_log ------------ //


            //JObject dbReqResProc = JanaBankDA.CallReqProc(ReqID, BrId, GLCode, AccNo, BankAccNo, 0, Amount, Narration, "admin", _config);

            //ErrCode = Convert.ToInt32(dbReqResProc["ErrCode"]);
            //ErrMsg = dbReqResProc["ErrMsg"].ToString();
            //UniqueReqNo = Convert.ToInt64(dbReqResProc["UniqueReqNo"]);

            if (ErrCode == -100)
            {
                String StatusCode = "";
                String SubStatusCode = "";
                String BankReferenceNo = "";
                String ResponseStr = "";
                String transferType = "";
                string UniqueResponseNo = "";

                // ---------- Creating request Json ------------ //

                var JSONresult = YesBankReq(_config, UniqueReqNo, FullName, Email, MobileNo, BankAccNo, IFSCCode, Amount);

                API_URL = "https://bst10pt.janabank.com:1084/ESPS/BankRequest";

                // ---------- Main Method sending request url ------------ //

                JObject API_Response = CallAPI(API_URL, JSONresult.ToString(), UniqueReqNo.ToString(), BankAccNo);

                string responseStr = API_Response["Responsestr"].ToString();
                string uniqueReseNo = API_Response["retrievalReferenceNumber"].ToString();
                string bankRefNo = API_Response["externalReferenceNo"].ToString();
                string requestType = API_Response["transfertype"].ToString();
                string Status = API_Response["Status"].ToString();


                if (Status == "Success")
                {

                    UniqueResponseNo = uniqueReseNo;
                    StatusCode = Status;
                    SubStatusCode = "0100";
                    BankReferenceNo = bankRefNo;
                    transferType = requestType;
                    ResponseStr = responseStr;
                    ErrCode = Convert.ToInt32(API_Response["ErrCode"]);
                    ErrMsg = API_Response["ErrMsg"].ToString();
                }
                else
                {

                    UniqueResponseNo = uniqueReseNo;
                    StatusCode = "Failed";
                    SubStatusCode = "0200";
                    BankReferenceNo = bankRefNo;
                    ResponseStr = responseStr;
                    ErrCode = Convert.ToInt32(API_Response["ErrCode"]);
                    ErrMsg = API_Response["ErrMsg"].ToString();
                }

                JObject dbfinalOutput = JanaBankDA.UpdateResString(Convert.ToInt32(ReqID), ResponseStr, UniqueResponseNo, StatusCode, SubStatusCode, BankReferenceNo, ErrMsg, transferType, _config);

                ErrCode = Convert.ToInt32(dbfinalOutput["ErrCode"]);
                ErrMsg = dbfinalOutput["ErrMsg"].ToString();

                
                if (ErrCode == -100)
            {
                message = "Transaction done successfully.";
            }
            else
            {
                message = "Transaction Unsuccessful.";
            }
            }

            JObject responseObj = new JObject(
                new JProperty("message", message)
            );

            return responseObj;
        }

        public static string YesBankReq(IConfiguration _config, Int64 UniqueRequestNo, String FullName, String Email, String MobileNo, String BankAccountNo, String IFSCCode, double Amount)
        {
                 
            var reqSer = "";
            try
            {
                string TransactionReferenceNo = JanaBankDA.GenerateRandomUserRefNo();

                var req = new
                {
                    MBRequest = new
                    {
                        headerInfo = new
                        {
                            dateTime = DateTime.Now.ToString("yyyyMMdd HH:mm:ss"),
                            consumerName = "ESPS",
                            requestType = "FundTransferIMPS",
                            projectID = ""
                        },
                        requestInfo = new
                        {
                            transRefNumber = "IB257195043795031088",
                            amtTxn = "1",
                            remarks = "SAVINGS",
                            benefAcctNo = "15510100010819",
                            benefIFSCCode = "FDRL0001551",

                            deviceIP= "180.179.183.29",
                            deviceID = "123456789012347",
                            latitude = "12.957317",
                            longitude= "77.648308",
                            userloginTime = DateTime.Now.ToString("yyyy-mm-dd  HH:MM:SS")

                        }
                    }

                };

                
                 reqSer = JsonConvert.SerializeObject(req, Formatting.Indented);
                reqSer = reqSer.Replace(@"=", ":");
            }

            catch (Exception ex)
            {

            }
            JanaBankDA.write_log_error("JanaBankDA.cs | YesBankReq |", reqSer.ToString());
            return reqSer;
        }

        public static JObject CallAPI(string URL, string RequestData, string UniqueRequestNo, string TransactionReferenceNo)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            String responseString = "";
            String DecryptResponseString = "";
            JObject DecryptionReturn = new JObject();
            string skStr = "";
            string ivStr = "";
            string uniqueresno = "";
            string bankreferenceno = "";
            string requestType = "";
            string Signature = "";
            string Payload = "";
            string Status = "";
            Int32 ErrorCode = 0;
            string ErrorMsg = "";
            try
            {

                var request = (HttpWebRequest)WebRequest.Create(URL);

                JObject returnData = JObject.Parse(JsonConvert.SerializeObject(aesEncrypt(RequestData)));

                 skStr = returnData.SelectToken(@"key").Value<string>();
                 ivStr = returnData.SelectToken(@"IV").Value<string>();

                JObject requestData = new JObject(
                    new JProperty("UserReferenceNo", UniqueRequestNo),
                    new JProperty("Source", "ESPS"),
                    new JProperty("TimeStamp", DateTime.Now.ToString("yyyyMMdd HH:mm:ss")),
                    new JProperty("SK", returnData.SelectToken(@"encryptedKey").Value<string>()),
                    new JProperty("SV", returnData.SelectToken(@"encryptedIV").Value<string>()),
                    new JProperty("Payload", returnData.SelectToken(@"encryptedString").Value<string>()),
                    new JProperty("TransactionReferenceNo", TransactionReferenceNo)

                );

                JanaBankDA.write_log_error("JanaBankDA.cs | CallAPI |", requestData.ToString());

                request.Method = "POST";
                request.Headers.Add("Consumerid", "19751511273714");
                request.Headers.Add("Requesttype", "FundTransferIMPS");
                request.Headers[HttpRequestHeader.Authorization] = "951984915b37d518cd46a4b7ddff8f9aeae97cf2f0ceacff71a0ed6055e8003e"; 
                request.ContentType = "application/json";
                request.Headers.Add("Requesttoken", returnData.SelectToken(@"encryptedRequestToken").Value<string>());

                JanaBankDA.write_log_error("JanaBankRTGS.aspx | CallAPI Requesttoken |", returnData.SelectToken(@"encryptedRequestToken").Value<string>());


                byte[] data = Encoding.ASCII.GetBytes(requestData.ToString());

                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {

                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }

                var response = (HttpWebResponse)request.GetResponse();

                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                JanaBankDA.write_log_error("JanaBankDA.cs | CallAPI ResponseString |", responseString.ToString());


                JObject Janabankresponse = JObject.Parse(responseString);
                 Signature = Janabankresponse["Signature"].ToString();
                 Payload = Janabankresponse["Payload"].ToString();
                 Status = Janabankresponse["Status"].ToString();


                signatureVerification sv = new signatureVerification();
                sv.signature = Signature;
                sv.payload = Payload;

                bool VerificationReturn = SignVerification(sv);

                if (VerificationReturn )
                {
                    encryptedPaylod decryptiondata = new encryptedPaylod();
                    decryptiondata.key = Convert.FromBase64String(skStr);
                    decryptiondata.IV = Convert.FromBase64String(ivStr);
                    decryptiondata.encryptedString = Payload;

                    DecryptResponseString = aesDecryption(decryptiondata).ToString();
                    DecryptionReturn = JObject.Parse(DecryptResponseString);
                    JToken headerToken = DecryptionReturn;

                    string headerInfo = headerToken.SelectToken("MBResponse.headerInfo").ToString();
                    JObject Jheaderobj = JObject.Parse(headerInfo);

                    string transactionInfo = headerToken.SelectToken("MBResponse.transactionInfo").ToString();
                    JObject Jtransactionobj = JObject.Parse(transactionInfo);

                    string responseInfo = headerToken.SelectToken("MBResponse.responseInfo").ToString();
                    JObject Jresponseobj = JObject.Parse(responseInfo);

                    requestType = Jheaderobj.SelectToken("requestType").ToString();
                    uniqueresno = Jresponseobj.SelectToken("retrievalReferenceNumber").ToString();
                    bankreferenceno = Jtransactionobj.SelectToken("externalReferenceNo").ToString();
                    ErrorCode = Convert.ToInt32(Jtransactionobj.SelectToken("errorCode"));
                    ErrorMsg = Jtransactionobj.SelectToken("replyText").ToString();


                }

                
                DecryptionReturn = new JObject(
                    new JProperty("Responsestr", DecryptResponseString),
                    new JProperty("retrievalReferenceNumber", uniqueresno),
                    new JProperty("externalReferenceNo", bankreferenceno),
                    new JProperty("transfertype", requestType),
                     new JProperty("Status", Status),
                     new JProperty("ErrCode", ErrorCode),
                     new JProperty("ErrMsg", ErrorMsg)
                     );





            }
            catch (WebException ex)
            {


                using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
            }

            JanaBankDA.write_log_error("JanaBankDA.cs | CallAPI DecryptedString |", DecryptionReturn.ToString());

            return DecryptionReturn;
        }

        
    }
}


