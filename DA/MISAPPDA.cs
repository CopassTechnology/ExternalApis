using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using System.Xml.Serialization;
using ExternalAPIs.BI;
using System.Collections;

namespace ExternalAPIs.DA
{
    public class MISAPPDA
    {
        // oracle connection
        public static string getoracleconn(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["OrcaleConnectionlive"];
        }
        public static OracleConnection getoracleOpenConn(IConfiguration _config)
        {
            string conStr = _config.GetSection("Appsetting")["OrcaleConnectionlive"];
            OracleConnection Con = new OracleConnection(conStr);
            if (Con.State != ConnectionState.Open)
            {
                Con.Open();
            }

            return Con;
        }
        public static OracleConnection getoracleCloseConn(IConfiguration _config)
        {
            string conStr = _config.GetSection("Appsetting")["OrcaleConnectionlive"];
            OracleConnection Con = new OracleConnection(conStr);

            return Con;
        }
        public static JObject handleErrorData(int ErrCode, string ErrMsg)
        {
            JObject Outputdata = new JObject(new JProperty("ErrCode", ErrCode),
                new JProperty("ErrMsg", ErrMsg));
            return Outputdata;
        }
        public static JObject getData(JObject data)
        {
            string retval = "";
            JObject dboutput = new JObject();
            try
            {
                retval = "Operation Done";

                string status = "";
                string message = "";

                dboutput = new JObject(
                    new JProperty("params",
                    new JObject(new JProperty("utr_no", retval))
                    ),
                    new JProperty("status", status),
                     new JProperty("message", message)
                  );

            }
            catch
            {
                throw;
            }

            return dboutput;

        }

        //login process
        public static JObject MobileUserLogin(string mobile_no, string password,string hoid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            string name = "";
            int soc_id =0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_login", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_loginpin", OracleType.VarChar,100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_socid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_IS_Verified", OracleType.LongVarChar, 10).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_name", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_soc_id", OracleType.VarChar, 100).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_loginpin"].Value = password;
                    Cmd.Parameters["in_socid"].Value = hoid;


                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                is_verified = Cmd.Parameters["out_IS_Verified"].Value.ToString();
                name = Cmd.Parameters["out_name"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                soc_id = Convert.ToInt32(Cmd.Parameters["out_soc_id"].Value);

                responseObj = new JObject(new JObject(new JProperty("register_response", new JObject(
                               new JProperty("mobile_no", mobile_no),
                               new JProperty("IS_verified", is_verified),
                               new JProperty("Name", name),
                               new JProperty("soc_id", soc_id),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));

            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);

            }
            return responseObj;
        }
        //public static JObject MobileUserLogin_verify(string mobile_no,string encrypted_pass IConfiguration _config)
        //{
        //    string retVal = "";
        //    string is_verified = "";
        //    string username = "";
        //    int OTP = 0;
        //    string ErrMsg = "";
        //    int ErrCode = 0;
        //    string name = "";
            
        //    JObject responseObj = new JObject();
        //    try
        //    {


        //        using (OracleConnection Con = getoracleOpenConn(_config))
        //        {
        //            string str = " select c.fullname, mr.mobile_no,mr.product_code from mobile_registration mr ";
        //            str += " inner join customerview c on mr.mobile_no = c.mobile and mr.product_code=c.hoid ";
        //            str += " where mr.mobile_no = " + mobile_no + "  ";

        //            DataTable dt = new DataTable();

        //            OracleCommand Cmd = new OracleCommand(str, Con);
        //            OracleDataAdapter AdpData = new OracleDataAdapter();
        //            AdpData.SelectCommand = Cmd;
        //            AdpData.Fill(dt);

        //            getoracleCloseConn(_config);

        //            string password = dt.Rows[0]["login_pin"].ToString();
                    

        //            if ( password != "")
        //            {
                        

        //                bool verify = ExternalAPIs.Shared.PasswordHasher.VerifyPasswordAES(password, encrypted_pass);

        //                if (verify == true)
        //                {
        //                    string soc_id = dt.Rows[0]["product_code"].ToString();
        //                    name = dt.Rows[0]["fullname"].ToString();
        //                    responseObj = new JObject(new JObject(new JProperty("register_response", new JObject(
        //                           new JProperty("mobile_no", mobile_no),
        //                           new JProperty("IS_verified", "Y"),
        //                           new JProperty("Name", name),
        //                           new JProperty("soc_id", soc_id),
        //                           new JProperty("ErrCode", ErrCode),
        //                           new JProperty("ErrMsg", ErrMsg)
        //                             ))));
        //                }
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrCode = 0;
        //        ErrMsg = "Unable to proceed - webserve";
        //        responseObj = handleErrorData(ErrCode, ErrMsg);

        //    }
        //    return responseObj;
        //}
        public static JObject MobileUserRegister(string mobile_no, string IMEI_NO,string society_id ,IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int hoid = 0;
            string flag = "register";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_appregister", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_imeino", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_society_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_limit", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_username", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_IS_Verified", OracleType.VarChar, 10).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_otp", OracleType.Number, 6).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_name", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_hoid", OracleType.Number).Direction = ParameterDirection.Output;


                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_imeino"].Value = IMEI_NO;
                    Cmd.Parameters["in_society_id"].Value = society_id;
                    Cmd.Parameters["in_flag"].Value = flag;
                    Cmd.Parameters["in_limit"].Value = 0;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                username = Cmd.Parameters["out_username"].Value.ToString();
                is_verified = Cmd.Parameters["out_IS_Verified"].Value.ToString();
                OTP = Convert.ToInt32(Cmd.Parameters["out_otp"].Value);
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                hoid = Convert.ToInt32(Cmd.Parameters["out_hoid"].Value);

