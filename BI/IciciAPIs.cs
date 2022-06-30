using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ExternalAPIs.BI
{

    public static class IciciAPIs
    {
        public static string getApiKey(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["MISAPIKey"];
        }
        public static string rsaEncrypt(string sampleText, string apitype)
        {
            return CryptoManagerICICI.EncryptUsingCertificate(sampleText, apitype);
        }
        public static RestRequest setRequestHeader(IConfiguration _config, RestRequest request, string configmode, string bank_id)
        {
            // JToken headerToken =  return_config_json(configmode);

            JObject BankData = DA.MISAPPDA.getbankapiDetails(bank_id, _config);
            JToken headerToken = BankData;
            request.AddHeader("x-forwarded-for", headerToken.SelectToken("Param.x-forwarded-for").ToString());
            //request.AddHeader("host", _config.GetSection("Appsetting")["icicihost"]);
            request.AddHeader("apikey", headerToken.SelectToken("Param.apikey").ToString());
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "text/plain");
            

            return request;
        }
        public static RestRequest setRequestHeader_upi(IConfiguration _config, RestRequest request, string configmode, string bank_id)
        {
            // JToken headerToken =  return_config_json(configmode);

            JObject BankData = DA.MISAPPDA.getbankapiDetails(bank_id, _config);
            JToken headerToken = BankData;
            request.AddHeader("x-forwarded-for", headerToken.SelectToken("Param.x-forwarded-for").ToString());
            //request.AddHeader("host", _config.GetSection("Appsetting")["icicihost"]);
            //request.AddHeader("apikey", "YXPhRl5LdYgUiAydww7tbTOWP6XNayQ4");
            //request.AddHeader("apikey", "73280ee0abc14eb5b3cde4e8175c2a64");
            request.AddHeader("apikey", " IGUiAsL1mL5k1kfJBbLXoIo31UjEWxQj");
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("x-priority", "1000");

            return request;
        }
        public static JToken getRequestHeader(IConfiguration _config, string configmode)
        {
            JToken headerToken = return_config_json(configmode);
            return headerToken;
        }
        public static Dictionary<string, string> getRequestParams(RestRequest request)
        {
            Dictionary<string, string> reqParams = new Dictionary<string, string>();

            foreach (var p in request.Parameters)
            {
                reqParams.Add(p.Name, p.Value.ToString());
            }

            return reqParams;
        }
        public static string Register(  string AGGRNAME, string AGGRID , string CORPID , string USERID,string URN, IConfiguration _config)
        {
            JObject BankData = DA.MISAPPDA.getbankapiDetails("1", _config);
            JToken headerToken = BankData;
            string Propert_Type = "";
            string Value_type = "";
            string iciciRegApi = "";

            iciciRegApi = headerToken.SelectToken("Param.iciciRegApi").ToString();

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);
            request = IciciAPIs.setRequestHeader(_config, request, "Live", "1");

            JObject requestData = new JObject(
               new JProperty("AGGRNAME", AGGRNAME),
               new JProperty("AGGRID", AGGRID),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("URN", URN)
            );

            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), "Live");


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);

            IRestResponse result = restClient.Execute(request);

            //string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            //string decryptedResponse = CryptoManagerICICI.DecryptResponse(EncryptedJsonPayload);
            //string decryptedResponse = result.Content.ToString();
            string decryptedResponse = CryptoManagerICICI.DecryptUsingCertificateRSA(result.Content.ToString());

            return decryptedResponse;
        }
        public static string deregister(string AGGRNAME, string AGGRID, string CORPID, string USERID, string URN, IConfiguration _config)
        {

            JObject BankData = DA.MISAPPDA.getbankapiDetails("1", _config);
            JToken headerToken = BankData;
            string iciciRegApi = headerToken.SelectToken("Param.iciciDeRegApi").ToString();

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, "Live", "1");

            JObject requestData = new JObject(
               new JProperty("AGGRNAME", AGGRNAME),
               new JProperty("AGGRID", AGGRID),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("URN", URN)
            );

            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), "Live");


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);

            IRestResponse result = restClient.Execute(request);

            //string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
           // string decryptedResponse = result.Content.ToString();
            string decryptedResponse = CryptoManagerICICI.DecryptUsingCertificateRSA(result.Content.ToString());
            return decryptedResponse;
        }
        public static string RegisterStatus(string AGGRNAME, string AGGRID, string CORPID, string USERID, string URN, IConfiguration _config)
        {

            JObject BankData = DA.MISAPPDA.getbankapiDetails("1", _config);
            JToken headerToken = BankData;
            string iciciRegApi = headerToken.SelectToken("Param.iciciRegStatApi").ToString();

            //string iciciRegApi = _config.GetSection("Appsetting")["iciciRegStatApi"];+

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, "Live", "1");

            JObject requestData = new JObject(
               new JProperty("AGGRNAME", AGGRNAME),
               new JProperty("AGGRID", AGGRID),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("URN", URN)
            );


            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), "Live");


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);

            IRestResponse result = restClient.Execute(request);

            string reqParams = "";
            foreach (Parameter param in request.Parameters)
            {
                reqParams = reqParams + param.Name + " = " + param.Value.ToString() + Environment.NewLine;
            }

            // string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());

            string decryptedResponse = CryptoManagerICICI.DecryptUsingCertificateRSA(result.Content.ToString());

            return decryptedResponse;
        }
        public static string BalanceFetch(IConfiguration _config, string configmode, string bank_id,string hoid)
        {
            JObject BankData = DA.MISAPPDA.getbankapiDetails(bank_id, _config);
            JToken headerToken = BankData;
            string iciciRegApi = headerToken.SelectToken("Param.iciciBalFetchApi").ToString();

            JObject Soc_details = DA.MISAPPDA.getsocietybankDetails( hoid, _config);
            JToken headerToken1 = Soc_details;
            
            string AGGRID = headerToken1.SelectToken("Acc_details[0].AGGRID").ToString();
            string CORPID = headerToken1.SelectToken("Acc_details[0].CORPID").ToString();
            string USERID = headerToken1.SelectToken("Acc_details[0].USERID").ToString();
            string URN = headerToken1.SelectToken("Acc_details[0].URN").ToString();
            string ACCOUNTNO = headerToken1.SelectToken("Acc_details[0].ACCNO").ToString();
            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);
            
            request = IciciAPIs.setRequestHeader(_config, request, configmode, bank_id);

            JObject requestData = new JObject(

               new JProperty("AGGRID", AGGRID),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("URN", URN),
               new JProperty("ACCOUNTNO", ACCOUNTNO)
            );
            Common.write_log_Success("Balancefetch.aspx | iciciReq Method Response |", "Balance Fetch API Status 1 : " + requestData);
            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), configmode);
            Common.write_log_Success("Balancefetch.aspx | iciciReq Method Response |", "Balance Fetch API Status 2 : " + EncryptedJsonPayload);

            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);

            string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            Common.write_log_Success("Balancefetch.aspx | iciciReq Method Response |", "Balance Fetch API Status 3 : " + result);
            Common.write_log_Success("Balancefetch.aspx | iciciReq Method Response |", "Balance Fetch API Status 4 : " + decryptedResponse);
            //string decryptedResponse = result.Content.ToString();

            return decryptedResponse;
        }
        public static string BankStatementAPI( string AGGRID, string CORPID, string USERID, string URN,string ACCOUNTNO,DateTime fromdate, DateTime todate, IConfiguration _config)
        {

            JObject BankData = DA.MISAPPDA.getbankapiDetails("1", _config);
            JToken headerToken = BankData;
            string iciciRegApi = headerToken.SelectToken("Param.iciciLinkedBankStatementAPI").ToString();


            //            string iciciRegApi = _config.GetSection("Appsetting")["iciciLinkedBankStatementAPI"];

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, "Live", "1");

            JObject requestData = new JObject(
               new JProperty("AGGRID", AGGRID),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("URN", URN),
               new JProperty("ACCOUNTNO", ACCOUNTNO),
               new JProperty("FROMDATE", fromdate),
               new JProperty("TODATE", todate)
            );

            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), "Live");


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);

            //  string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            string decryptedResponse = result.Content.ToString();

            return decryptedResponse;
        }
        public static string BankStatementPagination(IConfiguration _config, string configmode, string bank_id)
        {

            JObject BankData = DA.MISAPPDA.getbankapiDetails(bank_id, _config);
            JToken headerToken = BankData;
            string iciciRegApi = headerToken.SelectToken("Param.iciciLinkedBankStatementPagination").ToString();

            //string iciciRegApi = _config.GetSection("Appsetting")["iciciLinkedBankStatementPagination"];

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, configmode, bank_id);

            JObject requestData = new JObject(

               new JProperty("AGGRID", headerToken.SelectToken("Param.AGGRID").ToString()),
               new JProperty("CORPID", headerToken.SelectToken("Param.CORPID").ToString()),
               new JProperty("USERID", headerToken.SelectToken("Param.USERID").ToString()),
               new JProperty("URN", headerToken.SelectToken("Param.URN").ToString()),
               new JProperty("ACCOUNTNO", headerToken.SelectToken("Param.ACCOUNTNO").ToString())
            );

            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), configmode);


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);

            // string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            string decryptedResponse = result.Content.ToString();

            return decryptedResponse;
        }
        //public static string Bankstatment_api_paging(IConfiguration _config, int Record, string configmode, string bank_id)
        //{

        //    string response = "";
        //    if (Record <= 200)
        //    {
        //        response = BankStatementAPI(AGGRID, CORPID, USERID, URN, ACCOUNTNO, _config);
        //    }
        //    else
        //    {
        //        response = BankStatementPagination(_config, configmode, bank_id);
        //    }
        //    return response;

        //}
        public static List<string> Transactional(IConfiguration _config, string ben_acc, string ben_bank, string ben_ifsc, string ben_amount, string configmode, JObject Bank_details, string Bank_id,JObject Soc_details, string reqid , string trans_type , string ben_name)
        {

            List<string> response = new List<string>();

            dynamic obj = Bank_details;

            JObject json = Bank_details;
            JToken headerToken = json;

            dynamic obj1 = Soc_details;
            JObject json1 = Soc_details;
            JToken headerToken1 = json1;
            string AGGRNAME = headerToken1.SelectToken("Acc_details[0].AGGRNAME").ToString();
            string AGGRID = headerToken1.SelectToken("Acc_details[0].AGGRID").ToString();
            string CORPID = headerToken1.SelectToken("Acc_details[0].CORPID").ToString();
            string USERID = headerToken1.SelectToken("Acc_details[0].USERID").ToString();
            string URN = headerToken1.SelectToken("Acc_details[0].URN").ToString();
            string ACCOUNTNO = headerToken1.SelectToken("Acc_details[0].ACCNO").ToString();
            //  JToken headerToken = return_config_json(configmode);
            string iciciRegApi = headerToken.SelectToken("Param.iciciLinkedTransactionOTP").ToString();
            
            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, configmode, Bank_id);

            JObject requestData = new JObject(

               new JProperty("AGGRID", AGGRID),

               new JProperty("AGGRNAME", AGGRNAME),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("URN", URN),
               new JProperty("UNIQUEID", reqid),
               new JProperty("DEBITACC", ACCOUNTNO),
               new JProperty("CREDITACC", ben_acc),
               new JProperty("IFSC", ben_ifsc),
               new JProperty("AMOUNT", ben_amount),
               new JProperty("CURRENCY", "INR"),
               new JProperty("TXNTYPE", trans_type),
               new JProperty("PAYEENAME", ben_name),
               new JProperty("REMARKS", "Test"),
               new JProperty("CUSTOMERINDUCED", "N"),
               new JProperty("ACCOUNTNO", ACCOUNTNO)
            );
            Common.write_log_Success("Transaction.aspx | iciciReq Method Response |", "trans out API Status 1 : " + requestData);
            var dataVal = requestData.ToString();
            String input_string = Convert.ToString(dataVal);
            response.Add(input_string);
            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), configmode);
            Common.write_log_Success("Transaction.aspx | iciciReq Method Response |", "trans out API Status 2 : " + EncryptedJsonPayload);

            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);

            string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            //string decryptedResponse = result.Content.ToString();
            response.Add(decryptedResponse);
            Common.write_log_Success("Transaction.aspx | iciciReq Method Response |", "trans out API Status 2 : " + result);
            Common.write_log_Success("Transaction.aspx | iciciReq Method Response |", "trans out API Status 2 : " + decryptedResponse);
            return response;
            
        }
        public static string TransactionalOTP(IConfiguration _config, string ben_acc, string ben_bank, string ben_ifsc, string ben_amount, string configmode, JObject Bank_details, string Bank_id, string reqid)
        {
            dynamic obj = Bank_details;

            JObject json = Bank_details;
            JToken headerToken = json;
            
            string iciciRegApi = headerToken.SelectToken("Param.iciciLinkedTransactionOTP").ToString();

            //  string iciciRegApi = _config.GetSection("Appsetting")["iciciLinkedTransactionOTP"];

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, configmode, Bank_id);

            JObject requestData = new JObject(

               new JProperty("AGGRID", headerToken.SelectToken("Param.AGGRID").ToString()),
               new JProperty("AGGRNAME", headerToken.SelectToken("Param.AGGRNAME").ToString()),
               new JProperty("CORPID", headerToken.SelectToken("Param.CORPID").ToString()),
               new JProperty("USERID", headerToken.SelectToken("Param.USERID").ToString()),
               new JProperty("URN", headerToken.SelectToken("Param.URN").ToString()),
                new JProperty("UNIQUEID", reqid),
                 new JProperty("DEBITACC", headerToken.SelectToken("Param.DEBITACC").ToString()),

               new JProperty("CREDITACC", ben_acc),
               new JProperty("IFSC", ben_ifsc),
               new JProperty("AMOUNT", ben_amount),
                new JProperty("CURRENCY", headerToken.SelectToken("Param.CURRENCY").ToString()),
                 new JProperty("TXNTYPE", headerToken.SelectToken("Param.TXNTYPE").ToString()),
                 new JProperty("OTP", headerToken.SelectToken("Param.OTP").ToString()),
                 new JProperty("PAYEENAME", headerToken.SelectToken("Param.PAYEENAME").ToString()),
               new JProperty("REMARKS", "ICICIOTP GENRATION"),
                new JProperty("WORKFLOW_REQD", headerToken.SelectToken("Param.WORKFLOW_REQD").ToString()),
                 new JProperty("BENLEI", headerToken.SelectToken("Param.BENLEI").ToString()),

               new JProperty("ACCOUNTNO", headerToken.SelectToken("Param.ACCOUNTNO").ToString())
            );


            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), configmode);


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);

                  string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());

            //string decryptedResponse = result.Content.ToString();

            return decryptedResponse;
        }
        public static string CreateOTP(IConfiguration _config, string ben_acc, string ben_bank, string ben_ifsc, string ben_amount, string configmode, JObject Bank_details, string Bank_id, string reqid)
        {
            dynamic obj = Bank_details;

            JObject json = Bank_details;
            JToken headerToken = json;

            string iciciRegApi = headerToken.SelectToken("Param.iciciLinkedCreateOTP").ToString();

            // string iciciRegApi = _config.GetSection("Appsetting")["iciciLinkedCreateOTP"];

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, configmode, Bank_id);
            JObject requestData = new JObject(

               new JProperty("AGGRID", headerToken.SelectToken("Param.AGGRID").ToString()),
               new JProperty("AGGRNAME", headerToken.SelectToken("Param.AGGRNAME").ToString()),
               new JProperty("CORPID", headerToken.SelectToken("Param.CORPID").ToString()),
               new JProperty("USERID", headerToken.SelectToken("Param.USERID").ToString()),
               new JProperty("URN", headerToken.SelectToken("Param.URN").ToString()),
               new JProperty("UNIQUEID", reqid)
            );

            var dataVal = requestData.ToString();

            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), configmode);


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);

            //string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            string decryptedResponse = result.Content.ToString();

            return decryptedResponse;
        }
        public static string TransactionalStatus(string AGGRID, string CORPID, string USERID, string UNIQUEID,string URN, IConfiguration _config)
        {
            JObject BankData = DA.MISAPPDA.getbankapiDetails("1", _config);
            JToken headerToken = BankData;
            string iciciRegApi = headerToken.SelectToken("Param.iciciLinkedTransactionalStatus").ToString();
            Common.write_log_Success("trans_status.aspx | iciciReq Method Response |", "API Status : " + iciciRegApi);
            //string iciciRegApi = _config.GetSection("Appsetting")["iciciRegStatApi"];+

            var restClient = new RestClient(iciciRegApi);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader(_config, request, "Live", "1");

            JObject requestData = new JObject(
               new JProperty("AGGRID", AGGRID),
               new JProperty("CORPID", CORPID),
               new JProperty("USERID", USERID),
               new JProperty("UNIQUEID", UNIQUEID),
               new JProperty("URN", URN)
            );
            Common.write_log_Success("trans_status.aspx | iciciReq Method Response |", "API Status : " + requestData);

            var dataVal = requestData.ToString();
            Common.write_log_Success("trans_status.aspx | iciciReq Method Response |", "API Status : " + dataVal);
            string EncryptedJsonPayload = CryptoManagerICICI.EncryptUsingCertificate(requestData.ToString(), "Live");


            string data = EncryptedJsonPayload;
            request.AddParameter("text/plain", data, ParameterType.RequestBody);

            IRestResponse result = restClient.Execute(request);

            string decryptedResponse = CryptoManagerICICI.DecryptUsingCertificateRSA(result.Content.ToString());
            Common.write_log_Success("trans_status.aspx | iciciReq Method Response |", "API Status : " + decryptedResponse);
            return decryptedResponse;
        }
        public static JToken return_config_json(string tokenStr)
        {

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "BI/ICIC_endpoint_config.json");
            JObject JObject = ReadJSONData(path);
            return JObject.SelectToken(tokenStr);
        }
        public static JObject ReadJSONData(string jsonFilename)
        {
            try
            {
                JObject jObject;
                // Read JSON directly from a file    
                using (StreamReader file = System.IO.File.OpenText(jsonFilename))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObject = (JObject)JToken.ReadFrom(reader);
                }
                return jObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occurred : " + ex.Message);
                return null;
            }
        }


        public static JObject upi_outward(IConfiguration _config,  JObject Bank_details,  JObject upi_details)
        {

            dynamic obj = Bank_details;
            JObject json = Bank_details;
            JToken headerToken = json;
   
            //  JToken headerToken = return_config_json(configmode);
            string iciciupitrans = headerToken.SelectToken("Param.iciciLinkedUPITrans").ToString();

            var restClient = new RestClient(iciciupitrans);
            var request = new RestRequest(Method.POST);

            request = IciciAPIs.setRequestHeader_upi(_config, request, "Live", "1");
            Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "UPI Apiurl : " + iciciupitrans);

            var dataVal = upi_details.ToString();
            String input_string = Convert.ToString(dataVal);
            Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "UPI request data: " + request.Parameters[0] +  request.Parameters[1]+ request.Parameters[2]+ request.Parameters[3]
                + request.Parameters[4]+ request.Parameters[5]);

            request.AddParameter("text/plain", dataVal, ParameterType.RequestBody);
            Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "UPI request data befor request: " + request.Parameters[0] + request.Parameters[1] + request.Parameters[2] + request.Parameters[3]
                + request.Parameters[4] + request.Parameters[5] + request.Parameters[6]);
            IRestResponse result = restClient.Execute(request);
            Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "UPI DATA Status 5 : " + result.Content);
            string response = result.Content;
            JObject response_data = JObject.Parse(BI.Common.validJason(response));
            //JObject response_data = JObject.Parse(BI.Common.validPrefixJason(response));

            

            string encryptedPayload = (string)response_data["encryptedData"];
            string encryptedkey = (string)response_data["encryptedKey"];
            string decryptedKey = (string)CryptoManagerICICI.DecryptResponse(encryptedkey);
            string decryptPayloadAes = CryptoManagerICICI.aesDecryptString(decryptedKey, encryptedPayload);

            Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Final data Status 10 : " + decryptPayloadAes);
           
            JObject json1 = JObject.Parse(BI.Common.validPrefixJason(decryptPayloadAes));
            JToken headerToken2 = json1;
            

            Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "success : " +json1 );

            //string decryptedResponse = CryptoManagerICICI.DecryptResponse(result.Content.ToString());
            ////string decryptedResponse = result.Content.ToString();
            //Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "UPI DATA Status 4 : " + decryptPayloadAes);
            ////Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "UPI DATA Status 4 : " + UPI_FINAL_DATA);

            return json1;

            

        }

        // E_COLLECTION
        public static JObject ECValidation(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string AcceptOrReject = "";
            string Message = "";
            string Code = "";
           
            //JObject Outputdata = new JObject();
            string Outputdata = "";
            dynamic obj = data;
            string Encrypted_output = "";
            string requestId = (string)obj["requestId"];
            string service = (string)obj["service"];
            string oaepHashingAlgorithm = (string)obj["oaepHashingAlgorithm"];
            string iv = (string)obj["iv"];
            string clientInfo = (string)obj["clientInfo"];
            string optionalParam = (string)obj["optionalParam"];
            string encryptedPayload = (string)obj["encryptedData"];
            string encryptedkey = (string)obj["encryptedKey"];
            JObject Final_output = new JObject();
            string decryptedKey = (string)CryptoManagerICICI.DecryptResponse(encryptedkey);
            string decryptPayloadAes = CryptoManagerICICI.aesDecryptString(decryptedKey, encryptedPayload);
            try
            {
                JObject json = JObject.Parse(BI.Common.validPrefixJason(decryptPayloadAes));
                JToken headerToken = json;
                string VirtualAccountNumber = headerToken.SelectToken("VirtualAccountNumber").ToString();
                string UTR = headerToken.SelectToken("UTR").ToString();
                string ClientAccountNo = headerToken.SelectToken("ClientAccountNo").ToString();
                //1
               // int response = Convert.ToInt32(DA.MISAPPDA.ECValidation(VirtualAccountNumber, _config));
                JObject acc_data = DA.MISAPPDA.ECValidation(VirtualAccountNumber, _config);
                int count = Convert.ToInt32(headerToken.SelectToken("count").ToString());

                Common.write_log_Success("ECValidation.aspx | iciciReq Method Response |", "API Status : " + data);
                Common.write_log_Success("ECValidation.aspx | iciciReq Method Response |", "API Status : " + decryptPayloadAes);
                Common.write_log_Success("ECValidation.aspx | iciciReq Method Response |", "API Status : " + count);

                if (count > 0)
                        {
                            AcceptOrReject = "Y";
                            Message = "Success";
                            Code = "11";

                        }
                else
                {
                    AcceptOrReject = "N";
                    Message = "Reject";
                    Code = "12";
                }

                Outputdata = new JObject(new JProperty("AcceptOrReject", AcceptOrReject), new JProperty("Message", Message), new JProperty("Code", Code)).ToString();
                Common.write_log_Success("ECValidation.aspx | iciciReq Method Response |", "API Status : " + Outputdata);
                //generate random key
                string keyForICICIForEncryption = CryptoManagerICICI.RandomString(16);

                //encrypt key with icici certificate

                string encryptedKeyForResponse = CryptoManagerICICI.EncryptUsingCertificate(keyForICICIForEncryption, "icici_uat");

                //aes encryption of response data

                aesReturn aesReturn = CryptoManagerICICI.aesEncryptStringReturn(keyForICICIForEncryption, keyForICICIForEncryption + Outputdata);

                // generate response for icicic

                Final_output = new JObject(new JProperty("requestId",requestId),
                    new JProperty("service",service),
                    new JProperty("encryptedKey", encryptedKeyForResponse),
                    new JProperty("oaepHashingAlgorithm",oaepHashingAlgorithm),
                    //new JProperty("iv",aesReturn.iv),
                    new JProperty("iv", ""),
                    new JProperty("encryptedData", aesReturn.encryptedText),
                    new JProperty("clientInfo",clientInfo),
                    new JProperty("optionalParam",optionalParam));

                var decryptedOutput = CryptoManagerICICI.aesDecryptString(keyForICICIForEncryption, aesReturn.encryptedText);
                Common.write_log_Success("ECValidation.aspx | iciciReq Method Response |", "API Status : " + Final_output);
            }
            catch (Exception ex)
            {

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            return Final_output;
        }
        public static JObject MISPosting(IConfiguration _config, JObject data)
        
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string Response = "";
            string Code = "";
            string Outputdata = "";
            dynamic obj = data;
            string Encrypted_output = "";
            string requestId = (string)obj["requestId"];
            string service = (string)obj["service"];
            string oaepHashingAlgorithm = (string)obj["oaepHashingAlgorithm"];
            string iv = (string)obj["iv"];
            string clientInfo = (string)obj["clientInfo"];
            string optionalParam = (string)obj["optionalParam"];
            string encryptedPayload = (string)obj["encryptedData"];
            string encryptedkey = (string)obj["encryptedKey"];
            JObject Final_output = new JObject();
            string decryptedKey = (string)CryptoManagerICICI.DecryptResponse(encryptedkey);
            string decryptPayloadAes = CryptoManagerICICI.aesDecryptString(decryptedKey, encryptedPayload);
            Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + data);
            Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + decryptPayloadAes);
            try
            {
                JObject json = JObject.Parse(BI.Common.validPrefixJason(decryptPayloadAes));
                JToken headerToken = json;
                string VirtualAccountNumber = headerToken.SelectToken("VirtualAccountNumber").ToString();
                string UTR = headerToken.SelectToken("UTR").ToString();
                string ClientAccountNo = headerToken.SelectToken("ClientAccountNo").ToString();
                string amount = headerToken.SelectToken("Amount").ToString();
                string trans_type = headerToken.SelectToken("Mode").ToString();

                int response = Convert.ToInt32(DA.MISAPPDA.UTRValidation(UTR, _config));
               
                if (response == 0)
                {
                    JObject acc_data = DA.MISAPPDA.ECValidation(VirtualAccountNumber, _config);
                    Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + acc_data);
                    int count = Convert.ToInt32(acc_data.SelectToken("Count").ToString());
                    string name = acc_data.SelectToken("Name").ToString();
                    Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + name);

                    JObject dataresponse= DA.MISAPPDA.income_response_insert(json, _config);
                    Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + dataresponse);
                    string reqid = dataresponse.SelectToken("ReqNo").ToString();
                    if (reqid != "")
                        Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + reqid);
                    {
                        if (name == "Loan")
                        {
                            Int64 Req_id = Convert.ToInt64(reqid);
                            double Amt = Convert.ToDouble(amount);
                            MisAppAPIs.loan_incoming_trans(Req_id, VirtualAccountNumber, Amt, trans_type, _config);
                        }
                        else if (name == "Saving")
                        {
                            DA.MISAPPDA.income_transaction(VirtualAccountNumber, reqid, amount, trans_type, _config);
                        }
                    }
                    Response = "Success";
                    Code = "11";

                }

                else
                {
                    Response = "Duplicate UTR";
                    Code = "06";
                }
                Outputdata = new JObject(new JProperty("Response", Response), new JProperty("Code", Code) ).ToString();
                Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + Outputdata);
                //generate random key
                string keyForICICIForEncryption = CryptoManagerICICI.RandomString(16);

                //encrypt key with icici certificate

                string encryptedKeyForResponse = CryptoManagerICICI.EncryptUsingCertificate(keyForICICIForEncryption, "icici_uat");

                //aes encryption of response data

                aesReturn aesReturn = CryptoManagerICICI.aesEncryptStringReturn(keyForICICIForEncryption, keyForICICIForEncryption+Outputdata);

                // generate response for icicic

                Final_output = new JObject(new JProperty("requestId", requestId),
                    new JProperty("service", service),
                    new JProperty("encryptedKey", encryptedKeyForResponse),
                    new JProperty("oaepHashingAlgorithm", oaepHashingAlgorithm),
                    //new JProperty("iv", aesReturn.iv),
                    new JProperty("iv",""),
                    new JProperty("encryptedData", aesReturn.encryptedText),
                    new JProperty("clientInfo", clientInfo),
                    new JProperty("optionalParam", optionalParam));
                var decryptedOutput = CryptoManagerICICI.aesDecryptString(keyForICICIForEncryption, aesReturn.encryptedText);

                Common.write_log_Success("MISPosting.aspx | iciciReq Method Response |", "API Status : " + Final_output);
            }
            catch (Exception ex)
            {
                Common.write_log_error("MISPosting.aspx | iciciReq Method Response |", "failed : " + ex);
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            return Final_output;
        }


        public static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
            catch
            {
                throw;
            }
        }
        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch
            {
                throw;
            }
        }
    }
    public class upiValidationRequest
    {
        public string Source { get; set; }
        public string SubscriberId { get; set; }
        public string TxnId { get; set; }
        public string MerchantKey { get; set; }
    }

    public class xml
    {
        public string CustName { get; set; }
        public string ActCode { get; set; }
        public string TxnId { get; set; }
        public string Message { get; set; }
    }
}
