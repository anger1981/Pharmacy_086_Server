using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PharmaceuticalInformation.BaseTypes;
using System.Xml;

namespace PharmaceuticalInformation.Server
{
    public class Server : BaseType
    {

        #region ' Fields '

        //
        // Transfer
        //
        private PharmaceuticalInformation.Server.ExchangeBetweenSystemAndQueues ExchangeFTPAndDataBase;
        private PharmaceuticalInformation.Server.SynchronizationOfDataOfDataBases SynchronizationOfData;
        //
        private string StringOfConnection;
        private string StringOfConnection02;
        //
        // Paths Of Archives
        //
        //private string PathOfArchivesOfPriceList;
        //private string PathOfArchivesOfLogFiles;
        private string PathOfArchivesOfImporting;
        //
        // Paths Of Importing
        //
        //private string PathOfImportingData;
        private string PathOfImportingData;
        //private string PathOfImportingManData;
        private string PathOfImportingOfIodineFace;
        //
        // Paths Of Exporting
        //
        private string PathOfExportingDataOfFTP;
        private string PathToExportingPriceListsOnFTP;
        private string PathOfExportingScriptsOfFTP;
        private string PathOfExportingUpdatingOneSite;
        //
        // Paths Of Processing
        //
        private string PathOfArchivingProgram;
        private string PathOfTMPFolder;
        //
        // Timers
        //
        private System.Threading.Timer TimerOfImportingData;
        //
        // Working
        //
        private bool InWork;
        private int HourOfLastExportingOfScripts;
        private int HourOfLastExportingOfData;
        private int HourOfNextPrivateImporting;
        //private int CountOfRepeatedEnteringInProcedure;
        private int CountOfIterationWithLastScraping;

        #endregion

        #region ' Designer '

        public Server()
        {
            //
            // !!!
            //
            InitializingOfFields();
        }

