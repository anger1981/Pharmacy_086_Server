using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using PharmaceuticalInformation.BaseTypes;
using Test_pharm_server.PharmaceuticalInformation.Infrastructure;
using Test_pharm_server.PharmaceuticalInformation.Interfaces;
using Ninject;

namespace PharmaceuticalInformation.Server
{
    public class ExchangeBetweenSystemAndQueues : BaseType
    {

        #region ' Fields '

        //
        // Importing And Exporting
        //
        private PharmaceuticalInformation.Server.ImportingOfData ImportingOfData;
        private PharmaceuticalInformation.Server.ExportingOfData ExportingOfData;
        //
        // Paths Of Processing
        //
        private string _PathOfArchivingProgram;
        private string _PathOfTMPFolder;
        //
        // Access To Store Of Data
        //
        private string _AccessToStoreOfData;
        //
        // Paths Of Importing
        //
        private string _PathOfImportingDataFromQueue;
        private string _PathOfImportingOfIodineFace;
        //
        // Paths Of Exporting
        //
        private string _PathOfExportingDataOfFTP;
        private string _PathToExportingPriceListsOnFTP;
        private string _PathOfExportingUpdatingAllSites;
        private string _PathOfExportingUpdatingOneSite;
        //
        // Paths Of Archives
        //
        private string _PathOfArchivesOfImporting;
        //
        // Services
        //
        private PharmaceuticalInformation.Service.WorkingWithFiles WorkingWithFiles;
        private PharmaceuticalInformation.Service.WorkingWithFTP WorkingWithFTP;

        #endregion

        #region ' Designer '

        public ExchangeBetweenSystemAndQueues(IPharmacyInformation _IPhrmInf)
            : this(_IPhrmInf, "")
        {
            //
        }

        public ExchangeBetweenSystemAndQueues(IPharmacyInformation _IPhrmInf, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initializing Transfer
            //
            ImportingOfData = new ImportingOfData(_IPhrmInf, PathToLogFile);
            ExportingOfData = new ExportingOfData(_IPhrmInf, PathToLogFile);
            //
            // Initializing Services
            //
            WorkingWithFiles = new PharmaceuticalInformation.Service.WorkingWithFiles(PathToLogFile);
            WorkingWithFTP = new PharmaceuticalInformation.Service.WorkingWithFTP(PathToLogFile);
            //
            // Hiding Messages
            //
            this.ShowingMessages = false;
            //
            ImportingOfData.ShowingMessages = false;
            ExportingOfData.ShowingMessages = false;
            WorkingWithFiles.ShowingMessages = false;
            WorkingWithFTP.ShowingMessages = false;
        }

        #endregion

        #region ' Setting Paths '

        #region ' Paths Of Processing '

        // Path Of Archiving Program
        public string PathOfArchivingProgram
        {
            get { return _PathOfArchivingProgram; }
            set { _PathOfArchivingProgram = value; }
        }

        // Path Of TMP Folder
        public string PathOfTMPFolder
        {
            get { return _PathOfTMPFolder; }
            set { _PathOfTMPFolder = value; }
        }

        #endregion

        // Access To Store Of Data
        public string AccessToStoreOfData
        {
            get { return _AccessToStoreOfData; }
            set { _AccessToStoreOfData = value; }
        }

        #region ' Paths Of Importing '

        // Path Of Importing Data From Queue
        public string PathOfImportingDataFromQueue
        {
            get { return _PathOfImportingDataFromQueue; }
            set { _PathOfImportingDataFromQueue = value; }
        }

        // Path Of Importing Of IodineFace
        public string PathOfImportingOfIodineFace
        {
            get { return _PathOfImportingOfIodineFace; }
            set { _PathOfImportingOfIodineFace = value; }
        }

        #endregion

        #region ' Paths Of Exporting '

        // Path To Exporting Data Of FTP
        public string PathToExportingDataOfFTP
        {
            get { return _PathOfExportingDataOfFTP; }
            set { _PathOfExportingDataOfFTP = value; }
        }

        // Path To Exporting PriceLists On FTP
        public string PathToExportingPriceListsOnFTP
        {
            get { return _PathToExportingPriceListsOnFTP; }
            set { _PathToExportingPriceListsOnFTP = value; }
        }

        // Path Of Exporting Updating All Sites
        public string PathOfExportingUpdatingAllSites
        {
            get { return _PathOfExportingUpdatingAllSites; }
            set { _PathOfExportingUpdatingAllSites = value; }
        }

        // Path Of Exporting Updating One Site
        public string PathOfExportingUpdatingOneSite
        {
            get { return _PathOfExportingUpdatingOneSite; }
            set { _PathOfExportingUpdatingOneSite = value; }
        }

        #endregion

        // Path Of Archives Of Importing
        public string PathOfArchivesOfImporting
        {
            get { return _PathOfArchivesOfImporting; }
            set { _PathOfArchivesOfImporting = value; }
        }

        #endregion

        #region ' Downloading Data From Queue Imports '

