using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExternalAPIs.BI
{
    public  class Common
    {
        public static string validJason(string jsonStr)
        {
           // jsonStr = jsonStr.TrimEnd().TrimStart();
            int FirstOcc = jsonStr.IndexOf("{");
            int lastOcc = jsonStr.LastIndexOf("}") + 1;
            string json = jsonStr.Substring(FirstOcc, lastOcc - FirstOcc);
            return json;
        }
        public static string validPrefixJason(string jsonStr)
        {
            // jsonStr = jsonStr.TrimEnd().TrimStart();
            int FirstOcc = jsonStr.IndexOf("{\"");
            int lastOcc = jsonStr.LastIndexOf("}") + 1;
            string json = jsonStr.Substring(FirstOcc, lastOcc - FirstOcc);
            return json;
        }
        public static JObject Convert_DataTableToJSON(DataTable table, string Jobjectname)
        {
            //string JSONString = string.Empty;
            //JSONString = JsonConvert.SerializeObject(table);
            //JObject Jdata = JObject.Parse(JSONString);

            JObject responseData = new JObject(

                   new JProperty(Jobjectname, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(table, Formatting.Indented)))
                );
            return responseData;
        }
        public static object dataSetToJSON_string(DataSet ds)
        {

            ArrayList root = new ArrayList();
            List<Dictionary<string, object>> table;
            Dictionary<string, object> data;

            foreach (DataTable dt in ds.Tables)
            {
                table = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    data = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        data.Add(col.ColumnName, dr[col]);
                    }
                    table.Add(data);
                }
                root.Add(table);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(root);
        }
        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }
        public static string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
        public static string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }
        public static void write_log_error(string sourceObject, string description)
        {
            StreamWriter sw = null;
            string DATA_LOG = "C://COPASS/ICICI/ERROR_LOG/";
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
        public static void write_log_Success(string sourceObject, string description)
        {
            StreamWriter sw = null;
            string DATA_LOG = "C://COPASS/ICICI/SUCCESS_LOG/";
            try
            {
                DirectoryInfo dir = new DirectoryInfo(DATA_LOG);

                if (dir.Exists)
                {
                    string filePath = DATA_LOG + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " Success.log";

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
        public static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}
