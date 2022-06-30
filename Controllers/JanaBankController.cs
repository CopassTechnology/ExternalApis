using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.Encryption;
using ExternalAPIs.BI;
using ExternalAPIs.BI.HelperClasses;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ExternalAPIs.Controllers
{
    [Route("api/janabank")]
    [ApiController]
    public class JanaBankController : ControllerBase
    {
        private readonly IConfiguration _config;

        public JanaBankController(IConfiguration configuration)
        {
            _config = configuration;

        }


        [HttpGet]
        public IActionResult get()
        {
            return Ok("Service is Running");
        }

    
        [HttpGet("aesEncrypt")]
        public IActionResult aesEncrypt(string sampleText)
        {
            aesEncryptResponse response = JanaBankAPIs.aesEncrypt(sampleText);
            return Ok(response);
        }

        [HttpPost("signVerification")]
        public IActionResult SignVerification([FromBody] signatureVerification data)
        {
            bool retVal = JanaBankAPIs.SignVerification(data);
            return Ok(retVal);
        }

        [HttpPost("aesDecryption")]
        public IActionResult aesDecryption([FromBody] encryptedPaylod data)
        {
            string retVal = JanaBankAPIs.aesDecryption(data);
            return Ok(retVal);
        }

        //--------------------Babita Jana Bank Implementation---------------------------//
        [HttpPost("Jana_IMPS")]
        public IActionResult JanaBankIMPS([FromBody] JObject data)
        {
            //string retVal = (string)JanaBankAPIs.JanaBankIMPS(_config, data);0
            string retVal = (JanaBankAPIs.JanaBankIMPS(_config, data)).ToString();

            return Ok(retVal);
        }

        [HttpPost("Jana_ReqAPI")]
        public IActionResult JanaBankRequestAPI([FromBody] JObject data)
        {
            string retVal = (JanaBankAPIs.JanaBankReqAPI(_config, data)).ToString();

            return Ok(retVal);
        }
    }


}
