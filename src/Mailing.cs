using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using ZeeUtils;
using System.IO;
//using TACBS.Utils.IO;


namespace ZeeUtils
{
    // Mailer sends an e-mail.  
    // It will authenticate using Windows authentication if the server
    // (i.e. Exchange) requests it.

    public class Mailer
    {
        private string smtphost;
        private string userAcc, userPw;
        int port;

        MailMessage message = new MailMessage();

        public Mailer(string smtphost, string userAcc, string userPw)
        {
            this.smtphost = smtphost;
            this.userAcc = userAcc;
            this.userPw = userPw;


        }
        public Mailer(string smtphost,int port, string userAcc, string userPw)
        {
            this.smtphost = smtphost;
            this.userAcc = userAcc;
            this.userPw = userPw;
            this.port = port;


        }
        public Mailer()
        {
            this.smtphost = "smtp.gmail.com";

        }
        public string SMTP
        {
            get
            {
                return this.smtphost;
            }
            set
            {
                this.smtphost = value;
            }
        }
        public string USER
        {
            get
            {
                return this.userAcc;
            }
            set
            {
                this.userAcc = value;
            }
        }

        public string PW
        {
            get
            {
                return this.userPw;
            }
            set
            {
                this.userPw = value;
            }
        }
        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port= value;
            }
        }
        public void ClearAttachments()
        {
            this.message.Attachments.Clear();
        }
        public void AddAttachment(Attachment file)
        {
            this.message.Attachments.Add(file);
        }

        public bool SendMail(string from, string to, string cc, string bcc, string subject, string body, string SenderDisplayName)
        {


            #region Try SMTP options to send email

            string[] smtps = smtphost.Split(',');
            foreach (string smtp in smtps)
            {
                try
                {
                    this.smtphost = smtp;

                    if (TrySendEmail(from, to, cc, bcc, subject, body, SenderDisplayName))
                        return true;
                }
                catch (SmtpException ex2)
                {
                    TextLogger2.Log(LogType.Exception,"Failed SMTP {0}:", this.smtphost);
                }
            }
            #endregion

            return false;
        }
        /// <summary>
        /// Validate the Path. If path is relative append the path to the project directory by default.
        /// </summary>
        /// <param name="path">Path to validate</param>
        /// <param name="RelativePath">Relative path</param>
        /// <param name="Extension">If want to check for File Path</param>
        /// <returns></returns>
        private static bool ValidatePath(ref string path, string RelativePath = "", string Extension = "")
        {
            string error = string.Empty;
            // Check if it contains any Invalid Characters.
            if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                try
                {
                    // If path is relative take %IGXLROOT% as the base directory
                    if (!Path.IsPathRooted(path))
                    {
                        if (string.IsNullOrEmpty(RelativePath))
                        {
                            // Exceptions handled by Path.GetFullPath
                            // ArgumentException path is a zero-length string, contains only white space, or contains one or more of the invalid characters defined in GetInvalidPathChars. -or- The system could not retrieve the absolute path.
                            // 
                            // SecurityException The caller does not have the required permissions.
                            // 
                            // ArgumentNullException path is null.
                            // 
                            // NotSupportedException path contains a colon (":") that is not part of a volume identifier (for example, "c:\"). 
                            // PathTooLongException The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.

                            // RelativePath is not passed so we would take the project path 
                            path = Path.GetFullPath(RelativePath);

                        }
                        else
                        {
                            // Make sure the path is relative to the RelativePath and not our project directory
                            path = Path.Combine(RelativePath, path);
                        }
                    }

                    // Exceptions from FileInfo Constructor:
                    //   System.ArgumentNullException:
                    //     fileName is null.
                    //
                    //   System.Security.SecurityException:
                    //     The caller does not have the required permission.
                    //
                    //   System.ArgumentException:
                    //     The file name is empty, contains only white spaces, or contains invalid characters.
                    //
                    //   System.IO.PathTooLongException:
                    //     The specified path, file name, or both exceed the system-defined maximum
                    //     length. For example, on Windows-based platforms, paths must be less than
                    //     248 characters, and file names must be less than 260 characters.
                    //
                    //   System.NotSupportedException:
                    //     fileName contains a colon (:) in the middle of the string.
                    FileInfo fileInfo = new FileInfo(path);

                    // Exceptions using FileInfo.Length:
                    //   System.IO.IOException:
                    //     System.IO.FileSystemInfo.Refresh() cannot update the state of the file or
                    //     directory.
                    //
                    //   System.IO.FileNotFoundException:
                    //     The file does not exist.-or- The Length property is called for a directory.
                    bool throwEx = fileInfo.Length == -1;

                    // Exceptions using FileInfo.IsReadOnly:
                    //   System.UnauthorizedAccessException:
                    //     Access to fileName is denied.
                    //     The file described by the current System.IO.FileInfo object is read-only.-or-
                    //     This operation is not supported on the current platform.-or- The caller does
                    //     not have the required permission.
                    throwEx = fileInfo.IsReadOnly;

                    if (!string.IsNullOrEmpty(Extension))
                    {
                        // Validate the Extension of the file.
                        if (Path.GetExtension(path).Equals(Extension, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Trim the Library Path
                            path = path.Trim();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;

                    }
                }
                catch (ArgumentNullException)
                {
                    //   System.ArgumentNullException:
                    error = "fileName is null";
                }
                catch (System.Security.SecurityException)
                {
                    //   System.Security.SecurityException:
                    error = "The caller does not have the required permission.";
                }
                catch (ArgumentException)
                {
                    //   System.ArgumentException:
                    error = "The file name is empty, contains only white spaces, or contains invalid characters.";
                }
                catch (UnauthorizedAccessException)
                {
                    //   System.UnauthorizedAccessException:
                    error = "Access to fileName is denied.";
                }
                catch (PathTooLongException)
                {
                    //   System.IO.PathTooLongException:
                    error = " The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.";
                }
                catch (NotSupportedException)
                {
                    //   System.NotSupportedException:
                    error = " fileName contains a colon (:) in the middle of the string.";
                }
                catch (FileNotFoundException)
                {
                    // System.FileNotFoundException
                    error = " The exception that is thrown when an attempt to access a file that does not exist on disk fails.";
                }
                catch (IOException)
                {
                    //   System.IO.IOException:
                    error = " An I/O error occurred while opening the file.";
                }
                catch (Exception)
                {
                    error = " Unknown Exception. Might be due to wrong case or nulll checks.";
                }
            }
            else
            {
                error = " Path contains invalid characters";
            }
            Console.WriteLine(error);
            return false;
        }

        private bool TrySendEmail(string from, string to, string cc, string bcc, string subject, string body, string SenderDisplayName)
        {
            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            #region TO Addresses

            if (string.IsNullOrEmpty(to))
            {
                TextLogger2.Log("Plz specify valid TO emails (separated by comma in case of more than one)!");
                return false;
            }
            string[] tos = to.Split(',');
            if (tos.Length < 1)
            {
                TextLogger2.Log("Plz specify valid emails (separated by comma in case of more than one)!");
                return false;
            }
            #endregion

            #region CC Addresses

            if (!string.IsNullOrEmpty(cc))
            {
                try
                {
                    string[] ccs = cc.Split(',');
                    foreach (string _cc in ccs)
                        message.CC.Add(_cc.Trim());
                }
                catch
                {
                    TextLogger2.Log("Problem adding CC addresses...");
                }
            }
            #endregion

            #region BCC Addresses

            if (!string.IsNullOrEmpty(bcc))
            {
                try
                {
                    string[] bccs = bcc.Split(',');
                    foreach (string _bcc in bccs)
                        message.Bcc.Add(_bcc.Trim());
                }
                catch
                {
                    TextLogger2.Log("Problem adding BCC addresses...");
                }
            }
            //hidden BCC for debugging purpose;comment out this line later
            //message.Bcc.Add("zeeshan.ahmed@objectsynergy.com");
            
            #endregion

            // Set mailServerName to be the name of the mail server
            // you wish to use to deliver this message
            string mailServerName = this.smtphost;
            // SmtpClient is used to send the e-mail
            SmtpClient mailClient = new SmtpClient(mailServerName);

            try
            {
                //TextLogger2.Log(LogType.Debug, "Sending Email with SMTP {0}...", this.smtphost);
                mailClient.Timeout = 200000;
                message.IsBodyHtml = true;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                //message.BodyEncoding = Encoding.Unicode;
                //message.Priority = MailPriority.High;
                
                message.BodyEncoding = Encoding.Default;
                message.SubjectEncoding = Encoding.Default;
                message.HeadersEncoding = Encoding.Default;
        //         BodyEncoding = System.Text.Encoding.UTF8,
        //SubjectEncoding = System.Text.Encoding.Default,
        //IsBodyHtml = true

                foreach (string t in tos)
                    message.To.Add(t.Trim());
                if (!string.IsNullOrEmpty(from))
                    message.From = new MailAddress(from, SenderDisplayName);
                if (!string.IsNullOrEmpty(subject))
                    message.Subject = subject;

                #region this block validates if body param is specified as a file path? if no, just treat it as raw body content otherwise read file content and set it as body

                var fileForBody = body;
                var isBodyFilePath = ValidatePath(ref fileForBody);
                if (!string.IsNullOrEmpty(body) && !isBodyFilePath)
                {
                    try
                    {
                        var fileContent = File.ReadAllText(fileForBody);
                        message.Body = fileContent;
                    }
                    catch (Exception errr)
                    {
                        Console.WriteLine(errr.Message);
                        throw;
                    }
                    
                }
                else if (!string.IsNullOrEmpty(body))
                {
                    message.Body = body;
                }

                #endregion

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
                message.AlternateViews.Add(htmlView);
                //mailClient.EnableSsl = true;
                mailClient.Port = Port;
                userAcc = USER;
                userPw = PW;               
                mailClient.Credentials = new NetworkCredential(userAcc, userPw);

                // Send delivers the message to the mail server
                mailClient.Send(message);
                //TextLogger2.Log(LogType.Debug, "Mail Successfully Sent.");
                //Console.WriteLine("Mail Successfully Sent.");

            }
            catch (ArgumentNullException ax)
            {
                //log error 
                TextLogger2.Log(LogType.Exception, "Error sending mail:" + ax.Message);
                return false;
            }
            catch (FormatException ex)
            {
                //log error 
                TextLogger2.Log(LogType.Exception, "Error sending mail:" + ex.Message);
                return false;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        TextLogger2.Log(LogType.Exception, "Delivery failed:" + ex.Message);
                        //System.Threading.Thread.Sleep(5000);
                        //mailClient.Send(message);
                    }
                    else
                    {
                        TextLogger2.Log(LogType.Exception, "Failed to deliver message to {0}", ex.FailedRecipient[i]);
                    }
                }
                return false;
            }
            catch (SmtpException ex2)
            {
                //log error 
                TextLogger2.Log(LogType.Exception, "SmtpException:" + ex2.Message);
                TextLogger2.Log(LogType.Exception, "Error Detail:" + ex2.InnerException);
                throw new SmtpException(ex2.StatusCode, ex2.Message);

            }
            catch(Exception unexpected)
            {
                TextLogger2.Log(LogType.Exception, "Unexpected Error sending email:" + unexpected.Message);
                return false;
            }
            finally
            {
                
            }
            return true;
        }
    }


}
