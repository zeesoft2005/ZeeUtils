using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Net;
using System.IO;

namespace ZeeUtils
{
    public struct Config
    {
        public string ProcessDesc;
        public string ProcessName;
        public string RunMode;
        public string LOCALFolder;
        public string FTPFolder;
        public string LOCALArchivedFolder;
        public string FTPArchivedFolder;
        
        public string SQLConnection;
      

    }

    public static class Statics
    {
        /// <summary>
        /// Gets Actual ODBC Error Message
        /// </summary>
        /// <param name="exMessage"></param>
        /// <returns></returns>
        public static string GetODBCError(string exMessage) 
        {
            try { return exMessage.Substring(exMessage.LastIndexOf("[MYOB ODBC]") + 11); }
            catch { return exMessage; }
        }

        public static decimal ToDecimal(string value)
        {
            decimal output = 0;

            try
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    return output;

                output = decimal.Parse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                output = Math.Round(output, 2);
                //TextLogger2.Log(string.Format("decimal value {0} converted to {1}",value,output.ToString()));
            }
            catch (Exception xx)
            {
                //TextLogger2.Log(LogType.Debug, "Error converting deciaml value:" + value + "\nError:" + xx.Message);
            }

            return output;
        }

        public static double ToDouble(string value)
        {
            double output = 0;

            try
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    return output;

                output = Double.Parse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                output = Math.Round(output, 2);
                //TextLogger2.Log(string.Format("decimal value {0} converted to {1}",value,output.ToString()));
            }
            catch (Exception xx)
            {
                //TextLogger2.Log(LogType.Debug,"Error converting deciaml value:" + value + "\nError:" + xx.Message);
            }

