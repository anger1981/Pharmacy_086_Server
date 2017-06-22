using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PharmaceuticalInformation.BaseTypes;
using System.Xml;
using ServerOfSystem.PharmaceuticalInformation.Infrastructure;
using ServerOfSystem.PharmaceuticalInformation.Interfaces;
using Ninject;

namespace PharmaceuticalInformation.Server
{
    public class Server : BaseType
    {

        #region ' Fields '

        //
        // Transfer
        //
        private PharmaceuticalInformation.Server.ExchangeBetweenSystemAndQueues ExchangeFTPAndDataBase;
        //
        private string StringOfConnection;
        private string StringOfConnection02;
        //
        // Paths Of Archives
        //
        private string PathOfArchivesOfImporting;
        //
        // Paths Of Importing
        //
        private string PathOfImportingData;
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
        private int CountOfIterationWithLastScraping;

        #endregion

        #region ' Designer '

        public Server()
        {
            //
            // !!!
            //

            InitializingOfFields();

            this.RecordingInLogFile("Server Init");
            this.RecordingInLogFile("");
        }

        // Initializing Of Fields
        private void InitializingOfFields()
        {
            //
            // Reading Settings
            //
            //
            // Initialize String Of Connection
            //


            XmlDocument doc = new XmlDocument();
            doc.Load(Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "\\ServiceConfg.xml");

            StringOfConnection = doc.DocumentElement["StringOfConnection"].InnerText;

            StringOfConnection02 = doc.DocumentElement["StringOfConnection02"].InnerText;

            NinjectDependencyResolver.AddBindings(StringOfConnection);

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
            PathOfArchivesOfImporting = Path.GetDirectoryName(PathOfStarting) + "\\ArchivesOfImporting\\";

            PathOfTMPFolder = Path.GetDirectoryName(PathOfStarting) + "\\TMP\\";
            PathOfArchivingProgram = PathOfStarting + "\\Rar.exe";
            //
            // Access To Store Of Data
            //
            string AccessToStoreOfData = doc.DocumentElement["AccessToStoreOfData"].InnerText;
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
                PharmaceuticalInformation.Server.ExchangeBetweenSystemAndQueues(NinjectDependencyResolver.kernel.Get<IPharmacyInformation>(), this.PathToLogFile);
            //
            /*
            SynchronizationOfData = new
                PharmaceuticalInformation.Server.SynchronizationOfDataOfDataBases(StringOfConnection02, StringOfConnection, this.PathToLogFile);
            */
            //
            // Settings Of ExchangeFTPAndSystemDataBase
            //
            ExchangeFTPAndDataBase.AccessToStoreOfData = AccessToStoreOfData;

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
            ExchangeFTPAndDataBase.PathOfArchivesOfImporting = PathOfArchivesOfImporting;
            //
            // Initializing Working
            //
            InWork = false;
            HourOfLastExportingOfScripts = DateTime.Now.Hour;
            HourOfLastExportingOfData = DateTime.Now.Hour;
            HourOfNextPrivateImporting = 0;

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
            this.RecordingInLogFile("TimerOfImportingData_Tick");
            this.RecordingInLogFile("");

            if (!InWork)
            {               
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

                this.RecordingInLogFile("InWork");
                this.RecordingInLogFile("");

                InWork = true;
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

    }
}