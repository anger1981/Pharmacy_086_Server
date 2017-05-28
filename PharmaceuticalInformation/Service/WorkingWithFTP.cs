using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Service
{
    public class WorkingWithFTP : BaseType
    {


        #region ' Fields '

        //
        //private FtpWebRequest RequestOfDownload02;
        private FtpWebResponse ResponseOfGettingListOfFile;
        private FtpWebResponse ResponseOfDownloadingFile;

        #endregion


        #region ' Designer '

        public WorkingWithFTP(string PathToLogFile)
            : base(PathToLogFile)
        {
            //
        }

        public WorkingWithFTP()
        {
            //
        }

        #endregion


        #region ' GettingListOfDirectory '

        /*
        // Getting List Of Directory (ARH)
        public ArrayList GettingListOfDirectory(string PathToFTP, bool UsePassive)
        {
            //
            ArrayList ListOfFilesOfFTP = new ArrayList();
            //
            try
            {
                //
                // !!!
                //
                FtpWebRequest RequestToFTP = (FtpWebRequest)WebRequest.Create(PathToFTP);
                RequestToFTP.Timeout = 1000 * 60 * 5;
                RequestToFTP.UsePassive = UsePassive;
                RequestToFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse ResponseOfFTP = (FtpWebResponse)RequestToFTP.GetResponse();
                StreamReader ReadingOfList = new StreamReader(ResponseOfFTP.GetResponseStream());
                string TextOfListOfFiles = ReadingOfList.ReadToEnd();
                string[] Sep = new string[1] { String.Format("{0}{1}", (char)13, (char)10) };
                ListOfFilesOfFTP.AddRange(TextOfListOfFiles.Split(Sep, StringSplitOptions.RemoveEmptyEntries));
                //
                // !!!
                //
                ResponseOfFTP.Close();
                ReadingOfList.Close();
                ReadingOfList.Dispose();
                //
                ResponseOfFTP = null;
                ReadingOfList = null;
                RequestToFTP = null;
                //
                for (int i = 0; i < 8; i++)
                    GC.Collect();
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при получении списка файлов", E, false); }
            // Return
            return ListOfFilesOfFTP;
        }
        */

        /*
        // Getting List Of Directory (ARH)
        public ArrayList GettingListOfDirectory02(string PathToFTP, bool UsePassive)
        {
            //
            ArrayList ListOfFilesOfFTP = new ArrayList();
            //
            try
            {
                //
                // !!!
                //
                FtpWebRequest RequestToFTP = (FtpWebRequest)WebRequest.Create(PathToFTP);
                RequestToFTP.Timeout = 1000 * 60 * 5;
                RequestToFTP.ReadWriteTimeout = 1000 * 60 * 5;
                RequestToFTP.UsePassive = UsePassive;
                RequestToFTP.KeepAlive = true;
                RequestToFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                ResponseOfGettingListOfFile = (FtpWebResponse)RequestToFTP.GetResponse();
                StreamReader ReadingOfList = new StreamReader(ResponseOfGettingListOfFile.GetResponseStream());
                string TextOfListOfFiles = ReadingOfList.ReadToEnd();
                string[] Sep = new string[1] { String.Format("{0}{1}", (char)13, (char)10) };
                ListOfFilesOfFTP.AddRange(TextOfListOfFiles.Split(Sep, StringSplitOptions.RemoveEmptyEntries));
                //
                // !!!
                //
                //ResponseOfFTP02.Close();
                //ReadingOfList.Close();
                //ReadingOfList.Dispose();
                //
                //ResponseOfFTP = null;
                //ReadingOfList = null;
                //RequestToFTP = null;
                //
                /*
                for (int i = 0; i < 8; i++)
                    GC.Collect();
                8/
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при получении списка файлов", E, false); }
            // Return
            return ListOfFilesOfFTP;
        }
        */

        // Getting List Of Directory For Server
        public ArrayList GettingListOfDirectory03(string PathToFTP, bool UsePassive)
        {
            //
            ArrayList ListOfFilesOfFTP = new ArrayList();
            //
            try
            {
                //
                // !!!
                //
                FtpWebRequest RequestToFTP = (FtpWebRequest)WebRequest.Create(PathToFTP);
                RequestToFTP.Timeout = 1000 * 60 * 5;
                RequestToFTP.ReadWriteTimeout = 1000 * 60 * 5;
                RequestToFTP.UsePassive = UsePassive;
                RequestToFTP.KeepAlive = true;
                RequestToFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                //
                ResponseOfGettingListOfFile = (FtpWebResponse)RequestToFTP.GetResponse();
                //
                StreamReader ReadingOfList = new StreamReader(ResponseOfGettingListOfFile.GetResponseStream());
                string TextOfListOfFiles = ReadingOfList.ReadToEnd();
                ReadingOfList.Close();
                ReadingOfList.Dispose();
                //
                // Clearing Text Of List Of Files
                //
                if(PresenceExtraneousSymbols(TextOfListOfFiles))
                {
                    //
                    this.RecordingInLogFile("ERROR В списке файлов присутствуют посторонние символы");
                    //
                    TextOfListOfFiles = ClearingTextOfListOfFiles(TextOfListOfFiles);
                }
                //
                // Division Of Text Of List Of Files
                //
                string[] Separator = new string[1] { String.Format("{0}{1}", (char)13, (char)10) };
                ListOfFilesOfFTP.AddRange(TextOfListOfFiles.Split(Separator, StringSplitOptions.RemoveEmptyEntries));
                //
                // Clearing Working With FTP
                //
                RequestToFTP = null;
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при получении списка файлов", E, false); }
            //
            // Return
            //
            return ListOfFilesOfFTP;
        }

        // Reset Of Response Of Getting List Of Files
        public void ResetOfResponseOfGettingListOfFiles()
        {
            //
            // Reset
            //
            ResponseOfGettingListOfFile.Close();
            ResponseOfGettingListOfFile = null;
        }

        // Getting List Of Directory For Drugstore
        public ArrayList GettingListOfDirectory04(string PathToFTP, bool UsePassive)
        {
            //
            ArrayList ListOfFilesOfFTP = new ArrayList();
            //
            try
            {
                //
                // !!!
                //
                FtpWebRequest RequestToFTP = (FtpWebRequest)WebRequest.Create(PathToFTP);
                RequestToFTP.Timeout = 1000 * 60 * 5;
                RequestToFTP.ReadWriteTimeout = 1000 * 60 * 5;
                RequestToFTP.UsePassive = UsePassive;
                RequestToFTP.KeepAlive = false;
                RequestToFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                //
                FtpWebResponse ResponseOfGettingListOfFile = (FtpWebResponse)RequestToFTP.GetResponse();
                //
                StreamReader ReadingOfList = new StreamReader(ResponseOfGettingListOfFile.GetResponseStream());
                string TextOfListOfFiles = ReadingOfList.ReadToEnd();
                ReadingOfList.Close();
                ReadingOfList.Dispose();
                //
                // Clearing Text Of List Of Files
                //
                if (PresenceExtraneousSymbols(TextOfListOfFiles))
                {
                    //
                    this.RecordingInLogFile("ERROR В списке файлов присутствуют посторонние символы");
                    //
                    TextOfListOfFiles = ClearingTextOfListOfFiles(TextOfListOfFiles);
                }
                //
                // Division Of Text Of List Of Files
                //
                string[] Separator = new string[1] { String.Format("{0}{1}", (char)13, (char)10) };
                ListOfFilesOfFTP.AddRange(TextOfListOfFiles.Split(Separator, StringSplitOptions.RemoveEmptyEntries));
                //
                // Clearing Working With FTP
                //
                ResponseOfGettingListOfFile.Close();
                ResponseOfGettingListOfFile = null;
                RequestToFTP = null;
                //
                // Clearing
                //
                System.Threading.Thread.Sleep(108);
                //
                GC.Collect();
            }
            catch (Exception E)
            {
                //
                //ReturningMessageAboutError("Ошибка при получении списка файлов", E, false);
                //
                this.RecordingInLogFile(String.Format("ERROR Ошибка при получении списка файлов: {0}", E.Message));
            }
            //
            // Return
            //
            return ListOfFilesOfFTP;
        }

        #endregion


        // Uploading File
        public bool UploadingFile(string PathToLoadingFile, string PathToFTPFile, bool UsePassive)
        {
            //
            // !!!
            //
            bool ResultOfOperation = true;
            Stream StreamOfRequest = null;
            FileStream StreamOfFile = null;
            FtpWebResponse ResponseOfUploading = null;
            //
            try
            {
                //
                // Creating Request Of Uploading File
                //
                FtpWebRequest RequestOfUploading = (FtpWebRequest)WebRequest.Create(PathToFTPFile);
                RequestOfUploading.Timeout = 1000 * 60 * 5;
                RequestOfUploading.UsePassive = UsePassive;
                //RequestOfUploading.Proxy = null;
                RequestOfUploading.Method = WebRequestMethods.Ftp.UploadFile;
                //
                // Creating Streams
                //
                StreamOfRequest = RequestOfUploading.GetRequestStream();
                StreamOfFile = System.IO.File.Open(PathToLoadingFile, System.IO.FileMode.Open);
                //
                // Transfer Of File
                //
                byte[] buffer = new byte[1024];
                int bytesRead;
                while (true)
                {
                    bytesRead = StreamOfFile.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    StreamOfRequest.Write(buffer, 0, bytesRead);
                }
                //
                // Uploading
                //
                StreamOfRequest.Close();
                ResponseOfUploading = (System.Net.FtpWebResponse)RequestOfUploading.GetResponse();
            }
            catch (Exception E)
            {
                //
                ResultOfOperation = false;
                //
                /*
                ReturningMessageAboutError(
                  String.Format("{0} {1}", "Ошибка при загрузке файла", PathToLoadingFile), E, false);
                */
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при загрузке файла ({0}): {1}", 
                    Path.GetFileName(PathToLoadingFile), E.Message));
            }
            finally
            {
                if (ResponseOfUploading != null)
                    ResponseOfUploading.Close();
                if (StreamOfFile != null)
                    StreamOfFile.Close();
                if (StreamOfRequest != null)
                    StreamOfRequest.Close();
            }
            //
            // Return
            //
            return ResultOfOperation;
        }


        #region ' DownloadingFile02 '

        // Downloading File For Drugstor
        public bool DownloadingFile(string PathToFTPFile, string PathToDownloadingFile, bool UsePassive)
        {
            //
            // !!!
            //
            bool ResultOfOperation = true;
            Stream StreamOfResponse = null;
            FileStream StreamOfFile = null;
            //
            try
            {
                //
                // Creating Request To Downloading File
                //
                FtpWebRequest RequestOfDownload = (FtpWebRequest)WebRequest.Create(PathToFTPFile);
                RequestOfDownload.Timeout = 1000 * 60 * 5;
                //RequestOfDownload.KeepAlive = true;
                RequestOfDownload.UsePassive = UsePassive;
                //RequestOfDownload.Method = WebRequestMethods.Ftp.DownloadFile;
                //
                FtpWebResponse ResponseOfDownload = (FtpWebResponse)RequestOfDownload.GetResponse();
                StreamOfResponse = ResponseOfDownload.GetResponseStream();
                //
                // Transfer Of File
                //
                if (PathToDownloadingFile.Length != 0)
                {
                    //
                    StreamOfFile = File.Create(PathToDownloadingFile);
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    //
                    while (true)
                    {
                        bytesRead = StreamOfResponse.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        StreamOfFile.Write(buffer, 0, bytesRead);
                    }
                    //
                    buffer = null;
                }
                //
                // Clearing Working With FTP
                //
                if (ResponseOfDownload != null)
                { ResponseOfDownload.Close(); }
                //
                ResponseOfDownload = null;
                RequestOfDownload = null;
            }
            catch (Exception E)
            {
                //
                ResultOfOperation = false;
                //
                /*ReturningMessageAboutError(
                    String.Format("{0} {1}", "Ошибка при скачивании файла", PathToDownloadingFile), E, false);*/
                //
                RecordingInLogFile(
                    String.Format(
                    "ERROR Ошибка при скачивании файла ({0}) с FTP Сервера: {1}", 
                    Path.GetFileName(PathToDownloadingFile), E.Message));
            }
            finally
            {
                try
                {
                    //
                    if (StreamOfResponse != null)
                    { StreamOfResponse.Close(); StreamOfResponse.Dispose(); }
                    //
                    if (StreamOfFile != null)
                    { StreamOfFile.Close(); StreamOfFile.Dispose(); }
                }
                catch (Exception E)
                {
                    RecordingInLogFile(String.Format("ERROR Error At Clearing Of Streams: {0}", E.Message));
                }
            }
            //
            // Return
            //
            return ResultOfOperation;
        }

        // Downloading File For Servere
        public bool DownloadingFile02(string PathToFTPFile, string PathToDownloadingFile, bool UsePassive)
        {
            //
            // !!!
            //
            bool ResultOfOperation = true;
            Stream StreamOfResponse = null;
            FileStream StreamOfFile = null;
            //
            try
            {
                //
                // Creating Request To Downloading File
                //
                FtpWebRequest RequestOfDownload = (FtpWebRequest)WebRequest.Create(PathToFTPFile);
                RequestOfDownload.Timeout = 1000 * 60 * 5;
                RequestOfDownload.KeepAlive = true;
                RequestOfDownload.UsePassive = UsePassive;
                RequestOfDownload.Method = WebRequestMethods.Ftp.DownloadFile;
                //
                ResponseOfDownloadingFile = (FtpWebResponse)RequestOfDownload.GetResponse();
                StreamOfResponse = ResponseOfDownloadingFile.GetResponseStream();
                //
                // Transfer Of File
                //
                if (PathToDownloadingFile.Length > 0)
                {
                    //
                    StreamOfFile = File.Create(PathToDownloadingFile);
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    //
                    while (true)
                    {
                        bytesRead = StreamOfResponse.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        StreamOfFile.Write(buffer, 0, bytesRead);
                    }
                    //
                    buffer = null;
                }
                //
                RequestOfDownload = null;
            }
            catch (Exception E)
            {
                //
                ResultOfOperation = false;
                //
                /*ReturningMessageAboutError(
                    String.Format("{0} {1}", "Ошибка при скачивании файла", PathToDownloadingFile), E, false);*/
                //
                RecordingInLogFile(
                    String.Format(
                    "ERROR Ошибка при скачивании файла ({0}) с FTP Сервера: {1}", 
                    Path.GetFileName(PathToDownloadingFile), E.Message));
            }
            finally
            {
                try
                {
                    //
                    if (StreamOfResponse != null)
                    { StreamOfResponse.Close(); StreamOfResponse.Dispose(); }
                    //
                    if (StreamOfFile != null)
                    { StreamOfFile.Close(); StreamOfFile.Dispose(); }
                }
                catch (Exception E)
                {
                    RecordingInLogFile(String.Format("ERROR Error At Clearing Of Streams: {0}", E.Message));
                }
            }
            //
            // Return
            //
            return ResultOfOperation;
        }

        // Reset Of Response Of Downloading File
        public void ResetOfResponseOfDownloadingFile()
        {
            //
            // !!!
            //
            ResponseOfDownloadingFile.Close();
            ResponseOfDownloadingFile = null;
        }

        #endregion


        #region ' DeletingFile02 '

        // Deleting File
        public bool DeletingFile(string PathToFTPFile)
        {
            bool ResultOfOperation = true;
            FtpWebRequest RequestOfDeletings = null;
            FtpWebResponse ResponseOfDeletings = null;
            try
            {
                RequestOfDeletings = (FtpWebRequest)WebRequest.Create(PathToFTPFile);
                RequestOfDeletings.UsePassive = true;
                RequestOfDeletings.Timeout = 1000 * 60 * 5;
                RequestOfDeletings.Method = WebRequestMethods.Ftp.DeleteFile;
                this.RecordingInLogFile("R 1");
                ResponseOfDeletings = (FtpWebResponse)RequestOfDeletings.GetResponse();
                this.RecordingInLogFile("R 2");
                //
                RequestOfDeletings = null;
                //
                for (int i = 0; i < 8; i++)
                    GC.Collect();
            }
            catch (Exception E)
            {
                ResultOfOperation = false;
                ReturningMessageAboutError(
                    String.Format("{0} {1}", "Ошибка при удалении файла", PathToFTPFile), E, false);
            }
            finally
            {
                //
                if (ResponseOfDeletings != null)
                    ResponseOfDeletings.Close();
                //
                ResponseOfDeletings = null;
                RequestOfDeletings = null;
            }
            //
            for (int i = 0; i < 8; i++)
                GC.Collect();
            // Return
            return ResultOfOperation;
        }

        // Deleting File
        public bool DeletingFile02(string PathToFTPFile)
        {
            //
            bool ResultOfOperation = true;
            //
            //if (RequestOfDownload02 != null)
            //FtpWebRequest RequestOfDeletings = null;
            //FtpWebResponse ResponseOfDeletings = null;
            //
            try
            {
                FtpWebRequest RequestOfDeletings = (FtpWebRequest)WebRequest.Create(PathToFTPFile);
                //RequestOfDeletings.Timeout = 1000 * 60 * 5;
                //RequestOfDeletings.KeepAlive = true;
                //RequestOfDeletings.UsePassive = true;
                RequestOfDeletings.Method = WebRequestMethods.Ftp.DeleteFile;
                //this.RecordingInLogFile("R 1");
                FtpWebResponse ResponseOfDeletings = (FtpWebResponse)RequestOfDeletings.GetResponse();
                //this.RecordingInLogFile("R 2");
                //
                // Clearing Working With FTP
                //
                ResponseOfDeletings.Close();
                ResponseOfDeletings = null;
                RequestOfDeletings = null;
                //
                /*
                for (int i = 0; i < 8; i++)
                    GC.Collect();
                */
            }
            catch (Exception E)
            {
                //
                ResultOfOperation = false;
                //
                ReturningMessageAboutError(
                    String.Format("{0} {1}", "Ошибка при удалении файла", PathToFTPFile), E, false);
            }
            finally
            {
                //
                /*if (ResponseOfDeletings != null)
                    ResponseOfDeletings.Close();
                //
                ResponseOfDeletings = null;*/
                //RequestOfDeletings = null;
                //
                /*
                if (ResponseOfDownloadingFile != null)
                    ResponseOfDownloadingFile.Close();
                //
                ResponseOfDownloadingFile = null;
                */
            }
            //
            /*
            for (int i = 0; i < 8; i++)
                GC.Collect();
            */
            //
            // Return
            //
            return ResultOfOperation;
        }

        #endregion


        #region ' Clearing Text Of List Of Files '

        //Presence Extraneous symbols
        public bool PresenceExtraneousSymbols(string TextOfListOfFiles)
        {
            //
            bool Result = false;
            //
            // Checking
            //
            if (TextOfListOfFiles != null)
            {
                if (TextOfListOfFiles.IndexOf("<") > -1)
                    Result = true;
                if (TextOfListOfFiles.IndexOf("\"") > -1)
                    Result = true;
            }
            //
            // Return
            //
            return Result;
        }

        //Clearing Text Of List Of Files
        public string ClearingTextOfListOfFiles(string TextOfListOfFiles)
        {
            //
            string ClearedTextOfListOfFiles = "";
            System.Collections.ArrayList ListOfNamesOfFiles = new System.Collections.ArrayList();
            //
            // Replacing <BR> To 13
            //
            TextOfListOfFiles = TextOfListOfFiles.Replace("<BR>", String.Format("{0}", (char)13));
            //
            // Division Of Text Of List Of Files
            //
            //string[] Separator = new string[1] { String.Format("{0}{1}", (char)13, (char)10) };
            string[] Separator = new string[2] { String.Format("{0}", (char)10), String.Format("{0}", (char)13) };
            string[] StringsOfListOfFiles =
                TextOfListOfFiles.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            //
            // Getting Name Of File
            //
            for (int i = 0; i < StringsOfListOfFiles.Length; i++)
            {
                //
                if (StringsOfListOfFiles[i].Length > 0)
                {
                    //
                    // Checking Existence Of Tag A
                    //
                    int IndexOfBeginningA = StringsOfListOfFiles[i].IndexOf("<A ");
                    int IndexOfEndA = StringsOfListOfFiles[i].IndexOf(">");
                    //
                    if ((IndexOfBeginningA > -1) && (IndexOfEndA > -1))
                    {
                        //
                        // Getting Text Of Tag A
                        //
                        string TextOfA = "";
                        //
                        if (IndexOfEndA >= IndexOfBeginningA)
                            TextOfA =
                                StringsOfListOfFiles[i].Substring(IndexOfBeginningA, IndexOfEndA - IndexOfBeginningA);
                        //
                        // Checking Existence Of Name
                        //
                        int IndexOfBeginningName = TextOfA.IndexOf("\"");
                        int IndexOfEndName = TextOfA.LastIndexOf("\"");
                        //
                        if ((IndexOfBeginningName > -1) && (IndexOfEndName > -1))
                        {
                            //
                            // Getting Name Of File
                            //
                            string NameOfFile = "";
                            //
                            IndexOfBeginningName += 1;
                            //
                            if (IndexOfEndName >= IndexOfBeginningName)
                                NameOfFile =
                                    TextOfA.Substring(IndexOfBeginningName, IndexOfEndName - IndexOfBeginningName);
                            //
                            // Checking Name
                            //
                            if (NameOfFile.IndexOf("/") > -1)
                            {
                                //
                                string[] ElementsOfNameOfFile =
                                    NameOfFile.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                //
                                if (ElementsOfNameOfFile.Length > 0)
                                    NameOfFile = ElementsOfNameOfFile[ElementsOfNameOfFile.GetUpperBound(0)];
                                else
                                    NameOfFile = "";
                            }
                            //
                            if (NameOfFile.IndexOf("..") > -1)
                                NameOfFile = "";
                            //
                            // Addition Of Name
                            //
                            if (NameOfFile != "")
                                ListOfNamesOfFiles.Add(NameOfFile);
                        }
                    }
                }
            }
            //
            // Generation Of Text Of List Of Names Of Files
            //
            foreach (string CurrentName in ListOfNamesOfFiles)
                ClearedTextOfListOfFiles += CurrentName + String.Format("{0}{1}", (char)13, (char)10);
            //
            // Return
            //
            return ClearedTextOfListOfFiles;
        }

        #endregion

    }
}