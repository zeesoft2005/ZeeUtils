using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;

namespace ZeeUtils
{
    public class Logger
    {
        private static readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SISS\\AuditLog.txt");

        public static void Write(string message)
        {
            Write(message, 1);
        }

        public static void Write(string message, int rank)
        {
            Write(message, rank, LogType.Information);
        }

        public static void Write(string message, int rank, LogType logType)
        {
            if (rank <= 0) return;

            try
            {
                // first check the directory existence...
                if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                // check file existence...
                if (!File.Exists(logFilePath))
                    File.Create(logFilePath);

                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        if (string.IsNullOrEmpty(message))
                            writer.WriteLine();
                        else
                            writer.WriteLine(string.Format("{0} | {1} | {2}", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss"), logType.ToString(), message));
                    }
                
            }
            catch{}
        }
    }

   

}