            return output;
        }

        
        public static decimal ToDecimal(object value) 
        {
            decimal output = 0;
            //try { output = Convert.ToDecimal(value.ToString()); }

            //catch (Exception e)
            //{
            //    TextLogger2.Log("Error converting Amount:" + value + "\nError:" + e.Message);

            //}

          
            //if (value == null)
            //    return output;

            //if (string.IsNullOrEmpty(value.ToString()))
            //    return output;
            if (value == null)
                return output;
            try 
            { 
                output = decimal.Parse(value.ToString(), System.Globalization.NumberStyles.Any,CultureInfo.InvariantCulture);
                output = Math.Round(output, 2);
                //TextLogger2.Log(string.Format("decimal value {0} converted to {1}",value,output.ToString()));
            }
            catch (Exception xx){
                //TextLogger2.Log(LogType.Debug, "Error converting deciaml value:" + value + "\nError:" + xx.Message);
            }

            return output;
        }
        public static int ToInt(object value)
        {
            int output = 0;
            
            try { output = int.Parse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture); }
            catch (Exception xx)
            {
                //TextLogger2.Log(LogType.Debug, "Error converting integer value:" + value + "\nError:" + xx.Message);
            }

            return output;
        }

        public static string GetAppSetting(string name)
        {
            string value = string.Empty;
            XmlDocument doc = new XmlDocument();
            string configPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Config.xml";
            try
            {
                
               // TextLogger2.Log("Reading config from :" + configPath);                                
                doc.Load(configPath);
                value = doc.DocumentElement.SelectSingleNode("/Configuration/AppSettings/add[@key='" + name + "']").Attributes["value"].Value;
            }
            catch (Exception x)
            {
                TextLogger2.Log("Error reading app setting for" + name + ":" + x.Message + " from configPath:" + configPath);
                //throw;
            }
            return value;

        }
        public static string GetConnectionString(string name)
        {
            string value = string.Empty;
            XmlDocument doc = new XmlDocument();
            
            string configPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Config.xml";
            try
            {
                
                doc.Load(configPath);
                value = doc.DocumentElement.SelectSingleNode("/Configuration/ConnectionStrings/add[@name='" + name + "']").Attributes["connectionString"].Value;
            }
            catch (Exception x)
            {
                TextLogger2.Log("Error reading connection string:" + x.Message + " from configPath:" + configPath);
                //throw;
            }
            return value;

        }

        public static Config GetProcessConfig(string processName)
        {
            Config value = new Config();

            XmlDocument doc = new XmlDocument();
            string configPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Config.xml";
            try
            {
                
                // TextLogger2.Log("Reading config from :" + configPath);                                
                doc.Load(configPath);
                XmlNode node=doc.DocumentElement.SelectSingleNode("/Configuration/ProcessConfig/add[@ProcessName='" + processName + "']");
                value.ProcessDesc = node.Attributes["ProcessDesc"] == null ? "" : node.Attributes["ProcessDesc"].Value;
                value.ProcessName = node.Attributes["ProcessName"] == null ? "" : node.Attributes["ProcessName"].Value;
                value.RunMode = node.Attributes["RunMode"] == null ? "" : node.Attributes["RunMode"].Value;
                value.LOCALFolder = node.Attributes["LOCALFolder"] == null ? "" : node.Attributes["LOCALFolder"].Value;
                value.FTPFolder = node.Attributes["FTPFolder"] == null ? "" : node.Attributes["FTPFolder"].Value;

                value.LOCALArchivedFolder = node.Attributes["LOCALArchivedFolder"] == null ? "" : node.Attributes["LOCALArchivedFolder"].Value;
                value.FTPArchivedFolder = node.Attributes["FTPArchivedFolder"] == null ? "" : node.Attributes["FTPArchivedFolder"].Value;
                value.SQLConnection = node.Attributes["SQLConnection"] == null ? "" : node.Attributes["SQLConnection"].Value;

            }
            catch (Exception x)
            {
                TextLogger2.Log("Error reading process setting for" + processName + ":" + x.Message + " from configPath:" + configPath);
                //throw;
            }
            return value;

        }

        public static bool WriteToFTPDir(string ftpUserID, string ftpPassword,string fileToWrite, string ftpDir) 
        {
            
            try
            {

                FTPIO ftp = new FTPIO();

                ftp.ftpUserID = ftpUserID;
                ftp.ftpPassword = ftpPassword;
                ftp.ftpServerPath = ftpDir;

                return ftp.Upload(fileToWrite);
            }
            catch (Exception err)
            {
                TextLogger2.Log(string.Format("Error Uploading File {0} to FTP Dir {1}! \nError:{3}", fileToWrite, ftpDir, err.Message));
                throw err;
            }
            
    
        }

        public static bool IsPredefined_RA()
        {
            string keyName = "IsPredefined_RA";

            bool flag = false;

            string t = GetAppSetting("IsPredefined_RA");

            try
            {
                flag = bool.Parse(t);
            }
            catch (Exception x)
            {
                TextLogger2.Log("Error reading app setting for" + keyName + ":" + x.Message);
            }

            return flag;
        }

        public static List<string> GetPredefinedAccountList()
        {
            List<string> lst = new List<string>();

            XmlDocument doc = new XmlDocument();
            string configPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Config.xml";
            try
            {
                doc.Load(configPath);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Configuration/Account-List/add");
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["code"] != null)
                        lst.Add(node.Attributes["code"].Value);
                }
            }
            catch (Exception x)
            {
                TextLogger2.Log("Error reading Account_List :" + x.Message + " from configPath:" + configPath);
            }

            return lst;
        }

        public static string GetPredefinedAccountListAsString()
        {
            List<string> lst = GetPredefinedAccountList();

            StringBuilder sb = new StringBuilder("");

            foreach (string str in lst)
            {
                sb.AppendFormat("'{0}',", str);
            }

            if (lst.Count > 0)
                sb.Length = sb.Length - 1;

            return sb.ToString();
        }

        public static bool CreateDirectoryIfNotExist(string dirPath)
        {//Created this folder by not used yet
            bool flag = true;
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception x)
                {
                    TextLogger2.Log("Unable to create local folder [" + dirPath + "] :" + x.Message);
                    flag = false;
                }
            }
            return flag;
        }
    }
}
