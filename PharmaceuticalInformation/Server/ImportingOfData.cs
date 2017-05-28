using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Server
{
    public class ImportingOfData : BaseType
    {

        #region ' Fields '

        //
        private SqlConnection ConnectionToBase;
        private SqlDataAdapter _UpdatingOfData;
        //
        private Updating.UpdatingOfDataOfInformationForMsSQL UpdatingOfData;

        #endregion


        #region ' Designer '

        public ImportingOfData(string StringOfConnection)
            : this(StringOfConnection, "")
        {
            //
        }

        public ImportingOfData(string StringOfConnection, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initializing Fields
            //
            //this.StringOfConnection = StringOfConnection;
            //
            _UpdatingOfData = new SqlDataAdapter();
            _UpdatingOfData.ContinueUpdateOnError = true;
            //
            // Creating Of Connection
            //
            try
            {
                ConnectionToBase = new SqlConnection(StringOfConnection);
                ConnectionToBase.Open();
                ConnectionToBase.Close();
            }
            catch (Exception E) { throw new Exception(String.Format("Ошибка при создании подключения экспорта: {0}", E)); }
            //
            // !!!
            //
            UpdatingOfData = new Updating.UpdatingOfDataOfInformationForMsSQL(StringOfConnection, PathToLogFile);
        }

        #endregion


        #region ' Creating '

        // Creating Command
        private DbCommand CreatingCommand(string TextOfCommand, DbParameter[] ParametersOfCommand)
        {
            //
            DbCommand CreatedCommand = new SqlCommand(TextOfCommand, ConnectionToBase);
            //
            for (int i = 0; i <= ParametersOfCommand.GetUpperBound(0); i++)
                CreatedCommand.Parameters.Add(ParametersOfCommand[i]);
            // Return
            return CreatedCommand;
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
            //
            string TextOfInquiryOfPrivateImportings = 
                "SELECT ID, NameOfImporter, Active, PathOfImporting, UseOfSystemLogin, MaskOfFileOfImporting, UseOfRecoding FROM PrivateImportings; " +
                "SELECT ID, IDOfPrivateImportings, IDOfImporter, IDOfSystem FROM RecodingIDsOfDrugstoresOfImportings;";
            SqlCommand CommandOfSelectionOfPrivateImportings = 
                new SqlCommand(TextOfInquiryOfPrivateImportings, ConnectionToBase);
            SqlDataAdapter GettingPrivateImportings = new SqlDataAdapter(CommandOfSelectionOfPrivateImportings);
            //
            try
            {
                GettingPrivateImportings.FillSchema(TablesOfPrivateImporters, SchemaType.Source);
                GettingPrivateImportings.Fill(TablesOfPrivateImporters);
            }
            catch (Exception E)
            {
                //
                RecordingInLogFile(String.Format("ERROR Error Of Getting List Private Importers: {0}", E.Message));
                //
                if (ConnectionToBase.State == ConnectionState.Open)
                    ConnectionToBase.Close();
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

        // Updating Information Of Settings
        private void UpdatingInformationOfSettings(DataTable TableForUpdating, int IDOfDrugstore)
        {
            //
            // Name Of Procedure
            //
            string TextOfCommandOfUpdating = "UpdatingInformationOfSettings";
            //
            // Creating Parameters Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[3] {
                new SqlParameter("@IDOfDrugstore", SqlDbType.Int                 ),
                new SqlParameter("@Key",           SqlDbType.VarChar,  0, "Key"  ), 
                new SqlParameter("@Value",         SqlDbType.VarChar,  0, "Value")};
            //
            ParametersOfUpdatingCommand[0].Value = IDOfDrugstore;
            //
            // Updating
            //
            UpdatingTableOfService(
                TableForUpdating, TextOfCommandOfUpdating, "InformationOfSettings", ParametersOfUpdatingCommand);
            //
        }

        // Updating List Of Settings
        private void UpdatingListOfSettings(DataTable TableForUpdating, int IDOfDrugstore)
        {
            //
            // Name Of Procedure
            //
            string TextOfCommandOfUpdating = "UpdatingListOfSettings";
            //
            // Creating Parameters Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[3] {
                new SqlParameter("@IDOfDrugstore", SqlDbType.Int             ),
                new SqlParameter("@Key",           SqlDbType.VarChar,  0, "Key"), 
                new SqlParameter("@Value",         SqlDbType.VarChar,  0, "Value")};
            //
            ParametersOfUpdatingCommand[0].Value = IDOfDrugstore;
            //
            // Updating
            //
            UpdatingTableOfService(
                TableForUpdating, TextOfCommandOfUpdating, "ListOfSettings", ParametersOfUpdatingCommand);
            //
        }

        // Updating Registration Of Drugstores
        private void UpdatingRegistrationOfDrugstores(DataTable TableForUpdating, int IDOfDrugstore)
        {
            //
            // Clearing Registrated Drugstores
            //
            if (IDOfDrugstore > 0)
            {
                //
                // Creating Clearing Command
                //
                SqlCommand ClearingRegistration =
                    new SqlCommand(
                        String.Format(
                        "DELETE FROM RegistrationOfDrugstores WHERE ID_PH = {0}", IDOfDrugstore), ConnectionToBase);
                //
                // Executing Clearing
                //
                try
                {
                    //
                    OpeningConnection(ClearingRegistration.Connection);
                    //
                    // Executing
                    //
                    try { ClearingRegistration.ExecuteScalar(); }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при очистке регистраций аптеки: {0}", E.Message));
                    }
                    //
                    ClosingConnection(ClearingRegistration.Connection);
                }
                catch (Exception E)
                {
                    this.RecordingInLogFile(
                      String.Format("ERROR Ошибка при Открытии/Закрытии подключения очистки регистраций: {0}", E.Message));
                }
                //
                //ClosingConnection(ClearingRegistration.Connection);
            }
            //
            // Name Of Procedure
            //
            string TextOfCommandOfUpdating = "UpdatingRegistrationOfDrugstores";
            //
            // Creating Parameters Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[8] {
                new SqlParameter("@IDOfDrugstore",            SqlDbType.Int             ),
                new SqlParameter("@ID",                       SqlDbType.Int,     0, "ID"), 
                new SqlParameter("@PathToFolderOfPriceLists", SqlDbType.VarChar, 0, "PathToFolderOfPriceLists"), 
                new SqlParameter("@MaskOfFullPriceList",      SqlDbType.VarChar, 0, "MaskOfFullPriceList"     ), 
                new SqlParameter("@MaskOfIncomingPriceList",  SqlDbType.VarChar, 0, "MaskOfIncomingPriceList" ), 
                new SqlParameter("@MaskOfSoldPriceList",      SqlDbType.VarChar, 0, "MaskOfSoldPriceList"     ), 
                new SqlParameter("@UseOfIDOfPriceList",       SqlDbType.Bit,     0, "UseOfIDOfPriceList"      ), 
                new SqlParameter("@NotDeletingPriceList",     SqlDbType.Bit,     0, "NotDeletingPriceList"    )};
            //
            ParametersOfUpdatingCommand[0].Value = IDOfDrugstore;
            //
            // Updating
            //
            UpdatingTableOfService(
                TableForUpdating, TextOfCommandOfUpdating, "RegistrationOfDrugstores", ParametersOfUpdatingCommand);
            //
        }

        // Updating Dates Of Transfer
        private void UpdatingDatesOfTransfer(DataTable TableForUpdating, int IDOfDrugstore)
        {
            //
            // Converting Type Of Column Of ID
            //
            bool Successful = true;
            TableForUpdating.Columns.Add("ID_02", typeof(int));
            try
            {
                foreach (DataRow CurrentDate in TableForUpdating.Rows)
                    CurrentDate["ID_02"] = Convert.ToInt32(CurrentDate["ID"]);
            }
            catch { Successful = false; this.RecordingInLogFile("Ошибка при конвертировании ID дат"); }
            //
            TableForUpdating.PrimaryKey = new DataColumn[1] { TableForUpdating.Columns["ID_02"] };
            TableForUpdating.Columns.Remove("ID");
            TableForUpdating.Columns["ID_02"].ColumnName = "ID";
            //
            if (Successful)
            {
                //
                // Name Of Procedure
                //
                string TextOfCommandOfUpdating = "UpdatingDatesOfTransfer";
                //
                // Creating Parameters Of Updating
                //
                DbParameter[] ParametersOfUpdatingCommand = new DbParameter[5] {
                    new SqlParameter("@IDOfDrugstore", SqlDbType.Int                 ),
                    new SqlParameter("@ID",            SqlDbType.Int,      0, "ID"   ), 
                    new SqlParameter("@Name",          SqlDbType.VarChar,  0, "Name" ), 
                    new SqlParameter("@Value",         SqlDbType.Int,      0, "Value"), 
                    new SqlParameter("@Date",          SqlDbType.DateTime, 0, "Date" )};
                //
                ParametersOfUpdatingCommand[0].Value = IDOfDrugstore;
                //
                // Updating
                //
                UpdatingTableOfService(
                    TableForUpdating, TextOfCommandOfUpdating, "DatesOfTransfer", ParametersOfUpdatingCommand);
                //
            }
        }

        // Updating Log Of Drugstore
        private void UpdatingLogOfDrugstore(DataTable TableForUpdating, int IDOfDrugstore)
        {
            //
            // Name Of Procedure
            //
            string TextOfCommandOfUpdating ="UpdatingLogsOfDrugstores";
            //
            // Creating Parameters Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[2] {
                new SqlParameter("@IDOfDrugstore", SqlDbType.Int                 ),
                new SqlParameter("@SystemLog",     SqlDbType.VarChar,  0, "Value")};
            //
            ParametersOfUpdatingCommand[0].Value = IDOfDrugstore;
            //
            // Updating
            //
            UpdatingTableOfService(
                TableForUpdating, TextOfCommandOfUpdating, "LogsOfDrugstores", ParametersOfUpdatingCommand);
            //
        }

        // Updating Announcements Of Drugstore
        private void UpdatingAnnouncementsOfDrugstore(DataTable TableForUpdating, int IDOfDrugstore)
        {
            //
            // Name Of Procedure
            //
            string TextOfCommandOfUpdating = "UpdatingAnnouncementsOfDrugstore";
            //
            // Creating Parameters Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[5] {
                new SqlParameter("@IDOfDrugstore", SqlDbType.Int                    ),
                new SqlParameter("@ID",            SqlDbType.Int,     0, "ID"       ), 
                new SqlParameter("@Caption",       SqlDbType.VarChar, 0, "Caption"  ), 
                new SqlParameter("@Text",          SqlDbType.VarChar, 0, "Text"     ), 
                new SqlParameter("@Published",     SqlDbType.Bit,     0, "Published")};
            //
            ParametersOfUpdatingCommand[0].Value = IDOfDrugstore;
            //
            // Updating
            //
            UpdatingTableOfService(
                TableForUpdating, TextOfCommandOfUpdating, "Announcements", ParametersOfUpdatingCommand);
            //
        }

        // Updating Table Of Service
        private void UpdatingTableOfService(
            DataTable TableForUpdating, string TextOfCommand, string NameOfTable, DbParameter[] ParametersOfCommand)
        {
            //
            // Status Of Modified
            //
            TableForUpdating.AcceptChanges();
            foreach (DataRow CurrentRow in TableForUpdating.Rows)
                CurrentRow.SetModified();
            /*if (NameOfTable != "LogsOfDrugstores")
                CurrentRow.SetModified();
            else
                CurrentRow.SetAdded();*/
            //
            //if (NameOfTable != "LogsOfDrugstores")
            _UpdatingOfData.UpdateCommand = (SqlCommand)CreatingCommand(TextOfCommand, ParametersOfCommand);
            /*else
                _UpdatingOfData.InsertCommand = (SqlCommand)CreatingCommand(TextOfCommand, ParametersOfCommand);*/
            //
            //if (NameOfTable != "LogsOfDrugstores")
            _UpdatingOfData.UpdateCommand.CommandType = CommandType.StoredProcedure;
            //
            // Updating
            //
            UpdateOfUpdatingData(TableForUpdating, NameOfTable);
            //
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
                /*
                 object Obj = PricesOfDrugstore.Rows[i2]["AllPrices"];
                 Console.WriteLine(Obj is bool);
                 Console.WriteLine(Obj == null);
                 Console.WriteLine(Obj is DBNull);
                 */
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
                //CountOfImporting += PricesOfDrugstore.Rows.Count;
                ImportingOfPricesOfDrugstore(IDOfReception, IDsOfDrugstores[i], AllPrices, PricesOfDrugstore);
                //
            }
            // Return
            //return;// CountOfImporting;
        }

        // Importing Of PriceList Of Drugstore
        private void ImportingOfPricesOfDrugstore(int IDOfReception, int IDOfDrugstore, bool AllPrices, DataTable PriceList)
        {
            //
            DataTable PricesOfDrugstore = PriceList.Copy();
            //SqlConnection ConnectionToBase = new SqlConnection(StringOfConnection);
            int CountOfModification = 0;
            //
            // Refreshing Of Dates
            //
            if ((AllPrices) && (PricesOfDrugstore.Rows.Count >= 1))// && PricesOfDrugstore.Rows.Count > 100) // !!! Отключение ограничения
            {
                //
                // Creating List Of ID_PR For Updating Deleting
                //
                SqlCommand CommandOfSelection = new SqlCommand(
                    String.Format(
                    "SELECT ID_Product FROM Price_List WHERE ((Id_Pharmacy = {0}) AND (Is_Deleted = 0));",
                    IDOfDrugstore),
                    ConnectionToBase);
                //
                SqlDataAdapter GettingIDsOfProducts = new SqlDataAdapter(CommandOfSelection);
                DataTable IDsOfProducts = new DataTable();
                GettingIDsOfProducts.FillSchema(IDsOfProducts, SchemaType.Source);
                GettingIDsOfProducts.Fill(IDsOfProducts);
                //
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
                // Assignment Of Status Of Rows
                //
                IDsForDeleting.AcceptChanges();
                foreach (DataRow CurrentIDForDeleting in IDsForDeleting.Rows)
                    CurrentIDForDeleting.SetModified();
                //
                // Creating Command Of Updating Deleting
                //
                SqlCommand UpdatingOfDeletingOfPricesOfDrugstore = 
                    new SqlCommand(
                        String.Format(
                        "UPDATE Price_List " + 
                        "SET Date_upd = GetDate(), Is_deleted = 1 " + 
                        "WHERE (Id_Pharmacy = {0}) AND (Id_Product = @P1) AND (Is_deleted = 0); " + 
                        "INSERT INTO HistoryOfChangesOfPrices( " + 
                        "IDOfDrugstore, IDOfProduct, ModificationOfPrice, ModifiedPrice, DateOfChange) " + 
                        "VALUES({0}, @P1, 3, 0, GetDate());", 
                        IDOfDrugstore), 
                        ConnectionToBase);
                //
                UpdatingOfDeletingOfPricesOfDrugstore.Parameters.Add("@P1", SqlDbType.Int, 0, "ID");
                //
                // Assignment Of Commands
                //
                _UpdatingOfData.ContinueUpdateOnError = true;
                _UpdatingOfData.UpdateCommand = UpdatingOfDeletingOfPricesOfDrugstore;
                //
                // Updating
                //
                try { UpdateOfUpdatingData(IDsForDeleting, String.Format("Price_list Deleting {0}", IDOfDrugstore)); }
                catch (Exception E)
                {
                    //
                    if (ConnectionToBase.State == ConnectionState.Open)
                        ConnectionToBase.Close();
                    //
                    RecordingInLogFile(String.Format("Ошибка при пометке на удаление: {0}", E.Message));
                }
                //
                // Recording In Reports Of Importing Of PriceList
                //
                UpdatingReportsOfImporting(IDOfDrugstore, IDOfReception, IDsForDeleting.Rows.Count);
            }
            //
            // Creating Command Of Reading Of Status Of Rows
            //
            DbParameter[] ParametersOfSelectionCommand = new DbParameter[1] { 
                    new SqlParameter("@P1", SqlDbType.Int, 0, "ID_PR") };
            //
            SetStatusOfRows(PricesOfDrugstore,
                "", 
                String.Format(
                "(EXISTS(SELECT * FROM Price_list WHERE ((ID_Pharmacy = {0}) AND (Id_Product = @P1))))", 
                IDOfDrugstore), 
                ParametersOfSelectionCommand);
            //
            // Calculation Count Of Rows
            //
            /*int CountOfModifyingRows = 0;
            foreach (DataRow CurrentRow in PricesOfDrugstore.Rows)
                if ((CurrentRow.RowState == DataRowState.Added) || (CurrentRow.RowState == DataRowState.Modified))
                    CountOfModifyingRows++;*/
            CountOfModification += PricesOfDrugstore.GetChanges().Rows.Count;
            //
            ReadingStatus(PricesOfDrugstore);
            //
            // Creating Parameters Of Procedure Of Inserting
            //
            DbParameter[] ParametersOfInsertingCommand = new DbParameter[6] {
                new SqlParameter("@IDOfDrugstore", SqlDbType.Int                ), 
                new SqlParameter("@IDOfReception", SqlDbType.Int                ), 
                new SqlParameter("@P1",            SqlDbType.Int,     0, "ID_PR"), 
                new SqlParameter("@P2",            SqlDbType.Decimal, 0, "Price"), 
                new SqlParameter("@P3",            SqlDbType.Bit,     0, "Deleting"),
                new SqlParameter("@P4",            SqlDbType.Bit,     0, "Preferential")};
            //
            SqlCommand CommandOfInsertingPriceList =
                (SqlCommand)CreatingCommand("InsertingInPriceList", ParametersOfInsertingCommand);
            CommandOfInsertingPriceList.CommandType = CommandType.StoredProcedure;
            //
            CommandOfInsertingPriceList.Parameters["@IDOfDrugstore"].Value = IDOfDrugstore;
            CommandOfInsertingPriceList.Parameters["@IDOfReception"].Value = IDOfReception;
            //
            // Creating Parameters Of Procedure Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[6] {
                new SqlParameter("@IDOfDrugstore", SqlDbType.Int                ), 
                new SqlParameter("@IDOfReception", SqlDbType.Int                ), 
                new SqlParameter("@P1",            SqlDbType.Int,     0, "ID_PR"), 
                new SqlParameter("@P2",            SqlDbType.Decimal, 0, "Price"), 
                new SqlParameter("@P3",            SqlDbType.Bit,     0, "Deleting"), 
                new SqlParameter("@P4",            SqlDbType.Bit,     0, "Preferential")};
            //
            SqlCommand CommandOfUpdatingPriceList =
                (SqlCommand)CreatingCommand("UpdatingPriceList", ParametersOfUpdatingCommand);
            CommandOfUpdatingPriceList.CommandType = CommandType.StoredProcedure;
            //
            CommandOfUpdatingPriceList.Parameters["@IDOfDrugstore"].Value = IDOfDrugstore;
            CommandOfUpdatingPriceList.Parameters["@IDOfReception"].Value = IDOfReception;
            //
            // Assignment Of Commands
            //
            //_UpdatingOfData.ContinueUpdateOnError = true;
            _UpdatingOfData.InsertCommand = CommandOfInsertingPriceList;
            _UpdatingOfData.UpdateCommand = CommandOfUpdatingPriceList;
            //
            // Updating
            //
            UpdateOfUpdatingData(PricesOfDrugstore, String.Format("Price_list {0}", IDOfDrugstore));
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
            SqlCommand CommandOfRecording =
                new SqlCommand(
                    String.Format(
                    "INSERT INTO ReportsOfImportingOfPriceLists " +
                    "(ID_PH, ID_HR, CountNotConfirmed, CountOfAdditions, CountOfUnAdditions, " +
                    "CountOfChanges, CountOfUnchanges, CountOfDeletings, CountOfAllPrices) " +
                    "VALUES ({0}, {1}, 0, 0, 0, 0, 0, 0, 0);",
                    IDOfDrugstore, IDOfReception),
                    ConnectionToBase);
            //
            // Executing
            //
            try
            {
                CommandOfRecording.Connection.Open();
                CommandOfRecording.ExecuteNonQuery();
                CommandOfRecording.Connection.Close();
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при записе в ReportsOfImportingOfPriceLists: {0}", E.Message));
                //
                ClosingConnection(CommandOfRecording.Connection);
                //
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
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[4] {
                new SqlParameter("@IDOfDrugstore",    SqlDbType.Int), 
                new SqlParameter("@IDOfReception",    SqlDbType.Int), 
                new SqlParameter("@CountOfAllPrices", SqlDbType.Int), 
                new SqlParameter("@FullPriceList",    SqlDbType.Bit)};
            //
            SqlCommand CommandOfUpdating = (SqlCommand)CreatingCommand(
                "UPDATE ReportsOfImportingOfPriceLists " +
                "SET CountOfAllPrices = @CountOfAllPrices, FullPriceList = @FullPriceList " +
                "WHERE ((ID_PH = @IDOfDrugstore) AND (ID_HR = @IDOfReception));",
                ParametersOfUpdatingCommand);
            //
            CommandOfUpdating.Parameters["@IDOfDrugstore"].Value = IDOfDrugstore;
            CommandOfUpdating.Parameters["@IDOfReception"].Value = IDOfReception;
            CommandOfUpdating.Parameters["@CountOfAllPrices"].Value = CountOfPrices;
            CommandOfUpdating.Parameters["@FullPriceList"].Value = FullPriceList;
            //
            // Executing
            //
            try
            {
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteNonQuery();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении ReportsOfImportingOfPriceLists: {0}", E.Message));
                //
                ClosingConnection(CommandOfUpdating.Connection);
            }
        }

        // Updating Reports Of Importing
        private void UpdatingReportsOfImporting(int IDOfDrugstore, int IDOfReception, int CountOfDeleting)
        {
            //
            // Creating Command Of Recording
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[3] {
                new SqlParameter("@IDOfDrugstore",   SqlDbType.Int), 
                new SqlParameter("@IDOfReception",   SqlDbType.Int), 
                new SqlParameter("@CountOfDeleting", SqlDbType.Int) };
            //
            SqlCommand CommandOfUpdating = (SqlCommand)CreatingCommand(
                "UPDATE ReportsOfImportingOfPriceLists " +
                "SET CountNotConfirmed = @CountOfDeleting " +
                "WHERE ((ID_PH = @IDOfDrugstore) AND (ID_HR = @IDOfReception));",
                ParametersOfUpdatingCommand);
            //
            CommandOfUpdating.Parameters["@IDOfDrugstore"].Value = IDOfDrugstore;
            CommandOfUpdating.Parameters["@IDOfReception"].Value = IDOfReception;
            CommandOfUpdating.Parameters["@CountOfDeleting"].Value = CountOfDeleting;
            //
            // Executing
            //
            try
            {
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteNonQuery();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении ReportsOfImportingOfPriceLists: {0}", E.Message));
                //
                ClosingConnection(CommandOfUpdating.Connection);
            }
        }

        // Updating Date Of Actuals
        private void UpdatingDateOfActuals(int IDOfDrugstore)
        {
            //
            // Creating Command Of Updating
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[1] { 
                new SqlParameter("@IDOfDrugstore",   SqlDbType.Int) };
            //
            SqlCommand CommandOfUpdating = (SqlCommand)CreatingCommand(
                "UPDATE price_list SET Actual = GetDate() " + 
                "WHERE ((Id_Pharmacy = @IDOfDrugstore) AND (Is_Deleted = 0));", 
                ParametersOfUpdatingCommand);
            //
            CommandOfUpdating.Parameters["@IDOfDrugstore"].Value = IDOfDrugstore;
            //
            // Executing
            //
            try
            {
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteNonQuery();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении price_list (Date Of Actuals): {0}", E.Message));
                //
                ClosingConnection(CommandOfUpdating.Connection);
            }
        }

        // TMP
        protected void SetStatusOfRows(DataTable TableForStatus,
            string TextOfCheckingOfDeleting, string TextOfCheckingOfExisting, DbParameter[] ParametersOfCommand)
        {
            //
            try
            {
                //
                TableForStatus.Columns.Add("TMP_Status", typeof(string));
                //
                TableForStatus.AcceptChanges();
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    CurrentRow.SetModified();
                //
                string TextOfExistingOfCommand = "";
                if (TextOfCheckingOfDeleting != "")
                    TextOfExistingOfCommand = String.Format(
                        "IF ({0}) SET @Status = 'REM' ELSE IF ({1}) SET @Status = 'MOD' ELSE SET @Status = 'ADD';",
                        TextOfCheckingOfDeleting, TextOfCheckingOfExisting);
                else
                    TextOfExistingOfCommand = String.Format(
                        "IF ({0}) SET @Status = 'MOD' ELSE SET @Status = 'ADD';",
                        TextOfCheckingOfExisting);
                //
                DbParameter[] ParametersOfExistingCommand =
                    new DbParameter[ParametersOfCommand.Length + 1];
                for (int i = 0; i < ParametersOfCommand.Length; i++)
                    ParametersOfExistingCommand[i] = ParametersOfCommand[i];
                ParametersOfExistingCommand[ParametersOfCommand.Length] =
                    new SqlParameter("@Status", SqlDbType.VarChar, 3, "TMP_Status");
                //
                DbCommand CommandOfExisting = CreatingCommand(TextOfExistingOfCommand, ParametersOfExistingCommand);
                CommandOfExisting.Parameters["@Status"].Direction = ParameterDirection.Output;
                SqlDataAdapter ReadingStatus = new SqlDataAdapter();
                ReadingStatus.UpdateCommand = (SqlCommand)CommandOfExisting;
                //
                ReadingStatus.Update(TableForStatus);
                //
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    switch (CurrentRow["TMP_Status"].ToString())
                    {
                        case "REM":
                            CurrentRow.Delete();
                            break;
                        case "ADD":
                            CurrentRow.SetAdded();
                            break;
                        case "MOD":
                            CurrentRow.SetModified();
                            break;
                    }
                //
                TableForStatus.Columns.Remove("TMP_Status");
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении статуса строк", E, false); }
        }

        // TMP
        private void UpdateOfUpdatingData(DataTable DataForUpdating, string TableName)
        {
            //
            // Updating
            //
            if ((TableName != "Information") &&
                (TableName != "InformationOfSettings") &&
                (TableName != "ListOfSettings") &&
                (TableName != "RegistrationOfDrugstores") &&
                (TableName != "DatesOfTransfer") &&
                (TableName != "LogsOfDrugstores") &&
                (TableName != "AnnouncementsOfDrugstore") &&
                (TableName != "PriceList"))
                RecordingInLogFile(String.Format("Start Updating Table Of {0}", TableName));
            //
            int CountOfUpdating = 0;
            try { CountOfUpdating = _UpdatingOfData.Update(DataForUpdating); }
            catch (Exception E)
            { ReturningMessageAboutError(String.Format("Ошибка при обновлении таблицы {0}", TableName), E, false); }
            //
            if ((TableName != "Information") &&
                (TableName != "InformationOfSettings") &&
                (TableName != "ListOfSettings") &&
                (TableName != "RegistrationOfDrugstores") &&
                (TableName != "DatesOfTransfer") &&
                (TableName != "LogsOfDrugstores") &&
                (TableName != "AnnouncementsOfDrugstore") &&
                (TableName != "PriceList"))
                RecordingInLogFile(String.Format("End Updating Table Of {0}", TableName));
            //
            // Clearing Of UpdatingOfData 
            //
            _UpdatingOfData.InsertCommand = null;
            _UpdatingOfData.UpdateCommand = null;
            _UpdatingOfData.DeleteCommand = null;
        }

        #endregion


        #region ' Checking '

        // Drugstore Is Active
        public bool DrugstoreIsActive(int IDOfDrugstore)
        {
            //
            bool ResultOfActivation = false;
            //
            // !!!
            //
            SqlCommand ExistingDrugstore = 
                new SqlCommand(
                    String.Format("SELECT COUNT(*) FROM Pharmacy WHERE ((Is_Deleted = 0) AND (Id_Pharmacy = {0}));", 
                    IDOfDrugstore), 
                    ConnectionToBase);
            //
            // !!!
            //
            try
            {
                //
                OpeningConnection(ConnectionToBase);
                //
                int CountOfDrugstore = (int)ExistingDrugstore.ExecuteScalar();
                //
                ResultOfActivation = (CountOfDrugstore > 0) ? true : false;
                //
                ClosingConnection(ConnectionToBase);
            }
            catch (Exception E)
            {
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при проверке активации аптеки: {0}", E.Message));
                //
                ClosingConnection(ConnectionToBase);
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


        #region ' Management Of Connection '

        // Opening Connection
        private void OpeningConnection(DbConnection Connection)
        {
            //
            if (Connection != null)
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
        }

        // Closing Connection
        private void ClosingConnection(DbConnection Connection)
        {
            //
            if (Connection != null)
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
        }

        #endregion


        #endregion


        #region ' Importing Data From Service Of Help TMP '

        // Importing Data From Service Of Help
        public void ImportingDataFromServiceOfHelp(DataSet ImportedData)
        {
            if (ImportedData != null)
            {
                //
                // Recording Of Reception
                //
                bool ContainsPriceList = false;
                if (ImportedData.Tables.Contains("PriceList"))
                    if (ImportedData.Tables["PriceList"].Rows.Count > 0)
                        ContainsPriceList = true;
                //
                /*
                this.RecordingInLogFile(
                    String.Format("ImportingDataFromServiceOfHelp02 Count = {0}", ImportedData.Tables.Count));
                */
                //
                int IDOfReception = RecordingOfReception(108, ContainsPriceList, false, DateTime.Now);
                //
                foreach (DataTable CurrentTable in ImportedData.Tables)
                {
                    //
                    switch (CurrentTable.TableName)
                    {
                        case "Pharmacy":
                            {
                                //
                                /*
                                this.RecordingInLogFile(
                                    String.Format("{0} A {1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                                */
                                /*
                                //
                                // Deleting New Rows In Pharmacy
                                //
                                // Обрыв тотальный ???
                                ClearingNewRowsInPharmacy(CurrentTable);
                                //
                                this.RecordingInLogFile(
                                    String.Format("{0} B {1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                                //
                                // Filling Updating Of Date
                                //
                                foreach (DataRow CurrentRow in CurrentTable.Rows)
                                    CurrentRow["Updating"] = DateTime.Now;
                                //
                                // Updating 
                                //
                                this.RecordingInLogFile(
                                    String.Format("{0} C {1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                                //
                                UpdatingOfData.UpdatingOfPharmacy(CurrentTable);
                                */
                            }
                            break;
                        case "Products":
                            {
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", CurrentTable.TableName, CurrentTable.Rows.Count));*/
                                //
                                DataTable Products = CurrentTable.Copy();
                                //
                                // Filling Updating Of Date
                                //
                                foreach (DataRow CurrentRow in Products.Rows)
                                    CurrentRow["Updating"] = DateTime.Now;
                                //
                                // Renaming Name
                                //
                                foreach (DataRow CurrentProduct in Products.Rows)
                                {
                                    //
                                    if ((CurrentProduct["Name"] != null) &&
                                        !(CurrentProduct["Name"] is DBNull))
                                    {
                                        string NameOfProduct = CurrentProduct["Name"].ToString();
                                        if (NameOfProduct.Length > 2)
                                            if (NameOfProduct.EndsWith("\n") &&
                                                (NameOfProduct[NameOfProduct.Length - 2] != '\n'))
                                                NameOfProduct = NameOfProduct.Remove(NameOfProduct.Length - 2, 2);
                                        //
                                        NameOfProduct = NameOfProduct.Trim();
                                        CurrentProduct["Name"] = NameOfProduct;
                                    }
                                }
                                Products.AcceptChanges();
                                //
                                // Updating 
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", Products.TableName, Products.Rows.Count));*/
                                //
                                UpdatingOfData.UpdatingOfProducts(Products);
                            }
                            break;
                        case "PriceList":
                            {
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", CurrentTable.TableName, CurrentTable.Rows.Count));*/
                                //
                                // Renaming Table And Columns
                                //
                                DataTable PriceList = CurrentTable.Copy();
                                //
                                // Addition Of Column AllPrices
                                //
                                PriceList.Columns.Add("AllPrices", typeof(bool));
                                foreach (DataRow CurrentPrice in PriceList.Rows)
                                    CurrentPrice["AllPrices"] = false;
                                //
                                PriceList.AcceptChanges();
                                /*
                                //
                                // Recording Of Reception
                                //
                                int IDOfReception = RecordingOfReception(
                                    108, (PriceList.Rows.Count > 0) ? true : false, false, DateTime.Now);
                                */
                                //
                                // Importing Of Prices
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", PriceList.TableName, PriceList.Rows.Count));*/
                                //
                                if (IDOfReception > 0)
                                    ImportingOfPriceList(PriceList, IDOfReception);
                                //
                            }
                            break;
                        case "LogOfService":
                            {
                                //IDsOfModifications
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", CurrentTable.TableName, CurrentTable.Rows.Count));*/
                                //
                                //UpdatingLogOfDrugstore(CurrentTable, 108);
                            }
                            break;
                        case "IDsOfModifications":
                            {
                                //IDsOfModifications
                            }
                            break;
                        case "InformationOfData":
                            {
                                //InformationOfData
                            }
                            break;
                        default:
                            {
                                //
                                this.RecordingInLogFile(
                                    String.Format("Неизвестная таблица в наборе данных {0}", CurrentTable.TableName));
                            }
                            break;
                    }
                }
                //
                //this.RecordingInLogFile("");
            }
        }

        // Clearing New Rows In Pharmacy
        public void ClearingNewRowsInPharmacy(DataTable DataForPharmacy)
        {
            //
            // Creating Command Of Reading Of Status Of Rows
            //
            DbParameter[] ParametersOfSelectionCommand = new DbParameter[1] {
                new SqlParameter("@P1", SqlDbType.Int, 0, "Id_Pharmacy") };
            SetStatusOfRows(DataForPharmacy,
                "", "EXISTS(SELECT Id_Pharmacy FROM Pharmacy WHERE Id_Pharmacy = @P1)", ParametersOfSelectionCommand);
            //
            // Clearing Pharmacy
            //
            foreach (DataRow CurrentRow in DataForPharmacy.Rows)
                if (CurrentRow.RowState == DataRowState.Added)
                {
                    CurrentRow.AcceptChanges();
                    CurrentRow.Delete();
                }
            //
            DataForPharmacy.AcceptChanges();
            //
        }

        #endregion


        // Recording Of Reception
        private int RecordingOfReception(
            int IDOfDrugstore, bool ContainsPriceList, bool ContainsAnnouncements, DateTime LocalDateOfSending)
        {
            //
            // Generation Of Text Of Recording
            //
            string TextOfCommandOfRecording =
                "INSERT INTO HistoryOfReceptions " +
                "(ID_PH, ContainsPriceList, ContainsAnnouncements, " +
                "DateOfReception, LocalDateOfSending) " +
                "VALUES (@ID_PH, @ContainsPriceList, @ContainsAnnouncements, " +
                "GetDate(), @LocalDateOfSending); " +
                "SET @ID = (SELECT MAX(ID) FROM HistoryOfReceptions);";
            //
            // Creating Command Of Recording
            //
            DbParameter[] ParametersOfRecordingCommand = new DbParameter[5] {
                            new SqlParameter("@ID",                     SqlDbType.Int), 
                            new SqlParameter("@ID_PH",                  SqlDbType.Int), 
                            new SqlParameter("@ContainsPriceList",      SqlDbType.Bit), 
                            new SqlParameter("@ContainsAnnouncements",  SqlDbType.Bit), 
                            new SqlParameter("@LocalDateOfSending",     SqlDbType.DateTime)};
            //
            SqlCommand CommandOfRecording = (SqlCommand)
                CreatingCommand(TextOfCommandOfRecording, ParametersOfRecordingCommand);
            //
            CommandOfRecording.Parameters["@ID"].Direction = ParameterDirection.Output;
            //
            CommandOfRecording.Parameters["@ID_PH"].Value = IDOfDrugstore;
            CommandOfRecording.Parameters["@ContainsPriceList"].Value = ContainsPriceList;
            CommandOfRecording.Parameters["@ContainsAnnouncements"].Value = ContainsAnnouncements;
            CommandOfRecording.Parameters["@LocalDateOfSending"].Value = LocalDateOfSending;
            //
            // Executing
            //
            try
            {
                CommandOfRecording.Connection.Open();
                CommandOfRecording.ExecuteNonQuery();
                CommandOfRecording.Connection.Close();
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
            try { IDOfReception = (int)CommandOfRecording.Parameters["@ID"].Value; }
            catch { this.RecordingInLogFile("Ошибка при получении IDOfReception"); }
            //
            // Return
            //
            return IDOfReception;
        }


        #endregion

    }
}