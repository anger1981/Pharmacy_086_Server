using System;
using System.Collections.Generic;
using System.Text;
using PharmaceuticalInformation.BaseTypes;
using System.IO;
using System.Data;
using MySql.Data.MySqlClient;

namespace PharmaceuticalInformation.Drugstore
{
    public class FastTransferOfPriceLists : BaseType
    {

        #region ' Fields '

        //
        // Working Paths
        //
        private string PathToWorkingDirectory;
        private string PathToProgramOfArchiving;
        private string PathToSettingsFile;
        private string PathToTMPFolder;
        //
        // Working Modules Of Module Of Drugstore
        //
        private PharmaceuticalInformation.Drugstore.ManagementOfSettings ManagementOfSettings;
        private bool InitializedOfManagementOfSettings = false;
        //
        private PharmaceuticalInformation.Drugstore.ConvertingOfPriceList ConvertingOfPriceList;
        private bool InitializedOfConvertingOfPriceList = false;
        //
        private PharmaceuticalInformation.Drugstore.ExchangeFTPAndLocalDataBase ExchangeFTPAndLocalDataBase;
        private bool InitializedOfExchangeFTPAndLocalDataBase = false;
        //
        // Local Connection To Data Base
        //
        private MySqlConnection ConnectionToBase;
        private bool ConnectionIsWorking;
        //
        DataTable DataForImportingInPriceList;
        //
        // Timer
        //
        private System.Threading.Timer TimerOfImportingData;

        #endregion

        #region ' Designer '

