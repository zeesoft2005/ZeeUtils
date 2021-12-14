using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Net;


namespace ZeeUtils
{
    public class FTPIO
    {
        public string ftpServerPath;
        public string ftpUserID;
        public string ftpPassword;

        /// <summary>
        /// Method to upload the specified file to the specified FTP Server
        /// </summary>
        /// <param name="filename">file full name to be uploaded</param>
        public bool Upload(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);

            string uri = "ftp://" + ftpServerPath + fileInf.Name;
            //uri =  ftpServerIP  + fileInf.Name;
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();
           
            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();
                
                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream
                
                //FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                //TextLogger2.Log(string.Format("Upload File Complete, status {0}", response.StatusDescription));                                
                
                strm.Close();
                fs.Close();
                //response.Close();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        //public string MoveFile2(string fileFullPath, string dirToMoveTo)
        //{
        //    try
        //    {


        //        Uri fileToMove = new Uri("ftp://" + fileFullPath);
        //        string fileName = fileToMove.Segments[fileToMove.Segments.Length - 1];

        //        #region Copy
                
        //        FtpWebRequest reqFTP;
        //        // Create FtpWebRequest object from the Uri provided
        //        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(dirToMoveTo + fileName));
        //        reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        //        reqFTP.KeepAlive = false;
        //        reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
        //        reqFTP.UseBinary = true;
        //        StringReader fileData=new StringReader(ReadFile(fileFullPath));
        //        // Notify the server about the size of the uploaded file
        //        //reqFTP.ContentLength = fileData..Length;
        //            // Stream to which the file to be upload is written
        //            Stream strm = reqFTP.GetRequestStream();
                
                    
        //            //FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
        //            //TextLogger2.Log(string.Format("Upload File Complete, status {0}", response.StatusDescription));                                

        //            strm.Close();                    
        //            //response.Close();
        //            return true;
                

        //    }
        //    catch (Exception err)
        //    {
        //        TextLogger2.Log(err.Message, 1, LogType.Exception);
        //        throw err;
        //    }

        //}
        public string MoveFile(string file, string relaiveDirToMoveTo) 
        {
            try
            {

        
            Uri fileToMove = new Uri(file);
            string fileName = fileToMove.Segments[fileToMove.Segments.Length - 1];
            

            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(file);
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = System.Net.WebRequestMethods.Ftp.Rename;
            reqFTP.RenameTo = relaiveDirToMoveTo  +  fileName;
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            //if (response.StatusDescription.Contains("250")) //250 Rename successful
            //{
            //    reqFTP.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
            //    response = (FtpWebResponse)reqFTP.GetResponse();
            //    return response.StatusDescription;
            //}
            return response.StatusDescription;

            }
            catch (Exception err)
            {
                TextLogger2.Log(err.Message,1,LogType.Exception);
                throw err;
            }

        }
        public bool DeleteFile(string fileName)
        {
            try
            {
                string uri = "ftp://" + ftpServerPath + "/" + fileName;
                uri = fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteFTPFile(string filePath)
        {
            try
            {
                string uri = "ftp://" + filePath;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] GetFilesDetailList()
        {
            string[] downloadFiles;
            try
            {
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerPath + "/"));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.AppendLine(line);                    
                    line = reader.ReadLine();
                }
                
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
                //MessageBox.Show(result.ToString().Split('\n'));
            }
            catch (Exception ex)
            {
                //throw ex;
                downloadFiles = null;
                return downloadFiles;
            }
        }

        public string[] GetFileList()
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerPath));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.AppendLine(line);                    
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                //MessageBox.Show(response.StatusDescription);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }
        public string ReadFile(string file)
        {
            string fileContents = string.Empty;
            //string fileToRead=string.Format("ftp://{0}{1}",ftpServerPath,fileName);
            //CREATE AN FTP REQUEST WITH THE DOMAIN AND CREDENTIALS
            System.Net.FtpWebRequest reqFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(file);
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);


            //GET THE FTP RESPONSE
            using (System.Net.WebResponse resFTP = reqFTP.GetResponse())
            {
                //GET THE STREAM TO READ THE RESPONSE FROM
                using (System.IO.Stream resStream = resFTP.GetResponseStream())
                {
                    //CREATE A TXT READER (COULD BE BINARY OR ANY OTHER TYPE YOU NEED)
                    using (System.IO.TextReader tmpReader = new System.IO.StreamReader(resStream))
                    {
                        //STORE THE FILE CONTENTS INTO A STRING
                        fileContents = tmpReader.ReadToEnd();
                        //DO SOMETHING WITH SAID FILE CONTENTS
                    }
                }
            }
            return fileContents;
        }
        public string ReadFileAtServerPath() 
        {
            string fileContents = string.Empty; 
            //string fileToRead=string.Format("ftp://{0}{1}",ftpServerPath,fileName);
            //CREATE AN FTP REQUEST WITH THE DOMAIN AND CREDENTIALS
            System.Net.FtpWebRequest reqFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpServerPath);
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

 
            //GET THE FTP RESPONSE
            using (System.Net.WebResponse resFTP = reqFTP.GetResponse())
            {
                //GET THE STREAM TO READ THE RESPONSE FROM
                using (System.IO.Stream resStream = resFTP.GetResponseStream())
                {
                    //CREATE A TXT READER (COULD BE BINARY OR ANY OTHER TYPE YOU NEED)
                    using (System.IO.TextReader tmpReader = new System.IO.StreamReader(resStream))
                    {
                        //STORE THE FILE CONTENTS INTO A STRING
                        fileContents = tmpReader.ReadToEnd(); 
                        //DO SOMETHING WITH SAID FILE CONTENTS
                    }
                }
            }
            return fileContents;
        }
        public void Download(string filePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerPath + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerPath + "/" + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fileSize;
        }

        public void Rename(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerPath + "/" + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                // dirName = name of the directory to create.
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerPath + "/" + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
    }
}
