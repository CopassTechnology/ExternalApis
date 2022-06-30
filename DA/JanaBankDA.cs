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
    public class JanaBankDA
    {
        public static string getoracleconn(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["OrcaleConnectionlive"];
        }

        //--------------------Babita Jana Bank Implementation---------------------------//
        public static JObject getRequestDetails(string ReqId, IConfiguration _config)
        {
            int errorcode = 0;
            JObject Jdata = new JObject();

            try
            {

                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                String str = "select num_fundtrf_drbrid brid, num_fundtrf_drglcode drglcode, num_fundtrf_draccno draccno, num_fundtrf_amount amount, ";
                str += "var_fundtrf_remark remark, var_beneficiary_benefname benefname, num_customer_cellno mobile, num_beneficiary_accno bankaccno, ";
                str += "var_beneficiary_ifsccode ifsccode ";
                str += "from mapp_fundtrf_def ";
                str += "inner join mapp_beneficiary_def on num_beneficiary_id = num_fundtrf_beneficiaryid ";
                str += "inner join aoup_customer_def on num_customer_custinternalid = num_beneficiary_custintid ";
                str += "where num_fundtrf_reqid = " + ReqId;


                DataTable dt = new DataTable();

                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    errorcode = -100;

                         Jdata = new JObject(
                                 new JProperty("BRID", dt.Rows[0]["brid"].ToString()),
                                 new JProperty("DRGLCODE", dt.Rows[0]["drglcode"].ToString()),
                                new JProperty("DRACCNO", dt.Rows[0]["draccno"].ToString()),
                                new JProperty("AMOUNT", dt.Rows[0]["amount"].ToString()),
                                new JProperty("REMARK", dt.Rows[0]["remark"].ToString()),
                                new JProperty("BENEFNAME", dt.Rows[0]["benefname"].ToString()),
                                new JProperty("MOBILE", dt.Rows[0]["mobile"].ToString()),
                                new JProperty("BANKACCNO", dt.Rows[0]["bankaccno"].ToString()),
                                new JProperty("IFSCCODE", dt.Rows[0]["ifsccode"].ToString())
                            );
                }
                else
                {

                        Jdata = new JObject(
                                new JProperty("BRID", ""),
                                new JProperty("DRGLCODE", ""),
                                new JProperty("DRACCNO", ""),
                                new JProperty("AMOUNT", ""),
                                new JProperty("REMARK", ""),
                                new JProperty("BENEFNAME", ""),
                                new JProperty("MOBILE", ""),
                                new JProperty("BANKACCNO", ""),
                                new JProperty("IFSCCODE", ""),
                                new JProperty("errorcode", errorcode)
                            );

                }

            }

            catch (Exception ex)
            {

            }

            write_log_error("JanaBankDA.cs | getRequestDetails |", Jdata.ToString());
            return Jdata;
        }

        public static JObject getRequestDetailsAPI(string ReqId, IConfiguration _config)
        {
            int errorcode = 0;
            JObject Jdata = new JObject();

            try
            {

                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                String str = "select num_fundtrf_drbrid brid, num_fundtrf_drglcode drglcode, num_fundtrf_draccno draccno, num_fundtrf_amount amount, ";
                str += "var_fundtrf_remark remark, var_beneficiary_benefname benefname, num_customer_cellno mobile, num_beneficiary_accno bankaccno, ";
                str += "var_beneficiary_ifsccode ifsccode ";
                str += "from mapp_fundtrf_def ";
                str += "inner join mapp_beneficiary_def on num_beneficiary_id = num_fundtrf_beneficiaryid ";
                str += "inner join aoup_customer_def on num_customer_custinternalid = num_beneficiary_custintid ";
                str += "where num_fundtrf_reqid = " + ReqId + " and var_fundtrf_status = 'P' and num_fundtrf_trnsno is not null ";


                DataTable dt = new DataTable();

                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    errorcode = -100;
                    Jdata = new JObject(
                            new JProperty("BRID", dt.Rows[0]["brid"].ToString()),
                            new JProperty("DRGLCODE", dt.Rows[0]["drglcode"].ToString()),
                            new JProperty("DRACCNO", dt.Rows[0]["draccno"].ToString()),
                            new JProperty("AMOUNT", dt.Rows[0]["amount"].ToString()),
                            new JProperty("REMARK", dt.Rows[0]["remark"].ToString()),
                            new JProperty("BENEFNAME", dt.Rows[0]["benefname"].ToString()),
                            new JProperty("MOBILE", dt.Rows[0]["mobile"].ToString()),
                            new JProperty("BANKACCNO", dt.Rows[0]["bankaccno"].ToString()),
                            new JProperty("IFSCCODE", dt.Rows[0]["ifsccode"].ToString()),
                            new JProperty("errorcode", errorcode)
                         );

                }
                else
                {

                    Jdata = new JObject(
                            new JProperty("BRID", ""),
                            new JProperty("DRGLCODE", ""),
                            new JProperty("DRACCNO", ""),
                            new JProperty("AMOUNT", ""),
                            new JProperty("REMARK", ""),
                            new JProperty("BENEFNAME", ""),
                            new JProperty("MOBILE", ""),
                            new JProperty("BANKACCNO", ""),
                            new JProperty("IFSCCODE", ""),
                            new JProperty("errorcode", errorcode)
                        );

                }

            }

            catch (Exception ex)
            {

            }

            write_log_error("JanaBankDA.cs | getRequestDetailsAPI |", Jdata.ToString());

            return Jdata;
        }
        public static JObject CallReqProc(String ReqId, Int32 BrId, Int32 GLCode, Int64 AccNo, String BankAccNo, Int32 BankBrId, double Amount, string Narration,
            string InsBy, IConfiguration _config)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            Int64 UniqueReqNo = 0;

            JObject JdataOutput = new JObject();

            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                OracleCommand Cmd = new OracleCommand("aoup_YB_req_ins", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("in_ReqId", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_BrId", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_GLCode", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_AccNo", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_BankAccNo", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_BankBrId", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_Amount", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_Narration", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_InsBy", OracleType.VarChar).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_ErrorCode", OracleType.Number, 20).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_ErrorMsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_UniqueReqNo", OracleType.Number, 20).Direction = ParameterDirection.Output;

                Cmd.Parameters["in_ReqId"].Value = ReqId;
                Cmd.Parameters["in_BrId"].Value = BrId;
                Cmd.Parameters["in_GLCode"].Value = GLCode;
                Cmd.Parameters["in_AccNo"].Value = AccNo;
                Cmd.Parameters["in_BankAccNo"].Value = BankAccNo;
                Cmd.Parameters["in_BankBrId"].Value = BankBrId;
                Cmd.Parameters["in_Amount"].Value = Amount;
                Cmd.Parameters["in_Narration"].Value = Narration;
                Cmd.Parameters["in_InsBy"].Value = InsBy;

                Cmd.ExecuteNonQuery();
                Con.Close();

                ErrCode = Convert.ToInt32(Cmd.Parameters["out_ErrorCode"].Value.ToString());
                ErrMsg = Cmd.Parameters["out_ErrorMsg"].Value.ToString();
                UniqueReqNo = Convert.ToInt64(Cmd.Parameters["out_UniqueReqNo"].Value.ToString());
            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserver";
            }

            JdataOutput = new JObject(
               new JProperty("ErrCode", ErrCode),
               new JProperty("ErrMsg", ErrMsg),
               new JProperty("UniqueReqNo", UniqueReqNo)
               );

            write_log_error("JanaBankDA.cs | CallReqProc |", JdataOutput.ToString());

            return JdataOutput;
        }

        public static string GenerateRandomUserRefNo()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[30];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }


        public static JObject UpdateResString(Int64 UniqueReqNo, string ResponseStr, string UniqueResNo, string StatusCode,
           string SubStatusCode, string BankReferenceNo, string Reason, string transferType, IConfiguration _config)
        {

            int ErrCode = 0;
            string ErrMsg = "";

            JObject JdataOutput = new JObject();

            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                OracleCommand Cmd = new OracleCommand("aoup_YB_res_ins", Con);
                Cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Cmd.Parameters.Add("in_UniqueReqNo", OracleType.Number).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_ResString", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_UniqueResNo", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_StatusCode", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_SubStatusCode", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_BankReferenceNo", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_Reason", OracleType.VarChar).Direction = ParameterDirection.Input;
                Cmd.Parameters.Add("in_TransferType", OracleType.VarChar).Direction = ParameterDirection.Input;

                Cmd.Parameters.Add("out_errorcode", OracleType.Number, 20).Direction = ParameterDirection.Output;
                Cmd.Parameters.Add("out_errormsg", OracleType.VarChar, 200).Direction = ParameterDirection.Output;

                Cmd.Parameters["in_UniqueReqNo"].Value = UniqueReqNo;
                Cmd.Parameters["in_ResString"].Value = ResponseStr;
                Cmd.Parameters["in_UniqueResNo"].Value = UniqueResNo;
                Cmd.Parameters["in_StatusCode"].Value = StatusCode;
                Cmd.Parameters["in_SubStatusCode"].Value = SubStatusCode;
                Cmd.Parameters["in_BankReferenceNo"].Value = BankReferenceNo;

                if (Reason == null || Reason == "")
                {
                    Cmd.Parameters["in_Reason"].Value = DBNull.Value;
                }

                else
                {
                    Cmd.Parameters["in_Reason"].Value = Reason;
                }

                if (transferType == null || transferType == "")
                {
                    Cmd.Parameters["in_TransferType"].Value = DBNull.Value;
                }

                else
                {
                    Cmd.Parameters["in_TransferType"].Value = transferType;
                }

                Cmd.ExecuteNonQuery();
                Con.Close();

                ErrCode = Convert.ToInt32(Cmd.Parameters["out_errorcode"].Value.ToString());
                ErrMsg = Cmd.Parameters["out_errormsg"].Value.ToString();

            }
            catch (Exception ex)
            {
                ErrCode = 0;
                ErrMsg = "Unable to proceed - webserver";
            }

            JdataOutput = new JObject(
               new JProperty("ErrCode", ErrCode),
               new JProperty("ErrMsg", ErrMsg)
               );

            write_log_error("JanaBankDA.cs | UpdateResString |", JdataOutput.ToString());

            return JdataOutput;

        }


        public static void write_log_error(string sourceObject, string description)
        {
            string DATA_LOG = "C://COPASS/JanaBank/";
            StreamWriter sw = null;

            try
            {
                DirectoryInfo dir = new DirectoryInfo(DATA_LOG);

                if (dir.Exists)
                {
                    string filePath = DATA_LOG + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " Error.log";

                    sw = new StreamWriter(filePath, true);
                    DateTime dtNow = DateTime.Now;
                    sw.WriteLine("-----------------------------------------------------");
                    sw.WriteLine(dtNow.ToString("dd-MM-yyyy HH:mm:ss") + " " + sourceObject + " " + description);
                    sw.Flush();
                    sw.Dispose();
                }
            }

            catch (Exception Ex)
            {
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }
    }
}

