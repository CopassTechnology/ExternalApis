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
    [Route("cbs")]
    [ApiController]
    public class oneStackController : ControllerBase
    {
        private readonly IConfiguration _config;

        public oneStackController(IConfiguration configuration)
        {
            _config = configuration;

        }
        [HttpGet("encryptDecrypt")]
        public IActionResult encryptDecrypt(string sampleText)
        {
            string response = OneStackAPIs.encryptDecrypt(_config,sampleText);
          
            return Ok(response);
        }

        [HttpPost("encrypttest")]
        public IActionResult encryptTest([FromBody] JObject sampleText)
        {
            string response = OneStackAPIs.encryptedData(_config, sampleText.ToString());

            return Ok(response);
        }

        [HttpPost("decrypttest")]
        public IActionResult decrypttest([FromBody] JObject sampleText)
        {
            dynamic obj = sampleText;
            string encryptedData = (string)obj["data"];
            string response = OneStackAPIs.decrypttest(_config, encryptedData);

            return Ok(response);
        }

        //onborading

        [Route("check-user")]
        [HttpPost]
        public IActionResult checkUser([FromBody] JObject data)
        {
            
            JObject response = OneStackAPIs.checkUser(_config, data);
            return Ok(response);
        }


        [Route("new-user")]
        [HttpPost]
        public IActionResult new_user([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.new_user(_config, data);

            return Ok(response);
        }


        [Route("register")]
        [HttpPost]
        public IActionResult register([FromBody] JObject Data)
        {
            
            JObject response = OneStackAPIs.register(_config, Data);

            return Ok(response);
        }

        [Route("authenticate-registeration-otp")]

        [HttpPost]
        public IActionResult auth_registration_otp([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.auth_registration_otp(_config, data);

            return Ok(response);
        }


        //Tokenization

        [Route("update-token")]
        [HttpPost]
        public IActionResult update_token ([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.token(_config, data);

            return Ok(response);
        }

        //Customer Account

        [Route("accounts-discover")]
        [HttpPost]
        public IActionResult accounts_discover ([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.accounts_discover(_config, data);

            return Ok(response);

        }

        [Route("account-balance")]
        [HttpPost]
        public IActionResult account_balance([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.account_balance(_config, data);

            return Ok(response);

        }

        [Route("transactions-list")]
        [HttpPost]
        public IActionResult transactions_list([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.transactions_list(_config, data);

            return Ok(response);

        }


        [Route("account-mini-statement")]
        [HttpPost]
        public IActionResult account_mini_statement([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.account_mini_statement(_config, data);

            return Ok(response);

        }


        //Customer Details KYC , By Noor
        [Route("submit-customer-kyc")]
        [HttpPost]
        public IActionResult submit_customer_kyc([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.sub_customer_kyc(_config, data);

            return Ok(response);
        }
        //
        [Route("get-customer-details")]
        [HttpPost]
        public IActionResult get_customer_details([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.fetch_cust_details(_config, data);

            return Ok(response);

        }

        [Route("initiate-upi")]
        [HttpPost]
        public IActionResult initiateUpi([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.initiateUpi(_config, data);

            return Ok(response);

        }

        [Route("settlement-upi")]
        [HttpPost]
        public IActionResult SettlementUPI([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.SettlementUPI(_config, data);

            return Ok(response);

        }

        [Route("upi-status-check")]
        [HttpPost]
        public IActionResult UPIStatusCheck([FromBody] JObject data)
        {
            JObject response = OneStackAPIs.UPIStatusCheck(_config, data);

            return Ok(response);

        }
    }
}
