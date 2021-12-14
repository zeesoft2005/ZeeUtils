using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration;


namespace ZeeUtils
{
    public class FileUtility
    {
        public string FilePath { get; set; }
        public List<FileData> FileData { get; set; }
        public string MoveFilePath { get; set; }
        //public List<string> FileList { get; set; }

        public FileUtility()
        {
            //FilePath = @"c:\RMS.txt";
            //MoveFilePath = @"D:\Testfile.txt";
            FileData = new List<FileData>();
            //FileList = new List<string>();
        }
        public bool ReadFile()
        {
            string line;     // Read the file and display it line by line.         
            bool isFirstLine = false;
            try
            {
                using (StreamReader file = new StreamReader(FilePath))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (!isFirstLine)
                        {
                            char[] delimiters = new char[] { '\t' };
                            //string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                            string[] parts = line.Split(delimiters);
                            FileData data = new FileData();
                            data.JournalDate = string.IsNullOrWhiteSpace(parts[0]) ? string.Empty : parts[0].Trim();
                            data.Memo = string.IsNullOrWhiteSpace(parts[1]) ? string.Empty : parts[1].Trim();
                            data.AccountNumber = string.IsNullOrWhiteSpace(parts[2]) ? string.Empty : parts[2].Trim();
                            data.DebitAmount = string.IsNullOrWhiteSpace(parts[3]) ? "0" : parts[3].Trim();
                            data.GSTAmount = string.IsNullOrWhiteSpace(parts[4]) ? "0" : parts[4].Trim(); 
                            data.CreditAmount = string.IsNullOrWhiteSpace(parts[5]) ? "0" : parts[5].Trim(); 
                            data.TaxRate = string.IsNullOrWhiteSpace(parts[6]) ? string.Empty : parts[6].Trim();
                            
                            FileData.Add(data);
                        }
                        isFirstLine = false;
                    }
                    file.Close();
                }
            }
            catch(Exception ex)
            {
                ZeeUtils.Logger.Write("ReadFile : " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool MovedFile(string movedFilePath)
        {
            try
            {
                // Create unique file name
                MoveFilePath = movedFilePath;
                if (File.Exists(FilePath))
                {
                    File.Move(FilePath, MoveFilePath);
                }
            }
            catch(Exception ex)
            {
                ZeeUtils.Logger.Write("MovedFile : " + ex.ToString());
                return false;
            }
            return true;
        }

        public static List<string> ReadAllFiles(string sourceDir)
        {
            string[] fileList = Directory.GetFiles(sourceDir, "*.txt");
            List<string> FileList = new List<string>();
            try
            {
                foreach (string file in fileList)
                {
                    FileList.Add(file);
                }
            }
            catch(Exception ex)
            {
                ZeeUtils.Logger.Write("ReadAllFiles : " + ex.ToString());
                return null;
            }
            return FileList;
        }
    }

    public class FileData
    {
        public string JournalDate { get; set; }
        public string Memo { get; set; }
        public string AccountNumber { get; set; }
        public string DebitAmount { get; set; }
        public string GSTAmount { get; set; }
        public string CreditAmount { get; set; }
        public string TaxRate { get; set; }
        public FileData()
        {
            JournalDate = string.Empty;
            Memo = string.Empty;
            AccountNumber = string.Empty;
            DebitAmount = "0";
            GSTAmount = "0";
            CreditAmount = "0";
            TaxRate = string.Empty;
        }
    }
}