        // Initializing Of Fields
        private void InitializingOfFields()
        {
            //
            // Reading Settings
            //
            /*
            string PathToSettings = @"C:\Pharmaceutical Information\Server\Bin\Settings.xml";
            //
            System.Data.DataTable Settings = new System.Data.DataTable("Settings");
            Settings.ReadXml(PathToSettings);
            Settings.PrimaryKey = new System.Data.DataColumn[1] { Settings.Columns[0] };
            //
            string FTPServer = Settings.Rows.Find("FTP")["Value"].ToString();
            string DBServer = Settings.Rows.Find("DB")["Value"].ToString();
            */
            //
            // Initialize String Of Connection
            //

            XmlDocument doc = new XmlDocument();
            doc.Load("ServiceConfg.xml");

            StringOfConnection = doc.DocumentElement["StringOfConnection"].InnerText;

            StringOfConnection02 = doc.DocumentElement["StringOfConnection02"].InnerText;
           
            //
            // Initializing Settings Of Logs
            //
            string PathOfStarting = System.Windows.Forms.Application.StartupPath;
            //
            //ServerOfPharmInf
            this.PathToLogFile = PathOfStarting + "\\LogsOfServer.txt";
            //
            this.CountOfRowsInLogFile = 50000; //80000; //65000;900;//
            this.EnableScrapingLog = true;
            this.ShowingMessages = false;
            //
            //PathOfArchivesOfPriceList = Path.GetDirectoryName(PathOfStarting) + "\\ArchivesOfPriceList\\";
            //PathOfArchivesOfLogFiles = Path.GetDirectoryName(PathOfStarting) + "\\ArchivesOfLogFiles\\";
            PathOfArchivesOfImporting = Path.GetDirectoryName(PathOfStarting) + "\\ArchivesOfImporting\\";
            //                                                                   ArchivesOfUpdating
            PathOfTMPFolder = Path.GetDirectoryName(PathOfStarting) + "\\TMP\\";
            PathOfArchivingProgram = PathOfStarting + "\\Rar.exe";
            //
            // Access To Store Of Data
            //
            string AccessToStoreOfData = doc.DocumentElement["AccessToStoreOfData"].InnerText; ;
            //
            // Initializing Paths Of FTP
            //

            PathOfImportingData = doc.DocumentElement["PathOfImportingData"].InnerText;
            PathOfImportingOfIodineFace = doc.DocumentElement["PathOfImportingOfIodineFace"].InnerText;
            //
            // Initializing Path Of Exporting
            //
            PathOfExportingDataOfFTP = doc.DocumentElement["PathOfExportingDataOfFTP"].InnerText;
            PathToExportingPriceListsOnFTP = doc.DocumentElement["PathToExportingPriceListsOnFTP"].InnerText;
            PathOfExportingScriptsOfFTP = doc.DocumentElement["PathOfExportingScriptsOfFTP"].InnerText;
            PathOfExportingUpdatingOneSite = doc.DocumentElement["PathOfExportingUpdatingOneSite"].InnerText;

            //
            // Initialize ExchangeFTPAndSystemDataBase
            //
            ExchangeFTPAndDataBase = new 
                PharmaceuticalInformation.Server.ExchangeBetweenSystemAndQueues(StringOfConnection, this.PathToLogFile);
            //
            /*
            SynchronizationOfData = new
                PharmaceuticalInformation.Server.SynchronizationOfDataOfDataBases(StringOfConnection02, StringOfConnection, this.PathToLogFile);
            */
            //
            // Settings Of ExchangeFTPAndSystemDataBase
            //
            ExchangeFTPAndDataBase.AccessToStoreOfData = AccessToStoreOfData;
            //ExchangeFTPAndDataBase.PathOfImportingData = PathOfImportingData;
            //ExchangeFTPAndDataBase.PathToImportingManData = PathOfImportingManData;
            ExchangeFTPAndDataBase.PathOfImportingDataFromQueue = PathOfImportingData;
            ExchangeFTPAndDataBase.PathOfImportingOfIodineFace = PathOfImportingOfIodineFace;
            //
            ExchangeFTPAndDataBase.PathToExportingDataOfFTP = PathOfExportingDataOfFTP;
            ExchangeFTPAndDataBase.PathToExportingPriceListsOnFTP = PathToExportingPriceListsOnFTP;
            ExchangeFTPAndDataBase.PathOfExportingUpdatingAllSites = PathOfExportingScriptsOfFTP;
            ExchangeFTPAndDataBase.PathOfExportingUpdatingOneSite = PathOfExportingUpdatingOneSite;
            //
            ExchangeFTPAndDataBase.PathOfArchivingProgram = PathOfArchivingProgram;
            ExchangeFTPAndDataBase.PathOfTMPFolder = PathOfTMPFolder;
            //
            //ExchangeFTPAndDataBase.PathOfArchivesOfLogFiles = PathOfArchivesOfLogFiles;
            //ExchangeFTPAndDataBase.PathOfArchivesOfPriceList = PathOfArchivesOfPriceList;
            ExchangeFTPAndDataBase.PathOfArchivesOfImporting = PathOfArchivesOfImporting;
            //
            // Initializing Working
            //
            InWork = false;
            HourOfLastExportingOfScripts = DateTime.Now.Hour;
            HourOfLastExportingOfData = DateTime.Now.Hour;
            HourOfNextPrivateImporting = 0;
            //CountOfRepeatedEnteringInProcedure = 0;
            CountOfIterationWithLastScraping = 0;
        }

        #endregion

        #region ' Management Of Server '

