using System;
using System.Collections.Generic;
using System.Text;
using PharmaceuticalInformation.BaseTypes;
using System.IO;
using System.Data;
using System.Collections;

namespace PharmaceuticalInformation.Drugstore
{
    public class Drugstore : BaseType
    {

        #region ' Fields '

        private TypeOfWorking ModeOfWorking;
        //
        // Working Paths
        //
        private string PathToWorkingDirectory;
        //private string PathToLogFile;
        private string PathToProgramOfArchiving;
        private string PathToSettingsFile;
        private string PathToTMPFolder;
        //
        // Working Modules Of Module Of Drugstore
        //
        private PharmaceuticalInformation.Drugstore.ManagementOfSettings ManagementOfSettings;
        private bool InitializedOfManagementOfSettings = false;
        //
        private PharmaceuticalInformation.Drugstore.ExchangeFTPAndLocalDataBase ExchangeFTPAndLocalDataBase;
        private bool InitializedOfExchangeFTPAndLocalDataBase = false;
        //
        private PharmaceuticalInformation.Drugstore.IntegrityOfData IntegrityOfData;
        private bool InitializedOfIntegrityOfData = false;
        //
        private PharmaceuticalInformation.Drugstore.ConvertingOfPriceList ConvertingOfPriceList;
        private bool InitializedOfConvertingOfPriceList = false;
        //
        private PharmaceuticalInformation.Drugstore.ImportingPricesInPriceList ImportingPricesInPriceList;
        private bool InitializedOfImportingPricesInPriceList = false;
        //
        private PharmaceuticalInformation.Drugstore.ManagementOfPriceList ManagementOfPriceList;
        private bool InitializedOfManagementOfPriceList = false;
        //
        private PharmaceuticalInformation.Drugstore.ManagementOfComparisons ManagementOfComparisons;
        private bool InitializedOfManagementOfComparisons = false;
        //
        private PharmaceuticalInformation.Reading.ReadingOfDataForMySQL ReadingOfData;
        private bool InitializedOfReadingOfData = false;
        //
        private PharmaceuticalInformation.Updating.UpdatingOfDataOfInformationForMySQL UpdatingOfData;
        private bool InitializedOfUpdatingOfData = false;
        //
        // Timers
        //
        private System.Threading.Timer TimerOfImportingData;
        private System.Threading.Timer TimerOfUpdatingData;
        //
        // Threadings
        //
        private System.Threading.Thread ThreadOfImportingPriceLists;
        private bool ExecutingThreadOfImportingPriceLists;
        //
        private System.Threading.Thread ThreadOfUpdatingOfData;
        private bool ExecutingThreadOfUpdating;
        //
        // Events
        //
        public delegate void ReturningEvents(ItemOfExecuting[] HistoryOfExecuting);
        //
        public event ReturningEvents ImportingPriceListsIsCompiled;
        public event ReturningEvents UpdatingOfDataIsCompiled;
        //
        // !!!
        //
        private int IDOfSendingData;

        #endregion

        #region ' Designer '

        public Drugstore(string PathToWorkingDirectory, TypeOfWorking ModeOfWorking)
        {
            //PathToWorkingDirectory + "\\Drugstore.txt"
            //
            // Initializing Fields
            //

            this.PathToWorkingDirectory = PathToWorkingDirectory;
            this.ModeOfWorking = ModeOfWorking;
            this.IDOfSendingData = 0;
            //
            // Initializing Working Paths
            //
            if (!this.PathToWorkingDirectory.EndsWith("\\"))
                this.PathToWorkingDirectory += "\\";
            //
            base.PathToLogFile = this.PathToWorkingDirectory + "Drugstore.txt";
            PathToProgramOfArchiving = this.PathToWorkingDirectory + "Rar.exe";
            PathToSettingsFile = this.PathToWorkingDirectory + "Settings.bin";
            PathToTMPFolder = this.PathToWorkingDirectory + "TMP\\";
            //
            // Scraping Of Log File
            //
            this.CountOfRowsInLogFile = 5000;
            this.EnableScrapingLog = true;
            //
            this.RecordingInLogFile("Scraping Of Log File");
            this.RecordingInLogFile("");
            //
            this.EnableScrapingLog = false;
            //
            // Creating TMP Directory If He Not Exists
            //
            if (!Directory.Exists(PathToTMPFolder))
            {
                try { Directory.CreateDirectory(PathToTMPFolder); }
                catch (Exception E)
                {
                    //
                    //this.ReturningMessageAboutError("Ошибка при создании каталога TMP", E, true);
                    this.RecordingInLogFile(String.Format("Ошибка при создании каталога TMP: {0}", E.Message));
                }
            }
            //
            // Initializing Of Modules
            //
            InitializingOfModules(TypeOfInitialization.FullInitialization, this.ModeOfWorking);
        }

        #endregion

        #region ' Initializing Modules ' 

        // ReInitializing Of Modules
        public void ReInitializingOfModules()
        {
            //
            // Initialization Of Modules (ReducedInitialization)
            //
            InitializingOfModules(TypeOfInitialization.ReducedInitialization, this.ModeOfWorking);
        }