        // Downloading Data From FTP
        public void DownloadingDataFromFTP(StringBuilder LogsOfMethod)
        {
            //
            //RecordingInLogFile("Checking Importing");
            //
            // Getting List Of Imported Files
            //
            ArrayList ListOfImportedFiles = new ArrayList();
            //
            foreach (string CurrentFileName in WorkingWithFTP.GettingListOfDirectory03(_PathOfImportingDataFromQueue, false))
                ListOfImportedFiles.Add(String.Format("{0}{1}", _PathOfImportingDataFromQueue, CurrentFileName));
            //
            // Checking Of Count Of List Of Files
            //
            if (ListOfImportedFiles.Count > 0)
            {
                //
                RecordingInLogFile("Checking Importing");
                RecordingInLogFile("");
                //
                // Scaning Of Files Of Importing
                //
                foreach (string CurrentFile in ListOfImportedFiles)
                {
                    //
                    // Recording In Log File
                    //
                    RecordingInLogFile(
                        String.Format("Importing Of File: {0}", System.IO.Path.GetFileName(CurrentFile)));
                    //
                    // Initialization Of Paths
                    //
                    string PathOfFileOnFTP = CurrentFile;
                    string PathOfFileARH =
                        String.Format("{0}{1}", _PathOfTMPFolder, System.IO.Path.GetFileName(CurrentFile));
                    //
                    // Deleting File If It Exists
                    //
                    if (File.Exists(PathOfFileARH))
                    {
                        try { File.Delete(PathOfFileARH); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                              String.Format("Ошибка при удалении файла '{0}': {1}", PathOfFileARH, E.Message));
                        }
                    }
                    //
                    // Downloading File From Importing
                    //
                    bool ResultOfImporting = false;
                    //
                    RecordingInLogFile("Starting Importing From FTP");
                    //
                    try { ResultOfImporting = WorkingWithFTP.DownloadingFile02(PathOfFileOnFTP, PathOfFileARH, false); }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Error Of Importing Data From FTP: {0}", E.Message)); }
                    //
                    RecordingInLogFile(String.Format("Stoping Importing From FTP Result={0}", ResultOfImporting));
                    //
                    // !!!
                    //
                    if (ResultOfImporting)
                    {
                        //
                        // Waiting Of Imported File
                        //
                        WorkingWithFiles.WaitingOfExists(PathOfFileARH, 80);
                        WorkingWithFiles.WaitingOfIsAccess(PathOfFileARH, 80);
                        //
                        // Waiting 
                        //
                        System.Threading.Thread.Sleep(509);
                        //
                        // Deleting Of File On FTP
                        //
                        WorkingWithFTP.DeletingFile02(PathOfFileOnFTP);
                        //
                        // Processing Of Imported File
                        //
                        if (System.IO.Path.GetFileName(CurrentFile).ToLower().IndexOf("data") >= 0)
                        {
                            //
                            // Extraction From Archive
                            //
                            LogsOfMethod.Append("A");
                            if (WorkingWithFiles.ExtractionFromArchive(_PathOfArchivingProgram,
                                PathOfFileARH, _PathOfTMPFolder))
                            {
                                //
                                LogsOfMethod.Append("B");
                                //
                                // Renaming Name Of File
                                //
                                string PathOfFileBIN = System.IO.Path.ChangeExtension(PathOfFileARH, "bin");
                                //
                                // Waiting Of RAR
                                //
                                WorkingWithFiles.WaitingOfExists(PathOfFileARH, 108);
                                WorkingWithFiles.WaitingOfIsAccess(PathOfFileARH, 108);
                                //
                                // Waiting Of BIN
                                //
                                WorkingWithFiles.WaitingOfExists(PathOfFileBIN, 108);
                                WorkingWithFiles.WaitingOfIsAccess(PathOfFileBIN, 108);
                                //
                                // Waiting 
                                //
                                System.Threading.Thread.Sleep(18);
                                //
                                // Deleting Archive
                                //
                                LogsOfMethod.Append("C");
                                WorkingWithFiles.DeletingFile(PathOfFileARH);
                                LogsOfMethod.Append("D");
                                //
                                // Loading BIN File
                                //
                                if (System.IO.File.Exists(PathOfFileBIN))
                                    try
                                    {
                                        //
                                        // Converting BIN To DataSet
                                        //
                                        LogsOfMethod.Append("E");
                                        //
                                        System.IO.FileStream FS =
                                            new System.IO.FileStream(
                                                PathOfFileBIN, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                        DataSet DataFromDrugstore = WorkingWithFiles.Loading(FS);
                                        FS.Close();
                                        //
                                        // Importing Of Data
                                        //
                                        LogsOfMethod.Append("F");
                                        //
                                        try { ImportingOfData.ImportingDataFromDrugstore(DataFromDrugstore); }
                                        catch (Exception E)
                                        {
                                            RecordingInLogFile(
                                                String.Format("ERROR Ошибка при импортировании данных файла: {0}: {1}",
                                                PathOfFileBIN, E.Message));
                                        }
                                        //
                                        LogsOfMethod.Append("G");
                                        //
                                        // Waiting Of BIN
                                        //
                                        WorkingWithFiles.WaitingOfIsAccess(PathOfFileBIN, 108);
                                        //
                                        // Deleting Of BIN
                                        //
                                        WorkingWithFiles.DeletingFile(PathOfFileBIN);
                                        //
                                        LogsOfMethod.Append("H");
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(String.Format("Ошибка при конвертации файла {0}: {1}",
                                            PathOfFileBIN, E.Message));
                                    }
                                else
                                    RecordingInLogFile(
                                        String.Format("ERROR Not Exists File: {0}", PathOfFileBIN));
                            }
                            else
                            {
                                //
                                RecordingInLogFile(
                                    String.Format("ERROR Error Of Unarchiving File: {0}", PathOfFileARH));
                            }
                        }
                    }
                    else
                    {
                        //
                        RecordingInLogFile("Failure Of Importing");
                    }
                    //
                    RecordingInLogFile("");
                    //
                    // Collect Dust
                    //
                    GC.Collect();
                }
                //
                // Recording Pause
                //
                RecordingInLogFile("");
            }
            //
            // Collect Dust
            //
            GC.Collect();
            //
            // Return
            //
            //return LogsOfMethod;
        }

