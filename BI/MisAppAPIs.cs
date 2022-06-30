using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.DA;
using Newtonsoft.Json;
using ExternalAPIs.BI;
using System.Net;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web;

using System.Runtime.InteropServices;
using ExternalAPIs.Helper;

namespace ExternalAPIs.BI
{
    public class MisAppAPIs
    {
        public static string getApiKey(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["MISAPIKey"];
        }
        public static string encryptDecrypt(IConfiguration _config, string sampleText)
        {
            string response1 = "";
            int ErrCode = 0;
            string ErrMsg = "";
            try
            {
                string response = CryptoManagerMISAPP.EncryptString(getApiKey(_config), sampleText);
                response1 = CryptoManagerMISAPP.DecryptString(getApiKey(_config), response);
                //string response1 = CryptoManagerICICI.DecryptUsingCertificate(response);
            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
            }
            return response1;
        }
        public static string decryptedData(IConfiguration _config, string sampleText)

        {
            string decryptedData = "";
            int ErrCode = 0;
            string ErrMsg = "";
            try
            {
                // JToken headerToken   = data;
                //string sampletext = CryptoManagerMISAPP.DecryptString(getApiKey(_config), headerToken.SelectToken("params.sampleText").ToString()) ;

                decryptedData = CryptoManagerMISAPP.DecryptString(getApiKey(_config), sampleText);
            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
            }
            return decryptedData;
        }