        // Initialization Of Modules
        private void InitializingOfModules(TypeOfInitialization ModeOfInitialization, TypeOfWorking ModeOfWorking)
        {
            //
            // Recording In Reportings
            //
            this.RecordingInLogFile("Starting Initializing Of Modules");
            this.RecordingInLogFile("");
            //
            // Initializing Of Management Of Settings
            //
            if (ModeOfInitialization == TypeOfInitialization.FullInitialization)
            {
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
            }
            //
            // Getting ID Of First Registration Drugstore
            //
            if (ManagementOfSettingsIsWorking)
            {
                //
                // Getting ID Of Registration Drugstore
                //
                if (ManagementOfSettings.GettingRegistrationOfDrugstores().Rows.Count > 0)
                {
                    IDOfSendingData = 
                        Convert.ToInt32(ManagementOfSettings.GettingRegistrationOfDrugstores().Rows[0]["ID"]);
                }
                else
                    IDOfSendingData = 0;
            }
            //
            // Initializing Of Converting Of PriceList
            //
            if (ModeOfInitialization == TypeOfInitialization.FullInitialization)
            {
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
            }
            //
            // Initializing Of Exchange FTP And Local DataBase
            //
            if (ModeOfInitialization == TypeOfInitialization.FullInitialization)
            {
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
            }
            //
            // Settings Of Exchange FTP And LocalDataBase
            //
            if (ManagementOfSettingsIsWorking && ExchangeFTPAndLocalDataBaseIsWorking)
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
            // Checking
            //
            // Checking Of Connection
            //
            bool ConnectionOfIsChecked = this.CheckingOfConnection();
            //
            // Checking Of Communication
            //
            //bool CommunicationOfIsChecked = this.CheckingOfCommunication();
            //
            // Initializing Of Integrity Of Data
            //
            //if (ModeOfInitialization == TypeOfInitialization.FullInitialization) { }
            //
            if (ConnectionOfIsChecked)
            {
                try
                {
                    //
                    IntegrityOfData = 
                        new PharmaceuticalInformation.Drugstore.IntegrityOfData(
                            ManagementOfSettings.GettingStringOfConnection, PathToLogFile);
                    //
                    InitializedOfIntegrityOfData = true;
                    //
                    // Modification Of Schema (Correction Of DataBase)
                    //
                    this.RecordingInLogFile("Starting Correction");
                    //
                    try { IntegrityOfData.CorrectionOfDataBase(); }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при модернизации структуры БД: {0}", E.Message));
                    }
                    //
                    this.RecordingInLogFile("Stoping Correction");
                    this.RecordingInLogFile("");
                }
                catch (Exception E)
                {
                    //
                    InitializedOfIntegrityOfData = false;
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при инициализации модуля IntegrityOfData: {0}", E.Message));
                }
            }
            else
            {
                //
                IntegrityOfData = null;
                //
                InitializedOfIntegrityOfData = false;
                //
                this.RecordingInLogFile("INFOR Модуль IntegrityOfData Не проинициализирован");
            }
            //
            // Initializing Of Reading Of Data For MySQL
            //
            if (ModeOfWorking == TypeOfWorking.AllFunctions)
            {
                //
                if (ConnectionOfIsChecked)
                {
                    try
                    {
                        //
                        ReadingOfData = 
                            new PharmaceuticalInformation.Reading.ReadingOfDataForMySQL(
                                ManagementOfSettings.GettingStringOfConnection, 
                                PathToWorkingDirectory, PathToLogFile);
                        //
                        InitializedOfReadingOfData = true;
                    }
                    catch (Exception E)
                    {
                        //
                        InitializedOfReadingOfData = false;
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при инициализации модуля ReadingOfData: {0}", E.Message));
                    }
                }
                else
                {
                    //
                    ReadingOfData = null;
                    //
                    InitializedOfReadingOfData = false;
                    //
                    this.RecordingInLogFile("INFOR Модуль ReadingOfData Не проинициализирован");
                }
            }
            //
            // Initializing Of Updating Of Data Of Information For MySQL
            //
            if (ConnectionOfIsChecked)
            {
                try
                {
                    //
                    UpdatingOfData = 
                        new PharmaceuticalInformation.Updating.UpdatingOfDataOfInformationForMySQL(
                            ManagementOfSettings.GettingStringOfConnection, PathToLogFile);
                    //
                    InitializedOfUpdatingOfData = true;
                }
                catch (Exception E)
                {
                    //
                    InitializedOfUpdatingOfData = false;
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при инициализации модуля UpdatingOfData: {0}", E.Message));
                }
            }
            else
            {
                //
                UpdatingOfData = null;
                //
                InitializedOfUpdatingOfData = false;
                //
                this.RecordingInLogFile("INFOR Модуль UpdatingOfData Не проинициализирован");
            }
            //
            // Initializing Of Importing Prices In PriceList
            //
            try
            {
                //
                ImportingPricesInPriceList = 
                    new PharmaceuticalInformation.Drugstore.ImportingPricesInPriceList(
                        ManagementOfSettings.GettingStringOfConnection, ManagementOfComparisons, PathToLogFile);
                //
                InitializedOfImportingPricesInPriceList = true;
            }
            catch (Exception E)
            {
                //
                InitializedOfImportingPricesInPriceList = false;
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при инициализации модуля ImportingPricesInPriceList: {0}", E.Message));
            }
            //
            // Initializing Of Management Of Comparisons
            //
            if ((ConnectionOfIsChecked) && (!this.ConvertingOfPriceList.UseOfIDOfPriceList))
            {
                try
                {
                    //
                    ManagementOfComparisons =
                        new PharmaceuticalInformation.Drugstore.ManagementOfComparisons(
                            ManagementOfSettings.GettingStringOfConnection, PathToLogFile);
                    //
                    InitializedOfManagementOfComparisons = true;
                }
                catch (Exception E)
                {
                    //
                    InitializedOfManagementOfComparisons = false;
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при инициализации модуля ManagementOfComparisons: {0}", E.Message));
                }
            }
            else
            {
                //
                ManagementOfComparisons = null;
                //
                InitializedOfManagementOfComparisons = false;
                //
                this.RecordingInLogFile("INFOR Модуль ManagementOfComparisons Не проинициализирован");
            }
            //
            // Initializing Of Management Of PriceList
            //
            if (ModeOfWorking == TypeOfWorking.AllFunctions)
            {
                //
                if (ConnectionOfIsChecked)
                {
                    try
                    {
                        //
                        ManagementOfPriceList = 
                            new PharmaceuticalInformation.Drugstore.ManagementOfPriceList(
                                ManagementOfSettings.GettingStringOfConnection, ReadingOfData, PathToLogFile);
                        //
                        /*PharmaceuticalInformation.Drugstore.ManagementOfPriceList.ReturnOfEvent ReturnOfPriceList = 
                            new PharmaceuticalInformation.Drugstore.ManagementOfPriceList.ReturnOfEvent(ImportingPriceListIsCompleted);*/
                        //
                        //ManagementOfPriceList.LoadingPriceListIsCompleted += ReturnOfPriceList;
                        ManagementOfPriceList.ManagementOfComparisons = ManagementOfComparisons;
                        //
                        InitializedOfManagementOfPriceList = true;
                    }
                    catch (Exception E)
                    {
                        //
                        InitializedOfManagementOfPriceList = false;
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при инициализации модуля ManagementOfPriceList: {0}", E.Message));
                    }
                }
                else
                {
                    //
                    ManagementOfPriceList = null;
                    //
                    InitializedOfManagementOfPriceList = false;
                    //
                    this.RecordingInLogFile("INFOR Модуль ManagementOfPriceList Не проинициализирован");
                }
            }
            //
            // Recording In Reportings
            //
            this.RecordingInLogFile("Stoping Initializing Of Modules");
            this.RecordingInLogFile("");
            //
            // Starting Timers
            //
            // Timer Of Importing Data
            //
            int IntervalOfImporting = ManagementOfSettings.IntervalOfImporting;
            //
            /*
            if (IntervalOfImporting > 0)
            {
                TimerOfImportingData = 
                    new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfImportingData_Tick), 
                        null, new TimeSpan(0, 0, 5), new TimeSpan(IntervalOfImporting, 0, 0));
            }
            */
            //
            // Timer Of Updating Data
            //
            int IntervalOfUpdating = ManagementOfSettings.IntervalOfUpdating;
            //
            if (IntervalOfUpdating > 0)
            {
                //
                IntervalOfUpdating = IntervalOfUpdating / 1000;
                IntervalOfUpdating = IntervalOfUpdating / 60;
                //
                TimerOfUpdatingData = 
                    new System.Threading.Timer(new System.Threading.TimerCallback(TimerOfUpdatingData_Tick),
                        null, new TimeSpan(0, IntervalOfUpdating, 0), new TimeSpan(0, IntervalOfUpdating, 0));
            }
        }

        // Refreshing ID Of Sending
        public void RefreshingIDOfSending()
        {
            //
            // Getting ID Of First Registration Drugstore
            //
            if (ManagementOfSettingsIsWorking)
            {
                //
                // Getting ID Of Registration Drugstore
                //
                if (ManagementOfSettings.GettingRegistrationOfDrugstores().Rows.Count > 0)
                {
                    IDOfSendingData =
                        Convert.ToInt32(ManagementOfSettings.GettingRegistrationOfDrugstores().Rows[0]["ID"]);
                }
                else
                    IDOfSendingData = 0;
            }
        }

        // Initializing Of Management Of Comparisons
        /*public void InitializingOfManagementOfComparisons()
        {
            //
            if (this.ModeOfWorking == TypeOfWorking.AllFunctions)
            {
                //
                if (this.CheckingOfConnection())
                {
                    try
                    {
                        //
                        ManagementOfComparisons = 
                            new PharmaceuticalInformation.Drugstore.ManagementOfComparisons(
                                ManagementOfSettings.GettingStringOfConnection, PathToLogFile);
                        //
                        InitializedOfManagementOfComparisons = true;
                    }
                    catch (Exception E)
                    {
                        //
                        InitializedOfManagementOfComparisons = false;
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при инициализации модуля ManagementOfComparisons: {0}", E.Message));
                    }
                }
                else
                {
                    //
                    ManagementOfComparisons = null;
                    //
                    InitializedOfManagementOfComparisons = false;
                    //
                    this.RecordingInLogFile("INFOR Модуль ManagementOfComparisons Не проинициализирован");
                }
            }
        }*/

