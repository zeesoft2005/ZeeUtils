using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace ZeeUtils
{

    public enum LogType
    {
        Debug,
        Event,
        Exception,
        Information,
        Warning
    }
    public class TextLogger2
    {
        private static readonly bool logDebugInfo;
        const string DefaultPrefix = "FI_Log";
        const int RetentionCount = 7; // Number of logs to keep when housekeeping

        private static bool enabled = true;
        private static LogFlushType flushType = LogFlushType.AutoClose;
        private static string prefix; // initialized in static constructor
        private static string logDir; // ditto

        static string logfile_re_pattern;
        static Regex logfile_re;
        static DateTime nextHousekeeping = DateTime.MinValue;

        static StreamWriter writer = null;

        static TextLogger2()
        {
            try
            {
                Prefix = DefaultPrefix;
                logDir = @"C:\";
                string LogDebugInfo= Statics.GetAppSetting("LogDebugInfo");
                try { logDebugInfo = bool.Parse(LogDebugInfo); }
                catch { }
              
                //logDir = @"D:\DevelopersWork\zee\Current\AWA WBS Mobile\020108\ClientApps\AdminPanel\bin\Debug";
                ///logDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";                
                logDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
                int illegal_part = logDir.IndexOf("file:\\");
                if (illegal_part == 0)
                    logDir = logDir.Substring(6);
                //System.Windows.Forms.MessageBox.Show("Go to find error report file:"+logDir);     
            }
            catch (Exception e)
            {
                logDir = @"\Temp\";                
                logDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
                //System.Windows.Forms.MessageBox.Show("Go to find error report file:" + logDir); 
                //if (!Directory.Exists(logDir))
                //logDir = "";
            }
            logDebugInfo = true;
        }

        /// <summary>
        /// This class is "static". Prevent instantiation.
        /// </summary>
        public static string LogContents
        {
            get
            {
                try
                {
                    StringBuilder builder = new StringBuilder();

                    var lines = File.ReadAllLines(LogPath);

                    foreach (var line in lines)
                    {
                        builder.Append(line + Environment.NewLine);
                    }
                    return builder.ToString();
                }
                catch (Exception x)
                {

                    return "Error log could not be extracted...\nREASON:" + x.Message;
                }
            }
        }
        private TextLogger2()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to perform operations
        /// on log files, including the writing of log messages.
        /// </summary>
        /// <value><b>true</b> if operations should be performed on log files; otherwise <b>false</b>. The default is <b>true</b>. </value>
        public static bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the prefix used to name log files.
        /// </summary>
        /// <value>Any string containing characters valid in a filename. The default is "ox".</value>
        /// <remarks>
        /// Log files names are of the form <i>prefix</i>-<i>yymmdd</i>i>-log.txt, where <i>yymmdd</i> is the current date.
        /// </remarks>
        public static string Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                prefix = value;
                logfile_re_pattern = String.Format(@".*{0}-\d{{6}}-log.txt$", prefix);
                logfile_re = new Regex(logfile_re_pattern, RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// Gets the full pathname of the log file currently in use.
        /// </summary>
        /// <value>The full pathname of the log file currently in use.</value>
        /// <remarks>
        /// This property is not needed for logging, or for resetting or deleting logs. 
        /// It is provided for callers who  may wish to read the contents of the current log file.
        /// </remarks>
        public static string LogPath
        {
            get
            {
                return String.Format(@"{0}{1}-{2:yyMMdd}-log.txt", logDir, Prefix, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets the log file flushing method.
        /// </summary>
        /// <value>The log file flushing method - AutoClose, AutoFlush or Manual.</value>
        /// <remarks>
        /// <para>
        /// This property controls when log buffers are actually written to the file system, and 
        /// whether or not the log file is kept open by </para>
        /// <para>
        /// The default method is AutoClose. AutoClose is easiest to use and most robust flush method. It is also the slowest.
        /// The following characteristics will help you choose the most appropriate method. Timings are taken from tests
        /// on an iPAQ 2200.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Method</term>
        /// <description>Characteristics</description>
        /// </listheader>
        /// <item><term>AutoClose</term><description>Slowest. About 10.5 ms. Most robust. Messages never lost. Log file closed except when a message is being written.</description></item>
        /// <item><term>AutoFlush</term><description>About 4.5 ms. Messages never lost. Log file kept open, limiting access from other applications.</description></item>
        /// <item><term>Manual</term><description>Fastest. About 4.0 ms. Least robust. Messages may be lost unless caller flushes or closes at appropriate times. Log file kept open, limiting access from other applications.</description></item>
        /// </list>
        /// </remarks>
        public static LogFlushType FlushType
        {
            get
            {
                return flushType;
            }
            set
            {
                flushType = value;
            }
        }

        /// <summary>
        /// A convenience property that compensates for the absence of
        /// Enum.GetNames() in the Compact Framework.
        /// </summary>
        /// <exclude />
        public static LogFlushType[] FlushTypes
        {
            get
            {
                return new LogFlushType[] { LogFlushType.AutoClose, LogFlushType.AutoFlush, LogFlushType.Manual };
            }
        }

        /// <summary>
        /// Flushes the log stream if it is open.
        /// </summary>
        /// <remarks>This method is useful only if <see cref="FlushType">FlushType</see> is Manual. 
        /// Otherwise it will have no effect.</remarks>
        public static void Flush()
        {
            if (!Enabled)
                return;

            if (writer != null)
                writer.Flush();
        }

        /// <summary>
        /// Closes the log stream if it is open.
        /// </summary>
        /// <remarks>This method is useful only if <see cref="FlushType">FlushType</see> is AutoFlush or Manual.
        /// Otherwise (i.e. AutoClose) it will have no effect.</remarks>
        public static void Close()
        {
            if (!Enabled)
                return;

            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }

        /// <summary>
        /// Deletes the current log file and restarts the log with a "Log file has been reset" message.
        /// </summary>
        /// <remarks>
        ///  The net result is that the log file is recreated with this single message. All
        /// previous messages in this log file are lost.
        /// </remarks>
        public static void Reset()
        {
            if (!Enabled)
                return;

            Delete();
            Log(LogType.Event,"Log file reset under program control");
        }

        /// <summary>
        /// Deletes the current log file.
        /// </summary>
        /// <remarks> All previous messages in this log file are lost.</remarks>
        public static void Delete()
        {
            if (!Enabled)
                return;

            string logPath = LogPath;

            Close();
            File.Delete(logPath);
        }

        /// <summary>
        /// Log a message. Syntax as for String.Format.
        /// A time stamp, including hundredths of a second, is
        /// prepended.
        /// </summary>
        /// <param name="format">A String containing zero or more format items.</param>
        /// <param name="arg">An object array containg zero or more items to format.</param>
        /// <example>This example shows how to call the Log method.
        /// <code>
        /// class ConsoleApp
        /// {
        ///		[STAThread]
        ///		static void Main(string[] args)
        ///		{
        ///			Log("ConsoleApp starting, {0}.", "hello");
        ///		}
        /// }
        /// </code>
        /// <para>
        /// The log file would be named something like ox-041130-log.txt. The log file would be in the preferred 
        /// temporary files directory. Normally the name of this directory is the value of the environment variable
        /// TEMP. The message logged in this example would look something like
        /// </para>
        /// <code>
        /// 11:36:01.47 ConsoleApp starting, hello.
        /// </code>
        /// </example>
        public static void Log(string format, params object[] arg) 
        {
            Log(LogType.Information,format, arg);
        }
        public static void Log(string message, int rank, LogType logType)
        {
            Log(logType, message);
        }
        public static void Log(LogType logType,string format, params object[] arg)
        {
            if (!Enabled)
                return;

            string logPath = LogPath;

            try
            {
                if (DateTime.Now > nextHousekeeping)
                {
                    Housekeeping();
                    nextHousekeeping = DateTime.Now.AddDays(1); // once every 24 hours
                }

                Open();

                DateTime now = DateTime.Now;
                if (logType == LogType.Debug)
                {
                    if (logDebugInfo)
                    {
                        writer.Write(string.Format("{0} | {1} ", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss"), logType.ToString()));
                        writer.WriteLine("| " + format, arg);
                    }
                }
                else
                {
                    writer.Write(string.Format("{0} | {1} ", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss"), logType.ToString()));
                    writer.WriteLine("| "+format, arg);
                }

                //writer.Write("{0:HH:mm:ss.ff} ", now);
                //writer.WriteLine(format, arg);

                if (FlushType == LogFlushType.AutoFlush)
                    Flush();
                else if (FlushType == LogFlushType.AutoClose)
                    Close();
            }
            catch (Exception x)
            {
                // Not much we can do. Can't log it.
                // Some apps may want an event to be raised here.
                if (writer != null)
                {
                    try
                    {
                        writer.Close();
                    }
                    catch (Exception we)
                    {
                        Console.Beep();
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("ABNORMAL EXCEPTION INSIDE LOGGER:"+we.StackTrace);
                        Console.ResetColor();
                    }
                    writer = null;
                }
            }
        }

        private static void Open()
        {
            if (!Enabled)
                return;

            if (writer == null)
                writer = new StreamWriter(LogPath, true);
        }

        private static void Housekeeping()
        {
            if (!Enabled)
                return;

            // Remove all but the most recent 7 log files. 
            string[] paths = Directory.GetFiles(logDir);
            ArrayList logfiles = new ArrayList();

            foreach (string path in paths)
            {
                if (logfile_re.IsMatch(path))
                    logfiles.Add(new Logfile(path));
            }

            logfiles.Sort();

            for (int i = RetentionCount; i < logfiles.Count; i++)
                ((Logfile)logfiles[i]).Delete();
        }

        /// <summary>
        /// A class that encapsulates log files for sorting (descending)
        /// </summary>
        class Logfile : IComparable
        {
            string path;

            public Logfile(string path)
            {
                this.path = path;
            }

            public void Delete()
            {
                File.Delete(path);
            }

            #region IComparable Members

            // Sort descending, i.e. most recent dates first
            public int CompareTo(object obj)
            {
                return ((Logfile)obj).path.CompareTo(path);
            }

            #endregion
        }

        /// <summary>
        /// An enumeration of the ways that log files can be flushed.
        /// </summary>
        public enum LogFlushType
        {
            /// <summary>
            /// The log file is opened and closed for each message.
            /// </summary>
            AutoClose,
            /// <summary>
            /// The the log file is kept open and is flushed after each message is written.
            /// </summary>
            AutoFlush,
            /// <summary>
            /// The log file is kept open. Flushing is the responsibility of the client.
            /// </summary>
            Manual
        }
    }
}
