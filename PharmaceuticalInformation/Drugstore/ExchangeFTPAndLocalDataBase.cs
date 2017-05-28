using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ExchangeFTPAndLocalDataBase : BaseType
    {

        #region ' Fields '

        //
        private string _PathToExportOfFTP;
        private string _PathOfExportedPriceListsOfFTP;
        private string _PathToImportOfFTP;
        private string _PathToArchivingProgram;
        private string _PathToTMPFolder;
        private bool _UsePassive;
        private bool _IncludedShowingMessages;
        //
        private Service.WorkingWithFiles WorkWithFiles;
        private Service.WorkingWithFTP WorkWithFTP;
        //

        #endregion

        #region ' Designer '

        public ExchangeFTPAndLocalDataBase()
            : this("")
        {
            //
        }

        public ExchangeFTPAndLocalDataBase(string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            WorkWithFiles = new Service.WorkingWithFiles(PathToLogFile);
            WorkWithFTP = new Service.WorkingWithFTP(PathToLogFile);
            //this.ShowingMessages 
        }

        #endregion

        #region ' Parameters Of Work '

        // Included Showing Messages
        public bool IncludedShowingMessages
        {
            get { return _IncludedShowingMessages; }
            set
            {
                //
                _IncludedShowingMessages = value;
                //
                WorkWithFiles.ShowingMessages = _IncludedShowingMessages;
                WorkWithFTP.ShowingMessages = _IncludedShowingMessages;
            }
        }

        public string PathToExportOfFTP
        {
            get { return _PathToExportOfFTP; }
            set { _PathToExportOfFTP = value; }
        }

        // Path Of Exported PriceLists Of FTP
        public string PathOfExportedPriceListsOfFTP
        {
            get { return _PathOfExportedPriceListsOfFTP; }
            set { _PathOfExportedPriceListsOfFTP = value; }
        }

        public string PathToImportOfFTP
        {
            get { return _PathToImportOfFTP; }
            set { _PathToImportOfFTP = value; }
        }

        public bool UsePassive
        {
            get { return _UsePassive; }
            set { _UsePassive = value; }
        }

        public string PathToArchivingProgram
        {
            get { return _PathToArchivingProgram; }
            set { _PathToArchivingProgram = value; }
        }

        public string PathToTMPFolder
        {
            get { return _PathToTMPFolder; }
            set { _PathToTMPFolder = value; }
        }

        #endregion

        #region ' Getting Working '

        // Getting Working With Files
        public PharmaceuticalInformation.Service.WorkingWithFiles GettingWorkingWithFiles
        {
            get { return WorkWithFiles; }
        }

        // Getting Working With FTP
        public PharmaceuticalInformation.Service.WorkingWithFTP GettingWorkingWithFTP
        {
            get { return WorkWithFTP; }
        }

        #endregion

        #region ' Updating Data From FTP '

        // Updating Data Version 02 (Used By Server And Drugstore)
        public void UpdatingData02(PharmaceuticalInformation.Updating.UpdatingOfDataOfInformation Updating)
        {
            //
            // Updating Of PriceLists
            //
            UpdatingOfPriceLists(Updating);
            //
            // Updating Data
            //
            UpdatingData(Updating);
        }

        // Updating Of PriceLists
        private void UpdatingOfPriceLists(PharmaceuticalInformation.Updating.UpdatingOfDataOfInformation Updating)
        {
            //
            // !!!
            //
            try
            {
                //
                RecordingInLogFile("Checking Updating Of PriceLists");
                //
                // Getting List Of Files Of Updating Of PriceLists
                //
                ArrayList ListOfFilesOfUpdatingOfPriceLists = new ArrayList();
                //
                try
                {
                    ListOfFilesOfUpdatingOfPriceLists = 
                        WorkWithFTP.GettingListOfDirectory04(PathOfExportedPriceListsOfFTP, _UsePassive);
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при получении списка файлов обновления Прайс-Листов: {0}", E.Message)); }
                //
                if (ListOfFilesOfUpdatingOfPriceLists.Count > 0)
                {
                    //
                    //RecordingInLogFile(String.Format("Count Of ListOfFiles: {0}", ListOfFiles.Count.ToString()));
                    //
                    // Getting Number Of Updating Of PriceLists
                    //
                    int NumberOfUpdatingOfPriceLists = -1;
                    //
                    try
                    {
                        //
                        int[] ListOfNumbersOfUpdatingOfPriceLists = new int[ListOfFilesOfUpdatingOfPriceLists.Count];
                        //
                        for (int i = 0; i < ListOfNumbersOfUpdatingOfPriceLists.Length; i++)
                            ListOfNumbersOfUpdatingOfPriceLists[i] = 
                                Convert.ToInt32(
                                System.IO.Path.GetFileNameWithoutExtension(
                                ListOfFilesOfUpdatingOfPriceLists[i].ToString()));
                        //
                        NumberOfUpdatingOfPriceLists = 
                            Updating.FilteringOfListsOfExportedPriceLists(ListOfNumbersOfUpdatingOfPriceLists);
                    }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при фильтрации списка файлов обновления Прайс-Листов: {0}", E.Message)); }
                    //
                    RecordingInLogFile(
                        String.Format("Number Of Updating Of PriceLists: {0}", NumberOfUpdatingOfPriceLists));
                    //
                    if (NumberOfUpdatingOfPriceLists > -1)
                    {
                        //
                        WorkWithFTP.ShowingMessages = false;
                        //
                        RecordingInLogFile("");
                        RecordingInLogFile("Starting Updating Of PriceLists");
                        //
                        // !!!
                        //
                        try
                        {
                            //
                            RecordingInLogFile(String.Format("Loading Updating Of PriceLists: {0}", NumberOfUpdatingOfPriceLists));
                            //
                            string PathToFileOnFTP = 
                                String.Format("{0}{1}.rar", PathOfExportedPriceListsOfFTP, NumberOfUpdatingOfPriceLists);
                            string PathToFileARH = 
                                String.Format("{0}{1}.rar", _PathToTMPFolder, NumberOfUpdatingOfPriceLists);
                            //
                            // Downloading File Of Updating
                            //
                            bool ResultOfDownloading = false;
                            //
                            byte CountOfDownloading = 0;
                            while (CountOfDownloading < 2)
                            {
                                //
                                CountOfDownloading++;
                                //
                                ResultOfDownloading = 
                                    WorkWithFTP.DownloadingFile(PathToFileOnFTP, PathToFileARH, _UsePassive);
                                //
                                if (!ResultOfDownloading)
                                    System.Threading.Thread.Sleep(108);
                                //
                                if (ResultOfDownloading && (CountOfDownloading > 1))
                                    RecordingInLogFile("Repeating Loading Updating Of PriceLists");
                                //
                                if (ResultOfDownloading) break;
                            }
                            //
                            if (ResultOfDownloading)
                            {
                                //
                                WorkWithFiles.WaitingOfExists(PathToFileARH, 108);
                                WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 108);
                                //
                                if (WorkWithFiles.ExtractionFromArchive(_PathToArchivingProgram, PathToFileARH, _PathToTMPFolder))
                                {
                                    //
                                    string PathToFileTXT = System.IO.Path.ChangeExtension(PathToFileARH, "txt");
                                    //
                                    WorkWithFiles.WaitingOfExists(PathToFileTXT, 800);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileTXT, 800);
                                    //
                                    WorkWithFiles.WaitingOfExists(PathToFileARH, 800);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 800);
                                    //
                                    WorkWithFiles.DeletingFile(PathToFileARH);
                                    //
                                    // Updating Of PriceLists
                                    //
                                    if (System.IO.File.Exists(PathToFileTXT))
                                        Updating.UpdatingOfPriceLists(PathToFileTXT, NumberOfUpdatingOfPriceLists);
                                    else
                                        RecordingInLogFile("ERROR Ошибка, нет файла обновления Прайс-Листов");
                                    //
                                    // Deleting File
                                    //
                                    WorkWithFiles.DeletingFile(PathToFileTXT);
                                }
                                else { RecordingInLogFile("ERROR Неуспешная процедура разорхивирования файла."); }
                            }
                            else
                            { RecordingInLogFile("ERROR Неуспешная процедура загрузки файла обновления."); }
                        }
                        catch (Exception E)
                        { RecordingInLogFile(String.Format("ERROR Ошибка при скачке файла обновления Прайс-Листов: {0}", E.Message)); }
                        //шибка при обработке файла
                        //
                        RecordingInLogFile("Stoping Updating Of PriceLists");
                        //
                        WorkWithFTP.ShowingMessages = true;
                    }
                }
                //
                RecordingInLogFile(" ");
                //
                GC.Collect();
            }
            catch (Exception E)
            { RecordingInLogFile(String.Format("ERROR Ошибка в методе UpdatingOfPriceLists: {0}", E.Message)); }
        }

        // Updating Data
        private void UpdatingData(PharmaceuticalInformation.Updating.UpdatingOfDataOfInformation Updating)
        {
            //
            try
            {
                //
                RecordingInLogFile("Checking Updating");
                //
                ArrayList ListOfFiles = new ArrayList();
                //
                // Getting List Of Directory
                //
                try { ListOfFiles = WorkWithFTP.GettingListOfDirectory04(_PathToExportOfFTP, _UsePassive); }
                catch (Exception E) { RecordingInLogFile(String.Format("ERROR Ошибка при получении списка файлов: {0}", E.Message)); }
                //
                if (ListOfFiles.Count > 0)
                {
                    //
                    //RecordingInLogFile(String.Format("Count Of ListOfFiles: {0}", ListOfFiles.Count.ToString()));
                    //
                    // Filtering
                    //
                    int[] NumbersForUpdating = new int[0];
                    //
                    try
                    {
                        int[] NumbersOfExportingOnFTP = new int[ListOfFiles.Count];
                        //
                        for (int i = 0; i < NumbersOfExportingOnFTP.Length; i++)
                            NumbersOfExportingOnFTP[i] =
                                Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(ListOfFiles[i].ToString()));
                        //
                        NumbersForUpdating = Updating.FilteringOnNumberOfExported02(NumbersOfExportingOnFTP);
                        //
                    }
                    catch (Exception E) { RecordingInLogFile(String.Format("ERROR Ошибка при фильтрации списка: {0}", E.Message)); }
                    //
                    // Restriction Of Volume
                    //
                    try
                    {
                        if (NumbersForUpdating.Length > 200)
                        {
                            //
                            int[] AdmittedOfUpdates = new int[200];
                            for (int i = 0; i < AdmittedOfUpdates.Length; i++)
                                AdmittedOfUpdates[i] = NumbersForUpdating[i];
                            NumbersForUpdating = AdmittedOfUpdates;
                        }
                    }
                    catch (Exception E)
                    {
                        RecordingInLogFile(String.Format("ERROR Ошибка при ограничении списка (200): {0}", E.Message));
                    }
                    //
                    // Check Of Count
                    //
                    RecordingInLogFile(
                        String.Format("Count Of Packs For Updating: {0}", NumbersForUpdating.Length.ToString()));
                    //
                    if (NumbersForUpdating.Length > 0)
                    {
                        //
                        WorkWithFTP.ShowingMessages = false;
                        //
                        RecordingInLogFile("");
                        RecordingInLogFile("Starting Updating");
                        //
                        // !!!
                        //
                        try
                        {
                            foreach (int CurrentNumber in NumbersForUpdating)
                            {
                                //
                                RecordingInLogFile(String.Format("Loading Updating Of {0}", CurrentNumber));
                                //
                                // Creating Paths
                                //
                                string PathToFileOnFTP = String.Format("{0}{1}.rar", _PathToExportOfFTP, CurrentNumber);
                                string PathToFileARH = String.Format("{0}{1}.rar", _PathToTMPFolder, CurrentNumber);
                                //
                                // Downloading File Of Updating
                                //
                                bool ResultOfDownloading = false;
                                //
                                byte CountOfDownloading = 0;
                                while (CountOfDownloading < 2)
                                {
                                    //
                                    CountOfDownloading++;
                                    //
                                    ResultOfDownloading = 
                                        WorkWithFTP.DownloadingFile02(PathToFileOnFTP, PathToFileARH, _UsePassive);
                                    //
                                    if(!ResultOfDownloading)
                                        System.Threading.Thread.Sleep(108);
                                    //
                                    if (ResultOfDownloading && (CountOfDownloading > 1))
                                        RecordingInLogFile(String.Format("Repeating Downloading Of Updating {0}", CurrentNumber));
                                    //
                                    if (ResultOfDownloading) break;
                                }
                                //
                                // Waiting Of Existing And Access
                                //
                                if (ResultOfDownloading)
                                {
                                    WorkWithFiles.WaitingOfExists(PathToFileARH, 40);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 40);
                                }
                                //
                                // Extraction From Archive
                                //
                                bool ResultOfExtractionFromArchive = false;
                                string PathToFileBIN = System.IO.Path.ChangeExtension(PathToFileARH, "bin");
                                //
                                if (ResultOfDownloading)
                                {
                                    ResultOfExtractionFromArchive = 
                                        WorkWithFiles.ExtractionFromArchive(
                                        _PathToArchivingProgram, PathToFileARH, _PathToTMPFolder);
                                }
                                //
                                // Waiting Of Existing And Access
                                //
                                if (ResultOfExtractionFromArchive)
                                {
                                    //
                                    WorkWithFiles.WaitingOfExists(PathToFileBIN, 800);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileBIN, 800);
                                    //
                                    WorkWithFiles.WaitingOfExists(PathToFileARH, 800);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 800);
                                    //
                                    WorkWithFiles.DeletingFile(PathToFileARH);
                                }
                                //
                                // Loading Updating Of Data
                                //    
                                if (ResultOfExtractionFromArchive)
                                {
                                    //
                                    try
                                    {
                                        //
                                        // !!!
                                        //
                                        System.IO.FileStream FS = 
                                            new System.IO.FileStream(PathToFileBIN, System.IO.FileMode.Open,
                                        System.IO.FileAccess.Read);
                                        DataSet LoadedData = WorkWithFiles.Loading(FS);
                                        FS.Close();
                                        //
                                        // !!!
                                        //
                                        try { Updating.UpdatingOfData02(LoadedData); }
                                        catch (Exception E)
                                        {
                                            RecordingInLogFile(
                                                String.Format("ERROR Ошибка при загрузке обновления: {0}",
                                                E.Message));
                                        }
                                        //
                                        WorkWithFiles.DeletingFile(PathToFileBIN);
                                    }
                                    catch (Exception E)
                                    {
                                        /*
                                        ReturningMessageAboutError(
                                            String.Format("ERROR Ошибка при обработке файла: {0}", 
                                            PathToFileBIN), E, true);
                                        */
                                        this.RecordingInLogFile(
                                            String.Format("ERROR Ошибка при обработке файла ({0}): {1}",
                                            System.IO.Path.GetFileName(PathToFileBIN), E.Message));
                                    }
                                }
                            }
                        }
                        catch (Exception E)
                        { RecordingInLogFile(String.Format("ERROR Ошибка при скачке файлов: {0}", E.Message)); }
                        //
                        RecordingInLogFile("Stoping Updating");
                        //
                        WorkWithFTP.ShowingMessages = true;
                        //
                    }
                }
                else
                { RecordingInLogFile(String.Format("Count Of Packs For Updating: {0}", ListOfFiles.Count)); }
                //
                RecordingInLogFile(" ");
                //
                GC.Collect();
            }
            catch (Exception E) { RecordingInLogFile(String.Format("ERROR В Методе UpdatingData02: {0}", E.Message)); }
        }

        // Updating Data Version 01 ARH
        private void UpdatingDataARH01(PharmaceuticalInformation.Updating.UpdatingOfDataOfInformation Updating)
        {
            //
            RecordingInLogFile("Checking Updating");
            //
            ArrayList ListOfFiles = WorkWithFTP.GettingListOfDirectory04(_PathToExportOfFTP, _UsePassive);
            //
            if (ListOfFiles.Count > 0)
            {
                ArrayList ListOfDataSet = new ArrayList();
                //
                // Filtering
                //
                int[] NumbersOfExportingOnFTP = new int[ListOfFiles.Count];
                for (int i = 0; i < NumbersOfExportingOnFTP.Length; i++)
                    NumbersOfExportingOnFTP[i] =
                        Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(ListOfFiles[i].ToString()));
                int[] NumbersForUpdating = Updating.FilteringOnNumberOfExported(NumbersOfExportingOnFTP);
                //
                // Restriction Of Volume
                //
                if (NumbersForUpdating.Length > 100)
                {
                    //
                    int[] AdmittedOfUpdates = new int[100];
                    for (int i = 0; i < AdmittedOfUpdates.Length; i++)
                        AdmittedOfUpdates[i] = NumbersForUpdating[i];
                    //
                    NumbersForUpdating = AdmittedOfUpdates;
                }
                //
                // Check Of Count
                //
                if (NumbersForUpdating.Length > 0)
                {
                    //
                    RecordingInLogFile("Starting Updating");
                    //
                    foreach (int CurrentNumber in NumbersForUpdating)
                    {
                        RecordingInLogFile(String.Format("Number Of Updating {0}", CurrentNumber));
                        string PathToFileOnFTP = String.Format("{0}{1}.rar", _PathToExportOfFTP, CurrentNumber);
                        string PathToFileARH = String.Format("{0}{1}.rar", _PathToTMPFolder, CurrentNumber);
                        //
                        if (WorkWithFTP.DownloadingFile(PathToFileOnFTP, PathToFileARH, _UsePassive))
                        {
                            //
                            WorkWithFiles.WaitingOfExists(PathToFileARH, 40);
                            WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 40);
                            //
                            if (WorkWithFiles.ExtractionFromArchive(_PathToArchivingProgram, PathToFileARH, _PathToTMPFolder))
                            {
                                //
                                string PathToFileBIN = System.IO.Path.ChangeExtension(PathToFileARH, "bin");
                                //
                                WorkWithFiles.WaitingOfExists(PathToFileBIN, 800);
                                WorkWithFiles.WaitingOfIsAccess(PathToFileBIN, 800);
                                //
                                WorkWithFiles.WaitingOfExists(PathToFileARH, 800);
                                WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 800);
                                //
                                WorkWithFiles.DeletingFile(PathToFileARH);
                                //
                                try
                                {
                                    System.IO.FileStream FS = new System.IO.FileStream(PathToFileBIN, System.IO.FileMode.Open,
                                    System.IO.FileAccess.Read);
                                    ListOfDataSet.Add(WorkWithFiles.Loading(FS));
                                    FS.Close();
                                    //
                                    WorkWithFiles.DeletingFile(PathToFileBIN);
                                }
                                catch (Exception E)
                                {
                                    ReturningMessageAboutError(
                                        String.Format("Ошибка при открытии файла: {0}", PathToFileBIN), E, true);
                                }
                                finally { }
                            }
                        }
                    }
                    //
                    // Association List Of DataSet
                    //
                    int CountOfLastPackage = 0;
                    for (int i = 0; i <= ListOfDataSet.Count; i++)
                    {

                        if ((i != 0) && (((i % 10) == 0) || (i == ListOfDataSet.Count)))
                        {
                            int CountOfNewPackage = i - CountOfLastPackage;
                            DataSet[] DataSetForAssociation = new DataSet[CountOfNewPackage];
                            int Next = 0;
                            for (int i2 = CountOfLastPackage; i2 < i; i2++)
                                DataSetForAssociation[Next++] = (DataSet)ListOfDataSet[i2];
                            CountOfLastPackage = i;
                            //
                            DataSet AssociationsOfUpdating = Updating.AssociationDateSet(DataSetForAssociation);
                            //
                            // Updating
                            //
                            /*foreach (DataSet CurrentDataSet in ListOfDataSet)
                                Updating.UpdatingOfData(CurrentDataSet);*/
                            Updating.UpdatingOfData(AssociationsOfUpdating);
                        }
                    }
                    //
                    RecordingInLogFile("Stoping Updating");
                    //
                }
            }
            //
            RecordingInLogFile(" ");
            //
            GC.Collect();
        }

        // Updating Data Version 02 ARH
        private void UpdatingDataARH02(PharmaceuticalInformation.Updating.UpdatingOfDataOfInformation Updating)
        {
            //
            try
            {
                //
                //RecordingInLogFile("00");
                //
                RecordingInLogFile("Checking Updating");
                //
                ArrayList ListOfFiles = new ArrayList();
                //
                // Getting List Of Directory
                //
                try { ListOfFiles = WorkWithFTP.GettingListOfDirectory04(_PathToExportOfFTP, _UsePassive); }
                catch (Exception E) { RecordingInLogFile(String.Format("ERROR Ошибка при получении списка файлов: {0}", E.Message)); }
                //
                RecordingInLogFile("01");
                //
                if (ListOfFiles.Count > 0)
                {
                    //
                    RecordingInLogFile("02");
                    RecordingInLogFile(String.Format("Count Of ListOfFiles: {0}", ListOfFiles.Count.ToString()));
                    //
                    ArrayList ListOfDataSet = new ArrayList();
                    //
                    // Filtering
                    //
                    int[] NumbersForUpdating = new int[0];
                    //
                    RecordingInLogFile("03");
                    //
                    try
                    {
                        int[] NumbersOfExportingOnFTP = new int[ListOfFiles.Count];
                        //
                        RecordingInLogFile("04");
                        //
                        for (int i = 0; i < NumbersOfExportingOnFTP.Length; i++)
                            NumbersOfExportingOnFTP[i] =
                                Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(ListOfFiles[i].ToString()));
                        //
                        RecordingInLogFile("05");
                        //
                        NumbersForUpdating = Updating.FilteringOnNumberOfExported(NumbersOfExportingOnFTP);
                        //
                        RecordingInLogFile("06");
                        //
                    }
                    catch (Exception E) { RecordingInLogFile(String.Format("ERROR Ошибка при фильтрации списка: {0}", E.Message)); }
                    //
                    // Restriction Of Volume
                    //
                    RecordingInLogFile("07");
                    //
                    try
                    {
                        if (NumbersForUpdating.Length > 100)
                        {
                            //
                            //RecordingInLogFile("08");
                            //
                            int[] AdmittedOfUpdates = new int[100];
                            for (int i = 0; i < AdmittedOfUpdates.Length; i++)
                                AdmittedOfUpdates[i] = NumbersForUpdating[i];
                            //
                            //RecordingInLogFile("09");
                            //
                            NumbersForUpdating = AdmittedOfUpdates;
                        }
                    }
                    catch (Exception E) { RecordingInLogFile(String.Format("ERROR Ошибка при ограничении списка (100): {0}", E.Message)); }
                    //
                    RecordingInLogFile("10");
                    //
                    // Check Of Count
                    //
                    RecordingInLogFile(String.Format("Count Of NumbersForUpdating: {0}", NumbersForUpdating.Length.ToString()));
                    //
                    if (NumbersForUpdating.Length > 0)
                    {
                        //
                        //RecordingInLogFile("11");
                        //
                        RecordingInLogFile("Starting Updating");
                        //
                        // !!!
                        //
                        try
                        {
                            foreach (int CurrentNumber in NumbersForUpdating)
                            {
                                RecordingInLogFile(String.Format("Number Of Updating {0}", CurrentNumber));
                                string PathToFileOnFTP = String.Format("{0}{1}.rar", _PathToExportOfFTP, CurrentNumber);
                                string PathToFileARH = String.Format("{0}{1}.rar", _PathToTMPFolder, CurrentNumber);
                                //
                                //RecordingInLogFile("12");
                                //
                                if (WorkWithFTP.DownloadingFile(PathToFileOnFTP, PathToFileARH, _UsePassive))
                                {
                                    //
                                    //RecordingInLogFile("13");
                                    //
                                    WorkWithFiles.WaitingOfExists(PathToFileARH, 40);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 40);
                                    //
                                    if (WorkWithFiles.ExtractionFromArchive(_PathToArchivingProgram, PathToFileARH, _PathToTMPFolder))
                                    {
                                        //
                                        //RecordingInLogFile("14");
                                        //
                                        string PathToFileBIN = System.IO.Path.ChangeExtension(PathToFileARH, "bin");
                                        //
                                        WorkWithFiles.WaitingOfExists(PathToFileBIN, 800);
                                        WorkWithFiles.WaitingOfIsAccess(PathToFileBIN, 800);
                                        //
                                        WorkWithFiles.WaitingOfExists(PathToFileARH, 800);
                                        WorkWithFiles.WaitingOfIsAccess(PathToFileARH, 800);
                                        //
                                        WorkWithFiles.DeletingFile(PathToFileARH);
                                        //
                                        try
                                        {
                                            //
                                            //RecordingInLogFile("15");
                                            //
                                            System.IO.FileStream FS = new System.IO.FileStream(PathToFileBIN, System.IO.FileMode.Open,
                                            System.IO.FileAccess.Read);
                                            ListOfDataSet.Add(WorkWithFiles.Loading(FS));
                                            FS.Close();
                                            //
                                            //RecordingInLogFile("16");
                                            //
                                            WorkWithFiles.DeletingFile(PathToFileBIN);
                                        }
                                        catch (Exception E)
                                        {
                                            ReturningMessageAboutError(
                                                String.Format("Ошибка при открытии файла: {0}", PathToFileBIN), E, true);
                                        }
                                        finally { }
                                    }
                                }
                            }
                        }
                        catch (Exception E)
                        { RecordingInLogFile(String.Format("ERROR Ошибка при скачке файлов: {0}", E.Message)); }
                        //
                        // Association List Of DataSet
                        //
                        //RecordingInLogFile("17");
                        //
                        try
                        {
                            int CountOfLastPackage = 0;
                            for (int i = 0; i <= ListOfDataSet.Count; i++)
                            {
                                //
                                //RecordingInLogFile("18");
                                //
                                if ((i != 0) && (((i % 10) == 0) || (i == ListOfDataSet.Count)))
                                {
                                    //
                                    //RecordingInLogFile("19");
                                    //
                                    int CountOfNewPackage = i - CountOfLastPackage;
                                    DataSet[] DataSetForAssociation = new DataSet[CountOfNewPackage];
                                    int Next = 0;
                                    for (int i2 = CountOfLastPackage; i2 < i; i2++)
                                        DataSetForAssociation[Next++] = (DataSet)ListOfDataSet[i2];
                                    CountOfLastPackage = i;
                                    //
                                    //RecordingInLogFile("20");
                                    //
                                    DataSet AssociationsOfUpdating = Updating.AssociationDateSet(DataSetForAssociation);
                                    //
                                    //RecordingInLogFile("21");
                                    //
                                    // Updating
                                    //
                                    /*foreach (DataSet CurrentDataSet in ListOfDataSet)
                                        Updating.UpdatingOfData(CurrentDataSet);*/
                                    Updating.UpdatingOfData(AssociationsOfUpdating);
                                    //
                                    //RecordingInLogFile("22");
                                }
                            }
                        }
                        catch (Exception E) { RecordingInLogFile(String.Format("Ошибка при накладке обновлений: {0}", E.Message)); }
                        //
                        RecordingInLogFile("Stoping Updating");
                        //
                    }
                }
                //
                RecordingInLogFile(" ");
                //
                GC.Collect();
            }
            catch (Exception E) { RecordingInLogFile(String.Format("ERROR В Методе UpdatingData: {0}", E.Message)); }
        }

        #endregion

        #region ' Sending Data To FTP '

        public void SendingData(DataSet DataForSending, int IDOfDrugstore)
        {
            //
            RecordingInLogFile("Starting Sending");
            //
            if ((DataForSending != null) && (IDOfDrugstore > 0))
            {
                //
                string NameOfSending = CreatingNameOfFile(
                    String.Format("Data_{0}", IDOfDrugstore), DateTime.Now);
                //
                RecordingInLogFile(String.Format("Name Of Sending = {0}", NameOfSending));
                //
                string PathToFileBIN = String.Format("{0}{1}.bin", _PathToTMPFolder, NameOfSending);
                string PathToFileRAR = System.IO.Path.ChangeExtension(PathToFileBIN, "rar");
                string PathToFileOnFTP = String.Format("{0}{1}.rar", _PathToImportOfFTP, NameOfSending);
                //
                // !!!
                //
                try
                {
                    System.IO.FileStream FS = new System.IO.FileStream(PathToFileBIN, System.IO.FileMode.Create,
                        System.IO.FileAccess.Write);
                    WorkWithFiles.Saving(DataForSending, (System.IO.Stream)FS);
                    FS.Close();
                }
                catch (Exception E)
                {
                    //
                    /*
                    ReturningMessageAboutError(
                        String.Format("Ошибка при создании файла: {0}", PathToFileBIN), E, true);
                    */
                    //
                    this.RecordingInLogFile(
                        String.Format("Ошибка при создании файла ({0}): {1}",
                        System.IO.Path.GetFileName(PathToFileBIN), E.Message));
                }
                //
                // Waiting Of BIN
                //
                WorkWithFiles.WaitingOfExists(PathToFileBIN, 80);
                WorkWithFiles.WaitingOfIsAccess(PathToFileBIN, 80);
                //
                // !!!
                //
                if (System.IO.File.Exists(PathToFileBIN))
                    if (WorkWithFiles.IsAccessFile(PathToFileBIN))
                    {
                        if (WorkWithFiles.ArchivingFile(_PathToArchivingProgram, PathToFileRAR, PathToFileBIN))
                        {
                            //
                            // Waiting Of RAR
                            //
                            WorkWithFiles.WaitingOfExists(PathToFileRAR, 108);
                            WorkWithFiles.WaitingOfIsAccess(PathToFileRAR, 108);
                            //
                            // Waiting Of BIN
                            //
                            WorkWithFiles.WaitingOfExists(PathToFileBIN, 108);
                            WorkWithFiles.WaitingOfIsAccess(PathToFileBIN, 108);
                            //
                            // Waiting 
                            //
                            System.Threading.Thread.Sleep(18);
                            //
                            // Deleting BIN
                            //
                            WorkWithFiles.DeletingFile(PathToFileBIN);
                            //
                            if (WorkWithFiles.IsAccessFile(PathToFileRAR))
                            {
                                //
                                // Uploading File
                                //
                                bool ResultOfUploadingFile = false;
                                //
                                byte CountOfDownloading = 0;
                                while (CountOfDownloading < 2)
                                {
                                    //
                                    CountOfDownloading++;
                                    //
                                    ResultOfUploadingFile = 
                                        WorkWithFTP.UploadingFile(PathToFileRAR, PathToFileOnFTP, _UsePassive);
                                    //
                                    if (!ResultOfUploadingFile)
                                        System.Threading.Thread.Sleep(108);
                                    //
                                    if (ResultOfUploadingFile && (CountOfDownloading > 1))
                                        RecordingInLogFile("Repeating Sending");
                                    //
                                    if (ResultOfUploadingFile) break;
                                }
                                //
                                // !!!
                                //
                                if (ResultOfUploadingFile)
                                {
                                    //
                                    // Waiting Of RAR
                                    //
                                    WorkWithFiles.WaitingOfExists(PathToFileRAR, 108);
                                    WorkWithFiles.WaitingOfIsAccess(PathToFileRAR, 108);
                                    //
                                    // Waiting 
                                    //
                                    System.Threading.Thread.Sleep(18);
                                    //
                                    // Deleting Archive
                                    //
                                    WorkWithFiles.DeletingFile(PathToFileRAR);
                                }
                            }
                        }
                    }
            }
            else
                RecordingInLogFile("Data For Sending Is Null Or IDOfDrugstore is 0");
            //
            RecordingInLogFile("Stoping Sending");
            RecordingInLogFile("");
            //
            GC.Collect();
        }

        private string CreatingNameOfFile(string BaseName, DateTime Date)
        {
            string NameOfFile = String.Format(
                "{0}_{1}", BaseName,
                String.Format("{0}{1}{2}_{3}{4}{5}",
                Date.Year, Date.Month.ToString("00"), Date.Day.ToString("00"),
                Date.Hour.ToString("00"), Date.Minute.ToString("00"), Date.Second.ToString("00")));
            return NameOfFile;
        }

        #endregion

    }
}