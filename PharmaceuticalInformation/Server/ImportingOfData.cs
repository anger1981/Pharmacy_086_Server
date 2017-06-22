using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PharmaceuticalInformation.BaseTypes;
using System.Linq;
using EntityFramework.Extensions;
using ServerOfSystem.PharmaceuticalInformation.DataTools;
using ServerOfSystem.PharmaceuticalInformation.Infrastructure;
using ServerOfSystem.PharmaceuticalInformation.Interfaces;
using ServerOfSystem.PharmaceuticalInformation;
using Ninject;
using ServerOfSystem.PharmaceuticalInformation.Model;

namespace PharmaceuticalInformation.Server
{
    public class ImportingOfData : BaseType
    {

        #region ' Fields '

        private IPharmacyInformation IPhrmInf;

        public class PriceListDrugstore
        {
            public int ID_PR;
            public decimal Price;
            public bool Deleting;
            public bool Preferential;
        }

        #endregion


        #region ' Designer '

        public ImportingOfData(IPharmacyInformation _IPhrmInf)
            : this(_IPhrmInf, "")
        {
            //
        }

        public ImportingOfData(IPharmacyInformation _IPhrmInf, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initializing Fields
            //
            try
            {
                IPhrmInf = _IPhrmInf;
            }
            catch (Exception E) { throw new Exception(String.Format("Ошибка при создании подключения экспорта: {0}", E)); }
            //
            // !!!
            //
            //UpdatingOfData = new Updating.UpdatingOfDataOfInformationForMsSQL(StringOfConnection, PathToLogFile);
        }

        #endregion


        #region ' Getting List Of Private Importers '

