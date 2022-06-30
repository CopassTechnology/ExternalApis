using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.BI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ExternalAPIs.Controllers
{
    [Route("CIVS")]
    [ApiController]
    public class CIVSController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CIVSController(IConfiguration configuration)
        {
            _config = configuration;

        }

        [HttpGet("encryptDecrypt")]
        public IActionResult encryptDecrypt(string sampleText)
        {
            string response = CIVSAPIs.encryptDecrypt(_config, sampleText);

            return Ok(response);
        }


        [HttpPost("encryptData")]
        public IActionResult encryptTest([FromBody] JObject sampleText)
        {
            string response = CIVSAPIs.EncryptWithMerchantKey(_config, sampleText);

            return Ok(response);
        }

        [HttpPost("decryptData")]
        public IActionResult decryptInput([FromBody] JObject data)
        {

            string response = CIVSAPIs.DecrpttWithMerchantKey(_config, data);

            return Ok(response);
        }



        [HttpGet("Decrypt")]
        public IActionResult decryptedData(string sampleText)
        {
            string response = CIVSAPIs.decryptedData(_config, sampleText);

            return Ok(response);
        }

        [HttpPost("APIRequest")]
        public IActionResult PayloadRequest([FromBody] JObject data)
        {
            JObject response = CIVSAPIs.Payload_Req(_config, data);

            return Ok(response);
        }

    }
}
