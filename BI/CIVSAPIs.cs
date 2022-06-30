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
using System.Xml;

namespace ExternalAPIs.BI
{
    public class CIVSAPIs
    {
        public static string getApiKey(IConfiguration _config)
        {
            return _config.GetSection("Appsetting")["MISAPIKey"];
        }
        public static string getApiKey(IConfiguration _config, Int32 br_code)
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
                string response = CryptoManagerCIVSAPP.EncryptString(getApiKey(_config), sampleText);
                response1 = CryptoManagerCIVSAPP.DecryptString(getApiKey(_config), response);
                //string response1 = CryptoManagerICICI.DecryptUsingCertificate(response);
            }

            catch (Exception ex)
            {
                ErrCode = 100;
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
                ErrCode = 100;
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
                ErrCode = 100;
                ErrMsg = "Unable to proceed - webserve";
            }
            return encryptedData;
        }
        public static JObject checkData(IConfiguration _config, JObject data)
        {

            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decryptedData = CryptoManagerCIVSAPP.DecryptString(getApiKey(_config), encryptedData);
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
                string encryptedres = CryptoManagerCIVSAPP.EncryptString(getApiKey(_config), paramdata);
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
                ErrCode = 100;
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
                decryptedData = (string)CryptoManagerCIVSAPP.DecryptString(getApiKey(_config), encryptedData);

            }

            catch (Exception ex)
            {
                ErrCode = 100;
                ErrMsg = "Unable to proceed - webserve";
            }
            return decryptedData;
        }

        public static string EncryptWithMerchantKey(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string EncryptData = "";
            string apikey = "";
            try
            {
                dynamic obj = data;
                Int32 MerchantID = (Int32)obj["MerchantID"];
                dynamic encryptedData = obj["data"];
                apikey = DA.CIVSDA.getapikey(MerchantID, _config);
                string encrypt_data = Convert.ToString(encryptedData);

                EncryptData = (string)CryptoManagerMISAPP.EncryptString(apikey, encrypt_data);

            }

            catch (Exception ex)
            {
                ErrCode = 100;
                ErrMsg = "Unable to proceed - webserve";
            }
            return EncryptData;
        }

        
        public static string  Call_cibilurl(string Str, Int32 MerchantID,string ReferenceNumber,string refid, string ReportType, string FirstName, string MiddleName, string LastName, string GenderType, string BirthDate, string Address, string StateCode, string PinCode, string TelephoneNumber, string AadharNumber, string PANNumber, string VoterID, string RationCardNumber, string PassportNumber, string LoanAmount, IConfiguration _config, out string Status_str, out string error_str, out string xml_response, out string pdfdata ,out string pdfurl_data)
        {
            string res_data = "";
            string check_flag = "";
             pdfurl_data = "";
            string strHeaderTag, strVersion, strUserID, strPassword, strMemberRefNum, strEnquiryAmt, strLoanPurpose, strReportType, strFutureUse,
          strApplicantName1, strApplicantName2, strApplicantName3, strAlternateName, strVoterID, strUID, strPAN, strRationCard, strOtherID1Typedesc,
          strOtherID1, strOtherID2Typedesc, strOtherID2, strOtherID3Typedesc, strOtherID3, strTelephoneNumber1type, strTelephoneNumber1,
          strTelephoneNumber2type, strTelephoneNumber2, strBirthDate, strGenderType, stAddressType1, strAddressLine1, strCity_Town1, strState1,
          strPincode1, stAddressType2, strAddressLine2, strCity_Town2, strState2, strPincode2, strKeyPersonName, strKeyPersonrelation,
          strMemberrelationName1, strMemberrelationType1, strMemberrelationName2, strMemberrelationType2, strMemberrelationName3,
          strMemberrelationType3, strMemberrelationName4, strMemberrelationType4, strNomineeName, strNomineerelation, strAccount1,
          strAccount2, strBranchreferenceno, strKendra_Centrereferenceno, strEndSegmentTag, ResponseStr = "";
            string RequestString, responseString = "";
            string Status, ControlNo, MFI_HIT, CONS_HIT, str_headerInfo, str_searchInfo, str_consumerInfo, str_score, str_plscore, str_mfiEmpInfo, str_alert, str_creditRptSummary, str_recentEnquiry, str_mfiborrower, str_mfiaccinfo, str_mfidates, str_mfiamount, str_mfistatus, str_mfidpd, Error, ErrorMsg = "";
            Int32 ErrorCode=0;

            Status_str = "";
            error_str = "";
            xml_response = "";
            pdfdata = "";

            strHeaderTag = "MFEF";
            strVersion = "1";
            strUserID = "MF83111001_10";
            strPassword = "Tm3@Wc4%23Ay3%23Vp";
            strMemberRefNum = "1235458";// not found
            strEnquiryAmt = LoanAmount;
            strLoanPurpose = "43";
            strReportType = "8";

            strFutureUse = "MF8311";
            strApplicantName1 = FirstName ;
            strApplicantName2 = "";
            strApplicantName3 = "";
            strAlternateName = "";
            strVoterID = VoterID;
            strUID = AadharNumber;
            strPAN = PANNumber;
            strRationCard = RationCardNumber;
            strOtherID1Typedesc = "";
            strOtherID1 = "";
            strOtherID2Typedesc = "";
            strOtherID2 = "";
            strOtherID3Typedesc = "";
            strOtherID3 = "";
            strTelephoneNumber1type = "P03";
            strTelephoneNumber1 = TelephoneNumber;
            strTelephoneNumber2type = "";
            strTelephoneNumber2 = "";

            try
            {
                strBirthDate = Convert.ToDateTime(BirthDate).ToString("ddMMyyyy");
          
            strGenderType = GenderType;
                stAddressType1 = "";
                strAddressLine1 = "";
                strCity_Town1 = "";
                strState1 = "";
                strPincode1 = PinCode;
                stAddressType2 = "C";
                strAddressLine2 =  Address.ToString().Replace(",", "");
                strAddressLine2 = Address.ToString().Replace("&", " AND ");
                if (StateCode != "")
            {
                //strCity_Town2 = dt.Rows[0]["var_city_cityname"].ToString();
                strCity_Town2 = "Mumbai";
            }
            else
            {
                strCity_Town2 = "";
            }
            // strState2 = dt.Rows[0]["var_state_statecode"].ToString();
            if (StateCode.ToString() != "")
            {
                strState2 = StateCode.ToString();
            }
            else { strState2 = ""; }
            if (strPincode1.ToString() != "")
            {
                strPincode2 = strPincode1.ToString();
            }
            else
            { strPincode2 = ""; }

            strKeyPersonName = MiddleName ;
            strKeyPersonrelation = "K01";
            strMemberrelationName1 = "";
            strMemberrelationType1 = "";
            strMemberrelationName2 = "";
            strMemberrelationType2 = "";
            strMemberrelationName3 = "";
            strMemberrelationType3 = "";
            strMemberrelationName4 = "";
            strMemberrelationType4 = "";
            strNomineeName = "";
            strNomineerelation = "";
            strAccount1 = "";
            strAccount2 = "";
            strBranchreferenceno = "";
            strKendra_Centrereferenceno = "";
            strEndSegmentTag = "ES";


                RequestString = strHeaderTag + "," + strVersion + "," + strUserID + "," + strPassword + "," + strMemberRefNum + "," + strEnquiryAmt + "," +
               strLoanPurpose + "," + strReportType + "," + strFutureUse + "," + strApplicantName1 + "," + strApplicantName2 + "," + strApplicantName3 + "," +
               strAlternateName + "," + strVoterID + "," + strUID + "," + strPAN + "," + strRationCard + "," + strOtherID1Typedesc + "," +
               strOtherID1 + "," + strOtherID2Typedesc + "," + strOtherID2 + "," + strOtherID3Typedesc + "," + strOtherID3 + "," + strTelephoneNumber1type + "," +
               strTelephoneNumber1 + "," + strTelephoneNumber2type + "," + strTelephoneNumber2 + "," + strBirthDate + "," + strGenderType + "," +
               stAddressType1 + "," + strAddressLine1 + "," + strCity_Town1 + "," + strState1 + "," + strPincode1 + "," + stAddressType2 + "," +
               strAddressLine2 + "," + strCity_Town2 + "," + strState2 + "," + strPincode2 + "," + strKeyPersonName + "," + strKeyPersonrelation + "," +
               strMemberrelationName1 + "," + strMemberrelationType1 + "," + strMemberrelationName2 + "," + strMemberrelationType2 + "," + strMemberrelationName3 + "," +
               strMemberrelationType3 + "," + strMemberrelationName4 + "," + strMemberrelationType4 + "," + strNomineeName + "," + strNomineerelation + "," +
               strAccount1 + "," + strAccount2 + "," + strBranchreferenceno + "," + strKendra_Centrereferenceno + "," + strEndSegmentTag;


                String URL = "";
                if (MerchantID == 10201 || MerchantID == 30033)
                {
                    URL = "https://mfiuat.cibil.com/MFI/enquiry/comboreport/?recordStr=" + RequestString;
                }
                else
                {
                    URL = "https://mfi.cibil.com/MFI/enquiry/comboreport/?recordStr=" + RequestString;
                }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;//SecurityProtocolType.Tls1.2;

                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //responseString = getstr(); 

                var doc = new XmlDocument();
                doc.LoadXml(responseString);

                String xmlResponse = doc.InnerXml.ToString();
                xml_response = xmlResponse;
                String ControlNo1 = "";
                string CustIntId = "";
                res_data = xmlResponse;
                if (xmlResponse != "")
                { 
                    XmlElement root = doc.DocumentElement;


                    XmlNodeList NodeError = doc.DocumentElement.SelectNodes("error");

                    if (NodeError.Count > 0)
                    {
                        Status = "F";
                        Status_str = Status;
                        foreach (XmlNode userNode in NodeError)
                        {
                            error_str = userNode.InnerText;
                        }
                    }

                    else
                    {
                        Status = "S";
                        Status_str = Status;
                    }



                    if (Status =="S")
                    { 



                    XmlNodeList NodeheaderInfo = doc.DocumentElement.SelectNodes("headerInformation/informationData");
                    if (NodeheaderInfo.Count > 0)
                    {
                        foreach (XmlNode userNode in NodeheaderInfo)
                        {
                            XmlNodeList NodeheaderInfofield = userNode.SelectNodes("field");
                            if (NodeheaderInfofield.Count > 0)
                            {
                                foreach (XmlNode userNode1 in NodeheaderInfofield)
                                {
                                    if (userNode1.Attributes["fieldKey"] != null)
                                    {
                                        string fieldKey = userNode1.Attributes["fieldKey"].Value.ToString();
                                        if (fieldKey != null)
                                        {
                                                     if (fieldKey == "control number")
                                            {
                                                //aderInfo_controlnumber = userNode1.Attributes["value"].Value.ToString();
                                                ControlNo1 = userNode1.Attributes["value"].Value.ToString();
                                            }
                                            if (fieldKey.ToUpper()== "MEMBER REFERENCE NO")
                                            {
                                                CustIntId = userNode1.Attributes["value"].Value.ToString();
                                            }
                                           
                                        }
                                    }
                                }
                            }
                        }
                    }




                    if (ControlNo1 != null || ControlNo1 !="")
                    { 
                    string pdfURL = "";

                    pdfURL = "https://mfi.cibil.com/ConsumerServices/enquiry/combopdfreport";


                    pdfURL = "" + pdfURL + "?ecn_no=" + ControlNo1 + "&member_reference_no=" + CustIntId + "&member_code=" + strFutureUse + "&user_id=" + strUserID + "&password=" + strPassword + "  ";
                            pdfurl_data = pdfURL;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;//SecurityProtocolType.Tls1.2;

                    var pdfrequest = (HttpWebRequest)WebRequest.Create(pdfURL);
                    pdfrequest.Method = "POST";
                    pdfrequest.ContentLength = 0;

                    MemoryStream memoryStream = new MemoryStream();
                    byte[] buffer;
                    using (Stream responseStream = pdfrequest.GetResponse().GetResponseStream())
                    {
                        buffer = new byte[4096];
                        int bytes;
                        while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, bytes);
                        }
                    }
                        buffer = memoryStream.ToArray();

                        var inputStream = new MemoryStream(buffer);

                        var inputAsString = Convert.ToBase64String(inputStream.ToArray());
                    res_data = inputAsString;
                            pdfdata = res_data;
                    }
                    else
                    {
                            pdfdata = "";
                     }

                }
                else
                {
                        res_data = "No Data Available";
                        pdfdata = "";
                    }

                }
                else
                {

                    res_data = "No Data Available";
                    pdfdata = "";
                }
            }
            
            catch
            {
                strBirthDate = "";
            }
            return res_data;
        } 
    
  public static string DecrpttWithMerchantKey(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            string DecryptData = "";
            string apikey = "";
            try
            {
                dynamic obj = data;
                Int32 MerchantID = (Int32)obj["MerchantID"];
                string encryptedData = (string)obj["payload"];
                apikey = DA.CIVSDA.getapikey(MerchantID, _config);
                DecryptData = (string)CryptoManagerMISAPP.DecryptString(apikey, encryptedData);

            }

            catch (Exception ex)
            {
                ErrCode = 100;
                ErrMsg = "Unable to proceed - webserve";
            }
            return DecryptData;
        }

        //login process
        public static JObject Payload_Req(IConfiguration _config, JObject CIVSEntryinput)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            JObject Outputdata = new JObject();
            JObject json = new JObject();
            JObject responseObj = new JObject();
            string encryptoutput = "";
            try
            {
                dynamic obj = CIVSEntryinput;
                string encryptedData = (string)obj["payload"];
                Int32 MerchantID = (Int32)obj["MerchantID"];
                string apikey = DA.CIVSDA.getapikey(MerchantID,_config);

                if (apikey != "Invalid" || apikey != "Db connection failed")
                {
                    string decryptedData = (string)CryptoManagerCIVSAPP.DecryptString(apikey, encryptedData);
                    json = JObject.Parse(BI.Common.validJason(decryptedData));
                    JToken headerToken = json;
                    string _strdata = (string)headerToken.SelectToken("CivsDetails").ToString();
                    // string MerchantID = headerToken.SelectToken("CivsDetails.MerchantID").ToString();
                  
                    string ReferenceNumber = headerToken.SelectToken("CivsDetails.ReferenceNumber").ToString();
                    string ReportType = headerToken.SelectToken("CivsDetails.ReportType").ToString();
                    string FirstName = headerToken.SelectToken("CivsDetails.FirstName").ToString();
                    string MiddleName = headerToken.SelectToken("CivsDetails.MiddleName").ToString();
                    string LastName = headerToken.SelectToken("CivsDetails.LastName").ToString();
                    string gendertype = headerToken.SelectToken("CivsDetails.GenderType").ToString();
                    string BirthDate = headerToken.SelectToken("CivsDetails.BirthDate").ToString();
                    string Address =  headerToken.SelectToken("CivsDetails.Address").ToString().Replace(",","");
                    Address = Address.Replace("&", "And");
                    string StateCode = headerToken.SelectToken("CivsDetails.StateCode").ToString();
                    string PinCode = headerToken.SelectToken("CivsDetails.PinCode").ToString();
                    string TelephoneNumber = headerToken.SelectToken("CivsDetails.TelephoneNumber").ToString();
                    string AadharNumber = headerToken.SelectToken("CivsDetails.AadharNumber").ToString();
                    string PANNumber = headerToken.SelectToken("CivsDetails.PANNumber").ToString();
                    string VoterID = headerToken.SelectToken("CivsDetails.VoterID").ToString();
                    string RationCardNumber = headerToken.SelectToken("CivsDetails.RationCardNumber").ToString();
                    string PassportNumber = headerToken.SelectToken("CivsDetails.PassportNumber").ToString();
                    string LoanAmount = headerToken.SelectToken("CivsDetails.LoanAmount").ToString();
                    

                    responseObj = DA.CIVSDA.CIVSEntry(_strdata, MerchantID,  ReferenceNumber, ReportType, FirstName, MiddleName, LastName, gendertype, BirthDate, Address, StateCode, PinCode, TelephoneNumber, AadharNumber, PANNumber, VoterID, RationCardNumber, PassportNumber, LoanAmount, _config);
                    
                    string strdata = responseObj.ToString();
                     encryptoutput = (string)CryptoManagerCIVSAPP.EncryptString(apikey, strdata);
                    Outputdata = new JObject(new JProperty("PayloadOutput", encryptoutput));
                }
                else
                {
                    ErrCode = 99;
                    ErrMsg = "Invalid Api Key";
                    responseObj = new JObject(new JObject(new JProperty("PayloadOutput", new JObject(
                            new JProperty("xmlresponse", ""),
                               new JProperty("status", ""),
                               new JProperty("PDF_DATA_BASE64_STRING", ""),
                               new JProperty("xmlerror", ""),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
            ))));

                    string strdata = responseObj.ToString();
                    encryptoutput = (string)CryptoManagerCIVSAPP.EncryptString(apikey, strdata);
                    Outputdata = new JObject(new JProperty("CIVSReqOutput", encryptoutput));
                }

            }

            catch (Exception ex)
            {
                ErrCode = 200;
                ErrMsg = "Unable to proceed - webserve";
                responseObj = new JObject(new JObject(new JProperty("ApiOutput", new JObject(
                          new JProperty("xmlresponse", ""),
                               new JProperty("status", ""),
                               new JProperty("PDF_DATA_BASE64_STRING", ""),
                               new JProperty("xmlerror", ex.Message.ToString()),
                               new JProperty("ErrCode", ErrCode),
                               new JProperty("ErrMsg", ErrMsg)
           ))));

                string strdata = responseObj.ToString();
                encryptoutput = (string)CryptoManagerCIVSAPP.EncryptString(getApiKey(_config), strdata);
                Outputdata = new JObject(new JProperty("CIVSEntryoutput", encryptoutput));
            }
            return Outputdata;

        }


        public static string CIVSKEYDECRYPT(IConfiguration _config, JObject data)
        {
            int ErrCode = 0;
            string ErrMsg = "";
            dynamic obj = data;
            string encryptedData = (string)obj["data"];
            string decrypt_data="";

            JToken headerToken = encryptedData;
         
            //string keydata = headerToken.SelectToken("data").ToString();
            try
            {

                 
                string decryptedData = (string)CryptoManagerCIVSAPP.DecryptUsingCopassCertificateRSA(encryptedData);

                decrypt_data = decryptedData;
               
            }
            catch (Exception ex)
            {

            }
            return decrypt_data;
        }

        public static string CIVSKEYENCRYPT(IConfiguration _config, string data)
        {
            int ErrCode = 0;
            string ErrMsg = "";

           
            string encryptkey = data;
            try
            {

                string planedata = encryptkey;
                string encryptData = (string)CryptoManagerCIVSAPP.EncryptUsingCopassCertificateRSA(planedata);

                encryptkey = encryptData;
            }
            catch (Exception ex)
            {

            }
            return encryptkey;

        }
    }
}