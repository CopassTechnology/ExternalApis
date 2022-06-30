using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ExternalAPIs.BI;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;


namespace ExternalAPIs.DA
{
    public class OneStackDA 
    {

       public static string getoracleconn(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["OrcaleConnectionlive"];
        }
       public static string getApiKey(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["oneStackAPIKey"];
        }
       public static JObject getCustomerFromMobileNo(JObject data)
        {
            string retVal = "";
            //try
            //{
            //    //Data Access Code

            //    String query = "Select num_customer_customerid as CustomerId  from aoup_customer_def where num_customer_cellno=" + mobile_no;
            //    DataTable TblMobile = new DataTable();
            //    Models.GetCon.Query2DataTable.GetResult(TblMobile, query);

            //    if (TblMobile.Rows.Count > 0)
            //    {

            //        mobile_no = Convert.ToString(TblMobile.Rows[0][0]);
            retVal = "Y";
            //    }
            //    else
            //    {
            //        retVal = "N";
            //    }

            //    //retVal = "Yes";

            //}


            //catch
            //{
            //    retVal = "Error";
            //    throw;
            //}


            JObject dboutput = new JObject(
                new JProperty("params",
                new JObject(new JProperty("user_exists", retVal))
                ),
                new JProperty("status", "success")
              );


            return dboutput;
        }

        //onborading
       public static JObject new_user( string mobile_no,string socid, string panno, string dob, string username, string passwd, string debitlastfour, string debitpin, IConfiguration _config)
        {
            string retVal = "";
            string status = "";
            string reg_id = "";
            int ErrCode;
            string ErrMsg = "";
            string utr_no = "";
            string otp = "";

            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                OracleCommand Cmd = new OracleCommand("sp_onestack_reg ", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
               // Cmd.Parameters.Add("in_crn_no", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_mobile_no", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_socid", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_panno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_dob", OracleType.DateTime).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_username", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_passwd", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_debitlast4", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_debitpin", OracleType.Number).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_registration_id", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_otp", OracleType.Number, 6).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 50).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_errorcode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_utr_no", OracleType.Number, 10).Direction = ParameterDirection.Output;
               // Cmd.Parameters["in_crn_no"].Value = crn;
                Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                Cmd.Parameters["in_socid"].Value = socid;
                Cmd.Parameters["in_panno"].Value = panno;
                Cmd.Parameters["in_dob"].Value = dob;
                Cmd.Parameters["in_username"].Value = username;
                Cmd.Parameters["in_passwd"].Value = passwd;
                Cmd.Parameters["in_debitlast4"].Value = debitlastfour;
                Cmd.Parameters["in_debitpin"].Value = debitpin;

                Cmd.ExecuteNonQuery();
                Con.Close();

                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                utr_no = Cmd.Parameters["out_utr_no"].Value.ToString();
                reg_id = Cmd.Parameters["out_registration_id"].Value.ToString();
                otp = Cmd.Parameters["out_otp"].Value.ToString();
                status = "success";

            }
            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }


            string message = "";


            JObject dboutput = new JObject(new JProperty("Param", new JObject(
               new JProperty("param_array",
               new JObject(new JProperty("utr_no", utr_no))
               )
               )),
               new JProperty("reg_id", reg_id),
                new JProperty("message", message),
                new JProperty("otp", otp),
                new JProperty("ErrCode", ErrCode),
                new JProperty("ErrMsg", ErrMsg)
             );
            return dboutput;
        }
        //public static JObject aut_registration_otp(JObject data)
        //{
        //    string retVal = "";
        //    string opt = "1000001";
        //    //try
        //    //{
        //    //    //Data Access Code

        //    string status = "";
        //    string message = "";

        //    status = "success";
        //    message = "Request Accepted";

        //    JObject dboutput = new JObject(
        //        new JProperty("params",
        //        new JObject(new JProperty("customer"), new JObject(new JProperty("customer_token", "eyJraWQiOiJUDzMyW14ze6cgW1S6XHx2kq0Jw"))),
        //        new JObject(new JProperty("utr_no", retVal),
        //        new JProperty("otp", 1000001)
        //        )

        //        ),
        //        new JProperty("status", status),
        //        new JProperty("message", message)
        //      );