        #endregion

        #region ' Timers '

        private void TimerOfImportingData_Tick(object sender)
        {
            //
            // Starting Importing PriceLists
            //
            StartingImportingPriceLists(IDOfSendingData, true);
        }

        private void TimerOfUpdatingData_Tick(object sender)
        {
            //
            // Starting Updating OfData
            //
            StartingUpdatingOfData();
        }

        #endregion

        #region ' Checking '

        // Checking Is Registration
        public bool CheckingIsRegistration()
        {
            //
            // Checking
            //
            bool Checked = false;
            //
            if (ManagementOfSettingsIsWorking)
            {
                //
                DataTable ListOfRegistratedDrugstores = ManagementOfSettings.GettingRegistrationOfDrugstores();
                //
                int IDOfDrugstore = 0;
                //
                if ((ListOfRegistratedDrugstores.Rows.Count > 0) && 
                    (ListOfRegistratedDrugstores.Columns.Contains("ID")))
                    IDOfDrugstore = ((int)ListOfRegistratedDrugstores.Rows[0]["ID"]);
                //
                if (IDOfDrugstore > 0)
                    Checked = true;
            }
            //
            // Return
            //
            return Checked;
        }

        // Checking Existing In List Of DataBase
        public bool CheckingExistingInListOfDataBase()
        {
            //
            // Checking
            //
            bool Checked = false;
            //
            if (ManagementOfSettingsIsWorking && ReadingOfDataIsWorking)
            {
                //
                DataTable ListOfRegistratedDrugstores = ManagementOfSettings.GettingRegistrationOfDrugstores();
                //
                int IDOfDrugstore = 0;
                //
                if ((ListOfRegistratedDrugstores.Rows.Count > 0) && 
                    (ListOfRegistratedDrugstores.Columns.Contains("ID")))
                    IDOfDrugstore = ((int)ListOfRegistratedDrugstores.Rows[0]["ID"]);
                //
                DataTable DetailsOfDrugstore = new DataTable("DetailsOfDrugstore");
                //
                if (IDOfDrugstore > 0)
                { DetailsOfDrugstore = ReadingOfData.GettingDetailsOfDrugstore(IDOfSendingData); }
                //
                if (DetailsOfDrugstore != null)
                    if (DetailsOfDrugstore.Rows.Count > 0)
                        Checked = true;
            }
            //
            // Return
            //
            return Checked;
        }

        // Checking Of Connection
        public bool CheckingOfConnection()
        {
            //
            // Checking
            //
            bool Checked = false;
            //
            if(ManagementOfSettingsIsWorking)
                Checked = ManagementOfSettings.CheckingOfConnection();
            //
            // Return
            //
            return Checked;
        }

        // Checking Of Communication
        public bool CheckingOfCommunication()
        {
            //
            // Checking
            //
            bool Checked = false;
            //
            if (ManagementOfSettingsIsWorking)
                Checked = ManagementOfSettings.CheckingOfCommunication();
            //
            // Return
            //
            return Checked;
        }

        #endregion

        #region ' Importing PriceLists (Converting Of Data) '

