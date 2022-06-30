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

    [Route("MIS")]
    [ApiController]

    public class MISAPPController : ControllerBase
    {
        private readonly IConfiguration _config;
        public MISAPPController(IConfiguration configuration)
        {
            _config = configuration;

        }
        [HttpGet("encryptDecrypt")]
        public IActionResult encryptDecrypt(string sampleText)
        {
            string response = MisAppAPIs.encryptDecrypt(_config, sampleText);

            return Ok(response);
        }

        [HttpGet("Decrypt")]
        public IActionResult decryptedData(string sampleText)
        {
            string response = MisAppAPIs.decryptedData(_config, sampleText);

            return Ok(response);
        }

        [HttpGet("Encrypt")]
        public IActionResult encryptedData(string sampleText)
        {
            string response = MisAppAPIs.encryptedData(_config, sampleText);

            return Ok(response);
        }

        [HttpPost("encrypttest")]
        public IActionResult encryptTest([FromBody] JObject sampleText)
        {
            string response = MisAppAPIs.encryptedData(_config, sampleText.ToString());

            return Ok(response);
        }

        [HttpPost("decrypttest")]
        public IActionResult decrypttest([FromBody] JObject data)
        {

            string response = MisAppAPIs.Decrypttest(_config, data);

            return Ok(response);
        }

        [Route("check-data")]
        [HttpPost]
        public IActionResult checkData([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.checkData(_config, data);
            return Ok(response);
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Login(_config, data);

            return Ok(response);
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Register(_config, data);

            return Ok(response);
        }

        [Route("verifyotp")]
        [HttpPost]
        public IActionResult otp_verification([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.verifyotp(_config, data);

            return Ok(response);
        }

        [Route("resendotp")]
        [HttpPost]
        public IActionResult resend_otp([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.resendotp(_config, data);

            return Ok(response);
        }

        [Route("LoginPin")]
        [HttpPost]
        public IActionResult Login_pin([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.LoginPin(_config, data);

            return Ok(response);
        }

        [Route("Reset_pin")]
        [HttpPost]
        public IActionResult Reset_pin([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Reset_pin(_config, data);

            return Ok(response);


        }

        [Route("GetAccount")]
        [HttpPost]
        public IActionResult GetAccount([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.GetAccountlist(_config, data);

            return Ok(response);
        }

        [Route("Add_Beneficiary")]
        [HttpPost]
        public IActionResult Add_Beneficiary([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Add_Beneficiary(_config, data);
            return Ok(response);
        }

        [Route("Update_Beneficiary")]
        [HttpPost]
        public IActionResult Update_Beneficiary([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Update_Beneficiary(_config, data);

            return Ok(response);
        }

        [Route("getBeneficiary")]
        [HttpPost]
        public IActionResult getBeneficiary([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.GetBeneficiary_list(_config, data);

            return Ok(response);
        }

        [Route("AllBeneficiary")]
        [HttpPost]
        public IActionResult AllBeneficiary([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.AllBeneficiary_list(_config, data);

            return Ok(response);
        }

        [Route("Remove_Beneficiary")]
        [HttpPost]
        public IActionResult remove_Beneficiary([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Remove_Beneficiary(_config, data);

            return Ok(response);
        }

        [Route("Add_OnBoardSociety")]
        [HttpPost]
        public IActionResult Add_OnBoardSociety([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Add_OnBoardSociety(_config, data);

            return Ok(response);
        }

        [Route("Update_OnBoardSociety")]
        [HttpPost]
        public IActionResult Update_OnBoardSociety([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Update_OnBoardSociety(_config, data);

            return Ok(response);
        }

        [Route("Delete_OnBoardSociety")]
        [HttpPost]
        public IActionResult Remove_OnBoardSociety([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Remove_OnBoardSociety(_config, data);

            return Ok(response);
        }

        [Route("Trans")]
        [HttpPost]
        public IActionResult Tansaction([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Society_NEFT_Transaction(_config, data);

            return Ok(response);
        }

        [Route("Society_Details")]
        [HttpPost]
        public IActionResult Society_Details([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Society_Details(_config, data);

            return Ok(response);
        }

        [Route("Society_NEFT_Transaction")]
        [HttpPost]
        public IActionResult Society_NEFT_Transaction([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Society_NEFT_Transaction(_config, data); //= MisAppAPIs.Society_NEFT_Transaction(_config, data);

            return Ok(response);
        }

        [Route("Send_Sms")]
        [HttpPost]
        public IActionResult Send_Sms([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Send_sms(_config, data); //= MisAppAPIs.Society_NEFT_Transaction(_config, data);

            return Ok(response);
        }  // not working

        [Route("getlist")]
        [HttpPost]
        public IActionResult getlist([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.getPeninglist(_config, data);

            return Ok(response);
        }

        [Route("Sendsms_onreg")]
        [HttpPost]
        public IActionResult Sendsms_onreg(string mobile_no, int otp)
        {

            JObject response = MisAppAPIs.Sendsms_onreg(_config, mobile_no, otp);
            //= MisAppAPIs.Society_NEFT_Transaction(_config, data);

            return Ok(response);
        }

        [Route("Trans_release")]
        [HttpPost]
        public IActionResult Trans_release([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.release_trans(_config, data);

            return Ok(response);


        }

        [HttpGet("trans-status")]
        public IActionResult Trans_status(string socid)
        {

            JObject response = MisAppAPIs.Trans_status(_config, socid);
            return Ok(response);


        }

        [Route("Acc-bal")]
        [HttpPost]
        public IActionResult Acc_bal([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.acc_bal(_config, data);

            return Ok(response);


        }

        [Route("account-mini-statement")]
        [HttpPost]
        public IActionResult account_mini_statement([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.account_mini_statement(_config, data);

            return Ok(response);

        }

        [HttpGet("incoming-trans")]
        public IActionResult incoming_trans(string socid)
        {

            JObject response = MisAppAPIs.incoming_trans(_config, socid);
            return Ok(response);


        }

        [HttpGet("Rejected-trans")]
        public IActionResult Rejected_trans(string socid)
        {

            JObject response = MisAppAPIs.rejected_trans(_config, socid);
            return Ok(response);


        }

        [Route("QRcode")]
        [HttpPost]
        public IActionResult QRcode([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.QRcode(_config, data);

            return Ok(response);

        }
       
        [Route("Set_Limit")]
        [HttpPost]
        public IActionResult Set_Limit([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Setlimit(_config, data);

            return Ok(response);
        }

        [Route("Society_Deatils")]
        [HttpPost]
        public IActionResult Society_Deatils([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.Society_Deatils(_config, data);

            return Ok(response);
        }

        [Route("Loan_Details")]
        [HttpPost]
        public IActionResult Loan_Details([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Loan_Details(_config, data);
            return Ok(response);

        }

        [Route("Depo_Details")]
        [HttpPost]
        public IActionResult Depo_Details([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Depo_Details(_config, data);
            return Ok(response);

        }

        [Route("transfer_limt")]
        [HttpPost]
        public IActionResult transfer_limt([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Transfer_bal(_config, data);
            return Ok(response);

        }

        [Route("upi_transfer")]
        [HttpPost]
        public IActionResult upi_transfer([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.upi_transfer(_config, data);
            return Ok(response);

        }


        [Route("FM_transaction_approval")]
        [HttpPost]
        public IActionResult FM_transaction_approval([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.FM_transaction_approval(_config, data);
            return Ok(response);

        }

        //transaction from ezby
        [Route("Society_Transaction")]
        [HttpPost]
        public IActionResult Society_Transaction([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Society_Transaction(_config, data); 
            return Ok(response);
        }


        //register
        [Route("icici_regis")]
        [HttpPost]
        public IActionResult icici_regis([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.icici_regis(_config, data);
            return Ok(response);
        }
        //register_status
        [Route("icici_regis_status")]
        [HttpPost]
        public IActionResult icici_regis_status([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.icici_regis_status(_config, data);
            return Ok(response);
        }

        //icici_bank_statement
        [Route("icici_bank_statement")]
        [HttpPost]
        public IActionResult icici_bank_statement(string hoid,DateTime fromdate, DateTime todate)
        {
            JObject response = MisAppAPIs.icici_bank_statement(_config, hoid, fromdate, todate);
            return Ok(response);
        }

        //trans_reject
        [Route("trans_reject")]
        [HttpPost]
        public IActionResult trans_reject([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.trans_reject(_config, data);
            return Ok(response);
        }

        //trans_reject
        [Route("trans_approval_limt")]
        [HttpPost]
        public IActionResult trans_approval_limt(string hoid)
        {
            JObject response = MisAppAPIs.trans_approval_limt(_config, hoid);
            return Ok(response);
        }


        [Route("Verify_user")]
        [HttpPost]
        public IActionResult Verify_user([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Verify_user(_config, data);
            return Ok(response);
        }

        //send sms on transaction generate 
        [Route("trans_otp")]
        [HttpPost]
        public IActionResult trans_otp([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.trans_otp(_config, data);

            return Ok(response);
        }

        //send sms on beneficiary generate 
        [Route("ben_otp")]
        [HttpPost]
        public IActionResult Ben_otp([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.beneficiary_otp(_config, data);

            return Ok(response);
        }
        //send sms on limit set generate 
        [Route("limitset_otp")]
        [HttpPost]
        public IActionResult limitset_otp([FromBody] JObject data)
        {

            JObject response = MisAppAPIs.limitset_otp(_config, data);

            return Ok(response);
        }
        //kyc verify
        [Route("KYC_Verify")]
        [HttpPost]
        public IActionResult KYC_Verify([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.KYC_Verify(_config, data);

            return Ok(response);
        }

        //Bank details
        [Route("Bank_details")]
        [HttpPost]
        public IActionResult Bank_details([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.Bank_details(_config, data);

            return Ok(response);
        }

        //ICICI trans status
        [Route("icici_trans_status")]
        [HttpPost]
        public IActionResult icici_trans_status([FromBody] JObject data)
        {
            JObject response = MisAppAPIs.icici_trans_status(_config, data);
            return Ok(response);
        }

        //soc deatils for fm
        [Route("Soc_details_FM")]
        [HttpPost]
        public IActionResult Soc_details(string hoid)
        {
            JObject response = MisAppAPIs.Society_Details_FM(_config, hoid);
            return Ok(response);
        }
       
    }
}