                 responseObj = new JObject(new JObject(new JProperty("register_response", new JObject(
                                new JProperty("mobile_no", mobile_no),
                                new JProperty("username", username),
                                new JProperty("IS_verified", is_verified),
                                new JProperty("OTP", OTP),
                                new JProperty("Hoid", hoid),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;
        }
        public static JObject Mobileverifyotp(string mobile_no, string otp,string hoid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_otp_verification", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_otp", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_socid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_IS_Verified", OracleType.VarChar, 10).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_otp"].Value = otp;
                    Cmd.Parameters["in_socid"].Value = hoid;
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                is_verified = Cmd.Parameters["out_IS_Verified"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();

                 responseObj = new JObject(new JObject(new JProperty("otp_response", new JObject(
                                new JProperty("mobile_no", mobile_no),
                                new JProperty("IS_verified", is_verified),

                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";  //goto Exit;
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            
            return responseObj;
        }
        public static JObject Resend_OTP(string mobile_no,string hoid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_ResendOTP", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_socid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_otp", OracleType.Number, 6).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_IS_Verified", OracleType.VarChar, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_socid"].Value = hoid;
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                OTP = Convert.ToInt32(Cmd.Parameters["out_otp"].Value);
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                is_verified = Cmd.Parameters["out_IS_Verified"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("resendotp_response", new JObject(
                                new JProperty("mobile_no", mobile_no),
                                new JProperty("OTP", OTP),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg),
                                  new JProperty("is_verified", is_verified)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            
            return responseObj;
        }
        public static JObject loG_PIN(string mobile_no, string pin,string hoid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            string out_pin = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SP_LOGINPINGEN", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_loginpin", OracleType.VarChar,100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_socid", OracleType.VarChar,100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_pin", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_IS_Verified", OracleType.VarChar, 10).Direction = ParameterDirection.Output;
                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_loginpin"].Value = pin;
                    Cmd.Parameters["in_socid"].Value = hoid;
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                out_pin = Cmd.Parameters["out_pin"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                is_verified = Cmd.Parameters["out_IS_Verified"].Value.ToString();


                string pin_data = ExternalAPIs.Helper.encr.DecryptStringAES(out_pin, null);
                responseObj = new JObject(new JObject(new JProperty("loginpin_response", new JObject(
                                new JProperty("mobile_no", mobile_no),
                                new JProperty("pin", pin_data),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg),
                                new JProperty("is_verified", is_verified)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            
            return responseObj;
        }
        public static JObject Reset_pin(string mobile_no, string password,string hoid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                Cmd = new OracleCommand("sp_resetpin", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("in_mobileno", OracleType.Number, 10).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_password", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_socid", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                Cmd.Parameters["in_mobileno"].Value = mobile_no;
                Cmd.Parameters["in_password"].Value = password;
                Cmd.Parameters["in_socid"].Value = hoid;
                Cmd.ExecuteNonQuery();
                getoracleCloseConn(_config);

                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("Resetpin_response", new JObject(
                               new JProperty("mobile_no", mobile_no),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)

               ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

           
            return responseObj;
        }
        public static JObject MobileUserverify(string mobile_no,string hoid, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string count = "";
            string Name = "";
            int OTP = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                        Cmd = new OracleCommand("sp_user_verify", Con);
                        Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("in_socid", OracleType.Number).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                        Cmd.Parameters.Add("out_errorcode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                        Cmd.Parameters.Add("out_otp", OracleType.Number, 10).Direction = ParameterDirection.Output;

                        Cmd.Parameters["in_mobileno"].Value = mobile_no;
                        Cmd.Parameters["in_socid"].Value = hoid;

                        Cmd.ExecuteNonQuery();
                        getoracleCloseConn(_config);
                    }
                    ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                    ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                    OTP = Convert.ToInt32(Cmd.Parameters["out_otp"].Value.ToString());
                   
                    if (OTP > 0)
                    {
                    responseObj = new JObject(new JObject(new JProperty("user_response", new JObject(
                              new JProperty("ErrCode", ErrCode),
                              new JProperty("ErrMsg", ErrMsg),
                              new JProperty("OTP", OTP)

                    ))));
                }
                    else
                    {
                    responseObj = new JObject(new JObject(new JProperty("user_response", new JObject(
                          new JProperty("ErrCode", ErrCode),
                          new JProperty("ErrMsg", ErrMsg),
                          new JProperty("OTP", 0)

                ))));
                }
                
            }
            
                catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

        
            return responseObj;
        }

        //account list
        public static JObject getaccount_list(string mobile_no,string society_id, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            string dataobj;
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleCloseConn(_config))
                {
                    //string str = "SELECT A.num_accmast_brid AS BRID,A.var_accmast_accname as Name, B.BRNAME AS BRANCH_NAME, A.num_accmast_glcode AS GLOCDE,TO_CHAR(A.num_accmast_accno) AS ";
                    //str += "ACC_NO,D.num_accbalance_currentbal AS ACC_BAL,D.date_accbalance_lasttrnsdate AS     , ";
                    //str += " NVL(is_active, 0) AS IS_ACTIVE, mid AS MID, is_disable AS IS_DISABLE FROM aoup_accmast_def A ";
                    //str += " INNER JOIN branchlist B ON A.num_accmast_brid = B.brid ";
                    //str += "  INNER JOIN aoup_glheadmast_def E ON E.num_glheadmast_brid = B.hoid AND E.num_glheadmast_glcode = A.num_accmast_glcode ";
                    //str += "  INNER JOIN aoup_glsubtypemaster_def F ON F.num_glsubtypemst_glsubtypeid = E.num_glheadmast_glsubtypeid AND  F.num_glsubtypemst_glsubtypeid in (101,112) ";
                    //str += "  INNER JOIN customerview C ON A.num_accmast_custinternalid = C.internalid ";
                    //str += "  INNER JOIN aoup_accbalance_def D ON A.num_accmast_accno = D.num_accbalance_accno ";
                    //str += "  LEFT JOIN  mobile_user_accounts G ON A.num_accmast_accno = G.accno ";
                    //str += "  WHERE C.mobile = '" + mobile_no + "' and b.hoid=" + society_id + " and a.date_accmast_closedate is null ";

                    string str = "select br_id as BRID ,saving_accname as Name ,gl_code as GLOCDE,to_char(saving_accno) as ACC_NO,saving_curbal as ACC_BAL ";
                    str += " from vsavingviewformobile where cell_no = " + mobile_no + " and ho_id = " + society_id + "  ";
                    DataTable dt = new DataTable();
                    DataTable dt1 = new DataTable();

                    string str1 = " select trans_limit from mobile_registration where mobile_no = "+ mobile_no +" and product_code="+ society_id +" ";
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleCommand Cmd1 = new OracleCommand(str1, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    OracleDataAdapter AdpData1 = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData1.SelectCommand = Cmd1;
                    AdpData.Fill(dt);
                    AdpData1.Fill(dt1);
                    getoracleCloseConn(_config);

                   

                    if (dt.Rows.Count > 0  || dt1.Rows.Count > 0)
                    {
                        Jdata = BI.Common.Convert_DataTableToJSON(dt, "Account_Details");
                        responseObj = new JObject(new JObject(new JProperty("Account_response", new JObject(
                               new JProperty("data", Jdata),
                               new JProperty("trans_limit", dt1.Rows[0]["trans_limit"].ToString()),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg),
                               new JProperty("is_verified", is_verified)
               ))));
                    }
                    else
                    {
                        ErrCode = 1000;
                        ErrMsg = "Unable to get data. Please try again later";
                        responseObj = handleErrorData(ErrCode, ErrMsg);
                    }
                }
                 
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;
        }

        //beneficiary process
        public static JObject Add_Beneficiary(string mobile_no, string hoid, string BID, string accno, string acctype, string ownneft, string accname, string nickname, string bankname, string bankbranch, string ifsccode, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SP_beneficiary_action", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_BID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accno", OracleType.Number, 6).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_acctype", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ownneft", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_nickname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_branchname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ifsccode", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_Flag", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_BID", OracleType.Number, 5).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                    Cmd.Parameters["in_hoid"].Value = hoid;
                    Cmd.Parameters["in_BID"].Value = BID;
                    Cmd.Parameters["in_accno"].Value = accno;
                    Cmd.Parameters["in_acctype"].Value = acctype;
                    Cmd.Parameters["in_ownneft"].Value = ownneft;
                    Cmd.Parameters["in_accname"].Value = accname;
                    Cmd.Parameters["in_nickname"].Value = nickname;
                    Cmd.Parameters["in_bankname"].Value = bankname;
                    Cmd.Parameters["in_branchname"].Value = bankbranch;
                    Cmd.Parameters["in_ifsccode"].Value = ifsccode;
                    Cmd.Parameters["in_Flag"].Value = "INSERT";

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                BID = Cmd.Parameters["out_BID"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("Beneficiary_response", new JObject(
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)

                ))));
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            

            return responseObj;
        }
        public static JObject Update_Beneficiary(string mobile_no, string hoid, string BID, string accno, string acctype, string accname, string nickname, string bankname, string bankbranch, string ifsccode, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            string ownneft = "1";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SP_beneficiary_action", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_BID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accno", OracleType.Number, 6).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_acctype", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ownneft", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_nickname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_branchname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ifsccode", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_Flag", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_BID", OracleType.Number, 5).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                    Cmd.Parameters["in_hoid"].Value = hoid;
                    Cmd.Parameters["in_BID"].Value = BID;
                    Cmd.Parameters["in_accno"].Value = accno;
                    Cmd.Parameters["in_acctype"].Value = acctype;
                    Cmd.Parameters["in_ownneft"].Value = ownneft;
                    Cmd.Parameters["in_accname"].Value = accname;
                    Cmd.Parameters["in_nickname"].Value = nickname;
                    Cmd.Parameters["in_bankname"].Value = bankname;
                    Cmd.Parameters["in_branchname"].Value = bankbranch;
                    Cmd.Parameters["in_ifsccode"].Value = ifsccode;
                    Cmd.Parameters["in_Flag"].Value = "UPDATE";

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                BID = Cmd.Parameters["out_BID"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("Beneficiary_response", new JObject(
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)
                ))));
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            


            return responseObj;
        }
        public static JObject Remove_Beneficiary(string mobile_no, string hoid, string BID, string accno, string acctype, string accname, string nickname, string bankname, string bankbranch, string ifsccode, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            string ownneft = "1";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SP_beneficiary_action", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_BID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accno", OracleType.Number, 6).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_acctype", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ownneft", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_nickname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_branchname", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ifsccode", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_Flag", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_BID", OracleType.Number, 5).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                    Cmd.Parameters["in_hoid"].Value = hoid;
                    Cmd.Parameters["in_BID"].Value = BID;
                    Cmd.Parameters["in_accno"].Value = accno;
                    Cmd.Parameters["in_acctype"].Value = acctype;
                    Cmd.Parameters["in_ownneft"].Value = ownneft;
                    Cmd.Parameters["in_accname"].Value = accname;
                    Cmd.Parameters["in_nickname"].Value = nickname;
                    Cmd.Parameters["in_bankname"].Value = bankname;
                    Cmd.Parameters["in_branchname"].Value = bankbranch;
                    Cmd.Parameters["in_ifsccode"].Value = ifsccode;
                    Cmd.Parameters["in_Flag"].Value = "DELETE";

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                BID = Cmd.Parameters["out_BID"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("Beneficiary_response", new JObject(
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)))));
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            

            return responseObj;
        }
        public static JObject getBeneficiary_list(string mobile_no,string socid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            string dataobj;
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    //string str = "select b_id,to_char(acc_no) as accno,acc_type as acctype, acc_name as accname,  bank_name as bankname  , ifsc_code as ifsccode , bank_branch_name as branchname,nick_name as nickname  ";
                    //str += " from beneficiary_details ";
                    //str += " where muid=(select mid from mobile_registration where mobile_no=" + mobile_no + " and product_code =" + socid + ") and is_active=1 and active_time < sysdate and soc_id = " + socid + "";

                    string str = " select accno, acctype, accname, bankname, ifsccode, branchname, nickname from vbeneficiariesformobile where soc_id = "+ socid +" and mobile_no = "+ mobile_no +" and active_status = 'ACTIVE' " ;

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);

                    if (dt.Rows.Count > 0)
                    {
                        Jdata = BI.Common.Convert_DataTableToJSON(dt, "Beneficiary_Details");

                        responseObj = new JObject(new JObject(new JProperty("Beneficiary_response", new JObject(
                                     new JProperty("data", Jdata),
                                     new JProperty("ErrCode", ErrCode),
                                     new JProperty("ErrMsg", ErrMsg),
                                     new JProperty("is_verified", is_verified)
                     ))));
                    }
                    else
                    {
                        ErrCode = 1000;
                        ErrMsg = "Unable to get data. Please try again later";
                        responseObj = handleErrorData(ErrCode, ErrMsg);
                    }
                }
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

           
            return responseObj;
        }
        public static JObject allBeneficiary_list(string mobile_no,string socid, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            string dataobj;
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    //string str = "select b_id,to_char(acc_no) as accno,acc_type as acctype, acc_name as accname,  bank_name as bankname  , ifsc_code as ifsccode , bank_branch_name as branchname,nick_name as nickname, ";
                    //str += " case when active_time < sysdate then 'ACTIVE' ";
                    //str += " when active_time > sysdate then 'IN-ACTIVE' ";
                    //str += " end as status ";
                    //str += " from beneficiary_details ";
                    //str += " where muid=(select mid from mobile_registration where mobile_no=" + mobile_no + " and product_code = " + socid + ") ";

                    string str = " select accno,acctype,accname,bankname,ifsccode,branchname,nickname,active_status as status from vbeneficiariesformobile where soc_id = " + socid +" and mobile_no="+ mobile_no +"  ";

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);
                    if (dt.Rows.Count > 0)
                    {
                        Jdata = BI.Common.Convert_DataTableToJSON(dt, "Beneficiary_Details");

                        responseObj = new JObject(new JObject(new JProperty("Beneficiary_response", new JObject(
                                      new JProperty("data", Jdata),
                                      new JProperty("ErrCode", ErrCode),
                                      new JProperty("ErrMsg", ErrMsg),
                                      new JProperty("is_verified", is_verified)
                      ))));
                    }
                    else
                    {
                        ErrCode = 1000;
                        ErrMsg = "Unable to get data. Please try again later";
                        responseObj = handleErrorData(ErrCode, ErrMsg);
                    }
                }
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;
        }

        //society register
        public static JObject Add_Society(string hoid, string icici_ACC, string icici_ifsc, string icici_branch, string bankid, string approvalamt, string acc_prefix, string beneficiary_limit, 
            string upi_merchant_id, string bankglcode, string bankacno, string bankchrgincomeglcode, string bankchrgincomeacno, string bankchrgexpenseglcode, string bankchrgexpenseacno, string user_level, string approve_limit,string TRANS_PREFIX,string DAILYTRANSLIMIT, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SOC_REG_NEFT", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 10).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_ACC", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_ifsc", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_branch", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bank_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_approval_amt", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_acc_prefix", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_beneficiary_limit", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_upi_merchant_id", OracleType.VarChar,20).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("in_bankglcode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankacno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgincomeglcode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgincomeacno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgexpenseglcode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgexpenseacno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_user_level", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_approve_limit", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_TRANS_PREFIX", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_DAILYTRANSLIMIT", OracleType.Number).Direction = ParameterDirection.Input;


                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_uid_code", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_is_disable", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_hoid"].Value = hoid;
                    Cmd.Parameters["in_flag"].Value = "INSERT";
                    Cmd.Parameters["in_icici_ACC"].Value = icici_ACC;
                    Cmd.Parameters["in_icici_ifsc"].Value = icici_ifsc;
                    Cmd.Parameters["in_icici_branch"].Value = icici_branch;
                    Cmd.Parameters["in_bank_id"].Value = bankid;
                    Cmd.Parameters["in_approval_amt"].Value = approvalamt;
                    Cmd.Parameters["in_acc_prefix"].Value = acc_prefix;
                    Cmd.Parameters["in_beneficiary_limit"].Value = beneficiary_limit;
                    Cmd.Parameters["in_upi_merchant_id"].Value = upi_merchant_id;

                    Cmd.Parameters["in_bankglcode"].Value = bankglcode;
                    Cmd.Parameters["in_bankacno"].Value = bankacno;
                    Cmd.Parameters["in_bankchrgincomeglcode"].Value = bankchrgincomeglcode;
                    Cmd.Parameters["in_bankchrgincomeacno"].Value = bankchrgincomeacno;
                    Cmd.Parameters["in_bankchrgexpenseglcode"].Value = bankchrgexpenseglcode;
                    Cmd.Parameters["in_bankchrgexpenseacno"].Value = bankchrgexpenseacno;
                    Cmd.Parameters["in_user_level"].Value = user_level;
                    Cmd.Parameters["in_approve_limit"].Value = approve_limit;
                    Cmd.Parameters["in_TRANS_PREFIX"].Value = TRANS_PREFIX;
                    Cmd.Parameters["in_DAILYTRANSLIMIT"].Value = DAILYTRANSLIMIT;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                out_uid_code = Convert.ToInt32(Cmd.Parameters["out_uid_code"].Value);
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                is_disable = Convert.ToInt32(Cmd.Parameters["out_is_disable"].Value.ToString());

                 responseObj = new JObject(new JObject(new JProperty("Society_response", new JObject(
                                new JProperty("hoid", hoid),
                                new JProperty("icici_ACC", icici_ACC),
                                new JProperty("icici_ifsc", icici_ifsc),
                                new JProperty("icici_branch", icici_branch),
                                new JProperty("in_bank_id", bankid),
                                new JProperty("in_approval_amt", approvalamt),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg),
                                new JProperty("onboard_reg_id", out_uid_code)
                ))));
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            


            return responseObj;
        }
        public static JObject Update_Society(string hoid, string icici_ACC, string icici_ifsc, string icici_branch, string bankid, string approvalamt, string acc_prefix, string beneficiary_limit,
            string upi_merchant_id, string bankglcode, string bankacno, string bankchrgincomeglcode, string bankchrgincomeacno, string bankchrgexpenseglcode, string bankchrgexpenseacno, string user_level, string approve_limit, string TRANS_PREFIX,string DAILYTRANSLIMIT, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SOC_REG_NEFT", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 10).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_ACC", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_ifsc", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_branch", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bank_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_approval_amt", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_acc_prefix", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_beneficiary_limit", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_upi_merchant_id", OracleType.VarChar, 20).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("in_bankglcode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankacno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgincomeglcode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgincomeacno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgexpenseglcode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bankchrgexpenseacno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_user_level", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_approve_limit", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_TRANS_PREFIX", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_DAILYTRANSLIMIT", OracleType.Number).Direction = ParameterDirection.Input;


                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_uid_code", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_is_disable", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_hoid"].Value = hoid;
                    Cmd.Parameters["in_flag"].Value = "UPDATE";
                    Cmd.Parameters["in_icici_ACC"].Value = icici_ACC;
                    Cmd.Parameters["in_icici_ifsc"].Value = icici_ifsc;
                    Cmd.Parameters["in_icici_branch"].Value = icici_branch;
                    Cmd.Parameters["in_bank_id"].Value = bankid;
                    Cmd.Parameters["in_approval_amt"].Value = approvalamt;
                    Cmd.Parameters["in_acc_prefix"].Value = acc_prefix;
                    Cmd.Parameters["in_beneficiary_limit"].Value = beneficiary_limit;
                    Cmd.Parameters["in_upi_merchant_id"].Value = upi_merchant_id;

                    Cmd.Parameters["in_bankglcode"].Value = bankglcode;
                    Cmd.Parameters["in_bankacno"].Value = bankacno;
                    Cmd.Parameters["in_bankchrgincomeglcode"].Value = bankchrgincomeglcode;
                    Cmd.Parameters["in_bankchrgincomeacno"].Value = bankchrgincomeacno;
                    Cmd.Parameters["in_bankchrgexpenseglcode"].Value = bankchrgexpenseglcode;
                    Cmd.Parameters["in_bankchrgexpenseacno"].Value = bankchrgexpenseacno;
                    Cmd.Parameters["in_user_level"].Value = user_level;
                    Cmd.Parameters["in_approve_limit"].Value = approve_limit;
                    Cmd.Parameters["in_TRANS_PREFIX"].Value = TRANS_PREFIX;
                    Cmd.Parameters["in_DAILYTRANSLIMIT"].Value = DAILYTRANSLIMIT;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                out_uid_code = Convert.ToInt32(Cmd.Parameters["out_uid_code"].Value);
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                is_disable = Convert.ToInt32(Cmd.Parameters["out_is_disable"].Value.ToString());

                 responseObj = new JObject(new JObject(new JProperty("Society_response", new JObject(
                                new JProperty("hoid", hoid),
                                new JProperty("icici_ACC", icici_ACC),
                                new JProperty("icici_ifsc", icici_ifsc),
                                new JProperty("icici_branch", icici_branch),
                                new JProperty("in_bank_id", bankid),
                                new JProperty("in_approval_amt", approvalamt),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg),
                                new JProperty("onboard_reg_id", out_uid_code)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            


            return responseObj;
        }
        public static JObject Delete_Society(string hoid, string icici_ACC, string icici_ifsc, string icici_branch, string bankid, string approvalamt, string acc_prefix, string beneficiary_limit,
            string upi_merchant_id, string bankglcode, string bankacno, string bankchrgincomeglcode, string bankchrgincomeacno, string bankchrgexpenseglcode, string bankchrgexpenseacno, string user_level, string approve_limit, string TRANS_PREFIX,string DAILYTRANSLIMIT, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SOC_REG_ICICI", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 10).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_ACC", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_ifsc", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_icici_branch", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_acc_prefix", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_beneficiary_limit", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_upi_merchant_id", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_uid_code", OracleType.VarChar, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_is_disable", OracleType.Number, 5).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_hoid"].Value = hoid;
                    Cmd.Parameters["in_flag"].Value = "DELETE";
                    Cmd.Parameters["in_icici_ACC"].Value = icici_ACC;
                    Cmd.Parameters["in_icici_ifsc"].Value = icici_ifsc;
                    Cmd.Parameters["in_icici_branch"].Value = icici_branch;
                    Cmd.Parameters["in_approval_amt"].Value = approvalamt;
                    Cmd.Parameters["in_acc_prefix"].Value = acc_prefix;
                    Cmd.Parameters["in_beneficiary_limit"].Value = beneficiary_limit;
                    Cmd.Parameters["in_upi_merchant_id"].Value = upi_merchant_id;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                out_uid_code = Convert.ToInt32(Cmd.Parameters["out_out_uid_code"].Value);
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                is_disable = Convert.ToInt32(Cmd.Parameters["out_is_disable"].Value.ToString());

                 responseObj = new JObject(new JObject(new JProperty("Society_response", new JObject(
                                new JProperty("hoid", hoid),
                                new JProperty("icici_ACC", icici_ACC),
                                new JProperty("icici_ifsc", icici_ACC),
                                new JProperty("icici_branch", icici_branch),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg),
                                new JProperty("is_disable", is_disable)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            


            return responseObj;
        }
        public static JObject Society_Details(string Soci_id, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int out_pin = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            string dataobj;
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "SELECT A.num_accmast_brid AS BRID,B.society AS SOCIETY_NAME, B.BRNAME AS BRANCH_NAME, A.num_accmast_glcode AS GLOCDE,A.num_accmast_accno AS ";
                    str += "  WHERE C.mobile = '" + Soci_id + "' ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Society_Details");
                }
                 responseObj = new JObject(new JObject(new JProperty("Society_response", new JObject(
                               new JProperty("data", Jdata),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)

               ))));
            }


            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);

            }

            
            return responseObj;
        }

        //trans hit..
        public static JObject Society_NEFT_Transaction(string hoid, string flag, string mobileno, string glcode, string acc, string amount, string benbeficiary_accno, string benbeficiary_bank, string benbeficiary_ifsccode, string trns_type, string remark,string imei_no, string ip_add,string location,string ben_name, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            int req_id = 0;
            string icici_ACC = "";
            string icici_ifsc = "";
            string icici_branch = "";
            string BENEFICIARY_NAME = "";
            JObject responseObj = new JObject();
            try
            {
                //using (OracleConnection Con = getoracleOpenConn(_config))
                //{
                //    string str = "  SELECT acc_name as ACC_NAME FROM beneficiary_details M WHERE M.acc_no= '"+ benbeficiary_accno + "' AND  muid = (SELECT MID FROM mobile_registration WHERE mobile_no= "+ mobileno + ")  ";

                //    DataTable dt = new DataTable();
                //    OracleCommand Cmd1 = new OracleCommand(str, Con);
                //    OracleDataAdapter AdpData = new OracleDataAdapter();
                //    AdpData.SelectCommand = Cmd1;
                //    AdpData.Fill(dt);
                //    Cmd1.ExecuteNonQuery();
                //    getoracleCloseConn(_config);
                //    BENEFICIARY_NAME = dt.Rows[0]["ACC_NAME"].ToString();
                //}

                //Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  : " + BENEFICIARY_NAME);

                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_trans_InsUpd", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_origin", OracleType.VarChar, 50).Direction = ParameterDirection.Input;//M
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;
                    
                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_glcode", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accno", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_benbeficiary_accno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_benbeficiary_bank", OracleType.VarChar, 2000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_benbeficiary_ifsccode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_beneficiary_name", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_type", OracleType.VarChar, 300).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("remark1", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_imei_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ip_add", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_location", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_userid", OracleType.VarChar, 200).Direction = ParameterDirection.Input;//mobile no // webuser id

                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_req_id", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_flag"].Value = flag;
                    Cmd.Parameters["in_trans_origin"].Value = "M";
                    Cmd.Parameters["in_hoid"].Value = hoid;

                    Cmd.Parameters["in_mobileno"].Value = mobileno;
                    Cmd.Parameters["in_glcode"].Value = Convert.ToInt16(glcode);
                    Cmd.Parameters["in_accno"].Value = acc;
                    Cmd.Parameters["in_amount"].Value = Convert.ToInt16(amount);
                    Cmd.Parameters["in_benbeficiary_accno"].Value = benbeficiary_accno;
                    Cmd.Parameters["in_benbeficiary_bank"].Value = benbeficiary_bank;
                    Cmd.Parameters["in_benbeficiary_ifsccode"].Value = benbeficiary_ifsccode;
                    Cmd.Parameters["in_beneficiary_name"].Value = ben_name;
                    Cmd.Parameters["in_trans_type"].Value = trns_type;
                    Cmd.Parameters["remark1"].Value = remark;
                    Cmd.Parameters["in_imei_no"].Value = imei_no;
                    Cmd.Parameters["in_ip_add"].Value = ip_add;
                    Cmd.Parameters["in_location"].Value = location;
                    Cmd.Parameters["in_userid"].Value = mobileno;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                req_id = Convert.ToInt32(Cmd.Parameters["out_req_id"].Value);

                if (ErrCode == -100)
                {
                    responseObj = new JObject(new JObject(new JProperty("trans_response", new JObject(
                                   new JProperty("flag", "success"),
                                   new JProperty("reqid", req_id),
                                   new JProperty("ErrCode", ErrCode),
                                   new JProperty("ErrMsg", ErrMsg)

                   ))));
                }
                else
                {
                    responseObj = new JObject(new JObject(new JProperty("trans_response", new JObject(
                                   new JProperty("flag", "failed"),
                                   new JProperty("reqid", req_id),
                                   new JProperty("ErrCode", ErrCode),
                                   new JProperty("ErrMsg", ErrMsg)

                   ))));
                }
            }
            catch (Exception ex)
            {
                Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "Request Data Status  : " + ex);
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;
        }
        public static JObject Society_Transaction(string hoid,string flag, string mobileno, string glcode, string acc, string amount, string benbeficiary_accno, string benbeficiary_bank, string benbeficiary_ifsccode,string benbeficiary_name, string trns_type, string remark,string userid, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;

            string icici_ACC = "";
            string icici_ifsc = "";
            string icici_branch = "";

            string imei_no = "0";
            string ip_add = "0";
            string location = "0";
            int req_id = 0;
            JObject responseObj = new JObject();
            try
            {
                //DateTime DOB = dob.ToString("dd-MMM-2021");

                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_trans_InsUpd", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_origin", OracleType.VarChar, 50).Direction = ParameterDirection.Input;//M
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_glcode", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accno", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_benbeficiary_accno", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_benbeficiary_bank", OracleType.VarChar, 2000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_benbeficiary_ifsccode", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_beneficiary_name", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_type", OracleType.VarChar, 300).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("remark1", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_imei_no", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ip_add", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_location", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_userid", OracleType.VarChar, 200).Direction = ParameterDirection.Input;//mobile no // webuser id

                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_req_id", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_flag"].Value = flag;
                    Cmd.Parameters["in_trans_origin"].Value = "W";
                    Cmd.Parameters["in_hoid"].Value = hoid;

                    Cmd.Parameters["in_mobileno"].Value = mobileno;
                    Cmd.Parameters["in_glcode"].Value = Convert.ToInt16(glcode);
                    Cmd.Parameters["in_accno"].Value = acc;
                    Cmd.Parameters["in_amount"].Value = Convert.ToInt16(amount);
                    Cmd.Parameters["in_benbeficiary_accno"].Value = benbeficiary_accno;
                    Cmd.Parameters["in_benbeficiary_bank"].Value = benbeficiary_bank;
                    Cmd.Parameters["in_benbeficiary_ifsccode"].Value = benbeficiary_ifsccode;
                    Cmd.Parameters["in_beneficiary_name"].Value = benbeficiary_name;
                    Cmd.Parameters["in_trans_type"].Value = trns_type;
                    Cmd.Parameters["remark1"].Value = remark;
                    Cmd.Parameters["in_imei_no"].Value = "0";
                    Cmd.Parameters["in_ip_add"].Value = "0";
                    Cmd.Parameters["in_location"].Value = "0";
                    Cmd.Parameters["in_userid"].Value = userid;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                req_id = Convert.ToInt32(Cmd.Parameters["out_req_id"].Value);

                responseObj = new JObject(new JObject(new JProperty("Trans_response", new JObject(
                               new JProperty("mobileno", mobileno),
                               new JProperty("req_id", req_id),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)

               ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }




            return responseObj;
        }

        //sms process 
        public static JObject Send_sms(string mobile_no, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            string icici_ACC = "";
            string icici_ifsc = "";
            string icici_branch = "";
            string BRID = "";
            string mobno = "";
            string ACCNO = "";
            string OTP = "";
            string sms_senderID = "";

            try
            {
                string SMSURL = getsmsurl(BRID, mobno, ACCNO, OTP, sms_senderID, _config);

            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            JObject responseObj = new JObject(new JObject(new JProperty("Trans_response", new JObject(
                                new JProperty("mobileno", mobile_no),
                                new JProperty("icici_ACC", icici_ACC),
                                new JProperty("icici_ifsc", icici_ACC),
                                new JProperty("icici_branch", icici_branch),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)

                ))));

            return responseObj;
        } // not working
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
        public static string getsmsurl(string BRID, string mobno, string ACCNO, string OTP, string sms_senderID, IConfiguration _config)
        {
            string SMSURL = "";
            string ErrMsg = "";
            int ErrCode = 0;
            try
            {
                using (OracleConnection Con = getoracleCloseConn(_config))
                {
                    string Query = " SELECT brid, url,loginid,text1,password,text2,mobileno, text3,accno, text4,amt,text5,senderid,text6, templateid ";
                    Query += " FROM aoup_collsms_config WHERE brid='" + BRID + "' ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(Query, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    string urlT = dt.Rows[0]["url"].ToString();
                    string loginidT = dt.Rows[0]["loginid"].ToString();
                    string text1T = dt.Rows[0]["text1"].ToString();
                    string passwordT = dt.Rows[0]["password"].ToString();
                    string text2T = dt.Rows[0]["text2"].ToString();
                    string mobilenoT = mobno;
                    string text3T = dt.Rows[0]["text3"].ToString();
                    string accnoT = ACCNO;
                    string text4T = dt.Rows[0]["text4"].ToString();
                    string amtT = OTP.ToString();
                    string text5T = dt.Rows[0]["text5"].ToString();
                    string senderidT = sms_senderID;
                    string text6T = dt.Rows[0]["text6"].ToString();
                    string templateidT = dt.Rows[0]["templateid"].ToString();

                    SMSURL = urlT + loginidT + text1T + passwordT + text2T + mobilenoT + text3T + " " + accnoT + " " + text4T + " " + amtT + " " + text5T + senderidT + text6T + templateidT;
                }
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }
            return SMSURL;
        }
        public static JObject sms_reg(string otp, string mobile_no, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string msg = "success";
            JObject responseObj = new JObject();
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
                String Transaction_ID = jobject["Transaction_ID"].ToString();
                String MsgStatus = jobject["MsgStatus"].ToString();

                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("AOUP_COLLSMS_LOG_INSERT", Con);
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
                    getoracleCloseConn(_config);
                }
                responseObj = new JObject(new JObject(new JProperty("sendsms_response", new JObject(
                               new JProperty("mobileno", mobile_no),
                               new JProperty("sms", msg)
               ))));
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            return responseObj;
        }
        public static JObject sms_trans(string reqid, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string msg = "success";
            JObject Jdata = new JObject();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select s.ho_id as HOID, s.sender_id as senderid, s.username as username , s.password as password ";
                    str += "from tbl_sms_config s ";
                    str += " inner join transfer_trans_details t on s.ho_id=t.ho_id where t.requid='" + reqid + "' ";


                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                //end data 1

                //start data 2
                using (OracleConnection Con1 = getoracleOpenConn(_config))
                {
                    string ctr = " select ins_by as mobileno,amount as amount,holder_accno as accno  from transfer_trans_details m where m.requid='" + reqid + "' ";

                    OracleCommand amd = new OracleCommand(ctr, Con1);
                    OracleDataAdapter Adpdata1 = new OracleDataAdapter();
                    Adpdata1.SelectCommand = amd;
                    Adpdata1.Fill(dt1);
                    amd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }

                //end data 2

                string amount = dt1.Rows[0]["amount"].ToString();
                string mobile_no = dt1.Rows[0]["mobileno"].ToString();
                string acc_no = dt1.Rows[0]["accno"].ToString();


                string username = dt.Rows[0]["username"].ToString();
                string password = dt.Rows[0]["password"].ToString();
                string senderid = dt.Rows[0]["senderid"].ToString();

                string sms = "	Dear Customer, You have made a payment of Rs. " + amount + " using NEFT via IMPS from your Account " + acc_no + " with reference number " + reqid + ". Regards Sakhi";
                string smsrul = "http://103.15.179.45:8449/MessagingGateway/SendTransSMS?Username=" + username + "&Password=" + password + "&MessageType=txt&Mobile=" + mobile_no + "&SenderID=" + senderid + "&Message=" + sms + "";
                string responsesms2 = callurl(smsrul);
                var get_responsesms2 = responsesms2;
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }


            JObject responseObj = new JObject(new JObject(new JProperty("sendsms_response", new JObject(

                                new JProperty("mobileno", ""),
                                new JProperty("sms", msg)

                ))));
            return responseObj;
        }
        public static JObject SendSMS_onreg(string IN_brid, string sms_tempid, string smscount, string msgstatus, string transaction_id, string mobile_no, string amount, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string msg = "success";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("AOUP_COLLSMS_LOG_INSERT", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("IN_brid", OracleType.Number, 10).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("sms_tempid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("smscount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("msgstatus", OracleType.VarChar).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("transaction_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("amount", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters["IN_brid"].Value = IN_brid;
                    Cmd.Parameters["sms_tempid"].Value = sms_tempid;
                    Cmd.Parameters["smscount"].Value = smscount;
                    Cmd.Parameters["msgstatus"].Value = msgstatus;
                    Cmd.Parameters["transaction_id"].Value = transaction_id;
                    Cmd.Parameters["mobile_no"].Value = mobile_no;
                    Cmd.Parameters["amount"].Value = amount;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                }
                responseObj = new JObject(new JObject(new JProperty("sendsms_response", new JObject(

                               new JProperty("mobileno", mobile_no),
                               new JProperty("sms", msg),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg))
               )));
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }



            return responseObj;
        }
        public static JObject sms_trans_otp(string Flag,string hoid,string mobileno,int otp,string acc_no,string bene_name,string amount, IConfiguration _config)
        {

            string ErrMsg = "";
            int ErrCode = 0;
            string msg = "success";
            JObject Jdata = new JObject();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            JObject responseObj = new JObject();
            string requid = "0";
            try
            {
                if (Flag == "Trans_OTP")
                {
                    using (OracleConnection Con = getoracleOpenConn(_config))
                    {
                        string str = " select tsc.sender_id as senderid,tsc.username as username,tsc.password as password,nst.template as temp ";
                        str += " from NEFT_SMS_TEMP nst ";
                        str += "inner join tbl_sms_config tsc on nst.hoid = tsc.ho_id ";
                        str += " where hoid = " + hoid + " and nst.temp_id = 1 ";

                        OracleCommand Cmd = new OracleCommand(str, Con);
                        OracleDataAdapter AdpData = new OracleDataAdapter();
                        AdpData.SelectCommand = Cmd;
                        AdpData.Fill(dt);
                        Cmd.ExecuteNonQuery();
                        getoracleCloseConn(_config);
                    }
                    //end data 1

                    //start data 2
                    using (OracleConnection Con1 = getoracleOpenConn(_config))
                    {

                        string ctr = " select requid as requid from(select * from transfer_trans_def t ";
                        ctr += " inner join mobile_registration m on t.mid = m.mid ";
                        ctr += " where m.mobile_no = " + mobileno + " and trunc(t.trans_date) = trunc(sysdate) ";
                        ctr += " order by trans_date  desc)where rownum = 1 ";

                        OracleCommand amd = new OracleCommand(ctr, Con1);
                        OracleDataAdapter Adpdata1 = new OracleDataAdapter();
                        Adpdata1.SelectCommand = amd;
                        Adpdata1.Fill(dt1);
                        amd.ExecuteNonQuery();
                        getoracleCloseConn(_config);
                    }

                    //end data 2
                    if (dt1.Rows.Count == 0)
                    {
                        requid = "0";
                    }
                    else
                    {
                        requid = dt1.Rows[0]["requid"].ToString();
                    }
                    
                    string username = dt.Rows[0]["username"].ToString();
                    string password = dt.Rows[0]["password"].ToString();
                    string senderid = dt.Rows[0]["senderid"].ToString();

                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + requid+ username+ password+ senderid);
                    // string sms = "	Dear Customer, You have made a payment of Rs. " + amount + " using NEFT via IMPS from your Account " + acc_no + " with reference number " + requid + ". Regards Sakhi";
                    string temp = dt.Rows[0]["temp"].ToString();
                    string[] result = temp.Split("{#var#}");
                    string smsstr = "";

                    for (int i = 1; i < result.Length; i++)
                    {
                        result[i] = "{#var#}" + result[i];
                        result[i] = result[i].Replace("{#var#}", "{#var#}" + i.ToString());
                        smsstr = smsstr + result[i];
                    }
                    smsstr = result[0] + smsstr;
                    smsstr = smsstr.Replace("{#var#}1", otp.ToString()).Replace("{#var#}2", amount).Replace("{#var#}3", acc_no).Replace("{#var#}4", bene_name).Replace("{#var#}5", requid);
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + smsstr);
                    string smsrul = "http://103.15.179.45:8449/MessagingGateway/SendTransSMS?Username=" + username + "&Password=" + password + "&MessageType=txt&Mobile=" + mobileno + "&SenderID=" + senderid + "&Message=" + smsstr + "";
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + smsrul);
                    string responsesms2 = callurl(smsrul);
                    var get_responsesms2 = responsesms2;
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + responsesms2);
                     
                }
                else if (Flag == "Ben_OTP")
                {
                    using (OracleConnection Con = getoracleOpenConn(_config))
                    {
                        string str = " select tsc.sender_id as senderid,tsc.username as username,tsc.password as password,nst.template as temp ";
                        str += " from NEFT_SMS_TEMP nst ";
                        str += "inner join tbl_sms_config tsc on nst.hoid = tsc.ho_id ";
                        str += " where hoid = " + hoid + " and nst.temp_id = 4 ";

                        OracleCommand Cmd = new OracleCommand(str, Con);
                        OracleDataAdapter AdpData = new OracleDataAdapter();
                        AdpData.SelectCommand = Cmd;
                        AdpData.Fill(dt);
                        Cmd.ExecuteNonQuery();
                        getoracleCloseConn(_config);
                    }
                    //end data 1

                    string username = dt.Rows[0]["username"].ToString();
                    string password = dt.Rows[0]["password"].ToString();
                    string senderid = dt.Rows[0]["senderid"].ToString();

                    // string sms = "	Dear Customer, You have made a payment of Rs. " + amount + " using NEFT via IMPS from your Account " + acc_no + " with reference number " + requid + ". Regards Sakhi";
                    string temp = dt.Rows[0]["temp"].ToString();
                    string[] result = temp.Split("{#var#}");
                    string smsstr = "";

                    for (int i = 1; i < result.Length; i++)
                    {
                        result[i] = "{#var#}" + result[i];
                        result[i] = result[i].Replace("{#var#}", "{#var#}" + i.ToString());
                        smsstr = smsstr + result[i];
                    }
                    smsstr = result[0] + smsstr;
                    smsstr = smsstr.Replace("{#var#}1", otp.ToString()).Replace("{#var#}2", bene_name);
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + smsstr);
                    string smsrul = "http://103.15.179.45:8449/MessagingGateway/SendTransSMS?Username=" + username + "&Password=" + password + "&MessageType=txt&Mobile=" + mobileno + "&SenderID=" + senderid + "&Message=" + smsstr + "";
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + smsrul);
                    string responsesms2 = callurl(smsrul);
                    var get_responsesms2 = responsesms2;
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + responsesms2);

                }
                else if (Flag == "Limit_OTP")
                {
                    using (OracleConnection Con = getoracleOpenConn(_config))
                    {
                        string str = " select tsc.sender_id as senderid,tsc.username as username,tsc.password as password,nst.template as temp ";
                        str += " from NEFT_SMS_TEMP nst ";
                        str += "inner join tbl_sms_config tsc on nst.hoid = tsc.ho_id ";
                        str += " where hoid = " + hoid + " and nst.temp_id = 5 ";

                        OracleCommand Cmd = new OracleCommand(str, Con);
                        OracleDataAdapter AdpData = new OracleDataAdapter();
                        AdpData.SelectCommand = Cmd;
                        AdpData.Fill(dt);
                        Cmd.ExecuteNonQuery();
                        getoracleCloseConn(_config);
                    }
                    //end data 1


                    string username = dt.Rows[0]["username"].ToString();
                    string password = dt.Rows[0]["password"].ToString();
                    string senderid = dt.Rows[0]["senderid"].ToString();

                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + requid + username + password + senderid);
                    // string sms = "	Dear Customer, You have made a payment of Rs. " + amount + " using NEFT via IMPS from your Account " + acc_no + " with reference number " + requid + ". Regards Sakhi";
                    string temp = dt.Rows[0]["temp"].ToString();
                    string[] result = temp.Split("{#var#}");
                    string smsstr = "";

                    for (int i = 1; i < result.Length; i++)
                    {
                        result[i] = "{#var#}" + result[i];
                        result[i] = result[i].Replace("{#var#}", "{#var#}" + i.ToString());
                        smsstr = smsstr + result[i];
                    }
                    smsstr = result[0] + smsstr;
                    smsstr = smsstr.Replace("{#var#}1", otp.ToString());
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + smsstr);
                    string smsrul = "http://103.15.179.45:8449/MessagingGateway/SendTransSMS?Username=" + username + "&Password=" + password + "&MessageType=txt&Mobile=" + mobileno + "&SenderID=" + senderid + "&Message=" + smsstr + "";
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + smsrul);
                    string responsesms2 = callurl(smsrul);
                    var get_responsesms2 = responsesms2;
                    Common.write_log_Success("UPIOutward.aspx | iciciReq Method Response |", "First data  : " + responsesms2);

                }
            }

            
            
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            responseObj = new JObject(new JObject(new JProperty("sendsms_response", new JObject(

                                new JProperty("mobileno", mobileno),
                                new JProperty("sms", msg),
                                new JProperty("OTP", otp)

                ))));

            return responseObj;
        }
        
        //list for account balance,pending trans,mini_statement
        public static JObject getPendingList(string Status, string socid, IConfiguration _config)
        {
            string retVal = "";
            JObject Jdata = new JObject();
            string ErrMsg = "";
            int ErrCode = 0;
            string dataobj;
            string is_verified = "";
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    

                    string str = "  select na.reqid,na.t_id,na.name,na.h_glcode,na.h_acc,na.b_acc,na.b_bank,na.b_ifsc,na.amount,ac.custinternalid,ttd.level1_approval,ttd.level2_approval,ttd.level3_approval  ";
                    str += " from neft_approval na  ";
                    str += " inner join transfer_trans_def ttd on ttd.requid = na.reqid  ";
                    str += " inner join accountview ac on ttd.branch_accno = ac.accno  ";
                    str += " where trans_status = '" + Status + "' and ttd.hoid = " + socid + " order by  na.ins_date   ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Pending_Details");


                }
                 responseObj = new JObject(new JObject(new JProperty("Beneficiary_response", new JObject(
                              new JProperty("data", Jdata),
                              new JProperty("ErrCode", ErrCode),
                              new JProperty("ErrMsg", ErrMsg),
                              new JProperty("is_verified", is_verified)
              ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

           
            return responseObj;
        }
        public static JObject getAcc_bal(string accno, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string bal = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "SELECT num_accbalance_currentbal as BAL FROM aoup_accbalance_def WHERE num_accbalance_accno=" + accno + " ";


                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                    bal = dt.Rows[0]["BAl"].ToString();
                }
                 responseObj = new JObject(
                              new JProperty("Balance", bal));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
           


            return responseObj;
        }
        public static JObject mini_statment(string account_no, IConfiguration _config)
        {

            string ErrMsg = "";
            int ErrCode = 0;
            string jsonstr = "";
            DataTable dt = new DataTable();
            JObject Jdata = new JObject();
            JObject responseObj1 = new JObject();
            JObject responseObj = new JObject();
            DataTable dt1 = new DataTable();

            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {


                    //string str = " Select trunc(trnsdate) trnsdate, trnsno, trnstype, docno, chqno, amt, balance, crdr, narration from(  ";
                    //str += " SELECT date_depotrns_trnsdate trnsdate, num_depotrns_trnsno trnsno, ";
                    //str += " var_trnstype_trnstype trnstype, num_depotrns_docno docno, num_depotrns_chqno chqno, ";
                    //str += " num_depotrns_amount amt, 0 balance, CASE WHEN num_depotrns_amount >= 0 THEN 'CR' ";
                    //str += " WHEN num_depotrns_amount < 0 THEN 'DR' ELSE '' END crdr, var_depotrns_narration narration ";
                    //str += " FROM aoup_depotrns_def p ";
                    //str += " INNER JOIN aoup_trnstype_def t ON t.num_trnstype_trnstypeid = p.num_depotrns_trnstypeid ";
                    //str += " INNER JOIN glview gv on gv.brid = p.num_depotrns_brid   and gv.glcode = p.num_depotrns_glcode where gv.glsubtypeid in (101,112) ";
                    //str += " AND num_depotrns_accno = '" + account_no + "' ";
                    //str += " UNION ALL ";
                    //str += " SELECT trnsdate, trnsno, var_trnstype_trnstype trnstype, docno, chqno, ";
                    //str += " amount amt, 0 balance, CASE WHEN amount >= 0 THEN 'CR' WHEN amount < 0 THEN 'DR' ELSE '' END  crdr, narration ";
                    //str += " FROM daybook p ";
                    //str += " INNER JOIN aoup_trnstype_def t ON t.num_trnstype_trnstypeid = p.trnstypeid ";
                    //str += " where   accno = '" + account_no + "' ORDER BY trnsdate desc ";
                    //str += " ) where rownum <= 10  ";

                    //string str = "    SELECT trunc(trnsdate) trnsdate, trnsno, trnstype, docno, chqno, amt, balance, crdr, narration ";
                    //str += "FROM ( ";
                    //str += "SELECT A.date_depotrns_trnsdate trnsdate, A.num_depotrns_trnsno trnsno, ";
                    //str += " var_trnstype_trnstype trnstype, A.num_depotrns_docno docno, NVL(A.num_depotrns_chqno,0) chqno, ";
                    //str += " A.num_depotrns_amount amt, 0 balance, ";
                    //str += " CASE WHEN A.num_depotrns_amount >= 0 THEN 'CR' WHEN A.num_depotrns_amount < 0 THEN 'DR' ELSE '' END crdr, ";
                    //str += " A.var_depotrns_narration narration, A.date_depotrns_insdate as insdate ";
                    //str += " FROM aoup_depotrns_def A ";
                    //str += " INNER JOIN  aoup_trnstype_def B ON B.num_trnstype_trnstypeid = A.num_depotrns_trnstypeid ";
                    //str += " INNER JOIN  Society_Master D ON D.branch_id = A.num_depotrns_brid ";
                    //str += " INNER JOIN  aoup_glheadmast_def C ON C.num_glheadmast_brid = D.society_id AND C.num_glheadmast_glcode = A.num_depotrns_glcode ";
                    //str += " WHERE A.num_depotrns_accno = '"+ account_no +"' AND Rownum <= 10 ";
                    //str += " UNION ALL ";
                    //str += " SELECT trnsdate, trnsno, var_trnstype_trnstype trnstype, docno, nvl(chqno,0) chqno, amount amt, 0 balance, ";
                    //str += " CASE WHEN amount >= 0 THEN 'CR' WHEN amount < 0 THEN 'DR' ELSE '' END  crdr, narration,insdate  ";
                    //str += " FROM daybook p ";
                    //str += " INNER JOIN  aoup_trnstype_def t ON t.num_trnstype_trnstypeid = p.trnstypeid ";
                    //str += " WHERE       accno = '"+ account_no +"' AND Rownum <= 10 ) ";
                    //str += "  WHERE ROWNUM <= 10 ";
                    //str += " ORDER BY insdate DESC ";

                    string str = " SELECT * FROM ( SELECT * FROM ( SELECT * FROM ( ";
                    str += " SELECT A.date_depotrns_trnsdate trnsdate, A.num_depotrns_trnsno trnsno, ";
                    str += " var_trnstype_trnstype trnstype, A.num_depotrns_docno docno, NVL(A.num_depotrns_chqno,0) chqno, ";
                    str += " A.num_depotrns_amount amt, 0 balance, ";
                    str += " CASE WHEN A.num_depotrns_amount >= 0 THEN 'CR' WHEN A.num_depotrns_amount < 0 THEN 'DR' ELSE '' END crdr, ";
                    str += " A.var_depotrns_narration narration, A.date_depotrns_insdate as insdate ";
                    str += " FROM aoup_depotrns_def A ";
                    str += " INNER JOIN aoup_trnstype_def B ON B.num_trnstype_trnstypeid = A.num_depotrns_trnstypeid ";
                    str += " INNER JOIN  Society_Master D ON D.branch_id = A.num_depotrns_brid ";
                    str += " INNER JOIN  aoup_glheadmast_def C ON C.num_glheadmast_brid = D.society_id AND C.num_glheadmast_glcode = A.num_depotrns_glcode ";
                    str += " WHERE A.num_depotrns_accno = '" + account_no + "' ORDER BY A.date_depotrns_insdate desc ";
                    str += " ) WHERE ROWNUM <= 10 ";
                    str += " UNION ALL ";
                    str += " SELECT* FROM ( ";
                    str += " SELECT trnsdate, trnsno, var_trnstype_trnstype trnstype, docno, nvl(chqno,0) chqno, amount amt, 0 balance, ";
                    str += " CASE WHEN amount >= 0 THEN 'CR' WHEN amount< 0 THEN 'DR' ELSE '' END crdr, narration, insdate ";
                    str += " FROM daybook p ";
                    str += " INNER JOIN aoup_trnstype_def t ON t.num_trnstype_trnstypeid = p.trnstypeid ";
                    str += " WHERE accno = '" + account_no + "' ORDER BY insdate desc ";
                    str += " ) WHERE ROWNUM<= 10 ) ORDER BY insdate desc ";
                    str += " ) WHERE ROWNUM <= 10 ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    dt.Columns["trnsdate"].ColumnName = "txn_date";
                    dt.Columns["narration"].ColumnName = "txn_remark";
                    dt.Columns["crdr"].ColumnName = "txn_type";
                    dt.Columns["amt"].ColumnName = "amount";
                }
                if (dt.Rows.Count > 0)
                {
                    responseObj = new JObject(
                         new JProperty("transactions", JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dt, Formatting.Indented))));

                }
                else
                {
                    ErrCode = 1000;
                    ErrMsg = "Unable to get data. Please try again later";
                    responseObj = handleErrorData(ErrCode, ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;

        } 
        public static JObject getbankapiDetails(string bank_id, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string data = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "select BM.bank_id,BM.bank_name,BD.property_type,BD.value_type from  BANK_MASTER BM ";
                    str += "INNER JOIN  BANK_MASTER_DETAILS BD ON BM.bank_id = BD.bank_id ";
                    str += "  WHERE BM.bank_id= '" + bank_id + "' ";


                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                    StringBuilder sb = new StringBuilder();

                    sb.Append("{");
                    sb.Append("\"Param\":");
                    sb.Append("{");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("\"" + dt.Rows[i]["property_type"].ToString() + "\":" + "\"" + dt.Rows[i]["value_type"].ToString() + "\",");
                        if (i == dt.Rows.Count - 1)
                        {
                            //
                        }
                    }
                    sb.Append("}");
                    sb.Append("}");

                    data = sb.ToString();
                }
                dynamic json = JsonConvert.DeserializeObject(data);
                 responseObj = json;
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            


            return responseObj;
        }
        public static JObject getTransactioDetails(string Trans_id, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string data = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "select BM.bank_id,BM.bank_name,BD.property_type,BD.value_type from  BANK_MASTER BM ";
                    str += "INNER JOIN  BANK_MASTER_DETAILS BD ON BM.bank_id = BD.bank_id ";
                    str += "  WHERE BM.bank_id= '" + Trans_id + "' ";


                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                    StringBuilder sb = new StringBuilder();

                    sb.Append("{");
                    sb.Append("\"Param\":");
                    sb.Append("{");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("\"" + dt.Rows[i]["property_type"].ToString() + "\":" + "\"" + dt.Rows[i]["value_type"].ToString() + "\",");
                        if (i == dt.Rows.Count - 1)
                        {
                            //
                        }
                    }
                    sb.Append("}");
                    sb.Append("}");

                    data = sb.ToString();
                }
                dynamic json = JsonConvert.DeserializeObject(data);
                 responseObj = json;
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            


            return responseObj;
        }
        public static JObject getlimit(string socid, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string limit = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "select approval_amt as limit from society_neft_registration m where m.hoid=" + socid + " ";

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                    limit = dt.Rows[0]["limit"].ToString();
                }
                 responseObj = new JObject(
                               new JProperty("limit", limit));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            


            return responseObj;
        }
        

        //bank details
        public static JObject bank_details(string bank_id, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string data = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "select BM.bank_id,BM.bank_name,BD.property_type,BD.value_type from  BANK_MASTER BM ";
                    str += "INNER JOIN  BANK_MASTER_DETAILS BD ON BM.bank_id = BD.bank_id ";
                    str += "  WHERE BM.bank_id= '" + bank_id + "' ";


                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                    StringBuilder sb = new StringBuilder();

                    sb.Append("{");
                    sb.Append("\"Param\":");
                    sb.Append("{");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("\"" + dt.Rows[i]["property_type"].ToString() + "\":" + "\"" + dt.Rows[i]["value_type"].ToString() + "\",");
                        if (i == dt.Rows.Count - 1)
                        {
                            //
                        }
                    }
                    sb.Append("}");
                    sb.Append("}");

                    data = sb.ToString();
                }
                dynamic json = JsonConvert.DeserializeObject(data);
                 responseObj = json;
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            


            return responseObj;
        }
        public static JObject getsocietybankDetails( string Hoid, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string data = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select ibd.aggrname as aggrname,ibd.aggrid as aggrid,ibd.corpid as corpid,ibd.userid as userid,ibd.urn as urn,snr.reg_acc_no as accno from tbl_icicibankdetails ibd ";
                    str += " inner join bank_master bm on ibd.icicibankid = bm.bank_id ";
                    str += " inner join society_neft_registration snr on ibd.hoid= snr.hoid ";
                    str += " where bm.bank_id = 1 and ibd.hoid = " + Hoid + " ";


                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                   
                   responseObj = BI.Common.Convert_DataTableToJSON(dt, "Acc_details");

                }
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }



            return responseObj;

        }
        //tarnsfer details
        public static JObject Ben_Trans_details(string reqid, IConfiguration _config)
        {

            int ErrCode = 0;
            string ErrMsg = "";
            string data = "";
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " Select s.hoid,s.bank_id,b.beneficiary_accno,b.beneficiary_bank,b.beneficiary_ifsccode,b.amount ,t.trans_value,b.beneficiary_name ";
                    str += " from neft_beneficiary_trans b ";
                    str += " inner join transfer_trans_def t on b.requid = t.requid ";
                    str += " inner join society_neft_registration s on t.hoid = s.hoid ";
                    str += " where b.requid = '" + reqid + "' ";

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    StringBuilder sb = new StringBuilder();

                    sb.Append("{");
                    sb.Append("\"Param\":");
                    sb.Append("{");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("\"br_id\":");
                        sb.Append("\"" + dt.Rows[i]["hoid"].ToString() + "\",");
                        sb.Append("\"bank_id\":");
                        sb.Append("\"" + dt.Rows[i]["bank_id"].ToString() + "\",");
                        sb.Append("\"ben_acc\":");
                        sb.Append("\"" + dt.Rows[i]["beneficiary_accno"].ToString() + "\",");
                        sb.Append("\"ben_bank\":");
                        sb.Append("\"" + dt.Rows[i]["beneficiary_bank"].ToString() + "\",");

                        sb.Append("\"ben_acc\":");
                        sb.Append("\"" + dt.Rows[i]["beneficiary_accno"].ToString() + "\",");

                        sb.Append("\"ben_ifsc\":");
                        sb.Append("\"" + dt.Rows[i]["beneficiary_ifsccode"].ToString() + "\",");

                        sb.Append("\"amount\":");
                        sb.Append("\"" + dt.Rows[i]["amount"].ToString() + "\",");

                        sb.Append("\"Trans_type\":");
                        sb.Append("\"" + dt.Rows[i]["trans_value"].ToString() + "\",");

                        sb.Append("\"beneficiary_name\":");
                        sb.Append("\"" + dt.Rows[i]["beneficiary_name"].ToString() + "\"");
                    }
                    sb.Append("}");
                    sb.Append("}");

                    data = sb.ToString();


                    if (dt.Rows.Count == 0)
                    {
                        string data1 = "";
                    }
                }
                dynamic json = JObject.Parse(BI.Common.validJason(data));
                 responseObj = json;
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

           

            return responseObj;

        }
        public static JObject release_trans(string flag, string reqid, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("neft_trans_respons", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;


                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 10).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_reqid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_flag"].Value = flag;
                    Cmd.Parameters["in_reqid"].Value = reqid;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("trans_response", new JObject(
                               new JProperty("flag", flag),
                               new JProperty("reqid", reqid),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

           

            return responseObj;
        }
        public static JObject neft_transfer_reverse( string reqid, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_trans_Reversal", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_ReqNo", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters["in_ReqNo"].Value = reqid;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("trans_response", new JObject(

                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;
        }
        public static JObject trans_reject(string reqid, string hoid,string reject_msg, string remark,string userid, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("SP_TRANS_REJECT", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("IN_REQID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("IN_HOID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("IN_USERID", OracleType.VarChar,20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("IN_REJECTIONMSG", OracleType.VarChar,200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("IN_REMARK", OracleType.VarChar,200).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("OUT_ERRORMSG", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("OUT_ERRORCODE", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["IN_REQID"].Value = reqid;
                    Cmd.Parameters["IN_HOID"].Value = hoid;
                    Cmd.Parameters["IN_USERID"].Value = userid;
                    Cmd.Parameters["IN_REJECTIONMSG"].Value = reject_msg;
                    Cmd.Parameters["IN_REMARK"].Value =  remark;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["OUT_ERRORCODE"].Value);
                ErrMsg = Cmd.Parameters["OUT_ERRORMSG"].Value.ToString();
                responseObj = new JObject(new JObject(new JProperty("trans_response", new JObject(

                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            return responseObj;
        }
        public static JObject response_insert(JObject jObject, string reqid, string input_string, string output_string, IConfiguration _config)
        {
            JObject abc = JObject.FromObject(jObject);

            string Response = jObject["RESPONSE"].ToString();
            string Message = "";
            string Errorcode = "";
            string Responsecode = "";
            string Status = "";
            string Utrnumber = "";
            string urn = "";
            string uniqueid = "";
            string req_id = "";
            string aggr_id = "";
            string aggr_name = "";
            string Corp_id = "";

            String ErrCode = "";
            int ErrCode1 = 0;
            String ErrMsg = "";


            try
            {
                if (Response == "FAILURE")
                {

                    Message = jObject["MESSAGE"].ToString();
                    Errorcode = jObject["ERRORCODE"].ToString();
                    Responsecode = jObject["RESPONSECODE"].ToString();
                    Status = jObject["STATUS"].ToString();
                    Utrnumber = "";
                    urn = "";
                    uniqueid = "";
                    req_id = "";
                    aggr_id = "";
                    aggr_name = "";
                    Corp_id = "";

                }
                else if (Response == "SUCCESS")
                {
                    Utrnumber = jObject["UTRNUMBER"].ToString();
                    urn = jObject["URN"].ToString();
                    uniqueid = jObject["UNIQUEID"].ToString();
                    req_id = jObject["REQID"].ToString();
                    aggr_id = jObject["AGGR_ID"].ToString();
                    aggr_name = jObject["AGGR_NAME"].ToString();
                    Corp_id = jObject["CORP_ID"].ToString();
                }
                string constr = getoracleconn(_config);


                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_response", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("trns_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("uniqueid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("corp_id", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("aggr_id", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("aggr_name", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("status", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("reqid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("urn", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("utrnumber", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("response", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("response_code", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("errorcode", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("message", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("input_string", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("output_string", OracleType.VarChar, 500).Direction = ParameterDirection.Input;


                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_response", OracleType.VarChar, 200).Direction = ParameterDirection.Output;

                    Cmd.Parameters["trns_id"].Value = reqid;
                    Cmd.Parameters["uniqueid"].Value = uniqueid;
                    Cmd.Parameters["corp_id"].Value = Corp_id;
                    Cmd.Parameters["aggr_id"].Value = aggr_id;
                    Cmd.Parameters["aggr_name"].Value = aggr_name;
                    Cmd.Parameters["status"].Value = Status;
                    Cmd.Parameters["reqid"].Value = req_id;
                    Cmd.Parameters["urn"].Value = urn;
                    Cmd.Parameters["utrnumber"].Value = Utrnumber;
                    Cmd.Parameters["response"].Value = Response;
                    Cmd.Parameters["response_code"].Value = Responsecode;
                    Cmd.Parameters["errorcode"].Value = Errorcode;
                    Cmd.Parameters["message"].Value = Message;
                    Cmd.Parameters["input_string"].Value = input_string;
                    Cmd.Parameters["output_string"].Value = output_string;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Cmd.Parameters["out_ErrorCode"].Value.ToString();
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();

            }
            catch (Exception ex)
            {
                ErrCode1 = 0;
                ErrMsg = "Unable to proceed - webserve";

            }
            return abc;

        }
        public static JObject Trans_status(string socid, IConfiguration _config)
        {
            string retVal = "";
            JObject Jdata = new JObject();
            string ErrMsg = "";
            int ErrCode = 0;
            string dataobj;
            string is_verified = "";
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select nr.trns_id as reqid, h_glcode , h_acc,b_acc,b_bank,b_ifsc,name,na.amount,urn as urn,utrnumber as urtno, response as response ,message as msg ";
                    str += " from neft_response nr ";
                    str += " inner join transfer_trans_def ttd on ttd.requid = nr.trns_id ";
                    str += " inner join neft_approval na on na.reqid = ttd.requid ";
                    str += " where ttd.hoid = '" + socid + "' and TRUNC(nr.trans_time)= trunc(sysdate) ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Trans_Details");
                }
                 responseObj = new JObject(Jdata);
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            
            return responseObj;
        }
        public static JObject Setlimit(string mobile_no, string society_id, string limit, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int hoid = 0;
            string flag = "set_limit";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_appregister", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_imeino", OracleType.VarChar, 200).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_society_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_flag", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_limit", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_username", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_IS_Verified", OracleType.VarChar, 10).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_otp", OracleType.Number, 6).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 5).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_name", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_hoid", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_imeino"].Value = "";
                    Cmd.Parameters["in_society_id"].Value = society_id;
                    Cmd.Parameters["in_flag"].Value = flag;
                    Cmd.Parameters["in_limit"].Value = limit;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                 responseObj = new JObject(new JObject(new JProperty("Limit_response", new JObject(
                                new JProperty("mobile_no", mobile_no),
                                new JProperty("ErrCode", ErrCode),
                                new JProperty("ErrMsg", ErrMsg)
                ))));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            

            return responseObj;
        }
        public static JObject ECValidation(string acc_no, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            int count = 0;
            string name = "";
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                   // string str = " select count(accname) as count_data_saving from VirtualAccountView where v_accno='" + acc_no + "' ";
                    string str = " select count(accname) as count_data_saving,acc_type as acc_type from VirtualAccountView where v_accno='" + acc_no + "' group by accname,acc_type ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);

                    if (dt.Rows.Count > 0)
                    {
                        count = Convert.ToInt32(dt.Rows[0]["count_data_saving"].ToString());
                        name = dt.Rows[0]["acc_type"].ToString();
                    }
                    else
                    {
                        count = 0;
                        name = "";
                    }
                }
                responseObj = new JObject(
                                 new JProperty("Count", count),
                                 new JProperty("Name", name));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            return responseObj;
        }
        public static string UTRValidation(string utr_no, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            int count = 0;
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select count(utrnumber) as utr_no from( ";
                    str += " select utrnumber from neft_response ";
                    str += " union all ";
                    str += " select utrnumber from neft_income_trans ";
                    str += " ) a where utrnumber = " + utr_no + " ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    count = Convert.ToInt32(dt.Rows[0]["utr_no"].ToString());
                }
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }
            return count.ToString();
        }

        //sms for transaction 
        public static string Success_trans_sms(string req_id , IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string mobile_no = "";
            string sender_id ="";
            string username = "";
            string password = "";
            string sms_temp = "";
            string amount = "";
            string utrnumber = "";
            string acc_no = "";
            JObject jobject = new JObject();
            DataTable dt = new DataTable();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select ttd.ins_by as mobile_no,sender_id,username,password,ttd.amount,nr.utrnumber,to_char(ttd.holder_accno) as acc_no from transfer_trans_details  ttd ";
                    str += " inner join tbl_sms_config tsc on ttd.ho_id = tsc.ho_id ";
                    str += " inner join neft_response nr on ttd.requid = nr.trns_id ";
                    str += " where ttd.requid='" + req_id + "' ";

                    
                    Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                    mobile_no = dt.Rows[0]["mobile_no"].ToString();
                    sender_id = dt.Rows[0]["sender_id"].ToString();
                    username = dt.Rows[0]["username"].ToString();
                    password = dt.Rows[0]["password"].ToString();
                    amount = dt.Rows[0]["amount"].ToString();
                    utrnumber = dt.Rows[0]["utrnumber"].ToString();
                    acc_no = dt.Rows[0]["acc_no"].ToString();

                    sms_temp = "Dear Customer, You have made a payment of Rs. " + amount + " using NEFT via IMPS from your Account " + acc_no + " with reference number " + utrnumber + ". Regards Copass";
                    string SMSURL = "http://103.15.179.45:8449/MessagingGateway/SendTransSMS?Username=" + sender_id + "&Password=" + password + "&MessageType=txt&Mobile=" + mobile_no + "&SenderID=" + username + "&Message=" + sms_temp + " ";
                    string responsesms2 = callurl(SMSURL);
                    var get_responsesms2 = responsesms2;
                    jobject = JObject.Parse(get_responsesms2);

                
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }
            return jobject.ToString();
        }
        // incoming trans insert
        public static JObject income_response_insert(JObject jObject,  IConfiguration _config)
        {
            JObject abc = JObject.FromObject(jObject);
            int ErrCode = 0;
            string ErrMsg = "";
            JToken headerToken = abc;
            string VirtualAccountNumber = headerToken.SelectToken("VirtualAccountNumber").ToString();
            string clientcode = headerToken.SelectToken("ClientCode").ToString();
            string senderremark = headerToken.SelectToken("SenderRemark").ToString();
            string clientaccountno = headerToken.SelectToken("ClientAccountNo").ToString();
            string utrnumber = headerToken.SelectToken("UTR").ToString();
            string amt = headerToken.SelectToken("Amount").ToString();
            string payeraccnumber = headerToken.SelectToken("PayerAccNumber").ToString();
            string payername = headerToken.SelectToken("PayerName").ToString();
            string payerbankifsc = headerToken.SelectToken("PayerBankIFSC").ToString();
            string BANK_TRANS_NO = headerToken.SelectToken("BankInternalTransactionNumber").ToString();
            string input_string = abc.ToString();
            string ReqNo = "";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_incometrans", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_virtualaccountnumber", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_clientcode", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_senderremark", OracleType.VarChar, 100).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_clientaccountno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_utrnumber", OracleType.VarChar,50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amt", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_payeraccnumber", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_payername", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_payerbankifsc", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_input_string", OracleType.VarChar, 4000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_BANK_TRANS_NO", OracleType.VarChar, 50).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ReqNo", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_virtualaccountnumber"].Value = VirtualAccountNumber;
                    Cmd.Parameters["in_clientcode"].Value = clientcode;
                    Cmd.Parameters["in_senderremark"].Value = senderremark;
                    Cmd.Parameters["in_clientaccountno"].Value = clientaccountno;
                    Cmd.Parameters["in_utrnumber"].Value = utrnumber;
                    Cmd.Parameters["in_amt"].Value = amt;
                    Cmd.Parameters["in_payeraccnumber"].Value = payeraccnumber;
                    Cmd.Parameters["in_payername"].Value = payername;
                    Cmd.Parameters["in_payerbankifsc"].Value = payerbankifsc;
                    Cmd.Parameters["in_input_string"].Value = input_string;
                    Cmd.Parameters["in_BANK_TRANS_NO"].Value = BANK_TRANS_NO;



                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt16(Cmd.Parameters["out_ErrorCode"].Value.ToString());
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                ReqNo = Cmd.Parameters["out_ReqNo"].Value.ToString();
                responseObj = new JObject(
                                 new JProperty("ErrCode", ErrCode),
                                 new JProperty("ErrMsg", ErrMsg),
                                 new JProperty("ReqNo", ReqNo));

            }
            catch (Exception ex)
            {
               int ErrCode1 = 0;
                ErrMsg = "Unable to proceed - webserve";
                Common.write_log_error("release_trans api | release_trans process |", "API Status : " + ex + jObject);
                responseObj = handleErrorData(ErrCode1, ErrMsg);
            }

            return responseObj;

        }
        // incomin trans for fm
        public static JObject incoming_trans(string socid, IConfiguration _config)
        {
            string retVal = "";
            JObject Jdata = new JObject();
            string ErrMsg = "";
            int ErrCode = 0;
            string dataobj;
            string is_verified = "";
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select  vav.accno,vav.accname,payeraccnumber,payername,payerbankifsc,utrnumber,amount from neft_income_trans nit ";
                    str += " inner join virtualaccountview vav on  nit.virtualaccountnumber = vav.v_accno ";
                    str += " where nit.hoid=" + socid + " and trunc(ins_date) = trunc(sysdate) ";

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Trans_Details");
                }

                 responseObj = new JObject(Jdata);
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }



            return responseObj;
        }
        // income transaction internal
        public static JObject income_transaction(string virtualaccountnumber, string ReqID, string amount, string trans_id, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";

            string ReqNo = "";
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_trans_incoming", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_ReqID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_VirtualAccNo", OracleType.VarChar, 50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_id", OracleType.VarChar,10).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_VirtualAccNo"].Value = virtualaccountnumber;
                    Cmd.Parameters["in_ReqID"].Value = ReqID;
                    Cmd.Parameters["in_amount"].Value = amount;
                    Cmd.Parameters["in_trans_id"].Value = trans_id;


                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt16(Cmd.Parameters["out_ErrorCode"].Value.ToString());
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                responseObj = new JObject(new JObject(new JProperty("Income_trans", new JObject(
                                 new JProperty("ErrCode", ErrCode),
                                 new JProperty("ErrMsg", ErrMsg)))));

            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            return responseObj;

        }
        //society deatils(logo,address,banners)
        public static JObject Society_Deatils(string society_id, IConfiguration _config)
        {

            string ErrMsg = "";
            int ErrCode = 0;
            string Soc_Name = "";
            string Soc_add = "";
            string fund_transaction = "";
            string qr_transaction = "";
            string bill_payments = "";
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            JArray banners = new JArray();
            Byte[] Logo = new Byte[0];
            string Soc_logo_base64string = "";
            string base64string = "";
            JObject responseObj = new JObject();
            string daily_trans_limit = "";
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    //string str = " select var_societymaster_name as Soc_Name,var_societymaster_address as Soc_add,img_societymaster_logoimage Soc_img from aoup_societymaster_def m where num_societymaster_brid=" + society_id + " ";

                    //string str = " select var_societymaster_name as Soc_Name,var_societymaster_address as Soc_add,img_societymaster_logoimage Soc_img, fund_transaction, qr_transaction, bill_payments ";
                    //str += " from aoup_societymaster_def SM ";
                    //str += " INNER JOIN society_neft_config SNC ON SM.num_societymaster_brid = SNC.hoid ";
                    //str += " where num_societymaster_brid = "+ society_id +" ";

                    string str = " select var_societymaster_name as Soc_Name,var_societymaster_address as Soc_add,img_societymaster_logoimage Soc_img, ";
                    str += " snc.fund_transaction as fund_transaction, snc.qr_transaction as qr_transaction, snc.bill_payments as bill_payments ,snr.daily_trans_limit as daily_trans_limit ";
                    str += " from aoup_societymaster_def SM ";
                    str += " INNER JOIN society_neft_config SNC ON SM.num_societymaster_brid = SNC.hoid ";
                    str += " INNER JOIN society_neft_registration snr on sm.num_societymaster_brid = snr.hoid ";
                    str += " where num_societymaster_brid = "+ society_id +" ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                if (dt.Rows.Count > 0)
                {
                    Logo = (Byte[])dt.Rows[0]["Soc_img"];
                    Soc_Name = dt.Rows[0]["Soc_Name"].ToString();
                    Soc_add = dt.Rows[0]["Soc_add"].ToString();
                    fund_transaction = dt.Rows[0]["fund_transaction"].ToString();
                    qr_transaction = dt.Rows[0]["qr_transaction"].ToString();
                    bill_payments = dt.Rows[0]["bill_payments"].ToString();
                    daily_trans_limit = dt.Rows[0]["daily_trans_limit"].ToString();

                    Soc_logo_base64string = Convert.ToBase64String(Logo, 0, Logo.Length);
                }
                    using (OracleConnection Con = getoracleOpenConn(_config))
                    {
                        string str = " select hoid, banner_1, from_date_1, to_date_1, is_active_1, banner_2, from_date_2, to_date_2, is_active_2, ";
                        str += " banner_3, from_date_3, to_date_3, is_active_3, ";
                        str += " banner_4, from_date_4, to_date_4, is_active_4 ";
                        str += " from society_banner_details  ";
                        str += " where hoid = " + society_id + " ";

                        OracleCommand Cmd = new OracleCommand(str, Con);
                        OracleDataAdapter AdpData = new OracleDataAdapter();
                        AdpData.SelectCommand = Cmd;
                        AdpData.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            int bannerFound = 1;
                            DataRow row = dt1.Rows[0];

                            foreach (DataColumn col in dt1.Columns)
                            {
                                if (col.ColumnName.ToLower().Contains("banner") && row["IS_ACTIVE_" + bannerFound.ToString()].ToString() == "1")
                                {

                                    string bannerPath = row["Banner_" + bannerFound.ToString()].ToString();
                                    int bannerIndex = bannerPath.ToLower().IndexOf("banner");

                                    string bannerDirectoryPath = bannerPath.Substring(0, bannerIndex - 1);
                                    //string bannerDirectoryPath = bannerPath.Substring(0, bannerPath.Length - (bannerIndex - 1));
                                    bannerPath = (bannerPath.Replace(bannerDirectoryPath, "https://copass.in/")).Replace("\\", "/");
                                    base64string = ConvertImageURLToBase64(bannerPath);


                                    banners.Add(new JObject(new JProperty("Banner", base64string)));
                                    //No Code Required
                                    bannerFound += 1;
                                }

                            }


                        

                    }

                    getoracleCloseConn(_config);
                }

                responseObj = new JObject(new JObject(new JProperty("register_response", new JObject(
               new JProperty("Soc_name", Soc_Name),
               new JProperty("Soc_add", Soc_add),
               new JProperty("fund_transaction", fund_transaction),
               new JProperty("qr_transaction", qr_transaction),
               new JProperty("bill_payments", bill_payments),
               new JProperty("daily_trans_limit", daily_trans_limit),
               new JProperty("Soc_logo", Soc_logo_base64string),
                new JProperty("Banner_details", banners)

               ))));
            }


            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }





            return responseObj;
        }
        //Society details for logo.
        public static JObject Society_Deatils_FM(string society_id, IConfiguration _config)
        {

            string ErrMsg = "";
            int ErrCode = 0;
            string Soc_Name = "";
            string Soc_add = "";
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            JArray banners = new JArray();
            Byte[] Logo = new Byte[0];
            string Soc_logo_base64string = "";
            string base64string = "";
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select var_societymaster_name as Soc_Name,var_societymaster_address as Soc_add,img_societymaster_logoimage Soc_img from aoup_societymaster_def m where num_societymaster_brid=" + society_id + " ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                Logo = (Byte[])dt.Rows[0]["Soc_img"];
                Soc_Name = dt.Rows[0]["Soc_Name"].ToString();
                Soc_add = dt.Rows[0]["Soc_add"].ToString();


                Soc_logo_base64string = Convert.ToBase64String(Logo, 0, Logo.Length);

                responseObj = new JObject(new JObject(new JProperty("register_response", new JObject(
               new JProperty("Soc_name", Soc_Name),
               new JProperty("Soc_add", Soc_add),
               new JProperty("Soc_logo", Soc_logo_base64string)

               ))));
            }


            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }





            return responseObj;
        }
        public static string ConvertImageURLToBase64(String urldata)
        {

            string url = urldata;
            //create an object of StringBuilder type.
            StringBuilder _sb = new StringBuilder();
            //create a byte array that will hold the return value of the getImg method
            Byte[] _byte = GetImg(url);
            //appends the argument to the stringbulilder object (_sb)
            _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
            //return the complete and final url in a base64 format.
            return string.Format(_sb.ToString());

        }
        public static byte[] GetImg(string url)
        {
            //create a stream object and initialize it to null
            Stream stream = null;
            //create a byte[] object. It serves as a buffer.
            byte[] buf;
            try
            {
                //Create a new WebProxy object.
                WebProxy myProxy = new WebProxy();
                //create a HttpWebRequest object and initialize it by passing the colleague api url to a create method.
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                //Create a HttpWebResponse object and initilize it
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                //get the response stream
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    //get the content length in integer
                    int len = (int)(response.ContentLength);
                    //Read bytes
                    buf = br.ReadBytes(len);
                    //close the binary reader
                    br.Close();
                }
                //close the stream object
                stream.Close();
                //close the response object 
                response.Close();
            }
            catch (Exception exp)
            {
                //set the buffer to null
                buf = null;
            }
            //return the buffer
            return (buf);
        }
        private static Byte[] BitmapToBytesCode(System.Drawing.Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public static JObject Loan_Details(string mobile_no, string soc_id, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            string data = "";
            string v_loan_acc = "";
            string Full_name = "";
            string upi = "";


            try

            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    //string str = " select to_char(o.mobile_no) as MOBILE_NO , to_char(d.num_loan_accno) as LOAN_ACCNO,A.glname as LOAN_TYPE,to_char(d.num_loan_amount) as LOAN_AMOUNT,to_char(s.num_accbalance_currentbal) as LOAN_BALANCE , to_char(d.num_loan_intrate) as LOAN_INTRATE ,to_char(d.num_loan_period) as LOAN_PERIOD,d.date_loan_duedate as LOAN_DUEDATE,d.date_loan_inststartdate as LOAN_INSTSTARTDATE, to_char(d.num_loan_instalmentamt) as LOAN_INSTALMENTAMT, n.fullname as Full_name ";
                    //str += " from aoup_loan_def d ";
                    //str += " inner join aoup_loanappli_def m   on d.num_loan_accno=m.num_loanappli_accno ";
                    //str += " inner join customerview n on m.num_loanappli_custinternalid=n.internalid ";
                    //str += "  inner join aoup_accbalance_def s on s.num_accbalance_accno=d.num_loan_accno ";
                    //str += " inner join mobile_registration o on n.mobile=o.mobile_no ";
                    //str += " inner Join accountview A on A.accno=d.num_loan_accno and A.closedate is null ";
                    //str += "  WHERE o.mobile_no = '" + mobile_no + "' and A.society='" + soc_id + "'  ";

                    string str = " select mobile_no, to_char(loan_accno) LOAN_ACCNO, loan_type as LOAN_TYPE, TO_CHAR(loan_amount) LOAN_AMOUNT, TO_CHAR(loan_balance) as LOAN_BALANCE, to_char(loan_intrate) as LOAN_INTRATE , to_char(loan_period) as LOAN_PERIOD, loan_duedate as LOAN_DUEDATE, loan_inststartdate as LOAN_INSTSTARTDATE, ";
                    str += " loan_instalmentamt as LOAN_INSTALMENTAMT, full_name as Full_name ";
                    str += " from vloanviewformobile where mobile_no = "+ mobile_no +" and society_id = "+ soc_id +" ";


                    DataTable dt = new DataTable();
                    OracleCommand Cmd1 = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd1;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);

                    if (dt.Rows.Count > 0)
                    {
                        Jdata = BI.Common.Convert_DataTableToJSON(dt, "Loan_Acc_details");
                        responseObj = new JObject(new JObject(new JProperty("Loan_details", new JObject(
                                    new JProperty("data", Jdata),
                                    //new JProperty("upi", upi),
                                    // new JProperty("QR_Code", data),
                                    new JProperty("ErrCode", ErrCode),
                                    new JProperty("ErrMsg", ErrMsg)

                    ))));
                    }
                    else
                    {
                        ErrCode = 1000;
                        ErrMsg = "Unable to get data. Please try again later";
                        responseObj = handleErrorData(ErrCode, ErrMsg);
                    }
                    // v_loan_acc = dt.Rows[0]["LOAN_ACCNO"].ToString();
                    // Full_name = dt.Rows[0]["Full_name"].ToString();
                    // upi = "CPS." + v_loan_acc + "@icici";
                    //string upi_id = "upi://pay?pa=" + upi + "&pn=" + Full_name + "&am=0.00&tr=CPS12345670020&CU=INR&MC=5411";

                    //QR_Code_genrator qr = new QR_Code_genrator();
                    //data = qr.SendRequest(upi_id);
                }
                 
            }

            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            

            return responseObj;
        }
        public static JObject Depo_Details(string mobile_no, string soc_id, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            DataTable dt = new DataTable();
            JObject responseObj = new JObject();

            try
            {
                OracleConnection Con = new OracleConnection(getoracleconn(_config));
                //string str = " select to_char(c.mobile) as mobile_no,to_char(to_date(d.openingdate)) as opening_date, to_char(d.accno) as depo_accno,d.accname as depo_accname,to_char(z.glname) as deposite_type,to_char(d.amount) as depo_amount,to_char(d.period) as period,to_char(d.intrate) as interest_rate,to_char(d.dueamount) as maturity_amount,to_char(to_date(d.duedate)) as due_date,to_char(m.num_accbalance_currentbal) as current_bal,to_char(d.instalmentamt) as installment,n.var_depoappli_nominee as nominee ";
                //    str += " from depositview d ";
                //    str += " Inner Join aoup_depoappli_def n on n.num_depoappli_accno=d.accno ";
                //    str += "  Inner Join customerview c on c.internalid=d.custintid ";
                //    str += " Inner Join aoup_accbalance_def m on m.num_accbalance_accno=d.accno ";
                //    str += " inner join accountview z on z.accno=m.num_accbalance_accno ";
                //    str += "  inner join mobile_registration o on c.mobile=o.mobile_no and  d.closedate is  null ";
                //    str += "  WHERE o.mobile_no = '" + mobile_no + "' and z.society='" + soc_id + "'  ";

                //string str = " select d.openingdate,d.accname,to_char(d.accno) accno,d.amount,d.period,d.intrate,d.currentbal,d.duedate,d.instalmentamt ";
                //str += " from depositview d ";
                //str += "  inner join customerview c on d.custintid = c.internalid ";
                //str += "  where c.mobile = " + mobile_no + " and c.hoid = " + soc_id + "  and d.closedate is null and d.renewalflag = 'DE'";

                //string str = " select dep_date openingdate, substr(dep_accname,1,20) accname, to_char(dep_accno) accno, ";
                //str += " dep_amount as amount, to_char(dep_period) || ' ' || dep_period_type period, ";
                //str += " dep_intrate intrate, dep_cur_balance currentbal,  CASE WHEN dep_duedate IS NULL THEN 'N/A' ELSE TO_CHAR(dep_duedate) END duedate, dep_instalment instalmentamt,gl_name ";
                //str += " from vDepositViewForMobile where cell_no = "+ mobile_no +" and ho_id = "+soc_id+" ";

                string str = " select to_char(cell_no) as mobile_no,to_char(to_date(dep_date)) as opening_date, to_char(dep_accno) as depo_accno,dep_accname as depo_accname,to_char(gl_name) as deposite_type, ";
                str += " to_char(dep_amount) as depo_amount,to_char(dep_period) as period,to_char(dep_intrate) as interest_rate,to_char(dep_duedate) as due_date, ";
                str += " to_char(dep_cur_balance) as current_bal,to_char(dep_instalment) as installment ";
                str += " from vdepositviewformobile m where m.ho_id = "+ soc_id + " and cell_no = "+ mobile_no + " ";

                Con.Open();
                    
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                //getoracleCloseConn(_config);

                if (dt.Rows.Count > 0)
                {
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Depo_Acc_details");

                    responseObj = new JObject(new JObject(new JProperty("Deposit_Details", new JObject(
                                   new JProperty("Depo_Acc_details", JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dt, Formatting.Indented))),
                                   new JProperty("ErrCode", ErrCode),
                                   new JProperty("ErrMsg", ErrMsg)

                   ))));
                }
                else
                {
                    ErrCode = 1000;
                    ErrMsg = "Unable to get data. Please try again later";
                    responseObj = handleErrorData(ErrCode, ErrMsg);
                }
            }


            catch (Exception ex)
            
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            


            return responseObj;
        }
        public static JObject FM_transaction_approval(string reqid, string soc_id, string user_id, string approval_level, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            DataTable dt = new DataTable();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_fm_trans_approval", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_reqid", OracleType.VarChar).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_soc_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_user_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_approval_level", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errmsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errcode", OracleType.Number, 5).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_reqid"].Value = reqid;
                    Cmd.Parameters["in_soc_id"].Value = soc_id;
                    Cmd.Parameters["in_user_id"].Value = user_id;
                    Cmd.Parameters["in_approval_level"].Value = approval_level;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }

                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errcode"].Value);
                ErrMsg = Cmd.Parameters["out_errmsg"].Value.ToString();
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }

            JObject responseObj = new JObject(new JObject(new JProperty("FMRansaction_Approval", new JObject(

                                  new JProperty("ErrCode", ErrCode),
                                  new JProperty("ErrMsg", ErrMsg)

                ))));


            return responseObj;
        }
        public static JObject Transfer_bal(string mobile_no , string bene_acc,string sender_acc,string socid, IConfiguration _config)

        {
            string ErrMsg = "";
            int ErrCode = 0;
            int ErrorCode = 0;
            string ErrorMsg = "";
            int amt_limit = 0;
            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_balance_validation", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_sender_acc", OracleType.VarChar,20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_bene_acc", OracleType.VarChar,20).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_soc_id", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 50).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_amt_limit", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                    Cmd.Parameters["in_sender_acc"].Value = sender_acc;
                    Cmd.Parameters["in_bene_acc"].Value = bene_acc;
                    Cmd.Parameters["in_soc_id"].Value = socid;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                
                ErrorCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrorMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                amt_limit = Convert.ToInt32(Cmd.Parameters["out_amt_limit"].Value);

                 responseObj = new JObject(new JProperty("Society_response", new JObject(
                               new JProperty("Amt_limit", amt_limit),
                               new JProperty("ErrCode", ErrorCode),
                               new JProperty("ErrMsg", ErrorMsg)
               )));


                
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
            }
            return responseObj;
        }
        public static JObject account_upi_details(string account_no, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            DataTable dt = new DataTable();
            try

            {

                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select v_accno , accname,cust_internalid  from virtualaccountview v where v.accno=" + account_no + " ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }

                if (dt.Rows.Count > 0)
                {
                    string v_accno = dt.Rows[0]["v_accno"].ToString();
                    string accname = dt.Rows[0]["accname"].ToString();
                    string cust_internalid = dt.Rows[0]["cust_internalid"].ToString();

                    responseObj = new JObject(new JObject(new JProperty("account_details", new JObject(
                                       new JProperty("ErrCode", 100),
                                       new JProperty("ErrMsg", "Success"),
                                       new JProperty("v_accno", v_accno),
                                       new JProperty("accname", accname),
                                       new JProperty("cust_internalid", cust_internalid)
                       ))));
                }
                else
                {
                    responseObj = new JObject(new JObject(new JProperty("account_details", new JObject(
                                       new JProperty("ErrCode", 500),
                                       new JProperty("ErrMsg", "Failed")
                                       ))));
                }
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            return responseObj;
        }
        public static JObject uuid_details(string uuid, IConfiguration _config)
        {
            string retVal = "";
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            DataTable dt = new DataTable();
            try

            {

                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select count(mid) as valid from upi_qr_outgoing_trans where trans_id='" + uuid + "' ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }

                if (dt.Rows.Count > 0)
                {
                    int valid = Convert.ToInt32(dt.Rows[0]["valid"].ToString());

                    responseObj = new JObject(new JObject(new JProperty("account_details", new JObject(
                                       new JProperty("ErrCode", 100),
                                       new JProperty("ErrMsg", "Success"),
                                       new JProperty("v_accno", valid)
                       ))));
                }
                else
                {
                    responseObj = new JObject(new JObject(new JProperty("account_details", new JObject(
                                       new JProperty("ErrCode", 500),
                                       new JProperty("ErrMsg", "Failed")
                                       ))));
                }
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }

            return responseObj;
        }
        public static JObject UPIValidation(string acc_no, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            string count = "";
            string Name = "";
            JObject responseObj = new JObject();    
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select count(accname) as count_data , accname as Name  from VirtualAccountView where v_accno='" + acc_no + "' group by accname ";

                    DataTable dt = new DataTable();
                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    if (dt.Rows.Count > 0)
                    {
                        count = dt.Rows[0]["count_data"].ToString();
                        Name = dt.Rows[0]["Name"].ToString();
                        responseObj = new JObject(new JProperty("UPI_details", new JObject(
                                   new JProperty("count", count),
                                   new JProperty("Name", Name))));
                    }
                    else
                    {
                        responseObj = new JObject(new JProperty("UPI_details", new JObject(
                                       new JProperty("count", 0)
                       )));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";

            }
            return responseObj;
        }
        public static JObject trans_approval_limt(string hoid, IConfiguration _config)
        {
            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            DataTable dt = new DataTable();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select hoid,level_seq,min_amt,max_amt from NEFT_APPROVAL_LIMIT where hoid=" + hoid + " ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Approval_details");

                }
                responseObj =new JObject(new JProperty("Approval_response", new JObject(
                              new JProperty("data", Jdata)
              )));
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }
            return responseObj;
        }
        //trans rejected 
        public static JObject rejected_trans(string socid, IConfiguration _config)
        {
            string retVal = "";
            JObject Jdata = new JObject();
            string ErrMsg = "";
            int ErrCode = 0;
            string dataobj;
            string is_verified = "";
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = " select ttd.requid,tr.rejection_msg,tr.remark,nbt.beneficiary_accno,nbt.beneficiary_bank,nbt.beneficiary_ifsccode,ttd.amount ";
                    str += " from transfer_trans_def ttd  ";
                    str += " inner join trans_reject tr on ttd.requid=tr.reqid ";
                    str += " inner join neft_beneficiary_trans nbt on ttd.requid=nbt.requid ";
                    str += " where ttd.trans_status='FAILED' and trunc(rejection_date) = trunc(sysdate) and ttd.hoid=" + socid + "  ";

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Trans_Details");
                }

                responseObj = new JObject(Jdata);
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }



            return responseObj;
        }
        //upi outgoing trans update
        public static JObject upi_outgoing_data_insert(string mobile_no, string device_id, string convertedUUID, string payee, string upi, string outward_data,string amount,string socid, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            string Reqid = "";
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("upi_qr_Outward_trans", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_mobile_no", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_device_id", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_convertedUUID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_payee", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_upi", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_outward_data", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_reqid", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobile_no"].Value = mobile_no;
                    Cmd.Parameters["in_device_id"].Value = device_id;
                    Cmd.Parameters["in_convertedUUID"].Value = convertedUUID;
                    Cmd.Parameters["in_payee"].Value = payee;
                    Cmd.Parameters["in_upi"].Value = upi;
                    Cmd.Parameters["in_amount"].Value = amount;
                    Cmd.Parameters["in_outward_data"].Value = outward_data;
                    Cmd.Parameters["in_hoid"].Value = socid;


                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                Reqid = Cmd.Parameters["out_reqid"].Value.ToString();
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                responseObj = new JObject(new JObject(new JProperty("upi_response", new JObject(

                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg),
                               new JProperty("Reqid", Reqid)
               ))));
            }
            catch (Exception ex)
            {
                Common.write_log_Success("upi_qr_Outward_trans/Errorcode.aspx | iciciReq Method Response |", "Request Data Status  final : " + ex);
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            return responseObj;



        }
        //upi outward transaction
        public static JObject upi_outgoing_trans(string Reqid, string Account, string Amt,string Remark, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_upi_outgoing", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_flag", OracleType.VarChar,50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_ReqID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_accno", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_type", OracleType.VarChar,50).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("remark1", OracleType.VarChar, 50).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_flag"].Value = "submit";
                    Cmd.Parameters["in_ReqID"].Value = Reqid;
                    Cmd.Parameters["in_accno"].Value = Account;
                    Cmd.Parameters["in_amount"].Value = Amt;
                    Cmd.Parameters["in_trans_type"].Value = "UPI";
                    Cmd.Parameters["remark1"].Value = Remark;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                responseObj = new JObject(new JObject(new JProperty("upi_response", new JObject(

                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));
                Common.write_log_Success("incoming upi insert.aspx | iciciReq Method Response |", "upi transaction  : " + responseObj);
            }
            catch (Exception ex)
            {
                Common.write_log_error("incoming upi insert.aspx | iciciReq Method Response |", "upi Transaction failed : " + ex);
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            return responseObj;



        }
        //kyc verify
        public static JObject KYC_Verify(string hoid, string mobile_no, IConfiguration _config)
        {
           
            string ErrMsg = "";
            int ErrCode = 0;
            string is_verified = "";


            JObject responseObj = new JObject();
            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("mobile_kyc_verify", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_mobileno", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_hoid", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_Is_verify", OracleType.VarChar, 5).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_mobileno"].Value = mobile_no;
                    Cmd.Parameters["in_hoid"].Value = hoid;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                is_verified = Cmd.Parameters["out_Is_verify"].Value.ToString();
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();

                responseObj = new JObject(new JObject(new JProperty("KYC_response", new JObject(
                               new JProperty("mobile_no", mobile_no),
                               new JProperty("IS_verified", is_verified),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));

            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);

            }
            return responseObj;
        }
        //kyc verify
        public static JObject Bank_details( string Acc_no, IConfiguration _config)
        {

            string ErrMsg = "";
            int ErrCode = 0;
            JObject Jdata = new JObject();
            DataTable dt = new DataTable();
            JObject responseObj = new JObject();
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    string str = "select soc_name as society,brname,v_accno accno, accname,ifsc_code from virtualaccountview ";
                    str += " where accno = '"+ Acc_no + "' ";

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);

                    if (dt.Rows.Count > 0)
                    {
                        Jdata = BI.Common.Convert_DataTableToJSON(dt, "BD_details");
                        responseObj = new JObject(new JProperty("BD_response", new JObject(
                             new JProperty("data", Jdata))));
                    }
                    else 
                    {
                        ErrCode = 1000;
                        ErrMsg = "Unable to get data. Please try again later";
                        responseObj = handleErrorData(ErrCode, ErrMsg);
                    }

                }
               
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);

            }
            return responseObj;
        }
        //upi incoming trans update
        public static JObject upi_incoming_data_insert(string Bank_rrn, string payername, string payerva, string merchanttranid, string input_string, string amount, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int Reqid = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("upi_qr_inward_trans", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_Bank_rrn", OracleType.VarChar,500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_payername", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_payerva", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_merchanttranid", OracleType.VarChar,500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_input_string", OracleType.VarChar, 5000).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_reqid", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters["in_Bank_rrn"].Value = Bank_rrn;
                    Cmd.Parameters["in_payername"].Value = payername;
                    Cmd.Parameters["in_payerva"].Value = payerva;
                    Cmd.Parameters["in_merchanttranid"].Value = merchanttranid;
                    Cmd.Parameters["in_input_string"].Value = input_string;
                    Cmd.Parameters["in_amount"].Value = amount;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                Reqid = Convert.ToInt32(Cmd.Parameters["out_reqid"].Value.ToString());
                responseObj = new JObject(new JObject(new JProperty("upi_response", new JObject(

                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg),
                               new JProperty("Reqid", Reqid)
               ))));
                Common.write_log_Success("incoming upi insert.aspx | iciciReq Method Response |", "upi data insert success  : " + responseObj);
            }
            
            catch (Exception ex)
            {
                Common.write_log_error("incoming upi insert.aspx | iciciReq Method Response |", "upi data insert failed  : " + ex);
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            return responseObj;



        }
        //upi trans
        public static JObject upi_incoming_trans(string Reqid, string merchanttranid, string amount, IConfiguration _config)
        {
            string retVal = "";
            int out_uid_code = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            int is_disable = 0;
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_neft_upi_income", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.Add("in_ReqID", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_VirtualAccNo", OracleType.VarChar, 500).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_amount", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_trans_id", OracleType.VarChar, 50).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_ReqID"].Value = Reqid;
                    Cmd.Parameters["in_VirtualAccNo"].Value = merchanttranid;
                    Cmd.Parameters["in_amount"].Value = amount;
                    Cmd.Parameters["in_trans_id"].Value = "UPI";

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();
                responseObj = new JObject(new JObject(new JProperty("upi_response", new JObject(

                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));
                Common.write_log_Success("incoming upi insert.aspx | iciciReq Method Response |", "upi transaction  : " + responseObj);
            }
            catch (Exception ex)
            {
                Common.write_log_error("incoming upi insert.aspx | iciciReq Method Response |", "upi Transaction failed : " + ex);
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);
            }


            return responseObj;



        }
        // loan transacction
        public static DataTable getLoanDetails(String VirtualAccNo, Double Amount, IConfiguration _config)
        {
            Int32 Loanbrid = 0;
            Int32 LoanGL = 0;
            Int64 LoanAccno = 0;

            DataTable TblAmountSplit = new DataTable();

            TblAmountSplit.Columns.Add("GlCode", typeof(Int32));
            TblAmountSplit.Columns.Add("GlName", typeof(String));
            TblAmountSplit.Columns.Add("AccNo", typeof(Int64));
            TblAmountSplit.Columns.Add("AccName", typeof(String));
            TblAmountSplit.Columns.Add("Receivable", typeof(Double));
            TblAmountSplit.Columns.Add("Received", typeof(Double));

            Int32 GlCode = 0;
            String GlName = "";
            Int64 AccNo = 0;
            String AccName = "";
            Double ReceivableAmt = 0;
            Double ReceivedAmt = 0;

            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                String str = " Select brid,glcode,accno from virtualaccountview where v_accno = '" + VirtualAccNo + "' ";
                DataTable dt = new DataTable();

                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Loanbrid = Convert.ToInt32(dt.Rows[0]["brid"]);  //50056; 
                    LoanGL = Convert.ToInt32(dt.Rows[0]["glcode"]); //100; 
                    LoanAccno = Convert.ToInt64(dt.Rows[0]["accno"]); // 50051002100000150;

                    String Query = "SELECT date_daybegin_workingday workingday from aoup_daybegin_def where num_daybegin_brid = " + Loanbrid + " and ROWNUM = 1 order by date_daybegin_workingday desc";
                    DataTable wd = new DataTable();

                    OracleCommand Cmd1 = new OracleCommand(Query, Con);
                    OracleDataAdapter AdpData1 = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd1;
                    AdpData.Fill(wd);

                    DateTime WorkingDate = Convert.ToDateTime(wd.Rows[0]["workingday"]);

                    string URL = "https://copass.in/EzyB_Terminal/transactions/DesktopMethods.aspx?Mode=LoanTransferTransaction&BrId=" + Loanbrid + "&Date=" + WorkingDate.ToString("MM/dd/yyyy") + "&StrParam=" + LoanGL + "*" + LoanAccno + "*" + Amount + "";

                    WebRequest request = HttpWebRequest.Create(URL);
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    String Res = reader.ReadToEnd();
                    reader.Close();

                    if (Res != "")
                    {
                        ArrayList Arr1 = new ArrayList(Res.Split('^'));

                        if (Arr1[1].ToString() != "" && Arr1.Count == 3)
                        {
                            ArrayList ArrTemp = new ArrayList(Arr1[1].ToString().Split('~'));

                            for (int i = 0; i < ArrTemp.Count - 1; i++)
                            {
                                ArrayList ArrTempNew = new ArrayList(ArrTemp[i].ToString().Split('*'));


                                if (ArrTempNew[0].ToString() != "")
                                {
                                    GlCode = Convert.ToInt32(ArrTempNew[0]);
                                }
                                if (ArrTempNew[1].ToString() != "")
                                {
                                    GlName = ArrTempNew[1].ToString();
                                }
                                if (ArrTempNew[2].ToString() != "")
                                {
                                    AccNo = Convert.ToInt64(ArrTempNew[2]);
                                }
                                if (ArrTempNew[3].ToString() != "")
                                {
                                    AccName = ArrTempNew[3].ToString();
                                }
                                if (ArrTempNew[4].ToString() != "")
                                {
                                    ReceivableAmt = Convert.ToDouble(ArrTempNew[4]);
                                }

                                if (ArrTempNew[5].ToString() != "")
                                {
                                    ReceivedAmt = Convert.ToDouble(ArrTempNew[5]);
                                }
                                if (ReceivedAmt != 0)
                                {
                                    TblAmountSplit.Rows.Add(GlCode, GlName, AccNo, AccName, ReceivableAmt, ReceivedAmt);
                                }
                            }

                            TblAmountSplit.DefaultView.Sort = "GLCode, AccNo";
                            DataView dtView = TblAmountSplit.DefaultView;

                            TblAmountSplit = dtView.ToTable();
                        }
                    }
                }

            }

            catch (Exception ex)
            {

            }


            return TblAmountSplit;
        }
        public static JObject getTransactionDetails(Int64 in_ReqID, String VirtualAccNo, Double Amount, String in_trans_id, IConfiguration _config)
        {
            JObject responseObj = new JObject();
            // ------------------   Declarations  --------------------   // 

            Double LoanAmount = 0;

            Int16 v_hoid = 0;
            Int16 v_brid = 0;
            Int16 v_glcode = 0;
            Int64 v_acc_no = 0;
            String v_acc_name = "";
            Int16 v_bankglcode = 0;
            Int64 v_bankacno = 0;
            Int16 ho_brid = 0;
            Int16 ho_glcode = 0;
            Int64 ho_accno = 0;
            String ho_accname = "";
            Int16 v_branchId = 0;
            Int16 v_branchgl = 0;
            Int64 v_branchaccno = 0;
            String v_branchaccname = "";
            String v_str = "";
            String v_str2 = "";

            String v_tag = "";
            Double v_tag_value = 0;
            String v_s_tag = "";
            Double v_s_tag_value = 0;
            Double v_bankchg_income = 0;
            Double v_bankchg_expense = 0;
            Int64 v_RowCnt = 0;
            Int16 v_bankchrgincomeglcode = 0;
            Int64 v_bankchrgincomeacno = 0;
            Int16 v_bankchrgexpenseglcode = 0;
            Int64 v_bankchrgexpenseacno = 0;
            Int16 v_trans_id = 0;
            String v_payername = "";
            String v_utrnumber = "";

            Int32 ErrorCode = 0;
            string ErrorMsg = "";

            // -------------  Value Fetch -----------------------  //
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                String str1 = " Select society, brid, glcode, accno, accname ";//into v_hoid, v_brid, v_glcode, v_acc_no, v_acc_name  
                str1 += " from virtualaccountview where v_accno = '" + VirtualAccNo + "' ";

                DataTable dt1 = new DataTable();

                OracleCommand Cmd1 = new OracleCommand(str1, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd1;
                AdpData.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    v_hoid = Convert.ToInt16(dt1.Rows[0]["society"]);
                    v_brid = Convert.ToInt16(dt1.Rows[0]["brid"]);
                    v_glcode = Convert.ToInt16(dt1.Rows[0]["glcode"]);
                    v_acc_no = Convert.ToInt64(dt1.Rows[0]["accno"]);
                    v_acc_name = dt1.Rows[0]["accname"].ToString();
                }

                String str2 = " Select a.glcode, a.accno ,a.accname ";//into ho_glcode,ho_accno,ho_accname 
                str2 += " from accountview a where a.brid = " + v_brid + " and a.glsubtypeid = 313 and a.closedate is null ";

                DataTable dt2 = new DataTable();

                OracleCommand Cmd2 = new OracleCommand(str2, Con);
                OracleDataAdapter AdpData1 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd2;
                AdpData1.Fill(dt2);

                if (dt2.Rows.Count > 0)
                {
                    ho_glcode = Convert.ToInt16(dt2.Rows[0]["glcode"]);
                    ho_accno = Convert.ToInt64(dt2.Rows[0]["accno"]);
                    ho_accname = dt2.Rows[0]["accname"].ToString();
                }

                String str3 = " Select payername, utrnumber "; // into v_payername, v_utrnumber
                str3 += " from neft_income_trans where reqid = " + in_ReqID + " ";

                DataTable dt3 = new DataTable();

                OracleCommand Cmd3 = new OracleCommand(str3, Con);
                OracleDataAdapter AdpData3 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd3;
                AdpData1.Fill(dt3);

                if (dt3.Rows.Count > 0)
                {
                    v_payername = dt3.Rows[0]["payername"].ToString();
                    v_utrnumber = dt3.Rows[0]["utrnumber"].ToString();
                }

                String str4 = " Select brid  from branchlist where hoid = " + v_hoid + " and brcategory = 5 and brtype = 1  "; // into ho_brid 

                DataTable dt4 = new DataTable();

                OracleCommand Cmd4 = new OracleCommand(str4, Con);
                OracleDataAdapter AdpData4 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd4;
                AdpData1.Fill(dt4);

                if (dt4.Rows.Count > 0)
                {
                    ho_brid = Convert.ToInt16(dt4.Rows[0]["brid"]);
                }

                String str5 = " Select a.glcode,a.accno,a.accname  "; // into v_branchgl,v_branchaccno,v_branchaccname
                str5 += " from aoup_brconfig_def c inner join branchlist b on b.brid = c.num_brconfig_brid  inner join accountview a on a.society = b.hoid ";
                str5 += " where a.brid = " + v_hoid + " and c.num_brconfig_brid = " + v_brid + " and a.accno = c.num_brconfic_bradjaccathobr and a.glsubtypeid = 313 and a.closedate is null ";

                DataTable dt5 = new DataTable();

                OracleCommand Cmd5 = new OracleCommand(str5, Con);
                OracleDataAdapter AdpData5 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd5;
                AdpData1.Fill(dt5);

                if (dt5.Rows.Count > 0)
                {
                    v_branchgl = Convert.ToInt16(dt5.Rows[0]["glcode"]);
                    v_branchaccno = Convert.ToInt64(dt5.Rows[0]["accno"]);
                    v_branchaccname = dt5.Rows[0]["accname"].ToString();
                }

                String str6 = "  Select trans_id  from trans_type where trans_value = '" + in_trans_id + "' "; // into v_trans_id  
                DataTable dt6 = new DataTable();

                OracleCommand Cmd6 = new OracleCommand(str6, Con);
                OracleDataAdapter AdpData6 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd6;
                AdpData1.Fill(dt6);

                if (dt6.Rows.Count > 0)
                {
                    v_trans_id = Convert.ToInt16(dt6.Rows[0]["trans_id"]);
                }

                String str7 = "  SELECT bankglcode, bankacno, bankchrgincomeglcode, bankchrgincomeacno, bankchrgexpenseglcode, bankchrgexpenseacno ";
                str7 += "  FROM society_neft_registration WHERE hoid = " + v_hoid + " ";
                // into v_bankglcode, v_bankacno, v_bankchrgincomeglcode, v_bankchrgincomeacno, v_bankchrgexpenseglcode, v_bankchrgexpenseacno
                DataTable dt7 = new DataTable();

                OracleCommand Cmd7 = new OracleCommand(str7, Con);
                OracleDataAdapter AdpData7 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd7;
                AdpData1.Fill(dt7);

                if (dt7.Rows.Count > 0)
                {
                    v_bankglcode = Convert.ToInt16(dt7.Rows[0]["bankglcode"]);
                    v_bankacno = Convert.ToInt64(dt7.Rows[0]["bankacno"]);
                    v_bankchrgincomeglcode = Convert.ToInt16(dt7.Rows[0]["bankchrgincomeglcode"]);
                    v_bankchrgincomeacno = Convert.ToInt64(dt7.Rows[0]["bankchrgincomeacno"]);
                    v_bankchrgexpenseglcode = Convert.ToInt16(dt7.Rows[0]["bankchrgexpenseglcode"]);
                    v_bankchrgexpenseacno = Convert.ToInt64(dt7.Rows[0]["bankchrgexpenseacno"]);
                }

                v_RowCnt = 0;

                String str8 = "  select count(*) as v_RowCnt";// into v_RowCnt " 
                str8 += "  FROM society_transfer_charges WHERE hoid = " + v_hoid + " AND bank_id = 1 AND transtype_id = " + v_trans_id + " ";
                str8 += "  AND min_amount <= " + Amount + " AND max_amount >= " + Amount + " AND effective_date <= SYSDATE and in_out_tag = 'I' ";

                DataTable dt8 = new DataTable();

                OracleCommand Cmd8 = new OracleCommand(str8, Con);
                OracleDataAdapter AdpData8 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd8;
                AdpData1.Fill(dt8);

                if (dt8.Rows.Count > 0)
                {
                    v_RowCnt = Convert.ToInt64(dt8.Rows[0]["v_RowCnt"]);
                }

                if (v_RowCnt > 0)
                {

                    String str9 = "  SELECT tag, NVL(tag_value, 0) tag_value FROM society_transfer_charges ";// into v_tag, v_tag_value  
                    str9 += "  WHERE hoid = v_hoid AND bank_id = 1 AND transtype_id = v_trans_id AND min_amount <= in_amount AND max_amount >= in_amount ";
                    str9 += "  AND effective_date <= SYSDATE and in_out_tag = 'I' ";

                    DataTable dt9 = new DataTable();

                    OracleCommand Cmd9 = new OracleCommand(str9, Con);
                    OracleDataAdapter AdpData9 = new OracleDataAdapter();
                    AdpData1.SelectCommand = Cmd9;
                    AdpData1.Fill(dt9);

                    if (dt9.Rows.Count > 0)
                    {
                        v_tag = dt9.Rows[0]["tag"].ToString();
                        v_tag_value = Convert.ToDouble(dt9.Rows[0]["tag_value"]);
                    }
                }

                v_RowCnt = 0;

                String str10 = "  SELECT count(*) as v_RowCnt FROM society_transfer_charges  WHERE hoid = '" + v_hoid + "' AND bank_id=9999 ";
                str10 += " AND transtype_id='" + v_trans_id + "' AND min_amount <= " + Amount + " AND max_amount >= " + Amount + " AND effective_date <= SYSDATE and in_out_tag='I' ";

                DataTable dt10 = new DataTable();

                OracleCommand Cmd10 = new OracleCommand(str10, Con);
                OracleDataAdapter AdpData10 = new OracleDataAdapter();
                AdpData1.SelectCommand = Cmd10;
                AdpData1.Fill(dt10);

                if (dt10.Rows.Count > 0)
                {
                    v_RowCnt = Convert.ToInt64(dt10.Rows[0]["v_RowCnt"]);

                }
                if (v_RowCnt > 0)
                {
                    String str11 = "  SELECT tag, NVL(tag_value, 0) as tag_value "; // into v_s_tag, v_s_tag_value 
                    str11 += "  FROM society_transfer_charges WHERE hoid = '" + v_hoid + "' AND bank_id = 9999 AND transtype_id = '" + v_trans_id + "' ";
                    str11 += "  AND min_amount <= " + Amount + " AND max_amount >= " + Amount + " AND effective_date <= SYSDATE and in_out_tag = 'I'and rownum = 1 order by effective_date desc ";

                    DataTable dt11 = new DataTable();

                    OracleCommand Cmd11 = new OracleCommand(str11, Con);
                    OracleDataAdapter AdpData11 = new OracleDataAdapter();
                    AdpData1.SelectCommand = Cmd11;
                    AdpData1.Fill(dt11);

                    if (dt11.Rows.Count > 0)
                    {
                        v_s_tag = dt11.Rows[0]["tag"].ToString();
                        v_s_tag_value = Convert.ToDouble(dt11.Rows[0]["tag_value"]);
                    }
                }

                if (v_s_tag == "P")
                {
                    v_bankchg_income = Convert.ToDouble((Amount * v_s_tag_value) / 100);
                }
                else
                {
                    v_bankchg_income = Convert.ToDouble(v_s_tag_value);
                }

                if (v_tag == "P")
                {
                    v_bankchg_expense = Convert.ToDouble((Amount * v_tag_value) / 100);
                }
                else
                {
                    v_bankchg_expense = Convert.ToDouble(v_tag_value);
                }
                if (v_bankchg_income != 0)
                {
                    LoanAmount = Amount - v_bankchg_income;
                }
                else
                {
                    LoanAmount = Amount;
                }

                DataTable dataLoandetails = getLoanDetails(VirtualAccNo, LoanAmount, _config);


                v_str = ho_brid + "#" + v_bankglcode + "#" + v_bankacno + "#" + (Amount * -1) + "#" + "Received by " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + ho_brid + "#" + v_bankglcode + "#" + v_bankacno + "####$"
                 + ho_brid + "#" + v_branchgl + "#" + v_branchaccno + "#" + Amount + "#" + "Received by " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + ho_brid + "#" + v_branchgl + "#" + v_branchaccno + "####";

                if (v_bankchg_expense != 0)
                {
                    v_str += v_str + "$" + ho_brid + "#" + v_bankglcode + "#" + v_bankacno + "#" + v_bankchg_expense + "# Bank Charges for " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + ho_brid + "#" + v_bankglcode + "#" + v_bankacno + "####$" +
                              ho_brid + "#" + v_bankchrgexpenseglcode + "#" + v_bankchrgexpenseacno + "#" + (v_bankchg_expense * -1) + "# Bank Charges for " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + ho_brid + "#" + v_bankchrgexpenseglcode + "#" + v_bankchrgexpenseacno + "####";
                }

                v_str2 = v_brid + "#" + ho_glcode + "#" + ho_accno + "#" + (Amount * -1) + "#" + "Received by " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + v_brid + "#" + ho_glcode + "#" + ho_accno + "####";
                //"$" + v_brid + "#" + v_glcode + "#" + v_acc_no + "#" + Amount + "#" + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + v_brid + "#" + v_glcode + "#" + v_acc_no + "####";

                if (dataLoandetails.Rows.Count > 0)
                {
                    for (int i = 0; i < dataLoandetails.Rows.Count; i++)
                    {
                        v_str2 += "$" + v_brid + "#" + dataLoandetails.Rows[i]["GlCode"] + "#" + dataLoandetails.Rows[i]["AccNo"] + "#" + dataLoandetails.Rows[i]["Received"] + "#" + "Received by " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + v_brid + "#" + v_glcode + "#" + v_acc_no + "####";

                    }
                }
                else
                {
                    v_str2 += v_str2 + "$" + v_brid + "#" + v_glcode + "#" + v_acc_no + "#" + Amount + "#" + "Received by " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + v_brid + "#" + v_glcode + "#" + v_acc_no + "####";
                }

                if (v_bankchg_income != 0)
                {
                    v_str2 += v_str2 + "$" + v_brid + "#" + v_glcode + "#" + v_acc_no + "#" + (v_bankchg_income * -1) + "#" + "Bank Charges for " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + v_brid + "#" + v_glcode + "#" + v_acc_no + "####$" +
                              v_brid + "#" + v_bankchrgincomeglcode + "#" + v_bankchrgincomeacno + "#" + v_bankchg_income + "#" + "Bank Charges for " + in_trans_id + " from " + v_payername + " UTR-" + v_utrnumber + "#" + v_brid + "#" + v_bankchrgincomeglcode + "#" + v_bankchrgincomeacno + "####";
                }

                OracleCommand Cmd12 = new OracleCommand();
                Cmd12 = new OracleCommand("sp_neft_loan_income", Con);
                Cmd12.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd12.Parameters.Add("in_ReqID", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd12.Parameters.Add("in_VirtualAccNo", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                Cmd12.Parameters.Add("ho_brid", OracleType.Number, 20).Direction = ParameterDirection.Input;
                Cmd12.Parameters.Add("v_brid", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd12.Parameters.Add("v_str", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                Cmd12.Parameters.Add("v_str2", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                Cmd12.Parameters.Add("out_ErrorCode", OracleType.Number).Direction = ParameterDirection.Output;
                Cmd12.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 50).Direction = ParameterDirection.Output;

                Cmd12.Parameters["in_ReqID"].Value = in_ReqID;
                Cmd12.Parameters["in_VirtualAccNo"].Value = VirtualAccNo;
                Cmd12.Parameters["ho_brid"].Value = ho_brid;
                Cmd12.Parameters["v_brid"].Value = v_brid;
                Cmd12.Parameters["v_str"].Value = v_str;
                Cmd12.Parameters["v_str2"].Value = v_str2;

                Cmd12.ExecuteNonQuery();
                ErrorCode = Convert.ToInt32(Cmd12.Parameters["out_ErrorCode"].Value);
                ErrorMsg = Cmd12.Parameters["out_ErrorMsg"].Value.ToString();

                responseObj = new JObject(
                                new JProperty("ErrCode", ErrorCode),
                                new JProperty("ErrMsg", ErrorMsg)
                              );

                Con.Close();

            }

            catch (Exception ex)
            {

            }


            return responseObj;
        }

        public static JObject updateTrans(string reqid,string utrno, IConfiguration _config)
        {
            string retVal = "";
            JObject Jdata = new JObject();
            string ErrMsg = "";
            int ErrCode = 0;
            string dataobj;
            string is_verified = "";
            JObject responseObj = new JObject();

            try
            {
                OracleCommand Cmd = new OracleCommand();
                using (OracleConnection Con = getoracleOpenConn(_config))
                {
                    Cmd = new OracleCommand("sp_trans_narration_update", Con);
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Cmd.Parameters.Add("in_reqid", OracleType.Number).Direction = ParameterDirection.Input;
                    Cmd.Parameters.Add("in_utrno", OracleType.Number).Direction = ParameterDirection.Input;

                    Cmd.Parameters.Add("out_errorcode", OracleType.Number).Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 300).Direction = ParameterDirection.Output;

                    Cmd.Parameters["in_reqid"].Value = reqid;
                    Cmd.Parameters["in_utrno"].Value = utrno;

                    Cmd.ExecuteNonQuery();
                    getoracleCloseConn(_config);
                }
                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value);
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();

                responseObj = new JObject(new JObject(new JProperty("Narration_update", new JObject(
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));

            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData(ErrCode, ErrMsg);

            }

            return responseObj;
        }
    }
}

