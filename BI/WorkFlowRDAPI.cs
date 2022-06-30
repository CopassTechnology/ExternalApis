using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.DA;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ExternalAPIs.BI
{
    public class WorkFlowRDAPI
    {
        public static string getApiKey(IConfiguration _config)
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

        public static JObject CustomerGroupDetails(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject databaseoutput = new JObject();
            try
            {
                dynamic obj = data;
                string HoId = (string)obj["HoId"];
                string CustomerId = (string)obj["CustomerId"];
                JObject dataGroupdetails = new JObject();
                

                // Fetch GroupId 
                dataGroupdetails = WorkFlowRdDA.getGroupDetails(HoId, CustomerId, _config);

                Int64 Brid = Convert.ToInt64(dataGroupdetails["Brid"]);
                Int64 GroupId = Convert.ToInt64(dataGroupdetails["GroupId"]);
                Int16 Errorcode = Convert.ToInt16(dataGroupdetails["errorcode"]);

                if (GroupId != 0)
                { // Fetch Details
                    databaseoutput = WorkFlowRdDA.getCustomerDetails(Brid, GroupId, _config);
                }
                else
                {
                    ErrCode = 1000;
                    ErrMsg = "Unable to get data. Please try again later";
                    databaseoutput = WorkFlowRdDA.handleErrorData_1(ErrCode, ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrCode = 500;
                ErrMsg = "Unable to proceed - webserve";
                databaseoutput = handleErrorData("loginoutput", ErrCode, ErrMsg, _config);
            }
            return databaseoutput;
        }

    }
}

