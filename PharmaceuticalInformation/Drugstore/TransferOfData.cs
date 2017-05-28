using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class TransferOfData : BaseType 
    {

        #region ' Fields '

        //
        // Fields Of Transfer
        //
        private ConvertingOfPriceList ConvertingOfPL;
        private ManagementOfPriceList ManagementOfPL;
        private ExchangeFTPAndLocalDataBase ExchangeFTPAndLDB;
        //
        private string _StringOfConnection;
        private string _PathToLogFile;
        private int _IDOfDrugstore;
        private DataSet _Settings;
        private DataTable _DatesOfTransfer;

        #endregion

        #region ' Designer '

        public TransferOfData(
            ConvertingOfPriceList ConvertingOfPL, ManagementOfPriceList ManagementOfPL,
            ExchangeFTPAndLocalDataBase ExchangeFTPAndLDB)
            : this(ConvertingOfPL, ManagementOfPL, ExchangeFTPAndLDB, "")
        {
            //
        }

        public TransferOfData(
            ConvertingOfPriceList ConvertingOfPL, ManagementOfPriceList ManagementOfPL, 
            ExchangeFTPAndLocalDataBase ExchangeFTPAndLDB, string PathToLogFile)
            : base(PathToLogFile)
        {

            //
            // Initializing Fields Of Transfer
            //
            this.ConvertingOfPL = ConvertingOfPL;
            this.ManagementOfPL = ManagementOfPL;
            this.ExchangeFTPAndLDB = ExchangeFTPAndLDB;
            //
        }

        #endregion

        #region ' Creating '

        // Creating Structure For Sending
        private DataSet CreatingStructureForSending()
        {
            // Creating
            DataSet StructureForSending = new DataSet("SendingData");
            StructureForSending.RemotingFormat = SerializationFormat.Binary;
            //
            // Creating Information
            //
            DataTable Information = new DataTable("Information");
            Information.Columns.Add("Key", typeof(string));
            Information.Columns.Add("Value", typeof(object));
            Information.PrimaryKey = new DataColumn[1] { Information.Columns["Key"] };
            StructureForSending.Tables.Add(Information);
            //
            StructureForSending.Tables["Information"].Rows.Add("IDOfDrugstore", _IDOfDrugstore);
            StructureForSending.Tables["Information"].Rows.Add("DateOfSending", DateTime.Now);
            StructureForSending.Tables["Information"].AcceptChanges();
            //
            // Return
            //
            return StructureForSending;
        }

        #endregion

        #region ' Properties For Work '

        // String Of Connection
        public string StringOfConnection
        {
            get { return _StringOfConnection; }
            set { _StringOfConnection = value; }
        }

        // Settings
        public DataSet Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }

        // Dates Of Transfer
        public DataTable DatesOfTransfer
        {
            get { return _DatesOfTransfer; }
            set { _DatesOfTransfer = value; }
        }

        // Path To Send Log File
        public string PathToSendLogFile
        {
            get { return _PathToLogFile; }
            set { _PathToLogFile = value; }
        }

        // ID Of Drugstore
        public int IDOfDrugstore
        {
            get { return _IDOfDrugstore; }
            set { _IDOfDrugstore = value; }
        }

        #endregion

        #region ' Getting Fields Of Transfer '

        // Get Converting Of PriceList
        public ConvertingOfPriceList GetConvertingOfPriceList
        {
            get { return ConvertingOfPL; }
        }

        // Get Management Of PriceList
        public ManagementOfPriceList GetManagementOfPriceList
        {
            get { return ManagementOfPL; }
        }

        // Get Exchange FTP And Local Data Base
        public ExchangeFTPAndLocalDataBase GetExchangeFTPAndLocalDataBase
        {
            get { return ExchangeFTPAndLDB; }
        }

        #endregion

        #region ' Converting Of Data '

        // Converting And Loading PriceList
        public bool ConvertingAndLoadingPriceList(bool ShowingMessage)
        {
            //
            bool SuccessfulConverting = true;
            //
            // Recording In Reports
            //
            RecordingInLogFile(
                String.Format(
                "Загрузка файлов Прайс-Листов из директории: {0}",
                ""));
            /*
            RecordingInLogFile(
                String.Format(
                "Загрузка файлов Прайс-Листов из директории: {0}", 
                ConvertingOfPL.PathToFolderOfPriceLists));
            */
            //
            // Checking Of Existing Of Directory Of PriceLists
            //
            //bool ExistedOfDirectory = Directory.Exists(ConvertingOfPL.PathToFolderOfPriceLists);
            bool ExistedOfDirectory = Directory.Exists("");
            //
            // Recording In Reports And Cancel Convertings
            //
            /*
            if (!ExistedOfDirectory)
            {
                //
                ReturningMessageAboutError(
                    String.Format(
                    "Директория Прайс-Листов '{0}' не существует", 
                    ConvertingOfPL.PathToFolderOfPriceLists), 
                    new Exception("Exists False"), false);
                //
                SuccessfulConverting = false;
            }
            */
            /*if (!ExistedOfDirectory)
            {
                //
                ReturningMessageAboutError(
                    String.Format(
                    "Директория Прайс-Листов '{0}' не существует",
                    "",
                    new Exception("Exists False"), false));
                //
                SuccessfulConverting = false;
            }*/
            //
            // Getting List Of Files Of PriceLists
            //
            bool ExistedOfFilesOfPriceLists = false;
            ArrayList ListOfFilesOfPriceLists = new ArrayList();
            //
            if (ExistedOfDirectory)
            {
                //
                // Getting List Of Files
                //
                //string[] FilesOfDirectory = Directory.GetFiles(ConvertingOfPL.PathToFolderOfPriceLists);
                string[] FilesOfDirectory = Directory.GetFiles("");
                //
                // Addition Of Files Of PriceList In List
                //
                foreach (string CurrentFile in FilesOfDirectory)
                    if (ConvertingOfPL.ConformityOfFileToMasks(CurrentFile))
                    {
                        //
                        // Getting Date Of File Of PriceList
                        //
                        DateTime DateOfFileOfPriceList = new DateTime(1947, 7, 2);
                        //
                        try
                        {
                            //
                            DateTime CreationTime = File.GetCreationTime(CurrentFile);
                            DateTime WriteTime = File.GetLastWriteTime(CurrentFile);
                            //
                            if (CreationTime > WriteTime)
                                DateOfFileOfPriceList = CreationTime;
                            else if (WriteTime >= CreationTime)
                                DateOfFileOfPriceList = WriteTime;
                        }
                        catch (Exception E)
                        {
                            RecordingInLogFile(
                                String.Format("ERROR Ошибка при чтении даты файла Прайс-листа '{0}': {1}", 
                                Path.GetFileName(CurrentFile), E.Message));
                        }
                        //
                        // Addition
                        //
                        ArrayList DataOfFileOfPriceList = new ArrayList();
                        //
                        DataOfFileOfPriceList.Add(CurrentFile);
                        DataOfFileOfPriceList.Add(DateOfFileOfPriceList);
                        //
                        ListOfFilesOfPriceLists.Add(DataOfFileOfPriceList);
                    }
            }
            //
            // Checking Of Count Of Files Of PriceLists
            //
            if (ListOfFilesOfPriceLists.Count == 0)
            {
                //
                /*
                ReturningMessageOfInformation(
                    String.Format(
                    "В директории '{0}' НЕТ файлов Прайс-Листов",
                    ConvertingOfPL.PathToFolderOfPriceLists));
                */
                ReturningMessageOfInformation(
                    String.Format(
                    "В директории '{0}' НЕТ файлов Прайс-Листов",
                    ""));
                //
                ExistedOfFilesOfPriceLists = false;
                SuccessfulConverting = false;
            }
            else
            { ExistedOfFilesOfPriceLists = true; }
            //
            // Converting PriceLists And Addition For Importing To DataBase
            //
            int CountOfConvertingRows = 0;
            //
            if (ExistedOfFilesOfPriceLists)
            {
                //
                // Reset Data For Importing In PriceList
                //
                ManagementOfPL.ResetDataForImportingInPriceList();
                //
                // Scaning Of Files Of PriceLists
                //
                foreach (ArrayList CurrentFileOfPriceList in ListOfFilesOfPriceLists)
                {
                    //
                    try
                    {
                        //
                        DataTable PriceListFromFile = new DataTable();
                        //
                        // Converting File Of PriceList
                        //
                        PriceListFromFile = ConvertingOfPL.Converting(CurrentFileOfPriceList[0].ToString());
                        //
                        CountOfConvertingRows += PriceListFromFile.Rows.Count;
                        //
                        CurrentFileOfPriceList.Add(PriceListFromFile.Rows.Count);
                        CurrentFileOfPriceList.Add(false);
                        //
                        RecordingInLogFile(
                            String.Format("Дата файла Прайс-листа: {0}", CurrentFileOfPriceList[1].ToString()));
                        //
                        // Checking Count Of Converted Rows
                        //
                        if (PriceListFromFile.Rows.Count > 0)
                        {
                            //
                            // Importing Data In PriceList
                            //
                            try
                            {
                                //
                                // Addition Of PriceList For Importing
                                //
                                ManagementOfPL.AdditionOfPriceListForImporting(PriceListFromFile);
                                //
                                // Change Of Status Of Importing PriceList
                                //
                                if (ManagementOfPL.StatusOfImporting != 
                                    ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                                    if ((bool)PriceListFromFile.Rows[0]["AllPrices"])
                                    {
                                        ManagementOfPL.StatusOfImporting = 
                                            ManagementOfPriceList.StatusOfPriceList.FullPriceList;
                                    }
                                //
                                Console.WriteLine("S {0}", ManagementOfPL.GettingStatusOfPriceList.ToString());
                            }
                            catch (Exception E)
                            {
                                ReturningMessageAboutError(
                                    String.Format("Ошибка при добавлении данных файла Прайс-листа: {0}", 
                                    CurrentFileOfPriceList[0].ToString()), 
                                    E, false);
                            }
                        }
                        else
                        {
                            //
                            this.ReturningMessageOfInformation(
                                String.Format(
                                "В конвертированном файле Прайс-Листа '{0}' нет строк",
                                CurrentFileOfPriceList[0].ToString()));
                        }
                    }
                    catch (Exception E)
                    {
                        //
                        ReturningMessageAboutError(
                            String.Format(
                            "Ошибка при конвертации файла Прайс-Листа: {0}", 
                            CurrentFileOfPriceList[0].ToString()), 
                            E, false);
                        //
                        CurrentFileOfPriceList.Add(0);
                        CurrentFileOfPriceList.Add(true);
                    }
                    //
                    RecordingInLogFile("");
                }
            }
            //
            // Transfer Of Result Of Messages Of Converting
            //
            if (ExistedOfFilesOfPriceLists)// && ShowingMessage)
            {
                //
                string TextOfMessage = 
                    String.Format(
                    "Всего загружено файлов Прайс-Листов {0}\n", ListOfFilesOfPriceLists.Count);
                //
                foreach (ArrayList CurrentFileOfPriceList in ListOfFilesOfPriceLists)
                {
                    TextOfMessage += 
                        String.Format("Файл: {0}, От {1}, Строк: {2}{3}\n", 
                        Path.GetFileName(CurrentFileOfPriceList[0].ToString()), 
                        CurrentFileOfPriceList[1].ToString(), 
                        CurrentFileOfPriceList[2].ToString(), 
                        ((bool)CurrentFileOfPriceList[3]) ? ", Ошибка" : "");
                }
                //
                /*
                if (ShowingMessage)
                    new PharmaceuticalInformation.Service.Reportings("").
                        ReturningMessageOfInformation(TextOfMessage);
                */
                ManagementOfPL.MessageOfConverting = TextOfMessage;
            }
            //
            // Importing PriceList In DataBase
            //
            if ((CountOfConvertingRows > 0) && (ManagementOfPL.GettingCountForImporting > 0))
            {
                new System.Threading.Thread(
                    new System.Threading.ThreadStart(ManagementOfPL.ImportingPriceListInDataBase)).Start();
            }
            else { SuccessfulConverting = false; }
            //
            // Return
            //
            return SuccessfulConverting;
        }

        // Converting And Loading PriceList
        public bool ConvertingAndLoadingPriceListARH(bool ShowingMessage)
        {
            //
            bool SuccessfulConverting = true;
            //
            /*RecordingInLogFile(
                String.Format("Просмотр Папки с Прайс-листами: {0}", ConvertingOfPL.PathToFolderOfPriceLists));*/
            RecordingInLogFile(
                String.Format("Просмотр Папки с Прайс-листами: {0}", ""));
            //
            // Checking Folder Of Price Lists
            //
            //if (Directory.Exists(ConvertingOfPL.PathToFolderOfPriceLists))
            if (Directory.Exists(""))
            {
                //
                //string[] FilesOfDirectory = Directory.GetFiles(ConvertingOfPL.PathToFolderOfPriceLists);
                string[] FilesOfDirectory = Directory.GetFiles("");
                //
                // Checking Count Of Files In Directory
                //
                if (FilesOfDirectory.Length == 0)
                {
                    ReturningMessageAboutError(
                    String.Format("Нет файлов в директории: '{0}' ",
                    ""), new Exception("Exists False"), false);
                    /*ReturningMessageAboutError(
                    String.Format("Нет файлов в директории: '{0}' ",
                    ConvertingOfPL.PathToFolderOfPriceLists), new Exception("Exists False"), false);*/
                    //
                    SuccessfulConverting = false;
                }
                //
                // Scaning Of Files Of Directory
                //
                ArrayList CorrectFiles = new ArrayList();
                ManagementOfPL.StatusOfImporting = ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList;
                /*ManagementOfPriceList.StatusOfPriceList MainStatus = 
                    ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList;*/
                //
                foreach (string CurrentFile in FilesOfDirectory)
                {
                    //
                    bool CorrectConverting = true;
                    //
                    try
                    {
                        //
                        // Conformity Of File To Masks
                        //
                        if (ConvertingOfPL.ConformityOfFileToMasks(CurrentFile))
                        {
                            //
                            DataTable PriceListFromFile = new DataTable();
                            //
                            // Converting File Of PriceList
                            //
                            try { PriceListFromFile = ConvertingOfPL.Converting(CurrentFile); }
                            catch (Exception E)
                            {
                                ReturningMessageAboutError(
                                    String.Format("Ошибка при конвертации файла: {0}", CurrentFile), E, false);
                                CorrectConverting = false;
                            }
                            //
                            RecordingInLogFile(
                                String.Format("Count Of Processed Rows {0}", PriceListFromFile.Rows.Count));
                            //
                            // Checking Count Of Processing Rows
                            //
                            if (PriceListFromFile.Rows.Count > 0)
                            {
                                //
                                // Importing PriceList
                                //
                                try
                                {
                                    ManagementOfPL.AdditionOfPriceListForImporting(PriceListFromFile);
                                    //
                                    // Change Of Status Of PriceList
                                    //
                                    //if (ManagementOfPL.GettingStatusOfPriceList !=
                                    if (ManagementOfPL.StatusOfImporting != ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                                        if ((bool)PriceListFromFile.Rows[0]["AllPrices"])
                                        {
                                            ManagementOfPL.StatusOfImporting = 
                                                ManagementOfPriceList.StatusOfPriceList.FullPriceList;
                                            //MainStatus = ManagementOfPriceList.StatusOfPriceList.FullPriceList;
                                        }
                                    Console.WriteLine("S {0}", ManagementOfPL.GettingStatusOfPriceList.ToString());
                                }
                                catch (Exception E)
                                {
                                    ReturningMessageAboutError("Ошибка при импортировании Прайс-листа", E, false);
                                    CorrectConverting = false;
                                }
                            }
                            else
                            {
                                ReturningMessageAboutError(
                                    String.Format("В конвертированном Прайсе {0} нет строк", CurrentFile),
                                    new Exception("Count Of Rows 0"), false);
                                CorrectConverting = false;
                            }
                            //
                            // Deleting Of File Of PriceList
                            //
                            /*
                            if (CorrectConverting && (!ConvertingOfPL.NotDeletingPriceList))
                            {
                                RecordingInLogFile(String.Format("Удаление файла Прайс-листа {0}", CurrentFile));
                                try { /*CurrentFile.Delete();8/ }
                                catch (Exception E) { ReturningMessageAboutError("ERROR Ошибка при удалении", E, false); }
                            }
                            */
                            //
                            // Addition Of Result Of Converting
                            //
                            CorrectFiles.Add(CorrectConverting);
                        }
                    }
                    catch (Exception E)
                    {
                        ReturningMessageAboutError(
                            String.Format(
                            "Ошибка общего плана при конвертации и загрузке: {0}", CurrentFile), E, true);
                        CorrectConverting = false;
                    }
                }
                //
                // Updating Of PriceList
                //
                bool Updating = false;
                int CountOfConvertingPriceList = 0;
                //
                foreach (bool CurrentResult in CorrectFiles)
                    if (CurrentResult == true)
                    {
                        Updating = true;
                        CountOfConvertingPriceList++;
                    }
                //
                SuccessfulConverting = Updating;
                //
                if (Updating)
                {
                    //ManagementOfPL.ImportingPriceListInDataBase();
                    new System.Threading.Thread(
                            new System.Threading.ThreadStart(ManagementOfPL.ImportingPriceListInDataBase)).Start();
                    //
                    // Showing Message Of Result
                    //
                    string TextOfMessage = 
                            String.Format("Загружено Прайс-Листов: {0}", CountOfConvertingPriceList);
                    if (ShowingMessage)
                        new Service.Reportings("").ReturningMessageOfInformation(TextOfMessage);
                    this.RecordingInLogFile(TextOfMessage);
                    this.RecordingInLogFile("");
                }
            }
            else
            {
                ReturningMessageAboutError(
                    String.Format("Директория '{0}' не существует", 
                    ""), new Exception("Exists False"), false);
                /*ReturningMessageAboutError(
    String.Format("Директория '{0}' не существует",
    ConvertingOfPL.PathToFolderOfPriceLists), new Exception("Exists False"), false);*/
                SuccessfulConverting = false;
            }
            Console.WriteLine(SuccessfulConverting);
            //
            // Return
            //
            return SuccessfulConverting;
        }

        #endregion

        #region ' Sending Data '

        // Sending PriceList
        public void SendingPriceList(bool ShowingMessage)
        {
            //
            // Creating Structure For Sending 
            //
            DataSet StructureForSending = CreatingStructureForSending();
            //
            // Addition Of Data Of PriceList
            //
            AdditionOfDataOfPriceList(StructureForSending);
            //
            // Addition Of Data Of Settings
            //
            AdditionOfDataOfSettings(StructureForSending);
            //
            // Addition Of Data Of Dates Of Transfer
            //
            AdditionOfDataOfDatesOfTransfer(StructureForSending);
            //
            // Recording In Reportings
            //
            //new Service.Reportings(this.PathToLogFile).RecordingInLog("Отправка Прайс-Листа");
            RecordingInLogFile("Отправка Прайс-Листа");
            RecordingInLogFile("");
            //
            // Addition Of Data Of System Logs
            //
            AdditionOfDataOfSystemLogs(StructureForSending);
            //
            // Sending Data
            //
            ExchangeFTPAndLDB.SendingData(StructureForSending, _IDOfDrugstore);
            //
            // Reset Of Status Of Sending
            //
            //ManagementOfPL.ChangeOfSent(true);
            //
            // Message
            //
            string TextOfMessage = "Прайс-Лист Отправлен";
            //
            if (ShowingMessage)
            {
                new Service.Reportings(this.PathToLogFile).ReturningMessageOfInformation(TextOfMessage);
                RecordingInLogFile("");
            }
            else
            {
                //new Service.Reportings(this.PathToLogFile).RecordingInLog(TextOfMessage);
                RecordingInLogFile(TextOfMessage);
                RecordingInLogFile("");
            }
        }

        // Sending Log
        public void SendingLog()
        {
            //
            // Recording In LogFile
            //
            RecordingInLogFile("Sending Of System Logs");
            //
            // Creating Structure For Sending 
            //
            DataSet StructureForSending = CreatingStructureForSending();
            //
            // Addition Of Data Of Settings
            //
            AdditionOfDataOfSettings(StructureForSending);
            //
            // Addition Of Data Of Dates Of Transfer
            //
            AdditionOfDataOfDatesOfTransfer(StructureForSending);
            //
            // Addition Of Data Of System Logs
            //
            AdditionOfDataOfSystemLogs(StructureForSending);
            //
            // Sending Data
            //
            ExchangeFTPAndLDB.SendingData(StructureForSending, _IDOfDrugstore);
        }

        //
        public void SendingFileOfPriceList()
        {
            //
        }

        #endregion

        #region ' Addition For Sending '

        // Addition Of Data Of PriceList
        private void AdditionOfDataOfPriceList(DataSet StructureForSending)
        {
            //
            // Refresh Data Of PriceList
            //
            ManagementOfPL.RefreshData();
            //
            // Deactivation Of Filtering
            //
            bool EnabledFiltering = ManagementOfPL.EnabledFiltering;
            ManagementOfPL.EnabledFiltering = false;
            //
            // Getting
            //
            DataTable PriceListForSending = 
                ManagementOfPL.GettingPriceList("", 
                PharmaceuticalInformation.Reading.ReadingOfData.TypesOfFiltration.NotSearch, false, false);
            PriceListForSending.TableName = "PriceList";
            //
            // Filtering
            //
            DataTable FilteredPriceListForSending = new DataTable("FilteredPriceListForSending");
            //
            if (PriceListForSending.Rows.Count > 0)
            {
                //
                //
                //
                DataView FilteringOfPriceList = new DataView(PriceListForSending);
                FilteredPriceListForSending = 
                    FilteringOfPriceList.ToTable(
                    "PriceList", false, "ID_PH", "ID_PR", "Price", "Deleting", "Preferential", "AllPrices");
                //
                // Clearing PriceList For Sending
                //
                PriceListForSending.Clear();
                PriceListForSending.Dispose();
            }
            //
            // Addtion In Structure
            //
            if (FilteredPriceListForSending.Rows.Count > 0)
            { StructureForSending.Tables.Add(FilteredPriceListForSending); }
            else { RecordingInLogFile("Отправляемый Прайс-лист пуст."); RecordingInLogFile(""); }
            //
            // Activation Of Filtering
            //
            ManagementOfPL.EnabledFiltering = true;
            ManagementOfPL.EnabledFiltering = EnabledFiltering;
        }

        // Addition Of Data Of System Logs
        private void AdditionOfDataOfSystemLogs(DataSet StructureForSending)
        {
            //
            // Creating System Logs Of Drugstore
            //
            DataTable SystemLogsOfDrugstore = new DataTable("LogOfDrugstore");
            SystemLogsOfDrugstore.Columns.Add("Key", typeof(string));
            SystemLogsOfDrugstore.Columns.Add("Value", typeof(string));
            //
            // Reading Text Of System Logs
            //
            string TextOfSystemLogs = "";
            try
            {
                StreamReader SR = new StreamReader(_PathToLogFile, Encoding.Default);
                TextOfSystemLogs = SR.ReadToEnd();
                SR.Close();
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении журнала системы", E, false); }
            //
            // Addition In SystemLogsOfDrugstore
            //
            if ((TextOfSystemLogs != null) && (TextOfSystemLogs != ""))
                SystemLogsOfDrugstore.Rows.Add("SystemLogs", TextOfSystemLogs);
            else
                SystemLogsOfDrugstore.Rows.Add("SystemLogs", "ERROR Of Reading");
            //
            // Addition In Structure
            //
            StructureForSending.Tables.Add(SystemLogsOfDrugstore);
        }

        // Addition Of Data Of Settings
        private void AdditionOfDataOfSettings(DataSet StructureForSending)
        {
            // Checking
            if (_Settings != null)
            {
                // Addition Of Tables Of Settings
                foreach (DataTable CurrentTable in _Settings.Tables)
                    StructureForSending.Tables.Add(CurrentTable.Copy());
            }
        }

        // Addition Of Data Of Dates Of Transfer
        private void AdditionOfDataOfDatesOfTransfer(DataSet StructureForSending)
        {
            //
            if (_DatesOfTransfer != null)
            {
                // Addition Of Table Of Dates
                StructureForSending.Tables.Add(_DatesOfTransfer.Copy());
            }
        }

        #endregion

    }
}