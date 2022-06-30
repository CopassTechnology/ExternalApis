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
    [Route("api/icici/uat")]
    [ApiController]
    public class IciciController : ControllerBase
    {
       
        private readonly IConfiguration _config;
        
        public IciciController(IConfiguration configuration)
        {
            _config = configuration;
          
        }


        //[HttpGet("register")]
        //public IActionResult register(string bank_id)
        //{
        //    string response = IciciAPIs.Register(_config, "Uat",bank_id);
        //    return Ok(response);
        //}

        //[HttpGet("registerStatus")]
        //public IActionResult registerStatus(string bank_id)
        //{
        //    string response = IciciAPIs.RegisterStatus(_config, "Uat",bank_id);

        //    return Ok(response);
        //}

        //[HttpGet("deregister")]
        //public IActionResult deregister(string bank_id)
        //{
        //    string response = IciciAPIs.deregister(_config, "Uat",bank_id);
        //    return Ok(response);
        //}

        [HttpGet("balanceFetch")]
        public IActionResult balanceFetch(string bank_id,string hoid)
        {
            string response = IciciAPIs.BalanceFetch(_config, "Uat",bank_id,hoid);
            return Ok(response);
        }

        //[HttpGet("bankStatement")]
        //public IActionResult BankStatement(IConfiguration _config, string bank_id,int Record)
        //{
        //    string response = "";
        //    response = IciciAPIs.Bankstatment_api_paging(_config,Record,"Uat",bank_id);
        //    return Ok(response);
        //}

        //[HttpPost("transaction")]
        //public IActionResult transaction([FromBody] JObject data)
        //{
        //    string response = IciciAPIs.Transactional(_config, data);
        //    return Ok(response);
        //}




        //[HttpGet("transactionStatus")]
        //public IActionResult transactionalStatus(string bank_id)
        //{
        //    string response = IciciAPIs.TransactionalStatus(_config, "Uat", bank_id);
        //    return Ok(response);
        //}


        [HttpGet("encrypt")]
        public IActionResult encrypt(string sampleText)
        {
            string response = CryptoManagerICICI.EncryptUsingCertificate(sampleText,"Uat");
            string response1 = CryptoManagerICICI.DecryptResponse(response);
            //string response1 = CryptoManagerICICI.DecryptUsingCertificate(response);
            
            return Ok(response1);
        }

        [HttpGet("decrypt")]
        public IActionResult decrypt(string encryptText)
        {
            string response = CryptoManagerICICI.DecryptUsingCertificateRSA(encryptText);
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
    }
}