        // Downloading Data From FTP 02 (NO Working)
        public void DownloadingDataFromFTP02(StringBuilder LogsOfMethod)
        {
            //
            // Getting List Of Files For Importing
            //
            ArrayList ListOfFilesForImporting = new ArrayList();
            ArrayList ListOfNamesOfFilesForImporting = new ArrayList();
            //
            ListOfNamesOfFilesForImporting = 
                WorkingWithFTP.GettingListOfDirectory03(_PathOfImportingDataFromQueue, true);
            //
            foreach (string CurrentFileName in ListOfNamesOfFilesForImporting)
                ListOfFilesForImporting.Add(String.Format("{0}{1}", _PathOfImportingDataFromQueue, CurrentFileName));
            //
            // Checking Count Of Files For Importing
            //
            bool CountOfFilesIsOK = false;
            //
            if (ListOfFilesForImporting.Count > 0)
            { CountOfFilesIsOK = true; }
            //
            // Importing Files Of Data 
            //
            if(CountOfFilesIsOK)
            {
                //
                // Recording In Log File
                //
                RecordingInLogFile(String.Format("Importing Files Of Data (Count Of {0})", ListOfFilesForImporting.Count));
                RecordingInLogFile("");
                //
                // Downloading Files From List Of Files
                //
                foreach (string CurrentFile in ListOfFilesForImporting)
                {
                    //
                    // Recording In Log File
                    //
                    RecordingInLogFile(
                        String.Format("Importing Of File: {0}", Path.GetFileName(CurrentFile)));
                    //
                    // Creating Paths To File
                    //
                    string PathToFileInQueueOfImporting = CurrentFile;
                    string PathToImportedFile = 
                        String.Format("{0}{1}", _PathOfTMPFolder, Path.GetFileName(CurrentFile));
                    //
                    // Deleting Imported File If It He Exists
                    //
                    bool NotExistsSimilarFile = true;
                    //
                    if (File.Exists(PathToImportedFile))
                    {
                        try { File.Delete(PathToImportedFile); }
                        catch (Exception E)
                        {
                            //
                            this.RecordingInLogFile(
                              String.Format("ERROR Error At Deleting File Of Similar ({0}): {1}", 
                              Path.GetFileName(PathToImportedFile), E.Message));
                            //
                            NotExistsSimilarFile = false;
                        }
                    }
                    //
                    // Downloading File From Queue Of Importing
                    //
                    bool ResultOfDownloading = false;
                    //
                    if (NotExistsSimilarFile)
                    {
                        //
                        DateTime TimeOfBeginningOfDownloading = new DateTime(1947, 07, 02);
                        DateTime TimeOfEndingOfDownloading = new DateTime(1947, 07, 02);
                        //
                        RecordingInLogFile("Starting Downloading Of File");
                        TimeOfBeginningOfDownloading = DateTime.Now;
                        //
                        try
                        {
                            ResultOfDownloading = 
                                WorkingWithFTP.DownloadingFile02(
                                PathToFileInQueueOfImporting, PathToImportedFile, false);
                        }
                        catch (Exception E)
                        {
                            RecordingInLogFile(
                                String.Format("ERROR Error Of Downloading File ({0}): {1}", 
                                Path.GetFileName(PathToFileInQueueOfImporting), E.Message));
                        }
                        //
                        RecordingInLogFile("Stoping Downloading Of File");
                        TimeOfEndingOfDownloading = DateTime.Now;
                        //
                        if(ResultOfDownloading == false)
                            RecordingInLogFile("ERROR Fatal Downloading");
                        //
                        if (ResultOfDownloading)
                        {
                            //
                            TimeSpan TimeOfDownloading = TimeOfEndingOfDownloading.Subtract(TimeOfBeginningOfDownloading);
                            //
                            if (TimeOfDownloading.TotalSeconds > 10)
                            { WorkingWithFTP.ResetOfResponseOfDownloadingFile(); }
                        }
                        //
                        // Checking Of Existence Of File (Move To DownloadingFile02)
                        //
                        ResultOfDownloading = File.Exists(PathToImportedFile);
                    }
                    //
                    // Waiting Of Imported File
                    //
                    if (ResultOfDownloading)
                    {
                        //
                        // Waiting Of Imported File
                        //
                        WorkingWithFiles.WaitingOfIsAccess(PathToImportedFile, 80);
                        //
                        // Waiting
                        //
                        System.Threading.Thread.Sleep(509);
                    }
                    //
                    // Deleting File From Queue Of Importing
                    //
                    if (ResultOfDownloading)
                    {
                        //
                        // Deleting File
                        //
                        bool ResultOfDeletingFileInQueue = false;
                        //
                        ResultOfDeletingFileInQueue = WorkingWithFTP.DeletingFile02(PathToFileInQueueOfImporting);
                        //
                        if (ResultOfDeletingFileInQueue == false)
                            RecordingInLogFile(
                                String.Format("ERROR Error Of Deleting File ({0}) In Queue", 
                                Path.GetFileName(PathToFileInQueueOfImporting)));
                    }
                    //
                    // Checking Name Of Imported File
                    //
                    bool NameOfImportedFileIsOK = false;
                    //
                    if (ResultOfDownloading)
                    {
                        if ((Path.GetFileName(PathToImportedFile).ToLower().IndexOf("Data".ToLower()) >= 0) && 
                            (Path.GetFileName(PathToImportedFile).ToLower().IndexOf("_".ToLower()) >= 0))
                        { NameOfImportedFileIsOK = true; }
                    }
                    //
                    // Copying Imported File In Archive Of Importing
                    //
                    if (ResultOfDownloading && NameOfImportedFileIsOK)
                    {
                        //
                        // Creating Path To File In Archive
                        //
                        string PathToImportedFileInArchive = 
                            _PathOfArchivesOfImporting + Path.GetFileName(PathToImportedFile);
                        //
                        // Deleting Imported File If It He Existing In Archive
                        //
                        if (File.Exists(PathToImportedFileInArchive))
                        {
                            //
                            //
                            try { File.Delete(PathToImportedFileInArchive); }
                            catch (Exception E)
                            {
                                RecordingInLogFile(
                                    String.Format("ERROR Ошибка при удалении архивного файла ({0}): {1}", 
                                    Path.GetFileName(PathToImportedFileInArchive), E.Message));
                            }
                        }
                        //
                        // !!!
                        //
                        try { System.IO.File.Copy(PathToImportedFile, PathToImportedFileInArchive); }
                        catch
                        {
                            //
                            System.Threading.Thread.Sleep(509);
                            //
                            try { System.IO.File.Copy(PathToImportedFile, PathToImportedFileInArchive); }
                            catch (Exception E)
                            {
                                RecordingInLogFile(
                                    String.Format("ERROR Ошибка при копировании файла в архив: {0}: {1}",
                                    PathToImportedFileInArchive, E.Message));
                            }
                        }
                    }
                    //
                    // !!!
                    //
                    if (ResultOfDownloading && NameOfImportedFileIsOK)
                    {
                            //
                            // Extraction From Archive
                            //
                            LogsOfMethod.Append("A");
                            if (WorkingWithFiles.ExtractionFromArchive(_PathOfArchivingProgram,
                                PathToImportedFile, _PathOfTMPFolder))
                            {
                                //
                                LogsOfMethod.Append("B");
                                //
                                // Renaming Name Of File
                                //
                                string PathOfFileBIN = System.IO.Path.ChangeExtension(PathToImportedFile, "bin");
                                //
                                // Waiting Of RAR
                                //
                                WorkingWithFiles.WaitingOfExists(PathToImportedFile, 108);
                                WorkingWithFiles.WaitingOfIsAccess(PathToImportedFile, 108);
                                //
                                // Waiting Of BIN
                                //
                                WorkingWithFiles.WaitingOfExists(PathOfFileBIN, 108);
                                WorkingWithFiles.WaitingOfIsAccess(PathOfFileBIN, 108);
                                //
                                // Waiting 
                                //
                                System.Threading.Thread.Sleep(18);
                                //
                                // Deleting Archive
                                //
                                LogsOfMethod.Append("C");
                                WorkingWithFiles.DeletingFile(PathToImportedFile);
                                LogsOfMethod.Append("D");
                                //
                                // Loading BIN File
                                //
                                if (System.IO.File.Exists(PathOfFileBIN))
                                    try
                                    {
                                        //
                                        // Converting BIN To DataSet
                                        //
                                        LogsOfMethod.Append("E");
                                        //
                                        System.IO.FileStream FS =
                                            new System.IO.FileStream(
                                                PathOfFileBIN, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                        DataSet DataFromDrugstore = WorkingWithFiles.Loading(FS);
                                        FS.Close();
                                        //
                                        // Importing Of Data
                                        //
                                        LogsOfMethod.Append("F");
                                        //
                                        try { ImportingOfData.ImportingDataFromDrugstore(DataFromDrugstore); }
                                        catch (Exception E)
                                        {
                                            RecordingInLogFile(
                                                String.Format("ERROR Ошибка при импортировании данных файла: {0}: {1}",
                                                PathOfFileBIN, E.Message));
                                        }
                                        //
                                        LogsOfMethod.Append("G");
                                        //
                                        // Waiting Of BIN
                                        //
                                        WorkingWithFiles.WaitingOfIsAccess(PathOfFileBIN, 108);
                                        //
                                        // Deleting Of BIN
                                        //
                                        WorkingWithFiles.DeletingFile(PathOfFileBIN);
                                        //
                                        LogsOfMethod.Append("H");
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(String.Format("Ошибка при конвертации файла {0}: {1}",
                                            PathOfFileBIN, E.Message));
                                    }
                                else
                                    RecordingInLogFile(
                                        String.Format("ERROR Not Exists File: {0}", PathOfFileBIN));
                            }
                            else
                            {
                                //
                                RecordingInLogFile(
                                    String.Format("ERROR Error Of Unarchiving File: {0}", PathToImportedFile));
                            }
                        
                    }
                    /*else
                    {
                        //
                        RecordingInLogFile("Failure Of Importing");
                    }*/
                    //
                    RecordingInLogFile("");
                    //
                    // Collect Dust
                    //
                    GC.Collect();
                }
                //
                // Recording Pause
                //
                RecordingInLogFile("");
            }
            //
            // Collect Dust
            //
            GC.Collect();
            //
            // Return
            //
            //return LogsOfMethod;
        }

        // Downloading PriceLists Of Private Importings
        public void DownloadingPriceListsOfPrivateImportings()
        {
            //
            // Recording In Log File
            //
            RecordingInLogFile("Downloading PriceLists Of Private Importings");
            RecordingInLogFile("");
            //
            // Getting List Of Private Importers
            //
            ImportingOfData.PrivateImporter[] ListOfPrivateImporters = 
                ImportingOfData.GettingListOfPrivateImporters();
            //
            // Checking Of Activity Of Importers
            //
            bool ExistenceOfActivityOfImporters = false;
            foreach (ImportingOfData.PrivateImporter CurrentImporter in ListOfPrivateImporters)
                if (CurrentImporter.Active) { ExistenceOfActivityOfImporters = true; break; }
            //
            if (!ExistenceOfActivityOfImporters)
            {
                RecordingInLogFile("No Active Of Importers");
                RecordingInLogFile("");
            }
            //
            // Scaning Of Private Importers
            //
            foreach (ImportingOfData.PrivateImporter CurrentImporter in ListOfPrivateImporters)
                if (CurrentImporter.Active)
                {
                    //
                    // Recording In Log File
                    //
                    RecordingInLogFile(String.Format("NameOfImporter: {0} MaskOfFileOfImporting: {1} UseOfRecoding: {2}", 
                        CurrentImporter.NameOfImporter, 
                        CurrentImporter.MaskOfFileOfImporting, 
                        CurrentImporter.UseOfRecoding));
                    //
                    // Getting List Of Files Of PriceLists Of Importer
                    //
                    string PathToListOfPriceListsOfImporter = 
                        String.Format("{0}{1}", 
                        (CurrentImporter.UseOfSystemLogin) ? _AccessToStoreOfData : "", 
                        CurrentImporter.PathOfImporting);
                    //
                    ArrayList ListOfFilesOfPriceListsOfImporter = 
                        WorkingWithFTP.GettingListOfDirectory03(PathToListOfPriceListsOfImporter, false);
                    //
                    // Checking Count Of Files Of PriceLists Of Importer
                    //
                    RecordingInLogFile(
                        String.Format("Count Of Files Of PriceLists: {0}", 
                        ListOfFilesOfPriceListsOfImporter.Count));
                    //
                    // Scaning List Of Files Of PriceLists (Checking Mask Of File Of Importing)
                    //
                    foreach (string CurrentNameOfFileOfPriceList in ListOfFilesOfPriceListsOfImporter)
                        if (CurrentNameOfFileOfPriceList.IndexOf(CurrentImporter.MaskOfFileOfImporting) > -1)
                        {
                            //
                            // Creating Paths To File Of PriceList
                            //
                            string PathToFileInStorage =
                                String.Format("{0}{1}",
                                (CurrentImporter.UseOfSystemLogin) ? _AccessToStoreOfData : "",
                                CurrentImporter.PathOfImporting)
                                +
                                CurrentNameOfFileOfPriceList;
                            string PathToDownloadedFile =
                                String.Format("{0}{1}", _PathOfTMPFolder, CurrentNameOfFileOfPriceList);
                            //
                            // Deleting Imported File If It He Exists
                            //
                            bool NotExistsSimilarFile = true;
                            //
                            if (File.Exists(PathToDownloadedFile))
                            {
                                try { File.Delete(PathToDownloadedFile); }
                                catch (Exception E)
                                {
                                    //
                                    this.RecordingInLogFile(
                                      String.Format("ERROR Error At Deleting File Of Similar ({0}): {1}",
                                      PathToDownloadedFile, E.Message));
                                    //
                                    NotExistsSimilarFile = false;
                                }
                            }
                            //
                            // Downloading File From Storage
                            //
                            bool ResultOfDownloading = false;
                            //
                            if (NotExistsSimilarFile)
                            {
                                //
                                DateTime TimeOfBeginningOfDownloading = new DateTime(1947, 07, 02);
                                DateTime TimeOfEndingOfDownloading = new DateTime(1947, 07, 02);
                                //
                                RecordingInLogFile(
                                    String.Format("Starting Downloading Of File ({0})",
                                    Path.GetFileName(PathToFileInStorage)));
                                TimeOfBeginningOfDownloading = DateTime.Now;
                                //
                                try
                                {
                                    ResultOfDownloading =
                                        WorkingWithFTP.DownloadingFile02(
                                        PathToFileInStorage, PathToDownloadedFile, false);
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Error Of Downloading File ({0}): {1}",
                                        Path.GetFileName(PathToFileInStorage), E.Message));
                                }
                                //
                                RecordingInLogFile(
                                    String.Format("Stoping Downloading Of File ({0})",
                                    Path.GetFileName(PathToFileInStorage)));
                                TimeOfEndingOfDownloading = DateTime.Now;
                                //
                                if (ResultOfDownloading == false)
                                    RecordingInLogFile("ERROR Fatal Error Of Downloading");
                                //
                                if (ResultOfDownloading)
                                {
                                    //
                                    TimeSpan TimeOfDownloading = TimeOfEndingOfDownloading.Subtract(TimeOfBeginningOfDownloading);
                                    //
                                    if (TimeOfDownloading.TotalSeconds > 10)
                                    { WorkingWithFTP.ResetOfResponseOfDownloadingFile(); }
                                }
                                //
                                // Checking Of Existence Of File (Move To DownloadingFile02)
                                //
                                if (ResultOfDownloading == true)
                                    ResultOfDownloading = File.Exists(PathToDownloadedFile);
                            }
                            //
                            // Waiting Of Downloaded File
                            //
                            if (ResultOfDownloading)
                            {
                                WorkingWithFiles.WaitingOfIsAccess(PathToDownloadedFile, 80);
                                //
                                // Waiting
                                //
                                System.Threading.Thread.Sleep(509);
                            }
                            //
                            // Deleting File From Storage
                            //
                            if (ResultOfDownloading)
                            {
                                //
                                // Deleting File
                                //
                                bool ResultOfDeletingFromStorage = false;
                                //
                                ResultOfDeletingFromStorage =
                                    WorkingWithFTP.DeletingFile02(PathToFileInStorage);
                                //
                                if (ResultOfDeletingFromStorage == false)
                                    RecordingInLogFile(
                                        String.Format("ERROR Error Of Deleting File ({0}) In Storage",
                                        Path.GetFileName(PathToFileInStorage)));
                            }
                            //
                            // Converting Of File Of Price List
                            //
                            if (ResultOfDownloading)
                            {
                                //
                                Drugstore.ConvertingOfPriceList Convernt =
                                    new Drugstore.ConvertingOfPriceList(this.PathToLogFile);
                                //
                                Convernt.IDOfPharmacy = 12004;
                                Convernt.MaskOfFullPriceList = CurrentImporter.MaskOfFileOfImporting.ToLower();
                                Convernt.MaskOfReceipts = "prixod";
                                Convernt.MaskOfDeleting = "raxsod";
                                Convernt.UseOfIDOfPriceList = true;
                                //
                                // Converting
                                //
                                bool ResultOfConverting = false;
                                //
                                DataTable PriceListOfImporter = new DataTable("PriceListOfImporter");
                                //
                                try
                                {
                                    //
                                    PriceListOfImporter = Convernt.Converting(PathToDownloadedFile);
                                    PriceListOfImporter.AcceptChanges();
                                    //
                                    RecordingInLogFile(
                                        String.Format("Count Of Converting Prices Of PriceList: {0}",
                                        PriceListOfImporter.Rows.Count));
                                    //
                                    ResultOfConverting = true;
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Error Of Converting File Of PriceList ({0}): {1}",
                                        PathToDownloadedFile, E.Message));
                                }
                                //
                                RecordingInLogFile("");
                                //
                                // Recoding IDs Of Drugstore Of PriceList
                                //
                                if ((ResultOfConverting) &&
                                    (CurrentImporter.UseOfRecoding) &&
                                    (CurrentImporter.ExistenceOfRecodingIDs) &&
                                    (PriceListOfImporter.Columns.Contains("ID_PH")) &&
                                    (PriceListOfImporter.Rows.Count > 0))
                                {
                                    //
                                    RecordingInLogFile("Use Of Recoding IDs Of Drugstore");
                                    //
                                    ImportingOfData.PrivateImporter.RecodingID[] RecodingIDs =
                                        CurrentImporter.GettingRecodingIDs;
                                    //
                                    int CountOfDeletingPrices = 0;
                                    foreach (DataRow CurrentPrice in PriceListOfImporter.Rows)
                                    {
                                        //
                                        int IDOfDrugstore = ((int)CurrentPrice["ID_PH"]);
                                        //
                                        bool Recoded = false;
                                        foreach (ImportingOfData.PrivateImporter.RecodingID CurrentRecoding in RecodingIDs)
                                            if (IDOfDrugstore == CurrentRecoding.IDOfImporter)
                                            {
                                                IDOfDrugstore = CurrentRecoding.IDOfSystem;
                                                Recoded = true;
                                                break;
                                            }
                                        //
                                        if (Recoded)
                                            CurrentPrice["ID_PH"] = IDOfDrugstore;
                                        else
                                        { CurrentPrice.Delete(); CountOfDeletingPrices++; }
                                    }
                                    //
                                    PriceListOfImporter.AcceptChanges();
                                    //
                                    RecordingInLogFile(
                                        String.Format("Count Of Deleting Prices By Bad IDs Drugstore: {0}",
                                        CountOfDeletingPrices));
                                    //
                                    RecordingInLogFile("");
                                }
                                //
                                // Importing Of Data Of PriceList
                                //
                                if (ResultOfConverting)
                                    try
                                    {
                                        //
                                        RecordingInLogFile(
                                            String.Format("Count Of Importing Prices Of Importer '{0}' From File ({1}): {2}",
                                            CurrentImporter.NameOfImporter,
                                            Path.GetFileName(PathToDownloadedFile),
                                            PriceListOfImporter.Rows.Count));
                                        //
                                        RecordingInLogFile("");
                                        //
                                        ImportingOfData.ImportingOfDataOfPriceList(PriceListOfImporter, 12004);

                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Error Of Importing Prices Of Importer '{0}' From File ({1}): {2}",
                                            CurrentImporter.NameOfImporter, PathToDownloadedFile, E.Message));
                                    }
                                //
                                RecordingInLogFile("");
                                //
                                // Waiting Of Downloaded File
                                //
                                WorkingWithFiles.WaitingOfIsAccess(PathToDownloadedFile, 108);
                                //
                                // Deleting Downloaded File
                                //
                                WorkingWithFiles.DeletingFile(PathToDownloadedFile);
                            }
                        }
                        else { RecordingInLogFile("File Not Corresponds To Mask"); RecordingInLogFile(""); }
                }
        }

        // Downloading Iodine Face From FTP
        public void DownloadingIodineFaceFromFTP()
        {
            //
            RecordingInLogFile("Checking Importing IodineFace");
            //
            ArrayList ListOfImportedFiles = 
                WorkingWithFTP.GettingListOfDirectory03(_PathOfImportingOfIodineFace, false);
            //
            // Checking Of Count Of List Of Files
            //
            if (ListOfImportedFiles.Count > 0)
            {
                //
                // Scaning Of Files Of Importing
                //
                foreach (string CurrentFileName in ListOfImportedFiles)
                {
                    //
                    // Recording In Log File
                    //
                    RecordingInLogFile(String.Format("Importing Of PriceList File: {0}", CurrentFileName));
                    //
                    // Initialization Of Paths
                    //
                    string PathOfFileOnFTP = String.Format("{0}{1}", _PathOfImportingOfIodineFace, CurrentFileName);
                    string PathOfFileTXT = String.Format("{0}{1}", _PathOfTMPFolder, CurrentFileName);
                    //
                    // Downloading File From Importing
                    //
                    if (WorkingWithFTP.DownloadingFile(PathOfFileOnFTP, PathOfFileTXT, false))
                    {
                        //
                        // Waiting Of TXT
                        //
                        WorkingWithFiles.WaitingOfExists(PathOfFileTXT, 108);
                        WorkingWithFiles.WaitingOfIsAccess(PathOfFileTXT, 108);
                        //
                        // Deleting Of File On FTP
                        //
                        WorkingWithFTP.DeletingFile(PathOfFileOnFTP);
                        //
                        try
                        {
                            //
                            Drugstore.ConvertingOfPriceList Convernt =
                                new Drugstore.ConvertingOfPriceList(this.PathToLogFile);
                            //
                            Convernt.IDOfPharmacy = 12004;
                            Convernt.MaskOfFullPriceList = "nal";
                            Convernt.MaskOfReceipts = "prixod";
                            Convernt.MaskOfDeleting = "raxsod";
                            Convernt.UseOfIDOfPriceList = true;
                            //
                            // Converting TXT File In Table
                            //
                            DataTable TableOfPriceList = Convernt.Converting(PathOfFileTXT);
                            //
                            try
                            {
                                //int CountOfRows = -1;CountOfRows = 
                                //
                                ImportingOfData.ImportingOfDataOfPriceList(TableOfPriceList, 12004);
                                //
                                RecordingInLogFile(String.Format("Импортировано строк Йодлика: {0}", -1));
                            }
                            catch (Exception E)
                            {
                                RecordingInLogFile(
                                    String.Format("Ошибка при импортировании данных файла: {0}: {1}",
                                    PathOfFileTXT, E.Message));
                            }
                        }
                        catch (Exception E)
                        {
                            RecordingInLogFile(
                                String.Format("Ошибка при конвертации файла: {0}: {1}",
                                PathOfFileTXT, E.Message));
                        }
                        //
                        // Waiting Of TXT File
                        //
                        WorkingWithFiles.WaitingOfIsAccess(PathOfFileTXT, 108);
                        //
                        // Deleting Of TXT
                        //
                        WorkingWithFiles.DeletingFile(PathOfFileTXT);
                    }
                }
            }
            //
            RecordingInLogFile(" ");
            //
            GC.Collect();
        }

        #endregion

        #region ' Loading Updatings Of Data To Queues '

        // Loading Data To FTP
        public void LoadingDataToFTP()
        {
            //
            //RecordingInLogFile("Checking Exporting Of Data");
            //
            DataSet DataForExporting = ExportingOfData.Exporting();
            //
            // !!!
            //
            int CountOfExportedRows = 
                DataForExporting.Tables["Pharmacy"].Rows.Count +
                DataForExporting.Tables["GroupsOfProducts"].Rows.Count + 
                DataForExporting.Tables["Products"].Rows.Count + 
                DataForExporting.Tables["Announcements"].Rows.Count;
            
            if (CountOfExportedRows > 0)
            {
                //
                RecordingInLogFile("Checking Exporting Of Data");
                //
                string NameOfUpdating = DataForExporting.Tables["NumberOfExported"].Rows[0][0].ToString();
                //
                // Recording In Log File
                //
                RecordingInLogFile(
                    String.Format("Number Of Exported: {0}, Count Of Exported - PH: {1} PR: {2}", 
                    NameOfUpdating, 
                    DataForExporting.Tables["Pharmacy"].Rows.Count, 
                    DataForExporting.Tables["Products"].Rows.Count));
                //
                // Renaming DataSet
                //
                DataForExporting.DataSetName = 
                    WorkingWithFiles.CreatingNameOfFile(
                    "Updating", 
                    ((DateTime)DataForExporting.Tables["DateOfExported"].Rows[0][0]));
                //
                // Initialization Of Paths
                //
                string PathOfFileBIN = String.Format("{0}{1}.bin", _PathOfTMPFolder, NameOfUpdating);
                string PathOfFileRAR = System.IO.Path.ChangeExtension(PathOfFileBIN, "rar");
                string PathOfFileOnFTP = String.Format("{0}{1}.rar", _PathOfExportingDataOfFTP, NameOfUpdating);
                //
                // Saving Data In File
                //
                try
                {
                    System.IO.FileStream FS = new System.IO.FileStream(PathOfFileBIN, System.IO.FileMode.Create,
                        System.IO.FileAccess.Write);
                    WorkingWithFiles.Saving(DataForExporting, (System.IO.Stream)FS);
                    FS.Close();
                }
                catch (Exception E)
                {
                    this.RecordingInLogFile(
                        String.Format("Ошибка при создании файла: {0}: {1}", PathOfFileBIN, E.Message));
                }
                //
                // Checking Of File BIN
                //
                WorkingWithFiles.WaitingOfExists(PathOfFileBIN, 40);
                WorkingWithFiles.WaitingOfIsAccess(PathOfFileBIN, 40);
                //
                if (System.IO.File.Exists(PathOfFileBIN))
                    if (WorkingWithFiles.IsAccessFile(PathOfFileBIN))
                        if (WorkingWithFiles.ArchivingFile(_PathOfArchivingProgram, PathOfFileRAR, PathOfFileBIN))
                        {
                            //
                            // Checking Of File RAR
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 40);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 40);
                            //
                            // Checking Of File BIN
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileBIN, 40);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileBIN, 40);
                            //
                            // Deleting Of BIN File
                            //
                            WorkingWithFiles.DeletingFile(PathOfFileBIN);
                            //
                            // Uploading RAR File
                            //
                            if (WorkingWithFiles.IsAccessFile(PathOfFileRAR))
                            {
                                // false true
                                if (WorkingWithFTP.UploadingFile(PathOfFileRAR, PathOfFileOnFTP, false))
                                {
                                    //
                                    this.RecordingInLogFile("Loading File To FTP");
                                    //
                                    // Checking Of File RAR
                                    //
                                    WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 40);// ???
                                    WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 40);
                                    //
                                    // Deleting Of RAR File
                                    //
                                    WorkingWithFiles.DeletingFile(PathOfFileRAR);
                                    //
                                    // Updating Data Of Exporting
                                    //
                                    ExportingOfData.UpdatingDateOfExporting(((DateTime)DataForExporting.Tables["DateOfExported"].Rows[0][0]));
                                    ExportingOfData.IncrementOfNumberOfExported();
                                }
                                else
                                {
                                    //
                                    this.RecordingInLogFile(
                                        String.Format("Не успешная загрузка файла '{0}' на FTP Сервер", PathOfFileRAR));
                                    //
                                    // Checking Existing File
                                    //
                                    bool ExistingFile = false;
                                    ArrayList ListOfFilesExported = 
                                        WorkingWithFTP.GettingListOfDirectory03(_PathOfExportingDataOfFTP, false);
                                    //
                                    foreach (string CurrentName in ListOfFilesExported)
                                        if (Path.GetFileNameWithoutExtension(CurrentName) == NameOfUpdating)
                                            ExistingFile = true;
                                    //
                                    if (ExistingFile)
                                    {
                                        //
                                        this.RecordingInLogFile(String.Format("Файл '{0}' уже находится на FTP", NameOfUpdating));
                                        this.RecordingInLogFile(String.Format("Removing File '{0}'", NameOfUpdating));
                                        //
                                        WorkingWithFTP.DeletingFile02(PathOfFileOnFTP);
                                    }
                                }
                            }
                            else
                            {
                                //
                                this.RecordingInLogFile(
                                        String.Format("Ошибка при проверке файла '{0}'. Файл занят.", PathOfFileRAR));
                            }
                        }
                //
                RecordingInLogFile("");
                RecordingInLogFile("");
            }
            //
            //RecordingInLogFile("");
        }

        // Loading Exported File Of PriceLists
        public void LoadingExportedFileOfPriceLists()
        {
            try
            {
                //
                // Checking Of Existence Exported Prices
                //
                bool ExistenceExportedPrices = ExportingOfData.CheckingOfExistenceExportedPrices();
                //
                // Exporting
                //
                if (ExistenceExportedPrices)
                {
                    //
                    // Getting Number And Date
                    //
                    int NumberOfExportedPriceLists = ExportingOfData.GettingNumberOfExportedPriceLists();
                    //
                    DateTime LastDateOfUpdatingPriceLists = ExportingOfData.GettingDateOfUpdatingPriceLists();
                    //
                    // Checking Of Data About Exporting
                    //
                    bool CheckedDataOfExporting = false;
                    //
                    if ((NumberOfExportedPriceLists > 0) &&
                        (LastDateOfUpdatingPriceLists > new DateTime(1949, 01, 01)))
                    { CheckedDataOfExporting = true; }
                    //
                    // Exporting Data
                    //
                    if (CheckedDataOfExporting)
                    {
                        //
                        NumberOfExportedPriceLists++;
                        //
                        // Recording In Log File
                        //
                        RecordingInLogFile(
                            String.Format("Exporting Of PriceLists (Number Of Exporting: {0}, Date Of Exporting: {1})",
                            NumberOfExportedPriceLists, LastDateOfUpdatingPriceLists));
                        //
                        // Creating Paths
                        //
                        string PathOfFileTXT = 
                            String.Format("{0}{1}.txt", _PathOfTMPFolder, NumberOfExportedPriceLists);
                        string PathOfFileRAR = 
                            System.IO.Path.ChangeExtension(PathOfFileTXT, "rar");
                        string PathOfFileOnFTP = 
                            String.Format("{0}{1}.rar", _PathToExportingPriceListsOnFTP, NumberOfExportedPriceLists);
                        //
                        // Exporting Of PriceLists To File Of PriceLists
                        //
                        ExportingOfData.ExportingOfPriceLists(PathOfFileTXT);
                        //
                        // Checking Of File Of PriceLists
                        //
                        WorkingWithFiles.WaitingOfExists(PathOfFileTXT, 800);
                        WorkingWithFiles.WaitingOfIsAccess(PathOfFileTXT, 800);
                        //
                        // Archiving Of File Of PriceLists
                        //
                        bool ResultOfArchiving = false;
                        //
                        if (System.IO.File.Exists(PathOfFileTXT))
                            if (WorkingWithFiles.IsAccessFile(PathOfFileTXT))
                                ResultOfArchiving = 
                                    WorkingWithFiles.ArchivingFile(_PathOfArchivingProgram, PathOfFileRAR, PathOfFileTXT);
                        //
                        // Loading
                        //
                        if (ResultOfArchiving)
                        {
                            //
                            // Checking Of File RAR
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 800);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 800);
                            //
                            // Checking Of File TXT
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileTXT, 800);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileTXT, 800);
                            //
                            // Sleep 108
                            //
                            System.Threading.Thread.Sleep(1108); //1108
                            //
                            // Deleting Of TXT File
                            //
                            WorkingWithFiles.DeletingFile(PathOfFileTXT);
                            //
                            // Uploading RAR File
                            //
                            bool ResultOfUploading = false;
                            //
                            if (WorkingWithFiles.IsAccessFile(PathOfFileRAR))
                            {
                                //
                                RecordingInLogFile("Starting Of Uploading");
                                //
                                ResultOfUploading = 
                                  WorkingWithFTP.UploadingFile(PathOfFileRAR, PathOfFileOnFTP, false);
                                //
                                RecordingInLogFile("Stoping Of Uploading");
                            }
                            else
                            {
                                this.RecordingInLogFile(
                                        String.Format(
                                        "ERROR Ошибка при загрузке файла '{0}' на FTP. Нет достутпа к файлу.", 
                                        PathOfFileRAR));
                            }
                            //
                            // Processing Of Result Of Uploading
                            //
                            if (ResultOfUploading)
                            {
                                //
                                this.RecordingInLogFile("File Is Uploaded (Deleting Exported File)");
                                //
                                // Increment Of Number Of Exported PriceLists
                                //
                                ExportingOfData.IncrementOfNumberOfExportedPriceLists(LastDateOfUpdatingPriceLists);
                            }
                            //
                            // Deleting File For Uploading
                            //
                            if (File.Exists(PathOfFileRAR))
                            {
                                //
                                // Checking Of File RAR
                                //
                                WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 800);
                                //
                                // Sleep 108
                                //
                                System.Threading.Thread.Sleep(1108);
                                //
                                // Deleting Of RAR File
                                //
                                WorkingWithFiles.DeletingFile(PathOfFileRAR);
                            }
                            //
                            // !!!
                            //
                            if (!ResultOfUploading)
                            {
                                //
                                this.RecordingInLogFile(
                                    String.Format("ERROR Не успешная загрузка файла '{0}' на FTP Сервер", 
                                    PathOfFileRAR));
                            }
                        }
                        else { RecordingInLogFile("ERROR Неуспешная процедура архивирования файла."); }
                    }
                    //
                    // RecordingInLogFile
                    //
                    RecordingInLogFile("");
                    RecordingInLogFile("");
                }
            }
            catch (Exception E)
            { RecordingInLogFile(String.Format("ERROR Ошибка в методе LoadingExportedFileOfPriceLists: {0}", E.Message)); }
        }

        // Loading Scripts To FTP
        public void LoadingScriptsToFTP()
        {
            //
            RecordingInLogFile("Checking Exporting Of Scripts");
            //
            DateTime CurrentDate = ExportingOfData.GettingCurrentDate();
            //
            string StringOfScripts = ExportingOfData.ExportingOfScriptsOfData(CurrentDate);
            //
            if (StringOfScripts != "")
            {
                //
                // Recording In Log File
                //
                RecordingInLogFile(String.Format("Date Of Exported: {0}", CurrentDate));
                //
                // Initialization Of Paths
                //
                string BasePart =
                    String.Format(
                    "Update{0}{1}{2}_{3}{4}",
                    CurrentDate.Year, CurrentDate.Month.ToString("00"), CurrentDate.Day.ToString("00"),
                    CurrentDate.Hour.ToString("00"), CurrentDate.Minute.ToString("00"),
                    CurrentDate.Second.ToString("00"));
                //
                string PathOfFileSQL = String.Format("{0}{1}.sql", _PathOfTMPFolder, BasePart);
                string PathOfFileRAR = System.IO.Path.ChangeExtension(PathOfFileSQL, "rar");
                string PathOfFileOnFTP = String.Format("{0}{1}.rar", _PathOfExportingUpdatingAllSites, BasePart);
                //
                // Saving Scripts In File
                //
                try
                {
                    System.IO.FileStream FS = new System.IO.FileStream(PathOfFileSQL, System.IO.FileMode.Create,
                        System.IO.FileAccess.Write);
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(FS);
                    SW.WriteLine(StringOfScripts);
                    SW.Close();
                    FS.Close();
                }
                catch (Exception E)
                {
                    this.RecordingInLogFile(
                        String.Format("Ошибка при создании файла: {0}: {1}", PathOfFileSQL, E.Message));
                }
                //
                // Checking Of File SQL
                //
                WorkingWithFiles.WaitingOfExists(PathOfFileSQL, 40);
                WorkingWithFiles.WaitingOfIsAccess(PathOfFileSQL, 40);
                //
                if (System.IO.File.Exists(PathOfFileSQL))
                    if (WorkingWithFiles.IsAccessFile(PathOfFileSQL))
                        if (WorkingWithFiles.ArchivingFile(_PathOfArchivingProgram, PathOfFileRAR, PathOfFileSQL))
                        {
                            //
                            // Checking Of File RAR
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 60);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 60);
                            //
                            // Checking Of File SQL
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileSQL, 60);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileSQL, 60);
                            //
                            // Deleting Of SQL File
                            //
                            WorkingWithFiles.DeletingFile(PathOfFileSQL);
                            //
                            // Uploading RAR File
                            //
                            if (WorkingWithFiles.IsAccessFile(PathOfFileRAR))
                                if (WorkingWithFTP.UploadingFile(PathOfFileRAR, PathOfFileOnFTP, false))
                                {
                                    //
                                    // Checking Of File RAR
                                    //
                                    WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 60); // ???
                                    WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 60);
                                    //
                                    // Deleting Of RAR File
                                    //
                                    WorkingWithFiles.DeletingFile(PathOfFileRAR);
                                    //
                                    // Updating Date Of Exporting Of Scripts
                                    //
                                    ExportingOfData.UpdatingDateOfExportingOfScripts(CurrentDate);
                                }
                        }
            }
            //
            RecordingInLogFile("");
        }

        // Loading Updating Of All Sites
        public void LoadingUpdatingOfAllSites()
        {
            //
            //RecordingInLogFile("Checking Exporting Of Scripts");
            //
            // Getting Current Date
            //
            DateTime CurrentDate = ExportingOfData.GettingCurrentDate();
            //
            // Initializating Of Paths To Files
            //
            string BasePart = 
                String.Format(
                "Update{0}{1}{2}_{3}{4}", 
                CurrentDate.Year, CurrentDate.Month.ToString("00"), CurrentDate.Day.ToString("00"), 
                CurrentDate.Hour.ToString("00"), CurrentDate.Minute.ToString("00"), 
                CurrentDate.Second.ToString("00"));
            //
            string PathOfFileSQL = String.Format("{0}{1}.sql", _PathOfTMPFolder, BasePart);
            string PathOfFileRAR = System.IO.Path.ChangeExtension(PathOfFileSQL, "rar");
            string PathOfFileOnFTP = String.Format("{0}{1}.rar", _PathOfExportingUpdatingAllSites, BasePart);
            //
            // Exporting Of Scripts Of Data 03
            //
            DataTable InformationOfExporting = ExportingOfData.ExportingOfScriptsOfData03(CurrentDate, PathOfFileSQL);
            //
            //if (StringOfScripts != "")
            if (File.Exists(PathOfFileSQL))
            {
                //
                RecordingInLogFile("Checking Exporting Of Scripts");
                //
                // Recording In Log File
                //
                RecordingInLogFile(
                    String.Format(
                    "Count Of Prices: {0} Count Of Changes: {1} Count Of Deleting: {2} Count Of Full Dates: {3} " + 
                    "Date Of Start: {4} Date Of End: {5}", 
                    InformationOfExporting.Rows.Find("CountOfPrices")["Value"], 
                    InformationOfExporting.Rows.Find("CountOfChanges")["Value"], 
                    InformationOfExporting.Rows.Find("CountOfDeleting")["Value"], 
                    InformationOfExporting.Rows.Find("CountOfFullDates")["Value"], 
                    InformationOfExporting.Rows.Find("DateOfStart")["Value"], 
                    InformationOfExporting.Rows.Find("DateOfEnd")["Value"]));
                //
                // Checking Of File SQL
                //
                WorkingWithFiles.WaitingOfExists(PathOfFileSQL, 40);
                WorkingWithFiles.WaitingOfIsAccess(PathOfFileSQL, 40);
                //
                if (System.IO.File.Exists(PathOfFileSQL))
                    if (WorkingWithFiles.IsAccessFile(PathOfFileSQL))
                        if (WorkingWithFiles.ArchivingFile(_PathOfArchivingProgram, PathOfFileRAR, PathOfFileSQL))
                        {
                            //
                            // Checking Of File RAR
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 60);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 60);
                            //
                            // Checking Of File SQL
                            //
                            WorkingWithFiles.WaitingOfExists(PathOfFileSQL, 60);
                            WorkingWithFiles.WaitingOfIsAccess(PathOfFileSQL, 60);
                            //
                            // Deleting Of SQL File
                            //
                            WorkingWithFiles.DeletingFile(PathOfFileSQL);
                            //
                            // Uploading RAR File
                            //
                            // false true
                            if (WorkingWithFiles.IsAccessFile(PathOfFileRAR))
                                if (WorkingWithFTP.UploadingFile(PathOfFileRAR, PathOfFileOnFTP, false))
                                {
                                    //
                                    // Checking Of File RAR
                                    //
                                    WorkingWithFiles.WaitingOfExists(PathOfFileRAR, 60); // ???
                                    WorkingWithFiles.WaitingOfIsAccess(PathOfFileRAR, 60);
                                    //
                                    // Deleting Of RAR File
                                    //
                                    WorkingWithFiles.DeletingFile(PathOfFileRAR);
                                    //
                                    // Updating Date Of Exporting Of Scripts
                                    //
                                    ExportingOfData.UpdatingDateOfExportingOfScripts(CurrentDate);
                                }
                        }
            }
            //
            RecordingInLogFile("");
        }

        // Loading Updating Of One Site
        public void LoadingUpdatingOfOneSite()
        {
            //
            // Getting Dates
            //
            DateTime DateOfLastExport = ExportingOfData.GettingDate(9);
            DateTime CurrentDateOfStorage = ExportingOfData.GettingCurrentDateOfStorage();
            //
            // Creating Paths
            //
            string BaseComponent = 
                String.Format(
                "Updating_{0}{1}{2}_{3}{4}", 
                CurrentDateOfStorage.Year, 
                CurrentDateOfStorage.Month.ToString("00"), 
                CurrentDateOfStorage.Day.ToString("00"), 
                CurrentDateOfStorage.Hour.ToString("00"), 
                CurrentDateOfStorage.Minute.ToString("00"), 
                CurrentDateOfStorage.Second.ToString("00"));
            //
            string PathToFileOfExport = String.Format("{0}{1}.sql", _PathOfTMPFolder, BaseComponent);
            string PathToFileInArchive = String.Format("{0}{1}.rar", _PathOfTMPFolder, BaseComponent);
            string PathToFileInQueue = String.Format("{0}{1}.rar", _PathOfExportingUpdatingOneSite, BaseComponent);
            //
            // Checking Dates
            //
            bool CorrectDates = true;
            //
            if ((DateOfLastExport.Year == 1947) || (CurrentDateOfStorage.Year == 1947))
                CorrectDates = false;
            //
            // Exporting Stored Procedures Of Updating
            //
            if(CorrectDates)
                ExportingOfData.ExportingStoredProceduresOfUpdating(
                    PathToFileOfExport, DateOfLastExport, CurrentDateOfStorage);
            //
            // Checking Existence Exported File
            //
            bool ExistenceExportedFile = false;
            //
            if (File.Exists(PathToFileOfExport))
            { ExistenceExportedFile = true; }
            //
            // Archiving And Loading Exported File
            //
            if (ExistenceExportedFile)
            {
                //
                // Waiting Of Exporting File
                //
                WorkingWithFiles.WaitingOfExists(PathToFileOfExport, 40);
                WorkingWithFiles.WaitingOfIsAccess(PathToFileOfExport, 40);
                //
                // Checking Of Exported File
                //
                bool ExportedFileIsFree = false;
                //
                if (File.Exists(PathToFileOfExport))
                    if (WorkingWithFiles.IsAccessFile(PathToFileOfExport))
                    { ExportedFileIsFree = true; }
                //
                // Archiving
                //
                bool ResultOfArchiving = false;
                //
                if(ExportedFileIsFree)
                    ResultOfArchiving = WorkingWithFiles.ArchivingFile(
                        _PathOfArchivingProgram, PathToFileInArchive, PathToFileOfExport);
                //
                // Waiting Of Files
                //
                if (ResultOfArchiving)
                {
                    //
                    // Waiting Of Archived File
                    //
                    WorkingWithFiles.WaitingOfExists(PathToFileInArchive, 60);
                    WorkingWithFiles.WaitingOfIsAccess(PathToFileInArchive, 60);
                    //
                    // Waiting Of Exporting File
                    //
                    WorkingWithFiles.WaitingOfExists(PathToFileOfExport, 60);
                    WorkingWithFiles.WaitingOfIsAccess(PathToFileOfExport, 60);
                    //
                    // Deleting Of Exporting File
                    //
                    WorkingWithFiles.DeletingFile(PathToFileOfExport);
                }
                //
                // Loading File In Queue Of Updatings
                //
                if(ResultOfArchiving)
                {
                    //
                    // Uploading Archived File
                    //
                    bool ResultOfUploadingFile = false;
                    //
                    if (WorkingWithFiles.IsAccessFile(PathToFileInArchive))
                        ResultOfUploadingFile = 
                            WorkingWithFTP.UploadingFile(PathToFileInArchive, PathToFileInQueue, true);
                    //
                    // !!!
                    //
                    if (ResultOfUploadingFile)
                    {
                        //
                        // Waiting Of Archived File
                        //
                        WorkingWithFiles.WaitingOfExists(PathToFileInArchive, 60); // ???
                        WorkingWithFiles.WaitingOfIsAccess(PathToFileInArchive, 60);
                        //
                        // Deleting Of Archived File
                        //
                        WorkingWithFiles.DeletingFile(PathToFileInArchive);
                        //
                        // Updating Date Of Exporting Of Updatings For One Site
                        //
                        ExportingOfData.UpdatingDate(9, CurrentDateOfStorage);
                    }
                }
            }
            //
            RecordingInLogFile("");
        }

        #endregion

    }
}