using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ExternalAPIs.DA
{
    public class WorkFlowRdDA
    {
        public static string getoracleconn(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["OrcaleConnectionlive"];
        }
        public static JObject handleErrorData(string outprint, int ErrCode, string ErrMsg, IConfiguration _config)
        {
            JObject Outputdata = new JObject(new JProperty("ErrCode", ErrCode),
                new JProperty("ErrMsg", ErrMsg));
            string strdata = Outputdata.ToString();
            return Outputdata;
        }
        public static JObject handleErrorData_1(int ErrCode, string ErrMsg)
        {
            JObject Outputdata = new JObject(new JProperty("ErrCode", ErrCode),
                new JProperty("ErrMsg", ErrMsg));
            return Outputdata;
        }
        public static JObject getGroupDetails(string HoId, string CustomerId, IConfiguration _config)
        {
            Int16 errorcode = 0;
            JObject Jdata = new JObject();
            int ErrCode = 0;
            string ErrMsg = "";
            try
            {
                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                String str = " Select l.brid,l.groupid from aoup_customer_def c  ";
                str += " inner join loanview l on l.custid = c.num_customer_custinternalid inner join branchlist b on b.brid = l.brid ";
                str += "  where b.hoid = " + HoId + " and c.num_customer_customerid = " + CustomerId + " and rownum = 1 ";
                DataTable dt = new DataTable();

                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    errorcode = -100;

                    Jdata = new JObject(
                               new JProperty("Brid", dt.Rows[0]["brid"].ToString()),
                               new JProperty("GroupId", dt.Rows[0]["groupid"].ToString()),
                               new JProperty("errorcode", errorcode)
                              );

                }
                else
                {
                    errorcode = 0;

                    Jdata = new JObject(
                                              new JProperty("Brid", 0),
                                              new JProperty("GroupId", 0),
                                              new JProperty("errorcode", errorcode)
                                             );
                }


            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
                Jdata = handleErrorData("transaction_data", ErrCode, ErrMsg, _config);
            }

            
            return Jdata;
        }

        public static JObject getCustomerDetails(Int64 Brid, Int64 GroupId, IConfiguration _config)
        {
            int errorcode = 0;
            JObject Jdata = new JObject();
            JObject responseObj = new JObject();
            int ErrCode = 0;
            string ErrMsg = "";
            try
            {

                string constr = getoracleconn(_config);
                OracleConnection Con = new OracleConnection(constr);
                Con.Open();

                String str = " With GroupDetails as ( ";
                str += " Select c.num_customer_customerid customerid, c.num_customer_custinternalid custinternalid, num_customer_cellno Cellno, ";
                str += " l.brid,l.glcode,To_char(l.accno) accno,l.groupname,l.accname,l.loanamount,l.groupid ";
                str += " from aoup_customer_def c inner join loanview l on l.custid = c.num_customer_custinternalid ";
                str += " where l.brid = " + Brid + " and l.groupid = " + GroupId + " ), ";

                str += " BankDetails as ( ";
                str += " select num_custbankdet_custinternalid custinternalid,var_bank_bankname bankName, var_bankbranch_bankbranchname bankbranchName, ";
                str += " To_char(num_custbankdet_accno) bankaccno, var_bankbranch_ifsccode ifsccode, var_custbankdet_accname bankaccname, ";
                str += " Case when num_custbankdet_acctype = 1 then 'Saving Account' when num_custbankdet_acctype = 2 then 'Current Account' when num_custbankdet_acctype = 3 then 'CC Account' end acctype ";
                str += " from aoup_customerbankdet_def ";
                str += " inner join GroupDetails g on g.custinternalid = num_custbankdet_custinternalid ";
                str += " left outer join aoup_bankbranch_def ON num_custbankdet_bankbranchid = num_bankbranch_bankbranchid ";
                str += " inner join aoup_bank_def on num_bank_bankid = num_bankbranch_bankid ";
                str += " left join aoup_nachbankmst_def on num_nachbankmst_bankid = num_custbankdet_nachbankid ";
                str += " where num_custbankdet_custinternalid = g.custinternalid ), ";

                str += " LoanDetails as ( ";
                str += " Select d.customerid,d.custinternalid,d.Cellno,d.brid LoanBrid, d.glcode LoanGl, To_char(d.accno) LoanAccno, d.groupname,d.accname LoanAccName, d.loanamount LoanAmt, ";
                str += " ABS(a.currentbal) LoanCurrentbal, d.groupid   from accountview a ";
                str += " inner join GroupDetails d on d.brid = a.brid and d.custinternalid = a.custinternalid and d.glcode = a.glcode and d.accno = a.accno ";
                str += " where a.brid = " + Brid + "  and d.groupid = " + GroupId + " ), ";

                str += " LoanDues as ( ";
                str += " Select m.num_mthendlndue_brid mbrid, date_mthendlndue_date mdate,m.num_mthendlndue_glcode mglcode, m.num_mthendlndue_accno maccno, ";
                str += " ABS(NVL(m.num_mthendlndue_dueamt, 0)) dueamt,ABS(NVL(m.num_mthendlndue_dueint, 0)) dueint,ABS(NVL(m.num_mthendlndue_dueinstno, 0)) dueinstno ";
                str += "  from aoup_monthendloandue_def m ";
                str += " left join  LoanDetails l on l.LoanBrid = m.num_mthendlndue_brid and l.LoanGl = m.num_mthendlndue_glcode and l.LoanAccno = m.num_mthendlndue_accno ";
                str += " where m.num_mthendlndue_brid = l.LoanBrid and m.num_mthendlndue_glcode = l.LoanGl and m.num_mthendlndue_accno = l.LoanAccno and m.date_mthendlndue_date in ( ";
                str += "  select max(date_daybegin_workingday) from aoup_daybegin_def where num_daybegin_brid = " + Brid + " and date_daybegin_endofficerdate is not null ) ), ";

                //str += " DepositDetails as ( ";
                //str += " Select d.customerid,d.custinternalid,d.groupid,d.groupname,d.LoanAccName,d.Cellno,d.LoanBrid,d.LoanGl,To_char(d.LoanAccno) LoanAccno,  ";
                //str += " d.LoanAmt,d.LoanCurrentbal,l.dueamt,l.dueint,l.dueinstno,l.mdate as LoanDueDate, ";
                //str += " a.brid Depobrid, a.glcode DepoGL, To_char(a.accno) DepoAccno, ABS(a.currentbal) DepoCurrentbal, ";
                //str += " b.bankName,b.bankbranchName,b.bankaccno,b.ifsccode,b.bankaccname,b.acctype ";
                //str += " from accountview a ";
                //str += " left join LoanDetails d on d.LoanBrid = a.brid and d.custinternalid = a.custinternalid ";
                //str += " left join BankDetails b on b.custinternalid = d.custinternalid ";
                //str += " left join LoanDues l on l.mbrid = d.LoanBrid and l.mglcode = d.LoanGl and l.maccno = d.LoanAccno ";
                //str += " where a.brid = " + Brid + "  and a.gltypeid = 10 and d.custinternalid = a.custinternalid ) ";
                //str += "  Select * from DepositDetails ";

                str += " DepositDetails as ( ";
                str += " Select d.customerid,d.custinternalid,d.groupid,d.groupname,d.LoanAccName,d.Cellno,d.LoanBrid,d.LoanGl,To_char(d.LoanAccno) LoanAccno,  ";
                str += " d.LoanAmt,d.LoanCurrentbal,l.dueamt,l.dueint,l.dueinstno,l.mdate as LoanDueDate, ";
                str += " a.brid Depobrid, a.glcode DepoGL, To_char(a.accno) DepoAccno, ABS(a.currentbal) DepoCurrentbal,ABS(a.currentprov) DepoCurrentProv, ";
                str += " b.bankName,b.bankbranchName,b.bankaccno,b.ifsccode,b.bankaccname,b.acctype ";
                str += " ,c.var_customer_fstname firstname,c.var_customer_midname midname,c.var_customer_lstname lastname ";
                str += " from accountview a ";
                str += " left join LoanDetails d on d.LoanBrid = a.brid and d.custinternalid = a.custinternalid ";
                str += " left join BankDetails b on b.custinternalid = d.custinternalid ";
                str += " left join LoanDues l on l.mbrid = d.LoanBrid and l.mglcode = d.LoanGl and l.maccno = d.LoanAccno ";
                str += " inner join aoup_customer_def c on c.num_customer_custinternalid=d.custinternalid ";
                str += " where a.brid = " + Brid + "  and a.gltypeid = 10 and d.custinternalid = a.custinternalid ) ";
                str += "  Select * from DepositDetails ";

                DataTable dt = new DataTable();

                OracleCommand Cmd = new OracleCommand(str, Con);
                OracleDataAdapter AdpData = new OracleDataAdapter();
                AdpData.SelectCommand = Cmd;
                AdpData.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //errorcode = -100;

                    Jdata = BI.Common.Convert_DataTableToJSON(dt, "Customer_Details");

                    responseObj = new JObject(
                               new JProperty("data", Jdata)
                              );

                }
                else
                {
                    //errorcode = 0;

                    ErrCode = 1000;
                    ErrMsg = "Unable to get data. Please try again later";
                    responseObj = handleErrorData_1(ErrCode, ErrMsg);

                }

            }

            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = handleErrorData("transaction_data", ErrCode, ErrMsg, _config);
            }

           
            return responseObj;
        }


        

    }
}