        // Getting List Of Private Importers
        public PrivateImporter[] GettingListOfPrivateImporters()
        {
            //
            // Getting Tables Of Private Importers
            //
            DataSet TablesOfPrivateImporters = new DataSet("TablesOfPrivateImporters");

            DataTable DT_PrivImp = new DataTable();
            DataTable DT_RecID = new DataTable();

            try
            {
                IPhrmInf.EFPhrmInf.PrivateImportings.AsEnumerable().Fill(ref DT_PrivImp);
                IPhrmInf.EFPhrmInf.RecodingIDsOfDrugstoresOfImportings.AsEnumerable().Fill(ref DT_RecID);

                TablesOfPrivateImporters.Tables.Add(DT_PrivImp);
                TablesOfPrivateImporters.Tables.Add(DT_RecID);
            }
            catch (Exception E)
            {
                //
                RecordingInLogFile(String.Format("ERROR Error Of Getting List Private Importers: {0}", E.Message));
                //
                if (TablesOfPrivateImporters == null)
                    TablesOfPrivateImporters = new DataSet("TablesOfPrivateImporters");
            }
            //
            // Creating List Of Private Importers
            //
            PrivateImporter[] ListOfPrivateImporters = new PrivateImporter[0];
            //
            if (TablesOfPrivateImporters.Tables.Count >= 2)
            {
                try
                {
                    //
                    // Processing Tables
                    //
                    TablesOfPrivateImporters.Tables[0].TableName = "PrivateImportings";
                    TablesOfPrivateImporters.Tables[1].TableName = "RecodingIDsOfDrugstoresOfImportings";
                    TablesOfPrivateImporters.Relations.Add(
                        "GettingRecodingIDs", 
                        TablesOfPrivateImporters.Tables["PrivateImportings"].Columns["ID"], 
                        TablesOfPrivateImporters.Tables["RecodingIDsOfDrugstoresOfImportings"].Columns["IDOfPrivateImportings"]);
                    //
                    // Scaning Tables
                    //
                    ListOfPrivateImporters = 
                        new PrivateImporter[TablesOfPrivateImporters.Tables["PrivateImportings"].Rows.Count];
                    int IndexOfImporter = -1;
                    //
                    foreach (DataRow CurrentImporter in TablesOfPrivateImporters.Tables["PrivateImportings"].Rows)
                    {
                        //
                        PrivateImporter.RecodingID[] RecodingIDs = 
                            new PrivateImporter.RecodingID[CurrentImporter.GetChildRows("GettingRecodingIDs").Length];
                        int IndexOfRecoding = -1;
                        foreach (DataRow CurrentRecoding in CurrentImporter.GetChildRows("GettingRecodingIDs"))
                            RecodingIDs[++IndexOfRecoding] = 
                                new PrivateImporter.RecodingID(
                                    (int)CurrentRecoding["IDOfImporter"],
                                    (int)CurrentRecoding["IDOfSystem"]);
                        //
                        ListOfPrivateImporters[++IndexOfImporter] = 
                            new PrivateImporter(
                                CurrentImporter["ID"].ToString(), 
                                CurrentImporter["NameOfImporter"].ToString(), 
                                (bool)CurrentImporter["Active"], 
                                CurrentImporter["PathOfImporting"].ToString(), 
                                (bool)CurrentImporter["UseOfSystemLogin"], 
                                CurrentImporter["MaskOfFileOfImporting"].ToString(), 
                                (bool)CurrentImporter["UseOfRecoding"], 
                                RecodingIDs);
                    }
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Error Of Creating List Private Importings: {0}", E.Message)); }
            }
            //
            // Return
            //
            return ListOfPrivateImporters;
        }

        public class PrivateImporter
        {

            #region ' Fields '

            private string FieldOfID;
            private string FieldOfNameOfImporter;
            private bool FieldOfActive;
            private string FieldOfPathOfImporting;
            private bool FieldOfUseOfSystemLogin;
            private string FieldOfMaskOfFileOfImporting;
            private bool FieldOfUseOfRecoding;
            private RecodingID[] FieldOfRecodingIDs;

            #endregion

            #region ' Designer '

            public PrivateImporter(
                string ID, 
                string NameOfImporter, 
                bool Active, 
                string PathOfImporting,  
                bool UseOfSystemLogin, 
                string MaskOfFileOfImporting, 
                bool UseOfRecoding, 
                RecodingID[] RecodingIDs)
            {
                //
                // Initializing Fields
                //
                this.FieldOfID = ID;
                this.FieldOfNameOfImporter = NameOfImporter;
                this.FieldOfActive = Active;
                this.FieldOfPathOfImporting = PathOfImporting;
                this.FieldOfUseOfSystemLogin = UseOfSystemLogin;
                this.FieldOfMaskOfFileOfImporting = MaskOfFileOfImporting;
                this.FieldOfUseOfRecoding = UseOfRecoding;
                this.FieldOfRecodingIDs = RecodingIDs;
            }

            #endregion

            #region ' Parameters '

            // ID
            public string ID
            {
                get { return FieldOfID; }
            }

            // Active
            public bool Active
            {
                get { return FieldOfActive; }
            }

            // Name Of Importer
            public string NameOfImporter
            {
                get { return FieldOfNameOfImporter; }
            }

            // Path Of Importing
            public string PathOfImporting
            {
                get { return FieldOfPathOfImporting; }
            }

            // Use Of System Login
            public bool UseOfSystemLogin
            {
                get { return FieldOfUseOfSystemLogin; }
            }

            // Mask Of File Of Importing
            public string MaskOfFileOfImporting
            {
                get { return FieldOfMaskOfFileOfImporting; }
            }

            // Use Of Recoding
            public bool UseOfRecoding
            {
                get { return FieldOfUseOfRecoding; }
            }

            // Existence Of Recoding IDs
            public bool ExistenceOfRecodingIDs
            {
                get
                {
                    //
                    bool Result = false;
                    if (FieldOfRecodingIDs != null)
                        if (FieldOfRecodingIDs.Length > 0) Result = true;
                    //
                    return Result; 
                }
            }

            // Recoding IDs
            public RecodingID[] GettingRecodingIDs
            {
                get
                {
                    //
                    RecodingID[] ReturnedRecodingIDs = new RecodingID[FieldOfRecodingIDs.Length];
                    for (int i = 0; i < FieldOfRecodingIDs.Length; i++)
                        ReturnedRecodingIDs[i] = FieldOfRecodingIDs[i];
                    //
                    return FieldOfRecodingIDs;
                }
            }

            #endregion

            public struct RecodingID
            {

                #region ' Fields '

                public int IDOfImporter;
                public int IDOfSystem;

                #endregion

                #region ' Designer '

                public RecodingID(int IDOfImporter, int IDOfSystem)
                {
                    //
                    this.IDOfImporter = IDOfImporter;
                    this.IDOfSystem = IDOfSystem;
                }
                
                #endregion

            }

        }

        #endregion


        #region ' Importing Data '


        #region ' Importing Data From Drugstore '

        // Importing Data From Drugstore
        public void ImportingDataFromDrugstore(DataSet ImportedData)
        {
            //
            // !!!
            //
            this.RecordingInLogFile("Starting Importing Data");
            //
            if (ImportedData != null)
                if (ImportedData.DataSetName == "SendingData")
                {
                    //
                    // Getting ID Of Sending Of Drugstore
                    //
                    int IDOfDrugstore = 0;
                    //
                    try
                    { IDOfDrugstore = (int)ImportedData.Tables["Information"].Rows.Find("IDOfDrugstore")["Value"]; }
                    catch (Exception E)
                    { this.RecordingInLogFile(String.Format("Ошибка при получении ID Аптеки: {0}", E.Message)); }
                    //
                    // !!!
                    //
                    if (IDOfDrugstore > 0)
                    {
                        //
                        // Recording Of Reception
                        //
                        int IDOfReception = RecordingOfReception(
                            (int)ImportedData.Tables["Information"].Rows.Find("IDOfDrugstore")["Value"],
                            ImportedData.Tables.Contains("PriceList"),
                            ImportedData.Tables.Contains("AnnouncementsOfDrugstore"),
                            (DateTime)ImportedData.Tables["Information"].Rows.Find("DateOfSending")["Value"]);
                        //
                        // Recording In Log File
                        //
                        this.RecordingInLogFile(String.Format("ID = {0}", IDOfDrugstore));
                        //
                        if (IDOfReception > 0)
                        {
                            //
                            // Importing Service Data
                            //
                            this.RecordingInLogFile("Importing Service Data");
                            //this.RecordingInLogFile("Starting Importing Service Data");
                            //
                            foreach (DataTable CurrentTable in ImportedData.Tables)
                                switch (CurrentTable.TableName)
                                {
                                    case "Information":
                                        { }
                                        break;
                                    case "InformationOfSettings":
                                        UpdatingInformationOfSettings(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "ListOfSettings":
                                        UpdatingListOfSettings(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "RegistrationOfDrugstores":
                                        UpdatingRegistrationOfDrugstores(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "DatesOfTransfer":
                                        UpdatingDatesOfTransfer(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "LogOfDrugstore":
                                        UpdatingLogOfDrugstore(CurrentTable, IDOfDrugstore);
                                        break;
                                }
                            //
                            //this.RecordingInLogFile("Stoping Importing Service Data");
                            //
                            // Importing Of PriceLists
                            //
                            if (DrugstoreIsActive(IDOfDrugstore))
                            {
                                //
                                if (ImportedData.Tables.Contains("AnnouncementsOfDrugstore") ||
                                    ImportedData.Tables.Contains("PriceList"))
                                {
                                    //
                                    this.RecordingInLogFile("Starting Importing Data Of Drugstore");
                                    //
                                    foreach (DataTable CurrentTable in ImportedData.Tables)
                                        switch (CurrentTable.TableName)
                                        {
                                            case "AnnouncementsOfDrugstore":
                                                UpdatingAnnouncementsOfDrugstore(CurrentTable, IDOfDrugstore);
                                                break;
                                            case "PriceList":
                                                ImportingOfPriceList(CurrentTable, IDOfReception);
                                                break;
                                        }
                                    //
                                    this.RecordingInLogFile("Stoping Importing Data Of Drugstore");
                                }
                            }
                            //
                            // Checking Unknown Tables
                            //
                            this.RecordingInLogFile("Checking Unknown Tables");
                            //
                            foreach (DataTable CurrentTable in ImportedData.Tables)
                            {
                                if ((CurrentTable.TableName != "Information") &&
                                    (CurrentTable.TableName != "InformationOfSettings") &&
                                    (CurrentTable.TableName != "ListOfSettings") &&
                                    (CurrentTable.TableName != "RegistrationOfDrugstores") &&
                                    (CurrentTable.TableName != "DatesOfTransfer") &&
                                    (CurrentTable.TableName != "LogOfDrugstore") &&
                                    (CurrentTable.TableName != "AnnouncementsOfDrugstore") &&
                                    (CurrentTable.TableName != "PriceList"))
                                {
                                    //
                                    this.RecordingInLogFile(
                                        String.Format("Неизвестная таблица в наборе данных: {0}",
                                        CurrentTable.TableName));
                                    //
                                }
                            }
                        }
                        else
                            RecordingInLogFile(String.Format("Wrong IDOfReception: {0}", IDOfDrugstore));
                    }
                    else
                        RecordingInLogFile(String.Format("Wrong IDOfDrugstore: {0}", IDOfDrugstore));
                }
                else
                    RecordingInLogFile("DataSet Name Not equally SendingData");
            else
                this.RecordingInLogFile("DataSet Is Null");
            //
            this.RecordingInLogFile("Stoping Importing Data");
            //this.RecordingInLogFile("");
            //
        }

        // Importing Of Data Of PriceList (Importing Only PriceList)
        public void ImportingOfDataOfPriceList(DataTable PriceList, int IDOfDrugstore)
        {
            //
            // !!!
            //
            if (PriceList != null)
            {
                //
                // Importing Of PriceLists
                //
                if (DrugstoreIsActive(IDOfDrugstore))
                {
                    //
                    // Recording Of Reception
                    //
                    int IDOfReception = RecordingOfReception(
                        IDOfDrugstore, (PriceList.Rows.Count > 0) ? true : false, false, DateTime.Now);
                    //
                    // Importing Of Prices
                    //
                    if (IDOfReception > 0)
                        ImportingOfPriceList(PriceList, IDOfReception);
                }
            }
        }


        #region ' Importing Data In Tables Of Service  ' 

        // Updating Information Of Settings(Version Client)
        private void UpdatingInformationOfSettings(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        IPhrmInf.EFPhrmInf.UpdatingInformationOfSettings(IDOfDrugstore, row["key"].ToString(), row["value"].ToString());
                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении регистрационных данных аптеки {0}", IDOfDrugstore));
                    }
                }
                IPhrmInf.EFPhrmInf.SaveChanges();
            }            
        }

        // Updating List Of Settings
        private void UpdatingListOfSettings(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        IPhrmInf.EFPhrmInf.UpdatingListOfSettings(IDOfDrugstore, row["key"].ToString(), row["value"].ToString());
                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении данных настройки аптеки {0}", IDOfDrugstore));
                    }
                }
                IPhrmInf.EFPhrmInf.SaveChanges();
            }
        }

