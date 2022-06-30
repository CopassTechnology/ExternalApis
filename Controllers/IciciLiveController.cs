using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.BI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace ExternalAPIs.Controllers
{
    [Route("api/icici/live")]
    [ApiController]
    public class IciciLiveController : ControllerBase
    {

        private readonly IConfiguration _config;

        public IciciLiveController(IConfiguration configuration)
        {
            _config = configuration;

        }


        //[HttpGet("register")]
        //public IActionResult register(string bank_id)
        //{
        //    string response = IciciAPIs.Register(_config, "Live");
        //    return Ok(response);
        //}

        //[HttpGet("registerStatus")]
        //public IActionResult registerStatus(string bank_id)
        //{
        //    string response = IciciAPIs.RegisterStatus(_config, "Live", bank_id);

        //    return Ok(response);
        //}

        //[HttpGet("deregister")]
        //public IActionResult deregister(string bank_id,string hoid)
        //{
        //    string response = IciciAPIs.deregister(_config, "Live", bank_id,hoid);
        //    return Ok(response);
        //}

        [HttpGet("balanceFetch")]
        public IActionResult balanceFetch(string bank_id,string hoid)
        {
            string response = IciciAPIs.BalanceFetch(_config, "Live", bank_id,hoid);
            return Ok(response);
        }

        //[HttpGet("bankStatement")]
        //public IActionResult BankStatement(IConfiguration _config, string bank_id, int Record)
        //{
        //    string response = "";
        //    response = IciciAPIs.Bankstatment_api_paging(_config, Record, "Live", bank_id);
        //    return Ok(response);
        //}

        //[HttpPost("transaction")]
        //public IActionResult transaction([FromBody] JObject data)
        //{
        //    string response = IciciAPIs.Transactional(_config, data);
        //    return Ok(response);
        //}

        //[HttpGet("trainsactionOtp")]
        //public IActionResult transactionalOtp(string bank_id)
        //{
        //    string response = IciciAPIs.TransactionalOTP(_config, "Uat", bank_id);
        //    return Ok(response);
        //}

        //[HttpGet("createOtp")]
        //public IActionResult createOTP(string bank_id)
        //{
        //    string response = IciciAPIs.CreateOTP(_config, "Live", bank_id);
        //    return Ok(response);
        //}

        //[HttpGet("transactionStatus")]
        //public IActionResult transactionalStatus(string bank_id, string req_id)
        //{
        //    string response = IciciAPIs.TransactionalStatus(_config, "Live", bank_id);
        //    return Ok(response);
        //}


        [HttpGet("encrypt")]
        public IActionResult encrypt(string sampleText)
        {
            string response = CryptoManagerICICI.EncryptUsingCertificate(sampleText, "Live");
            //  string response1 = CryptoManagerICICI.DecryptResponse(response);
            //  string response1 = CryptoManagerICICI.DecryptUsingCertificate(response);

            return Ok(response);
        }
        [HttpPost("encryptpost")]
        public IActionResult encryptpost([FromBody] JObject sampleText)
        {
            string response = CryptoManagerICICI.EncryptUsingCertificate(sampleText.ToString(), "Live");
            //  string response1 = CryptoManagerICICI.DecryptResponse(response);
            //  string response1 = CryptoManagerICICI.DecryptUsingCertificate(response);

            return Ok(response);
        }


        [HttpGet("decrypt")]
        public IActionResult decrypt(string encryptText)
        {
            string response = CryptoManagerICICI.DecryptResponse(encryptText);
            return Ok(response);
        }

        [HttpPost("decryptpost")]
        public IActionResult decryptpost([FromBody] JObject data)
        {
            string response = CryptoManagerICICI.DecryptUsingCertificateRSA(data.SelectToken("data").ToString());

            return Ok(response);
        }

        [HttpGet("ecollection/validation")]
        public IActionResult validation(string encryptText)
        {
            string response = CryptoManagerICICI.DecryptUsingCertificateRSA(encryptText);
            return Ok(response);
        }

        [HttpGet("ecollection/callback")]
        public IActionResult callback(string encryptText)
        {
            string response = CryptoManagerICICI.DecryptUsingCertificateRSA(encryptText);
            return Ok(response);
        }

        [HttpGet("deregister")]
        public IActionResult deregister(string AGGRNAME, string AGGRID, string CORPID, string USERID, string URN)
        {
            string response = IciciAPIs.deregister(AGGRNAME, AGGRID, CORPID, USERID, URN,_config);
            return Ok(response);
        }


        //E_collection
        [Route("ECValidation")]
        [HttpPost]
        public IActionResult ECValidation([FromBody] JObject data)
        {

            JObject response = IciciAPIs.ECValidation(_config, data);

            return Ok(response);

        }

        [Route("MISPosting")]
        [HttpPost]
        public IActionResult MISPosting([FromBody] JObject data)
        {

            JObject response = IciciAPIs.MISPosting(_config, data);

            return Ok(response);

        }

        //upi_collection
        [Route("UPIValidation")]
        [Produces("application/xml")]
        [HttpPost]
        public async Task<IActionResult> UPIValidation()
        {
            int ErrCode = 0;
            string xml;
            string retXml = "<XML>";
            upiValidationRequest reqData;
            XmlDocument xmlDoc = new XmlDocument();
            string ErrMsg = "";
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8))
                {
                    xml = await reader.ReadToEndAsync();
                    xml = xml.ToString().Replace("XML>", "upiValidationRequest>");
                    reqData = Common.Deserialize<upiValidationRequest>(xml);
                    string source = reqData.Source;
                    string Customer_id = reqData.SubscriberId.ToUpper();
                    string TxnId = reqData.TxnId;

                    JObject response = DA.MISAPPDA.UPIValidation(Customer_id, _config);
                    JToken headerToken1 = response;
                    int count = Convert.ToInt32(headerToken1.SelectToken("UPI_details.count").ToString());
                    if (count > 0)
                    {
                        string name = headerToken1.SelectToken("UPI_details.Name").ToString();
                        retXml = retXml + "<CustName>" + name + "</CustName>" + "<ActCode>0</ActCode>";
                        retXml = retXml + "<Message>VALID</Message>" + "<TxnId>" + reqData.TxnId + "</TxnId></XML>";
                    }
                    else
                    {
                        retXml = retXml + "<Message>INVALID</Message>" + "<ActCode>1</ActCode></XML>";
                    }
                }
                
                xmlDoc.LoadXml(retXml);
            }

            catch (Exception ex)
            {
                ErrCode = 100;
                ErrMsg = "Unable to proceed - webserve";
                retXml = retXml + "<ErrCode>"+ ErrCode + "</ErrCode>" + "<ErrMsg>"+ ErrMsg +"</ErrMsg></XML>";
                xmlDoc.LoadXml(retXml);
            }
            return Ok(xmlDoc);

        }

        [Route("UPIMISPosting")]
        [HttpPost]
        public async Task<IActionResult> UPIMISPosting()
        {
            string reqEncData = "";
            string response = "";
            string ErrMsg = "";
            int ErrCode = 0; 
            JObject response1 = new JObject();
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8))
                {
                    reqEncData = await reader.ReadToEndAsync();

                    response = CryptoManagerICICI.DecryptUsingCertificateRSA(reqEncData);
                    JObject decrypted_data = JObject.Parse(response);
                    JToken headerToken = decrypted_data;
                    Common.write_log_Success("incoming upi.aspx | iciciReq Method Response |", "Final data Status 1 : " + decrypted_data);

                    string PayerName = headerToken.SelectToken("PayerName").ToString();
                    string PayerAmount  = headerToken.SelectToken("PayerAmount").ToString();
                    string PayerVA  = headerToken.SelectToken("PayerVA").ToString();
                    string BankRRN = headerToken.SelectToken("BankRRN").ToString();  
                    string merchantTranId = (headerToken.SelectToken("merchantTranId").ToString()).ToUpper();

                    string input_string = decrypted_data.ToString();
                    Common.write_log_Success("incoming upi.aspx | iciciReq Method Response |", "Final data Status 2 : " + PayerName+PayerAmount+PayerVA+BankRRN+merchantTranId);
                    JObject data_response = DA.MISAPPDA.upi_incoming_data_insert(BankRRN,PayerName,PayerVA,merchantTranId,input_string,PayerAmount,_config);

                    string data = data_response.ToString();

                    int Err_Code = Convert.ToInt32(data_response.SelectToken("upi_response.ErrCode").ToString());
                    Common.write_log_Success("incoming upi.aspx | iciciReq Method Response |", "upi errorCode 3 : " + Err_Code);
                    if (Err_Code == 150)
                    {
                        string Reqid = data_response.SelectToken("upi_response.Reqid").ToString();
                        JObject Upi_trans = DA.MISAPPDA.upi_incoming_trans(Reqid, merchantTranId, PayerAmount, _config);
                        Common.write_log_Success("incoming upi.aspx | iciciReq Method Response |", "Final data Status 4 : " + Upi_trans);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Common.write_log_error(" icicilive_c.aspx | iciciReq Method Response |", "UPI income failed : " + ex);
                ErrCode = 100;
                ErrMsg = "Unable to proceed - webserve";
                response1 = new JObject(new JProperty("ErrCode", ErrCode),
                    new JProperty("ErrMsg", ErrMsg));
            }
            return Ok(response);

        }

    }
}