        public FastTransferOfPriceLists(string PathToWorkingDirectory)
        {
            //
            // Initializing Fields
            //
            DataForImportingInPriceList = new DataTable("DataForImportingInPriceList");
            ConnectionIsWorking = false;
            //
            this.PathToWorkingDirectory = PathToWorkingDirectory;
            //
            // Initializing Working Paths
            //
            if (!this.PathToWorkingDirectory.EndsWith("\\"))
                this.PathToWorkingDirectory += "\\";
            //
            this.PathToLogFile = this.PathToWorkingDirectory + "AutomaticalDrugstore.txt";
            PathToProgramOfArchiving = this.PathToWorkingDirectory + "Rar.exe";
            PathToSettingsFile = this.PathToWorkingDirectory + "Settings.bin";
            PathToTMPFolder = Directory.GetParent(this.PathToWorkingDirectory).Parent.FullName + "\\TMP\\";
            //
            // Scraping Of Log File
            //
            ScrapingOfLogFile();
            //
            // Recording In Reports
            //
            this.RecordingInLogFile("Starting Initializing Of Fast Transfer Of PriceLists");
            this.RecordingInLogFile("");
            //
            // Creating TMP Directory If He Not Exists
            //
            if (!Directory.Exists(PathToTMPFolder))
            {
                try { Directory.CreateDirectory(PathToTMPFolder); }
                catch (Exception E)
                { this.RecordingInLogFile(String.Format("Ошибка при создании каталога TMP: {0}", E.Message)); }
            }
            //
            // Initializing Of Management Of Settings
            //
            try
            {
                //
                ManagementOfSettings = 
                    new PharmaceuticalInformation.Drugstore.ManagementOfSettings(
                        PathToSettingsFile, PathToLogFile);
                //
                InitializedOfManagementOfSettings = true;
            }
            catch (Exception E)
            {
                //
                InitializedOfManagementOfSettings = false;
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при инициализации модуля ManagementOfSettings: {0}", E.Message));
            }
            //
            // Initializing Of Converting Of PriceList
            //
            try
            {
                //
                ConvertingOfPriceList = 
                    new PharmaceuticalInformation.Drugstore.ConvertingOfPriceList(PathToLogFile);
                //
                InitializedOfConvertingOfPriceList = true;
            }
            catch (Exception E)
            {
                //
                InitializedOfConvertingOfPriceList = false;
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при инициализации модуля ConvertingOfPriceList: {0}", E.Message));
            }
            //
            // Initializing Of Exchange FTP And Local DataBase
            //
            try
            {
                //
                ExchangeFTPAndLocalDataBase = 
                    new PharmaceuticalInformation.Drugstore.ExchangeFTPAndLocalDataBase(PathToLogFile);
                //
                InitializedOfExchangeFTPAndLocalDataBase = true;
            }
            catch (Exception E)
            {
                //
                InitializedOfExchangeFTPAndLocalDataBase = false;
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при инициализации модуля ExchangeFTPAndLocalDataBase: {0}", E.Message));
            }
            //
            // Settings Of Exchange FTP And LocalDataBase
            //
            if (InitializedOfManagementOfSettings && InitializedOfExchangeFTPAndLocalDataBase)
            {
                ExchangeFTPAndLocalDataBase.PathToExportOfFTP = 
                    ManagementOfSettings.GettingPathOfExportingOnFTP;
                ExchangeFTPAndLocalDataBase.PathOfExportedPriceListsOfFTP = 
                    ManagementOfSettings.GettingPathOfExportedPriceListsOnFTP;
                ExchangeFTPAndLocalDataBase.PathToImportOfFTP = 
                    ManagementOfSettings.GettingPathOfImportingOfFTP;
                ExchangeFTPAndLocalDataBase.UsePassive = ManagementOfSettings.GettingPassiveMode;
                ExchangeFTPAndLocalDataBase.PathToTMPFolder = PathToTMPFolder;
                ExchangeFTPAndLocalDataBase.PathToArchivingProgram = PathToProgramOfArchiving;
            }
            //
            // Creating Of Connection
            //
            ConnectionIsWorking = true;
            /*
            if (InitializedOfManagementOfSettings)
            {
                //
                // Recording In Reports
                //
                this.RecordingInLogFile("Starting Creating Of Connection");
                this.RecordingInLogFile("");
                //
                // Getting String Of Connection
                //
                string StringOfConnection = ManagementOfSettings.GettingStringOfConnection;
                //
                ConnectionIsWorking = true;
                //
                // Checking Connection
                //
                try
                {
                    //
                    ConnectionToBase = new MySqlConnection(StringOfConnection);
                    //
                    try
                    {
                        ConnectionToBase.Open();
                        ConnectionToBase.Close();
                    }
                    catch (Exception E)
                    {
                        //
                        ConnectionIsWorking = false;
                        //
                        this.RecordingInLogFile(String.Format("ERROR Ошибка при открытии подключения редактирования: {0}", E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    ConnectionIsWorking = false;
                    //
                    this.RecordingInLogFile(String.Format("ERROR Ошибка при создании подключения редактирования: {0}", E.Message));
                }
                //
                // Recording In Reports
                //
                this.RecordingInLogFile("Stoping Creating Of Connection");
                this.RecordingInLogFile("");
            }
            */
            //
            // Initializing Timer Of Importing Data
            //
            if (InitializedOfManagementOfSettings && ConnectionIsWorking)
            {
                //
                int IntervalOfImporting = 6;//ManagementOfSettings.IntervalOfImporting;
                //
                if (IntervalOfImporting > 0)
                {
                    TimerOfImportingData = 
                        new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfImportingData_Tick),
                            null, new TimeSpan(0, 0, 5), new TimeSpan(IntervalOfImporting, 0, 0));
                }
            }
            //
            // Recording In Reports
            //
            this.RecordingInLogFile("Stoping Initializing Of Fast Transfer Of PriceLists");
            this.RecordingInLogFile("");
        }

        // Scraping Of Log File
        public new void ScrapingOfLogFile()
        {
            //
            this.CountOfRowsInLogFile = 5000;
            this.EnableScrapingLog = true;
            //
            this.RecordingInLogFile("Scraping Of Log File");
            this.RecordingInLogFile("");
            //
            this.EnableScrapingLog = false;
        }

        #endregion

        #region ' Management Of Transfer '

        // Starting Transfer Of PriceLists
        public void StartingTransferOfPriceLists()
        {
            //
            // Recording In Reports
            //
            this.RecordingInLogFile("Starting Transfer Of PriceLists");
            this.RecordingInLogFile("");
        }

        // Stoping Transfer Of PriceLists
        public void StopingTransferOfPriceLists()
        {
            //
            // Recording In Reports
            //
            this.RecordingInLogFile("Stoping Transfer Of PriceLists");
            this.RecordingInLogFile("");
            //
            // Sending Logs
            //
            SendingLogs(102);
        }

        #endregion

        #region ' Timer '

        private void TimerOfImportingData_Tick(object sender)
        {
            //
            // Starting Importing PriceLists
            //
            ConvertingAndTransferOfPriceLists();
        }

        #endregion

        #region ' Converting And Transfer Of PriceLists '

        // Converting And Transfer Of PriceLists
        public void ConvertingAndTransferOfPriceLists()
        {
            //
            // Scraping Of Log File
            //
            ScrapingOfLogFile();
            //
            // Recording In Reports
            //
            this.RecordingInLogFile("Converting And Transfer Of PriceLists");
            this.RecordingInLogFile("");
            //
            // Sanction On Executing Of Importing
            //
            bool SanctionOnExecutingOfImporting = true;
            //
            // Checking Working Necessary Modules
            //
            if (!(
                InitializedOfManagementOfSettings && 
                InitializedOfConvertingOfPriceList && 
                ConnectionIsWorking))
                SanctionOnExecutingOfImporting = false;
            //
            // Getting Path To PriceLists AND Settings Of ConvertingOfPriceList
            //
            string PathToFolderOfPriceLists = "";
            //
            // Getting ID Of First Registered Drugstore
            //
            int IDOfDrugstore = 0;
            //
            if (SanctionOnExecutingOfImporting)
            {
                System.Data.DataTable ListOfRegisteredDrugstores = 
                    ManagementOfSettings.GettingRegistrationOfDrugstores();
                //
                if (ListOfRegisteredDrugstores.Rows.Count > 0)
                {
                    IDOfDrugstore = (int)ListOfRegisteredDrugstores.Rows[0]["ID"];
                }
                else
                    SanctionOnExecutingOfImporting = false;
            }
            //
            // !!!
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                if (IDOfDrugstore > 0)
                {
                    //
                    // Clearing Settings Of ConvertingOfPriceList
                    //
                    ConvertingOfPriceList.MaskOfFullPriceList = "";
                    ConvertingOfPriceList.MaskOfReceipts = "";
                    ConvertingOfPriceList.MaskOfDeleting = "";
                    ConvertingOfPriceList.UseOfIDOfPriceList = false;
                    ConvertingOfPriceList.IDOfPharmacy = 0;
                    //
                    PathToFolderOfPriceLists = 
                        ManagementOfSettings.GettingPathToFolderOfPriceLists(IDOfDrugstore);
                    //
                    // Settings Of ConvertingOfPriceList
                    //
                    ConvertingOfPriceList.MaskOfFullPriceList = 
                        ManagementOfSettings.GettingMaskOfFullPriceList(IDOfDrugstore);
                    //
                    ConvertingOfPriceList.MaskOfReceipts = 
                        ManagementOfSettings.GettingMaskOfIncomingPriceList(IDOfDrugstore);
                    //
                    ConvertingOfPriceList.MaskOfDeleting = 
                        ManagementOfSettings.GettingMaskOfSoldPriceList(IDOfDrugstore);
                    //
                    ConvertingOfPriceList.UseOfIDOfPriceList = 
                        ManagementOfSettings.GettingUseOfIDOfPriceList(IDOfDrugstore);
                    //
                    ConvertingOfPriceList.IDOfPharmacy = IDOfDrugstore;
                }
                else
                    SanctionOnExecutingOfImporting = false;
            }
            //
            // !!!
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                // Checking Of Existing Of Directory Of PriceLists
                //
                bool ExistedOfDirectory = false;
                //
                try { ExistedOfDirectory = Directory.Exists(PathToFolderOfPriceLists); }
                catch (Exception E)
                {
                    RecordingInLogFile(
                        String.Format("ERROR Ошибка при проверке существования Директории Прайс-Листов: {0}", E.Message));
                }
                //
                // Recording In Reports And Cancel Convertings
                //
                if (ExistedOfDirectory)
                {
                    //
                    // Recording In Reports
                    //
                    RecordingInLogFile(
                        String.Format("Загрузка файлов Прайс-Листов из директории: {0}", PathToFolderOfPriceLists));
                }
                else
                {
                    //
                    // Recording In Reports
                    //
                    RecordingInLogFile(
                        String.Format("ERROR Директория Прайс-Листов '{0}' не существует", PathToFolderOfPriceLists));
                    //
                    SanctionOnExecutingOfImporting = false;
                }
            }
            //
            // Getting List Of Files Of PriceLists
            //
            DataTable ListOfFilesOfPriceLists = new DataTable("ListOfFilesOfPriceLists");
            //
            ListOfFilesOfPriceLists.Columns.Add("PathToFile", typeof(string));
            ListOfFilesOfPriceLists.Columns.Add("DateOfFile", typeof(DateTime));
            ListOfFilesOfPriceLists.Columns.Add("CountOfRows", typeof(int));
            ListOfFilesOfPriceLists.Columns.Add("ErrorConverting", typeof(bool));
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                // Getting List Of Files Of PriceLists
                //
                string[] FilesOfDirectory = new string[0];
                //
                try { FilesOfDirectory = Directory.GetFiles(PathToFolderOfPriceLists); }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при получении списка файлов Прйс-Листов: {0}", E.Message)); }
                //
                // Addition Of Files Of PriceList In List
                //
                foreach (string CurrentFile in FilesOfDirectory)
                    if (ConvertingOfPriceList.ConformityOfFileToMasks(CurrentFile))
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
                        ListOfFilesOfPriceLists.Rows.Add(CurrentFile, DateOfFileOfPriceList);
                    }
                //
                // Checking Of Count Of Files Of PriceLists
                //
                if (ListOfFilesOfPriceLists.Rows.Count == 0)
                {
                    //
                    // Recording In Reports
                    //
                    RecordingInLogFile(
                        String.Format("В директории '{0}' НЕТ файлов Прайс-Листов", PathToFolderOfPriceLists));
                    //
                    SanctionOnExecutingOfImporting = false;
                }
            }
            //
            // Converting PriceLists And Addition For Importing To DataBase
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                // Reset Data For Importing In PriceList
                //
                DataForImportingInPriceList = new DataTable("DataForImportingInPriceList");
                //
                // Scaning Of Files Of PriceLists
                //
                foreach (DataRow CurrentFileOfPriceList in ListOfFilesOfPriceLists.Rows)
                {
                    //
                    try
                    {
                        //
                        DataTable PriceListFromFile = new DataTable();
                        //
                        // Converting File Of PriceList
                        //
                        PriceListFromFile = 
                            ConvertingOfPriceList.Converting(CurrentFileOfPriceList["PathToFile"].ToString());
                        //
                        CurrentFileOfPriceList["CountOfRows"] = PriceListFromFile.Rows.Count;
                        CurrentFileOfPriceList["ErrorConverting"] = false;
                        //
                        RecordingInLogFile(
                            String.Format("Дата файла Прайс-листа: {0}", 
                            CurrentFileOfPriceList["DateOfFile"].ToString()));
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
                                AdditionOfPriceListForImporting(PriceListFromFile);
                                //
                                // Change Of Status Of Importing PriceList
                                //
                                /*
                                if (GettingStatusOfPriceList != StatusOfPriceList.FullPriceList)
                                    if ((bool)PriceListFromFile.Rows[0]["AllPrices"])
                                    {
                                        ChangeOfStatusOfPriceList(StatusOfPriceList.FullPriceList);
                                    }
                                */
                            }
                            catch (Exception E)
                            {
                                RecordingInLogFile(
                                    String.Format("ERROR Ошибка при добавлении данных файла Прайс-листа '{0}': {1}", 
                                    CurrentFileOfPriceList["PathToFile"].ToString(), 
                                    E.Message));
                            }
                        }
                        else
                        {
                            //
                            RecordingInLogFile(
                                String.Format("В конвертированном файле Прайс-Листа '{0}' нет строк", 
                                PriceListFromFile.Rows.Count.ToString()));
                        }
                    }
                    catch (Exception E)
                    {
                        //
                        RecordingInLogFile(
                            String.Format(
                            "ERROR Ошибка при конвертации файла Прайс-Листа '{0}': {1}", 
                            CurrentFileOfPriceList[0].ToString(), E.Message));
                        //
                        CurrentFileOfPriceList["CountOfRows"] = 0;
                        CurrentFileOfPriceList["ErrorConverting"] = true;
                    }
                    //
                    RecordingInLogFile("");
                }
            }
            //
            // Recording In Reports
            //
            this.RecordingInLogFile(String.Format("Count Of Prices For Sending: {0}", DataForImportingInPriceList.Rows.Count));
            this.RecordingInLogFile("");
            //
            // Sending PriceList
            //
            SendingPriceList(102); // IDOfDrugstore
            //
            // Reset Data For Importing In PriceList
            //
            DataForImportingInPriceList.Dispose();
            DataForImportingInPriceList = new DataTable("DataForImportingInPriceList");
            //
            // !!!
            //
            System.Threading.Thread.Sleep(1000);
            //
            // !!!
            //
            GC.Collect();
        }

        // Importing Prices In PriceList
        public void AdditionOfPriceListForImporting(DataTable NewPrices)
        {
            //
            // Checking Of Table
            //
            bool CorrectTable = true;
            string[] ContainsColumns = new string[6] { "ID_PH", "ID_PR", "Price", "Deleting", "Preferential", "AllPrices" };
            foreach (string CurrentColumns in ContainsColumns)
                if (!NewPrices.Columns.Contains(CurrentColumns))
                    CorrectTable = false;
            //
            // Addition And Association Of PriceLists
            //
            if (CorrectTable)
            {
                //
                // Comparisons
                //
                /*
                try
                {
                    //
                    // Checking Presence Of Comparisons
                    //
                    bool ComparisonsPresence = false;
                    //
                    if (ConnectionIsWorking)
                    {
                        //
                        MySqlCommand Executing = new MySqlCommand("SELECT COUNT(*) FROM Matching", ConnectionToBase);
                        //
                        try
                        {
                            //
                            int CountOfComparisons = -1;
                            //
                            OpeningConnection(ConnectionToBase);
                            //
                            CountOfComparisons = Convert.ToInt32(Executing.ExecuteScalar());
                            //
                            ClosingConnection(ConnectionToBase);
                            //
                            if (CountOfComparisons > 0)
                                ComparisonsPresence = true;
                        }
                        catch (Exception E)
                        {
                            //
                            ClosingConnection(ConnectionToBase);
                            //
                            RecordingInLogFile(String.Format("ERROR Ошибка при проверке наличия сопоставлений: {0}", E.Message));
                        }
                    }
                    //
                    // Converting IDs Of Products
                    //
                    if (ComparisonsPresence)
                    {
                        //
                        RecordingInLogFile("Usage Of Table Of Comparisons");
                        //
                        // Getting Comparisons
                        //
                        DataTable ListOfComparisons = new DataTable("Comparisons");
                        //
                        MySqlCommand SelectionOfComparisons = new MySqlCommand(
                            "SELECT Id_client_product AS 'ID_PC', Id_product AS 'ID_PR' FROM Matching;", 
                            ConnectionToBase);
                        MySqlDataAdapter GettingComparisons = new MySqlDataAdapter(SelectionOfComparisons);
                        //
                        FillingWithDataOfTable(ListOfComparisons, "Comaprisons", GettingComparisons, true);
                        //
                        // Converting Of Codes
                        //
                        int CountOfConverting = 0, CountOfDeleting = 0;
                        //
                        if (ListOfComparisons.Rows.Count > 0)
                        {
                            //
                            NewPrices.AcceptChanges();
                            //
                            ListOfComparisons.PrimaryKey = new DataColumn[1] { ListOfComparisons.Columns["ID_PC"] };
                            //
                            foreach (DataRow CurrentPrice in NewPrices.Rows)
                            {
                                //
                                DataRow FindRow = ListOfComparisons.Rows.Find(CurrentPrice["ID_PR"]);
                                if (FindRow != null)
                                { CurrentPrice["ID_PR"] = FindRow["ID_PR"]; CountOfConverting++; }
                                else
                                { CurrentPrice.Delete(); CountOfDeleting++; }
                            }
                        }
                        //
                        // Recording In Reports
                        //
                        int CountOfPrices = NewPrices.Rows.Count;
                        //
                        RecordingInLogFile(
                            String.Format("Count Of Prices: {0} Count Of Converting: {1}, Count Of Deleting: {2}", 
                            CountOfPrices, CountOfConverting, CountOfDeleting));
                        //
                        // Accept Changes
                        //
                        NewPrices.AcceptChanges();
                    }
                }
                catch (Exception E)
                {
                    //
                    ClosingConnection(ConnectionToBase);
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при конвертации кодов продуктов: {0}", E.Message));
                }
                */
                //
                // Association Of PriceLists
                //
                if (DataForImportingInPriceList.Rows.Count == 0)
                    DataForImportingInPriceList = NewPrices.Clone();
                //
                foreach (DataRow CurrentPrice in NewPrices.Rows)
                { DataForImportingInPriceList.Rows.Add(CurrentPrice.ItemArray); }
            }
            else
            {
                RecordingInLogFile("ERROR Error In table NewPrices there are no necessary columns");
            }
        }

        #endregion

        #region ' Status Of PriceList '

        // Getting Status Of PriceList
        public StatusOfPriceList GettingStatusOfPriceList
        {
            get
            {
                //
                // Reading Status Of PriceList
                //
                StatusOfPriceList CurrentStatus = StatusOfPriceList.SupplementingPriceList;
                //
                MySqlCommand GettingStatus = new MySqlCommand(
                    "SELECT Value FROM service WHERE Id_Service = 6;", ConnectionToBase);
                //
                if (ConnectionIsWorking)
                    try
                    {
                        //
                        // !!!
                        //
                        int ValueOfStatus = -1;
                        //
                        OpeningConnection(GettingStatus.Connection);
                        //
                        ValueOfStatus = (int)GettingStatus.ExecuteScalar();
                        //
                        ClosingConnection(GettingStatus.Connection);
                        //
                        // !!!
                        //
                        if (ValueOfStatus == 1)
                            CurrentStatus = StatusOfPriceList.FullPriceList;
                        else
                            CurrentStatus = StatusOfPriceList.SupplementingPriceList;
                    }
                    catch (Exception E)
                    {
                        //
                        ClosingConnection(GettingStatus.Connection);
                        //
                        RecordingInLogFile(
                            String.Format("ERROR Ошибка при чтении статуса Прайс-Листа: {0}", E.Message));
                    }
                //
                // Return
                //
                return CurrentStatus;
            }
        }

        // Change Of Status Of PriceList
        public void ChangeOfStatusOfPriceList(StatusOfPriceList NewStatus)
        {
            //
            // Updating Status Of PriceList
            //
            MySqlCommand UpdatingStatus = new MySqlCommand(
                String.Format("UPDATE service SET Value = {0} WHERE Id_Service = 6;", 
                (NewStatus == StatusOfPriceList.FullPriceList) ? "1" : "0"), ConnectionToBase);
            try
            {
                //
                // !!!
                //
                OpeningConnection(UpdatingStatus.Connection);
                //
                UpdatingStatus.ExecuteScalar();
                //
                ClosingConnection(UpdatingStatus.Connection);
            }
            catch (Exception E)
            {
                //
                ClosingConnection(UpdatingStatus.Connection);
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при обновлении статуса Прайс-Листа: {0}", E.Message));
            }
        }

        // Status Of PriceList
        public enum StatusOfPriceList
        {
            FullPriceList,
            SupplementingPriceList
        }

        #endregion

        #region ' Filling With Data Of Table '

        // Filling With Data Of Table
        private void FillingWithDataOfTable(DataTable FilledTable, string TableName, MySqlDataAdapter GettingData, bool CreatingSchema)
        {
            //
            // Filling Of Data
            //
            try
            {
                if (CreatingSchema)
                    GettingData.FillSchema(FilledTable, SchemaType.Source);
                GettingData.Fill(FilledTable);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(String.Format("ERROR Ошибка при заполнении таблицы '{0}': {1}", TableName, E.Message));
            }
        }

        #endregion

        #region ' Management Of Connection '

        // Opening Connection
        private void OpeningConnection(MySqlConnection Connection)
        {
            //
            if ((Connection != null) && ConnectionIsWorking)
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
        }

        // Closing Connection
        private void ClosingConnection(MySqlConnection Connection)
        {
            //
            if ((Connection != null) && ConnectionIsWorking)
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
        }

        #endregion

        #region ' Sending Data '

        // Sending PriceList
        public void SendingPriceList(int IDOfDrugstore)
        {
            //
            // Checking Is Registration
            //
            bool CheckedRegistration = true; // this.CheckingIsRegistration();
            //
            // Checking Of Communication
            //
            bool CheckedCommunication = true;// this.CheckingOfCommunication();
            //
            if (CheckedRegistration && CheckedCommunication)
            {
                //
                // Creating Structure For Sending 
                //
                DataSet StructureForSending = CreatingStructureForSending(IDOfDrugstore);
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
                //AdditionOfDataOfDatesOfTransfer(StructureForSending);
                //
                // Recording In Reportings
                //
                this.RecordingInLogFile("Отправка Прайс-Листа");
                this.RecordingInLogFile("");
                //
                // Addition Of Data Of System Logs
                //
                AdditionOfDataOfSystemLogs(StructureForSending);
                //
                // Sending Data
                //
                ExchangeFTPAndLocalDataBase.SendingData(StructureForSending, IDOfDrugstore);
                //
                // Message
                //
                this.RecordingInLogFile("Прайс-Лист Отправлен");
            }
            //
            // Message Of Sending
            //
            if (!CheckedRegistration || !CheckedCommunication)
            {
                //
                this.RecordingInLogFile("Прайс-Лист НЕ Отправлен");
            }
            //
            // Recording In LogFile Of No Registration
            //
            if (!CheckedRegistration)
            {
                RecordingInLogFile("INFOR Нет регистрации Аптеки, Отправка данных невозможна");
                RecordingInLogFile("");
            }
            //
            // Recording In LogFile Of No Communication
            //
            if (!CheckedCommunication)
            {
                RecordingInLogFile("INFOR Нет связи с FTP Сервером, Отправка данных невозможна");
                RecordingInLogFile("");
            }
        }

        // Sending Logs
        public void SendingLogs(int IDOfDrugstore)
        {
            //
            // Checking Is Registration
            //
            bool CheckedRegistration = true; // this.CheckingIsRegistration();
            //
            // Checking Of Communication
            //
            bool CheckedCommunication = true;// this.CheckingOfCommunication();
            //
            if (CheckedRegistration && CheckedCommunication)
            {
                //
                // Creating Structure For Sending 
                //
                DataSet StructureForSending = CreatingStructureForSending(IDOfDrugstore);
                //
                // Addition Of Data Of Settings
                //
                AdditionOfDataOfSettings(StructureForSending);
                //
                // Addition Of Data Of Dates Of Transfer
                //
                //AdditionOfDataOfDatesOfTransfer(StructureForSending);
                //
                // Addition Of Data Of System Logs
                //
                AdditionOfDataOfSystemLogs(StructureForSending);
                //
                // Sending Data
                //
                ExchangeFTPAndLocalDataBase.SendingData(StructureForSending, IDOfDrugstore);
                //
                // Message
                //
                this.RecordingInLogFile("Системный журнал Отправлен");
            }
            //
            // Message Of Sending
            //
            if (!CheckedRegistration || !CheckedCommunication)
            {
                //
                this.RecordingInLogFile("Системный журнал НЕ Отправлен");
            }
            //
            // Recording In LogFile Of No Registration
            //
            if (!CheckedRegistration)
            {
                RecordingInLogFile("INFOR Нет регистрации Аптеки, Отправка данных невозможна");
                RecordingInLogFile("");
            }
            //
            // Recording In LogFile Of No Communication
            //
            if (!CheckedCommunication)
            {
                RecordingInLogFile("INFOR Нет связи с FTP Сервером, Отправка данных невозможна");
                RecordingInLogFile("");
            }
        }

        // Checking Of Communication
        public bool CheckingOfCommunication()
        {
            //
            // Checking
            //
            bool Checked = false;
            //
            if (InitializedOfManagementOfSettings)
                Checked = ManagementOfSettings.CheckingOfCommunication();
            //
            // Return
            //
            return Checked;
        }

        #region ' Addition For Sending '

        // Creating Structure For Sending
        private DataSet CreatingStructureForSending(int IDOfDrugstore)
        {
            //
            // Creating
            //
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
            // Filling
            //
            StructureForSending.Tables["Information"].Rows.Add("IDOfDrugstore", IDOfDrugstore);
            StructureForSending.Tables["Information"].Rows.Add("DateOfSending", DateTime.Now);
            StructureForSending.Tables["Information"].AcceptChanges();
            //
            // Return
            //
            return StructureForSending;
        }

        // Addition Of Data Of PriceList
        private void AdditionOfDataOfPriceList(DataSet StructureForSending)
        {
            //
            // Getting
            //
            //DataTable PriceListForSending = DataForImportingInPriceList.Copy();
            DataTable PriceListForSending = DataForImportingInPriceList;
            //
            PriceListForSending.TableName = "PriceList";
            //
            // Filtering
            //
            /*
            DataView FilteringOfPriceList = new DataView(PriceListForSending);
            PriceListForSending = 
                FilteringOfPriceList.ToTable(
                "PriceList", false, "ID_PH", "ID_PR", "Price", "Deleting", "Preferential", "AllPrices");
            */
            //
            // Addition In Structure
            //
            StructureForSending.Tables.Add(PriceListForSending);
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
            //
            try
            {
                StreamReader SR = 
                    new StreamReader(PathToWorkingDirectory + "AutomaticalDrugstore.txt", Encoding.Default);
                TextOfSystemLogs = SR.ReadToEnd();
                SR.Close();
            }
            catch (Exception E)
            { this.RecordingInLogFile(String.Format("ERROR Ошибка при чтении журнала сервиса: {0}", E.Message)); }
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
            //
            if (ManagementOfSettings.GettingAllSettings != null)
            {
                //
                // Addition Of Tables Of Settings
                //
                foreach (DataTable CurrentTable in ManagementOfSettings.GettingAllSettings.Tables)
                    StructureForSending.Tables.Add(CurrentTable.Copy());
            }
        }

        // Addition Of Data Of Dates Of Transfer
        private void AdditionOfDataOfDatesOfTransfer(DataSet StructureForSending)
        {
            //
            DataTable DatesOfTransfer = new DataTable("DatesOfTransfer");
            //
            // Creating Text Of Command Of Selection
            //
            string TextOfCommandOfFilling = 
                "SELECT S.Id_Service AS 'ID', S.Name_short AS 'Name', S.Value AS 'Value', S.Date_Service AS 'Date' " + 
                "FROM Service AS S " + 
                "ORDER BY S.Id_Service;";
            //
            // Filling
            //
            MySqlCommand SelectionOfDatesOfTransfer = new MySqlCommand(TextOfCommandOfFilling, ConnectionToBase);
            MySqlDataAdapter GettingDatesOfTransfer = new MySqlDataAdapter(SelectionOfDatesOfTransfer);
            //
            FillingWithDataOfTable(DatesOfTransfer, "DatesOfTransfer", GettingDatesOfTransfer, true);
            //
            // Addition Of Table Of Dates
            //
            StructureForSending.Tables.Add(DatesOfTransfer);
        }

        #endregion

        #endregion

    }
}