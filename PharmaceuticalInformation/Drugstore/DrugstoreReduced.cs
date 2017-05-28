using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class DrugstoreReduced : BaseType
    {

        #region ' Fields '

        //
        // Transfer
        //
        private ConvertingOfPriceList Converting;
        private ManagementOfPriceList Management;
        private Reading.ReadingOfDataForMySQL Reading;
        private ExchangeFTPAndLocalDataBase Exchange;
        private TransferOfData Transfer;
        //
        // Timer
        //
        private System.Threading.Timer TimerOfTransfer;
        //
        private string _PathToFileOfSettings;
        private DataTable CurrentSettings;
        private int IDOfPharmacy = 103;
        //
        private bool InWork;

        #endregion

        #region ' Designer '

        public DrugstoreReduced()
        {
            //
            // Initializing Paths Of Starting Folder And Log File
            //
            //string PathOfStartingFolder = System.Windows.Forms.Application.StartupPath;
            //this.PathToLogFile = PathOfStartingFolder + "\\ServerOfTransferOfPriceLists.txt";
            //
            RecordingInLogFile("--");
            RecordingInLogFile("Starting Initializing");
            //
            // Initializing Of Fields
            //
            InitializingOfFields();
            //
            RecordingInLogFile("Stoping Initializing");
            //RecordingInLogFile("Transfer Of PriceLists");
        }

        // Initializing Of Fields
        public void InitializingOfFields()
        {
            //
            // Initializing Paths Of Starting Folder And Log File
            //
            string PathOfStartingFolder = System.Windows.Forms.Application.StartupPath;
            this.PathToLogFile = PathOfStartingFolder + "\\ServerOfTransferOfPriceLists.txt";
            this.EnableScrapingLog = true;
            this.CountOfRowsInLogFile = 1000;
            //
            this.PathToFileOfSettings = PathOfStartingFolder + "\\Settings.bin";
            //
            // Reading Settings Of Working
            //
            CurrentSettings = new DataTable("Settings");
            DataTable SettingsOfWorking = GettingSettings();
            //
            // Initializing Converting Of PriceList
            //
            try
            {
                Converting = new ConvertingOfPriceList(this.PathToLogFile);
                //
                // Settings Of ConvertingOfPriceList
                //
                /*Converting.PathToFolderOfPriceLists = 
                    SettingsOfWorking.Rows.Find("PathToFolderOfPriceLists")["Value"].ToString();*/
                Converting.IDOfPharmacy =
                    (int)SettingsOfWorking.Rows.Find("IDOfPharmacy")["Value"];
                Converting.MaskOfFullPriceList =
                    SettingsOfWorking.Rows.Find("MaskOfFullPriceList")["Value"].ToString();
                Converting.MaskOfReceipts =
                    SettingsOfWorking.Rows.Find("MaskOfReceptionOfPriceList")["Value"].ToString();
                Converting.MaskOfDeleting =
                    SettingsOfWorking.Rows.Find("MaskOfDefectionOfPriceList")["Value"].ToString();
                //
                /*Converting.NotDeletingPriceList =
                    (bool)SettingsOfWorking.Rows.Find("NotDeletingPriceList")["Value"];*/
                Converting.UseOfIDOfPriceList = (bool)SettingsOfWorking.Rows.Find("UseOfIDOfPriceList")["Value"];
                //
                Converting.ShowingMessages = false;
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Error At Initializaing ConvertingOfPriceList: {0}", E.Message));
            }
            //
            RecordingInLogFile("Initializing ConvertingOfPriceList");
            //
            // Initializing Reading Of Information Data For MySQL
            //
            try
            {
                Reading =
                    new Reading.ReadingOfDataForMySQL(
                        SettingsOfWorking.Rows.Find("StringOfConnection")["Value"].ToString(),
                        PathOfStartingFolder + "\\", this.PathToLogFile);
                //
                // Settings Of ReadingOfInformationDataForMySQL
                //
                Reading.ShowingMessages = false;
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Error At Initializaing ReadingOfInformationDataForMySQL: {0}", E.Message));
            }
            //
            RecordingInLogFile("Initializing ReadingOfInformationDataForMySQL");
            //
            // Initializing Management Of PriceList
            //
            try
            {
                Management = new ManagementOfPriceList(
                    SettingsOfWorking.Rows.Find("StringOfConnection")["Value"].ToString(),
                    Reading, this.PathToLogFile);
                //
                // Settings Of ManagementOfPriceList
                //
                Management.LoadingPriceListIsCompleted += new ManagementOfPriceList.ReturnOfEvent(Management_LoadingPriceListIsCompleted);
                Management.ShowingMessages = false;
                //Management.SizeOfPackageOfUpdating = 1000;
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Error At Initializaing ManagementOfPriceList: {0}", E.Message));
            }
            //
            RecordingInLogFile("Initializing ManagementOfPriceList");
            //
            // Exchange FTP And Local DataBase
            //
            try
            {
                Exchange = new ExchangeFTPAndLocalDataBase(this.PathToLogFile);
                //
                // Settings Of ExchangeFTPAndLocalDataBase
                //
                Exchange.PathToArchivingProgram = PathOfStartingFolder + "\\Rar.exe";
                Exchange.PathToImportOfFTP = SettingsOfWorking.Rows.Find("PathOfImportingData")["Value"].ToString();
                Exchange.PathToTMPFolder = Path.GetDirectoryName(PathOfStartingFolder) + "\\TMP\\";
                Exchange.UsePassive = (bool)SettingsOfWorking.Rows.Find("UsePassive")["Value"];
                //
                Exchange.ShowingMessages = false;
                Exchange.IncludedShowingMessages = false;
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Error At Initializaing ExchangeFTPAndLocalDataBase: {0}", E.Message));
            }
            //
            RecordingInLogFile("Initializing ExchangeFTPAndLocalDataBase");
            //
            // Transfer Of Data
            //
            try
            {
                Transfer = new TransferOfData(Converting, Management, Exchange, this.PathToLogFile);
                //
                // Settings Of TransferOfData
                //
                Transfer.DatesOfTransfer = Reading.GettingDatesOfTransfer();
                Transfer.PathToSendLogFile = this.PathToLogFile;
                Transfer.StringOfConnection = SettingsOfWorking.Rows.Find("StringOfConnection")["Value"].ToString();
                Transfer.IDOfDrugstore = (int)SettingsOfWorking.Rows.Find("IDOfPharmacy")["Value"];
                //
                Transfer.ShowingMessages = false;
                //
                // Creating List Of Settings
                //
                DataSet Settings = new DataSet("SendingData");
                //
                DataTable InformationOfSettings = new DataTable("InformationOfSettings");
                InformationOfSettings.Columns.Add("Key", typeof(string));
                InformationOfSettings.Columns.Add("Value", typeof(object));
                InformationOfSettings.Rows.Add("VersionOfSettings", "1");
                InformationOfSettings.Rows.Add("VersionOfApplication", "1.5.0.0");
                Settings.Tables.Add(InformationOfSettings);
                //
                DataTable ListOfSettings = GettingSettings().Copy(); ;
                ListOfSettings.TableName = "ListOfSettings";
                Settings.Tables.Add(ListOfSettings);
                //
                Settings.AcceptChanges();
                //
                Transfer.Settings = Settings;
                //
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Error At Initializaing TransferOfData: {0}", E.Message));
            }
            RecordingInLogFile("Initializing TransferOfData");
            //
            //Transfer.SendingLog();
        }

        #endregion

        #region ' Gettings Of Settings Of Working '

        // Path To File Of Settings
        private string PathToFileOfSettings
        {
            get
            { return _PathToFileOfSettings; }
            set
            { _PathToFileOfSettings = value; }
        }

        // Reading Settings
        private DataTable ReadingSettings()
        {
            //
            DataTable ReadSettings = new DataTable("Settings");
            //
            // !!!
            //
            if (File.Exists(_PathToFileOfSettings))
            {
                //
                try
                {
                    //
                    // Creating FileStream
                    //
                    FileStream FS = new FileStream(_PathToFileOfSettings, FileMode.Open);
                    //
                    // Deserialize Settings
                    //
                    try { ReadSettings = (DataTable)new BinaryFormatter().Deserialize(FS); }
                    catch (Exception E)
                    { this.RecordingInLogFile(String.Format("Ошибка при чтении файла настроек: {0}", E.Message)); }
                    //
                    // Closing FileStream
                    //
                    try
                    { FS.Close(); }
                    catch (Exception E)
                    { this.RecordingInLogFile(String.Format("Ошибка при закрытии файла настроек: {0}", E.Message)); }
                }
                catch (Exception E)
                { this.RecordingInLogFile(String.Format("Ошибка при открытии файла настроек: {0}", E.Message)); }
            }
            else
            { this.RecordingInLogFile("Нет файла настроек"); }
            //
            // !!!
            //
            if ((ReadSettings.Rows.Contains("ServerOfDataBase")) && (ReadSettings.Rows.Contains("Port")))
            {
                ReadSettings.Rows.Add(
                    "StringOfConnection",
                    String.Format("Server={0};Port={1};Database=Pharm66;User Id=root;Password=1;Character Set=cp1251;Persist Security Info=True",
                    ReadSettings.Rows.Find("ServerOfDataBase")["Value"],
                    ReadSettings.Rows.Find("Port")["Value"]));
            }
            //
            if (ReadSettings.Rows.Contains("ServerOfFTP"))
            {
                ReadSettings.Rows.Add(
                        "PathOfImportingData", 
                        String.Format(
                        "ftp://pharm:86921@{0}/ImportingData/", 
                        ReadSettings.Rows.Find("ServerOfFTP")["Value"]));
            }
            //
            // Return
            //
            return ReadSettings;
        }

        // Getting Settings
        private DataTable GettingSettings()
        {
            //
            DataTable ReturnSettings = new DataTable("Settings");
            //
            if (CurrentSettings.Rows.Count > 0)
            {
                ReturnSettings = CurrentSettings.Copy();
            }
            else
            {
                //
                // Reading Settings
                //
                DataTable ReadSettings = new DataTable("Settings");
                //
                ReadSettings = ReadingSettings();
                //
                // Checking Settings
                //
                bool SettingsSuccessful = false;
                //
                SettingsSuccessful = true;
                //
                // !!!
                //
                if (SettingsSuccessful)
                {
                    ReturnSettings = ReadSettings.Copy();
                }
                else
                {
                    //
                    // Creating Structure Of Settings
                    //
                    DataTable DefaulSettings = new DataTable("Settings");
                    DefaulSettings.Columns.Add("Key", typeof(string));
                    DefaulSettings.Columns.Add("Value", typeof(object));
                    DefaulSettings.PrimaryKey = new DataColumn[1] { DefaulSettings.Columns["Key"] };
                    //
                    // Filling Of 01
                    //
                    DefaulSettings.Rows.Add(
                        "StringOfConnection",
                        "Server=127.0.0.1;Port=3307;Database=Pharm66;User Id=root;Password=1;Character Set=cp1251;Persist Security Info=True");
                    DefaulSettings.Rows.Add(
                        "PathOfImportingData",
                        "ftp://pharm:86921@FTP.MEDINCOM.RU/ImportingData/");
                    DefaulSettings.Rows.Add("UsePassive", false);
                    //
                    // Filling Of 02
                    //
                    DefaulSettings.Rows.Add("PathToFolderOfPriceLists",
                        @"C:\Pharmaceutical Information\ServerOfTransferOfPriceLists\PriceLists\");
                    DefaulSettings.Rows.Add("MaskOfFullPriceList", "price");
                    DefaulSettings.Rows.Add("MaskOfReceptionOfPriceList", "prixod");
                    DefaulSettings.Rows.Add("MaskOfDefectionOfPriceList", "rasxod");
                    DefaulSettings.Rows.Add("IDOfPharmacy", IDOfPharmacy);
                    DefaulSettings.Rows.Add("NotDeletingPriceList", true);
                    DefaulSettings.Rows.Add("UseOfIDOfPriceList", true);
                    //
                    DefaulSettings.Rows.Add("IntervalOfExporting", 3);
                    //
                    ReturnSettings = DefaulSettings.Copy();
                    CurrentSettings = DefaulSettings.Copy();
                }
            }
            //
            // Return
            //
            return ReturnSettings;
        }

        #endregion

        #region ' Management Of Server '

        // Starting Of Server
        public void StartingOfServer()
        {
            //
            // Starting
            //
            //
            int IntervalOfExporting = (int)GettingSettings().Rows.Find("IntervalOfExporting")["Value"];
            //
            // Timer Of Transfer
            ///*
            TimerOfTransfer =
                new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfTransfer_Tick),
                    null, new TimeSpan(0, 3, 0), new TimeSpan(IntervalOfExporting, 0, 0));
            //*/
            /*
            TimerOfTransfer =
                new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfTransfer_Tick),
                    null, new TimeSpan(0, 3, 0), new TimeSpan(0, 20, 0));
            //*/
            //
            RecordingInLogFile("Starting Service");
            //
            //Transfer.SendingLog();
            //
            //RecordingInLogFile("Starting Service");
        }

        // Stoping Of Server
        public void StopingOfServer()
        {
            //
            // Stoping
            //
            TimerOfTransfer.Dispose();
            //
            RecordingInLogFile("Stoping Service");
            //
            Transfer.SendingLog();
            //
            //RecordingInLogFile("Stoping Service");
        }

        #endregion

        #region ' Transfer Of Data '

        // Timer Of Importing
        private void TimerOfTransfer_Tick(object sender)
        {
            //
            if (!InWork)
            {
                //
                InWork = true;
                //
                try
                { Transfer.ConvertingAndLoadingPriceList(false); }
                catch (Exception E)
                {
                    RecordingInLogFile(String.Format("ERROR Error At Converting: {0}", E.Message));
                    //
                    InWork = false;
                    //
                    Transfer.SendingLog();
                }
                //
                RecordingInLogFile("Converting Is Successful");
                //
                //InWork = false;
                //
                // Loading PriceList Is Completed
                //
                Management_LoadingPriceListIsCompleted1();
            }
        }

        // Loading PriceList Is Completed
        private void Management_LoadingPriceListIsCompleted1()
        {
            //
            try
            {
                Transfer.SendingPriceList(false);
                //
                RecordingInLogFile("Sending Is Successful");
            }
            catch (Exception E)
            {
                RecordingInLogFile(String.Format("ERROR Error At Sending: {0}", E.Message));
                //
                InWork = false;
                //
                Transfer.SendingLog();
            }
            //
            // Scraping Of Logs Text
            //
            //this.ScrapingOfLogFile();
            //
            // !!!
            //
            InWork = false;
        }

        // Loading PriceList Is Completed
        private void Management_LoadingPriceListIsCompleted(string MessageOfConverting, int A, int B)
        {
            //
            RecordingInLogFile("Management_LoadingPriceListIsCompleted");
        }

        #endregion

    }
}