        // Converting And Loading PriceList
        public bool ConvertingAndLoadingPriceList(int IDOfDrugstore, bool ShowingMessage)
        {
            //
            // Checking Working  Modules 
            //
            if(!(ManagementOfPriceListIsWorking && ConvertingOfPriceListIsWorking))
                return false;
            //
            string PathToFolderOfPriceLists = "";
            //
            // !!!
            //
            if (IDOfDrugstore != 0)
            {
                //
                PathToFolderOfPriceLists = ManagementOfSettings.GettingPathToFolderOfPriceLists(IDOfDrugstore);
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



            #region 'reset ManagementOfComparisons in ManagementOfPriceList if UseOfIDOfPriceList'

            if (this.ConvertingOfPriceList.UseOfIDOfPriceList)
                ManagementOfPriceList.ManagementOfComparisons = null;
            else ManagementOfPriceList.ManagementOfComparisons = this.ManagementOfComparisons;

            #endregion

            //
            // !!!
            //
            bool SuccessfulConverting = true;
            //
            // Recording In Reports
            //
            /*
            RecordingInLogFile(
                String.Format(
                "Загрузка файлов Прайс-Листов из директории: {0}",
                ConvertingOfPriceList.PathToFolderOfPriceLists));
            */
            RecordingInLogFile(
                String.Format("Загрузка файлов Прайс-Листов из директории: {0}", PathToFolderOfPriceLists));
            //
            // Checking Of Existing Of Directory Of PriceLists
            //
            //bool ExistedOfDirectory = Directory.Exists(ConvertingOfPriceList.PathToFolderOfPriceLists);
            bool ExistedOfDirectory = Directory.Exists(PathToFolderOfPriceLists);
            //
            // Recording In Reports And Cancel Convertings
            //
            if (!ExistedOfDirectory)
            {
                //
                /*
                ReturningMessageAboutError(
                    String.Format(
                    "Директория Прайс-Листов '{0}' не существует",
                    ConvertingOfPriceList.PathToFolderOfPriceLists),
                    new Exception("Exists False"), false);
                */
                ReturningMessageAboutError(
                    String.Format(
                    "Директория Прайс-Листов '{0}' не существует", PathToFolderOfPriceLists), 
                    new Exception("Exists False"), false);

                //
                SuccessfulConverting = false;
            }
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
                //string[] FilesOfDirectory = Directory.GetFiles(ConvertingOfPriceList.PathToFolderOfPriceLists);
                string[] FilesOfDirectory = Directory.GetFiles(PathToFolderOfPriceLists);
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
                    ConvertingOfPriceList.PathToFolderOfPriceLists));
                */
                ReturningMessageOfInformation(
                    String.Format(
                    "В директории '{0}' НЕТ файлов Прайс-Листов", PathToFolderOfPriceLists));
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
                ManagementOfPriceList.ResetDataForImportingInPriceList();
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
                        PriceListFromFile = ConvertingOfPriceList.Converting(CurrentFileOfPriceList[0].ToString());
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
                                ManagementOfPriceList.AdditionOfPriceListForImporting(PriceListFromFile);
                                //
                                // Change Of Status Of Importing PriceList
                                //
                                if (ManagementOfPriceList.StatusOfImporting != 
                                    ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                                    if ((bool)PriceListFromFile.Rows[0]["AllPrices"])
                                    {
                                        ManagementOfPriceList.StatusOfImporting = 
                                            ManagementOfPriceList.StatusOfPriceList.FullPriceList;
                                    }
                                //
                                Console.WriteLine("S {0}", ManagementOfPriceList.GettingStatusOfPriceList.ToString());
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
                ManagementOfPriceList.MessageOfConverting = TextOfMessage;
            }
            //
            // Importing PriceList In DataBase
            //
            if ((CountOfConvertingRows > 0) && (ManagementOfPriceList.GettingCountForImporting > 0))
            {
                new System.Threading.Thread(
                    new System.Threading.ThreadStart(ManagementOfPriceList.ImportingPriceListInDataBase)).Start();
            }
            else { SuccessfulConverting = false; }
            //
            // Return
            //
            return SuccessfulConverting;
        }

        /*public void ImportingPriceListIsCompleted(string MessageOfConverting, int CountOfImportedPrices, int CountOfRepeatings)
        {
            //
            // Loading PriceList Is Compiled
            //
            //LoadingPriceListIsCompiled(MessageOfConverting);
        }*/

        private int IDOfDrugstoreOfImportingOfPriceLists = 0;
        private bool AutoSendingImportedPriceList = false;

        // Starting Importing PriceLists
        public void StartingImportingPriceLists(int IDOfDrugstore, bool AutoSendingPriceList)
        {
            //
            // Initializing And Starting Importing
            //
            if (ExecutingThreadOfImportingPriceLists == false)
            {
                //
                IDOfDrugstoreOfImportingOfPriceLists = IDOfDrugstore;
                AutoSendingImportedPriceList = AutoSendingPriceList;
                //
                ThreadOfImportingPriceLists = 
                    new System.Threading.Thread(new System.Threading.ThreadStart(ImportingPriceLists));
                //
                ThreadOfImportingPriceLists.Start();
                ExecutingThreadOfImportingPriceLists = true;
            }
        }

        // Stoping Importing PriceLists
        public void StopingImportingPriceLists()
        {
            //
            // Aborting Importing Of Data
            //
            if (ExecutingThreadOfImportingPriceLists && (ThreadOfImportingPriceLists != null))
            {
                ThreadOfImportingPriceLists.Abort();
                ExecutingThreadOfImportingPriceLists = false;
            }
        }

        // Getting Executing Thread Of Importing
        public bool GettingExecutingThreadOfImporting
        {
            get { return ExecutingThreadOfImportingPriceLists; }
        }

        private bool SanctionOfImportingConvertedPriceList = true;
        // Importing PriceLists
        private void ImportingPriceLists()
        {
            //
            // Initializing Of Fields
            //
            int IDOfDrugstore = IDOfDrugstoreOfImportingOfPriceLists;
            bool AutoSendingPriceList = AutoSendingImportedPriceList;
            //
            // Sanction On Executing Of Importing
            //
            bool SanctionOnExecutingOfImporting = true;
            //
            // Checking Working Necessary Modules
            //
            if (!(ManagementOfSettingsIsWorking && 
                ConvertingOfPriceListIsWorking && 
                ImportingPricesInPriceListIsWorking))
            {
                //
                SanctionOnExecutingOfImporting = false;
                //
                // Recording In Reports
                //
                RecordingInLogFile(
                    String.Format("ERROR Нет модулей для импортирования: {0}", 
                    String.Concat(
                    ((ManagementOfSettingsIsWorking) ? "" : "ManagementOfSettings "), 
                    ((ConvertingOfPriceListIsWorking) ? "" : "ConvertingOfPriceList "), 
                    ((ImportingPricesInPriceListIsWorking) ? "" : "ImportingPricesInPriceList "))));
            }
            //
            // Getting Path To PriceLists AND Settings Of ConvertingOfPriceList
            //
            string PathToFolderOfPriceLists = "";
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                if (IDOfDrugstore != 0)
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
            //ArrayList ListOfFilesOfPriceLists = new ArrayList();
            DataTable ListOfFilesOfPriceLists = new DataTable("ListOfFilesOfPriceLists");
            //
            //ListOfFilesOfPriceLists.Columns.Add("ID", typeof(int));
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
                        //ArrayList DataOfFileOfPriceList = new ArrayList();
                        //
                        //DataOfFileOfPriceList.Add(CurrentFile);
                        //DataOfFileOfPriceList.Add(DateOfFileOfPriceList);
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
                ManagementOfPriceList.ResetDataForImportingInPriceList();
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
                        //CountOfConvertingRows += PriceListFromFile.Rows.Count;
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
                                ManagementOfPriceList.AdditionOfPriceListForImporting(PriceListFromFile);
                                //
                                // Change Of Status Of Importing PriceList
                                //
                                if (ImportingPricesInPriceList.StatusOfImporting != 
                                    ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                                    if ((bool)PriceListFromFile.Rows[0]["AllPrices"])
                                    {
                                        ImportingPricesInPriceList.StatusOfImporting = 
                                            ManagementOfPriceList.StatusOfPriceList.FullPriceList;
                                    }
                            }
                            catch (Exception E)
                            {
                                RecordingInLogFile(
                                    String.Format("Ошибка при добавлении данных файла Прайс-листа '{0}': {1}", 
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
                            "Ошибка при конвертации файла Прайс-Листа '{0}': {1}", 
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
            // Creating Of Message Of Result Of Converting
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                string TextOfMessage = 
                    String.Format(
                    "Всего загружено файлов Прайс-Листов {0}\n", ListOfFilesOfPriceLists.Rows.Count);
                //
                foreach (DataRow CurrentFileOfPriceList in ListOfFilesOfPriceLists.Rows)
                {
                    TextOfMessage += 
                        String.Format("Файл: {0}, От {1}, Строк: {2}{3}\n", 
                        Path.GetFileName(CurrentFileOfPriceList["PathToFile"].ToString()), 
                        CurrentFileOfPriceList["DateOfFile"].ToString(), 
                        CurrentFileOfPriceList["CountOfRows"].ToString(), 
                        ((bool)CurrentFileOfPriceList["ErrorConverting"]) ? ", Ошибка" : "");
                }
            }
            //
            // Checking Of Count Of Converted Rows
            //
            if (SanctionOnExecutingOfImporting)
            {
                //
                int CountOfConvertedRows = 0;
                //
                foreach (DataRow CurrentFile in ListOfFilesOfPriceLists.Rows)
                { CountOfConvertedRows = CountOfConvertedRows + ((int)CurrentFile["CountOfRows"]); }
                //
                if (CountOfConvertedRows == 0)
                    SanctionOnExecutingOfImporting = false;
            }
            //
            // Importing PriceList In DataBase
            //
            if (SanctionOnExecutingOfImporting && (ImportingPricesInPriceList.GettingCountForImporting > 0))
            {
                //
                if (SanctionOfImportingConvertedPriceList)
                {
                    /*
                    new System.Threading.Thread(
                        new System.Threading.ThreadStart(ImportingPricesInPriceList.ImportingPriceListInDataBase)).Start();
                    */
                    //
                    ImportingPricesInPriceList.ImportingPriceListInDataBase();
                }
            }
            //
            // Waiting Of End Of Importing
            //
            while (ImportingPricesInPriceList.GettingOfExecutingImporting)
            { System.Threading.Thread.Sleep(500); }
            //
            // Auto Sending PriceList
            //
            if (AutoSendingPriceList)
            {
                //
                // SendingPriceList
                //
                SendingPriceList(false); // AdditionOfDataOfDatesOfTransfer ? AdditionOfDataOfPriceList ?
            }
            //
            // Loading PriceList Is Compiled
            //
            // Возвращает сообщение об итогах конвертации
            //
            ItemOfExecuting[] HistoryOfExecuting = new ItemOfExecuting[0];
            //
            ImportingPriceListsIsCompiled(HistoryOfExecuting);
        }

        #endregion

        #region ' Updating Of Data '

        // Starting Updating Data
        public void StartingUpdatingOfData()
        {
            //
            // Initializing And Starting Updating
            //
            if (ExecutingThreadOfUpdating == false)
            {
                //
                ThreadOfUpdatingOfData = 
                    new System.Threading.Thread(new System.Threading.ThreadStart(UpdatingData));
                //
                ThreadOfUpdatingOfData.Start();
                ExecutingThreadOfUpdating = true;
            }
        }

        // Stoping Updating Of Data
        public void StopingUpdatingOfData()
        {
            //
            // Aborting Updating Of Data
            //
            if (ExecutingThreadOfUpdating && (ThreadOfUpdatingOfData != null))
            {
                ThreadOfUpdatingOfData.Abort();
                ExecutingThreadOfUpdating = false;
            }
        }

        // Updating Data And Scheme
        private void UpdatingData()
        {
            //
            // Checking Of Communication
            //
            bool CheckedCommunication = this.CheckingOfCommunication();
            //
            // Updating And Sending
            //
            if (CheckedCommunication)
            {
                //
                // Updating Data 
                //
                if (ExchangeFTPAndLocalDataBaseIsWorking && UpdatingOfDataIsWorking)
                {
                    //
                    // Updating Data 02
                    //
                    ExchangeFTPAndLocalDataBase.UpdatingData02(UpdatingOfData);
                    //
                    // Update Of Buffer Of Products
                    //
                    if (ReadingOfDataIsWorking)
                    { ReadingOfData.UpdateOfBufferOfProducts(); }
                }
                else
                { this.RecordingInLogFile("INFOR Нет Инициализированных модулей для обновления данных"); }
                //
                // Sending Log
                //
                SendingLog();
            }
            else
            {
                //
                this.RecordingInLogFile("INFOR Нет связи с FTP Сервером, Обновление данных невозможно");
                this.RecordingInLogFile("");
            }
            //
            // Updating Of Data Is Compiled
            //
            ExecutingThreadOfUpdating = false;
            //
            ItemOfExecuting[] HistoryOfExecuting = new ItemOfExecuting[0];
            //
            UpdatingOfDataIsCompiled(HistoryOfExecuting);
        }

        // Getting Executing Thread Of Updating
        public bool GettingExecutingThreadOfUpdating
        {
            get { return ExecutingThreadOfUpdating; }
        } 

        #endregion

        #region ' Service '

        // Creating Label On Desktop
        public void CreatingLabelOnDesktop()
        {
            //
            //C:
            //Documents and Settings
            //kaa
            //Local Settings
            //Apps
            //2.0
            //6L02R4QN.RGY
            //0GLWB9O9.Q1C
            //
            //
            //C:
            //Users
            //МеD МаГ
            //AppData
            //Local
            //Apps
            //2.0
            //RZ5KBZGK.VL3
            //R5RORY28.R5Z
            //med_..tion_0f5c9933a3db4437_0001.0006_81f47c78ba45b1d1
            //
            if (CheckingOfNetwork())
            {
                //
                // Generation Of Path Of Creating Label
                //
                string PathOfCreatingLabel = "";
                //
                try
                {
                    //
                    string[] FoldersOfPath = PathToWorkingDirectory.Split('\\');
                    //
                    if (PathToWorkingDirectory.ToUpper().IndexOf("Documents and Settings".ToUpper()) > -1)
                    {
                        //
                        if (FoldersOfPath.Length >= 3)
                        {
                            //
                            string PathToDesktop = String.Join("\\", FoldersOfPath, 0, 3);
                            //
                            PathToDesktop += "\\Рабочий стол";
                            //
                            if (System.IO.Directory.Exists(PathToDesktop))
                            { PathOfCreatingLabel = PathToDesktop + "\\Справка 086.appref-ms"; }
                            else
                                this.RecordingInLogFile(
                                     String.Format("ERROR Нет папки рабочего стола: {0}", PathToDesktop));
                        }
                        else
                            this.RecordingInLogFile(
                                String.Format("ERROR Слишком короткий путь: {0}", FoldersOfPath.Length));
                    }
                    else if (PathToWorkingDirectory.ToUpper().IndexOf("Users".ToUpper()) > -1)
                    {
                        //
                        if (FoldersOfPath.Length >= 3)
                        {
                            //
                            string PathToDesktop = String.Join("\\", FoldersOfPath, 0, 3);
                            //
                            PathToDesktop += "\\Рабочий стол";
                            //
                            if (System.IO.Directory.Exists(PathToDesktop))
                            { PathOfCreatingLabel = PathToDesktop + "\\Справка 086.appref-ms"; }
                            else
                                this.RecordingInLogFile(
                                     String.Format("ERROR Нет папки рабочего стола: {0}", PathToDesktop));
                        }
                        else
                            this.RecordingInLogFile(
                                String.Format("ERROR Слишком короткий путь: {0}", FoldersOfPath.Length));
                    }
                }
                catch (Exception E)
                {
                    this.RecordingInLogFile(String.Format("Ошибка при генерации пути ярлыка: {0}", 
                        E.Message));
                }
                //
                // Checking Of Existence Of Label
                //
                bool LabelOfExists = false;
                //
                if (PathOfCreatingLabel.Length > 0)
                {
                    try { LabelOfExists = System.IO.File.Exists(PathOfCreatingLabel); }
                    catch (Exception E)
                    {
                        //
                        this.RecordingInLogFile(String.Format("Ошибка при проверке наличия ярлыка: {0}", 
                            E.Message));
                        //
                        LabelOfExists = false;
                    }
                }
                //
                // Creating Label Of Application
                //
                if ((PathOfCreatingLabel.Length > 0) && !(LabelOfExists))
                {
                    //
                    try
                    {
                        //
                        // Creating Streams Of Label
                        //
                        System.IO.FileStream FS = 
                            new System.IO.FileStream(PathOfCreatingLabel, 
                                System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                        System.IO.StreamWriter SW = 
                            new System.IO.StreamWriter(FS, Encoding.Unicode);
                        //
                        // Text Of Label
                        //
                        string TextOfFile = 
                            "http://www.medincom.ru/Med_Pharm.application#Med_Pharm.application, Culture=neutral, PublicKeyToken=0f5c9933a3db4437, processorArchitecture=msil";
                        //
                        // Writing Text Of Label
                        //
                        try { SW.Write(TextOfFile); }
                        catch (Exception E)
                        { this.RecordingInLogFile(String.Format("Ошибка при записи текста ярлыка: {0}", E.Message)); }
                        //
                        // Closing Steams
                        //
                        try
                        {
                            SW.Close();
                            FS.Close();
                        }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(String.Format("Ошибка при закрытии потоков создания ярлыка: {0}",
                              E.Message));
                        }
                    }
                    catch (Exception E)
                    { this.RecordingInLogFile(String.Format("Ошибка при создании потоков создания ярлыка: {0}", E.Message)); }
                }
            }
        }

        #region ' Checking Of Installation Of Application '

        //Checking Of Network
        public bool CheckingOfNetwork()
        {
            //
            // Checking
            //
            bool InstallationOfNetwork = false;
            //
            if (PathToWorkingDirectory != null)
            {
                //
                string[] FoldersOfPath = PathToWorkingDirectory.Split('\\');
                //
                if (FoldersOfPath.Length > 0)
                {
                    //
                    // Checking On Network
                    //
                    int CountOfConditionsConfirmedOfNetwork = 0;
                    //
                    for (int i = 0; i < FoldersOfPath.Length; i++)
                    {
                        if (FoldersOfPath[i].Length > 0)
                        {
                            if ((FoldersOfPath[i].ToUpper() == "Documents and Settings".ToUpper()) || 
                                (FoldersOfPath[i].ToUpper() == "Local Settings".ToUpper()) || 
                                (FoldersOfPath[i].ToUpper() == "Apps".ToUpper()))
                                CountOfConditionsConfirmedOfNetwork++;
                        }
                    }
                    //
                    if (CountOfConditionsConfirmedOfNetwork > 0)
                        InstallationOfNetwork = true;
                }
            }
            //
            // Return
            //
            return InstallationOfNetwork;
        }

        //Checking Of Local
        public bool CheckingOfLocal()
        {
            //
            // Checking
            //
            bool InstallationOfLocals = false;
            //
            if (PathToWorkingDirectory != null)
            {
                //
                string[] FoldersOfPath = PathToWorkingDirectory.Split('\\');
                //
                if (FoldersOfPath.Length > 0)
                {
                    //
                    // Checking On Localness
                    //
                    int CountOfConditionsConfirmedOfLocals = 0;
                    //
                    for (int i = 0; i < FoldersOfPath.Length; i++)
                    {
                        if (FoldersOfPath[i].Length > 0)
                        {
                            if ((FoldersOfPath[i].ToUpper() == "Program Files".ToUpper()) || 
                                (FoldersOfPath[i].ToUpper() == "Pharmaceutical Information".ToUpper()) || 
                                (FoldersOfPath[i].ToUpper() == "Drugstores Information".ToUpper()) || 
                                (FoldersOfPath[i].ToUpper() == "Drugstore".ToUpper()))
                                CountOfConditionsConfirmedOfLocals++;
                        }
                    }
                    if (CountOfConditionsConfirmedOfLocals == 3)
                        InstallationOfLocals = true;
                }
            }
            //
            // Return
            //
            return InstallationOfLocals;
        }

        // Getting String Of Installation Of Application
        public string GettingStringOfInstallationOfApplication()
        {
            //
            string StringOfInstallationOfApplication = "";
            //
            // Checking
            //
            bool InstallationOfNetwork = CheckingOfNetwork();
            bool InstallationOfLocals = CheckingOfLocal();
            //
            // Generation Of String Of Installation
            //
            if ((InstallationOfNetwork == true) && (InstallationOfLocals == false))
            { StringOfInstallationOfApplication = "Installation Of Application Is Network"; }
            else if ((InstallationOfNetwork == false) && (InstallationOfLocals == true))
            { StringOfInstallationOfApplication = "Installation Of Application Is Local"; }
            else
            {
                StringOfInstallationOfApplication = 
                    String.Format("Installation Of Application Is Undefined: {0} {1}",
                    InstallationOfNetwork, InstallationOfLocals);
            }
            //
            // Return
            //
            return StringOfInstallationOfApplication;
        }

        #endregion

        #endregion

        #region ' Getting Modules '

        // Getting Management Of Settings
        public PharmaceuticalInformation.Drugstore.ManagementOfSettings GettingManagementOfSettings
        {
            get { return ManagementOfSettings; }
        }

        // Getting Exchange FTP And Local DataBase
        public PharmaceuticalInformation.Drugstore.ExchangeFTPAndLocalDataBase GettingExchangeFTPAndLocalDataBase
        {
            get { return ExchangeFTPAndLocalDataBase; }
        }

        // Getting Integrity Of Data
        public PharmaceuticalInformation.Drugstore.IntegrityOfData GettingIntegrityOfData
        {
            get { return IntegrityOfData; }
        }

        // Getting Converting Of PriceList
        public PharmaceuticalInformation.Drugstore.ConvertingOfPriceList GettingConvertingOfPriceList
        {
            get { return ConvertingOfPriceList; }
        }

        // Getting Importing Prices In PriceList
        public PharmaceuticalInformation.Drugstore.ImportingPricesInPriceList GettingImportingPricesInPriceList
        {
            get { return ImportingPricesInPriceList; }
        }

        // Getting Management Of PriceList
        public PharmaceuticalInformation.Drugstore.ManagementOfPriceList GettingManagementOfPriceList
        {
            get { return ManagementOfPriceList; }
        }

        // Getting Management Of Comparisons
        public PharmaceuticalInformation.Drugstore.ManagementOfComparisons GettingManagementOfComparisons
        {
            get { return ManagementOfComparisons; }
        }

        // Getting Reading Of Data
        public PharmaceuticalInformation.Reading.ReadingOfDataForMySQL GettingReadingOfData
        {
            get { return ReadingOfData; }
        }

        // Getting Updating Of Data
        public PharmaceuticalInformation.Updating.UpdatingOfDataOfInformationForMySQL GettingUpdatingOfData
        {
            get { return UpdatingOfData; }
        }

        // Getting Path To LogFile
        public string GettingPathToLogFile
        {
            get { return this.PathToLogFile; }
        }

        #endregion

        #region ' Getting Status Of Modules '

        // Management Of Settings Is Working
        public bool ManagementOfSettingsIsWorking
        {
            get { return InitializedOfManagementOfSettings; }
        }

        // Exchange FTP And Local DataBase Is Working
        public bool ExchangeFTPAndLocalDataBaseIsWorking
        {
            get { return InitializedOfExchangeFTPAndLocalDataBase; }
        }

        // Integrity Of Data Is Working
        public bool IntegrityOfDataIsWorking
        {
            get { return InitializedOfIntegrityOfData; }
        }

        // Converting Of Price List Is Working
        public bool ConvertingOfPriceListIsWorking
        {
            get { return InitializedOfConvertingOfPriceList; }
        }

        // Importing Prices In PriceList Is Working
        public bool ImportingPricesInPriceListIsWorking
        {
            get { return InitializedOfImportingPricesInPriceList; }
        }

        // Management Of PriceList Is Working
        public bool ManagementOfPriceListIsWorking
        {
            get { return InitializedOfManagementOfPriceList; }
        }

        // Management Of Comparisons Is Working
        public bool ManagementOfComparisonsIsWorking
        {
            get { return InitializedOfManagementOfComparisons; }
        }

        // Reading Of Data Is Working
        public bool ReadingOfDataIsWorking
        {
            get { return InitializedOfReadingOfData; }
        }

        // Management Of Settings Is Working
        public bool UpdatingOfDataIsWorking
        {
            get { return InitializedOfUpdatingOfData; }
        }

        #endregion

        #region ' Updating Of Application '

        //
        string TypeOfUpdatingApplication = "UpdatingsOfDrugstore";
        //string TypeOfUpdatingApplication = "UpdatingsOfAutomaticalDrugstore";
        //
        string NameOfApplication = "Drugstore.exe";
        //string NameOfApplication = "AutomaticalDrugstore.exe";
        //
        string NewNameOfApplication = "NewDrugstore.exe";
        //string NewNameOfApplication = "NewAutomaticalDrugstore.exe";
        //
        string LastNameOfApplication = "LastDrugstore.exe";
        //string LastNameOfApplication = "LastAutomaticalDrugstore.exe";
        //
        bool SanctionOfCheckingOfExistingOfUpdating = true; // true; // false;

        // Updating Of Application
        public bool UpdatingOfApplication(
            string PathToLogFile, 
            PharmaceuticalInformation.Drugstore.ManagementOfSettings ManagementOfSettings, 
            PharmaceuticalInformation.Drugstore.ExchangeFTPAndLocalDataBase ExchangeFTPAndLocalDataBase)
        {
            //
            // Initializing Of Reportings
            //
            PharmaceuticalInformation.Service.Reportings Reportings = 
                new PharmaceuticalInformation.Service.Reportings(PathToLogFile);
            //
            // Checking Modules
            //
            if ((ManagementOfSettings == null) || (ExchangeFTPAndLocalDataBase == null))
            {
                //
                SanctionOfCheckingOfExistingOfUpdating = false;
                //
                Reportings.RecordingInLog("Transfered Module IS Null");
            }
            //
            // Checking Of Existence Of Updating Of Application
            //
            bool ExistenceOfUpdating = false;
            string NameOfUpdatingOfApplication = "";
            //
            if (SanctionOfCheckingOfExistingOfUpdating)
            {
                //
                Reportings.RecordingInLog("Checking Of Existence Of Updating Of Application");
                //
                // Getting String Of Version Of Application
                //
                string StringOfVersionOfApplication = ManagementOfSettings.VersionOfApplication;
                StringOfVersionOfApplication = StringOfVersionOfApplication.Replace(".", "");
                //
                // Converting String To Number Of Version
                //
                int NumberOfVersionOfApplication = -1;
                //
                try { NumberOfVersionOfApplication = Convert.ToInt32(StringOfVersionOfApplication); }
                catch (Exception E)
                {
                    //
                    NumberOfVersionOfApplication = -1;
                    //
                    Reportings.RecordingInLog(
                        String.Format("ERROR Ошибка при конвертации строки версии приложения: {0}", E.Message));
                }
                //
                // Getting Number Of Published Updating
                //
                //int NumberOfPublishedUpdating = -1;
                //
                if (NumberOfVersionOfApplication > 0)
                {
                    //
                    // Creating Path To Updating Application
                    //
                    string PathToListOfUpdatingApplication = 
                        String.Format(
                        "ftp://{0}@{1}/{2}/", 
                        ManagementOfSettings.GettingLoginAndPasswordToFTP, 
                        ManagementOfSettings.GettingServerOfFTP, 
                        TypeOfUpdatingApplication);
                    //
                    // Getting List Of Updatings Of Application
                    //
                    ArrayList ListOfUpdatingsOfApplication = new ArrayList();
                    //
                    PharmaceuticalInformation.Service.WorkingWithFTP WorkingWithFTP = 
                        ExchangeFTPAndLocalDataBase.GettingWorkingWithFTP;
                    //
                    ListOfUpdatingsOfApplication = 
                        WorkingWithFTP.GettingListOfDirectory04(
                        PathToListOfUpdatingApplication, ManagementOfSettings.GettingPassiveMode);
                    //
                    // Processing Of List Of Updatings Of Application
                    //
                    if (ListOfUpdatingsOfApplication.Count > 0)
                    {
                        //
                        // Creating List Of Numbers Of Updating
                        //
                        ArrayList ListOfNumbersOfUpdatings = new ArrayList();
                        //
                        foreach (string CurrentUpdating in ListOfUpdatingsOfApplication)
                        {
                            try
                            {
                                //
                                int NumberOfUpdating = Convert.ToInt32(CurrentUpdating.Replace(".", ""));
                                //
                                ListOfNumbersOfUpdatings.Add(NumberOfUpdating);
                            }
                            catch (Exception E)
                            {
                                //
                                Reportings.RecordingInLog(
                                  String.Format("ERROR Ошибка при конвертации номера обновления: {0}", E.Message));
                                //
                                ListOfNumbersOfUpdatings.Add(-1);
                            }
                        }
                        //
                        // Searching Maximal Number Of Updating
                        //
                        int MaximalNumberOfUpdating = -1;
                        int IndexOfNumberOfUpdating = -1;
                        //
                        for (int i = 0; i < ListOfNumbersOfUpdatings.Count; i++)
                            if (((int)ListOfNumbersOfUpdatings[i]) > MaximalNumberOfUpdating)
                            {
                                //
                                MaximalNumberOfUpdating = (int)ListOfNumbersOfUpdatings[i];
                                //
                                IndexOfNumberOfUpdating = i;
                            }
                        //
                        // !!!
                        //
                        if ((ListOfUpdatingsOfApplication.Count == ListOfNumbersOfUpdatings.Count) && 
                            (MaximalNumberOfUpdating > NumberOfVersionOfApplication))
                        {
                            //
                            ExistenceOfUpdating = true;
                            //
                            //NumberOfPublishedUpdating = MaximalNumberOfUpdating;
                            //
                            NameOfUpdatingOfApplication = 
                                ListOfUpdatingsOfApplication[IndexOfNumberOfUpdating].ToString();
                        }
                    }
                }
            }
            //
            // Loading Of Updating
            //
            bool ApplicationIsLoaded = false;
            //
            if (ExistenceOfUpdating)
            {
                //
                // True advance payment
                //
                ApplicationIsLoaded = true;
                //
                Reportings.RecordingInLog(
                    String.Format("Downloading Of Updating Application ({0})", NameOfUpdatingOfApplication));
                //
                // Creating Path To Downloading Of Updating Application
                //
                string PathToDownloadingOfUpdatingApplication = 
                    String.Format(
                    "ftp://{0}@{1}/{2}/{3}/{4}", 
                    ManagementOfSettings.GettingLoginAndPasswordToFTP, 
                    ManagementOfSettings.GettingServerOfFTP, 
                    TypeOfUpdatingApplication, 
                    NameOfUpdatingOfApplication, 
                    NameOfApplication);
                //
                // Creating Path To Loading Of Updating Application
                //
                string PathToLoadingOfUpdatingApplication = 
                    String.Format("{0}\\{1}", 
                    Path.GetDirectoryName(PathToLogFile), 
                    NewNameOfApplication);
                //
                // Deleting Already Exists File Of Downloaded Updating
                //
                if (File.Exists(PathToLoadingOfUpdatingApplication))
                {
                    //
                    Reportings.RecordingInLog("The File Of Updating Application Already Exists");
                    //
                    Reportings.RecordingInLog("Deleting File Of Updating Application");
                    //
                    try { File.Delete(PathToLoadingOfUpdatingApplication); }
                    catch (Exception E)
                    {
                        //
                        ApplicationIsLoaded = false;
                        //
                        Reportings.RecordingInLog(
                          String.Format("ERROR Ошибка при удалении ранее загруженного файла обновления: {0}", E.Message));
                    }
                }
                //
                // Downloading Of Updating
                //
                if (ApplicationIsLoaded)
                {
                    //
                    PharmaceuticalInformation.Service.WorkingWithFTP WorkingWithFTP = 
                        ExchangeFTPAndLocalDataBase.GettingWorkingWithFTP;
                    //
                    ApplicationIsLoaded = 
                        WorkingWithFTP.DownloadingFile(
                        PathToDownloadingOfUpdatingApplication, 
                        PathToLoadingOfUpdatingApplication, 
                        ManagementOfSettings.GettingPassiveMode);
                }
                //
                // Checking Of Existing Of Downloaded Application
                //
                if (!File.Exists(PathToLoadingOfUpdatingApplication))
                { ApplicationIsLoaded = false; }
            }
            //
            // Renaming Of File Of Application
            //
            bool ApplicationIsRenamed = false;
            //
            if (ApplicationIsLoaded)
            {
                //
                // True advance payment
                //
                ApplicationIsRenamed = true;
                //
                // Creating Paths
                //
                string PathToNewApplication = 
                    String.Format("{0}\\{1}", Path.GetDirectoryName(PathToLogFile), NewNameOfApplication);
                //
                string PathToCurrentApplication = 
                    String.Format("{0}\\{1}", Path.GetDirectoryName(PathToLogFile), NameOfApplication);
                //
                string PathToLastApplication = 
                    String.Format("{0}\\{1}", Path.GetDirectoryName(PathToLogFile), LastNameOfApplication);
                //
                // Renaming
                //
                if ((File.Exists(PathToNewApplication)) && (File.Exists(PathToCurrentApplication)))
                {
                    //
                    // Checking Of Existence Of Last Application
                    //
                    if (File.Exists(PathToLastApplication))
                    {
                        //
                        Reportings.RecordingInLog("The File Last Application Exists");
                        //
                        Reportings.RecordingInLog("Deleting File Of Last Application");
                        //
                        try { File.Delete(PathToLastApplication); }
                        catch (Exception E)
                        {
                            //
                            ApplicationIsRenamed = false;
                            //
                            Reportings.RecordingInLog(
                                String.Format("ERROR Ошибка при удалении файла предыдущей версии приложения: {0}", E.Message));
                        }
                    }
                    //
                    // Renaming Of Current Application
                    //
                    if (ApplicationIsRenamed)
                        try { File.Move(PathToCurrentApplication, PathToLastApplication); }
                        catch (Exception E)
                        {
                            //
                            ApplicationIsRenamed = false;
                            //
                            Reportings.RecordingInLog(
                                String.Format("ERROR Ошибка при перемещении текущего приложения в архив: {0}", E.Message));
                        }
                    //
                    // Renaming Of New Application
                    //
                    if (ApplicationIsRenamed)
                        try { File.Move(PathToNewApplication, PathToCurrentApplication); }
                        catch (Exception E)
                        {
                            //
                            ApplicationIsRenamed = false;
                            //
                            Reportings.RecordingInLog(
                                String.Format("ERROR Ошибка при перемещении нового приложения в текущие: {0}", E.Message));
                        }
                    //
                    //ApplicationIsRenamed = true;
                }
                else
                    ApplicationIsRenamed = false;
                //
            }
            Reportings.RecordingInLog("");
            //
            // Return
            //
            return ApplicationIsRenamed;
        }

        #endregion

        #region ' Sending Data '

        // Creating Structure For Sending
        private DataSet CreatingStructureForSending()
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
            StructureForSending.Tables["Information"].Rows.Add("IDOfDrugstore", IDOfSendingData);
            StructureForSending.Tables["Information"].Rows.Add("DateOfSending", DateTime.Now);
            StructureForSending.Tables["Information"].AcceptChanges();
            //
            // Return
            //
            return StructureForSending;
        }

        // Sending Log
        public void SendingLog()
        {
            //
            // Recording In LogFile
            //
            RecordingInLogFile("Sending Of System Logs");
            RecordingInLogFile("");
            //
            // Checking Is Registration
            //
            bool CheckedRegistration = this.CheckingIsRegistration();
            //
            // Checking Of Communication
            //
            bool CheckedCommunication = this.CheckingOfCommunication();
            //
            if (CheckedRegistration && CheckedCommunication)
            {
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
                ExchangeFTPAndLocalDataBase.SendingData(StructureForSending, IDOfSendingData);
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
            if(!CheckedCommunication)
            {
                RecordingInLogFile("INFOR Нет связи с FTP Сервером, Отправка данных невозможна");
                RecordingInLogFile("");
            }
        }

        // Sending PriceList
        public void SendingPriceList(bool ShowingMessage)
        {
            //
            // Checking Is Registration
            //
            bool CheckedRegistration = this.CheckingIsRegistration();
            //
            // Checking Of Communication
            //
            bool CheckedCommunication = this.CheckingOfCommunication();
            //
            if (CheckedRegistration && CheckedCommunication)
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
                this.RecordingInLogFile("Отправка Прайс-Листа");
                this.RecordingInLogFile("");
                //
                // Addition Of Data Of System Logs
                //
                AdditionOfDataOfSystemLogs(StructureForSending);
                //
                // Sending Data
                //
                ExchangeFTPAndLocalDataBase.SendingData(StructureForSending, IDOfSendingData);
                //
                // Message
                //
                string TextOfMessage = "Прайс-Лист Отправлен";
                //
                if (ShowingMessage)
                    new Service.Reportings(this.PathToLogFile).ReturningMessageOfInformation(TextOfMessage);
                else
                    new Service.Reportings(this.PathToLogFile).RecordingInLog(TextOfMessage);
            }
            //
            // Message Of Sending
            //
            if (!CheckedRegistration || !CheckedCommunication)
            {
                //
                string TextOfMessage = "Прайс-Лист НЕ Отправлен";
                //
                if (ShowingMessage)
                    new Service.Reportings(this.PathToLogFile).ReturningMessageOfInformation(TextOfMessage);
                else
                    new Service.Reportings(this.PathToLogFile).RecordingInLog(TextOfMessage);
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
            //
            // Reset Of Status Of Sending
            //
            //ManagementOfPL.ChangeOfSent(true);
        }

        // Sending File Of PriceList
        public void SendingFileOfPriceList()
        {
            //
        }

        #region ' Addition For Sending '

        private bool SanctionOfAutonomicSendingPriceList = true;
        // Addition Of Data Of PriceList
        private void AdditionOfDataOfPriceList(DataSet StructureForSending)
        {
            /*
            //
            if (ManagementOfPriceListIsWorking)
            {
                //
                // Refresh Data Of PriceList
                //
                ManagementOfPriceList.RefreshData();
                //
                // Deactivation Of Filtering
                //
                bool EnabledFiltering = ManagementOfPriceList.EnabledFiltering;
                ManagementOfPriceList.EnabledFiltering = false;
                //
                // Getting
                //
                DataTable PriceListForSending = 
                    ManagementOfPriceList.GettingPriceList(
                    "", PharmaceuticalInformation.Reading.ReadingOfData.TypesOfFiltration.NotSearch, false, false);
                //
                PriceListForSending.TableName = "PriceList";
                //
                // Filtering
                //
                DataView FilteringOfPriceList = new DataView(PriceListForSending);
                //FilteringOfPriceList.RowFilter = "(Sent = false)";
                PriceListForSending = 
                    FilteringOfPriceList.ToTable(
                    "PriceList", false, "ID_PH", "ID_PR", "Price", "Deleting", "Preferential", "AllPrices");
                //
                // Addition In Structure
                //
                StructureForSending.Tables.Add(PriceListForSending);
                //
                // Activation Of Filtering
                //
                ManagementOfPriceList.EnabledFiltering = true;
                ManagementOfPriceList.EnabledFiltering = EnabledFiltering;
            }
            */
            //
            if (ImportingPricesInPriceListIsWorking)
            {
                //
                // Creating
                //
                DataTable PriceListForSending = new DataTable("PriceList");
                //
                // Getting PriceList
                //
                if (ImportingPricesInPriceList.CanReadingPriceListFromDataBase)
                {
                    PriceListForSending = ImportingPricesInPriceList.GettingPriceListFromDataBase();
                }
                else if(SanctionOfAutonomicSendingPriceList)
                {
                    //
                    PriceListForSending = ImportingPricesInPriceList.GettingPriceListForImporting();
                }
                //
                // Addition In Structure
                //
                StructureForSending.Tables.Add(PriceListForSending);
            }
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
                StreamReader SR = new StreamReader(PathToWorkingDirectory + "Drugstore.txt", Encoding.Default);
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
            //if (ReadingOfDataIsWorking)
            if (ImportingPricesInPriceListIsWorking)
            {
                //
                // Addition Of Table Of Dates
                //
                //StructureForSending.Tables.Add(ReadingOfData.GettingDatesOfTransfer());
                StructureForSending.Tables.Add(ImportingPricesInPriceList.GettingDatesOfTransfer());
            }
        }

        #endregion

        #endregion

        #region ' Mode Of Initialization '

        private enum TypeOfInitialization : byte
        {
            FullInitialization,
            ReducedInitialization
        }

        #endregion

        #region ' Mode Of Working '

        public enum TypeOfWorking : byte
        {
            AllFunctions,
            OnlyTransferOfData
        }

        #endregion

        #region ' Item Of Executing '

        public struct ItemOfExecuting
        {
            public string TypeOfMarker;
            public object ValueOfMarker;
        }

        #endregion

    }
}