        // Updating Registration Of Drugstores
        private void UpdatingRegistrationOfDrugstores(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        IPhrmInf.EFPhrmInf.UpdatingRegistrationOfDrugstores(IDOfDrugstore, Convert.ToInt32(row["ID"]), row["PathToFolderOfPriceLists"].ToString(),
                            row["MaskOfFullPriceList"].ToString(), row["MaskOfIncomingPriceList"].ToString(), row["MaskOfSoldPriceList"].ToString(),
                            Convert.ToBoolean(row["UseOfIDOfPriceList"]), Convert.ToBoolean(row["NotDeletingPriceList"]));
                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении регистрационных данных аптеки {0}", IDOfDrugstore));
                    }
                }
                IPhrmInf.EFPhrmInf.SaveChanges();
            }
        }

        // Updating Dates Of Transfer
        private void UpdatingDatesOfTransfer(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        IPhrmInf.EFPhrmInf.UpdatingDatesOfTransfer(IDOfDrugstore, Convert.ToInt32(row["ID"]), row["Name"].ToString(), Convert.ToInt32(row["Value"]), Convert.ToDateTime(row["Date"]));

                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении информации об обновлениях справочных данных аптеки {0}", IDOfDrugstore));
                    }
                }
                IPhrmInf.EFPhrmInf.SaveChanges();
            }
        }

        // Updating Log Of Drugstore
        private void UpdatingLogOfDrugstore(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        IPhrmInf.EFPhrmInf.UpdatingLogsOfDrugstores(IDOfDrugstore, row["Value"].ToString());

                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении данных лога аптеки {0}", IDOfDrugstore));
                    }
                }
                IPhrmInf.EFPhrmInf.SaveChanges();
            }
        }

        // Updating Announcements Of Drugstore
        private void UpdatingAnnouncementsOfDrugstore(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        IPhrmInf.EFPhrmInf.UpdatingAnnouncementsOfDrugstore(IDOfDrugstore, Convert.ToInt32(row["ID"]), row["Caption"].ToString(), row["Text"].ToString(), Convert.ToBoolean(row["Published"]));

                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении таблицы объявлений, полученных аптекой {0}", IDOfDrugstore));
                    }
                }
                IPhrmInf.EFPhrmInf.SaveChanges();
            }            
        }

        #endregion


        #region ' Import Of PriceList '

        // Import Of PriceList
        private void ImportingOfPriceList(DataTable PriceList, int IDOfReception)
        {
            //
            // Converting UInt32 In Int32
            //
            PriceList.Columns.Add("ID_PH2", typeof(Int32));
            PriceList.Columns.Add("ID_PR2", typeof(Int32));
            //
            foreach (DataRow CurrentRow in PriceList.Rows)
            {
                //
                try
                {
                    CurrentRow["ID_PH2"] = Convert.ToInt32(CurrentRow["ID_PH"]);
                    CurrentRow["ID_PR2"] = Convert.ToInt32(CurrentRow["ID_PR"]);
                }
                catch { this.RecordingInLogFile("Ошибка при конвертации ID_PH and ID_PR"); }
            }
            //
            try
            {
                if (PriceList.PrimaryKey.Length != 0)
                    PriceList.PrimaryKey = new DataColumn[2] { PriceList.Columns["ID_PH2"], PriceList.Columns["ID_PR2"] };
            }
            catch { this.RecordingInLogFile("asl"); }
            //
            PriceList.Columns.Remove("ID_PH");
            PriceList.Columns.Remove("ID_PR");
            //
            PriceList.Columns["ID_PH2"].ColumnName = "ID_PH";
            PriceList.Columns["ID_PR2"].ColumnName = "ID_PR";
            //
            // Converting Null And DBNUll in 0
            //
            foreach (DataRow CurrentPrice in PriceList.Rows)
                if ((CurrentPrice["Price"] == null) || (CurrentPrice["Price"] is DBNull))
                    CurrentPrice["Price"] = 0;
            //
            // Getting IDs Of Drugstores
            //
            DataTable TableOfIDsOfDrugstores = new DataView(PriceList).ToTable(true, "ID_PH");
            int[] IDsOfDrugstores = new int[TableOfIDsOfDrugstores.Rows.Count];
            for (int i = 0; i < IDsOfDrugstores.Length; i++)
                IDsOfDrugstores[i] = (int)TableOfIDsOfDrugstores.Rows[i]["ID_PH"];
            //
            // Checking Existence Of Drugstores
            //
            System.Collections.ArrayList CheckedIDs = new System.Collections.ArrayList();
            foreach (int CurrentID in IDsOfDrugstores)
            {
                if (DrugstoreIsActive(CurrentID))
                    CheckedIDs.Add(CurrentID);
            }
            IDsOfDrugstores = new int[0];
            IDsOfDrugstores = (int[]) CheckedIDs.ToArray(typeof(int));
            //
            // Recording In Reports Of Importing
            //
            bool Successful = true;
            foreach (int CurrentID in IDsOfDrugstores)
            { Successful = RecordingInReportsOfImporting(CurrentID, IDOfReception); if (!Successful) break; }
            //
            // Importing Of Drugstores
            //
            //int CountOfImporting = 0;
            DataView GettingDrugstore = new DataView(PriceList);
            for (int i = 0; ((i < IDsOfDrugstores.Length) && Successful); i++)
            {
                //
                // Getting Prices Of Drugstore
                //
                GettingDrugstore.RowFilter = String.Format("ID_PH={0}", IDsOfDrugstores[i]);
                DataTable PricesOfDrugstore = GettingDrugstore.ToTable("PricesOfDrugstore");
                //
                // Addition Of TMP Of ID Of Prices
                //
                PricesOfDrugstore.Columns.Add(new DataColumn("ID", typeof(int)));
                for (int i2 = 0; i2 < PricesOfDrugstore.Rows.Count; i2++)
                    PricesOfDrugstore.Rows[i2]["ID"] = (i2 + 1);
                PricesOfDrugstore.PrimaryKey = new DataColumn[1] { PricesOfDrugstore.Columns["ID"] };
                //
                // Filling ID Of Prices And ID_PR Of Products
                //
                int[,] IDAndIDPROfPrices = new int[PricesOfDrugstore.Rows.Count, 2];
                for (int i2 = 0; i2 <= IDAndIDPROfPrices.GetUpperBound(0); i2++)
                {
                    IDAndIDPROfPrices[i2, 0] = (int)PricesOfDrugstore.Rows[i2]["ID"];
                    IDAndIDPROfPrices[i2, 1] = (int)PricesOfDrugstore.Rows[i2]["ID_PR"];
                }
                //
                // Search And Liquidation Of Recurrences
                //
                for (int i2 = 0; i2 <= IDAndIDPROfPrices.GetUpperBound(0); i2++)
                    for (int i3 = 0; i3 <= IDAndIDPROfPrices.GetUpperBound(0); i3++)
                        if ((i2 != i3) && (i2 > i3) && (IDAndIDPROfPrices[i2, 1] == IDAndIDPROfPrices[i3, 1]))
                        {
                            DataRow GetRow = PricesOfDrugstore.Rows.Find(IDAndIDPROfPrices[i3, 0]);
                            if (GetRow != null)
                                GetRow.Delete();
                        }
                //
                PricesOfDrugstore.AcceptChanges();
                //
                // Getting AllPrices From Drugstore
                //
                bool AllPrices = false;
                for (int i2 = 0; i2 < PricesOfDrugstore.Rows.Count; i2++)
                    if (!(PricesOfDrugstore.Rows[i2]["AllPrices"] is DBNull))
                        if ((bool)PricesOfDrugstore.Rows[i2]["AllPrices"])
                        { AllPrices = true; break; }
                //
                // Recording In Reports Of Importing Of PriceList
                //
                UpdatingReportsOfImporting(
                    IDsOfDrugstores[i], IDOfReception, PricesOfDrugstore.Rows.Count, AllPrices);
                //
                // Clearing Columns Of Drugstore
                //
                DataView FilteringOfColumns = new DataView(PricesOfDrugstore);
                PricesOfDrugstore =
                    FilteringOfColumns.ToTable(
                    "PricesOfDrugstore", true, "ID_PR", "Price", "Deleting", "Preferential");
                PricesOfDrugstore.PrimaryKey = new DataColumn[] { PricesOfDrugstore.Columns["ID_PR"] };
                //
                // Importing Of Prices Of Drugstore
                //
                ImportingOfPricesOfDrugstore(IDOfReception, IDsOfDrugstores[i], AllPrices, PricesOfDrugstore);
                //
            }
        }

        // Importing Of PriceList Of Drugstore
        private void ImportingOfPricesOfDrugstore(int IDOfReception, int IDOfDrugstore, bool AllPrices, DataTable PriceList)
        {
            //
            DataTable PricesOfDrugstore = PriceList.Copy();

            int CountOfModification = 0;
            //
            // Refreshing Of Dates
            //
            if ((AllPrices) && (PricesOfDrugstore.Rows.Count >= 1))// && PricesOfDrugstore.Rows.Count > 100) // !!! Отключение ограничения
            {
                //
                // Creating List Of Id_Product For Updating Deleting. The actual prices of pharmacy products that are not listed in the incoming PriceList should be marked as deleted
                //

                // Take Id_Product actual prices
                DataTable IDsOfProducts = new DataTable();
                IPhrmInf.EFPhrmInf.price_list.Where(p => p.Id_Pharmacy == IDOfDrugstore && !p.Is_deleted)
                    .Select(p => new { p.Id_Product }).Fill(ref IDsOfProducts);
                
                // Create table for product that should be marked deleted in pricelist
                DataTable IDsForDeleting = new DataTable();
                IDsForDeleting.Columns.Add("ID", typeof(int));
                //
                foreach (DataRow CurrentIDOfProduct in IDsOfProducts.Rows)
                {
                    bool Addition = true;
                    foreach (DataRow CurrentPriceOfDrugstore in PricesOfDrugstore.Rows)
                        if (((int)CurrentIDOfProduct[0]) == ((int)CurrentPriceOfDrugstore["ID_PR"]))
                        { Addition = false; break; }
                    if (Addition)
                        IDsForDeleting.Rows.Add(CurrentIDOfProduct[0]);
                }
                //
                CountOfModification += IDsForDeleting.Rows.Count;
                //
                // Creating Command Of Updating Deleting
                //
                IEnumerable<dynamic> IDsForDeleting_IE = IDsForDeleting.AsEnumerable();

                IEnumerable<int> IDsForDeleting_IE_i = IDsForDeleting_IE.Select(p => (int) p.ID);

                IEnumerable<HistoryOfChangesOfPrice> IDsForDeleting_IE_HP = IDsForDeleting_IE
                    .Select(p => new HistoryOfChangesOfPrice
                    {
                        IDOfDrugstore = IDOfDrugstore,
                        IDOfProduct = (int)p.ID,
                        ModificationOfPrice = 3,
                        ModifiedPrice = 0,
                        DateOfChange = DateTime.Now
                    });

                try
                {
                    //Check all active price in Pharmacy from resived price_list and which not listed in resived price_list as deleted
                    IPhrmInf.EFPhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore && !pl.Is_deleted)
                        .Join(IDsForDeleting_IE_i, p => p.Id_Product, pt => pt, (p, pt) => p)
                        .UpdateAsync(pl => new price_list { Is_deleted = true });

                    //Set this changes in history of price
                    IPhrmInf.EFPhrmInf.HistoryOfChangesOfPrices.AddRange(IDsForDeleting_IE_HP);
                }
                catch (Exception E)
                {
                    //
                    RecordingInLogFile(String.Format("Ошибка при пометке на удаление: {0}", E.Message));
                }
                //
                // Recording In Reports Of Importing Of PriceList
                //
                UpdatingReportsOfImporting(IDOfDrugstore, IDOfReception, IDsForDeleting.Rows.Count);
            }
           
            ////
            //// Updating and Inserting global price_list from recived prices
            ////

            IEnumerable<PriceListDrugstore> PricesOfDrugstore_IE = PricesOfDrugstore.AsEnumerable()
                .Select(p => new PriceListDrugstore { ID_PR = p.ID_PR, Price = p.Price, Deleting = p.Deleting, Preferential = p.Preferential }).ToArray();


            try
            {
                //update existing prices which presents in recived price_list
                PricesOfDrugstore_IE.Join(IPhrmInf.EFPhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore),
                        pld => pld.ID_PR, p => p.Id_Product, (pld, p) => pld)
                    .Select(pld => IPhrmInf.EFPhrmInf.UpdatingPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential))
                    .ToList();

                //Insert new prices, which not exists in global price_list
                PricesOfDrugstore_IE.Where(pld => !IPhrmInf.EFPhrmInf.price_list.Where(p => p.Id_Product == pld.ID_PR && p.Id_Pharmacy == IDOfDrugstore).Any())
                    .Select(pld => IPhrmInf.EFPhrmInf.InsertingInPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential))
                    .ToList();
            }
            catch (Exception E)
            {
                RecordingInLogFile(String.Format("Ошибка при обновлении существующих и вставке новых позиций из файла остатков: {0}", E.Message));
            }

            IPhrmInf.EFPhrmInf.SaveChanges();

            //
            // Updating Date Of Actuals
            //
            if ((CountOfModification > 0) && (AllPrices == true))
            {
                //
                UpdatingDateOfActuals(IDOfDrugstore);
            }
            //
        }

        // Reading Status
        private void ReadingStatus(DataTable TableForReading)
        {
            //
            int S_M = 0, S_A = 0, S_R = 0, S_U = 0;
            for (int i = 0; i < TableForReading.Rows.Count; i++)
                if (TableForReading.Rows[i].RowState == DataRowState.Added)
                    S_A++;
                else if (TableForReading.Rows[i].RowState == DataRowState.Deleted)
                    S_R++;
                else if (TableForReading.Rows[i].RowState == DataRowState.Modified)
                    S_M++;
                else if (TableForReading.Rows[i].RowState == DataRowState.Unchanged)
                    S_U++;
            RecordingInLogFile(
                String.Format("CountR={4} Add={0} Rem={1} Mod={2} Unc={3}",
                S_A, S_R, S_M, S_U, TableForReading.Rows.Count));
        }

        // Recording In Reports Of Importing
        private bool RecordingInReportsOfImporting(int IDOfDrugstore, int IDOfReception)
        {
            //
            bool Successful = true;
            //
            // Creating Command Of Recording
            //

            ReportsOfImportingOfPriceList ripl = new ReportsOfImportingOfPriceList
            {
                ID_PH = IDOfDrugstore,
                ID_HR = IDOfReception,
                CountNotConfirmed = 0,
                CountOfAdditions = 0,
                CountOfUnAdditions = 0,
                CountOfChanges = 0,
                CountOfUnChanges = 0,
                CountOfDeletings = 0,
                CountOfAllPrices = 0
            };

            try
            {
                IPhrmInf.EFPhrmInf.ReportsOfImportingOfPriceLists.Add(ripl);
                IPhrmInf.EFPhrmInf.SaveChanges();
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при записе в ReportsOfImportingOfPriceLists: {0}", E.Message));
                Successful = false;
            }
            // Return
            return Successful;
        }

        // Updating Reports Of Importing
        private void UpdatingReportsOfImporting(int IDOfDrugstore, int IDOfReception, int CountOfPrices, bool FullPriceList)
        {
            //
            // Creating Command Of Recording
            //
            try
            {
                IPhrmInf.EFPhrmInf.ReportsOfImportingOfPriceLists.Where(ripl => ripl.ID_PH == IDOfDrugstore && ripl.ID_HR == IDOfReception).
                   Update(ripl_n => new ReportsOfImportingOfPriceList { CountOfAllPrices = CountOfPrices, FullPriceList = FullPriceList });
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении ReportsOfImportingOfPriceLists: {0}", E.Message));
            }
        }

        // Updating Reports Of Importing
        private void UpdatingReportsOfImporting(int IDOfDrugstore, int IDOfReception, int CountOfDeleting)
        {
            //
            // Creating Command Of Recording
            //
            try
            {
                IPhrmInf.EFPhrmInf.ReportsOfImportingOfPriceLists.Where(ripl => ripl.ID_PH == IDOfDrugstore && ripl.ID_HR == IDOfReception).
                   Update(ripl_n => new ReportsOfImportingOfPriceList { CountNotConfirmed = CountOfDeleting });
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении ReportsOfImportingOfPriceLists: {0}", E.Message));
            }
        }

        // Updating Date Of Actuals
        private void UpdatingDateOfActuals(int IDOfDrugstore)
        {
            //
            // Creating Command Of Updating
            //            
            try
            {
                IPhrmInf.EFPhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore && !pl.Is_deleted).
                    Update(plu => new price_list { Actual = DateTime.Now });
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении price_list (Date Of Actuals): {0}", E.Message));
            }
        }

        #endregion


        #region ' Checking '

        // Drugstore Is Active
        public bool DrugstoreIsActive(int IDOfDrugstore)
        {
            bool ResultOfActivation = false;

            try
            {
                int CountOfDrugstore = IPhrmInf.EFPhrmInf.Pharmacies.Where(p => !p.Is_deleted && p.Id_Pharmacy == IDOfDrugstore).Count();
                //
                ResultOfActivation = (CountOfDrugstore > 0) ? true : false;
            }
            catch (Exception E)
            {
                RecordingInLogFile(String.Format("ERROR Ошибка при проверке активации аптеки: {0}", E.Message));
            }
            //
            // NO 103
            //
            if (IDOfDrugstore == 103)
                ResultOfActivation = false;
            //
            // Return
            //
            return ResultOfActivation;
        }

        #endregion


        #endregion

        // Recording Of Reception
        private int RecordingOfReception(
            int IDOfDrugstore, bool ContainsPriceList, bool ContainsAnnouncements, DateTime LocalDateOfSending)
        {
            //
            // Generation New HistoryOfReception
            //
            HistoryOfReception hr = new HistoryOfReception
            {
                ID_PH = IDOfDrugstore,
                ContainsPriceList = ContainsPriceList,
                ContainsAnnouncements = ContainsAnnouncements,
                DateOfReception = DateTime.Now,
                LocalDateOfSending = LocalDateOfSending
            };            
            //
            // Executing
            //
            try
            {
                IPhrmInf.EFPhrmInf.HistoryOfReceptions.Add(hr);
                IPhrmInf.EFPhrmInf.SaveChanges();
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("Ошибка при регистрации приема данных ID {0}: {1}",
                    IDOfDrugstore, E.Message));
            }
            //
            // Getting ID Of Reception Of Data
            //
            int IDOfReception = 0;
            try
            {
                IDOfReception = IPhrmInf.EFPhrmInf.HistoryOfReceptions.Max(i => i.ID);
            }
            catch { this.RecordingInLogFile("Ошибка при получении IDOfReception"); }
            //
            // Return
            //
            return IDOfReception;
        }


        #endregion

    }
}