        //    return dboutput;
        //}
       public static JObject Userexits(string mobile_no,string socid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            string crn = "";
            string bank_id = "";
            string branch_id = "";

            try
            {
                //DateTime DOB = dob.ToString("dd-MMM-2021");
                string constr = getoracleconn(_config);


                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                OracleCommand Cmd = new OracleCommand("sp_onestack_checkuser", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("in_mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_socid", OracleType.Number).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_user_exists", OracleType.VarChar, 1).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_bankid", OracleType.VarChar, 1).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_branchid", OracleType.VarChar, 1).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_crn", OracleType.VarChar, 1).Direction = ParameterDirection.Output;
                


                Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                Cmd.Parameters["in_socid"].Value = socid;

                Cmd.ExecuteNonQuery();
                Con.Close();

                is_verified = Cmd.Parameters["out_user_exists"].Value.ToString();
                 crn = Cmd.Parameters["out_crn"].Value.ToString();
                 branch_id = Cmd.Parameters["out_branchid"].Value.ToString() ;
                 bank_id = Cmd.Parameters["out_bankid"].Value.ToString() ;
            }

            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }


            JObject responseObj = new JObject(new JProperty("Param", new JObject(
               new JProperty("param_array",
               new JObject(new JProperty("user_exists", is_verified),
               new JProperty("crn" , crn),new JProperty("branch_id" , branch_id),new JProperty("bank_id" ,bank_id))
               )
               )),
               new JProperty("status", "success"),
                new JProperty("ErrCode", ErrCode),
                new JProperty("ErrMsg", ErrMsg)
             );

            return responseObj;
        }
       public static JObject sms_reg(string otp, string mobile_no, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string msg = "success";
            string MsgStatus = "";
            try
            {
                string sms = "" + otp + " is your Mobile login OTP. Do not share it with anyone. Regards CoPASS.";

                string smscount = "1";
                string amt = "1";
                string brid = "1";
                string temp_id = "1007016939687274204";

                JObject Outputdata1 = new JObject();

                string SMSURL = "http://198.15.103.106/API/pushsms.aspx?loginID=copass&password=654321&mobile=" + mobile_no + "&text=" + sms + "&senderid=COPASS&route_id=1&Unicode=0&Template_id=1007016939687274204";

                string responsesms2 = callurl(SMSURL);
                var get_responsesms2 = responsesms2;
                JObject jobject = JObject.Parse(get_responsesms2);
                string Transaction_ID = jobject["Transaction_ID"].ToString();
                MsgStatus = jobject["MsgStatus"].ToString();

                string constr = getoracleconn(_config);


                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                OracleCommand Cmd = new OracleCommand("AOUP_COLLSMS_LOG_INSERT", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("IN_brid", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("sms_tempid", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("smscount", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("msgstatus", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("transaction_id", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("amount", OracleType.Number).Direction = ParameterDirection.Input;


                Cmd.Parameters["IN_brid"].Value = brid;
                Cmd.Parameters["sms_tempid"].Value = temp_id;
                Cmd.Parameters["smscount"].Value = smscount;
                Cmd.Parameters["msgstatus"].Value = MsgStatus;
                Cmd.Parameters["transaction_id"].Value = Transaction_ID;
                Cmd.Parameters["mobile_no"].Value = mobile_no;
                Cmd.Parameters["amount"].Value = amt;

                Cmd.ExecuteNonQuery();
                Con.Close();
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed";
            }

            JObject responseObj = new JObject(new JObject(new JProperty("sendsms_response", new JObject(

                                new JProperty("mobileno", mobile_no),
                                new JProperty("sms", msg),
                                new JProperty("MsgStatus", MsgStatus)

                ))));


            return responseObj;
        }
       public static string callurl(string url)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string urlText = reader.ReadToEnd(); // it takes the response from your url. now you can use as your need  
                return urlText.ToString();
            }
            catch
            {
                throw;
            }
        }

       public static JObject aut_registration_otp(string utr_no, string otp, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string outMsg = "";
            int outerrcd = 0;
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();
                OracleCommand Cmd = new OracleCommand("sp_Authenticate_OTP ", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("in_utr", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_otp", OracleType.Number).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_IS_Verified", OracleType.VarChar, 1).Direction = ParameterDirection.Output;

                Cmd.Parameters["in_utr"].Value = utr_no;
                Cmd.Parameters["in_otp"].Value = otp;


                Cmd.ExecuteNonQuery();
                Con.Close();

                outMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();

            }

            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }
            JObject responseObj = new JObject(new JProperty("Param", new JObject(
               new JProperty("param_array",
               new JObject(new JProperty("status", "success"))
               )
               )),
                new JProperty("ErrCode", ErrCode),
                new JProperty("ErrMsg", outMsg)
             );

            return responseObj;
        }


        //Tokenization

       public static JObject gettoken(string bank_id, string branch_id, string crn, string customer_token, IConfiguration _config)
        {
            string retVal = "";
            string status = "";
            string reg_id = "";
            int ErrCode;
            string ErrMsg = "";
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();
                OracleCommand Cmd = new OracleCommand("SP_TOKEN_REG ", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.Parameters.Add("IN_society_id", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("IN_branch_id", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("IN_crn_no", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("IN_customer_token", OracleType.VarChar, 500).Direction = ParameterDirection.Input;


                Cmd.Parameters.Add("out_status", OracleType.VarChar, 50).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_msg", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                

                Cmd.Parameters["IN_society_id"].Value = bank_id;
                Cmd.Parameters["IN_branch_id"].Value = branch_id;
                Cmd.Parameters["IN_crn_no"].Value = crn;
                Cmd.Parameters["IN_customer_token"].Value = customer_token;

                Cmd.ExecuteNonQuery();
                Con.Close();

                //ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                status = Cmd.Parameters["out_status"].Value.ToString();
                ErrMsg = Cmd.Parameters["out_msg"].Value.ToString();
                
            }
            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }
            string message = "";

            
            JObject dboutput = new JObject(
                new JProperty("params",
                new JObject(new JProperty("customer", retVal))
                ),
                new JProperty("status", status),
                new JProperty("message", ErrMsg)

              );

            return dboutput;

        }

        //Customer Account

       public static JObject accounts_discover(string bank_id, string branch_id, string customer_token, IConfiguration _config)
        {
            string retVal = "";
            string status = "";
            string tokenid = "";
            int ErrCode;
            string ErrMsg = "";
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();
                OracleCommand Cmd = new OracleCommand("sp_onestack_acclist ", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.Parameters.Add("in_bank_id", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_branch_id", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_customer_token", OracleType.VarChar, 500).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_status", OracleType.VarChar, 50).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_msg", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_code", OracleType.Number).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_tokenid", OracleType.VarChar,500).Direction = ParameterDirection.Output;

                Cmd.Parameters["in_bank_id"].Value = bank_id;
                Cmd.Parameters["in_branch_id"].Value = branch_id;
                Cmd.Parameters["in_customer_token"].Value = customer_token;
                

                Cmd.ExecuteNonQuery();
                Con.Close();

                status = Cmd.Parameters["out_status"].Value.ToString();
                ErrMsg = Cmd.Parameters["out_msg"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_code"].Value);
                tokenid = Cmd.Parameters["out_tokenid"].Value.ToString();

            }
            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }
            string message = "";
            JObject dboutput = new JObject(
                new JProperty("params",
                new JObject(new JProperty("customer", retVal))
                ),
                new JProperty("status", status),
                new JProperty("message", ErrMsg),
                new JProperty("code", ErrCode),
                new JProperty("tokenid", tokenid)

              );

            return dboutput;
        }

       public static JObject tokendetails(string token, IConfiguration _config)
        {
            string msg = "";
            JObject Jdata = new JObject();
            string dataobj;
            string ErrMsg = "";
            int ErrCode = 0;
            string Edata = "";
            try
            {
                //DateTime DOB = dob.ToString("dd-MMM-2021");
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                string str = "select account_no,account_type,fi_type,product_no,account_ref_no,account_statuts as account_status,created_date,last_updated,fi_name ";
                str += " from tbl_onestack_acclist where token_id='" + token + "' ";

                DataTable dt = new DataTable();

                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);
                dt.Columns["ACCOUNT_NO"].ColumnName = "account_no";
                dt.Columns["ACCOUNT_TYPE"].ColumnName = "account_type";
                dt.Columns["FI_TYPE"].ColumnName = "fi_type";
                dt.Columns["PRODUCT_NO"].ColumnName = "product_no";
                dt.Columns["ACCOUNT_REF_NO"].ColumnName = "account_ref_no";
                dt.Columns["ACCOUNT_STATUS"].ColumnName = "account_status";
                dt.Columns["CREATED_DATE"].ColumnName = "created_date";
                dt.Columns["LAST_UPDATED"].ColumnName = "last_updated";
                dt.Columns["FI_NAME"].ColumnName = "fi_name";
                if (dt.Rows.Count > 0)
                {
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "accounts");
                }
                else
                {
                    Edata = "no account";
                }

            }
            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }
            JObject Jmain = new JObject(new JObject(new JProperty("param_array", Jdata)));

            string encryptedres = CBSKit.CBSSecurity.Encrypt(Jmain.ToString(), getApiKey(_config));
            JObject responseObj = new JObject();
            if (Edata != "")
            {
                 responseObj = new JObject(new JProperty("Account_response", new JObject(
                                   
                                   new JProperty("ErrCode", "500")
                                   
                   )));
            }
            else
            {
                 responseObj = new JObject(new JProperty("Account_response", new JObject(
                                   new JProperty("data", encryptedres),
                                   new JProperty("ErrCode", ErrCode),
                                   new JProperty("ErrMsg", ErrMsg)
                   )));
            }
            return responseObj;
        }

       public static JObject account_balance(string account_no,string customer_token,  IConfiguration _config)
        {
            string status = "";
            JObject Jdata = new JObject();
            string dataobj;
            string ErrMsg = "";
            int ErrCode = 0;
            string currency = "";
            int bal = 0;

            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();
                OracleCommand Cmd = new OracleCommand("sp_onestack_accbal ", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.Parameters.Add("in_customer_token", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_account_no", OracleType.VarChar, 50).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_status", OracleType.VarChar, 50).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_errmsg", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_errcode", OracleType.Number).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_currency", OracleType.VarChar, 5).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_bal", OracleType.Number).Direction = ParameterDirection.Output;

                Cmd.Parameters["in_customer_token"].Value = customer_token;
                Cmd.Parameters["in_account_no"].Value = account_no;


                Cmd.ExecuteNonQuery();
                Con.Close();

                status = Cmd.Parameters["out_status"].Value.ToString();
                ErrMsg = Cmd.Parameters["out_errmsg"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errcode"].Value);
                currency = Cmd.Parameters["out_currency"].Value.ToString();
                bal = Convert.ToInt32(Cmd.Parameters["out_bal"].Value);

            }
            catch (Exception ex)
            {
                // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

                //goto Exit;
            }
            string message = "";

            


            JObject dboutput = new JObject(new JProperty("Param", new JObject(
               new JProperty("param_array",
               new JObject(new JProperty("current_balance", bal), new JProperty("effective_available_balance", bal), new JProperty("currency", currency))
               )
               ),
               new JObject(new JProperty ("final",new JObject(
              new JProperty("status", status),
               
                new JProperty("message", ErrMsg),
                new JProperty("code", ErrCode)
                
             )))));
            return dboutput;
           
        }

        //Customer Details KYC , By Noor

       public static JObject customer_kyc(string bank_id, string branch_id, string customer_token, string first_name, string last_name,
        string middle_name, DateTime dob, string pan_no, string aadhar_no, string credit_score, string email_id, string mobile_no,
        string address_line_1, string address_line_2, string city, string state, string country, string postal_code, IConfiguration _config)
        {
            string status = "";
            int ErrCode;
            string ErrMsg = "";
            string message = "";
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();
                OracleCommand Cmd = new OracleCommand("sp_onestack_kyc ", Con);

                Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Cmd.Parameters.Add("in_bank_id", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_branch_id", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_customer_token", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_first_name", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_last_name", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_middle_name", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_dob", OracleType.DateTime).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_pan_no", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_aadhar_no", OracleType.Number, 20).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_credit_score", OracleType.Number, 20).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_email_id", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_mobile_no", OracleType.Number, 50).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_address_line_1", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_address_line_2", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_city", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_state", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_country", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_postal_code", OracleType.Number, 10).Direction = ParameterDirection.Input;


                Cmd.Parameters.Add("out_status", OracleType.VarChar, 50).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_msg", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_code", OracleType.Number).Direction = ParameterDirection.Output;

                //take the input parameters from sp
                Cmd.Parameters["in_bank_id"].Value = bank_id;
                Cmd.Parameters["in_branch_id"].Value = branch_id;
                Cmd.Parameters["in_customer_token"].Value = customer_token;
                Cmd.Parameters["in_first_name"].Value = first_name;

                Cmd.Parameters["in_last_name"].Value = last_name;
                Cmd.Parameters["in_middle_name"].Value = middle_name;
                Cmd.Parameters["in_dob"].Value = dob;

                Cmd.Parameters["in_pan_no"].Value = pan_no;
                Cmd.Parameters["in_aadhar_no"].Value = aadhar_no;
                Cmd.Parameters["in_credit_score"].Value = credit_score;
                Cmd.Parameters["in_email_id"].Value = email_id;

                Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                Cmd.Parameters["in_address_line_1"].Value = address_line_1;
                Cmd.Parameters["in_address_line_2"].Value = address_line_2;
                Cmd.Parameters["in_city"].Value = city;

                Cmd.Parameters["in_state"].Value = state;
                Cmd.Parameters["in_country"].Value = country;
                Cmd.Parameters["in_postal_code"].Value = postal_code;

                Cmd.ExecuteNonQuery();
                Con.Close();

                status = Cmd.Parameters["out_status"].Value.ToString();
                ErrMsg = Cmd.Parameters["out_msg"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_code"].Value);

            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            JObject dboutput = new JObject(
                new JProperty("ErrCode", ErrCode),
                new JProperty("status", status),
                new JProperty("message", ErrMsg)
             );
            return dboutput;
        }

       public static JObject getcustomer(string bank_id, string branch_id, string token, IConfiguration _config)//
        {
            string ErrMsg = "";
            int ErrCode = 0;

            JObject Jdata = new JObject();
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                //string str = " select first_name,last_name,middle_name,dob,pan_no,aadhar_no,credit_score,email_id,mobile_no,address_line_1,address_line_2,city,state,country,postal_code  ";
                //str += " from tbl_onestack_kyc where token_id ='" + token + "' ";

                string str = " select var_customer_fstname as First_name, var_customer_lstname  as Last_name ";
                str += " from aoup_customer_def cd ";
                str += " inner join tbl_token_master ttm on ttm.mobile_no = cd.num_customer_cellno ";
                str += " where cd.num_customer_brid = '" + branch_id + "' and ttm.token_id = '" + token + "' ";

                DataTable dt = new DataTable();
                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                // call all the dbs parameter s in JProperty 

                if (dt.Rows.Count > 0)
                {
                   // Jdata = new JObject(
                   //new JProperty("first_name", dt.Rows[0]["First_name"].ToString()),
                   //new JProperty("last_name", dt.Rows[0]["Last_name"].ToString()),
                   //new JProperty("middle_name", dt.Rows[0]["middle_name"].ToString()),
                   //new JProperty("dob", dt.Rows[0]["dob"].ToString()),
                   //new JProperty("pan_no", dt.Rows[0]["pan_no"].ToString()),
                   //new JProperty("aadhar_no", dt.Rows[0]["aadhar_no"].ToString()),
                   //new JProperty("credit_score", dt.Rows[0]["credit_score"].ToString()),
                   //new JProperty("email_id", dt.Rows[0]["email_id"].ToString()),
                   //new JProperty("mobile_no", dt.Rows[0]["mobile_no"].ToString()),
                   //new JProperty("address_line_1", dt.Rows[0]["address_line_1"].ToString()),
                   //new JProperty("address_line_2", dt.Rows[0]["address_line_2"].ToString()),
                   //new JProperty("city", dt.Rows[0]["city"].ToString()),
                   //new JProperty("state", dt.Rows[0]["state"].ToString()),
                   //new JProperty("country", dt.Rows[0]["country"].ToString()),
                   // new JProperty("postal_code", dt.Rows[0]["postal_code"].ToString())
                   //);

                    Jdata = new JObject(
                   new JProperty("first_name", dt.Rows[0]["First_name"].ToString()),
                   new JProperty("last_name", dt.Rows[0]["Last_name"].ToString()),
                   new JProperty("middle_name",""),
                   new JProperty("dob", ""),
                   new JProperty("pan_no", ""),
                   new JProperty("aadhar_no",""),
                   new JProperty("credit_score", ""),
                   new JProperty("email_id",""),
                   new JProperty("mobile_no", ""),
                   new JProperty("address_line_1", ""),
                   new JProperty("address_line_2",""),
                   new JProperty("city", ""),
                   new JProperty("state",""),
                   new JProperty("country", ""),
                    new JProperty("postal_code",""));
                }
                else
                {
                    Jdata = new JObject(
                       new JProperty("first_name", ""),
                       new JProperty("last_name", ""),
                       new JProperty("middle_name", ""),
                       new JProperty("dob", ""),
                       new JProperty("pan_no", ""),
                       new JProperty("aadhar_no", ""),
                       new JProperty("credit_score", ""),
                       new JProperty("email_id", ""),
                       new JProperty("mobile_no", ""),
                       new JProperty("address_line_1", ""),
                       new JProperty("address_line_2", ""),
                       new JProperty("city", ""),
                       new JProperty("state", ""),
                       new JProperty("country", ""),
                        new JProperty("postal_code", "")
                       );
                }
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            JObject resObj = new JObject(
            // new JProperty("param_array",(new JObject(new JProperty("data", Jdata)))));
            new JProperty("param_array", Jdata)); // fetch the all parameter store in Jdata


            string encryptedres = CBSKit.CBSSecurity.Encrypt(resObj.ToString(), getApiKey(_config)); // encrypt the data 

            JObject responseObj = new JObject(new JProperty("data", encryptedres));
            return responseObj;// return data in JObject
        }

       // mini statment.
       public static JObject mini_statment(string account_no, IConfiguration _config)
        {
            
            string ErrMsg = "";
            int ErrCode = 0;
            string jsonstr = "";
            DataTable dt = new DataTable();
            JObject Jdata = new JObject();
            JObject resObj = new JObject();
            JObject responseObj = new JObject();
            DataTable dt1 = new DataTable();
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                string str = " Select trunc(trnsdate) trnsdate,(amt*100) amt,crdr,narration from ( ";
                str += " SELECT date_depotrns_trnsdate trnsdate, num_depotrns_trnsno trnsno, ";
                str += " var_trnstype_trnstype trnstype, num_depotrns_docno docno,num_depotrns_chqno chqno, ";
                //str += " CASE WHEN num_depotrns_amount >= 0 THEN num_depotrns_amount ELSE 0 END credit, ";
                //str += " CASE WHEN num_depotrns_amount < 0 THEN num_depotrns_amount ELSE 0 END debit, ";
                str += " Case when num_depotrns_amount <0 then num_depotrns_amount * -1 else num_depotrns_amount end amt, 0 balance, CASE WHEN num_depotrns_amount >= 0 THEN 'cr' ";
                str += " WHEN num_depotrns_amount < 0 THEN 'dr' ELSE '' END crdr, var_depotrns_narration narration ";
                str += " FROM aoup_depotrns_def p ";
                str += " INNER JOIN aoup_trnstype_def t ON p.num_depotrns_trnstypeid = t.num_trnstype_trnstypeid ";
                str += " INNER JOIN glview gv on p.num_depotrns_glcode=gv.glcode where gv.glsubtypeid=101 ";
                str += " AND num_depotrns_accno = '" + account_no + "'  UNION ALL ";
                str += " SELECT trnsdate, trnsno, var_trnstype_trnstype trnstype, docno, chqno, ";
                //str += " CASE WHEN amount >= 0 THEN amount ELSE 0 END credit, ";
                //str += " CASE WHEN amount < 0 THEN amount ELSE 0 END debit,
                str += " case when amount  < 0 then amount * -1 else amount end amt ,0 balance, CASE WHEN amount >= 0 THEN 'cr' WHEN amount < 0 THEN 'dr' ELSE '' END  crdr, narration ";
                str += " FROM daybook p INNER JOIN aoup_trnstype_def t ON p.trnstypeid = t.num_trnstype_trnstypeid ";
                str += " where   accno = '" + account_no + "' and rownum <=10 ORDER BY trnsdate desc) ";
                str += " where rownum <=10 ";

                
                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);
                dt.Columns["trnsdate"].ColumnName = "txn_date";
                dt.Columns["narration"].ColumnName = "txn_remark";
                dt.Columns["crdr"].ColumnName = "txn_type";
                dt.Columns["amt"].ColumnName = "amount";

                string constr1 = getoracleconn(_config);
                OracleConnection Con1 = new OracleConnection(constr1);
                Con1.Open();

                string str1 = " SELECT num_accbalance_currentbal as BAL FROM aoup_accbalance_def WHERE num_accbalance_accno = '" + account_no + "' ";

                OracleCommand Cmd1 = new OracleCommand(str1, Con1);
                OracleDataAdapter AdpData1 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd1;
                AdpData1.Fill(dt1);


            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            // JObject resObj = new JObject(
            //new JProperty("param_array", Jdata));

            // JObject responseObj = new JObject(new JProperty("data", Jdata));
            if (dt.Rows.Count > 0 )
            {
                responseObj = new JObject(
                    new JProperty("account_no", account_no),
                    new JProperty("available_balance", dt1.Rows[0]["BAl"].ToString()),
                    new JProperty("currency", "INR"),
                    new JProperty("cheques_in_clearing", ""),
                    new JProperty("effective_available_balance", ""),
                     new JProperty("transactions", JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dt, Formatting.Indented))));

                resObj = new JObject(
                new JProperty("param_array", responseObj),new JProperty("Error_code",500));
            }
            else
            {
                resObj = new JObject(
                new JProperty("param_array", ""), new JProperty("Error_code", 100));
            }
            return resObj;

        }

       public static JObject transactions_list(string account_no, DateTime txn_date_from, DateTime txn_date_to, IConfiguration _config)
        {

            string ErrMsg = "";
            int ErrCode = 0;
            string jsonstr = "";
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            JObject Jdata = new JObject();
            JObject resObj = new JObject();
            JObject responseObj = new JObject();
            string txn_date_from1 = txn_date_from.ToString("dd-MMM-yyyy");
            string txn_date_to1 = txn_date_to.ToString("dd-MMM-yyyy");
            try
            {
               


                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                string str = " Select TRUNC (trnsdate) value_date, trunc(trnsdate) trnsdate,(amt*100) amt,crdr,narration,chqno from ( ";
                str += " SELECT date_depotrns_trnsdate trnsdate, num_depotrns_trnsno trnsno, ";
                str += " var_trnstype_trnstype trnstype, num_depotrns_docno docno,num_depotrns_chqno chqno, ";
                //str += " CASE WHEN num_depotrns_amount >= 0 THEN num_depotrns_amount ELSE 0 END credit, ";
                //str += " CASE WHEN num_depotrns_amount < 0 THEN num_depotrns_amount ELSE 0 END debit, ";
                str += " Case when num_depotrns_amount <0 then num_depotrns_amount * -1 else num_depotrns_amount end amt, 0 balance, CASE WHEN num_depotrns_amount >= 0 THEN 'cr' ";
                str += " WHEN num_depotrns_amount < 0 THEN 'dr' ELSE '' END crdr, var_depotrns_narration narration ";
                str += " FROM aoup_depotrns_def p ";
                str += " INNER JOIN aoup_trnstype_def t ON p.num_depotrns_trnstypeid = t.num_trnstype_trnstypeid ";
                str += " INNER JOIN glview gv on p.num_depotrns_glcode=gv.glcode where gv.glsubtypeid=101 ";
                str += " AND num_depotrns_accno = '" + account_no + "' AND date_depotrns_trnsdate >= '" + txn_date_from1 + "'  AND date_depotrns_trnsdate <= '" + txn_date_to1 + "' ";
                str +=    " UNION ALL ";
                str += " SELECT trnsdate, trnsno, var_trnstype_trnstype trnstype, docno, chqno, ";
                //str += " CASE WHEN amount >= 0 THEN amount ELSE 0 END credit, ";
                //str += " CASE WHEN amount < 0 THEN amount ELSE 0 END debit,
                str += " case when amount  < 0 then amount * -1 else amount end amt,0 balance, CASE WHEN amount >= 0 THEN 'cr' WHEN amount < 0 THEN 'dr' ELSE '' END  crdr, narration ";
                str += " FROM daybook p INNER JOIN aoup_trnstype_def t ON p.trnstypeid = t.num_trnstype_trnstypeid ";
                str += " AND trnsdate >= '" + txn_date_from1 + "' AND trnsdate <= '" + txn_date_to1 + "' ";
                str += " where   accno = '" + account_no + "' and rownum <=10 ORDER BY trnsdate desc) ";
                str += " where rownum <=10 ";


                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);
                dt.Columns["value_date"].ColumnName = "value_date";
                dt.Columns["trnsdate"].ColumnName = "txn_date";
                dt.Columns["narration"].ColumnName = "txn_remark";
                dt.Columns["crdr"].ColumnName = "txn_type";
                dt.Columns["amt"].ColumnName = "amount";
                dt.Columns["chqno"].ColumnName = "cheque_no";

                string constr1 = getoracleconn(_config);
                OracleConnection Con1 = new OracleConnection(constr1);
                Con1.Open();

                string str1 = " SELECT num_accbalance_currentbal as BAL FROM aoup_accbalance_def WHERE num_accbalance_accno = '" + account_no + "' ";

                OracleCommand Cmd1 = new OracleCommand(str1, Con1);
                OracleDataAdapter AdpData1 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd1;
                AdpData1.Fill(dt1);


            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            // JObject resObj = new JObject(
            //new JProperty("param_array", Jdata));

            // JObject responseObj = new JObject(new JProperty("data", Jdata));
            if (dt.Rows.Count > 0)
            {
                responseObj = new JObject(
                    new JProperty("account_no", account_no),
                    new JProperty("available_balance", dt1.Rows[0]["BAl"].ToString()),
                    new JProperty("currency", "INR"),
                    new JProperty("cheques_in_clearing", ""),
                    new JProperty("effective_available_balance", ""),
                    new JProperty("txn_date_from", txn_date_from),
                    new JProperty("txn_date_to" , txn_date_to),
                     new JProperty("transactions", JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dt, Formatting.Indented))));

                resObj = new JObject(
                new JProperty("param_array", responseObj), new JProperty("Error_code", 500));
            }
            else
            {
                resObj = new JObject(
                new JProperty("param_array", ""), new JProperty("Error_code", 100));
            }
            return resObj;

        }

        //upi payments
        public static JObject initiateUpi(string bank_id, string branch_id, string customer_token, string account_no, string receiver_upi_id, string receiver_name,
             string sender_upi_id, string amount, string upi_transaction_date, string upi_transaction_no, string settlement_status, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            //string customer_token = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            string status = "";
            string message = "";
            DateTime? somedate = null;
            JObject responseObj = new JObject();
            try
            {

                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                ////Call Procedure
                OracleCommand Cmd = new OracleCommand("SP_ONESTACK_UPITRANS", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;


                Cmd.Parameters.Add("in_flag", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_token_id", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_acc_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_soc_id", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_branch_id", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_receiver_upi_id", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_receiver_name", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_sender_upi_id", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_upi_transaction_date", OracleType.DateTime).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_upi_transaction_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_settlement_status", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_settlement_utr_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_utr_transaction_date", OracleType.DateTime).Direction = ParameterDirection.Input;


                Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;


                Cmd.Parameters["in_flag"].Value = "insert";
                Cmd.Parameters["in_token_id"].Value = customer_token;
                Cmd.Parameters["in_acc_no"].Value = account_no;
                Cmd.Parameters["in_soc_id"].Value = bank_id;
                Cmd.Parameters["in_branch_id"].Value = branch_id;
                Cmd.Parameters["in_receiver_upi_id"].Value = receiver_upi_id;
                Cmd.Parameters["in_receiver_name"].Value = receiver_name;
                Cmd.Parameters["in_sender_upi_id"].Value = sender_upi_id;
                Cmd.Parameters["in_amount"].Value = amount;
                Cmd.Parameters["in_upi_transaction_date"].Value = upi_transaction_date;
                Cmd.Parameters["in_upi_transaction_no"].Value = upi_transaction_no;
                Cmd.Parameters["in_settlement_status"].Value = settlement_status;
                Cmd.Parameters["in_settlement_utr_no"].Value = "0";
                Cmd.Parameters["in_utr_transaction_date"].Value = somedate;



                Cmd.ExecuteNonQuery();
                Con.Close();

                ErrCode = Convert.ToInt16(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();

                if (ErrCode == 200)
                {
                    status = "success";
                    message = "Record inserted successfully";
                    responseObj = new JObject(new JProperty("initiateUpi", new JObject(
                                       new JProperty("status", status),
                                       new JProperty("message", message)
                       )));
                }
                else
                {
                    status = "failed";
                    message = "Something went wrong please contact support team";
                    responseObj = new JObject(new JProperty("initiateUpi", new JObject(
                                       new JProperty("status", status),
                                       new JProperty("message", message)
                       )));
                }
            }


            catch (Exception ex)
            {

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

            }
            return responseObj;
        }

        public static JObject SettlementUPI(string bank_id, string branch_id, string upi_transaction_no,
            string settlement_utr_no, string settlement_status, string utr_transaction_date,
            string amount,
            IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            //string customer_token = "";
            string status = "";
            string message = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();

            try
            {
                //string customer_token = "sl4pBlE9hR204StswZHQ2JuqapvG";

                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();


                OracleCommand Cmd = new OracleCommand("SP_ONESTACK_UPITRANS", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("in_flag", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_token_id", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_acc_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_soc_id", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_branch_id", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_receiver_upi_id", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_receiver_name", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_sender_upi_id", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_upi_transaction_date", OracleType.DateTime).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_upi_transaction_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_settlement_status", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_settlement_utr_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_utr_transaction_date", OracleType.DateTime).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;


                Cmd.Parameters["in_flag"].Value = "Update";
                Cmd.Parameters["in_token_id"].Value = "";
                Cmd.Parameters["in_acc_no"].Value = "";
                Cmd.Parameters["in_soc_id"].Value = bank_id;
                Cmd.Parameters["in_branch_id"].Value = branch_id;
                Cmd.Parameters["in_receiver_upi_id"].Value = "";
                Cmd.Parameters["in_receiver_name"].Value = "";
                Cmd.Parameters["in_sender_upi_id"].Value = "";
                Cmd.Parameters["in_upi_transaction_no"].Value = upi_transaction_no;
                Cmd.Parameters["in_settlement_utr_no"].Value = settlement_utr_no;
                Cmd.Parameters["in_settlement_status"].Value = settlement_status;
                Cmd.Parameters["in_utr_transaction_date"].Value = utr_transaction_date;
                Cmd.Parameters["in_amount"].Value = amount;


                Cmd.ExecuteNonQuery();
                Con.Close();

                ErrCode = Convert.ToInt16(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();

                if (ErrCode == 300)
                {
                    status = "success";
                    message = "Record updated successfully";
                    responseObj = new JObject(new JProperty("SettlementUPI", new JObject(
                                       new JProperty("status", status),
                                       new JProperty("message", message)
                       )));
                }
                else
                {
                    status = "failed";
                    message = "Something went wrong please contact support team";
                    responseObj = new JObject(new JProperty("initiateUpi", new JObject(
                                       new JProperty("status", status),
                                       new JProperty("message", message)
                       )));
                }

            }


            catch (Exception ex)
            {

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

            }




            return responseObj;


            //catch (Exception ex)
            //{
            //    // writeLogCoPASSMobileApp("Error Text : " + ex.Message + ", Request : " + Request);

            //    ErrCode = 0;
            //    ErrMsg = "Unable to proceed - webserve";

            //    //goto Exit;
            //}


            //JObject responseObj = new JObject(new JProperty("Param", new JObject(
            //   new JProperty("param_array",
            //   new JObject(new JProperty("user_exists", is_verified),
            //   new JProperty("crn", crn), new JProperty("branch_id", branch_id), new JProperty("bank_id", bank_id))
            //   )
            //   )),
            //   new JProperty("status", "success"),
            //    new JProperty("ErrCode", ErrCode),
            //    new JProperty("ErrMsg", ErrMsg)
            // );

            //return responseObj;





        }


        public static JObject UPIStatusCheck(string bank_id, string branch_id, string customer_token,
            string upi_transaction_no, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            //string customer_token = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            string status = "";
            string message = "";
            JObject responseObj = new JObject();
            try
            {
                //customer_token = "sl4pBlE9hR204StswZHQ2JuqapvG";

                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();


                string str = "select upi_transaction_no,settlement_utr_no,settlement_status,utr_transaction_date  ";
                str += " from tbl_onestack_upitrans where ";
                str += "token_id= '" + customer_token + "' and upi_transaction_no='" + upi_transaction_no + "' ";


                DataTable dt = new DataTable();
                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    status = "success";
                    message = "UTR status fetch successfully";
                    Jdata = new JObject(
                   new JProperty("upi_transaction_no", dt.Rows[0]["upi_transaction_no"].ToString()),
                   new JProperty("settlement_utr_no", dt.Rows[0]["settlement_utr_no"].ToString()),
                   new JProperty("settlement_status", dt.Rows[0]["settlement_status"].ToString()),
                   new JProperty("utr_transaction_date", dt.Rows[0]["utr_transaction_date"].ToString()));

                    responseObj = new JObject(
                                    new JProperty("status", status),
                                    new JProperty("message", message),
                                    new JProperty("param_array", Jdata)
                        );
                }
                else
                {
                    status = "failed";
                    message = "Something went wrong please contact support team";

                    responseObj = new JObject(
                                  new JProperty("status", status),
                                  new JProperty("message", message)
                      //new JProperty("param_array", Jdata)
                      );

                }





            }


            catch (Exception ex)
            {

                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

            }




            return responseObj;
        }

    }
}
