using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ExternalAPIs.DA
{
    public class CIVSDA
    {
        // oracle connection
        public static string getoracleconn(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["OrcaleConnectionlocal"];
        }
        public static OracleConnection getoracleOpenConn(IConfiguration _config)
        {
            string conStr = _config.GetSection("Appsetting")["OrcaleConnectionlocal"];
            OracleConnection Con = new OracleConnection(conStr);
            if (Con.State != ConnectionState.Open)
            {
                Con.Open();
            }

            return Con;
        }
        public static OracleConnection getoracleCloseConn(IConfiguration _config)
        {
            string conStr = _config.GetSection("Appsetting")["OrcaleConnectionlocal"];
            OracleConnection Con = new OracleConnection(conStr);

            return Con;
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
        public static JObject CIVSEntry(string Str,Int32 MerchantID,  string ReferenceNumber, string ReportType, string FirstName, string MiddleName , string LastName,string GenderType, string BirthDate, string Address, string StateCode, string PinCode, string TelephoneNumber, string AadharNumber, string PANNumber, string VoterID, string RationCardNumber, string PassportNumber, string LoanAmount, IConfiguration _config)
        {
            string retVal = "";
            string is_verified = "";
            string username = "";
            int OTP = 0;
            string ErrMsg = "";
            int ErrCode = 0;
            string name = "";
            string Request_str = "";
            JObject responseObj = new JObject();
            string errormsgstr = "";
            Int32 numMerchantID = Convert.ToInt32(MerchantID);
            Int32 RefID=0;
            string datavalue = "";
            string Status_str = "";
            string error = ""; string xml_response = "";
            string pdfdata = "";
            string pdfurl_data = "";
            try
            {

                if(MerchantID.ToString()=="")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr +" Merchant Id Can Not Be Blank!";
                    goto JJ;
                }
           
                if (ReferenceNumber == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " Reference Number Can Not Be Blank!";
                    goto JJ;
                }

                if (ReportType == "" || (ReportType != "3" && ReportType != "8"))
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " ReportType Can Not Be Blank! and ReportType Must be 3 or 8.";
                    goto JJ;
                }
                if (FirstName == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " FirstName Can Not Be Blank!";
                    goto JJ;
                }
                if (MiddleName == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " MiddleName Can Not Be Blank!";
                    goto JJ;
                }
                if (LastName == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " MiddleName Can Not Be Blank!";
                    goto JJ;
                }
                if (GenderType == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " GenderType Can Not Be Blank!";
                    goto JJ;
                }
                if (BirthDate == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " BirthDate Can Not Be Blank!";
                    goto JJ;
                }
                if (Address == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " Address Can Not Be Blank!";
                    goto JJ;
                }
                if (StateCode == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " State Code Can Not Be Blank!";
                    goto JJ;
                }

                if (PinCode == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " Pin Code Can Not Be Blank!";
                    goto JJ;
                }

                if (PinCode == "")
                {
                    ErrCode = 120;
                    errormsgstr = errormsgstr + " Pin Code Can Not Be Blank!";
                    goto JJ;
                }

            JJ:
               
                ErrMsg = errormsgstr;
                if(ErrCode !=120)
                    {

                    Request_str = MerchantID + "|" +  ReferenceNumber + "|" +
                              ReportType + "|" + FirstName + "|" + MiddleName + "|" + LastName + "|" + GenderType + "|" +
                              Convert.ToDateTime(BirthDate).ToString("dd/MMM/yyyy") + "|" + Address + "|" + StateCode + "|" + PinCode + "|" +
                              TelephoneNumber + "|" + AadharNumber + "|" + PANNumber + "|" + VoterID + "|" +
                              RationCardNumber + "|" + PassportNumber + "|" + LoanAmount;


                    using (OracleConnection Con = getoracleOpenConn(_config))
                    {



                        OracleCommand Cmd = new OracleCommand("sp_cibil_reqres_data", Con);
                        Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        Cmd.Parameters.Add("in_flag", OracleType.VarChar,20).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("in_req_id", OracleType.Number).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("in_br_id", OracleType.Number).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("in_reference", OracleType.Number).Direction = ParameterDirection.Input;

                        Cmd.Parameters.Add("in_req_str", OracleType.VarChar,4000 ).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("in_pdfurl", OracleType.VarChar, 4000).Direction = ParameterDirection.Input;
                        Cmd.Parameters.Add("in_api_res", OracleType.VarChar, 200).Direction = ParameterDirection.Input;

                        Cmd.Parameters.Add("out_error_code", OracleType.Number).Direction = ParameterDirection.Output;
                        Cmd.Parameters.Add("out_error_msg", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                        Cmd.Parameters.Add("out_reqid", OracleType.Number).Direction = ParameterDirection.Output;

                        Cmd.Parameters["in_flag"].Value = "Insert";
                        Cmd.Parameters["in_req_id"].Value = 0;
                        Cmd.Parameters["in_reference"].Value = ReferenceNumber;
                        Cmd.Parameters["in_br_id"].Value = Convert.ToInt16(MerchantID);
                        Cmd.Parameters["in_req_str"].Value = Request_str;
                        Cmd.Parameters["in_pdfurl"].Value = "0";
                        Cmd.Parameters["in_api_res"].Value = "0";


                        Cmd.ExecuteNonQuery();
                        getoracleCloseConn(_config);
                        ErrCode = Convert.ToInt32(Cmd.Parameters["out_error_code"].Value.ToString());
                        ErrMsg = Cmd.Parameters["out_error_msg"].Value.ToString();
                        RefID = Convert.ToInt32(Cmd.Parameters["out_reqid"].Value.ToString());


                    }

                    if (ErrCode == 100)
                    {

                        string strdata = BI.CIVSAPIs.Call_cibilurl(Request_str, MerchantID,ReferenceNumber, RefID.ToString(), ReportType, FirstName, MiddleName, LastName, GenderType, BirthDate, Address, StateCode, PinCode, TelephoneNumber, AadharNumber, PANNumber, VoterID, RationCardNumber, PassportNumber, LoanAmount, _config, out  Status_str, out  error, out  xml_response, out  pdfdata, out pdfurl_data);
                        datavalue = strdata;
                        string api_status = "";
                        if (Status_str== "S")
                        {
                            api_status = "Success";
                        }
                        else
                        {
                            api_status = "Fail";
                        }
                            using (OracleConnection Con = getoracleOpenConn(_config))
                            {



                                OracleCommand Cmd = new OracleCommand("sp_cibil_reqres_data", Con);
                                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                Cmd.Parameters.Add("in_flag", OracleType.VarChar, 20).Direction = ParameterDirection.Input;
                                Cmd.Parameters.Add("in_req_id", OracleType.Number).Direction = ParameterDirection.Input;
                                Cmd.Parameters.Add("in_br_id", OracleType.Number).Direction = ParameterDirection.Input;
                                Cmd.Parameters.Add("in_req_str", OracleType.VarChar, 4000).Direction = ParameterDirection.Input;
                                Cmd.Parameters.Add("in_reference", OracleType.Number).Direction = ParameterDirection.Input;
                            Cmd.Parameters.Add("in_pdfurl", OracleType.VarChar, 4000).Direction = ParameterDirection.Input;

                            Cmd.Parameters.Add("in_api_res", OracleType.VarChar, 200).Direction = ParameterDirection.Input;

                                Cmd.Parameters.Add("out_error_code", OracleType.Number).Direction = ParameterDirection.Output;
                                Cmd.Parameters.Add("out_error_msg", OracleType.VarChar, 500).Direction = ParameterDirection.Output;
                                Cmd.Parameters.Add("out_reqid", OracleType.Number).Direction = ParameterDirection.Output;

                                Cmd.Parameters["in_flag"].Value = "Update";
                                Cmd.Parameters["in_req_id"].Value = RefID;
                                Cmd.Parameters["in_reference"].Value = ReferenceNumber;
                                Cmd.Parameters["in_br_id"].Value = Convert.ToInt16(MerchantID);
                                Cmd.Parameters["in_req_str"].Value = Request_str;
                            Cmd.Parameters["in_pdfurl"].Value = pdfurl_data;
                            Cmd.Parameters["in_api_res"].Value = api_status;


                                Cmd.ExecuteNonQuery();
                                getoracleCloseConn(_config);
                                ErrCode = Convert.ToInt32(Cmd.Parameters["out_error_code"].Value.ToString());
                                ErrMsg = Cmd.Parameters["out_error_msg"].Value.ToString();
                                RefID = Convert.ToInt32(Cmd.Parameters["out_reqid"].Value.ToString());


                            }

                    }
                    else if (ErrCode == 500)
                    {
                        datavalue = "Contact to Administrator";
                    }
                   





                }
                


            }
            catch (Exception ex)
            {
                ErrCode = 121;
                ErrMsg = "Unable to proceed - webserve";
            
            }

        

            responseObj = new JObject(new JObject(new JProperty("ApiOutput", new JObject(
                               new JProperty("xmlresponse", xml_response),
                               new JProperty("status", Status_str),
                               new JProperty("PDF_DATA_BASE64_STRING", pdfdata),
                               new JProperty("xmlerror", error),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
               ))));
        
          return responseObj;
        }


        public static Boolean ValidateChecksum(string Request, Int32 MerchantID, string ClientChecksum, IConfiguration _config)
        {
            bool Response = false;
            try
            {
                using (OracleConnection Con = getoracleOpenConn(_config))
                {

                    Request = Request.TrimStart('{');
                    string[] RequestArr = Request.Split(',');
                    String qur = "select var_checksum from aoup_cibilchecksum_det where num_companycode='" + MerchantID + "'";
                    DataTable TblMerchant = new DataTable();

                    OracleCommand Cmd = new OracleCommand(qur, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(TblMerchant);
                    if (TblMerchant.Rows.Count > 0)
                    {

                        String Checksumkey = TblMerchant.Rows[0]["var_checksum"].ToString();
                        Request = Request.Remove(Request.LastIndexOf(',') + 1);

                        String NewRquest = Request.Remove(Request.Length - 1);

                        String ServerChecksum = GetHMACSHA256(NewRquest, Checksumkey);

                        if (ServerChecksum.ToUpper() == ClientChecksum.ToUpper())
                        {
                            Response = true;
                        }
                        else
                        {
                            Response = false;
                        }
                    }
                    else
                    {
                        Response = false;
                    }
                }
                
                return Response;
            }

            catch (Exception ex)
            {
                return false;
            }

        }

        public static string GetHMACSHA256(string text, string key)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] hashValue;
            byte[] keybyt = encoder.GetBytes(key);
            byte[] message = encoder.GetBytes(text);

            HMACSHA256 hashString = new HMACSHA256(keybyt);
            string hex = "";

            hashValue = hashString.ComputeHash(message);

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }


        public static string getapikey(Int32 MerchantID, IConfiguration _config)

        {
           string responseObj = "";
            string apikey = "";
            try
            {
                using (OracleConnection Con = getoracleCloseConn(_config))
                {
                    string str = " Select apikey from cibil_br_apikey where br_id=" + MerchantID.ToString()  + " ";
                    

                    DataTable dt = new DataTable();

                    OracleCommand Cmd = new OracleCommand(str, Con);
                    OracleDataAdapter AdpData = new OracleDataAdapter();
                    AdpData.SelectCommand = Cmd;
                    AdpData.Fill(dt);
                    getoracleCloseConn(_config);
                    
                    if (dt.Rows.Count >0)
                    {
                        apikey = dt.Rows[0]["apikey"].ToString();
                    }
                    else
                    {
                        apikey = "Invalid";
                    }
                }
            }

            catch (Exception ex)
            {
                apikey = "Db connection failed";
            }

             responseObj = apikey;

            return responseObj;
        }

        //public static void CallProcedure(String in_CibilIntID, string in_ReqStr, string in_ResStr, string in_Status, string in_ControlNo, string in_MFI_HIT, string in_CONS_HIT, string in_Error, string in_Mode, string in_InsBy, out Int32 ErrorCode, out String ErrorMsg)
        //{
        //    GetCon Con = new GetCon();
        //    Con.OpenConn();

        //    OracleCommand Cmd = new OracleCommand("aoup_CibilReqRes_ins", Con.connection);
        //    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    Cmd.Parameters.Add("in_CibilIntID", OracleType.Number, 10);
        //    Cmd.Parameters.Add("in_ReqStr", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_ResStr", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_Status", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_ControlNo", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_MFI_HIT", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_CONS_HIT", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_Error", OracleType.VarChar);
        //    Cmd.Parameters.Add("in_Mode", OracleType.Number, 10);
        //    Cmd.Parameters.Add("in_InsBy", OracleType.VarChar);
        //    Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 10);
        //    Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 300);

        //    Cmd.Parameters["in_CibilIntID"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_ReqStr"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_ResStr"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_Status"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_ControlNo"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_MFI_HIT"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_CONS_HIT"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_Error"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_Mode"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["in_InsBy"].Direction = ParameterDirection.Input;
        //    Cmd.Parameters["out_ErrorCode"].Direction = ParameterDirection.Output;
        //    Cmd.Parameters["out_ErrorMsg"].Direction = ParameterDirection.Output;

        //    Cmd.Parameters["in_CibilIntID"].Value = in_CibilIntID;
        //    Cmd.Parameters["in_ReqStr"].Value = in_ReqStr;
        //    if (in_ResStr == "")
        //    {
        //        Cmd.Parameters["in_ResStr"].Value = DBNull.Value;
        //    }
        //    else
        //    {
        //        Cmd.Parameters["in_ResStr"].Value = in_ResStr;
        //    }
        //    if (in_Status == "")
        //    {
        //        Cmd.Parameters["in_Status"].Value = DBNull.Value;
        //    }
        //    else
        //    {
        //        Cmd.Parameters["in_Status"].Value = in_Status;
        //    }

        //    if (in_ControlNo == "")
        //    {
        //        Cmd.Parameters["in_ControlNo"].Value = DBNull.Value;
        //    }
        //    else
        //    {
        //        Cmd.Parameters["in_ControlNo"].Value = in_ControlNo;
        //    }

        //    if (in_MFI_HIT == "")
        //    {
        //        Cmd.Parameters["in_MFI_HIT"].Value = DBNull.Value;
        //    }
        //    else
        //    {
        //        Cmd.Parameters["in_MFI_HIT"].Value = in_MFI_HIT;
        //    }

        //    if (in_CONS_HIT == "")
        //    {
        //        Cmd.Parameters["in_CONS_HIT"].Value = DBNull.Value;
        //    }
        //    else
        //    {
        //        Cmd.Parameters["in_CONS_HIT"].Value = in_CONS_HIT;
        //    }

        //    if (in_Error == "")
        //    {
        //        Cmd.Parameters["in_Error"].Value = DBNull.Value;
        //    }
        //    else
        //    {
        //        Cmd.Parameters["in_Error"].Value = in_Error;
        //    }

        //    Cmd.Parameters["in_Mode"].Value = in_Mode;
        //    Cmd.Parameters["in_InsBy"].Value = in_InsBy;

        //    Cmd.ExecuteNonQuery();
        //    Con.CloseConn();

        //    ErrorCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value);
        //    ErrorMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
        //}



    }


}
    