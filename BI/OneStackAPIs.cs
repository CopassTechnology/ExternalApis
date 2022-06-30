using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.DA;

namespace ExternalAPIs.BI
{
    public class OneStackAPIs
    {
        public static string getApiKey(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["oneStackAPIKey"];
        }
        public static string encryptDecrypt(IConfiguration _config, string sampleText)
        {
            string response = CBSKit.CBSSecurity.Encrypt(sampleText, getApiKey(_config));
            string response1 = CBSKit.CBSSecurity.Decrypt(sampleText, getApiKey(_config));
            //string response1 = CryptoManagerICICI.DecryptUsingCertificate(response);

            return response1;
        }
        public static string decrypttest(IConfiguration _config, string sampleText)
        {
            
            string decryptedData = CBSKit.CBSSecurity.Decrypt(sampleText, getApiKey(_config));

            return decryptedData;
        }
        public static string encryptedData(IConfiguration _config, string sampleText)
        {

            string encryptedData = CBSKit.CBSSecurity.Encrypt(sampleText, getApiKey(_config));

            return encryptedData;
        }

        //onborading

        public static JObject checkUser(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {
            
            // DataBase Method for checking  user exists in database "Y/N" Start
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;
            string mobile_no = headerToken.SelectToken("param_array.mobile_no").ToString();
            string socid = headerToken.SelectToken("param_array.bank_id").ToString();

            JObject databaseoutput = OneStackDA.Userexits(mobile_no, socid, _config);

            //Database output converted into  Jproperty[Data] fromat end

            string paramdata = "";
            paramdata = BI.Common.validJason(databaseoutput["Param"].ToString());
            string encryptedres =  CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
            //Onestack final output formatm
            JObject responseObj = new JObject(
                new JProperty("ver",obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString()),
                new JProperty("txnid", obj["txnid"].ToString()),
                new JProperty("status", databaseoutput["status"].ToString()),
                new JProperty("data", encryptedres)
            );
            return responseObj;
            }
            catch (Exception)
            {
                JObject responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString()),
                new JProperty("txnid", obj["txnid"].ToString()),
                new JProperty("status", "failed"),
                new JProperty("data", "Unable to proceed - webserve"));
                return responseObj;
            }
        }
        public static JObject new_user(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));

            try
            {

                string otp;
                string errcode = "";
                string errmsg = "";
                string msg = "";

                
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                //string crn = headerToken.SelectToken("param_array.crn").ToString();

                string mobile_no = headerToken.SelectToken("param_array.mobile_no").ToString();
                string socid = headerToken.SelectToken("param_array.bank_id").ToString();
                string pan_no = headerToken.SelectToken("param_array.pan_no").ToString();
                string dob = headerToken.SelectToken("param_array.dob").ToString();
                string username = headerToken.SelectToken("param_array.username").ToString();

                string password = headerToken.SelectToken("param_array.password").ToString();
                string debit_last_4 = headerToken.SelectToken("param_array.debit_last_4").ToString();
                string debit_pin = headerToken.SelectToken("param_array.debit_pin").ToString();
                JObject databaseoutput = OneStackDA.new_user(mobile_no, socid, pan_no, dob, username, password, debit_last_4, debit_pin, _config);
                otp = databaseoutput["otp"].ToString();
                errcode = databaseoutput["ErrCode"].ToString();
                errmsg = databaseoutput["ErrMsg"].ToString();

                //string smsoutres;
                //if (errcode == "500")
                //{
                //    JObject smsoutput = OneStackDA.sms_reg(otp, mobile_no, _config);
                //    // JObject json_sms = JObject.Parse(BI.Common.validJason(smsoutput));
                //    JToken headerToken_sms = smsoutput;
                //    smsoutres = headerToken_sms.SelectToken("sendsms_response.MsgStatus").ToString();
                //    msg = "OTP sent to registered mobile number.";
                //}
                //else
                //{
                //    smsoutres = "failed";
                //    msg = "Unable to send OTP on register mobile number";

                //}
                string paramdata = "";

                paramdata = BI.Common.validJason(databaseoutput["Param"].ToString());
                string encryptedres = CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
                //Onestack final output format
                JObject responseObj = new JObject();
                if (errcode == "500")
                {
                    responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),

                   new JProperty("registration_id", obj["registration_id"].ToString()),
                   new JProperty("status", "success"),
                   new JProperty("message", errmsg),
                   new JProperty("data", encryptedres)
               );
                }
                else
                {
                    responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),
                   new JProperty("status", "failed"),
                   new JProperty("message", errmsg)

                    );
                }
                return responseObj;
            }
            catch (Exception)
            {
              JObject  responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),
                new JProperty("status", "failed"),
                new JProperty("data", "Unable to proceed - webserve"));
                return responseObj;
            }
        }
        //public static JObject auth_registration_otp(IConfiguration _config, JObject data)
        //{
        //    dynamic obj = data;
        //    string encryptedData = data.SelectToken("data").ToString();
        //    encryptedData = CryptoManagerOneStack.EncryptString(getApiKey(_config),encryptedData);
        //    string decryptedData = CBSKit.CBSSecurity.Decrypt( encryptedData, getApiKey(_config));
        //    // DataBase Method for checking  user exists in database "Y/N" Start

        //    JObject databaseoutput = OneStackDA.aut_registration_otp(data);

        //    //Database output converted into  Jproperty[Data] fromat end

        //    string paramdata = (string)databaseoutput["param"];
        //    string encryptedres =  CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
            
        //    //Onestack final output format
        //    JObject responseObj = new JObject(
        //        new JProperty("ver", obj["ver"].ToString()),
        //        new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
        //        new JProperty("txnid", obj["txnid"].ToString()),
        //        new JProperty("registration_id", obj["registration_id"].ToString()),
        //        new JProperty("status", databaseoutput["status"].ToString()),
        //        new JProperty("message", databaseoutput["message"].ToString())
        //    );

        //    return responseObj;
        //}
        public static JObject register(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {

                string otp;
                string errcode = "";
                string errmsg = "";
                string msg = "";


                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string crn = headerToken.SelectToken("param_array.crn").ToString();

                string mobile_no = headerToken.SelectToken("param_array.mobile_no").ToString();
                string socid = headerToken.SelectToken("param_array.bank_id").ToString();
                string pan_no = headerToken.SelectToken("param_array.pan_no").ToString();
                string dob = headerToken.SelectToken("param_array.dob").ToString();
                string username = headerToken.SelectToken("param_array.username").ToString();

                string password = headerToken.SelectToken("param_array.password").ToString();
                string debit_last_4 = headerToken.SelectToken("param_array.debit_last_4").ToString();
                string debit_pin = headerToken.SelectToken("param_array.debit_pin").ToString();
                JObject databaseoutput = OneStackDA.new_user(mobile_no, socid, pan_no, dob, username, password, debit_last_4, debit_pin, _config);
                otp = databaseoutput["otp"].ToString();
                errcode = databaseoutput["ErrCode"].ToString();
                errmsg = databaseoutput["ErrMsg"].ToString();

                //string smsoutres;
                //if (errcode == "500")
                //{
                //    JObject smsoutput = OneStackDA.sms_reg(otp, mobile_no, _config);
                //    smsoutres = smsoutput["MsgStatus"].ToString();
                //    msg = "OTP sent to registered mobile number.";
                //}
                //else
                //{
                //    smsoutres = "failed";
                //    msg = "Unable to send OTP on register mobile number";

                //}

                string paramdata = (string)databaseoutput["param"];
                string encryptedres = CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
                //Onestack final output format
                JObject responseObj = new JObject();
                if (errcode == "500")
                {
                    responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),

                   new JProperty("registration_id", databaseoutput["reg_id"].ToString()),
                   new JProperty("status", "success"),
                   new JProperty("message", errmsg),
                   new JProperty("data", encryptedres)
               );
                }
                else
                {
                    responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),
                   new JProperty("status", "failed"),
                   new JProperty("message", errmsg));
                }
                return responseObj;
            }
            catch (Exception)
            {
              JObject  responseObj = new JObject(
                 new JProperty("ver", obj["ver"].ToString()),
                 new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                 new JProperty("txnid", obj["txnid"]),
                 new JProperty("status", "failed"),
                 new JProperty("message", "Unable to proceed - webserve"));
                return responseObj;
            }
        }
        public static JObject auth_registration_otp(IConfiguration _config, JObject data)
        {

            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {

            string errcode = "";
            string errmsg = "";
            string msg = "";
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;
            string utr_no = headerToken.SelectToken("param_array.utr_no").ToString();
            string otp = headerToken.SelectToken("param_array.otp").ToString();
            JObject databaseoutput = OneStackDA.aut_registration_otp(utr_no, otp, _config);

            string paramdata = "";

            paramdata = BI.Common.validJason(databaseoutput["Param"].ToString());
            string encryptedres = CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
            JObject responseObj = new JObject();
            responseObj = new JObject(
              new JProperty("ver", obj["ver"].ToString()),
              new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
              new JProperty("txnid", obj["txnid"]),
              new JProperty("status", "success"),
              new JProperty("message", databaseoutput["ErrMsg"].ToString()) );
            return responseObj;
            }
            catch (Exception)
            {
               JObject responseObj = new JObject(
             new JProperty("ver", obj["ver"].ToString()),
             new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
             new JProperty("txnid", obj["txnid"]),
             new JProperty("status", "failed"),
             new JProperty("message", "Unable to proceed - webserve"));

                return responseObj;
            }
        }



        //Tokenization

        public static JObject token(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {

            
            string otp;
            string errcode = "";
            string errmsg = "";
            string msg = "";

            
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;

            string customer = headerToken.SelectToken("param_array.customer").ToString();
            JObject Jcustobj = JObject.Parse(customer);
            JToken jToken = Jcustobj;

            string bank_id = jToken.SelectToken("bank_id").ToString();
            string branch_id = jToken.SelectToken("branch_id").ToString();
            string crn = jToken.SelectToken("crn").ToString();
            string customer_token = jToken.SelectToken("customer_token").ToString();

            JObject databaseoutput = OneStackDA.gettoken(bank_id,branch_id,crn,customer_token,_config);
            string paramdata = (string)databaseoutput["param"];
            string encryptedres =  CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
            //Onestack final output format
            JObject responseObj = new JObject();

            responseObj = new JObject(
           new JProperty("ver", obj["ver"].ToString()),
           new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
           new JProperty("txnid", obj["txnid"]),
           new JProperty("status", databaseoutput["status"].ToString()),
           new JProperty("message", databaseoutput["message"].ToString()));
           //new JProperty("data", encryptedres));
            
            return responseObj;
                }
            catch (Exception)
            {
                JObject responseObj = new JObject(
              new JProperty("ver", obj["ver"].ToString()),
              new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
              new JProperty("txnid", obj["txnid"]),
              new JProperty("status", "failed"),
              new JProperty("message", "Unable to proceed - webserve"));

                return responseObj;
            }
        }


        //Customer Account

        public static JObject accounts_discover(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {
            string otp;
            string errcode = "";
            string status = "";
            string msg = "";
            string token = "";

            
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;

            string customer = headerToken.SelectToken("param_array.customer").ToString();
            JObject Jcustobj = JObject.Parse(customer);
            JToken jToken = Jcustobj;

            string bank_id = jToken.SelectToken("bank_id").ToString();
            string branch_id = jToken.SelectToken("branch_id").ToString();
            string customer_token = jToken.SelectToken("customer_token").ToString();

            JObject databaseoutput = OneStackDA.accounts_discover(bank_id, branch_id, customer_token, _config);
            errcode = databaseoutput["code"].ToString();
            status = databaseoutput["status"].ToString();
            msg = databaseoutput["message"].ToString();
            token = databaseoutput["tokenid"].ToString();

            JObject tokendetails = DA.OneStackDA.tokendetails(token, _config);
                JObject token_d = JObject.Parse(tokendetails.ToString());
                JToken jststr = token_d;
                string Fdata = jststr.SelectToken("Account_response.ErrCode").ToString();
            string paramdata = (string)databaseoutput["param"];
            string encryptedres = CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
            JObject responseObj = new JObject();


            if (errcode == "100")
            {
                responseObj = new JObject(
           new JProperty("ver", obj["ver"].ToString()),
           new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
           new JProperty("txnid", obj["txnid"]),
           new JProperty("status", databaseoutput["status"].ToString()),
           new JProperty("message", databaseoutput["message"].ToString()),
           new JProperty("data", tokendetails.SelectToken("Account_response.data").ToString()));
            }
            else if(errcode == "200")
                {
                responseObj = new JObject(
          new JProperty("ver", obj["ver"].ToString()),
          new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
          new JProperty("txnid", obj["txnid"]),
          new JProperty("status", databaseoutput["status"].ToString()),
          new JProperty("message", databaseoutput["message"].ToString()),
          new JProperty("data", tokendetails.SelectToken("Account_response.data").ToString()));
            }
            else if(errcode == "500" || Fdata=="500")
            {
                responseObj = new JObject(
          new JProperty("ver", obj["ver"].ToString()),
          new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
          new JProperty("txnid", obj["txnid"]),
          new JProperty("status", databaseoutput["status"].ToString()),
          new JProperty("message", databaseoutput["message"].ToString()));
            }
            //Onestack final output format
            

           // responseObj = new JObject(
           //new JProperty("ver", obj["ver"].ToString()),
           //new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
           //new JProperty("txnid", obj["txnid"]),
           //new JProperty("status", databaseoutput["status"].ToString()),
           //new JProperty("message", databaseoutput["message"].ToString()),
           //new JProperty("data", encryptedres));

            return responseObj;
            }
            catch (Exception)
            {
               JObject responseObj = new JObject(
             new JProperty("ver", obj["ver"].ToString()),
             new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
             new JProperty("txnid", obj["txnid"]),
             new JProperty("status", "failed"),
             new JProperty("message", "Unable to proceed - webserve"));
                return responseObj;
            }
        }
        public static JObject account_balance(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {

            string otp;
            string errcode = "";
            string errmsg = "";
            string msg = "";

            
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;

            string customer = headerToken.SelectToken("param_array.customer").ToString();
            JObject Jcustobj = JObject.Parse(customer);
            JToken jToken = Jcustobj;

            string bank_id = jToken.SelectToken("bank_id").ToString();
            string branch_id = jToken.SelectToken("branch_id").ToString();
            string account_no = jToken.SelectToken("account_no").ToString();
            string customer_token = jToken.SelectToken("customer_token").ToString();

            JObject databaseoutput = OneStackDA.account_balance(account_no, customer_token,_config);
           // string Jdata1 = databaseoutput.SelectToken("Param[0].final").ToString();
            string paramdata = (databaseoutput["Param"].ToArray()[0]).ToString();
            string paramdata1 =  (databaseoutput["Param"].ToArray()[1]).ToString();
            JToken finaldata = JObject.Parse(paramdata1);
            string code = finaldata.SelectToken("final.code").ToString();

            string encryptedres = CBSKit.CBSSecurity.Encrypt(paramdata, getApiKey(_config));
            //Onestack final output format
            JObject responseObj = new JObject();
            

            if (code == "100")
            {
                responseObj = new JObject(
               new JProperty("ver", obj["ver"].ToString()),
               new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
               new JProperty("txnid", obj["txnid"]),
               new JProperty("status", finaldata.SelectToken("final.status").ToString()),
               new JProperty("message", finaldata.SelectToken("final.message").ToString()),
               new JProperty("data", encryptedres));
            }
            else if (code == "500" || code == "0")
            {
                responseObj = new JObject(
               new JProperty("ver", obj["ver"].ToString()),
               new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
               new JProperty("txnid", obj["txnid"]),
               new JProperty("status", "failed"),
               new JProperty("message", "Provided account number does not match with bank database."));
               //new JProperty("account_details", encryptedres));

            }

            return responseObj;
            }
            catch (Exception)
            {
              JObject  responseObj = new JObject(
               new JProperty("ver", obj["ver"].ToString()),
               new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
               new JProperty("txnid", obj["txnid"]),
               new JProperty("status", "failed"),
               new JProperty("message", "Unable to proceed - webserve"));
                return responseObj;
            }
        }
        public static JObject transactions_list(IConfiguration _config, JObject data)
        {
            JObject responseObj = new JObject();
            string otp;
            string errcode = "";
            string errmsg = "";
            string msg = "";

            dynamic obj = data;
            string encryptedData = (string)obj["data"];

            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;

                string customer = headerToken.SelectToken("param_array.customer").ToString();
                JObject Jcustobj = JObject.Parse(customer);
                JToken jToken = Jcustobj;
               
                string bank_id = jToken.SelectToken("bank_id").ToString();
                string branch_id = jToken.SelectToken("branch_id").ToString();
                string account_no = jToken.SelectToken("account_no").ToString();
                string customer_token = jToken.SelectToken("customer_token").ToString();

                DateTime txn_date_from = Convert.ToDateTime(headerToken.SelectToken("param_array.txn_date_from").ToString());
                
                DateTime txn_date_to = Convert.ToDateTime(headerToken.SelectToken("param_array.txn_date_to").ToString());
                
                // string min_amount = jToken.SelectToken("min_amount").ToString();
                //string max_amount = jToken.SelectToken("max_amount").ToString();
                // string txn_type = jToken.SelectToken("txn_type").ToString();
                //string cheque_no_from = jToken.SelectToken("cheque_no_from").ToString();
                //string cheque_no_to = jToken.SelectToken("cheque_no_to").ToString();
                //string txn_remarks = jToken.SelectToken("txn_remarks").ToString();

                JObject databaseoutput = OneStackDA.transactions_list(account_no, txn_date_from, txn_date_to, _config);
                JObject output = (JObject)databaseoutput.SelectToken("param_array");

                int err_code = Convert.ToInt16(databaseoutput["Error_code"]);
                JObject resObj = new JObject(
                new JProperty("param_array", output));

                string encryptedres = CBSKit.CBSSecurity.Encrypt(resObj.ToString(), getApiKey(_config));

                if (err_code == 500)
                {
                    responseObj = new JObject();

                    responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),
                   new JProperty("status", "success"),
                   new JProperty("message", "Account details fetched successfully."),
                   new JProperty("data", encryptedres));
                }
                else
                {
                    responseObj = new JObject(
                      new JProperty("ver", obj["ver"].ToString()),
                      new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                      new JProperty("txnid", obj["txnid"]),
                      new JProperty("status", "failed"),
                      new JProperty("message", "Provided account number does not match with bank database.")
                      );
                }


                return responseObj;
            }
            catch (Exception)
            {
                responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new JProperty("txnid", obj["txnid"]),
                new JProperty("status", "failed"),
                new JProperty("message", "Unable to proceed - webserve"));
                return responseObj;
            }
        }
        public static JObject account_mini_statement(IConfiguration _config, JObject data)
        {
            JObject responseObj = new JObject();
            string otp;
            string errcode = "";
            string errmsg = "";
            string msg = "";

            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            try
            {
                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;

                string customer = headerToken.SelectToken("param_array.customer").ToString();
                JObject Jcustobj = JObject.Parse(customer);
                JToken jToken = Jcustobj;

                string bank_id = jToken.SelectToken("bank_id").ToString();
                string branch_id = jToken.SelectToken("branch_id").ToString();
                string account_no = jToken.SelectToken("account_no").ToString();
                string customer_token = jToken.SelectToken("customer_token").ToString();

                JObject databaseoutput = OneStackDA.mini_statment(account_no, _config);
                JObject output = (JObject)databaseoutput.SelectToken("param_array");

                int err_code = Convert.ToInt16(databaseoutput["Error_code"]);
                JObject resObj = new JObject(
                new JProperty("param_array", output));

                string encryptedres = CBSKit.CBSSecurity.Encrypt(resObj.ToString(), getApiKey(_config));

                if (err_code == 500)
                {
                    responseObj = new JObject();

                    responseObj = new JObject(
                   new JProperty("ver", obj["ver"].ToString()),
                   new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                   new JProperty("txnid", obj["txnid"]),
                   new JProperty("status", "success"),
                   new JProperty("message", "Account details fetched successfully."),
                   new JProperty("data", encryptedres));
                }
                else
                {
                    responseObj = new JObject(
                      new JProperty("ver", obj["ver"].ToString()),
                      new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                      new JProperty("txnid", obj["txnid"]),
                      new JProperty("status", "failed"),
                      new JProperty("message", "Provided account number does not match with bank database.")
                      );
                }


                return responseObj;
            }
            catch (Exception)
            {
                responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new JProperty("txnid", obj["txnid"]),
                new JProperty("status", "failed"),
                new JProperty("message", "Unable to proceed - webserve"));
                return responseObj;
            }
        }


        //Customer Details KYC , By Noor

        public static JObject fetch_cust_details(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;

            string customer = headerToken.SelectToken("param_array.customer").ToString();
            JObject Jcustobj = JObject.Parse(customer);
            JToken jToken = Jcustobj;

            string bank_id = jToken.SelectToken("bank_id").ToString();
            string branch_id = jToken.SelectToken("branch_id").ToString();
            string customer_token = jToken.SelectToken("customer_token").ToString();

            JObject databaseoutput = OneStackDA.getcustomer(bank_id, branch_id, customer_token, _config);

            string dataencrypt = (string)databaseoutput["data"];

            JObject responseObj = new JObject();

            if (dataencrypt is not null)
            {

                responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new JProperty("txnid", obj["txnid"]),
                new JProperty("status", "success"),
                new JProperty("message", "Customer details fetched successfully."),
                new JProperty("data", dataencrypt)
                );
            }
            else
            {
                responseObj = new JObject(
               new JProperty("ver", obj["ver"].ToString()),
               new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
               new JProperty("txnid", obj["txnid"]),
               new JProperty("status", "failed"),
               new JProperty("message", "Provided customer token does not match with bank database.")
               );
            }

            return responseObj;
        }
        public static JObject sub_customer_kyc(IConfiguration _config, JObject data)
        {
            Int16 errcode = 0;
            string status = "";
            string msg = "";

            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
            JToken headerToken = json;

            string customer = headerToken.SelectToken("param_array.customer").ToString();
            JObject Jcustobj = JObject.Parse(customer);
            JToken jToken = Jcustobj;

            string bank_id = jToken.SelectToken("bank_id").ToString();
            string branch_id = jToken.SelectToken("branch_id").ToString();
            string customer_token = jToken.SelectToken("customer_token").ToString();


            string first_name = headerToken.SelectToken("param_array.first_name").ToString();//same name as given in onestack "first_name" parameters
            string last_name = headerToken.SelectToken("param_array.last_name").ToString();
            string middle_name = headerToken.SelectToken("param_array.middle_name").ToString();

            DateTime dob = DateTime.Now;

            //if ((dob is null) ||  (dob == ""))
            //{
            //    dob = "1900-01-01";
            //}

            // DateTime dob = Convert.ToDateTime(headerToken.SelectToken("param_array.dob").ToString());

            string pan_no = headerToken.SelectToken("param_array.pan_no").ToString();
            string aadhar_no = headerToken.SelectToken("param_array.aadhar_no").ToString();
            string credit_score = headerToken.SelectToken("param_array.credit_score").ToString();
            string email_id = headerToken.SelectToken("param_array.email_id").ToString();
            string mobile_no = headerToken.SelectToken("param_array.mobile_no").ToString();
            string address_line_1 = headerToken.SelectToken("param_array.address_line_1").ToString();
            string address_line_2 = headerToken.SelectToken("param_array.address_line_2").ToString();
            string city = headerToken.SelectToken("param_array.city").ToString();
            string state = headerToken.SelectToken("param_array.state").ToString();
            string country = headerToken.SelectToken("param_array.country").ToString();
            string postal_code = headerToken.SelectToken("param_array.postal_code").ToString();

            JObject databaseoutput = OneStackDA.customer_kyc(bank_id, branch_id, customer_token, first_name, last_name, middle_name, dob,
             pan_no, aadhar_no, credit_score, email_id, mobile_no, address_line_1, address_line_2, city, state, country, postal_code, _config);

            errcode = Convert.ToInt16(databaseoutput["ErrCode"]);
            status = (string)databaseoutput["status"];
            msg = (string)databaseoutput["message"];

            JObject responseObj = new JObject();

            if (errcode == 100)
            {
                responseObj = new JObject(
           new JProperty("ver", obj["ver"].ToString()),
           new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
           new JProperty("txnid", obj["txnid"]),
           new JProperty("status", status),
           new JProperty("message", msg)
          );
            }
            else if (errcode == 200)
            {
                responseObj = new JObject(
            new JProperty("ver", obj["ver"].ToString()),
            new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new JProperty("txnid", obj["txnid"]),
            new JProperty("status", status),
            new JProperty("message", msg)
            );
            }
            else if (errcode == 500)
            {
                responseObj = new JObject(
            new JProperty("ver", obj["ver"].ToString()),
            new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new JProperty("txnid", obj["txnid"]),
            new JProperty("status", status),
            new JProperty("message", msg)
           );
            }
            return responseObj;
        }


        //upi payments
        public static JObject initiateUpi(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            JObject responseObj = new JObject();
            try
            {

                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;
                string bank_id = headerToken.SelectToken("param_array.customer.bank_id").ToString();
                string branch_id = headerToken.SelectToken("param_array.customer.branch_id").ToString();
                string customer_token = headerToken.SelectToken("param_array.customer.customer_token").ToString();
                string account_no = headerToken.SelectToken("param_array.customer.account_no").ToString();
                string receiver_upi_id = headerToken.SelectToken("param_array.receiver_upi_id").ToString();
                string receiver_name = headerToken.SelectToken("param_array.receiver_name").ToString();
                string sender_upi_id = headerToken.SelectToken("param_array.sender_upi_id").ToString();
                string amount = headerToken.SelectToken("param_array.amount").ToString();
                string upi_transaction_date = headerToken.SelectToken("param_array.upi_transaction_date").ToString();
                string upi_transaction_no = headerToken.SelectToken("param_array.upi_transaction_no").ToString();
                string settlement_status = headerToken.SelectToken("param_array.settlement_status").ToString();



                JObject databaseoutput = OneStackDA.initiateUpi(bank_id, branch_id, customer_token, account_no, receiver_upi_id, receiver_name, sender_upi_id,
                amount, upi_transaction_date, upi_transaction_no, settlement_status, _config);

                string dataencrypt = databaseoutput["initiateUpi"].ToString();
                JObject json_new = JObject.Parse(dataencrypt);
                //   JToken headerToken = json;




                responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new JProperty("txnid", obj["txnid"]),
                new JProperty("status", json_new["status"]),
                new JProperty("message", json_new.SelectToken("message"))

                );

            }
            catch (Exception ex)
            {

                throw;
            }

            return responseObj;


        }
        public static JObject SettlementUPI(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            JObject responseObj = new JObject();
            JArray newdata1 = new JArray();
            //string utr_transactions;
            //string settlement_utr_no;
            //string settlement_status;
            //string utr_transaction_date;
            //string amount;

            // JObject json = new JObject();
            try
            {



                JObject json = JObject.Parse(BI.Common.validJason(decryptedData).ToString());
                JToken headerToken = json;
                string bank_id = headerToken.SelectToken("param_array.bank_id").ToString();
                string branch_id = headerToken.SelectToken("param_array.branch_id").ToString();

                string utr_transactions = headerToken.SelectToken("param_array.utr_transactions").ToString();

                ////  JToken utr_transactions_sub = json1;

                // string upi_transaction_no = utr_transactions_sub.SelectToken("upi_transaction_no").ToString();
                // string settlement_utr_no = headerToken.SelectToken("param_array.utr_transactions.settlement_utr_no").ToString();
                //string settlement_status = headerToken.SelectToken("param_array.utr_transactions.settlement_status").ToString();
                //string utr_transaction_date = headerToken.SelectToken("param_array.utr_transactions.utr_transaction_date").ToString();
                //string amount = headerToken.SelectToken("param_array.utr_transactions.amount").ToString();
                //  string utr_transactions = headerToken.SelectToken("param_array.utr_transactions").ToString();
                // JObject utr_transactions_jdata = JObject.Parse(utr_transactions);

                JArray jsonarray = new JArray();
                jsonarray = JArray.Parse(utr_transactions);
                string newdata = jsonarray.ToString();
                dynamic data1 = JObject.Parse(jsonarray[0].ToString());



                //List<JObject> jsonObject = new List<JObject>();

                //jsonObject.Add(GetValue("upi_transaction_no").ToString());
                //jsonObject.Add("Honda");
                //jsonObject.Add("Honda");
                //jsonObject.Add("Honda");
                //jsonObject.Add("Honda");


                foreach (JObject region in jsonarray)
                {
                    string upi_transaction_no = region.GetValue("upi_transaction_no").ToString();
                    string settlement_utr_no = region.GetValue("settlement_utr_no").ToString();
                    string settlement_status = region.GetValue("settlement_status").ToString();
                    string utr_transaction_date = region.GetValue("utr_transaction_date").ToString();
                    string amount = region.GetValue("amount").ToString();
                    newdata1.Add(new JObject(new JProperty("data", region)));
                    // do something

                    JObject databaseoutput = OneStackDA.SettlementUPI(bank_id, branch_id, upi_transaction_no, settlement_utr_no, settlement_status, utr_transaction_date,
                   amount, _config);
                    string dataencrypt = databaseoutput["SettlementUPI"].ToString();
                    JObject json_new = JObject.Parse(dataencrypt);



                    responseObj = new JObject(
                    new JProperty("ver", obj["ver"].ToString()),
                    new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("txnid", obj["txnid"]),
                    new JProperty("status", json_new["status"]),
                    new JProperty("message", json_new.SelectToken("message"))

                    );

                }


                /*string newdata_1 = newdata1.ToString();*/

            }
            catch (Exception)
            {
                responseObj = new JObject(
                new JProperty("ver", obj["ver"].ToString()),
                new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new JProperty("txnid", obj["txnid"]),
                new JProperty("status", "failed"),
                new JProperty("message", "Unable to proceed - webserve"));
                return responseObj;
            }

            return responseObj;


        }
        public static JObject UPIStatusCheck(IConfiguration _config, JObject data)
        {
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CBSKit.CBSSecurity.Decrypt(encryptedData, getApiKey(_config));
            JObject responseObj = new JObject();
            try
            {



                JObject json = JObject.Parse(BI.Common.validJason(decryptedData));
                JToken headerToken = json;

                string bank_id = headerToken.SelectToken("param_array.customer.bank_id").ToString();
                string branch_id = headerToken.SelectToken("param_array.customer.branch_id").ToString();
                string customer_token = headerToken.SelectToken("param_array.customer.customer_token").ToString();
                string upi_transaction_no = headerToken.SelectToken("param_array.upi_transaction_no").ToString();

                JObject databaseoutput = OneStackDA.UPIStatusCheck(bank_id, branch_id, customer_token, upi_transaction_no, _config);  //UPIStatusCheck

                string status = databaseoutput.SelectToken("status").ToString();

                //   JToken headerToken = json;



                if (status == "success")
                {
                    string param_array = databaseoutput.SelectToken("param_array").ToString();
                    JObject json1 = JObject.Parse(BI.Common.validJason(param_array));
                    JObject jdata = new JObject(new JProperty("param_array", json1));
                    string finaldata = jdata.ToString();
                    string encryptedres = CBSKit.CBSSecurity.Encrypt(finaldata, getApiKey(_config));

                    responseObj = new JObject(
                    new JProperty("ver", obj["ver"].ToString()),
                    new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("txnid", obj["txnid"]),
                     new JProperty("status", databaseoutput.SelectToken("status")),
                    new JProperty("message", databaseoutput.SelectToken("message")),
                     new JProperty("data", encryptedres));

                }

                else
                {
                    responseObj = new JObject(
                    new JProperty("ver", obj["ver"].ToString()),
                    new JProperty("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new JProperty("txnid", obj["txnid"]),
                     new JProperty("status", databaseoutput.SelectToken("status")),
                    new JProperty("message", databaseoutput.SelectToken("message")));
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return responseObj;


        }


    }



}