        // Starting Of Server
        public void StartingOfServer()
        {
            //
            // Starting
            //
            // Timer Of Importing Data
            TimerOfImportingData = 
                new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfImportingData_Tick), 
                    null, new TimeSpan(0, 0, 0), new TimeSpan(0, 1, 0));
            //
            RecordingInLogFile("Starting Service");
            RecordingInLogFile("");
        }

        // Stoping Of Server
        public void StopingOfServer()
        {
            //
            // Stoping
            //
            TimerOfImportingData.Dispose();
            //
            RecordingInLogFile("Stoping Service");
            RecordingInLogFile("");
        }

        #endregion

        #region ' Transfer Of Data '

        // Timer Of Importing
        private void TimerOfImportingData_Tick(object sender)
        {
            //
            if (!InWork)
            {
                //
                // !!!
                //
                /*
                CountOfRepeatedEnteringInProcedure++;
                //
                if (CountOfRepeatedEnteringInProcedure == 10)
                {
                    this.RecordingInLogFile("Is Working");
                    this.RecordingInLogFile("");
                    CountOfRepeatedEnteringInProcedure = 0;
                }
                */
                //
                // !!!
                //
                CountOfIterationWithLastScraping++;
                //
                if (CountOfIterationWithLastScraping == 30)
                {
                    //
                    this.RecordingInLogFile("Scraping Of Logs");
                    this.RecordingInLogFile("");
                    //
                    ScrapingOfLogFile();
                    //
                    CountOfIterationWithLastScraping  = 0;
                }
                //
                // Blocking Enter To Procedure
                //
                InWork = true;
                //
                // Importing Modifications Of Help
                //
                /*
                try { SynchronizationOfData.ImportingModifications(); }
                catch (Exception E)
                {
                    RecordingInLogFile(
                        String.Format("ERROR Error Of Importing Modifications From SPR01: {0}", E.Message));
                }
                */
                //
                // Downloading Data From FTP
                //
                StringBuilder LogsOfDownloadingDataFromFTP = new StringBuilder();
                //
                try { ExchangeFTPAndDataBase.DownloadingDataFromFTP(LogsOfDownloadingDataFromFTP); }
                catch (Exception E)
                {
                    RecordingInLogFile(
                      String.Format("ERROR Error Of Downloading Data From FTP {0} : {1}",
                      LogsOfDownloadingDataFromFTP.ToString(), E.Message));
                }
                //
                // Downloading IodineFace From FTP
                //
                /*
                if ((DateTime.Now.Hour == 9) && (DateTime.Now.Minute >= 0) && (DateTime.Now.Minute <= 10))
                    try { ExchangeFTPAndDataBase.DownloadingIodineFaceFromFTP(); }
                    catch (Exception E)
                    {
                        RecordingInLogFile(
                          String.Format("ERROR Error Of Downloading PriceList Of Iodine: {0}", E.Message));
                    }
                */
                //
                // Downloading PriceLists Of Private Importings
                //
                int[] HoursOfPrivateImporting = new int[] { 3, 9, 15, 21 };
                bool ExecutingPrivateImporting = false;
                int CurrentHour = DateTime.Now.Hour;
                //
                if ((HourOfNextPrivateImporting == 0) || (CurrentHour == HourOfNextPrivateImporting))
                {
                    //
                    ExecutingPrivateImporting = true;
                    //
                    if (HourOfNextPrivateImporting == 0)
                        for (int i = CurrentHour; i <= 23; i++)
                        {
                            //
                            for (int i2 = 0; i2 < HoursOfPrivateImporting.Length; i2++)
                                if (i == HoursOfPrivateImporting[i2])
                                    HourOfNextPrivateImporting = i;
                            //
                            if (HourOfNextPrivateImporting > 0)
                                break;
                        }
                    //
                    if (HourOfNextPrivateImporting == 0)
                        HourOfNextPrivateImporting = HoursOfPrivateImporting[0];
                    //
                    for (int i = 0; i < HoursOfPrivateImporting.Length; i++)
                        if (CurrentHour == HoursOfPrivateImporting[i])
                            HourOfNextPrivateImporting =
                                (i == HoursOfPrivateImporting.GetUpperBound(0)) ?
                                HoursOfPrivateImporting[0] :
                                HoursOfPrivateImporting[i + 1];
                }
                //
                if(ExecutingPrivateImporting)
                    try { ExchangeFTPAndDataBase.DownloadingPriceListsOfPrivateImportings(); }
                    catch (Exception E)
                    {
                        RecordingInLogFile(
                          String.Format("ERROR Error Of Downloading PriceLists Of Private Importings: {0}", E.Message));
                    }
                //
                // Exporting Data Of Updating
                //
                /*
                try { SynchronizationOfData.ExportingDataOfUpdating(); }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Error Of Exporting Data To SPR01: {0}", E.Message)); }
                */
                //
                // Loading Of Data On FTP
                //
                if (HourOfLastExportingOfData != DateTime.Now.Hour)
                {
                    //
                    HourOfLastExportingOfData = DateTime.Now.Hour;
                    //
                    try
                    { ExchangeFTPAndDataBase.LoadingDataToFTP(); }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Error Of Loading Data To FTP: {0}", E.Message)); }
                }
                //
                // Loading Exported File Of PriceLists
                //
                ExchangeFTPAndDataBase.LoadingExportedFileOfPriceLists();
                //
                // Loading Updatings For Sites
                //
                //CountOfCheckingOfExporting++;
                if (HourOfLastExportingOfScripts != DateTime.Now.Hour)
                {
                    //
                    // Change Of Hour Of Exporting
                    //
                    HourOfLastExportingOfScripts = DateTime.Now.Hour;
                    //
                    // Loading Updating Of All Sites
                    //
                    try { ExchangeFTPAndDataBase.LoadingUpdatingOfAllSites(); }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Error Of Loading Scripts To FTP: {0}", E.Message)); }
                    //
                    // Sleep
                    //
                    System.Threading.Thread.Sleep(108);
                    //
                    // Loading Updating Of One Site
                    //
                    //ExchangeFTPAndDataBase.LoadingUpdatingOfOneSite();
                }
                //
                // Sanction Of Entering To Procedure
                //
                InWork = false;
            }
            //
        }

        #endregion

        #region ' Archive '

        //
        // !!!
        //
        //CountOfRepeatedEnteringInProcedure = 0;
        /*else
            {
                //
                //CountOfRepeatedEnteringInProcedure++;
                //
                // Checking Of Duration
                //
                //if (CountOfRepeatedEnteringInProcedure == 30)
                {
                    //
                    //RecordingInLogFile("Reset For Repeated Input");
                    //
                    // Reset Of Fields Of Control
                    //
                    //CountOfRepeatedEnteringInProcedure = 0;
                    //InWork = false;
                    //
                    // Restarting Server
                    //
                    //StopingOfServer();
                    //InitializingOfFields();
                    //StartingOfServer();
                }
            }*/
        //private bool[] Comp = new bool[1];
        //private System.Threading.Timer TimerOfExportingData;
        //private System.Threading.Timer TimerOfImportingOfIodineFace;
        //ExchangeFTPAndDataBase.LoadingCSVDataToFTP();
        //Comp[0] = false;
        //if (Comp[0])
        //Comp[0] = true;
        //
        // Loading Of CSV On FTP
        //
        //Comp[0] = true;
        // 1
        // Timer Of Exporting Data
        /*
        TimerOfExportingData =
            new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfExportingData_Tick),
                null, new TimeSpan(0, 0, 0), new TimeSpan(0, 3, 0));
        */
        // Timer Of Importing Of IodineFace
        /*
        TimerOfImportingOfIodineFace =
            new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfImportingOfIodineFace_Tick),
                null, new TimeSpan(0, 0, 0), new TimeSpan(0, 55, 0));
        */
        //TimerOfExportingData.Dispose();
        //TimerOfImportingOfIodineFace.Dispose();
        /*
        // Timer Of Exporting
        private void TimerOfExportingData_Tick(object sender)
        {
            //if (Comp[0])
            {
                //
                // Loading Of Data On FTP
                //
                ExchangeFTPAndDataBase.LoadingDataToFTP();
                //
                // Loading Of CSV On FTP
                //
                ExchangeFTPAndDataBase.LoadingCSVDataToFTP();
                //
                // Loading Of Scripts On FTP
                //
                CountOfCheckingOfExporting++;
                if (CountOfCheckingOfExporting == 60)
                {
                    CountOfCheckingOfExporting = 0;
                    ExchangeFTPAndDataBase.LoadingScriptsToFTP();
                }
            }
        }

        // Timer Of Importing Of IodineFace
        private void TimerOfImportingOfIodineFace_Tick(object sender)
        {
            //
            if ((DateTime.Now.Hour == 9) && (DateTime.Now.Minute >= 0) && (DateTime.Now.Minute <= 10))
            {
                //
                // Downloading IodineFace From FTP
                //
                //Comp[0] = false;
                ExchangeFTPAndDataBase.DownloadingIodineFaceFromFTP();
                //Comp[0] = true;
                //
            }
        }
        */

        #endregion

    }
}