        public static string encryptedData(IConfiguration _config, string sampleText)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string encryptedData = "";
            try
            {
                encryptedData = CryptoManagerMISAPP.EncryptString(getApiKey(_config), sampleText);
            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
            }
            return encryptedData;
        }
        public static JObject checkData(IConfiguration _config, JObject data)
        {

            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
            int ErrCode = 0;
            string ErrMsg = "";
            JObject responseObj = new JObject();
            try
            {
                // DataBase Method for checking  user exists in database "Y/N" Start

                JObject databaseoutput = MISAPPDA.getData(data);

                //Database output converted into  Jproperty[Data] fromat end

                //
                string paramdata = (string)databaseoutput["param"];
                string encryptedres = CryptoManagerMISAPP.EncryptString(getApiKey(_config), paramdata);
                //Onestack final output format
                responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString()),
                new JProperty("txnid", obj["txnid"].ToString()),
                new JProperty("registration_id", obj["registration_id"].ToString()),
                new JProperty("status", databaseoutput["status"].ToString()),
                new JProperty("data", encryptedres)
            );
            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
            }

            return responseObj;
        }
        public static string Decrypttest(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string decryptedData = "";
            try
            {
                dynamic obj = data;
                string encryptedData = (string)obj["data"];
                decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);

            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
            }
            return decryptedData;
        }
        //public static JObject handleErrorData(string outprint, int ErrCode, string ErrMsg, IConfiguration _config)
        //{
        //    int Errcode = 500;
        //    string Errmsg = "failed";
        //    JObject Outputdata = new JObject(new JProperty("ErrCode", ErrCode),
        //        new JProperty("ErrMsg", ErrMsg));

        //    string strdata = Outputdata.ToString();
        //    string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
        //    Outputdata = new JObject(new JProperty(outprint, encryptoutput));
        //    return Outputdata;
        //}
        public static JObject handleErrorData_1(string outprint, IConfiguration _config)
        {
            int Errcode = 500;
            string Errmsg = "Network busy, Please try later ["+ Errcode +"]";
            JObject Outputdata = new JObject(new JProperty("ErrCode", Errcode),
                new JProperty("ErrMsg", Errmsg));

            string strdata = Outputdata.ToString();
            string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
            Outputdata = new JObject(new JProperty(outprint, encryptoutput));
            return Outputdata;
        }
        //login process
        public static JObject Login(IConfiguration _config, JObject logininput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            string encryptedData = "";
            string decryptedData = "";
            string mobile_no = "";
            string Password = "";
            string HOID = "";
            string pin_data = "";
            string strdata = "";
            string encryptoutput = "";
            JObject jdata = new JObject();
            try
            {
                dynamic obj = logininput;
                encryptedData = (string)obj["logininput"];
                decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                mobile_no = headerToken.SelectToken("logindetails.mobile_no").ToString();
                Password = headerToken.SelectToken("logindetails.password").ToString();
                HOID = headerToken.SelectToken("logindetails.socid").ToString();
                pin_data = encr.EncryptStringAES(Password);             
                responseObj = DA.MISAPPDA.MobileUserLogin(mobile_no, pin_data, HOID , _config);                        
                strdata = responseObj.ToString();                
                encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("loginoutput", encryptoutput));
            }
            catch (Exception ex)
            {
                Common.write_log_error("Login api | Login process |", "API Status : " + ex + logininput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Register(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            string encryptedData ="";
            string decryptedData = "";
            string mobile_no = "";
            string IMEI_NO = "";
            string Society_id = "";
            JObject responseObj = new JObject();
            int otp = 0;
            JObject smsoutput = new JObject();
            string strdata = "";
            string encryptoutput = "";
            JObject jdata = new JObject();
            try
            {
                dynamic obj = reginput;
                encryptedData = (string)obj["reginput"];
                decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                mobile_no = headerToken.SelectToken("register.mobile_no").ToString();
                IMEI_NO = headerToken.SelectToken("register.IMEI_NO").ToString();
                Society_id = headerToken.SelectToken("register.Society_id").ToString();
                responseObj = DA.MISAPPDA.MobileUserRegister(mobile_no, IMEI_NO, Society_id, _config);
                JObject headertoken = responseObj;
                otp = Convert.ToInt32(headertoken.SelectToken("register_response.OTP").ToString());
                if (otp > 0)
                {
                    smsoutput = MisAppAPIs.Sendsms_onreg(_config, mobile_no, otp);
                }
                strdata = responseObj.ToString();
                encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("regoutput", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Register api | Register process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject verifyotp(IConfiguration _config, JObject verifyotpinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = verifyotpinput;
                string encryptedData = (string)obj["otpinput"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("otp.mobile_no").ToString();
                string otp = headerToken.SelectToken("otp.OTP").ToString();                
                string socid = headerToken.SelectToken("otp.socid").ToString();                
                JObject responseObj = DA.MISAPPDA.Mobileverifyotp(mobile_no, otp,socid, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("otpoutput", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex + verifyotpinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject resendotp(IConfiguration _config, JObject resendotpinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = resendotpinput;
                string encryptedData = (string)obj["resendotp_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("resendotp.mobile_no").ToString();
                string hoid = headerToken.SelectToken("resendotp.socid").ToString();
                JObject responseObj = DA.MISAPPDA.Resend_OTP(mobile_no,hoid, _config);
                JObject headertoken = responseObj;
                int otp = Convert.ToInt32(headertoken.SelectToken("resendotp_response.OTP").ToString());
                JObject smsoutput = MisAppAPIs.Sendsms_onreg(_config, mobile_no, otp);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("resendotp_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("resendotp api | resendotp process |", "API Status : " + ex + resendotpinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject LoginPin(IConfiguration _config, JObject LoginPininput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = LoginPininput;
                string encryptedData = (string)obj["Logpin_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Logpin.mobile_no").ToString();
                string pin = headerToken.SelectToken("Logpin.pin").ToString();
                string socid = headerToken.SelectToken("Logpin.socid").ToString();
                string pin_data = encr.EncryptStringAES(pin);
                
                JObject responseObj = DA.MISAPPDA.loG_PIN(mobile_no, pin_data,socid, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Logpin_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("LoginPin api | LoginPin process |", "API Status : " + ex + LoginPininput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Reset_pin(IConfiguration _config, JObject Resetpin)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = Resetpin;
                string encryptedData = (string)obj["Resetpin"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Reset_pin.mobile_no").ToString();
                string password = headerToken.SelectToken("Reset_pin.password").ToString();
                string socid = headerToken.SelectToken("Reset_pin.socid").ToString();
                string pin_data = encr.EncryptStringAES(password);
                JObject responseObj = DA.MISAPPDA.Reset_pin(mobile_no, pin_data,socid, _config);
                // dynamic objout = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Resetoutput", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Reset_pin api | Reset_pin process |", "API Status : " + ex + Resetpin);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Verify_user(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["userinput"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("user.mobile_no").ToString();
                string socid = headerToken.SelectToken("user.socid").ToString();
                JObject responseObj = DA.MISAPPDA.MobileUserverify(mobile_no,socid,  _config);
                JObject headertoken = responseObj;
                int OTP = Convert.ToInt32(headertoken.SelectToken("user_response.OTP").ToString());
                if (OTP > 0)
                {
                    JObject smsoutput = MisAppAPIs.Sendsms_onreg(_config, mobile_no, OTP);
                }
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("useroutput", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Verify_user api | Verify_user process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //account list
        public static JObject GetAccountlist(IConfiguration _config, JObject account)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = account;
                string encryptedData = (string)obj["Account_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Accountlist.mobile_no").ToString();
                string society_id = headerToken.SelectToken("Accountlist.society_id").ToString();
                JObject responseObj = DA.MISAPPDA.getaccount_list(mobile_no, society_id, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Account_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("GetAccountlist api | GetAccountlist process |", "API Status : " + account);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //beneficiary process
        public static JObject Add_Beneficiary(IConfiguration _config, JObject beni_data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = beni_data;
                string encryptedData = (string)obj["Beneficiary_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);

                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Beneficiary.mobile_no").ToString();
                string hoid = headerToken.SelectToken("Beneficiary.hoid").ToString();
                string Bid = headerToken.SelectToken("Beneficiary.BID").ToString();
                string acc_no = headerToken.SelectToken("Beneficiary.accno").ToString();
                string acc_type = headerToken.SelectToken("Beneficiary.acctype").ToString();
                string own_neft = headerToken.SelectToken("Beneficiary.ownneft").ToString();
                string account_name = headerToken.SelectToken("Beneficiary.accname").ToString();
                string account_nickname = headerToken.SelectToken("Beneficiary.nickname").ToString();
                string bankname = headerToken.SelectToken("Beneficiary.bankname").ToString();
                string bankbranch = headerToken.SelectToken("Beneficiary.branchname").ToString();
                string ifsccode = headerToken.SelectToken("Beneficiary.ifsccode").ToString();
                JObject responseObj = DA.MISAPPDA.Add_Beneficiary(mobile_no, hoid, Bid, acc_no, acc_type, own_neft, account_name, account_nickname, bankname, bankbranch, ifsccode, _config);
                // dynamic objout = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Beneficiary_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Add_Beneficiary api | Add_Beneficiary process |", "API Status : " + ex + beni_data);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Update_Beneficiary(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["Beneficiary_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Beneficiary.mobile_no").ToString();
                string hoid = headerToken.SelectToken("Beneficiary.hoid").ToString();
                string Bid = headerToken.SelectToken("Beneficiary.BID").ToString();
                string acc_no = headerToken.SelectToken("Beneficiary.accno").ToString();
                string acc_type = headerToken.SelectToken("Beneficiary.acctype").ToString();
                string own_neft = headerToken.SelectToken("Beneficiary.ownneft").ToString();
                string account_name = headerToken.SelectToken("Beneficiary.accname").ToString();
                string account_nickname = headerToken.SelectToken("Beneficiary.nickname").ToString();
                string bankname = headerToken.SelectToken("Beneficiary.bankname").ToString();
                string bankbranch = headerToken.SelectToken("Beneficiary.branchname").ToString();
                string ifsccode = headerToken.SelectToken("Beneficiary.ifsccode").ToString();
                JObject responseObj = DA.MISAPPDA.Update_Beneficiary(mobile_no, hoid, Bid, acc_no, own_neft, account_name, account_nickname, bankname, bankbranch, ifsccode, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Beneficiary_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Update_Beneficiary api | Update_Beneficiary process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Remove_Beneficiary(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["Beneficiary_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Beneficiary.mobile_no").ToString();
                string hoid = headerToken.SelectToken("Beneficiary.hoid").ToString();
                string Bid = headerToken.SelectToken("Beneficiary.BID").ToString();
                string acc_no = headerToken.SelectToken("Beneficiary.acc_no").ToString();
                string acc_type = headerToken.SelectToken("Beneficiary.acctype").ToString();
                string account_name = headerToken.SelectToken("Beneficiary.accname").ToString();
                string account_nickname = headerToken.SelectToken("Beneficiary.nickname").ToString();
                string bankname = headerToken.SelectToken("Beneficiary.bankname").ToString();
                string bankbranch = headerToken.SelectToken("Beneficiary.bankbranch").ToString();
                string ifsccode = headerToken.SelectToken("Beneficiary.ifsccode").ToString();
                JObject responseObj = DA.MISAPPDA.Remove_Beneficiary(mobile_no, hoid, Bid, acc_no, acc_type, account_name, account_nickname, bankname, bankbranch, ifsccode, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Beneficiary_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Remove_Beneficiary api | Remove_Beneficiary process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject GetBeneficiary_list(IConfiguration _config, JObject account)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = account;
                string encryptedData = (string)obj["Beneficiary_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Beneficiary.mobile_no").ToString();
                string socid = headerToken.SelectToken("Beneficiary.socid").ToString();
                JObject responseObj = DA.MISAPPDA.getBeneficiary_list(mobile_no,socid, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Beneficiary_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("GetBeneficiary_list api | GetBeneficiary_list process |", "API Status : " + ex + account);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject AllBeneficiary_list(IConfiguration _config, JObject account)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = account;
                string encryptedData = (string)obj["Beneficiary_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Beneficiary.mobile_no").ToString();
                string socid = headerToken.SelectToken("Beneficiary.socid").ToString();
                JObject responseObj = DA.MISAPPDA.allBeneficiary_list(mobile_no,socid, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Beneficiary_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("AllBeneficiary_list api | AllBeneficiary_list process |", "API Status : " + ex +account);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //society register
        public static JObject Add_OnBoardSociety(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["Society_add_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string hoid = headerToken.SelectToken("Society.hoid").ToString();
                string icici_ACC = headerToken.SelectToken("Society.icici_ACC").ToString();
                string icici_ifsc = headerToken.SelectToken("Society.icici_ifsc").ToString();
                string icici_branch = headerToken.SelectToken("Society.icici_branch").ToString();
                string bankid = headerToken.SelectToken("Society.bankid").ToString();
                string approvalamt = headerToken.SelectToken("Society.approvalamt").ToString();
                string acc_prefix = headerToken.SelectToken("Society.acc_prefix").ToString();
                string beneficiary_limit = headerToken.SelectToken("Society.beneficiary_limit").ToString();
                string upi_merchant_id = headerToken.SelectToken("Society.upi_merchant_id").ToString();
                string bankglcode = headerToken.SelectToken("Society.bankglcode").ToString();
                string bankacno = headerToken.SelectToken("Society.bankacno").ToString();
                string bankchrgincomeglcode = headerToken.SelectToken("Society.bankchrgincomeglcode").ToString();
                string bankchrgincomeacno = headerToken.SelectToken("Society.bankchrgincomeacno").ToString();
                string bankchrgexpenseglcode = headerToken.SelectToken("Society.bankchrgexpenseglcode").ToString();
                string bankchrgexpenseacno = headerToken.SelectToken("Society.bankchrgexpenseacno").ToString();
                string user_level = headerToken.SelectToken("Society.user_level").ToString();
                string approve_limit = headerToken.SelectToken("Society.approve_limit").ToString();
                string TRANS_PREFIX = headerToken.SelectToken("Society.TRANS_PREFIX").ToString();
                string DAILYTRANSLIMIT = headerToken.SelectToken("Society.DAILYTRANSLIMIT").ToString();

                JObject responseObj = DA.MISAPPDA.Add_Society(hoid, icici_ACC, icici_ifsc, icici_branch, bankid, approvalamt, acc_prefix, beneficiary_limit, upi_merchant_id, bankglcode, bankacno, bankchrgincomeglcode, bankchrgincomeacno, bankchrgexpenseglcode, bankchrgexpenseacno, user_level, approve_limit, TRANS_PREFIX, DAILYTRANSLIMIT, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Society_add_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Add_OnBoardSociety api | Add_OnBoardSociety process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Update_OnBoardSociety(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["Society_update_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string hoid = headerToken.SelectToken("Society.hoid").ToString();
                string icici_ACC = headerToken.SelectToken("Society.icici_ACC").ToString();
                string icici_ifsc = headerToken.SelectToken("Society.icici_ifsc").ToString();
                string icici_branch = headerToken.SelectToken("Society.icici_branch").ToString();
                string bankid = headerToken.SelectToken("Society.bankid").ToString();
                string approvalamt = headerToken.SelectToken("Society.approvalamt").ToString();
                string acc_prefix = headerToken.SelectToken("Society.acc_prefix").ToString();
                string beneficiary_limit = headerToken.SelectToken("Society.beneficiary_limit").ToString();
                string upi_merchant_id = headerToken.SelectToken("Society.upi_merchant_id").ToString();
                string bankglcode = headerToken.SelectToken("Society.bankglcode").ToString();
                string bankacno = headerToken.SelectToken("Society.bankacno").ToString();
                string bankchrgincomeglcode = headerToken.SelectToken("Society.bankchrgincomeglcode").ToString();
                string bankchrgincomeacno = headerToken.SelectToken("Society.bankchrgincomeacno").ToString();
                string bankchrgexpenseglcode = headerToken.SelectToken("Society.bankchrgexpenseglcode").ToString();
                string bankchrgexpenseacno = headerToken.SelectToken("Society.bankchrgexpenseacno").ToString();
                string user_level = headerToken.SelectToken("Society.user_level").ToString();
                string approve_limit = headerToken.SelectToken("Society.approve_limit").ToString();
                string TRANS_PREFIX = headerToken.SelectToken("Society.TRANS_PREFIX").ToString();
                string DAILYTRANSLIMIT = headerToken.SelectToken("Society.DAILYTRANSLIMIT").ToString();

                JObject responseObj = DA.MISAPPDA.Update_Society(hoid, icici_ACC, icici_ifsc, icici_branch, bankid, approvalamt, acc_prefix, beneficiary_limit, upi_merchant_id, bankglcode, bankacno, bankchrgincomeglcode, bankchrgincomeacno, bankchrgexpenseglcode, bankchrgexpenseacno, user_level, approve_limit, TRANS_PREFIX, DAILYTRANSLIMIT, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Society_update_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Update_OnBoardSociety api | Update_OnBoardSociety process |", "API Status : " + ex + reginput);
                
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Remove_OnBoardSociety(IConfiguration _config, JObject reginput)
        {
            dynamic obj = reginput;
            string encryptedData = (string)obj["Society_delete_input"];
            string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;
            string hoid = headerToken.SelectToken("Society.hoid").ToString();
            string icici_ACC = headerToken.SelectToken("Society.icici_ACC").ToString();
            string icici_ifsc = headerToken.SelectToken("Society.icici_ifsc").ToString();
            string icici_branch = headerToken.SelectToken("Society.icici_branch").ToString();
            string bankid = headerToken.SelectToken("Society.bankid").ToString();
            string approvalamt = headerToken.SelectToken("Society.approvalamt").ToString();
            string acc_prefix = headerToken.SelectToken("Society.acc_prefix").ToString();
            string beneficiary_limit = headerToken.SelectToken("Society.beneficiary_limit").ToString();
            string upi_merchant_id = headerToken.SelectToken("Society.upi_merchant_id").ToString();
            string bankglcode = headerToken.SelectToken("Society.bankglcode").ToString();
            string bankacno = headerToken.SelectToken("Society.bankacno").ToString();
            string bankchrgincomeglcode = headerToken.SelectToken("Society.bankchrgincomeglcode").ToString();
            string bankchrgincomeacno = headerToken.SelectToken("Society.bankchrgincomeacno").ToString();
            string bankchrgexpenseglcode = headerToken.SelectToken("Society.bankchrgexpenseglcode").ToString();
            string bankchrgexpenseacno = headerToken.SelectToken("Society.bankchrgexpenseacno").ToString();
            string user_level = headerToken.SelectToken("Society.user_level").ToString();
            string approve_limit = headerToken.SelectToken("Society.approve_limit").ToString();
            string TRANS_PREFIX = headerToken.SelectToken("Society.TRANS_PREFIX").ToString();
            string DAILYTRANSLIMIT = headerToken.SelectToken("Society.DAILYTRANSLIMIT").ToString();

            JObject responseObj = DA.MISAPPDA.Delete_Society(hoid, icici_ACC, icici_ifsc, icici_branch, bankid, approvalamt, acc_prefix, beneficiary_limit, upi_merchant_id, bankglcode, bankacno, bankchrgincomeglcode, bankchrgincomeacno, bankchrgexpenseglcode, bankchrgexpenseacno, user_level, approve_limit, TRANS_PREFIX, DAILYTRANSLIMIT, _config);
            string strdata = responseObj.ToString();
            string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
            JObject Outputdata = new JObject(new JProperty("Society_delete_output", encryptoutput));
            return Outputdata;
            Common.write_log_error("Remove_OnBoardSociety api | Remove_OnBoardSociety process |", "API Status : "  + reginput);
        }
        //public static JObject Tansaction(IConfiguration _config, JObject reginput)
        //{
        //    dynamic obj = reginput;
        //    string encryptedData = (string)obj["Trans_input"];
        //    string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
        //    JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
        //    JToken headerToken = json;
        //    string mobile_no = headerToken.SelectToken("Trans.mobile_no").ToString();
        //    string hoid = headerToken.SelectToken("Trans.hoid").ToString();
        //    string icici_ACC = headerToken.SelectToken("Trans.icici_ACC").ToString();
        //    string acctype = headerToken.SelectToken("Trans.acctype").ToString();
        //    string ownneft = headerToken.SelectToken("Trans.ownneft").ToString();



        //    JObject responseObj = DA.MISAPPDA.Trans(mobile_no,hoid, icici_ACC, acctype, ownneft, _config);

        //    // dynamic objout = responseObj;
        //    string strdata = responseObj.ToString();
        //    string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
        //    JObject Outputdata = new JObject(new JProperty("Trans_output", encryptoutput));
        //    return Outputdata;
        //}
        public static JObject Society_Details(IConfiguration _config, JObject societyinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = societyinput;
                string encryptedData = (string)obj["Society_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string Soc_id = headerToken.SelectToken("Society.Soc_id").ToString();
                JObject responseObj = DA.MISAPPDA.Society_Details(Soc_id, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Society_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Society_Details api | Society_Details process |", "API Status : " + ex + societyinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //list for account balance,pending trans,mini_statement
        public static JObject getPeninglist(IConfiguration _config, JObject listinput)
        {

            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = listinput;
                string encryptedData = (string)obj["PendingList_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string list = headerToken.SelectToken("PendingList.status").ToString();
                string socid = headerToken.SelectToken("PendingList.socid").ToString();
                JObject responseObj = DA.MISAPPDA.getPendingList(list, socid, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("PendingList_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("getPeninglist api | getPeninglist process |", "API Status : " + ex + listinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject acc_bal(IConfiguration _config, JObject listinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = listinput;
                string encryptedData = (string)obj["Acc_no_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string acc_no = headerToken.SelectToken("Accno_input.acc_no").ToString();
                JObject responseObj = DA.MISAPPDA.getAcc_bal(acc_no, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Acc_no_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("acc_bal api | acc_bal process |", "API Status : " + ex + listinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject account_mini_statement(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject responseObj = new JObject();
            string otp;
            string errcode = "";
            string errmsg = "";
            string msg = "";
            JObject Outputdata = new JObject();
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
            //string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;

                string customer = headerToken.SelectToken("Account_list").ToString();
                JObject Jcustobj = JObject.Parse(customer);
                JToken jToken = Jcustobj;

                string account_no = jToken.SelectToken("account_no").ToString();

                JObject databaseoutput = MISAPPDA.mini_statment(account_no, _config);

                string strdata = databaseoutput.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
               Outputdata = new JObject(new JProperty("transaction_data", encryptoutput));
            }
            catch (Exception ex)
            {
                Common.write_log_error("account_mini_statement api | account_mini_statement process |", "API Status : " + ex + data);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject Trans_status(IConfiguration _config, string socid)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject trans_data = new JObject();
            try
            {
                trans_data = DA.MISAPPDA.Trans_status(socid, _config);
            }

            catch (Exception ex)
            {
                Common.write_log_error("Trans_status api | Trans_status process |", "API Status : " + ex + socid);
                trans_data = handleErrorData_1("loginoutput", _config);
            }
            return trans_data;
        }
        //set limit
        public static JObject Setlimit(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["limitinput"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("limit.mobile_no").ToString();
                string Society_id = headerToken.SelectToken("limit.Society_id").ToString();
                string limit = headerToken.SelectToken("limit.limit").ToString();
                JObject responseObj = DA.MISAPPDA.Setlimit(mobile_no, Society_id, limit, _config);
                JObject headertoken = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("limitoutput", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Setlimit api | Setlimit process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //Get society details
        public static JObject Society_Deatils(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["Societyinput"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string Society_id = headerToken.SelectToken("Soc_details.Society_id").ToString();
                JObject responseObj = DA.MISAPPDA.Society_Deatils(Society_id, _config);
                JObject headertoken = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Societyoutput", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Society_Deatils api | Society_Deatils process |", "API Status : " + ex + reginput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //transferable amt
        public static JObject Transfer_bal(IConfiguration _config, JObject checklimit)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = checklimit;
                string encryptedData = (string)obj["checklimit"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("check_limit.mobile_no").ToString();
                string sender_acc = headerToken.SelectToken("check_limit.sender_acc").ToString();
                string bene_acc = headerToken.SelectToken("check_limit.bene_acc").ToString();
                string socid = headerToken.SelectToken("check_limit.socid").ToString();

                JObject response = MISAPPDA.Transfer_bal(mobile_no, bene_acc, sender_acc,socid, _config);
                string strdata = response.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Transfer_limt", encryptoutput));
            }
            catch (Exception ex)
            {
                Common.write_log_error("Transfer_bal api | Transfer_bal process |", "API Status : " + ex + checklimit);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //sms process
        public static JObject Send_sms(IConfiguration _config, JObject transinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = transinput;
                string encryptedData = (string)obj["SMS_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("SMS.mobile_no").ToString();
                string OTP = headerToken.SelectToken("SMS.OTP").ToString();
                string message = "Your Transaction OTP" + OTP;
                JObject responseObj = DA.MISAPPDA.Send_sms(mobile_no, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("SMS_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Send_sms api | Send_sms process |", "API Status : " + transinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        } // not working
        public static JObject Sendsms_onreg(IConfiguration _config, string mobile_no, int otp)
        {
            int ErrCode1 = 0;
            string ErrMsg1 = "";
            JObject Outputdata = new JObject();
            try
            {
                string sms = "" + otp + " is your Mobile login OTP. Do not share it with anyone. Regards CoPASS.";
                string smscount = "1";
                string amt = "1";
                string brid = "1";
                string ErrCode = "";
                string ErrMsg = "";
                JObject Outputdata1 = new JObject();
                string SMSURL = "http://198.15.103.106/API/pushsms.aspx?loginID=copass&password=654321&mobile=" + mobile_no + "&text=" + sms + "&senderid=COPASS&route_id=1&Unicode=0&Template_id=1007016939687274204";
                string responsesms2 = callurl(SMSURL);
                var get_responsesms2 = responsesms2;
                JObject jobject = JObject.Parse(get_responsesms2);
                String Transaction_ID = jobject["Transaction_ID"].ToString();
                String MsgStatus = jobject["MsgStatus"].ToString();
                JObject msginput = DA.MISAPPDA.SendSMS_onreg(brid, "1007016939687274204", smscount, MsgStatus, Transaction_ID, mobile_no, amt, _config);
                string strdata = msginput.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("SMS_output", encryptoutput));
                Outputdata1 = Outputdata;
            }

            catch (Exception ex)
            {
                Common.write_log_error("Sendsms_onreg api | Sendsms_onreg process |", "API Status : " + ex + mobile_no + otp);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        public static string callurl(string url)
        {
            string responseString;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                var postData = request;
                request.Method = "POST";
                request.ContentType = "application/json";

                byte[] data = Encoding.ASCII.GetBytes("");

                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }

                var response = (HttpWebResponse)request.GetResponse();

                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;

            }
            catch
            {
                throw;
            }
        }
        //incoming trans process
        public static JObject incoming_trans(IConfiguration _config, string socid)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject trans_data = new JObject();
            try
            {
                trans_data = DA.MISAPPDA.incoming_trans(socid, _config);
            }

            catch (Exception ex)
            {
                Common.write_log_error("incoming_trans api | incoming_trans process |", "API Status : " + ex + socid);
                trans_data = handleErrorData_1("loginoutput", _config);
            }
            return trans_data;
        }
        //rejected trans process
        public static JObject rejected_trans(IConfiguration _config, string socid)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject trans_data = new JObject();
            try
            {
                trans_data = DA.MISAPPDA.rejected_trans(socid, _config);
            }

            catch (Exception ex)
            {
                Common.write_log_error("rejected_trans api | rejected_trans process |", "API Status : " + ex + socid);
                trans_data = handleErrorData_1("loginoutput", _config);
            }
            return trans_data;
        }
        //qr code process
        public static JObject QRcode(IConfiguration _config, JObject Qr_code)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string data = "";
            JObject response = new JObject();
            try
            {
                dynamic obj = Qr_code;
                string encryptedData = (string)obj["qr_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string account_no = headerToken.SelectToken("QRdetails.account_no").ToString();
                string hoid = headerToken.SelectToken("QRdetails.hoid").ToString();
                JObject acc_details = DA.MISAPPDA.account_upi_details(account_no, _config);
                string v_accno = acc_details.SelectToken("account_details.v_accno").ToString();
                string acc_name = acc_details.SelectToken("account_details.accname").ToString();
                string upi = "CPS." + v_accno + "@icici";
                string upi_id = "upi://pay?pa=" + upi + "&pn="+acc_name+"&am=0.00&tr=CPS12345670020&CU=INR&MC=5411";
                //copassuat@icici
                QR_Code_genrator qr = new QR_Code_genrator();




                data = qr.SendRequest(upi_id);
                response = new JObject(new JProperty("UPI", upi),
                    (new JProperty("QR_details", data)));
            }

            catch (Exception ex)
            {
                Common.write_log_error("QRcode api | QRcode process |", "API Status : " + ex + Qr_code);
                response = handleErrorData_1("loginoutput", _config);
            }
            return response;
        }
        //loan deatils
        public static JObject Loan_Details(IConfiguration _config, JObject DepoDet)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = DepoDet;
                string encryptedData = (string)obj["LoanDet"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("LoanDet.mobile_no").ToString();
                string soc_id = headerToken.SelectToken("LoanDet.soc_id").ToString();
                JObject responseObj = DA.MISAPPDA.Loan_Details(mobile_no, soc_id, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("LoanDetout", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Loan_Details api | Loan_Details process |", "API Status : " + ex + DepoDet);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //depo details
        public static JObject Depo_Details(IConfiguration _config, JObject DepoDet)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = DepoDet;
                string encryptedData = (string)obj["DepoDet"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("DepoDet.mobile_no").ToString();
                string soc_id = headerToken.SelectToken("DepoDet.soc_id").ToString();

                JObject responseObj = DA.MISAPPDA.Depo_Details(mobile_no, soc_id, _config);
                // dynamic objout = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("DepoDetout", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Depo_Details api | Depo_Details process |", "API Status : " + ex + DepoDet);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //upi transfer
        //public static JObject upi_transfer(IConfiguration _config, JObject upi_details)
        //{
        //    int ErrCode = 0;
        //    string ErrMsg = "";
        //    JObject Outputdata = new JObject();
        //    JObject response = new JObject();

        //    string v_accno = "";
        //    string cust_internalid = "";
        //    string substr = "";
        //    string upi = "";
        //    string convertedUUID = "";
        //    JObject requestData = new JObject();
        //    string account_provider = "";
        //    string mobile_no = "";
        //    string device_id = "";
        //    string upi_data = "";
        //    string amount   = "";
        //    string remarks = "";
        //    JObject bank_details = new JObject();

        //    try
        //    {
        //        dynamic obj = upi_details;
        //        string encryptedData = (string)obj["Upi_trans_details"];
        //        string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
        //        JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
        //        JToken headerToken = json;

        //        convertedUUID = IsExist(_config);

        //             mobile_no = headerToken.SelectToken("Deatils.mobile_no").ToString();
        //            device_id = headerToken.SelectToken("Deatils.device-id").ToString();
        //            upi_data = headerToken.SelectToken("Deatils.upi_detials").ToString();
        //            amount = headerToken.SelectToken("Deatils.amount").ToString();
        //            remarks = headerToken.SelectToken("Deatils.remarks").ToString();
        //            account_provider = headerToken.SelectToken("Deatils.account-provider").ToString();

        //            response = DA.MISAPPDA.account_upi_details(account_provider, _config);
        //            JToken data = response;
        //            int Errorcode = Convert.ToInt32(data.SelectToken("account_details.ErrCode").ToString());


        //            if (Errorcode == 100)
        //            {
        //                v_accno = data.SelectToken("account_details.v_accno").ToString();
        //                substr = v_accno.Substring(v_accno.Length - 3);
        //                cust_internalid = data.SelectToken("account_details.cust_internalid").ToString();
        //                upi = "copassuat." + v_accno + "@icici";
        //                Uri myUri = new Uri(upi_data);
        //                string payee = HttpUtility.ParseQueryString(myUri.Query).Get("pa");
        //                string payee_name = HttpUtility.ParseQueryString(myUri.Query).Get("pn");

        //                requestData = new JObject(

        //                    new JProperty("mobile", mobile_no),
        //                    new JProperty("device-id", device_id),
        //                    new JProperty("seq-no", convertedUUID),
        //                    new JProperty("account-provider", substr),
        //                    new JProperty("payee-va", payee),
        //                    new JProperty("payer-va", upi),
        //                    new JProperty("profile-id", cust_internalid),
        //                    new JProperty("amount", amount),
        //                    //new JProperty("pre-approved", "P"),
        //                    //new JProperty("use-default-acc", "D"),
        //                    //new JProperty("default-debit", "N"),
        //                    //new JProperty("default-credit", "N"),
        //                    new JProperty("payee-name", payee_name),
        //                    new JProperty("mcc", "6011"),
        //                    new JProperty("merchant-type", "ENTITY"),
        //                    new JProperty("txn-type", "merchantToPersonPay"),
        //                    new JProperty("channel-code", "MICICI"),
        //                    new JProperty("remarks", remarks)
        //                  );

        //                string outward_data = requestData.ToString();
        //             bank_details = DA.MISAPPDA.bank_details("1", _config);
        //            JObject insert_data = DA.MISAPPDA.upi_outgoing_data_insert(mobile_no,device_id,convertedUUID,payee,upi,outward_data,amount,_config);
        //            }


        //        string output_data = IciciAPIs.upi_outward(_config, bank_details, requestData);
        //        Outputdata = new JObject(new JProperty("data", output_data));

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrCode = 500;
        //        ErrMsg = "Unable to proceed - webserve";
        //        Outputdata = handleErrorData("Upi_trans_output", ErrCode, ErrMsg, _config);
        //    }
        //    return Outputdata;
        //}
        //fm release process
        public static JObject upi_transfer(IConfiguration _config, JObject upi_details)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject response = new JObject();

            //string v_accno = "";
            string v_accno = null;
            int? abc = null;
            string cust_internalid = "";
            string substr = "";
            string upi = "";
            string convertedUUID = "";
            JObject requestData = new JObject();
            JObject Final_output = new JObject();
            string account_provider = "";
            string mobile_no = "";
            string device_id = "";
            string upi_data = "";
            string amount = "";
            string remarks = "";
            JObject bank_details = new JObject();
           string success = "";
            string message = "";
            string reqid = "";
            string socid = "";
            try
            {
                dynamic obj = upi_details;
                string encryptedData = (string)obj["Upi_trans_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + decryptedData);
                
                mobile_no = headerToken.SelectToken("Deatils.mobile_no").ToString();
                device_id = headerToken.SelectToken("Deatils.device-id").ToString();
                upi_data = headerToken.SelectToken("Deatils.upi_detials").ToString();
                amount = headerToken.SelectToken("Deatils.amount").ToString();
                remarks = headerToken.SelectToken("Deatils.remarks").ToString();
                socid = headerToken.SelectToken("Deatils.socid").ToString();
                account_provider = headerToken.SelectToken("Deatils.account-provider").ToString();

                //string Balance_fetch = IciciAPIs.BalanceFetch(_config, "Live", "1", socid);
                //JObject bal_fetch = JObject.Parse(BI.Common.validJason(Balance_fetch));
                //string EFFECTIVEBAL = bal_fetch.SelectToken("EFFECTIVEBAL").ToString();
                //Common.write_log_Success("UPIOutward/EFFECTIVEBAL.aspx | iciciReq Method Response |", "Request Data Status  final : " + Balance_fetch);
                //Common.write_log_Success("UPIOutward/EFFECTIVEBAL.aspx | iciciReq Method Response |", "Request Data Status  final : " + EFFECTIVEBAL);
                convertedUUID = IsExist(_config);
                response = DA.MISAPPDA.account_upi_details(account_provider, _config);
                JToken data = response;
                int Errorcode = Convert.ToInt32(data.SelectToken("account_details.ErrCode").ToString());

                Common.write_log_Success("UPIOutward/Errorcode.aspx | iciciReq Method Response |", "Request Data Status  final : " + Errorcode);
                if (Errorcode == 100)
                {
                    v_accno = data.SelectToken("account_details.v_accno").ToString();
                    substr = v_accno.Substring(v_accno.Length - 3);
                    cust_internalid = data.SelectToken("account_details.cust_internalid").ToString();
                    //upi = "copassuat." + v_accno + "@icici";
                    upi ="CPS." + v_accno + "@icici";
                    Uri myUri = new Uri(upi_data);
                    string payee = HttpUtility.ParseQueryString(myUri.Query).Get("pa");
                    string payee_name = HttpUtility.ParseQueryString(myUri.Query).Get("pn");

                    requestData = new JObject(

                        //new JProperty("mobile", mobile_no),
                        new JProperty("mobile", "9920672549"),
                        //new JProperty("device-id", device_id),
                        new JProperty("device-id", "644387644387644387644387"),
                        new JProperty("seq-no", convertedUUID),
                        new JProperty("account-provider", substr),
                        new JProperty("payee-va", payee),
                       //new JProperty("payer-va", upi),
                        new JProperty("payer-va", "copass@icici"),
                       //new JProperty("profile-id", cust_internalid),
                        new JProperty("profile-id", "167767243"),
                        new JProperty("amount", amount),
                        new JProperty("pre-approved", "P"),
                        new JProperty("use-default-acc", "D"),
                        new JProperty("default-debit", "N"),
                        new JProperty("default-credit", "N"),
                        new JProperty("payee-name", payee_name),
                        new JProperty("mcc", "6011"),
                        new JProperty("merchant-type", "ENTITY"),
                        new JProperty("txn-type", "merchantToPersonPay"),
                        new JProperty("channel-code", "MICICI"),
                        new JProperty("remarks", remarks)
                      );
                    string outward_data = requestData.ToString();
                    Common.write_log_Success("UPIOutward/Errorcode.aspx | iciciReq Method Response |", "Request Data Status  final : " + outward_data);
                    JObject insert_data = DA.MISAPPDA.upi_outgoing_data_insert(mobile_no, device_id, convertedUUID, payee, upi, outward_data, amount,socid, _config);
                    reqid = insert_data.SelectToken("upi_response.Reqid").ToString();
                    Common.write_log_Success("UPIOutward/reqid.aspx | iciciReq Method Response |", "Request Data Status  final : " + reqid);
                    Common.write_log_Success("UPIOutward/insert_data.aspx | iciciReq Method Response |", "Request Data Status  final : " + insert_data);
                    if (reqid != "0")
                    {
                        string req_id = insert_data.SelectToken("upi_response.Reqid").ToString();
                        JObject finaldata = DA.MISAPPDA.upi_outgoing_trans(req_id, account_provider, amount, remarks, _config);


                        string keyForICICIForEncryption = CryptoManagerICICI.RandomString(16);
                        //encrypt key with icici certificate

                        string encryptedKeyForResponse = CryptoManagerICICI.EncryptUsingCertificate(keyForICICIForEncryption, "icici_Live_upi");

                        //aes encryption of response data

                        aesReturn aesReturn = CryptoManagerICICI.aesEncryptStringReturn(keyForICICIForEncryption, keyForICICIForEncryption + requestData);
                        // generate response for icicic

                        Final_output = new JObject(new JProperty("requestId", ""),
                            new JProperty("service", "PaymentApi"),
                            new JProperty("encryptedKey", encryptedKeyForResponse),
                            new JProperty("oaepHashingAlgorithm", "NONE"),
                            //new JProperty("iv", aesReturn.iv),
                            new JProperty("iv", ""),
                            new JProperty("encryptedData", aesReturn.encryptedText),
                            new JProperty("clientInfo", ""),
                            new JProperty("optionalParam", ""));
                        var decryptedOutput = CryptoManagerICICI.aesDecryptString(keyForICICIForEncryption, aesReturn.encryptedText);


                        bank_details = DA.MISAPPDA.bank_details("1", _config);

                        JObject output_data = IciciAPIs.upi_outward(_config, bank_details, Final_output);
                        JObject Outputdata_1 = new JObject(new JProperty("data", output_data));
                        bool successMsg = Convert.ToBoolean(output_data.SelectToken("success"));

                        message = output_data.SelectToken("message").ToString();
                        string BankRRN = output_data.SelectToken("BankRRN").ToString();
                        Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  final : " + success + message + successMsg + BankRRN);
                        if (successMsg == true)
                        {
                            DA.MISAPPDA.updateTrans(reqid, BankRRN, _config);
                            Outputdata = new JObject(new JProperty("success", successMsg), new JProperty("message", message));
                        }
                        else
                        {
                            success = "failed";
                            message = "failed to transafer 100";
                            Outputdata = new JObject(new JProperty("success", success), new JProperty("message", message));
                        }
                    }
                    else
                    {
                        success = "failed";
                        message = "failed to transafer 500";
                        Outputdata = new JObject(new JProperty("success", success), new JProperty("message", message));
                    }
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  final : " + Outputdata);
                }

            }

            catch (Exception ex)
            {
                Common.write_log_error("upi_transfer api | upi_transfer process |", "API Status : " + ex + upi_details);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject FM_transaction_approval(IConfiguration _config, JObject FMApprovalout)
        {

            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {

                dynamic obj = FMApprovalout;
                string encryptedData = (string)obj["transaction_approval"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string reqid = headerToken.SelectToken("FMApproval.reqid").ToString();
                string soc_id = headerToken.SelectToken("FMApproval.soc_id").ToString();
                string user_id = headerToken.SelectToken("FMApproval.user_id").ToString();
                string approval_level = headerToken.SelectToken("FMApproval.approval_level").ToString();


                JObject responseObj = DA.MISAPPDA.FM_transaction_approval(reqid, soc_id, user_id, approval_level, _config);
                // dynamic objout = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("FMApprovalout", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("FM_transaction_approval api | FM_transaction_approval process |", "API Status : " + ex +FMApprovalout);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //trans hit..
        public static JObject Society_NEFT_Transaction(IConfiguration _config, JObject transinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                string trans_neft_type = "";
                string flag1 = "";
                dynamic obj = transinput;
                JObject responseObj = new JObject();
                JObject responseObj2 = new JObject();
                string encryptedData = (string)obj["trans_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string hoid = headerToken.SelectToken("trans.socid").ToString();
                string flag = headerToken.SelectToken("trans.flag").ToString();
                string mobileno = headerToken.SelectToken("trans.mobileno").ToString();
                string glcode = headerToken.SelectToken("trans.glcode").ToString();
                string acc = headerToken.SelectToken("trans.acc").ToString();
                string amount = headerToken.SelectToken("trans.amount").ToString();
                int amt = Convert.ToInt16(amount);
                string benbeficiary_accno = headerToken.SelectToken("trans.benbeficiary_accno").ToString();
                string benbeficiary_bank = headerToken.SelectToken("trans.benbeficiary_bank").ToString();
                string benbeficiary_ifsccode = headerToken.SelectToken("trans.benbeficiary_ifsccode").ToString();
                string remark = headerToken.SelectToken("trans.remark").ToString();
                int trans_type = Convert.ToInt32(headerToken.SelectToken("trans.trans_type").ToString());
                if (trans_type == 1)
                {
                    trans_neft_type = "RGS";
                }
                else if (trans_type == 2)
                {
                    trans_neft_type = "IFS";
                }
                else if (trans_type == 3)
                {
                    trans_neft_type = "RTG";
                }
                string imei_no = headerToken.SelectToken("trans.imei_no").ToString();
                string ip_add = headerToken.SelectToken("trans.ip_add").ToString();
                string location = headerToken.SelectToken("trans.location").ToString();
                string ben_name = headerToken.SelectToken("trans.ben_name").ToString();
                JObject responseObj1 = DA.MISAPPDA.getlimit(hoid, _config);

                int limit = Convert.ToInt16(responseObj1["limit"].ToString());
                Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  : " + responseObj1 );
                Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  : " + transinput);
                
                if (amt <= limit)
                {
                    responseObj2 = DA.MISAPPDA.Society_NEFT_Transaction(hoid, flag, mobileno, glcode, acc, amount, benbeficiary_accno, benbeficiary_bank, benbeficiary_ifsccode, trans_neft_type, remark, imei_no, ip_add, location, ben_name, _config);
                    string reqid = responseObj2.SelectToken("trans_response.reqid").ToString();

                    if (reqid != "")
                    {
                        flag1 = "success";
                        responseObj = MisAppAPIs.release_trans_limit(_config, flag1, reqid, hoid);
                    }
                }
                else
                {
                    responseObj = DA.MISAPPDA.Society_NEFT_Transaction(hoid, flag, mobileno, glcode, acc, amount, benbeficiary_accno, benbeficiary_bank, benbeficiary_ifsccode, trans_neft_type, remark, imei_no, ip_add, location, ben_name, _config);
                }
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("trans_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("Society_NEFT_Transaction api | Society_NEFT_Transaction process |", "API Status : " + ex +transinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //transaction process icici apis calling
        public static JObject release_trans(IConfiguration _config, JObject transinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = transinput;
                string encryptedData = (string)obj["trans_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;

                string flag = headerToken.SelectToken("trans.flag").ToString();
                string reqid = headerToken.SelectToken("trans.reqid").ToString();
                string hoid = headerToken.SelectToken("trans.hoid").ToString();

                // Calling Benificary Details 

                JObject Ben_details = DA.MISAPPDA.Ben_Trans_details(reqid, _config);
                JToken BenToken = Ben_details;
                string Bank_id = BenToken.SelectToken("Param.bank_id").ToString();
                string ben_acc = BenToken.SelectToken("Param.ben_acc").ToString();
                string ben_bank = BenToken.SelectToken("Param.ben_bank").ToString();
                string ben_ifsc = BenToken.SelectToken("Param.ben_ifsc").ToString();
                string amount = BenToken.SelectToken("Param.amount").ToString();
                string trans_type = BenToken.SelectToken("Param.Trans_type").ToString();
                string ben_name = BenToken.SelectToken("Param.beneficiary_name").ToString();

                // Calling sms
                JObject sms = DA.MISAPPDA.sms_trans(reqid, _config);
                //getting icici details for api
                JObject bank_details = DA.MISAPPDA.bank_details(Bank_id, _config);
                //getting soc icici details
                JObject Soc_details = DA.MISAPPDA.getsocietybankDetails(hoid, _config);


                List<string> transaction_response = IciciAPIs.Transactional(_config, ben_acc, ben_bank, ben_ifsc, amount, "Live", bank_details, Bank_id, Soc_details, reqid, trans_type, ben_name);

                JObject jObject = JObject.Parse(transaction_response[1]);
                string input_string = transaction_response[0].ToString();
                string output_string = transaction_response[1].ToString();
                DA.MISAPPDA.response_insert(jObject, reqid, input_string, output_string, _config);


                JObject json1 = JObject.Parse(BI.Common.validJason(transaction_response[1]));
                JToken ICICIToken = json1;

                //string success = ICICIToken.SelectToken("success").ToString();

                string response = ICICIToken.SelectToken("RESPONSE").ToString();
                JObject responseObj = new JObject();
                if (response == "FAILURE")
                {
                    flag = "failed";
                    responseObj = DA.MISAPPDA.release_trans(flag, reqid, _config);
                    DA.MISAPPDA.neft_transfer_reverse(reqid, _config);


                }
                else
                {
                    flag = "success";
                    responseObj = DA.MISAPPDA.release_trans(flag, reqid, _config);
                    string sms_send = DA.MISAPPDA.Success_trans_sms(reqid, _config);
                }

                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("trans_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("release_trans api | release_trans process |", "API Status : " + ex + transinput);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;


        }
        public static JObject release_trans_limit(IConfiguration _config, string flag, string reqid, string hoid)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                JObject Ben_details = DA.MISAPPDA.Ben_Trans_details(reqid, _config);
                JToken BenToken = Ben_details;
                string Bank_id = BenToken.SelectToken("Param.bank_id").ToString();
                string ben_acc = BenToken.SelectToken("Param.ben_acc").ToString();
                string ben_bank = BenToken.SelectToken("Param.ben_bank").ToString();
                string ben_ifsc = BenToken.SelectToken("Param.ben_ifsc").ToString();
                string amount = BenToken.SelectToken("Param.amount").ToString();
                string trans_type = BenToken.SelectToken("Param.Trans_type").ToString();
                string ben_name = BenToken.SelectToken("Param.beneficiary_name").ToString();
                JObject sms = DA.MISAPPDA.sms_trans(reqid, _config);
                JObject bank_details = DA.MISAPPDA.bank_details(Bank_id, _config);
                JObject Soc_details = DA.MISAPPDA.getsocietybankDetails(hoid, _config);
                List<string> transaction_response = IciciAPIs.Transactional(_config, ben_acc, ben_bank, ben_ifsc, amount, "Live", bank_details, Bank_id, Soc_details, reqid, trans_type, ben_name);
                
                JObject jObject = JObject.Parse(transaction_response[1]);
                string input_string = transaction_response[0].ToString();
                string output_string = transaction_response[1].ToString();
                Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  : " + input_string + output_string);
                DA.MISAPPDA.response_insert(jObject, reqid, input_string, output_string, _config);
                JObject json1 = JObject.Parse(BI.Common.validJason(transaction_response[1]));
                JToken ICICIToken = json1;

                string response = ICICIToken.SelectToken("RESPONSE").ToString();
                JObject responseObj = new JObject();
                if (response == "FAILURE")
                {
                    flag = "failed";
                    responseObj = DA.MISAPPDA.release_trans(flag, reqid, _config);
                    DA.MISAPPDA.neft_transfer_reverse(reqid, _config);
                }
                else
                {
                    flag = "success";
                    responseObj = DA.MISAPPDA.release_trans(flag, reqid, _config);
                    string sms_send = DA.MISAPPDA.Success_trans_sms(reqid, _config);
                }

                //string strdata = responseObj.ToString();
                //string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                //Outputdata = new JObject(new JProperty("trans_output", encryptoutput));
                Outputdata = responseObj;
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;


        }
        //from web
        public static JObject Society_Transaction(IConfiguration _config, JObject transinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            string trans_neft_type = "";

            try
            {

                dynamic obj = transinput;
                string encryptedData = (string)obj["trans_input"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string hoid = headerToken.SelectToken("trans.hoid").ToString();
                string flag = headerToken.SelectToken("trans.flag").ToString();
                string mobileno = headerToken.SelectToken("trans.mobileno").ToString();
                string glcode = headerToken.SelectToken("trans.glcode").ToString();
                string acc = headerToken.SelectToken("trans.acc").ToString();
                string amount = headerToken.SelectToken("trans.amount").ToString();
                string benbeficiary_accno = headerToken.SelectToken("trans.benbeficiary_accno").ToString();
                string benbeficiary_bank = headerToken.SelectToken("trans.benbeficiary_bank").ToString();
                string benbeficiary_ifsccode = headerToken.SelectToken("trans.benbeficiary_ifsccode").ToString();
                string remark = headerToken.SelectToken("trans.remark").ToString();
                string userid = headerToken.SelectToken("trans.userid").ToString();
                string benbeficiary_name = headerToken.SelectToken("trans.benbeficiary_name").ToString();

                int trans_type = Convert.ToInt32(headerToken.SelectToken("trans.trans_type").ToString());
                if (trans_type == 1)
                {
                    trans_neft_type = "RGS";
                }
                else if (trans_type == 2)
                {
                    trans_neft_type = "IFS";
                }
                else if (trans_type == 3)
                {
                    trans_neft_type = "RTG";
                }

                JObject responseObj = DA.MISAPPDA.Society_Transaction(hoid, flag, mobileno, glcode, acc, amount, benbeficiary_accno, benbeficiary_bank, benbeficiary_ifsccode, benbeficiary_name, trans_neft_type, remark, userid, _config);

                // dynamic objout = responseObj;
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("trans_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //trans reject 
        public static JObject trans_reject(IConfiguration _config, JObject trans_details)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = trans_details;
                string encryptedData = (string)obj["trans_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;

                string reqid = headerToken.SelectToken("trans.reqid").ToString();
                string hoid = headerToken.SelectToken("trans.hoid").ToString();
                string reject_msg = headerToken.SelectToken("trans.msg").ToString();
                string remark = headerToken.SelectToken("trans.remark").ToString();
                string userid = headerToken.SelectToken("trans.user_id").ToString();

                JObject trans_output = DA.MISAPPDA.neft_transfer_reverse(reqid, _config);
                int code = Convert.ToInt32(trans_output.SelectToken("trans_response.ErrCode").ToString());

                if (code == -100)
                {

                    DA.MISAPPDA.trans_reject(reqid, hoid, reject_msg, remark, userid, _config);
                    Outputdata = new JObject(new JProperty("trans_output", new JObject(new JProperty("ErrorMsg", "Transaction rejected"))));
                }
                else
                {
                    Outputdata = new JObject(new JProperty("trans_output", new JObject(new JProperty("ErrorMsg", "Transaction rejection failed"))));
                }
            }
            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        // icici_regis
        public static JObject icici_regis(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["ICICIReg_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string AGGRNAME = headerToken.SelectToken("ICICIRegistration.aggrname").ToString();
                string AGGRID = headerToken.SelectToken("ICICIRegistration.aggrid").ToString();
                string CORPID = headerToken.SelectToken("ICICIRegistration.corpid").ToString();
                string USERID = headerToken.SelectToken("ICICIRegistration.userid").ToString();
                string URN = headerToken.SelectToken("ICICIRegistration.urn").ToString();

                String Jdata = IciciAPIs.Register(AGGRNAME, AGGRID, CORPID, USERID, URN, _config);

                JObject jdata = JObject.Parse(Jdata);
                string response = jdata.SelectToken("Response").ToString();
                string message = jdata.SelectToken("Message").ToString();


                //string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), Jdata);

                Outputdata = new JObject(new JProperty("ICICIReg_output", new JObject(new JProperty("Errormsg", message),
                    new JProperty("Errorcode", "100"),
                    new JProperty("Status", response))));


            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        // icici_regis_status
        public static JObject icici_regis_status(IConfiguration _config, JObject reginput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = reginput;
                string encryptedData = (string)obj["ICICIReg_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string AGGRNAME = headerToken.SelectToken("ICICIRegistration.aggrname").ToString();
                string AGGRID = headerToken.SelectToken("ICICIRegistration.aggrid").ToString();
                string CORPID = headerToken.SelectToken("ICICIRegistration.corpid").ToString();
                string USERID = headerToken.SelectToken("ICICIRegistration.userid").ToString();
                string URN = headerToken.SelectToken("ICICIRegistration.urn").ToString();

                String Jdata = IciciAPIs.RegisterStatus(AGGRNAME, AGGRID, CORPID, USERID, URN, _config);

                JObject jdata = JObject.Parse(Jdata);


                string RESPONSE = jdata.SelectToken("RESPONSE").ToString();
                string STATUS = jdata.SelectToken("Status").ToString();

                //string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), Jdata);

                Outputdata = new JObject(new JProperty("ICICIReg_output", new JObject(new JProperty("Errormsg", RESPONSE),
                    new JProperty("Errorcode", "100"),
                    new JProperty("Status", STATUS))));

            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //icici_bank_statement
        public static JObject icici_bank_statement(IConfiguration _config, string hoid, DateTime fromdate, DateTime todate)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                JObject Soc_details = DA.MISAPPDA.getsocietybankDetails(hoid, _config);
                JObject json1 = Soc_details;
                JToken headerToken1 = json1;
                string AGGRNAME = headerToken1.SelectToken("Acc_details[0].AGGRNAME").ToString();
                string AGGRID = headerToken1.SelectToken("Acc_details[0].AGGRID").ToString();
                string CORPID = headerToken1.SelectToken("Acc_details[0].CORPID").ToString();
                string USERID = headerToken1.SelectToken("Acc_details[0].USERID").ToString();
                string URN = headerToken1.SelectToken("Acc_details[0].URN").ToString();
                string ACCOUNTNO = headerToken1.SelectToken("Acc_details[0].ACCNO").ToString();

                string statement = IciciAPIs.BankStatementAPI(AGGRID, CORPID, USERID, URN, ACCOUNTNO, fromdate, todate, _config);


                JObject icici_statement = JObject.Parse(BI.Common.validJason(statement));

                string encryptedPayload = (string)icici_statement["encryptedData"];
                string encryptedkey = (string)icici_statement["encryptedKey"];

                string decryptedKey = (string)CryptoManagerICICI.DecryptResponse(encryptedkey);
                string decryptPayloadAes = CryptoManagerICICI.aesDecryptString(decryptedKey, encryptedPayload);

                JObject output_data = JObject.Parse(BI.Common.validJason(decryptPayloadAes));

                Outputdata = new JObject(new JProperty("ICICIReg_output", new JObject(
                    new JProperty("Errorcode", "100"),
                    new JProperty("Data", output_data))));

            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public static JObject trans_approval_limt(IConfiguration _config, string hoid)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {

                Outputdata = DA.MISAPPDA.trans_approval_limt(hoid, _config);


            }
            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        public static string IsExist(IConfiguration _config)
        {
            string convertedUUID = "";
            Guid MyUniqueId = Guid.NewGuid();
            convertedUUID = MyUniqueId.ToString();
            convertedUUID = "ICI" + convertedUUID.Replace("-", "");

            JObject uuid_details = DA.MISAPPDA.uuid_details(convertedUUID, _config);
            int count = Convert.ToInt32(uuid_details.SelectToken("account_details.v_accno").ToString());

            if(count == 0)
            {
                return convertedUUID;
            }
            else
            {
                IsExist( _config);
            }
            return convertedUUID;
        }
        // trans otp sms
        public static JObject trans_otp(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            JObject otpdata =new JObject();
            try
            {
                string Flag = "Trans_OTP";
                dynamic obj = data;
                string encryptedData = (string)obj["trans_data"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("details.mobile_no").ToString();
                string hoid = headerToken.SelectToken("details.hoid").ToString();
                string acc_no = headerToken.SelectToken("details.acc_no").ToString();
                string bene_acc = headerToken.SelectToken("details.bene_acc").ToString();
                string bene_name = headerToken.SelectToken("details.bene_name").ToString();
                string amount = headerToken.SelectToken("details.amount").ToString();
                otpdata = DA.MISAPPDA.Resend_OTP(mobile_no,hoid, _config);
                int otp = Convert.ToInt32(otpdata.SelectToken("resendotp_response.OTP").ToString());
                responseObj = DA.MISAPPDA.sms_trans_otp(Flag,hoid, mobile_no, otp, acc_no,bene_name,amount, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("trans_data_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        //kyc verify
        public static JObject KYC_Verify(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            JObject otpdata = new JObject();
            try
            {
                dynamic obj = data;
                string encryptedData = (string)obj["KYC_Data"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("KYC.mobile_no").ToString();
                string hoid = headerToken.SelectToken("KYC.hoid").ToString();
                responseObj = DA.MISAPPDA.KYC_Verify(hoid, mobile_no, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("KYC_Data_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        //Bank details
        public static JObject Bank_details(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            JObject otpdata = new JObject();
            try
            {
                dynamic obj = data;
                string encryptedData = (string)obj["Bank_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                
                json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string acc_no = headerToken.SelectToken("BD.acc_no").ToString();
                responseObj = DA.MISAPPDA.Bank_details( acc_no, _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Bank_details_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        //ICICI Transaction status
        public static JObject icici_trans_status(IConfiguration _config, JObject trans_input)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            try
            {
                dynamic obj = trans_input;
                string encryptedData = (string)obj["ICICITrans_details"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string AGGRID = headerToken.SelectToken("ICICITrans.aggrid").ToString();
                string CORPID = headerToken.SelectToken("ICICITrans.corpid").ToString();
                string USERID = headerToken.SelectToken("ICICITrans.userid").ToString();
                string UNIQUEID = headerToken.SelectToken("ICICITrans.uniqueid").ToString();
                string URN = headerToken.SelectToken("ICICITrans.urn").ToString();

                String Jdata = IciciAPIs.TransactionalStatus( AGGRID, CORPID, USERID, UNIQUEID, URN, _config);

                JObject jdata = JObject.Parse(Jdata);
                Common.write_log_Success("trans_status.aspx | iciciReq Method Response |", "API Status : " + Jdata);

                string RESPONSE = jdata.SelectToken("RESPONSE").ToString();
                string STATUS = jdata.SelectToken("Status").ToString();

                //string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), Jdata);

                Outputdata = new JObject(new JProperty("ICICITrans_output", new JObject(new JProperty("Errormsg", RESPONSE),
                    new JProperty("Errorcode", "100"),
                    new JProperty("Status", STATUS))));

            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        //socitey details for FM
        public static JObject Society_Details_FM(IConfiguration _config, string soc_id)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            
            JObject Outputdata = new JObject();
            try
            {
                JObject responseObj = DA.MISAPPDA.Society_Deatils_FM(soc_id, _config);
                Outputdata = new JObject(new JProperty("Society_output", responseObj));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;
        }
        public bool IsNullOrEmpty(string input)
        {
            return input == null || input == string.Empty;
        }
        //loan trans
        public static JObject loan_incoming_trans(Int64 ReqID, string VirtualAccNo, Double Amount, string TransID,  IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject databaseoutput = new JObject();
            try
            {
                
                databaseoutput = MISAPPDA.getTransactionDetails(ReqID, VirtualAccNo, Amount, TransID, _config);

            }
            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                databaseoutput = handleErrorData_1("loginoutput", _config);
            }
            return databaseoutput;
        }
        public static JObject beneficiary_otp(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            JObject otpdata = new JObject();
            try
            {
                string Flag = "Ben_OTP";
                dynamic obj = data;
                string encryptedData = (string)obj["Ben_data"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Otpdetails.mobile_no").ToString();
                string hoid = headerToken.SelectToken("Otpdetails.hoid").ToString();
                string ben_name = headerToken.SelectToken("Otpdetails.name_of_beneficiary").ToString();
                otpdata = DA.MISAPPDA.Resend_OTP(mobile_no, hoid, _config);
                int otp = Convert.ToInt32(otpdata.SelectToken("resendotp_response.OTP").ToString());
                responseObj = DA.MISAPPDA.sms_trans_otp(Flag, hoid, mobile_no, otp,"0", ben_name, "0", _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("ben_data_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }
        public static JObject limitset_otp(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            JObject otpdata = new JObject();
            try
            {
                string Flag = "Limit_OTP";
                dynamic obj = data;
                string encryptedData = (string)obj["Limit_data"];
                string decryptedData = (string)CryptoManagerMISAPP.DecryptString(getApiKey(_config), encryptedData);
                json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string mobile_no = headerToken.SelectToken("Otpdetails.mobile_no").ToString();
                string hoid = headerToken.SelectToken("Otpdetails.hoid").ToString();
                otpdata = DA.MISAPPDA.Resend_OTP(mobile_no, hoid, _config);
                int otp = Convert.ToInt32(otpdata.SelectToken("resendotp_response.OTP").ToString());
                responseObj = DA.MISAPPDA.sms_trans_otp(Flag, hoid, mobile_no, otp, "0", "0", "0", _config);
                string strdata = responseObj.ToString();
                string encryptoutput = (string)CryptoManagerMISAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("Limit_data_output", encryptoutput));
            }

            catch (Exception ex)
            {
                Common.write_log_error("verifyotp api | verifyotp process |", "API Status : " + ex);
                Outputdata = handleErrorData_1("loginoutput", _config);
            }
            return Outputdata;

        }

       
    }
}   


