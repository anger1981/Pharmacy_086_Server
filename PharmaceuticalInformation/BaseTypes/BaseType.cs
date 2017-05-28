using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PharmaceuticalInformation.BaseTypes
{
    public class BaseType
    {

        #region ' Fields '

        protected string PathToLogFile;
        //
        private bool _ShowingMessages;
        //
        private int _CountOfRowsInLogFile;
        private bool _EnableScrapingLog;

        #endregion

        #region ' Designer '

        //
        public BaseType()
            : this("")
        {
            //
        }

        //
        public BaseType(string PathToLogFile)
        {
            //
            this.PathToLogFile = PathToLogFile;
            //
            _ShowingMessages = true;
            //
            _CountOfRowsInLogFile = 1000;
            //
            _EnableScrapingLog = false;
        }

        #endregion

        #region ' Messages '

        // Showing Messages
        public bool ShowingMessages
        {
            get { return _ShowingMessages; }
            set { _ShowingMessages = value; }
        }

        // Returning Message About Error
        protected void ReturningMessageAboutError(
            string TextOfCircumstances, Exception Exc, bool FatalError)
        {
            //
            // Returning Message About Error
            //
            ReturningMessageAboutError(TextOfCircumstances, Exc, true, FatalError);
        }

        // Returning Message About Error
        protected void ReturningMessageAboutError(
            string TextOfCircumstances, Exception Exc, bool RecordingInLog, bool FatalError)
        {
            //
            // !!!
            //
            string TextOfMessage = String.Format("{0}: {1}", TextOfCircumstances, Exc.Message);
            //
            // Recording In Log File
            //
            if(RecordingInLog)
                this.RecordingInLogFile("ERROR MESSAGE " + TextOfMessage);
            //
            // Showing Message
            //
            if (_ShowingMessages)
            {
                MessageBox.Show(
                    TextOfMessage, (FatalError) ? "FATAL ERROR" : "ERROR", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //
        }

        // Returning Message Of Information
        protected void ReturningMessageOfInformation(string TextOfMessage)
        {
            //
            // Returning Message Of Information
            //
            ReturningMessageOfInformation(TextOfMessage, true);
        }

        // Returning Message Of Information
        protected void ReturningMessageOfInformation(string TextOfMessage, bool RecordingInLog)
        {
            //
            // Showing Message
            //
            if (_ShowingMessages)
            {
                MessageBox.Show(
                    TextOfMessage, "Information",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            //
            // Recording In Log File
            //
            if(RecordingInLog)
                this.RecordingInLogFile(TextOfMessage);
        }

        #endregion

        #region ' Working With Log File '

        // Recording In Log File (Not Correct)
        //protected
        protected void RecordingInLogFile(string Record)
        {
            //
            if ((PathToLogFile != "") && (PathToLogFile != null))
            {
                //
                int ResultOfWriting = 0;
                bool Repetition = true;
                //
                while (Repetition)
                {
                    //
                    Repetition = false;
                    //
                    try
                    {
                        //
                        // Creating Streams
                        //
                        FileStream FS = new FileStream(PathToLogFile, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter SW = new StreamWriter(FS, Encoding.Default);
                        FS.Seek(FS.Length, SeekOrigin.Begin);
                        //
                        // Writing In Stream
                        //
                        try { SW.WriteLine(String.Format("{0}   {1}", DateTime.Now, Record)); }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при записи в поток Log", E, false, false); }
                        //
                        // Closing Streams
                        //
                        try
                        {
                            SW.Close();
                            FS.Close();
                        }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при закрытии потоков Log", E, false, false); }
                    }
                    catch /*(Exception E)*/
                    {
                        //
                        //this.ReturningMessageAboutError("Ошибка при создании потоков Log", E, false, false);
                        ResultOfWriting += 1;
                    }
                    //
                    // Repeating Of Writing
                    //
                    if (ResultOfWriting == 1)
                    {
                        //
                        ResultOfWriting = 2;
                        //
                        Repetition = true;
                        //
                        System.Threading.Thread.Sleep(108);
                    }
                }
                //
                // Scraping Of Log File
                //
                ScrapingOfLogFile();
            }
        }

        // Scraping Of Log File
        //protected 
        protected void ScrapingOfLogFile()
        {
            //
            if(_EnableScrapingLog)
                try
                {
                    //
                    // Reading Text Of Log File
                    //
                    string TextOfLogFile = "";
                    //
                    try
                    {
                        //
                        FileStream FS = new FileStream(this.PathToLogFile, FileMode.Open, FileAccess.Read);
                        StreamReader SR = new StreamReader(FS, Encoding.Default);
                        //
                        try { TextOfLogFile = SR.ReadToEnd(); }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при чтении текста Log файла", E, false, false); }
                        //
                        try
                        {
                            SR.Close();
                            FS.Close();
                        }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при закрытии потоков Log файла", E, false, false); }
                        //
                    }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при создании потоков Log", E, false, false); }
                    //
                    // Scraping Text Of Log File
                    //
                    string ScrapTextOfLogFile = "";
                    //
                    if ((TextOfLogFile != "") && (TextOfLogFile != null))
                    {
                        //
                        string[] RowsOfText = TextOfLogFile.Split('\n');
                        //
                        if (RowsOfText.Length > _CountOfRowsInLogFile)
                        {
                            //
                            // Scraping
                            //
                            string[] ScrapOfRows = new string[_CountOfRowsInLogFile];
                            //
                            int IndexOfInserting = 0;
                            for (int i = RowsOfText.GetUpperBound(0) - ScrapOfRows.GetUpperBound(0); i <= RowsOfText.GetUpperBound(0); i++)
                                ScrapOfRows[IndexOfInserting++] = RowsOfText[i];
                            //
                            ScrapTextOfLogFile = String.Join("\n", ScrapOfRows);
                        }
                    }
                    //
                    // Writing Text In Log File
                    //
                    if ((ScrapTextOfLogFile != "") && (ScrapTextOfLogFile != null))
                    {
                        try
                        {
                            //
                            FileStream FS = new FileStream(this.PathToLogFile, FileMode.Create, FileAccess.ReadWrite);
                            StreamWriter SW = new StreamWriter(FS, Encoding.Default);
                            //
                            try { SW.Write(ScrapTextOfLogFile); }
                            catch (Exception E)
                            { this.ReturningMessageAboutError("Ошибка при записи текста в Log файл", E, false, false); }
                            //
                            try
                            {
                                SW.Close();
                                FS.Close();
                            }
                            catch (Exception E)
                            { this.ReturningMessageAboutError("Ошибка при закрытии потоков Log файла", E, false, false); }
                            //
                        }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при создании потоков записи Log файла", E, false, false); }
                    }
                    //
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при обрезании Log файла", E, false ,false); }
        }

        // Count Of Rows In Log File
        public int CountOfRowsInLogFile
        {
            get { return _CountOfRowsInLogFile; }
            set { _CountOfRowsInLogFile = value; }
        }

        // Enable Scraping Log
        public bool EnableScrapingLog
        {
            get { return _EnableScrapingLog; }
            set { _EnableScrapingLog = value; }
        }

        #endregion

    }
}