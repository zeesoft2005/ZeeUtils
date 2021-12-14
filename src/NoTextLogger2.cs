using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace ZeeUtils
{
    public enum NoLogType
    {
        Debug,
        Event,
        Exception,
        Information,
        Warning
    }
    public class NoTextLogger2
    {
      

        

        public static void Log(string format, params object[] arg) 
        {
 
        }
        public static void Log(string message, int rank, NoLogType logType)
        {
 
        }
        public static void Log(NoLogType logType, string format, params object[] arg)
        {
        }
        public static void Close() { }
    }
}
