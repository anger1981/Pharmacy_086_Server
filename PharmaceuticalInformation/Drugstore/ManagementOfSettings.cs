using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ManagementOfSettings : BaseType
    {

        #region ' Fields '

        //private PharmaceuticalInformation.Service.Reportings Reportings;
        private string PathToSettingsFile;
        private DataSet SettingsOfMedicalPharm;
        private DataSet BackupCopySettingsOfMedicalPharm;
        private string[] _IntervalsOfUpdating;
        //
        public static string VersionOfApplication = "1.7.8.0";

        #endregion

        #region ' Designer '

        public ManagementOfSettings(string PathToSettingsFile, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initializing Fields
            //
            this.PathToSettingsFile = PathToSettingsFile;
            //this.Reportings = Reportings;
            //
            // Initializing Of Intervals Of Updating
            //
            _IntervalsOfUpdating = new string[10]
            {
                "(Не обновлять)", 
                "3 минуты", "10 минут", "30 минут", 
                "1 час", "3 часа", "6 часов", 
                "8 часов", "12 часов", "24 часа" 
            };
            //
            // Reading Of Settings
            //
            bool SuccessfulPerusal = false;
            //
            if (System.IO.File.Exists(PathToSettingsFile))
            {
                //
                // Reading Settings
                //
                try
                {
                    //
                    // Creating FileStream
                    //
                    FileStream FS = new FileStream(PathToSettingsFile, FileMode.Open, FileAccess.Read);
                    //
                    // Deserializing Settings
                    //
                    try
                    {
                        //
                        SettingsOfMedicalPharm = (DataSet)new BinaryFormatter().Deserialize(FS);
                        //
                        SuccessfulPerusal = true;
                    }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при чтении файла настроек", E, true); }
                    //
                    // Closing FileStream
                    //
                    try
                    { FS.Close(); }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при закрытии файла настроек", E, false); }
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при открытии файла настроек", E, true); }
            }
            else if(false)
            {
                //
                // Recording In Reports
                //
                this.RecordingInLogFile("Нет файла настроек. Поиск файла настроек.");
                //
                // Searching And Copying Settings
                //
                try
                {
                    //
                    // Getting List Of Files Of Settings
                    //
                    ArrayList ListOfFilesOfSettings = new ArrayList();
                    //
                    string PathOfDirectoryOfApplication = Path.GetDirectoryName(PathToSettingsFile);
                    string PathOfParentDirectory = Path.GetDirectoryName(PathOfDirectoryOfApplication);
                    string[] DirectoriesForSearching = Directory.GetDirectories(PathOfParentDirectory);
                    //
                    foreach (string CurrentDirectory in DirectoriesForSearching)
                        foreach (string CurrentFile in Directory.GetFiles(CurrentDirectory, "Settings.bin"))
                            ListOfFilesOfSettings.Add(CurrentFile);
                    //
                    this.RecordingInLogFile(
                        String.Format("Count Of ListOfFilesOfSettings: {0} ", 
                        ListOfFilesOfSettings.Count));
                    //
                    // Searching Files Of Settings
                    //
                    DataSet[] ListOfSettings = new DataSet[ListOfFilesOfSettings.Count];
                    //
                    int NumberOfFile = -1;
                    //
                    foreach (string CurrentFile in ListOfFilesOfSettings)
                    {
                        //
                        NumberOfFile++;
                        //
                        ListOfSettings[NumberOfFile] = null;
                        //
                        try
                        {
                            //
                            FileStream FS = new FileStream(CurrentFile, FileMode.Open, FileAccess.Read);
                            //
                            try { ListOfSettings[NumberOfFile] = ((DataSet)new BinaryFormatter().Deserialize(FS)); }
                            catch (Exception E)
                            { this.ReturningMessageAboutError("Ошибка при чтении найденного файла настроек", E, true); }
                            //
                            try { FS.Close(); }
                            catch (Exception E)
                            { this.ReturningMessageAboutError("Ошибка при закрытии найденного файла настроек", E, true); }
                        }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при открытии найденного файла настроек", E, true); }
                    }
                    //
                    // Selection Of File Of Settings
                    //
                    int IndexOfSettings = -1;
                    //
                    for (int i = 0; i < ListOfSettings.Length; i++)
                        if (ListOfSettings[i] != null)
                        {
                            try
                            {
                                //
                                DataRow VersionOfApplication = 
                                    ListOfSettings[i].Tables["InformationOfSettings"].Rows.Find("VersionOfApplication");
                                if (VersionOfApplication != null)
                                {
                                    //
                                    string StringOfVersionOfApplication = VersionOfApplication["Value"].ToString();
                                    if (
                                        /*
                                        (StringOfVersionOfApplication == "1.5") ||
                                        (StringOfVersionOfApplication == "1.5.0.1") ||
                                        (StringOfVersionOfApplication == "1.5.0.2") ||
                                        (StringOfVersionOfApplication == "1.5.0.3") ||
                                        (StringOfVersionOfApplication == "1.5.0.4") ||
                                        (StringOfVersionOfApplication == "1.5.0.5") ||
                                        (StringOfVersionOfApplication == "1.5.0.6") ||
                                        (StringOfVersionOfApplication == "1.5.0.7") ||
                                        (StringOfVersionOfApplication == "1.5.0.8") ||
                                        (StringOfVersionOfApplication == "1.5.0.9") ||
                                        (StringOfVersionOfApplication == "1.5.1.0") ||
                                        (StringOfVersionOfApplication == "1.5.1.1") ||
                                        (StringOfVersionOfApplication == "1.5.1.2") ||
                                        (StringOfVersionOfApplication == "1.5.1.3") ||
                                        (StringOfVersionOfApplication == "1.5.1.4") ||
                                        (StringOfVersionOfApplication == "1.5.1.5") ||
                                        (StringOfVersionOfApplication == "1.5.1.6") ||
                                        (StringOfVersionOfApplication == "1.5.1.7") ||
                                        */
                                        (StringOfVersionOfApplication == "1.6.0.0") ||
                                        (StringOfVersionOfApplication == "1.6.1.0") ||
                                        (StringOfVersionOfApplication == "1.6.2.0") ||
                                        (StringOfVersionOfApplication == "1.6.3.0") ||
                                        (StringOfVersionOfApplication == "1.6.4.0") ||
                                        (StringOfVersionOfApplication == "1.6.5.0") ||
                                        (StringOfVersionOfApplication == "1.6.6.0") ||
                                        (StringOfVersionOfApplication == "1.6.7.0") ||
                                        (StringOfVersionOfApplication == "1.6.8.0") ||
                                        (StringOfVersionOfApplication == "1.6.9.0") ||
                                        (StringOfVersionOfApplication == "1.6.10.0") ||
                                        (StringOfVersionOfApplication == "1.7.0.0") ||
                                        (StringOfVersionOfApplication == "1.7.1.0") ||
                                        (StringOfVersionOfApplication == "1.7.2.0") ||
                                        (StringOfVersionOfApplication == "1.7.3.0") ||
                                        (StringOfVersionOfApplication == "1.7.4.0") ||
                                        (StringOfVersionOfApplication == "1.7.5.0") /* Current */
                                        )
                                    {
                                        IndexOfSettings = i; // "1.7.0.0"
                                    }

                                }
                            }
                            catch (Exception E)
                            { this.ReturningMessageAboutError("Ошибка при проверки версии файла настроек", E, true); }
                        }
                    //
                    // Copying File Of Settings
                    //
                    if (IndexOfSettings != -1)
                    {
                        //
                        // Copying File Of Settings
                        //
                        File.Copy(ListOfFilesOfSettings[IndexOfSettings].ToString(), PathToSettingsFile);
                        //
                        // Copying File Of Buffer
                        //
                        try
                        {
                            //
                            string DirectoryOfFileOfSettings = 
                                Path.GetDirectoryName(ListOfFilesOfSettings[IndexOfSettings].ToString());
                            //
                            string DirectoryOfApplication = 
                                Path.GetDirectoryName(PathToSettingsFile);
                            //
                            string PathToFileOfBuffer = DirectoryOfFileOfSettings + "\\Buffer.bin";
                            string PathToCopyingFileOfBuffer = DirectoryOfApplication + "\\Buffer.bin";
                            //
                            if (File.Exists(PathToFileOfBuffer))
                            {
                                //
                                File.Copy(PathToFileOfBuffer, PathToCopyingFileOfBuffer);
                                //
                                this.RecordingInLogFile("Файл Буфера скопирован");
                            }
                            else
                            { this.RecordingInLogFile("ERROR В проверяемой папке нет файла буфера"); }
                        }
                        catch (Exception E)
                        { this.ReturningMessageAboutError("Ошибка при копировании файла Буфера", E, true); }
                        //
                        // !!!
                        //
                        if (File.Exists(PathToSettingsFile))
                        {
                            //
                            // Recording In Reports
                            //
                            this.RecordingInLogFile("Файл настроек Скопирован");
                            //
                            try
                            {
                                //
                                // Creating FileStream
                                //
                                FileStream FS = new FileStream(PathToSettingsFile, FileMode.Open, FileAccess.Read);
                                //
                                // Deserialize Settings
                                //
                                try
                                {
                                    //
                                    SettingsOfMedicalPharm = (DataSet)new BinaryFormatter().Deserialize(FS);
                                    //
                                    SuccessfulPerusal = true;
                                }
                                catch (Exception E)
                                { this.ReturningMessageAboutError("Ошибка при чтении файла настроек", E, true); }
                                //
                                // Closing FileStream
                                //
                                try
                                { FS.Close(); }
                                catch (Exception E)
                                { this.ReturningMessageAboutError("Ошибка при закрытии файла настроек", E, false); }
                            }
                            catch (Exception E)
                            { this.ReturningMessageAboutError("Ошибка при открытии файла настроек", E, true); }
                        }
                        else
                        { this.ReturningMessageOfInformation("Файл настроек НЕ Скопирован"); }
                    }
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка общего плана при поиске файла настроек", E, true); }
                //PathToSettingsFile
                //
                if (!File.Exists(PathToSettingsFile))
                    this.ReturningMessageAboutError("Нет файла настроек", new Exception("No File"), true);
            }
            //
            // Creating Default Settings
            //
            if (SuccessfulPerusal == false)
            { SettingsOfMedicalPharm = CreatingSetOfSettings(); }
            //
            // Processing Data Of Settings
            //
            if (SuccessfulPerusal == true)
            {
                try
                {
                    //
                    // Clearing Registration Of Drugstores
                    //
                    if (SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count > 0)
                    {
                        //
                        // Clearing 0
                        //
                        for (int i = 0; i < SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count; i++)
                        {
                            //
                            if ((int)SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows[i]["ID"] == 0)
                                SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows[i].Delete();
                        }
                    }
                    //
                    string CurrentVersionOfApplication = "0";
                    //
                    // Checking Version Of Application
                    //
                    CurrentVersionOfApplication = SettingsOfMedicalPharm.Tables["InformationOfSettings"].
                        Rows.Find("VersionOfApplication")["Value"].ToString();
                    //
                    if (
                        !(
                        /*
                        (CurrentVersionOfApplication == "1.5.1.6") ||
                        (CurrentVersionOfApplication == "1.5.1.7") ||
                        
                        (CurrentVersionOfApplication == "1.6.0.0") ||
                        (CurrentVersionOfApplication == "1.6.1.0") ||
                        (CurrentVersionOfApplication == "1.6.2.0") ||
                        (CurrentVersionOfApplication == "1.6.3.0") ||
                        (CurrentVersionOfApplication == "1.6.4.0") ||
                        (CurrentVersionOfApplication == "1.6.5.0") ||
                        (CurrentVersionOfApplication == "1.6.6.0") ||
                        (CurrentVersionOfApplication == "1.6.7.0") ||
                        (CurrentVersionOfApplication == "1.6.8.0") ||
                        (CurrentVersionOfApplication == "1.6.9.0") ||
                        */
                        (CurrentVersionOfApplication == "1.6.10.0") ||
                        (CurrentVersionOfApplication == "1.7.0.0") ||
                        (CurrentVersionOfApplication == "1.7.1.0") ||
                        (CurrentVersionOfApplication == "1.7.2.0") ||
                        (CurrentVersionOfApplication == "1.7.3.0") ||
                        (CurrentVersionOfApplication == "1.7.4.0") ||
                        (CurrentVersionOfApplication == "1.7.5.0") ||
                        (CurrentVersionOfApplication == "1.7.6.0") ||
                        (CurrentVersionOfApplication == "1.7.7.0") ||
                        (CurrentVersionOfApplication == "1.7.8.0") ||
                        (CurrentVersionOfApplication == "1.7.8.1"))
                        )
                    {
                        SettingsOfMedicalPharm.Tables["InformationOfSettings"].
                            Rows.Find("VersionOfApplication")["Value"] = "0";
                    }
                    //
                    // Change Of Version Of Application
                    //
                    CurrentVersionOfApplication = SettingsOfMedicalPharm.Tables["InformationOfSettings"].
                        Rows.Find("VersionOfApplication")["Value"].ToString();
                    //
                    if (CurrentVersionOfApplication != "0")
                        SettingsOfMedicalPharm.Tables["InformationOfSettings"].
                            Rows.Find("VersionOfApplication")["Value"] = VersionOfApplication;
                    //
                    // Clearing Of All Registrations Of Drugstores
                    //

                    if (CurrentVersionOfApplication == "0")
                        for (int i = 0; i < SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count; i++)
                        { SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows[i].Delete(); }
                    //
                    // Accepting Changes
                    //
                    AcceptChanges();
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при обработке данных файла настроек", E, true); }
            }
            //
            // Copying Backup
            //
            BackupCopySettingsOfMedicalPharm = SettingsOfMedicalPharm.Copy();
        }

        // Creating Set Of Settings
        private DataSet CreatingSetOfSettings()
        {
            //
            // Creating Settings Of Medical Pharm
            DataSet SettingsOfMedicalPharm = new DataSet("SettingsOfMedicalPharm");
            SettingsOfMedicalPharm.RemotingFormat = SerializationFormat.Binary;
            //
            // Creating Table Of Information Of Settings
            //
            DataTable InformationOfSettings = new DataTable("InformationOfSettings");
            InformationOfSettings.Columns.Add("Key", typeof(string));
            InformationOfSettings.Columns.Add("Value", typeof(object));
            InformationOfSettings.PrimaryKey = new DataColumn[] { InformationOfSettings.Columns["Key"] };
            SettingsOfMedicalPharm.Tables.Add(InformationOfSettings);
            //
            // Creating Table Of ListOfSettings
            //
            DataTable ListOfSettings = new DataTable("ListOfSettings");
            ListOfSettings.Columns.Add("Key", typeof(string));
            ListOfSettings.Columns.Add("Value", typeof(object));
            ListOfSettings.PrimaryKey = new DataColumn[] { ListOfSettings.Columns["Key"] };
            SettingsOfMedicalPharm.Tables.Add(ListOfSettings);
            //
            // Creating Table Of Registration Of Drugstores
            //
            DataTable RegistrationOfDrugstores = new DataTable("RegistrationOfDrugstores");
            RegistrationOfDrugstores.Columns.Add("ID", typeof(int));
            RegistrationOfDrugstores.Columns.Add("PathToFolderOfPriceLists", typeof(string));
            RegistrationOfDrugstores.Columns.Add("MaskOfFullPriceList", typeof(string));
            RegistrationOfDrugstores.Columns.Add("MaskOfIncomingPriceList", typeof(string));
            RegistrationOfDrugstores.Columns.Add("MaskOfSoldPriceList", typeof(string));
            RegistrationOfDrugstores.Columns.Add("UseOfIDOfPriceList", typeof(bool));
            RegistrationOfDrugstores.Columns.Add("NotDeletingPriceList", typeof(bool));
            RegistrationOfDrugstores.PrimaryKey = new DataColumn[1] { RegistrationOfDrugstores.Columns["ID"] };
            SettingsOfMedicalPharm.Tables.Add(RegistrationOfDrugstores);
            //
            // Filling Of Information Of Settings
            //
            InformationOfSettings.Rows.Add("VersionOfSettings", 1);
            InformationOfSettings.Rows.Add("VersionOfApplication", VersionOfApplication);
            //
            // Filling With Keys (10) ListOfSettings
            //
            ListOfSettings.Rows.Add("ServerOfDataBase", "127.0.0.1");
            ListOfSettings.Rows.Add("NumberOfPort", 3307);
            ListOfSettings.Rows.Add("PasswordOfConnection", "1");
            //
            ListOfSettings.Rows.Add("ServerOfFTP", "FTP.MEDINCOM.RU");
            ListOfSettings.Rows.Add("PassiveMode", false);
            //
            ListOfSettings.Rows.Add("IndexOfDrugstore", 0);
            ListOfSettings.Rows.Add("SoldProducts", false);
            ListOfSettings.Rows.Add("OnlyPreferential", false);
            //
            ListOfSettings.Rows.Add("IntervalOfUpdating", 0);
            ListOfSettings.Rows.Add("AutoSendingPriceList", false);
            //
            // Filling With Registrations Of Drugstores
            //
            //RegistrationOfDrugstores.Rows.Add(0, "C:\\", "price", "prixod", "rasxod", false, true);
            //
            // Return
            //
            return SettingsOfMedicalPharm;
        }

        #endregion

        #region ' Converting Of Settings '

        /*
                public ManagementOfSettings(string PathToSettingsFile, Service.Reportings Reportings)
        {
            //
            // Initializing Fields
            //
            this.PathToSettingsFile = PathToSettingsFile;
            this.Reportings = Reportings;
            //
            // Initializing Of Intervals Of Updating
            //
            _IntervalsOfUpdating = new string[10]
            {
                "(Не обновлять)", 
                "3 минуты", "10 минут", "30 минут", 
                "1 час", "3 часа", "6 часов", 
                "8 часов", "12 часов", "24 часа"
            };
            //
            // Reading Of Settings
            //
            bool SuccessfulPerusal = false;
            //
            if (System.IO.File.Exists(PathToSettingsFile))
            {
                //
                // Reading Settings
                //
                try
                {
                    //
                    // Creating FileStream
                    //
                    FileStream FS = new FileStream(PathToSettingsFile, FileMode.Open);
                    //
                    // Deserialize Settings
                    //
                    try
                    {
                        SettingsOfMedicalPharm = (DataSet)new BinaryFormatter().Deserialize(FS);
                        //
                        SuccessfulPerusal = true;
                    }
                    catch (Exception E)
                    { Reportings.ReturningMessageAboutError("Ошибка при чтении файла настроек", E, true); }
                    //
                    // Closing FileStream
                    //
                    try
                    { FS.Close(); }
                    catch (Exception E)
                    { Reportings.ReturningMessageAboutError("Ошибка при закрытии файла настроек", E, false); }
                }
                catch (Exception E)
                { Reportings.ReturningMessageAboutError("Ошибка при открытии файла настроек", E, true); }
            }
            else
            {
                //
                // Recording In Reports
                //
                Reportings.RecordingInLog("Нет файла настроек. Поиск файла настроек.");
                //
                // Searching And Copying Settings
                //
                try
                {
                    //
                    // Searching Files Of Settings
                    //
                    string PathOfDirectoryOfApplication = Path.GetDirectoryName(PathToSettingsFile);
                    string PathOfParentDirectory = Path.GetDirectoryName(PathOfDirectoryOfApplication);
                    string[] DirectoriesForSearching = Directory.GetDirectories(PathOfParentDirectory);
                    //
                    ArrayList ListOfFilesOfSettings = new ArrayList();
                    //
                    foreach (string CurrentDirectory in DirectoriesForSearching)
                        foreach (string CurrentFile in Directory.GetFiles(CurrentDirectory, "*.bin"))
                            ListOfFilesOfSettings.Add(CurrentFile);
                    //
                    // Converting Of Files Of Settings
                    //
                    DataSet[] ListOfSettings = new DataSet[ListOfFilesOfSettings.Count];
                    //
                    int NumberOfFile = -1;
                    //
                    foreach (string CurrentFile in ListOfFilesOfSettings)
                    {
                        NumberOfFile++;
                        ListOfSettings[NumberOfFile] = null;
                        try
                        {
                            FileStream FS = new FileStream(CurrentFile, FileMode.Open);
                            try { ListOfSettings[NumberOfFile] = ((DataSet)new BinaryFormatter().Deserialize(FS)); }
                            catch (Exception E)
                            { Reportings.ReturningMessageAboutError("Ошибка при чтении найденного файла настроек", E, true); }
                            try { FS.Close(); }
                            catch (Exception E)
                            { Reportings.ReturningMessageAboutError("Ошибка при закрытии найденного файла настроек", E, true); }
                        }
                        catch (Exception E)
                        { Reportings.ReturningMessageAboutError("Ошибка при открытии найденного файла настроек", E, true); }
                    }
                    //
                    // Selection of Settings
                    //
                    //System.Windows.Forms.MessageBox.Show(IndexOfSettings); 
                    int IndexOfSettings = -1;
                    //
                    //
                    for (int i = 0; i < ListOfSettings.Length; i++ )
                        if (ListOfSettings[i] != null)
                        {
                            try
                            {
                                //
                                DataRow VersionOfApplication = 
                                    ListOfSettings[i].Tables["InformationOfSettings"].Rows.Find("VersionOfApplication");
                                if (VersionOfApplication != null)
                                {
                                    //
                                    string StringOfVersionOfApplication = VersionOfApplication["Value"].ToString();
                                    if ((StringOfVersionOfApplication == "1.5") ||
                                        (StringOfVersionOfApplication == "1.5.0.1") ||
                                        (StringOfVersionOfApplication == "1.5.0.2") ||
                                        (StringOfVersionOfApplication == "1.5.0.3") ||
                                        (StringOfVersionOfApplication == "1.5.0.4") ||
                                        (StringOfVersionOfApplication == "1.5.0.5") ||
                                        (StringOfVersionOfApplication == "1.5.0.6") ||
                                        (StringOfVersionOfApplication == "1.5.0.7") ||
                                        (StringOfVersionOfApplication == "1.5.0.8") ||
                                        (StringOfVersionOfApplication == "1.5.0.9") ||
                                        (StringOfVersionOfApplication == "1.5.1.0") ||
                                        (StringOfVersionOfApplication == "1.5.1.1") ||
                                        (StringOfVersionOfApplication == "1.5.1.2") ||
                                        (StringOfVersionOfApplication == "1.5.1.3") ||
                                        (StringOfVersionOfApplication == "1.5.1.4") ||
                                        (StringOfVersionOfApplication == "1.5.1.5") ||
                                        
                                        (StringOfVersionOfApplication == "1.5.1.6") ||
                                        (StringOfVersionOfApplication == "1.5.1.7") ||
                                        (StringOfVersionOfApplication == "1.6.0.0") ||
                                        (StringOfVersionOfApplication == "1.6.1.0")
                                        )
                                    {
                                        IndexOfSettings = i; // "1.6.2.0"
                                    }
                                    
                                }
                            }
                            catch { }
                        }
                    //
                    // !!!
                    //
                    if (IndexOfSettings != -1)
                    {
                        //
                        File.Copy(ListOfFilesOfSettings[IndexOfSettings].ToString(), PathToSettingsFile);
                        //
                        if (File.Exists(PathToSettingsFile))
                        {
                            //
                            //Reportings.ReturningMessageOfInformation("Файл настроек Скопирован");
                            Reportings.RecordingInLog("Файл настроек Скопирован");
                            //
                            try
                            {
                                //
                                // Creating FileStream
                                //
                                FileStream FS = new FileStream(PathToSettingsFile, FileMode.Open);
                                //
                                // Deserialize Settings
                                //
                                try
                                {
                                    SettingsOfMedicalPharm = (DataSet)new BinaryFormatter().Deserialize(FS);
                                    //
                                    SuccessfulPerusal = true;
                                }
                                catch (Exception E)
                                { Reportings.ReturningMessageAboutError("Ошибка при чтении файла настроек", E, true); }
                                //
                                // Closing FileStream
                                //
                                try
                                { FS.Close(); }
                                catch (Exception E)
                                { Reportings.ReturningMessageAboutError("Ошибка при закрытии файла настроек", E, false); }
                            }
                            catch (Exception E)
                            { Reportings.ReturningMessageAboutError("Ошибка при открытии файла настроек", E, true); }
                        }
                        else
                        {
                            Reportings.ReturningMessageOfInformation("Файл настроек НЕ Скопирован");
                        }
                        //ListOfFilesOfSettings[IndexOfDrugstore].ToString();
                        //PathToSettingsFile
                    }
                }
                catch (Exception E)
                { Reportings.ReturningMessageAboutError("Ошибка общего плана при поиске настроек", E, true); }
                //PathToSettingsFile
                //
                if (!File.Exists(PathToSettingsFile))
                    Reportings.ReturningMessageAboutError("Нет файла настроек", new Exception("No File"), true);
            }
            //
            // Reading Old Settings
            //
            if (SuccessfulPerusal == false)
            {
                //
                // Creating List Of Settings
                //
                SettingsOfMedicalPharm = CreatingSetOfSettings();
                //
                // Reading And Filling Old Settings
                //
                //ReadingOldFormatOfSettings();
                //
                // Saving Settings
                //
                //SavingSettings();
            }
            //
            // !!!
            //
            try
            {
                //
                // Clearing
                //
                if (SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count > 0)
                {
                    //
                    //int IDOfDrugstore = (int)SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows[0]["ID"];
                    //
                    // Clearing 184
                    //
                    /8
                    if (IDOfDrugstore == 184)
                    {
                        SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Clear();
                        //SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Add(0, "C:\\", "price", "prixod", "rasxod", false, true);
                    }
                    8/
                    //
                    // Clearing 0
                    //
                    for (int i = 0; i < SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count; i++)
                    {
                        //
                        if ((int)SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows[i]["ID"] == 0)
                            SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows[i].Delete();
                    }
                }
                //
                // New Version
                //
                SettingsOfMedicalPharm.Tables["InformationOfSettings"].Rows.Find("VersionOfApplication")["Value"] = 
                    VersionOfApplication;
                //
                SettingsOfMedicalPharm.AcceptChanges();
            }
            catch { }
            //
            // Accept Changes
            //
            //SettingsOfMedicalPharm.RemotingFormat = SerializationFormat.Binary;
            //
            if (SuccessfulPerusal == true)
            { AcceptChanges(); }
            else
            { BackupCopySettingsOfMedicalPharm = SettingsOfMedicalPharm.Copy(); }
            //
            // Copying Backup
            //
            //BackupCopySettingsOfMedicalPharm = SettingsOfMedicalPharm.Copy();
            //
        }
         */

        /*
        // Reading Old Format Of Settings
        private void ReadingOldFormatOfSettings()
        {
            //
            // Reading Old Format Of Settings And Filling Of ListOfSettings
            //
            // Drugstore
            //
            try
            {
                int IDOfDrugstore = Convert.ToInt32(Pharm66.Properties.Settings.Default["ID_Pharmacy"]);
                if (IDOfDrugstore > 0)
                {
                    DataRow NewDrugstore = SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].NewRow();
                    //
                    NewDrugstore["ID"] = IDOfDrugstore;
                    //
                    string PathToFolderOfPriceLists = Pharm66.Properties.Settings.Default["Path_Drug"].ToString();
                    if (PathToFolderOfPriceLists.Length > 1)
                        NewDrugstore["PathToFolderOfPriceLists"] = PathToFolderOfPriceLists;
                    //
                    PathToFolderOfPriceLists = Pharm66.Properties.Settings.Default["Path_AbonDB"].ToString();
                    if (PathToFolderOfPriceLists.Length > 1)
                        NewDrugstore["PathToFolderOfPriceLists"] = PathToFolderOfPriceLists;
                    //
                    // "IDOfDrugstoreOfAbonent" Pharm66.Properties.Settings.Default["Id_pharm_Abon"].ToString()
                    NewDrugstore["MaskOfFullPriceList"] = Pharm66.Properties.Settings.Default["Price_mask"].ToString().ToLower();
                    NewDrugstore["MaskOfIncomingPriceList"] = Pharm66.Properties.Settings.Default["Income_mask"].ToString().ToLower();
                    NewDrugstore["MaskOfSoldPriceList"] = Pharm66.Properties.Settings.Default["Defect_mask"].ToString().ToLower();
                    NewDrugstore["UseOfIDOfPriceList"] = Convert.ToBoolean(ReadingConfigurationFromXML("ce_is_use_id_pharm"));
                    NewDrugstore["NotDeletingPriceList"] = Convert.ToBoolean(ReadingConfigurationFromXML("ce_is_keep_price"));
                    //
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Add(NewDrugstore);
                }
            }
            catch (Exception E)
            {
                Reportings.ReturningMessageAboutError(
                    "Ошибка при чтении старого формата настроек Drugstore", E, true);
            }
            //
            // Connections
            //
            try
            {
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("ServerOfDataBase")["Value"] =
                    Pharm66.Properties.Settings.Default["Server_Name"].ToString();
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("NumberOfPort")["Value"] =
                    Convert.ToInt32(Pharm66.Properties.Settings.Default["Server_Port"]);
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("ServerOfFTP")["Value"] =
                    Pharm66.Properties.Settings.Default["def_Ftp"].ToString();
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("PassiveMode")["Value"] =
                    Convert.ToBoolean(ReadingConfigurationFromXML("ce_is_ftp_passive"));
            }
            catch (Exception E)
            {
                Reportings.ReturningMessageAboutError(
                    "Ошибка при чтении старого формата настроек Connections", E, true);
            }
            //
            // Updating
            //
            try
            {
                string TextOfInterval = Pharm66.Properties.Settings.Default["Interval"].ToString();
                //
                int IntervalOfUpdating = 0;
                if ((TextOfInterval != null) && (TextOfInterval != ""))
                {
                    //
                    if (TextOfInterval.IndexOf("(Не обновлять)") > -1)
                        IntervalOfUpdating = 0;
                    else if ((TextOfInterval.IndexOf("3") > -1) && (TextOfInterval.IndexOf("минуты") > -1))
                        IntervalOfUpdating = 1000 * 60 * 3;
                    else if ((TextOfInterval.IndexOf("10") > -1) && (TextOfInterval.IndexOf("минут") > -1))
                        IntervalOfUpdating = 1000 * 60 * 10;
                    else if ((TextOfInterval.IndexOf("30") > -1) && (TextOfInterval.IndexOf("минут") > -1))
                        IntervalOfUpdating = 1000 * 60 * 30;
                    else if ((TextOfInterval.IndexOf("1 ") > -1) && (TextOfInterval.IndexOf("час") > -1))
                        IntervalOfUpdating = 1000 * 60 * 60;
                    else if ((TextOfInterval.IndexOf("3") > -1) && (TextOfInterval.IndexOf("час") > -1))
                        IntervalOfUpdating = 1000 * 60 * 60 * 3;
                    else if (TextOfInterval.IndexOf("6") > -1)
                        IntervalOfUpdating = 1000 * 60 * 60 * 6;
                    else if (TextOfInterval.IndexOf("8") > -1)
                        IntervalOfUpdating = 1000 * 60 * 60 * 8;
                    else if (TextOfInterval.IndexOf("12") > -1)
                        IntervalOfUpdating = 1000 * 60 * 60 * 12;
                    else if (TextOfInterval.IndexOf("24") > -1)
                        IntervalOfUpdating = 1000 * 60 * 60 * 24;
                }
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IntervalOfUpdating")["Value"] =
                    IntervalOfUpdating;
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("AutoSendingPriceList")["Value"] =
                    Convert.ToBoolean(ReadingConfigurationFromXML("ce_is_auto_send"));
            }
            catch (Exception E)
            {
                Reportings.ReturningMessageAboutError(
                    "Ошибка при чтении старого формата настроек Updating", E, true);
            }
            //
            // PriceList
            //
            try
            {
                bool OwnPreparations = Convert.ToBoolean(ReadingConfigurationFromXML("ce_ed_is_my_pharm"));
                int IndexOfDrugstore = OwnPreparations == true ? 1 : 0;
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IndexOfDrugstore")["Value"] =
                    IndexOfDrugstore;
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("SoldProducts")["Value"] =
                    Convert.ToBoolean(ReadingConfigurationFromXML("ce_ed_is_defect"));
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("OnlyPreferential")["Value"] =
                    Convert.ToBoolean(ReadingConfigurationFromXML("ce_Is_Privelege"));
            }
            catch (Exception E)
            {
                Reportings.ReturningMessageAboutError(
                    "Ошибка при чтении старого формата настроек PriceList", E, true);
            }
            //
        }
        
        // Reading Configuration From XML
        public string ReadingConfigurationFromXML(string element)
        {
            //
            string ReadValue = "НЕТ";
            //
            try
            {
                string fname = (Pharm66.Properties.Settings.Default["Path_Root"]).ToString() + "Grid.xml";
                System.Xml.XmlReader read = System.Xml.XmlReader.Create(fname);
                while (read.Read())
                {
                    if (read.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        if (read.Name == element)
                        {
                            ReadValue = read.ReadElementString();
                            break;
                        }
                    }
                }
                read.Close();
            }
            catch { }
            // Return
            return ReadValue;
        }

         */

        #endregion;

        #region ' Checking '

        // Checking Of Connection
        public bool CheckingOfConnection()
        {
            //
            bool Checking = true;
            //
            this.RecordingInLogFile("Checking Of Connection");
            //
            // Checking Of Connection
            //
            try
            {
                MySql.Data.MySqlClient.MySqlConnection CheckingOfConnection = new 
                    MySql.Data.MySqlClient.MySqlConnection(this.GettingStringOfConnection);
                //
                try
                {
                    //
                    CheckingOfConnection.Open();
                    CheckingOfConnection.Close();
                }
                catch (Exception E)
                {
                    //
                    Checking = false;
                    //
                    this.RecordingInLogFile(
                        String.Format("Ошибка при открытии подключения к БД по адресу {0} и порту {1}: {2}",
                        this.GettingServerOfDataBase, this.GettingNumberOfPort, E.Message));
                }
            }
            catch (Exception E)
            {
                //
                Checking = false;
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при подключении к БД по адресу {0} и порту {1}: {2}", 
                    this.GettingServerOfDataBase, this.GettingNumberOfPort, E.Message));
            }
            //
            if (Checking)
            { this.RecordingInLogFile("Checking Of Connection Is OK"); }
            //
            // Return
            //
            return Checking;
        }

        // Checking Of Communication
        public bool CheckingOfCommunication()
        {
            //
            bool Checking = true;
            //
            this.RecordingInLogFile("Checking Of Communication");
            //
            // Checking
            //
            try
            {
                //
                FtpWebRequest RequestToFTP = (FtpWebRequest)WebRequest.Create(this.GettingPathOfImportingOfFTP);
                RequestToFTP.Timeout = 1000 * 60 * 5;
                RequestToFTP.ReadWriteTimeout = 1000 * 60 * 5;
                RequestToFTP.KeepAlive = false;
                RequestToFTP.UsePassive = this.GettingPassiveMode;
                RequestToFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                //
                FtpWebResponse ResponseOfGettingListOfFile = (FtpWebResponse)RequestToFTP.GetResponse();
                StreamReader ReadingOfList = new StreamReader(ResponseOfGettingListOfFile.GetResponseStream());
                ReadingOfList.Close();
                ReadingOfList.Dispose();
                //ResponseOfGettingListOfFile.Close();
                //
                /*
                FtpWebRequest RequestToFTP = (FtpWebRequest)WebRequest.Create(this.GettingPathOfImportingOfFTP);
                RequestToFTP.UsePassive = this.GettingPassiveMode;
                RequestToFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse ResponseOfFTP = (FtpWebResponse)RequestToFTP.GetResponse();
                StreamReader ReadingOfList = new StreamReader(ResponseOfFTP.GetResponseStream());
                ReadingOfList.Close();
                */
            }
            catch (Exception E)
            {
                //
                Checking = false;
                //
                this.RecordingInLogFile(
                  String.Format("Ошибка при подключении к FTP Серверу {0}, режим {1}: {2}",
                  this.GettingServerOfFTP, this.GettingPassiveMode, E.Message));
            }
            //
            if (Checking)
            { this.RecordingInLogFile("Checking Of Communication Is OK"); }
            //
            // Return
            //
            return Checking;
        }

        // Checking Of Password Of Management
        public bool CheckingOfPasswordOfManagement(string Password)
        {
            //85419623
            //78651296
            if (Password == "78651296")
                return true;
            else
                return false;
        }

        #endregion

        #region ' Exists File '

        // Existing File Full
        public void ExistingFileFull(int IDOfDrugstore)
        {
            // Existing File
            ExistingFile(1, IDOfDrugstore);
        }

        // Existing File Incoming
        public void ExistingFileIncoming(int IDOfDrugstore)
        {
            // Existing File
            ExistingFile(2, IDOfDrugstore);
        }

        // Existing File Deleting
        public void ExistingFileDeleting(int IDOfDrugstore)
        {
            // Existing File
            ExistingFile(3, IDOfDrugstore);
        }

        // Existing File
        private void ExistingFile(int IndexOfExisting, int IDOfDrugstore)
        {
            //
            if (IDOfDrugstore > 0)
            {
                //
                string PathToFolderOfPriceLists = GettingPathToFolderOfPriceLists(IDOfDrugstore);
                //
                // Selection Of Mask Of File
                //
                string Mask = "";
                //
                Console.WriteLine(IDOfDrugstore);
                //
                switch (IndexOfExisting)
                {
                    case 1:
                        Mask = GettingMaskOfFullPriceList(IDOfDrugstore);
                        break;
                    case 2:
                        Mask = GettingMaskOfIncomingPriceList(IDOfDrugstore);
                        break;
                    case 3:
                        Mask = GettingMaskOfSoldPriceList(IDOfDrugstore);
                        break;
                }
                //
                Console.WriteLine(Mask);
                //
                //
                // Existing Directory
                //
                if (Directory.Exists(PathToFolderOfPriceLists))
                {
                    //
                    bool FileOfExist = false;
                    //
                    string[] FilesOfDirectory = Directory.GetFiles(PathToFolderOfPriceLists);
                    //
                    // Selection Of File
                    //
                    if (Mask != "")
                        foreach (string CurrentFile in FilesOfDirectory)
                            if (Path.GetFileName(CurrentFile).IndexOf(Mask) > -1)
                            { FileOfExist = true; break; }
                    //
                    // Show Of Message
                    //
                    if (FileOfExist)
                    {
                        new Service.Reportings("").ReturningMessageOfInformation(
                            String.Format("В директории '{0}' Есть файл c маской '{1}'",
                            PathToFolderOfPriceLists, Mask));
                    }
                    else
                    {
                        new Service.Reportings("").ReturningMessageOfInformation(
                            String.Format("В директории '{0}' Нет файла по Маске '{1}'",
                            PathToFolderOfPriceLists, Mask));
                    }
                }
                else
                {
                    new Service.Reportings("").ReturningMessageAboutError(
                        String.Format("Директория '{0}' не существует", PathToFolderOfPriceLists),
                        new Exception("Exists False"), false);
                }
            }
        }

        #endregion

        #region ' Management Of Registration Of Drugstore '

        // Registration Of Drugstore
        public void RegistrationOfDrugstore(int IDOfDrugstore, string Password)
        {
            //
            if ((IDOfDrugstore > 0) && (IDOfDrugstore != 108) && CheckingOfPasswordOfManagement(Password))
            {
                //
                DataRow FindRow = SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                if (FindRow == null)
                {
                    //
                    DataRow NewDrugstore = SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].NewRow();
                    //
                    NewDrugstore["ID"] = IDOfDrugstore;
                    NewDrugstore["PathToFolderOfPriceLists"] = "C:\\";
                    NewDrugstore["MaskOfFullPriceList"] = "price";
                    NewDrugstore["MaskOfIncomingPriceList"] = "prixod";
                    NewDrugstore["MaskOfSoldPriceList"] = "rasxod";
                    NewDrugstore["UseOfIDOfPriceList"] = false;
                    NewDrugstore["NotDeletingPriceList"] = false;
                    //
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Add(NewDrugstore);
                }
            }
        }

        // Removing Drugstore
        public void RemovingDrugstore(int IDOfDrugstore, string Password)
        {
            //
            if ((IDOfDrugstore > 0) && CheckingOfPasswordOfManagement(Password))
            {
                // Getting
                DataRow FindRow =
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                //
                // Deleting
                //
                if (FindRow != null)
                {
                    FindRow.Delete();
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].AcceptChanges();
                }
            }
        }

        // Getting Registration Of Drugstores
        public DataTable GettingRegistrationOfDrugstores()
        {
            return SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Copy();
        }

        #endregion

        #region ' Getting Settings '

        // Getting Copy Setting
        public DataTable GettingSetting
        {
            get { return SettingsOfMedicalPharm.Tables["ListOfSettings"].Copy(); }
        }

        // Getting All Settings
        public DataSet GettingAllSettings
        {
            get { return SettingsOfMedicalPharm.Copy(); }
        }

        #region ' Connection To DataBase '

        // Getting Server Of DataBase
        public string GettingServerOfDataBase
        {
            get
            {
                return Convert.ToString(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("ServerOfDataBase")["Value"]);
            }
        }

        // Getting Number Of Port
        public int GettingNumberOfPort
        {
            get
            {
                return Convert.ToInt32(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("NumberOfPort")["Value"]);
            }
        }

        // Getting String Of Connection
        public string GettingStringOfConnection
        {
            get
            {
                //
                // Creating String Of Connection
                //
                //allow zero datetime=True;default command timeout=300;
                string StringOfConnection = String.Format(
                    "Server={0};Port={1};Database=Pharm66;User Id=root;Password={2};Character Set=cp1251;Persist Security Info=True",
                    this.GettingServerOfDataBase, this.GettingNumberOfPort,
                    Convert.ToString(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("PasswordOfConnection")["Value"]));
                //
                // Return
                //
                return StringOfConnection;
            }
        }

        #endregion

        #region ' Connection To FTP Server '

        // Getting Server Of FTP
        public string GettingServerOfFTP
        {
            get
            {
                return Convert.ToString(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("ServerOfFTP")["Value"]);
            }
        }

        // Getting Login And Password To FTP
        public string GettingLoginAndPasswordToFTP
        {
            get
            {
                return "Drugstore:45561013";
            }
        }

        // Getting Path Of Exporting On FTP
        public string GettingPathOfExportingOnFTP
        {
            get
            {
                return String.Format("ftp://Drugstore:45561013@{0}/ExportedData/", this.GettingServerOfFTP);
            }
        }

        // Getting Path Of Exported Price Lists On FTP
        public string GettingPathOfExportedPriceListsOnFTP
        {
            get
            {
                return String.Format("ftp://Drugstore:45561013@{0}/ExportedPriceLists/", this.GettingServerOfFTP);
            }
        }

        // Getting Path Of Importing Of FTP
        public string GettingPathOfImportingOfFTP
        {
            get
            {
                return String.Format("ftp://Drugstore:45561013@{0}/ImportingData/", this.GettingServerOfFTP);
            }
        }

        // Getting Passive Mode
        public bool GettingPassiveMode
        {
            get
            {
                return Convert.ToBoolean(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("PassiveMode")["Value"]);
            }
        }

        #endregion

        #region ' Settings For Loading PriceList '

        // Getting IDs Of Editings Of Drugstores
        public int[] GettingIDsOfEditingsOfDrugstores
        {
            get
            {
                //
                int[] ReturnIDs =
                    new int[SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count];
                //
                int Increment = 0;
                //
                foreach (DataRow CurrentDrugstore in SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows)
                    ReturnIDs[Increment++] = Convert.ToInt32(CurrentDrugstore["ID"]);
                // Return
                return ReturnIDs;
            }
        }

        // Getting Path To Folder Of PriceLists
        public string GettingPathToFolderOfPriceLists(int IDOfDrugstore)
        {
            // Getting Value
            object Value = ReadingValue(IDOfDrugstore, "PathToFolderOfPriceLists");
            // Return
            return (Value != null) ? Value.ToString() : "";
        }

        // Getting Mask Of Full PriceList
        public string GettingMaskOfFullPriceList(int IDOfDrugstore)
        {
            // Getting Value
            object Value = ReadingValue(IDOfDrugstore, "MaskOfFullPriceList");
            // Return
            return (Value != null) ? Value.ToString() : "";
        }

        // Getting Mask Of Incoming PriceList
        public string GettingMaskOfIncomingPriceList(int IDOfDrugstore)
        {
            // Getting Value
            object Value = ReadingValue(IDOfDrugstore, "MaskOfIncomingPriceList");
            // Return
            return (Value != null) ? Value.ToString() : "";
        }

        // Getting Mask Of Sold PriceList
        public string GettingMaskOfSoldPriceList(int IDOfDrugstore)
        {
            // Getting Value
            object Value = ReadingValue(IDOfDrugstore, "MaskOfSoldPriceList");
            // Return
            return (Value != null) ? Value.ToString() : "";
        }

        // Getting Use Of ID Of PriceList
        public bool GettingUseOfIDOfPriceList(int IDOfDrugstore)
        {
            // Getting Value
            object Value = ReadingValue(IDOfDrugstore, "UseOfIDOfPriceList");
            // Return
            return (Value != null) ? Convert.ToBoolean(Value) : false;
        }

        // Getting Not Deleting PriceList
        public bool GettingNotDeletingPriceList(int IDOfDrugstore)
        {
            // Getting Value
            object Value = ReadingValue(IDOfDrugstore, "NotDeletingPriceList");
            // Return
            return (Value != null) ? Convert.ToBoolean(Value) : false;
        }

        // Reading Value
        private object ReadingValue(int IDOfDrugstore, string Parameter)
        {
            //
            object ReturnValue = null;
            //
            if (IDOfDrugstore > 0)
            {
                DataRow FindRow =
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                if (FindRow != null)
                    ReturnValue = FindRow[Parameter];
            }
            //
            return ReturnValue;
        }

        #endregion

        // Interval Of Importing
        public int IntervalOfImporting
        {
            get
            {
                /*
                return Convert.ToInt32(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IntervalOfUpdating")["Value"]);
                */
                // 3
                //
                return 3;
            }
        }

        #region ' Updating Of Data '

        // Index Of Interval Of Updating
        public int IndexOfIntervalOfUpdating
        {
            get
            {
                //
                int ReturnIndexOfIntervalOfUpdating = 0;
                //
                switch (IntervalOfUpdating)
                {
                    case 0:
                        ReturnIndexOfIntervalOfUpdating = 0;//(Не обновлять)
                        break;
                    case 1000 * 60 * 3:
                        ReturnIndexOfIntervalOfUpdating = 1;//3 минуты
                        break;
                    case 1000 * 60 * 10:
                        ReturnIndexOfIntervalOfUpdating = 2;//10 минут
                        break;
                    case 1000 * 60 * 30:
                        ReturnIndexOfIntervalOfUpdating = 3;//30 минут
                        break;
                    case 1000 * 60 * 60:
                        ReturnIndexOfIntervalOfUpdating = 4;//1 час
                        break;
                    case 1000 * 60 * 60 * 3:
                        ReturnIndexOfIntervalOfUpdating = 5;//3 часа
                        break;
                    case 1000 * 60 * 60 * 6:
                        ReturnIndexOfIntervalOfUpdating = 6;//6 часов
                        break;
                    case 1000 * 60 * 60 * 8:
                        ReturnIndexOfIntervalOfUpdating = 7;//8 часов
                        break;
                    case 1000 * 60 * 60 * 12:
                        ReturnIndexOfIntervalOfUpdating = 8;//12 часов
                        break;
                    case 1000 * 60 * 60 * 24:
                        ReturnIndexOfIntervalOfUpdating = 9;//24 часа
                        break;
                }
                // Return
                return ReturnIndexOfIntervalOfUpdating;
            }
        }

        // Interval Of Updating
        public int IntervalOfUpdating
        {
            get
            {
                return Convert.ToInt32(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IntervalOfUpdating")["Value"]);
            }
        }

        // Getting Intervals Of Updating
        public string[] GettingIntervalsOfUpdating
        {
            get { return _IntervalsOfUpdating; }
        }

        // Auto Sending PriceList
        public bool AutoSendingPriceList
        {
            get
            {
                return Convert.ToBoolean(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("AutoSendingPriceList")["Value"]);
            }
        }

        #endregion

        #region ' Management Of PriceList '

        // Index Of Drugstore
        public int IndexOfDrugstore
        {
            get
            {
                return Convert.ToInt32(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IndexOfDrugstore")["Value"]);
            }
        }

        // Sold Products
        public bool SoldProducts
        {
            get
            {
                return Convert.ToBoolean(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("SoldProducts")["Value"]);
            }
        }

        // Only Preferential
        public bool OnlyPreferential
        {
            get
            {
                return Convert.ToBoolean(
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("OnlyPreferential")["Value"]);
            }
        }

        #endregion

        #endregion

        #region ' Change Of Settings '

        #region ' Change Of Connection To DataBase '

        // Change Of Server Of DataBase
        public void ChangeOfServerOfDataBase(string NewServerOfDataBase)
        {
            //
            if ((NewServerOfDataBase != null) && (NewServerOfDataBase != ""))
                if (NewServerOfDataBase.Length > 0)
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("ServerOfDataBase")["Value"] =
                        NewServerOfDataBase;
        }

        // Change Of Number Of Port
        public void ChangeOfNumberOfPort(decimal NewPort)
        {
            //
            if ((NewPort > 0) && (NewPort <= 65536))
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("NumberOfPort")["Value"] =
                    Convert.ToInt32(NewPort);
        }

        #endregion

        #region ' Change Of Connection To FTP Server '

        // Change Of Server Of FTP
        public void ChangeOfServerOfFTP(string NewServer)
        {
            //
            if ((NewServer != null) && (NewServer != ""))
                if (NewServer.Length > 0)
                    SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("ServerOfFTP")["Value"] = NewServer;
        }

        // Change Of Passive Mode
        public void ChangeOfPassiveMode(bool NewValue)
        {
            //
            SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("PassiveMode")["Value"] = NewValue;
        }

        #endregion

        #region ' Change Of Settings For Loading PriceList '

        // Change Path To Folder Of PriceLists
        public void ChangePathToFolderOfPriceLists(int IDOfDrugstore, string NewPath)
        {
            //
            if (IDOfDrugstore > 0)
                if ((NewPath != null) && (NewPath != ""))
                    if (Directory.Exists(NewPath))
                    {
                        DataRow FindRow =
                            SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                        if (FindRow != null)
                            FindRow["PathToFolderOfPriceLists"] = NewPath;
                    }
        }

        #region ' Change Of Masks '

        // Change Of Mask Of Full PriceList
        public void ChangeOfMaskOfFullPriceList(int IDOfDrugstore, string NewMask)
        {
            //
            ChangeOfMask(IDOfDrugstore, NewMask, "MaskOfFullPriceList");
        }

        // Change Of Mask Of Incoming PriceList
        public void ChangeOfMaskOfIncomingPriceList(int IDOfDrugstore, string NewMask)
        {
            //
            ChangeOfMask(IDOfDrugstore, NewMask, "MaskOfIncomingPriceList");
        }

        // Change Of Mask Of Sold PriceList
        public void ChangeOfMaskOfSoldPriceList(int IDOfDrugstore, string NewMask)
        {
            //
            ChangeOfMask(IDOfDrugstore, NewMask, "MaskOfSoldPriceList");
        }

        // Change Of Mask
        private void ChangeOfMask(int IDOfDrugstore, string NewMask, string Parameter)
        {
            //
            if (IDOfDrugstore > 0)
                if ((NewMask != null) && (NewMask != ""))
                    if (NewMask.Length > 0)
                    {
                        //
                        NewMask = NewMask.ToLower();
                        //
                        char[] RemovingChars = Path.GetInvalidFileNameChars();
                        foreach (char CurrentChar in RemovingChars)
                            NewMask = NewMask.Replace(CurrentChar.ToString(), "");
                        //
                        if (NewMask.Length > 0)
                        {
                            DataRow FindRow =
                                SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                            if (FindRow != null)
                                FindRow[Parameter] = NewMask;
                        }
                    }
        }

        #endregion

        // Change Of Use Of ID Of PriceList
        public void ChangeOfUseOfIDOfPriceList(int IDOfDrugstore, bool NewValue)
        {
            //
            if (IDOfDrugstore > 0)
            {
                DataRow FindRow = 
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                if (FindRow != null) 
                    FindRow["UseOfIDOfPriceList"] = NewValue;
            }
        }

        // Change Of Not Deleting PriceList
        public void ChangeOfNotDeletingPriceList(int IDOfDrugstore, bool NewValue)
        {
            //
            if (IDOfDrugstore > 0)
            {
                DataRow FindRow =
                    SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Find(IDOfDrugstore);
                if (FindRow != null)
                    FindRow["NotDeletingPriceList"] = NewValue;
            }
        }

        #endregion

        #region ' Change Of Updating Of Data '

        // Change Of Index Of Interval Of Updating
        public void ChangeOfIndexOfIntervalOfUpdating(int NewIndex)
        {
            //
            if ((NewIndex >= 0) && (NewIndex <= 9))
            {
                //
                int NewIntervalOfUpdating = 0;
                //
                switch (NewIndex)
                {
                    case 0:
                        NewIntervalOfUpdating = 0;//(Не обновлять)
                        break;
                    case 1:
                        NewIntervalOfUpdating = 1000 * 60 * 3;//3 минуты
                        break;
                    case 2:
                        NewIntervalOfUpdating = 1000 * 60 * 10;//10 минут
                        break;
                    case 3:
                        NewIntervalOfUpdating = 1000 * 60 * 30;//30 минут
                        break;
                    case 4:
                        NewIntervalOfUpdating = 1000 * 60 * 60;//1 час
                        break;
                    case 5:
                        NewIntervalOfUpdating = 1000 * 60 * 60 * 3;//3 часа
                        break;
                    case 6:
                        NewIntervalOfUpdating = 1000 * 60 * 60 * 6;//6 часов
                        break;
                    case 7:
                        NewIntervalOfUpdating = 1000 * 60 * 60 * 8;//8 часов
                        break;
                    case 8:
                        NewIntervalOfUpdating = 1000 * 60 * 60 * 12;//12 часов
                        break;
                    case 9:
                        NewIntervalOfUpdating = 1000 * 60 * 60 * 24;//24 часа
                        break;
                }
                //
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IntervalOfUpdating")["Value"] = 
                    NewIntervalOfUpdating;
            }
        }

        // Change Of Auto Sending PriceList
        public void ChangeOfAutoSendingPriceList(bool NewValue)
        {
            //
            SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("AutoSendingPriceList")["Value"] = NewValue;
        }

        #endregion

        #region ' Change Of Management Of PriceList '

        // Change Of Index Of Drugstore
        public void ChangeOfIndexOfDrugstore(int NewIndex)
        {
            //
            if ((NewIndex >= 0) &&
                (NewIndex <= SettingsOfMedicalPharm.Tables["RegistrationOfDrugstores"].Rows.Count)) 
                SettingsOfMedicalPharm.Tables["ListOfSettings"].Rows.Find("IndexOfDrugstore")["Value"] = NewIndex;
        }

        #endregion

        #endregion

        #region ' Management Of Change '

        // Accept Changes
        public void AcceptChanges()
        {
            //
            // Recording In Log File
            //
            this.RecordingInLogFile("Accept Changes");
            //
            // Removing Backup Copy
            //
            SettingsOfMedicalPharm.AcceptChanges();
            BackupCopySettingsOfMedicalPharm = SettingsOfMedicalPharm.Copy();
            //
            // Saving Settings
            //
            SavingSettings();
        }

        // Reset Of Changes
        public void ResetOfChanges()
        {
            //
            // Recording In Log File
            //
            this.RecordingInLogFile("Reset Of Changes");
            //
            // Restoration of Settings
            //
            SettingsOfMedicalPharm = BackupCopySettingsOfMedicalPharm.Copy();
        }

        #endregion

        #region ' Service '

        // Saving Settings
        private void SavingSettings()
        {
            //
            // Writing Settings In DAT File
            //
            SettingsOfMedicalPharm.RemotingFormat = SerializationFormat.Binary;
            //bool SuccessfulSaving = false;
            //
            try
            {
                //
                // Creating FileStream
                //
                FileStream FS = new FileStream(PathToSettingsFile, FileMode.Create);
                //
                // Deserialize Settings
                //
                try
                {
                    new BinaryFormatter().Serialize(FS, SettingsOfMedicalPharm);
                    //
                    //SuccessfulSaving = true;
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при сохранении настроек в файл", E, true); }
                //
                // Closing FileStream
                //
                try
                { FS.Close(); }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при закрытии файла настроек", E, false); }
            }
            catch (Exception E)
            { this.ReturningMessageAboutError("Ошибка при создании файла настроек", E, true); }
        }

        #endregion

    }
}