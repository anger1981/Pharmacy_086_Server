using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Server
{
    public class ExportingOfData : BaseType
    {

        #region ' Fields '

        private string StringOfConnection;
        private SqlConnection ConnectionToBase;
        private SqlCommand CommandOfSelection;
        private SqlDataAdapter FillingOfData;

        #endregion


        #region ' Designer '

        public ExportingOfData(string StringOfConnection, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initializing String Of Connection
            //
            this.StringOfConnection = StringOfConnection;
            //
            // Creating Of Connection
            //
            try { ConnectionToBase = new SqlConnection(this.StringOfConnection); }
            catch (Exception E)
            {
                throw new Exception(
                    String.Format("Ошибка при создании подключения Экспорта данных: {0}", E.Message));
            }
            //
            // Checking Of Opening Of Connection
            //
            if (ConnectionToBase != null)
                try
                {
                    ConnectionToBase.Open();
                    ConnectionToBase.Close();
                }
                catch (Exception E)
                {
                    throw new Exception(
                      String.Format("Ошибка при открытии подключения Экспорта данных: {0}", E.Message));
                }
            //
            // Initializing Fields
            //
            CommandOfSelection = new SqlCommand("", ConnectionToBase);
            CommandOfSelection.CommandTimeout = 1000;
            CommandOfSelection.Parameters.Add(new SqlParameter("@P1", typeof(DateTime)));
            FillingOfData = new SqlDataAdapter(CommandOfSelection);
        }

        #endregion


        #region ' Exporting Of Data '

        // Exporting Of Data
        public DataSet Exporting()
        {
            //
            // Initialize ExportedDataSet
            //
            DataSet ExportedDataSet = new DataSet("ExportedData");
            ExportedDataSet.RemotingFormat = SerializationFormat.Binary;
            //
            // Selections For Exporting Data
            //
            string[,] SchemasOfTables = new string[4,2]
            { 
              {
                  "Pharmacy", 
                  "SELECT Id_Pharmacy AS 'ID', Id_District AS 'ID_DI', Name_full AS 'Name', Addr AS 'Address', Phone AS 'Phone', Mail AS 'Mail', Web AS 'Site', Hours AS 'Schedule', Trans AS 'Transport', Is_deleted AS 'Deleting' " + 
                  "FROM Pharmacy " + 
                  "WHERE (Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND @P1); "
              },
              {
                  "GroupsOfProducts", 
                  "SELECT Id_product_group AS 'ID', Name_full AS 'Name', Date_upd AS 'DateOfUpdating', Is_deleted AS 'Deleting' " + 
                  "FROM product_group " + 
                  "WHERE (Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND @P1); "
              },
              {
                  "Products", 
                  "SELECT Id_Product AS 'ID', Id_product_group AS 'ID_PG', Name_full AS 'Name', Composition AS 'Composition', Description AS 'Description', Date_upd AS 'Updating', Is_deleted AS 'Deleting' " + 
                  "FROM Product " + 
                  "WHERE (Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND @P1); "
              },
              /*{
                  "PriceList",
                  "SELECT Id_Pharmacy AS 'ID_PH', ID_Product AS 'ID_PR', Price AS 'Price', Date_upd AS 'Updating', Is_privilege AS 'Preferential', Is_deleted AS 'Deleting' " + 
                  "FROM Price_List " + 
                  "WHERE ((Id_Pharmacy IN (SELECT Id_Pharmacy FROM Pharmacy WHERE Is_deleted = 0)) AND " + 
                  "(Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND @P1)); "
              },*/
              /*{
                  "FullUpdatingOfDates",
                  "SELECT RIPL.ID_PH AS 'ID', HR.DateOfReception AS 'Date' " + 
                  "FROM HistoryOfReceptions AS HR, ReportsOfImportingOfPriceLists AS RIPL " + 
                  "WHERE ((HR.DateOfReception " + 
                  "BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND @P1) AND " + 
                  "(HR.ID = RIPL.ID_HR) AND (RIPL.FullPriceList = 1) AND " + 
                  "((RIPL.CountNotConfirmed + RIPL.CountOfAdditions + " + 
                  "RIPL.CountOfChanges + RIPL.CountOfDeletings) > 0)); "
              },*/
              /*
              {
                  "CountOfExported", 
                  "SELECT ID_PH AS 'ID_PH', ID AS 'ID', Caption AS 'Caption', [Text] AS 'Text', Published AS 'Published', DateOfUpdating AS 'DateOfUpdating' " + 
                  "FROM Announcements " + 
                  "WHERE (DateOfUpdating BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND GetDate());"
              }*/
              {
                  "Announcements", 
                  "SELECT ID_PH AS 'ID_PH', ID AS 'ID', Caption AS 'Caption', [Text] AS 'Text', Published AS 'Published', DateOfUpdating AS 'DateOfUpdating' " + 
                  "FROM Announcements " + 
                  "WHERE (DateOfUpdating BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 8) AND GetDate());"
              }
            };
            //
            // Announcements
            // CountOfExported
            //
            // Date Of Exporting
            bool GettingOfData = false;
            int CountOfExported = 0;
            //
            try
            {
                SqlCommand GettingData = new SqlCommand("SELECT GetDate() AS 'CurrentDate';", ConnectionToBase);
                GettingData.Connection.Open();
                DateTime DateOfExported = Convert.ToDateTime(GettingData.ExecuteScalar());
                //GettingData.Connection.Close();
                CommandOfSelection.Parameters["@P1"].Value = DateOfExported;
                //
                GettingData.CommandText = "SELECT Value FROM Service WHERE Id_Service = 8;";
                //GettingData.Connection.Open();
                int NumberOfExported = Convert.ToInt32(GettingData.ExecuteScalar());
                GettingData.Connection.Close();
                //
                ExportedDataSet.Tables.Add("DateOfExported");
                ExportedDataSet.Tables["DateOfExported"].Columns.Add("DateOfExported", typeof(DateTime));
                ExportedDataSet.Tables["DateOfExported"].Rows.Add(DateOfExported);
                ExportedDataSet.Tables["DateOfExported"].AcceptChanges();
                //
                ExportedDataSet.Tables.Add("NumberOfExported");
                ExportedDataSet.Tables["NumberOfExported"].Columns.Add("DateOfExported", typeof(int));
                ExportedDataSet.Tables["NumberOfExported"].Rows.Add(++NumberOfExported);
                ExportedDataSet.Tables["NumberOfExported"].AcceptChanges();
                //
                /*
                ExportedDataSet.Tables.Add("CountOfExported");
                ExportedDataSet.Tables["CountOfExported"].Columns.Add("CountOfExported", typeof(int));
                ExportedDataSet.Tables["CountOfExported"].Rows.Add(CountOfExported);
                ExportedDataSet.Tables["CountOfExported"].AcceptChanges();
                */
                //
                GettingOfData = true;
            }
            catch (Exception E) 
            { ReturningMessageAboutError("Ошибка при получении даты или номера экспорта", E, true); }
            //
            // Reading Data
            //
            if (GettingOfData)
            {
                for (int i = 0; i <= SchemasOfTables.GetUpperBound(0); i++)
                {
                    // Creating Text Of Command
                    CommandOfSelection.CommandText = SchemasOfTables[i, 1];
                    //
                    // Filling
                    //
                    try
                    {
                        FillingOfData.FillSchema(ExportedDataSet, SchemaType.Source, SchemasOfTables[i, 0]);
                        CountOfExported += FillingOfData.Fill(ExportedDataSet, SchemasOfTables[i, 0]);
                    }
                    catch (Exception E)
                    {
                        ReturningMessageAboutError(
                            String.Format("Ошибка при чтении данных из таблицы {0}", SchemasOfTables[i, 0]), E, false);
                    }
                }
                //
                /*
                ExportedDataSet.Tables["CountOfExported"].Rows[0][0] = CountOfExported;
                ExportedDataSet.Tables["CountOfExported"].AcceptChanges();
                */
            }
            //
            // Return
            //
            return ExportedDataSet;
        }

        public bool UpdatingDateOfExporting(DateTime DateOfExported)
        {
            //
            bool ResultOfUpdating = true;
            //
            // Creating Updating Date
            //
            SqlCommand CommandOfUpdating = new SqlCommand(
                "UPDATE Service SET Date_Service = @P1 WHERE Id_Service = 8;", ConnectionToBase);
            CommandOfUpdating.Parameters.Add(new SqlParameter("@P1", SqlDbType.DateTime));
            CommandOfUpdating.Parameters["@P1"].Value = DateOfExported;
            //
            // Executing Updating
            //
            try
            {
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteNonQuery();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                ResultOfUpdating = false;
                ReturningMessageAboutError(
                    String.Format("Ошибка при обновлении даты экспортирования {0}", DateOfExported), E, false);
            }
            //
            // Return
            //
            return ResultOfUpdating;
        }

        public bool IncrementOfNumberOfExported()
        {
            //
            bool ResultOfExecuting = true;
            //
            // Creating Increment
            //
            SqlCommand CommandOfUpdating = new SqlCommand(
                    "UPDATE Service SET Value = Value + 1 WHERE Id_Service = 8;", ConnectionToBase);
            //
            // Executing Increment
            //
            try
            {
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteNonQuery();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                ResultOfExecuting = false;
                ReturningMessageAboutError("Ошибка при обновлении номера экспорирования", E, false);
            }
            //
            // Return
            //
            return ResultOfExecuting;
        }

        #endregion


        #region ' Exporting Of PriceLists '

        // Checking Of Existence Exported Prices
        public bool CheckingOfExistenceExportedPrices()
        {
            //
            bool ExistenceExportedPrices = false;
            //
            // Creating Checking
            //
            SqlCommand GettingCount = 
                new SqlCommand(
                    "SELECT COUNT(*) FROM Price_List " + 
                    "WHERE Date_upd > (SELECT Date_Service FROM Service WHERE Id_Service = 6);", ConnectionToBase);
            //
            // Getting Count Of Exported Prices
            //
            int CountOfExportedPrices = -1;
            //
            try
            {
                //
                GettingCount.Connection.Open();
                //
                CountOfExportedPrices = Convert.ToInt32(GettingCount.ExecuteScalar());
                //
                GettingCount.Connection.Close();
            }
            catch (Exception E)
            {
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при проверке наличия экспортных Прайс-Листов: {0}", E.Message));
                //
                GettingCount.Connection.Close();
            }
            //
            // Checking Existence
            //
            if (CountOfExportedPrices > 0)
                ExistenceExportedPrices = true;
            //
            // Return
            //
            return ExistenceExportedPrices;
        }

        // Getting Number Of Exported PriceLists
        public int GettingNumberOfExportedPriceLists()
        {
            //
            int NumberOfExported = -1;
            //
            // Creating Getting
            //
            SqlCommand GettingNumber = 
                new SqlCommand("SELECT Value FROM Service WHERE Id_Service = 6;", ConnectionToBase);
            //
            // Getting Number
            //
            try
            {
                //
                GettingNumber.Connection.Open();
                //
                NumberOfExported = Convert.ToInt32(GettingNumber.ExecuteScalar());
                //
                GettingNumber.Connection.Close();
            }
            catch (Exception E)
            {
                //
                NumberOfExported = -1;
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при получении номера экспортирования Прайс-Листов: {0}", E.Message));
                //
                GettingNumber.Connection.Close();
            }
            //
            // Return
            //
            return NumberOfExported;
        }

        // Getting Date Of Exported PriceLists
        public DateTime GettingDateOfExportedPriceLists()
        {
            //
            DateTime DateOfExported = new DateTime(1947, 07, 02);
            //
            // Creating Getting
            //
            SqlCommand GettingDate = 
                new SqlCommand("SELECT Date_Service FROM Service WHERE Id_Service = 6;", ConnectionToBase);
            //
            // Getting Date
            //
            try
            {
                //
                GettingDate.Connection.Open();
                //
                DateOfExported = Convert.ToDateTime(GettingDate.ExecuteScalar());
                //
                GettingDate.Connection.Close();
            }
            catch (Exception E)
            {
                //
                DateOfExported = new DateTime(1947, 07, 02);
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при получении даты экспортирования Прайс-Листов: {0}", E.Message));
                //
                GettingDate.Connection.Close();
            }
            //
            // Return
            //
            return DateOfExported;
        }

        // Getting Date Of Updating PriceLists
        public DateTime GettingDateOfUpdatingPriceLists()
        {
            //
            DateTime DateOfExported = new DateTime(1947, 07, 02);
            //
            // Creating Getting
            //
            SqlCommand GettingDate = 
                new SqlCommand("SELECT MAX(Date_upd) FROM Price_List", ConnectionToBase);
            //
            // Getting Date
            //
            try
            {
                //
                GettingDate.Connection.Open();
                //
                DateOfExported = Convert.ToDateTime(GettingDate.ExecuteScalar());
                //
                GettingDate.Connection.Close();
            }
            catch (Exception E)
            {
                //
                DateOfExported = new DateTime(1947, 07, 02);
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при получении даты экспортирования Прайс-Листов: {0}", E.Message));
                //
                GettingDate.Connection.Close();
            }
            //
            // Return
            //
            return DateOfExported;
        }

        // Exporting Of PriceLists
        public void ExportingOfPriceLists(string PathToFileOfExportingPriceLists)
        {
            //
            RecordingInLogFile("Starting Exporting Of PriceLists");
            //
            // Creating Of Filling Of PriceLists
            //
            string TextOfCommandOfSelection = 
                "SELECT ID_Pharmacy AS 'ID_DR', ID_Product AS 'ID_PR', Price AS 'Price', " + 
                "Actual AS 'Actual', Is_Privilege AS 'Preferential' " + 
                "FROM Price_List " + 
                "WHERE (Is_Deleted = 0) " + 
                "ORDER BY ID_Pharmacy, ID_Product";
            //
            SqlCommand SelectionOfPriceLists = 
                new SqlCommand(TextOfCommandOfSelection, ConnectionToBase);
            //
            SqlDataAdapter FillingOfPriceLists = new SqlDataAdapter(SelectionOfPriceLists);
            //
            // Filling Of PriceLists
            //
            RecordingInLogFile("Starting Filling Of Exported PriceLists");
            //
            DataTable ExportedPriceLists = new DataTable("PriceLists");
            bool ResultOfFilling = true;
            //
            try
            {
                //
                FillingOfPriceLists.FillSchema(ExportedPriceLists, SchemaType.Source);
                FillingOfPriceLists.Fill(ExportedPriceLists);
            }
            catch (Exception E)
            { 
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при заполнении таблицы экспортируемый Прайс-Листов: {0}", E.Message));
                //
                ResultOfFilling = false;
            }
            //
            RecordingInLogFile("Stoping Filling Of Exported PriceLists");
            //
            // Exporting To File Of PriceLists
            //
            if (ResultOfFilling)
            {
                //
                RecordingInLogFile("Starting Exporting To File Of PriceLists");
                //
                try
                {
                    //
                    // Creating Of Streams
                    //
                    FileStream FS = 
                        new FileStream(PathToFileOfExportingPriceLists, FileMode.Create, FileAccess.ReadWrite);
                    //
                    StreamWriter SW = new StreamWriter(FS);
                    //
                    // Writing Prices And Dates
                    //
                    try
                    {
                        //
                        int IDOfDrugstore = 0;
                        //
                        foreach (DataRow CurrentPrice in ExportedPriceLists.Rows)
                        {
                            //
                            // Writing Date
                            //
                            if (IDOfDrugstore != ((int)CurrentPrice["ID_DR"]))
                            {
                                //
                                IDOfDrugstore = ((int)CurrentPrice["ID_DR"]);
                                //
                                SW.WriteLine(
                                    String.Format("{0},{1}", 
                                    IDOfDrugstore, ((DateTime)CurrentPrice["Actual"]).ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                            //
                            // Writing Price
                            //
                            string NewStringOfPrices = 
                                String.Format(
                                "{0},{1},{2},{3}", 
                                CurrentPrice["ID_DR"], 
                                CurrentPrice["ID_PR"], 
                                CurrentPrice["Price"].ToString().Replace(",", "."), 
                                (((bool)CurrentPrice["Preferential"]) == true) ? "1" : "0");
                            //(((DateTime)CurrentPrice["Actual"])).ToString("yyyy-MM-dd"),
                            //
                            SW.WriteLine(NewStringOfPrices);
                        }
                    }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при записи цена в файл Прайс-Листов: {0}", E.Message)); }
                    //
                    // Clearing Of ExportedPriceLists
                    //
                    ExportedPriceLists.Clear();
                    ExportedPriceLists.Dispose();
                    //
                    // Closing And Clearing Streams
                    //
                    try
                    {
                        //
                        SW.Close();
                        SW.Dispose();
                        //
                        //GC.SuppressFinalize(FS);
                        //
                        FS.Close();
                        FS.Dispose();
                        //
                        System.Threading.Thread.Sleep(1108);
                        //
                        GC.Collect();
                    }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии потоков записи файла экспортируемых Прайс-Листов: {0}", E.Message)); }
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при создании потоков записи файла экспортируемых Прайс-Листов: {0}", E.Message)); }
                //
                RecordingInLogFile("Stoping Exporting To File Of PriceLists");
            }
            //
            System.Threading.Thread.Sleep(108);
            //
            GC.Collect();
            //
            RecordingInLogFile("Stoping Exporting Of PriceLists");
        }

        // Increment Of Number Of Exported PriceLists
        public bool IncrementOfNumberOfExportedPriceLists(DateTime DateOfExported)
        {
            //
            bool ResultOfUpdating = true;
            //
            // Creating Updating 
            //
            SqlCommand CommandOfUpdating = new SqlCommand(
                "UPDATE Service SET Value = Value + 1, Date_Service = @P1 WHERE Id_Service = 6;", ConnectionToBase);
            CommandOfUpdating.Parameters.Add(new SqlParameter("@P1", SqlDbType.DateTime));
            CommandOfUpdating.Parameters["@P1"].Value = DateOfExported;
            //
            // Updating Number And Date Of Exported PriceLists
            //
            try
            {
                //
                CommandOfUpdating.Connection.Open();
                //
                CommandOfUpdating.ExecuteNonQuery();
                //
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                //
                ResultOfUpdating = false;
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при обновлении номера экспорирования Прайс-Листов: {0}", E.Message));
                //
                CommandOfUpdating.Connection.Close();
            }
            //
            // Return
            //
            return ResultOfUpdating;
        }

        #endregion


        #region ' Exporting Of Modifications '

        // Exporting Of Modifications
        public DataSet ExportingOfModifications()
        {
            //
            // Initialize ExportedDataSet
            //
            DataSet ExportedDataSet = new DataSet("ExportedData");
            ExportedDataSet.RemotingFormat = SerializationFormat.Binary;
            //
            // Addition Of Information Of Data
            //
            DataTable Information = new DataTable("InformationOfData");
            Information.Columns.Add("Key", typeof(string));
            Information.Columns.Add("Value", typeof(object));
            Information.PrimaryKey = new DataColumn[1] { Information.Columns["Key"] };
            //
            Information.Rows.Add("CountOfRows", 0);
            Information.Rows.Add("Source", "ServerOfServiceOfHelp");
            //
            Information.AcceptChanges();
            //
            ExportedDataSet.Tables.Add(Information);
            //
            // Reading Modifications
            //
            string[,] SchemasOfTables = new string[4, 2]
            {
                {
                    "Pharmacy", 
                    "SELECT M.ID AS 'IDOfModification', " + 
                    "P.Id_Pharmacy AS 'ID', Id_District AS 'ID_DI', Name_full AS 'Name', Addr AS 'Address', " + 
                    "Phone AS 'Phone', Mail AS 'Mail', Web AS 'Site', Hours AS 'Schedule', Trans AS 'Transport', " + 
                    "Date_upd AS 'Updating', Is_deleted AS 'Deleting' " + 
                    "FROM ModifiedData AS M, Pharmacy AS P " + 
                    "WHERE ((Type = 1) AND (M.ID01 = P.Id_Pharmacy));"
                },
                {
                    "Products", 
                    "SELECT M.ID AS 'IDOfModification', " + 
                    "P.Id_Product AS 'ID', Id_product_group AS 'ID_PG', Name_full AS 'Name', " + 
                    "Composition AS 'Composition', Description AS 'Description', Date_upd AS 'Updating', " + 
                    "Is_deleted AS 'Deleting' " + 
                    "FROM ModifiedData AS M, Product AS P " + 
                    "WHERE ((Type = 2) AND (M.ID01 = P.ID_Product));"
                },
                {
                    "PriceList", 
                    "SELECT M.ID AS 'IDOfModification', " + 
                    "PL.Id_Pharmacy AS 'ID_PH', Id_Product AS 'ID_PR', Price AS 'Price', " + 
                    "Is_deleted AS 'Deleting', Is_privilege AS 'Preferential' " + 
                    "FROM ModifiedData AS M, Price_List AS PL " + 
                    "WHERE ((Type = 3) AND ((M.ID01 = PL.Id_Pharmacy) AND (M.ID02 = PL.Id_Product)));"
                },
                {
                    "IDsOfModifications", 
                    "SELECT ID AS 'IDOfModification' " + 
                    "FROM ModifiedData " + 
                    "WHERE (Type IN (1, 2, 3));"
                }
            };
            //
            int CountOfExported = 0;
            SqlCommand SelectionOfModifications = new SqlCommand("", ConnectionToBase);
            SqlDataAdapter GettingModifications = new SqlDataAdapter(SelectionOfModifications);
            //
            for (int i = 0; i <= SchemasOfTables.GetUpperBound(0); i++)
            {
                // Creating Text Of Command
                SelectionOfModifications.CommandText = SchemasOfTables[i, 1];
                //
                // Filling
                //
                try
                {
                    GettingModifications.FillSchema(ExportedDataSet, SchemaType.Source, SchemasOfTables[i, 0]);
                    CountOfExported += GettingModifications.Fill(ExportedDataSet, SchemasOfTables[i, 0]);
                }
                catch (Exception E)
                {
                    ReturningMessageAboutError(
                        String.Format("Ошибка при чтении данных из таблицы {0}", SchemasOfTables[i, 0]), E, false);
                }
            }
            //
            ExportedDataSet.Tables["InformationOfData"].Rows.Find("CountOfRows")["Value"] = CountOfExported;
            //
            ExportedDataSet.AcceptChanges();
            //
            // Return
            //
            return ExportedDataSet;
        }

        // Clearing Of Modifications
        public void ClearingOfModifications(DataSet Modifications)
        {
            //
            if (Modifications != null)
                if (Modifications.Tables.Contains("IDsOfModifications"))
                {
                    //
                    DataTable IDsOfModifications = Modifications.Tables["IDsOfModifications"].Copy();
                    //
                    IDsOfModifications.AcceptChanges();
                    foreach (DataRow CurrentID in IDsOfModifications.Rows)
                        CurrentID.Delete();
                    //
                    SqlCommand CommandOfDeleting = new SqlCommand(
                        "DELETE FROM ModifiedData WHERE ID = @P1;", ConnectionToBase);
                    CommandOfDeleting.Parameters.Add("@P1", SqlDbType.Int, 0, "IDOfModification");
                    //
                    SqlDataAdapter ClearingData = new SqlDataAdapter();
                    ClearingData.DeleteCommand = CommandOfDeleting;
                    //
                    try { ClearingData.Update(IDsOfModifications); }
                    catch (Exception E) 
                    { this.RecordingInLogFile(String.Format("Ошибка при удалении ID изменений: {0}", E.Message)); }
                }
        }

        #endregion


        #region ' Exporting Of Scripts Of Data '

        // Getting Current Date
        public DateTime GettingCurrentDate()
        {
            //
            // Creating Connection
            //
            SqlConnection ConnectionToBase = new SqlConnection();
            try
            {
                ConnectionToBase = new SqlConnection(StringOfConnection);
                ConnectionToBase.Open();
                ConnectionToBase.Close();
            }
            catch (Exception E) { this.RecordingInLogFile(String.Format("{0}: {1}", "Ошибка при создании подключения", E.Message)); }
            //
            // Getting Current Date
            //
            SqlCommand GettingData = new SqlCommand("SELECT GetDate() AS 'CurrentDate';", ConnectionToBase);
            GettingData.Connection.Open();
            DateTime CurrentDate = Convert.ToDateTime(GettingData.ExecuteScalar());
            GettingData.Connection.Close();
            //
            // Return
            //
            return CurrentDate;
        }

        // Updating Date Of Exporting Of Scripts
        public bool UpdatingDateOfExportingOfScripts(DateTime DateOfExported)
        {
            //
            bool ResultOfOperation = true;
            SqlCommand CommandOfUpdating = new SqlCommand(
                "UPDATE Service SET Date_Service = @P1 WHERE Id_Service = 10;", ConnectionToBase);
            CommandOfUpdating.Parameters.Add(new SqlParameter("@P1", SqlDbType.DateTime));
            CommandOfUpdating.Parameters["@P1"].Value = DateOfExported;
            // Updating Date
            try
            {
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteNonQuery();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                ResultOfOperation = false;
                ReturningMessageAboutError(
                    String.Format("Ошибка при обновлении даты экспортирования {0}", DateOfExported), E, false);
            }
            //
            // Return
            //
            return ResultOfOperation;
        }

        #region ' Exporting Of Scripts Of Data '

        // Exporting Of Scripts Of Data
        public string ExportingOfScriptsOfData(DateTime DateOfExported)
        {
            //
            DataTable PriceListForExporting = new DataTable();
            //
            // Creating Command Of Selection
            //
            SqlCommand CommandOfSelection = new SqlCommand(
                "SELECT * FROM Price_List WHERE Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 10) AND @P1;",
                ConnectionToBase);
            //
            // Addition Of Parameters
            //
            CommandOfSelection.Parameters.Add("@P1", SqlDbType.DateTime);
            CommandOfSelection.Parameters["@P1"].Value = DateOfExported;
            //
            // Creating DataAdapter For Filling
            //
            SqlDataAdapter FillingData = new SqlDataAdapter(CommandOfSelection);
            //
            // Filling Of Data
            //
            try
            {
                FillingData.FillSchema(PriceListForExporting, SchemaType.Source);
                FillingData.Fill(PriceListForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(String.Format("{0}: {1}", "Ошибка при заполнении таблицы PriceList", E.Message));
                return "";
            }
            //
            // Scaning Of PriceList For Exporting
            //
            string TextOfCommands = "";
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            foreach (DataRow CurRow in PriceListForExporting.Rows)
            {
                //
                // Creating Text Of Command Of Deleting
                //
                TextOfCommands +=
                    String.Format(
                    "DELETE FROM {0}.Price_List WHERE (Id_Pharmacy = {1} AND Id_Product = {2});",
                    ConnectionToBase.Database,
                    ((System.Int32)CurRow["Id_Pharmacy"]),
                    ((System.Int32)CurRow["Id_Product"]));
                TextOfCommands += Paragraph;
                //
                // Creating Text Of Command Of Inserting
                //
                if (((System.Boolean)CurRow["Is_deleted"]) != true)
                {
                    TextOfCommands +=
                        String.Format(
                        "INSERT INTO {0}.Price_List " +
                        "(Id_Pharmacy, Id_Product, Price, Date_upd, Is_deleted, Is_privilege) " +
                        Paragraph + "VALUES ({1}, {2}, {3}, '{4}', {5}, {6});",
                        ConnectionToBase.Database,
                        Convert.ToInt32(CurRow["Id_Pharmacy"]), Convert.ToInt32(CurRow["Id_Product"]),
                        GettingStringOfDecimal(CurRow["Price"]),
                        GettingStringOfDate(((System.DateTime)CurRow["Date_upd"])),
                        GettingStringOfBoolean(((System.Boolean)CurRow["Is_deleted"])),
                        GettingStringOfBoolean(((System.Boolean)CurRow["Is_privilege"])));
                    TextOfCommands += Paragraph;
                    //
                }
                TextOfCommands += Paragraph;
            }
            //
            // Return
            //
            return TextOfCommands;
        }

        // Exporting Of Scripts Of Data 02
        public void ExportingOfScriptsOfData02(DateTime DateOfExported, string PathOfFileSQL)
        {
            //
            bool SuccessfulFilling = true;
            DataTable PriceListForExporting = new DataTable();
            //
            // Creating Command Of Selection
            //
            SqlCommand CommandOfSelection = new SqlCommand(
                "SELECT * FROM Price_List WHERE Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 10) AND @P1;",
                ConnectionToBase);
            //
            // Addition Of Parameters
            //
            CommandOfSelection.Parameters.Add("@P1", SqlDbType.DateTime);
            CommandOfSelection.Parameters["@P1"].Value = DateOfExported;
            //
            // Creating DataAdapter For Filling
            //
            SqlDataAdapter FillingData = new SqlDataAdapter(CommandOfSelection);
            //
            // Filling Of Data
            //
            try
            {
                FillingData.FillSchema(PriceListForExporting, SchemaType.Source);
                FillingData.Fill(PriceListForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при заполнении таблицы PriceList: {0}", E.Message));
                SuccessfulFilling = false;
            }
            //
            // Creating File Of Exporting Scripts
            //
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Checking
            //
            bool SuccessfulChecking = false;
            //
            if (SuccessfulFilling)
                if (PriceListForExporting.Rows.Count > 0)
                    SuccessfulChecking = true;
            //
            if (SuccessfulChecking)
                try
                {
                    //
                    // Creating Streams Of File
                    //
                    System.IO.FileStream FS = new System.IO.FileStream(
                        PathOfFileSQL, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(FS);
                    //
                    // Writing Scripts In File
                    //
                    try
                    {
                        foreach (DataRow CurRow in PriceListForExporting.Rows)
                        {
                            //
                            // Creating Text Of Command Of Deleting
                            //
                            string TextOfCommandDeleting =
                                String.Format(
                                "DELETE FROM {0}.Price_List WHERE (Id_Pharmacy = {1} AND Id_Product = {2});",
                                ConnectionToBase.Database,
                                ((System.Int32)CurRow["Id_Pharmacy"]),
                                ((System.Int32)CurRow["Id_Product"]));
                            //
                            SW.Write(TextOfCommandDeleting + Paragraph);
                            //
                            // Creating Text Of Command Of Inserting
                            //
                            if (((System.Boolean)CurRow["Is_deleted"]) != true)
                            {
                                string TextOfCommandInserting =
                                    String.Format(
                                    "INSERT INTO {0}.Price_List " +
                                    "(Id_Pharmacy, Id_Product, Price, Date_upd, Is_deleted, Is_privilege) " +
                                    Paragraph + "VALUES ({1}, {2}, {3}, '{4}', {5}, {6});",
                                    ConnectionToBase.Database,
                                    Convert.ToInt32(CurRow["Id_Pharmacy"]), Convert.ToInt32(CurRow["Id_Product"]),
                                    GettingStringOfDecimal(CurRow["Price"]),
                                    GettingStringOfDate(((System.DateTime)CurRow["Date_upd"])),
                                    GettingStringOfBoolean(((System.Boolean)CurRow["Is_deleted"])),
                                    GettingStringOfBoolean(((System.Boolean)CurRow["Is_privilege"])));
                                //
                                SW.Write(TextOfCommandInserting + Paragraph);
                                //
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                                String.Format("ERROR Ошибка при записи в файл: {0}: {1}", PathOfFileSQL, E.Message));
                    }
                    //
                    // Closing Streams
                    //
                    try
                    {
                        SW.Close();
                        FS.Close();
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при закрытии файла: {0}: {1}", PathOfFileSQL, E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании файла: {0}: {1}", PathOfFileSQL, E.Message));
                    //
                    // Deleting File
                    //
                    try
                    {
                        if (File.Exists(PathOfFileSQL))
                            File.Delete(PathOfFileSQL);
                    }
                    catch (Exception E2)
                    {
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при удалении файла: {0}: {1}", PathOfFileSQL, E2.Message));
                    }
                }
            //
        }

        // Exporting Of Scripts Of Data 03
        public DataTable ExportingOfScriptsOfData03(DateTime DateOfExported, string PathToFileOfScripts)
        {
            //
            // Initializing Of Variables
            //
            bool SuccessfulFilling = true;
            //
            DataTable InformationOfExporting = new DataTable("InformationOfExporting");
            InformationOfExporting.Columns.Add("Key", typeof(string));
            InformationOfExporting.Columns.Add("Value", typeof(object));
            InformationOfExporting.PrimaryKey = new DataColumn[1] { InformationOfExporting.Columns["Key"] };
            //
            DataTable PricesForExporting = new DataTable("PricesForExporting");
            DataTable DatesForExporting = new DataTable("DatesForExporting");
            //
            // Creating Texts Of Inquiries
            //
            string TextOfInquiryOfPrices =
                "SELECT Id_Pharmacy AS 'ID_PH', Id_Product AS 'ID_PR', Price AS 'Price', Date_upd AS 'Updating', " +
                "Is_privilege AS 'Preferential', Is_deleted AS 'Deleting' " + 
                "FROM Price_List " +
                "WHERE Date_upd BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 10) AND @P1;";
            //
            string TextOfInquiryOfDates = 
                "SELECT RIPL.ID_PH AS 'ID', HR.DateOfReception AS 'Date' " + 
                "FROM HistoryOfReceptions AS HR, ReportsOfImportingOfPriceLists AS RIPL " + 
                "WHERE ((HR.DateOfReception BETWEEN (SELECT Date_Service FROM Service WHERE Id_Service = 10) AND @P1) AND " + 
                "(HR.ID = RIPL.ID_HR) AND (RIPL.FullPriceList = 1) AND " + 
                "((RIPL.CountNotConfirmed + RIPL.CountOfAdditions + RIPL.CountOfChanges + RIPL.CountOfDeletings) > 0));";
            //
            // Creating DataAdapter For Filling
            //
            SqlCommand CommandOfSelection = new SqlCommand("", ConnectionToBase);
            CommandOfSelection.Parameters.Add("@P1", SqlDbType.DateTime);
            CommandOfSelection.Parameters["@P1"].Value = DateOfExported;
            SqlDataAdapter FillingWithData = new SqlDataAdapter(CommandOfSelection);
            //
            // Filling Of Data
            //
            try
            {
                CommandOfSelection.CommandText = TextOfInquiryOfPrices;
                FillingWithData.FillSchema(PricesForExporting, SchemaType.Source);
                FillingWithData.Fill(PricesForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при заполнении таблицы PricesForExporting: {0}", E.Message));
                SuccessfulFilling = false;
            }
            //
            try
            {
                CommandOfSelection.CommandText = TextOfInquiryOfDates;
                FillingWithData.FillSchema(DatesForExporting, SchemaType.Source);
                FillingWithData.Fill(DatesForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при заполнении таблицы DatesForExporting: {0}", E.Message));
                SuccessfulFilling = false;
            }
            //
            // Checking
            //
            if ((PricesForExporting.Rows.Count == 0) && (DatesForExporting.Rows.Count == 0))
                SuccessfulFilling = false;
            //
            // Filling Of Information Of Exporting
            //
            if (SuccessfulFilling)
            {
                //
                InformationOfExporting.Rows.Add("CountOfPrices", PricesForExporting.Rows.Count);
                InformationOfExporting.Rows.Add("CountOfChanges", PricesForExporting.Select("Deleting <> 1").Length);
                InformationOfExporting.Rows.Add("CountOfDeleting", PricesForExporting.Select("Deleting =  1").Length);
                //
                InformationOfExporting.Rows.Add("CountOfFullDates", DatesForExporting.Rows.Count);
                //
                ConnectionToBase.Open();
                InformationOfExporting.Rows.Add("DateOfStart", 
                    new SqlCommand("SELECT Date_Service FROM Service WHERE Id_Service = 10", ConnectionToBase).ExecuteScalar());
                ConnectionToBase.Close();
                InformationOfExporting.Rows.Add("DateOfEnd", DateOfExported);
            }
            //
            // Creating File Of Exporting Scripts
            //
            if (SuccessfulFilling)
                try
                {
                    //
                    string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
                    //
                    // Creating Streams Of File
                    //
                    System.IO.FileStream FS = new System.IO.FileStream(
                        PathToFileOfScripts, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(FS, Encoding.Default);
                    //
                    // Writing Scripts In File
                    //
                    try
                    {
                        //
                        foreach (DataRow CurrentPrice in PricesForExporting.Rows)
                        {
                            //
                            // Creating Text Of Command Of Deleting
                            //
                            string TextOfCommandOfDeleting =
                                String.Format(
                                "DELETE FROM Pharm66.Price_List WHERE (Id_Pharmacy = {1} AND Id_Product = {2});{0}",
                                Paragraph, ((int)CurrentPrice["ID_PH"]), ((int)CurrentPrice["ID_PR"]));
                            //
                            SW.Write(TextOfCommandOfDeleting);
                            //
                            // Creating Text Of Command Of Inserting
                            //
                            if (((bool)CurrentPrice["Deleting"]) != true)
                            {
                                string TextOfCommandOfInserting =
                                    String.Format(
                                    "INSERT INTO Pharm66.Price_List " + 
                                    "(Id_Pharmacy, Id_Product, Price, Date_upd, Is_deleted, Is_privilege) {0}" + 
                                    "VALUES ({1}, {2}, {3}, '{4}', 0, {6});{0}",
                                    Paragraph, ((int)CurrentPrice["ID_PH"]), ((int)CurrentPrice["ID_PR"]),
                                    GettingStringOfDecimal(CurrentPrice["Price"]),
                                    GettingStringOfDate(((System.DateTime)CurrentPrice["Updating"])),
                                    GettingStringOfBoolean(((System.Boolean)CurrentPrice["Deleting"])),
                                    GettingStringOfBoolean(((System.Boolean)CurrentPrice["Preferential"])));
                                //
                                SW.Write(TextOfCommandOfInserting);
                                //
                            }
                        }
                        //
                        foreach (DataRow CurrentDate in DatesForExporting.Rows)
                        {
                            //
                            // Creating Text Of Command Of Updating
                            //
                            string TextOfCommandOfUpdating =
                                String.Format(
                                "UPDATE Pharm66.Price_List SET Date_upd = '{2}' WHERE Id_Pharmacy = {1};{0}",
                                Paragraph, 
                                ((int)CurrentDate["ID"]), GettingStringOfDate(((DateTime)CurrentDate["Date"])));
                            //
                            SW.Write(TextOfCommandOfUpdating);
                            //
                        }
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                                String.Format("ERROR Ошибка при записи в файл: {0}: {1}", PathToFileOfScripts, E.Message));
                    }
                    //
                    // Closing Streams
                    //
                    try
                    {
                        SW.Close();
                        FS.Close();
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при закрытии файла: {0}: {1}", PathToFileOfScripts, E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании файла: {0}: {1}", PathToFileOfScripts, E.Message));
                    //
                    // Deleting File
                    //
                    try
                    {
                        if (File.Exists(PathToFileOfScripts))
                            File.Delete(PathToFileOfScripts);
                    }
                    catch (Exception E2)
                    {
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при удалении файла: {0}: {1}", PathToFileOfScripts, E2.Message));
                    }
                }
            //
            // Return
            //
            return InformationOfExporting;
        }

        #endregion

        #endregion


        #region ' Exporting Updatings For Drugstore '

        // Exporting Updatings For Drugstore
        public DataSet ExportingUpdatingsForDrugstore()
        {
            //
            // Return
            //
            return new DataSet("UpdatingsForDrugstore");
        }

        #endregion


        #region ' Exporting Updatings PriceLists For Drugstore '

        //

        #endregion


        #region ' Exporting Updatings For Sites '


        #region ' Creating Scripts Of Updating '

        // Exporting Scripts Of Updating
        public void ExportingScriptsOfUpdating(
            string PathToFileOfScripts,
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
        }

        // Creating Scripts Of Updating
        public void CreatingScriptsOfUpdating(DataSet DataForCreatingScripts, string PathToFileOfScripts)
        {
            //
        }

        // Creating Scripts Of Updating
        public string CreatingScriptsOfUpdating(DataSet DataForCreatingScripts)
        {
            //
            // Return;
            //
            return "";
        }

        #endregion


        #region ' Creating Of Executing Of Stored Procedures '

        // Exporting Stored Procedures Of Updating
        public void ExportingStoredProceduresOfUpdating(
            string PathToFileOfScripts, 
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Exporting Of Modified Data
            //
            DataTable[] DataForExporting = new DataTable[4];
            //
            DataForExporting[0] = 
                GettingModificationsOfDrugstores(DateOfStartingModification, DateOfEndingModification);
            DataForExporting[1] = 
                GettingModificationsOfProducts(DateOfStartingModification, DateOfEndingModification);
            DataForExporting[2] = 
                GettingModificationsOfPriceLists(DateOfStartingModification, DateOfEndingModification);
            DataForExporting[3] = 
                GettingModificationsOfDatesOfPriceLists(DateOfStartingModification, DateOfEndingModification);
            //
            // Checking Of Result Of Exporting
            //
            bool CorrectResultOfExporting = true;
            //
            if (DataForExporting.Length == 0)
                CorrectResultOfExporting = false;
            //
            // !!!
            //
            if (CorrectResultOfExporting)
            {
                //
                // !!!
                //
                //DataForExporting[3]
            }
            //
            // Reporting Of Result Of Exporting 
            //
            if (CorrectResultOfExporting)
            {
                //
                // Recording In Log File
                //
                RecordingInLogFile(
                        String.Format(
                        "Date Of Beginning: {0} Date Of Ending: {1}", 
                        DateOfStartingModification, DateOfEndingModification));
                //
                // Counts Of Drugstores, Products, Prices, Dates
                //
                foreach (DataTable CurrentData in DataForExporting)
                    switch (CurrentData.TableName)
                    {
                        case "Drugstores":
                            {
                                //
                                // Getting Count Of Drugstores
                                //
                                int
                                    CountOfDrugstores = CurrentData.Rows.Count, 
                                    CountOfChanges = CurrentData.Select("Deleting <> 1").Length, 
                                    CountOfDeleting = CurrentData.Select("Deleting =  1").Length;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(
                                        String.Format(
                                        "Count Of Drugstores: {0} Count Of Changes: {1} Count Of Deleting: {2}", 
                                        CountOfDrugstores, CountOfChanges, CountOfDeleting));
                            }
                            break;
                        case "Products":
                            {
                                //
                                // Getting Count Of Products
                                //
                                int
                                    CountOfProducts = CurrentData.Rows.Count, 
                                    CountOfChanges = CurrentData.Select("Deleting <> 1").Length, 
                                    CountOfDeleting = CurrentData.Select("Deleting =  1").Length;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(
                                        String.Format(
                                        "Count Of Products: {0} Count Of Changes: {1} Count Of Deleting: {2}", 
                                        CountOfProducts, CountOfChanges, CountOfDeleting));
                            }
                            break;
                        case "PriceLists":
                            {
                                //
                                // Getting Count Of Prices
                                //
                                int
                                    CountOfPrices = CurrentData.Rows.Count, 
                                    CountOfChanges = CurrentData.Select("Deleting <> 1").Length, 
                                    CountOfDeleting = CurrentData.Select("Deleting =  1").Length;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(
                                        String.Format(
                                        "Count Of Prices: {0} Count Of Changes: {1} Count Of Deleting: {2} ", 
                                        CountOfPrices, CountOfChanges, CountOfDeleting));
                            }
                            break;
                        case "DatesOfPriceLists":
                            {
                                //
                                // Getting Count Of Dates
                                //
                                int CountOfDates = CurrentData.Rows.Count;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(String.Format("Count Of Full Dates: {0} ", CountOfDates));
                            }
                            break;
                    }
            }
            //
            // Creating Of Executing Of Stored Procedures
            //
            string TextOfStoredProcedures = "";
            //
            if (CorrectResultOfExporting)
                TextOfStoredProcedures = CreatingOfExecutingOfStoredProcedures(DataForExporting);
            //
            // Checking Sanction Of Writing
            //
            bool SanctionOfWriting = false;
            //
            if (CorrectResultOfExporting && (TextOfStoredProcedures.Length > 0))
                SanctionOfWriting = true;
            //
            // Writing Text In File
            //
            if (SanctionOfWriting)
                try
                {
                    //
                    // Creating Streams Of Writing
                    //
                    FileStream FS = new FileStream(PathToFileOfScripts, FileMode.Create, FileAccess.Write);
                    StreamWriter SW = new StreamWriter(FS, Encoding.Default);
                    //
                    // Writing Text Of Stored Procedures
                    //
                    SW.WriteLine(TextOfStoredProcedures);
                    //
                    // Closing Streams
                    //
                    try
                    {
                        SW.Close();
                        FS.Close();
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при закрытии потоков файла ({0}): {1}", 
                            PathToFileOfScripts, E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании потоков файла ({0}): {1}", 
                        PathToFileOfScripts, E.Message));
                    //
                    // Deleting File
                    //
                    try
                    {
                        if (File.Exists(PathToFileOfScripts))
                            File.Delete(PathToFileOfScripts);
                    }
                    catch (Exception E2)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при удалении файла ({0}): {1}", 
                            PathToFileOfScripts, E2.Message));
                    }
                }
        }

        // Creating Of Executing Of Stored Procedures (Not Check DataForCreatingScripts)
        public void CreatingOfExecutingOfStoredProcedures(DataTable[] DataForCreatingScripts, string PathToFileOfScripts)
        {
            //
            // !!!
            //
            //string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Creating Streams Of Writing
            //
            FileStream FS = null;
            StreamWriter SW = null;
            bool StreamsAreCreated = false;
            //
            try
            {
                //
                FS = new FileStream(PathToFileOfScripts, FileMode.Create, FileAccess.Write);
                SW = new StreamWriter(FS, Encoding.Default);
                //
                StreamsAreCreated = true;
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при создании потоков файла ({0}): {1}", 
                    PathToFileOfScripts, E.Message));
                //
                StreamsAreCreated = false;
                //
                // Deleting File
                //
                try
                {
                    if (File.Exists(PathToFileOfScripts))
                        File.Delete(PathToFileOfScripts);
                }
                catch (Exception E2)
                {
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при удалении файла ({0}): {1}", 
                        PathToFileOfScripts, E2.Message));
                }
            }
            //
            // Creating Executing Of Stored Procedures And Writing In File
            //
            if (StreamsAreCreated)
            {
                foreach (DataTable CurrentData in DataForCreatingScripts)
                    switch (CurrentData.TableName)
                    {
                        case "Drugstores":
                            {
                                foreach (DataRow CurrentDrugstore in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingDrugstoresOfSite({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9});", 
                                            CurrentDrugstore["ID"], 
                                            CurrentDrugstore["ID_DI"], 
                                            CurrentDrugstore["Name"], 
                                            CurrentDrugstore["Address"], 
                                            CurrentDrugstore["Phone"], 
                                            CurrentDrugstore["Mail"], 
                                            CurrentDrugstore["Site"], 
                                            CurrentDrugstore["Schedule"], 
                                            CurrentDrugstore["Transport"], 
                                            GettingStringOfBoolean((bool)CurrentDrugstore["Deleting"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingDrugstoresOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                        case "Products":
                            {
                                foreach (DataRow CurrentProduct in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingProductsOfSite({0}, {1}, '{2}', '{3}', '{4}', {5});", 
                                            CurrentProduct["ID"], 
                                            CurrentProduct["ID_PG"], 
                                            GettingStringOfString((string)CurrentProduct["Name"]), 
                                            GettingStringOfString((string)CurrentProduct["Composition"]), 
                                            GettingStringOfDate((DateTime)CurrentProduct["Updating"]), 
                                            GettingStringOfBoolean((bool)CurrentProduct["Deleting"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingProductsOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                        case "PriceLists":
                            {
                                foreach (DataRow CurrentPrice in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingPriceListsOfSite({0}, {1}, '{2}', '{3}', {4}, {5});", 
                                            CurrentPrice["ID_PH"], 
                                            CurrentPrice["ID_PR"], 
                                            GettingStringOfDecimal((decimal)CurrentPrice["Price"]), 
                                            GettingStringOfDate((DateTime)CurrentPrice["Updating"]), 
                                            GettingStringOfBoolean((bool)CurrentPrice["Preferential"]), 
                                            GettingStringOfBoolean((bool)CurrentPrice["Deleting"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingPriceListsOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                        case "DatesOfPriceLists":
                            {
                                foreach (DataRow CurrentDate in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingDatesOfPriceListOfSite({0}, '{1}');", 
                                            CurrentDate["ID"], 
                                            GettingStringOfDate((DateTime)CurrentDate["Date"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingDatesOfPriceListOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                    }
            }
            //
            // Closing Streams
            //
            try
            {
                //
                if (SW != null)
                    SW.Close();
                //
                if (FS != null)
                    FS.Close();
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при закрытии потоков файла ({0}): {1}", 
                    PathToFileOfScripts, E.Message));
            }
        }

        // Creating Of Executing Of Stored Procedures (Not Check DataForCreatingScripts)
        public string CreatingOfExecutingOfStoredProcedures(DataTable[] DataForCreatingScripts)
        {
            //
            StringBuilder StringOfExecutingOfStoredProcedures = new StringBuilder();
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Creating Executing Of Stored Procedures
            //
            foreach (DataTable CurrentData in DataForCreatingScripts)
                switch (CurrentData.TableName)
                {
                    case "Drugstores":
                        {
                            foreach (DataRow CurrentDrugstore in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingDrugstoresOfSite({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9});{10}", 
                                        CurrentDrugstore["ID"], 
                                        CurrentDrugstore["ID_DI"], 
                                        CurrentDrugstore["Name"], 
                                        CurrentDrugstore["Address"], 
                                        CurrentDrugstore["Phone"], 
                                        CurrentDrugstore["Mail"], 
                                        CurrentDrugstore["Site"], 
                                        CurrentDrugstore["Schedule"], 
                                        CurrentDrugstore["Transport"], 
                                        GettingStringOfBoolean((bool)CurrentDrugstore["Deleting"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingDrugstoresOfSite: {0}", E.Message));
                                }
                        }
                        break;
                    case "Products":
                        {
                            foreach (DataRow CurrentProduct in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingProductsOfSite({0}, {1}, '{2}', '{3}', '{4}', {5});{6}", 
                                        CurrentProduct["ID"], 
                                        CurrentProduct["ID_PG"], 
                                        GettingStringOfString((string)CurrentProduct["Name"]), 
                                        GettingStringOfString((string)CurrentProduct["Composition"]), 
                                        GettingStringOfDate((DateTime)CurrentProduct["Updating"]), 
                                        GettingStringOfBoolean((bool)CurrentProduct["Deleting"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingProductsOfSite: {0}", E.Message));
                                }
                        }
                        break;
                    case "PriceLists":
                        {
                            foreach (DataRow CurrentPrice in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingPriceListsOfSite({0}, {1}, '{2}', '{3}', {4}, {5});{6}", 
                                        CurrentPrice["ID_PH"], 
                                        CurrentPrice["ID_PR"], 
                                        GettingStringOfDecimal((decimal)CurrentPrice["Price"]), 
                                        GettingStringOfDate((DateTime)CurrentPrice["Updating"]), 
                                        GettingStringOfBoolean((bool)CurrentPrice["Preferential"]), 
                                        GettingStringOfBoolean((bool)CurrentPrice["Deleting"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingPriceListsOfSite: {0}", E.Message));
                                }
                        }
                        break;
                    case "DatesOfPriceLists":
                        {
                            foreach (DataRow CurrentDate in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingDatesOfPriceListOfSite({0}, '{1}');{2}", 
                                        CurrentDate["ID"], 
                                        GettingStringOfDate((DateTime)CurrentDate["Date"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingDatesOfPriceListOfSite: {0}", E.Message));
                                }
                        }
                        break;
                }
            //
            // Return
            //
            return StringOfExecutingOfStoredProcedures.ToString();
        }

        #endregion


        #region ' Getting Strings Of Types Of Data '

        // Getting String Of Decimal
        private string GettingStringOfDecimal(object Value)
        {
            //
            string StringOfDecimal = "0.000";
            //
            // Checking Of Value
            //
            bool ValueIsCorrect = true;
            //
            if ((Value == null) || (Value is DBNull) || !(Value is Decimal))
                ValueIsCorrect = false;
            //
            // Formating Value
            //
            if (ValueIsCorrect)
            {
                try
                {
                    //
                    Decimal ValueForFormating = Convert.ToDecimal(Value);
                    //
                    StringOfDecimal = String.Format("{0:f3}", ValueForFormating);
                    StringOfDecimal = StringOfDecimal.Replace(",", ".");
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при конвертации Decimal: {0}", E.Message)); }
            }
            //
            // Return
            //
            return StringOfDecimal;
        }

        // Getting String Of Date
        private string GettingStringOfDate(DateTime Value)
        {
            //
            string StringOfDate = "";
            //
            // Formating Value
            //
            StringOfDate = 
                String.Format("{0}-{1}-{2} {3}:{4}:{5}", 
                Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second);
            //
            // Return
            //
            return StringOfDate;
        }

        // Getting String Of Boolean
        private string GettingStringOfBoolean(bool Value)
        {
            //
            string StringOfBoolean = "0";
            //
            // Formating Value
            //
            StringOfBoolean = String.Format("{0}", (Value) ? 1 : 0);
            //
            // Return
            //
            return StringOfBoolean;
        }

        // Getting String Of String
        private string GettingStringOfString(string Value)
        {
            //
            string StringOfString = "";
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Formating Value
            //
            StringOfString = Value.Replace(Paragraph, "");
            //
            // Return
            //
            return StringOfString;
        }

        #endregion

        #endregion


        #region ' Getting Modifications '

        // Getting Modifications Of Drugstores
        public DataTable GettingModificationsOfDrugstores(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Creating Getting
            //
            string NameOfTable = "Drugstores";
            string TextOfInquiry = 
                "SELECT Id_Pharmacy AS 'ID', Id_District AS 'ID_DI', Name_full AS 'Name', Addr AS 'Address', Phone AS 'Phone', Mail AS 'Mail', Web AS 'Site', Hours AS 'Schedule', Trans AS 'Transport', Is_deleted AS 'Deleting' " + 
                "FROM Pharmacy " + 
                "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("Drugstores");
            //
            ModifiedData = 
                GettingModificationsOfData(
                NameOfTable, TextOfInquiry, DateOfStartingModification, DateOfEndingModification);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of Groups Of Products
        public DataTable GettingModificationsOfGroupsOfProducts(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Creating Getting
            //
            string NameOfTable = "GroupsOfProducts";
            string TextOfInquiry = 
                "SELECT Id_product_group AS 'ID', Name_full AS 'Name', Date_upd AS 'DateOfUpdating', Is_deleted AS 'Deleting' " + 
                "FROM Product_Group " + 
                "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("GroupsOfProducts");
            //
            ModifiedData = 
                GettingModificationsOfData(
                NameOfTable, TextOfInquiry, DateOfStartingModification, DateOfEndingModification);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of Products
        public DataTable GettingModificationsOfProducts(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Creating Getting
            //
            string NameOfTable = "Products";
            string TextOfInquiry = 
                "SELECT Id_Product AS 'ID', Id_product_group AS 'ID_PG', Name_full AS 'Name', Composition AS 'Composition', Description AS 'Description', Date_upd AS 'Updating', Is_deleted AS 'Deleting' " + 
                "FROM Product " + 
                "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("Products");
            //
            ModifiedData = 
                GettingModificationsOfData(
                NameOfTable, TextOfInquiry, DateOfStartingModification, DateOfEndingModification);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of PriceLists
        public DataTable GettingModificationsOfPriceLists(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Creating Getting
            //
            string NameOfTable = "PriceLists";
            string TextOfInquiry = 
                "SELECT Id_Pharmacy AS 'ID_PH', Id_Product AS 'ID_PR', Price AS 'Price', Date_upd AS 'Updating', " + 
                "Is_privilege AS 'Preferential', Is_deleted AS 'Deleting' " + 
                "FROM Price_List " + 
                "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("PriceLists");
            //
            ModifiedData = 
                GettingModificationsOfData(
                NameOfTable, TextOfInquiry, DateOfStartingModification, DateOfEndingModification);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of DatesOfPriceLists
        public DataTable GettingModificationsOfDatesOfPriceLists(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Creating Getting
            //
            string NameOfTable = "DatesOfPriceLists";
            string TextOfInquiry = 
                "SELECT RIPL.ID_PH AS 'ID', HR.DateOfReception AS 'Date' " + 
                "FROM HistoryOfReceptions AS HR, ReportsOfImportingOfPriceLists AS RIPL " + 
                "WHERE ((HR.DateOfReception BETWEEN @DateOfStarting AND @DateOfEnding) AND " + 
                "(HR.ID = RIPL.ID_HR) AND (RIPL.FullPriceList = 1) AND " + 
                "((RIPL.CountNotConfirmed + RIPL.CountOfAdditions + RIPL.CountOfChanges + RIPL.CountOfDeletings) > 0));";
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("DatesOfPriceLists");
            //
            ModifiedData = 
                GettingModificationsOfData(
                NameOfTable, TextOfInquiry, DateOfStartingModification, DateOfEndingModification);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of Announcements
        public DataTable GettingModificationsOfAnnouncements(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Creating Getting
            //
            string NameOfTable = "CountOfExported"; // Announcements
            string TextOfInquiry = 
                "SELECT ID_PH AS 'ID_PH', ID AS 'ID', Caption AS 'Caption', [Text] AS 'Text', Published AS 'Published', DateOfUpdating AS 'DateOfUpdating' " + 
                "FROM Announcements " + 
                "WHERE (DateOfUpdating BETWEEN @DateOfStarting AND @DateOfEnding);";
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("CountOfExported"); // Announcements
            //
            ModifiedData = 
                GettingModificationsOfData(
                NameOfTable, TextOfInquiry, DateOfStartingModification, DateOfEndingModification);
            //
            // Return
            //
            return ModifiedData;
        }

        private DataTable GettingModificationsOfData(
            string NameOfTable,
            string TextOfInquiry,
            DateTime DateOfStartingModification,
            DateTime DateOfEndingModification)
        {
            //
            DataTable ModifiedData = new DataTable(NameOfTable);
            //
            // Creating Getting Modifications
            //
            SqlCommand SelectionOfData = new SqlCommand(TextOfInquiry, ConnectionToBase);
            //
            SelectionOfData.Parameters.Add("@DateOfStarting", SqlDbType.DateTime);
            SelectionOfData.Parameters.Add("@DateOfEnding", SqlDbType.DateTime);
            SelectionOfData.Parameters["@DateOfStarting"].Value = DateOfStartingModification;
            SelectionOfData.Parameters["@DateOfEnding"].Value = DateOfEndingModification;
            //
            SqlDataAdapter FillingWithData = new SqlDataAdapter(SelectionOfData);
            //
            // Filling With Data
            //
            try
            {
                FillingWithData.FillSchema(ModifiedData, SchemaType.Source);
                FillingWithData.Fill(ModifiedData);
            }
            catch (Exception E)
            {
                RecordingInLogFile(
                  String.Format("ERROR Ошибка при заполнении таблицы {0}: {1}", NameOfTable, E.Message));
            }
            //
            // Return
            //
            return ModifiedData;
        }

        #endregion


        #region ' Management Of Dates And Numbers '

        // Getting Current Date Of Storage
        public DateTime GettingCurrentDateOfStorage()
        {
            //
            DateTime CurrentDateOfStorage = new DateTime(1947, 07, 02);
            //
            // Creating Getting
            //
            SqlCommand GettingData = new SqlCommand("SELECT GetDate() AS 'CurrentDate';", ConnectionToBase);
            //
            // Getting Current Date
            //
            try
            {
                //
                GettingData.Connection.Open();
                //
                CurrentDateOfStorage = Convert.ToDateTime(GettingData.ExecuteScalar());
                //
                GettingData.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { GettingData.Connection.Close(); }
                catch (Exception E2)
                { this.RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                CurrentDateOfStorage = new DateTime(1947, 07, 02);
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при получении текущей даты хранилища данных: {0}", E.Message));
            }
            //
            // Return
            //
            return CurrentDateOfStorage;
        }

        // Getting Date
        public DateTime GettingDate(int IDOfDate)
        {
            //
            DateTime ReturnedDate = new DateTime(1947, 07, 02);
            //
            // Creating Getting
            //
            SqlCommand SelectionOfDate = 
                new SqlCommand("SELECT Date_Service FROM Service WHERE Id_Service = @ID;", ConnectionToBase);
            //
            SelectionOfDate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            SelectionOfDate.Parameters["@ID"].Value = IDOfDate;
            //
            // Getting Date
            //
            try
            {
                //
                SelectionOfDate.Connection.Open();
                //
                ReturnedDate = Convert.ToDateTime(SelectionOfDate.ExecuteScalar());
                //
                SelectionOfDate.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { SelectionOfDate.Connection.Close(); }
                catch (Exception E2)
                { this.RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                ReturnedDate = new DateTime(1947, 07, 02);
                //
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при получении даты: {0}", E.Message));
            }
            //
            // Return
            //
            return ReturnedDate;
        }

        // Getting Number
        public int GettingNumber(int IDOfNumber)
        {
            //
            int ReturnedNumber = 0;
            //
            // Creating Getting
            //
            SqlCommand SelectionOfDate = 
                new SqlCommand("SELECT Value FROM Service WHERE Id_Service = @ID;", ConnectionToBase);
            //
            SelectionOfDate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            SelectionOfDate.Parameters["@ID"].Value = ReturnedNumber;
            //
            // Getting Date
            //
            try
            {
                //
                SelectionOfDate.Connection.Open();
                //
                ReturnedNumber = Convert.ToInt32(SelectionOfDate.ExecuteScalar());
                //
                SelectionOfDate.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { SelectionOfDate.Connection.Close(); }
                catch (Exception E2)
                { this.RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                ReturnedNumber = 0;
                //
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при получении номера: {0}", E.Message));
            }
            //
            // Return
            //
            return ReturnedNumber;
        }

        // Updating Number
        public void UpdatingNumber(int IDOfNumber, int NewNumber)
        {
            //
            // Creating Updating
            //
            SqlCommand CommandOfUpdatingNumber = new SqlCommand(
                "UPDATE Service SET Value = @Value WHERE Id_Service = @ID;", ConnectionToBase);
            //
            CommandOfUpdatingNumber.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            CommandOfUpdatingNumber.Parameters["@ID"].Value = IDOfNumber;
            //
            CommandOfUpdatingNumber.Parameters.Add(new SqlParameter("@Value", SqlDbType.Int));
            CommandOfUpdatingNumber.Parameters["@Value"].Value = NewNumber;
            //
            // Updating Date
            //
            try
            {
                //
                CommandOfUpdatingNumber.Connection.Open();
                //
                CommandOfUpdatingNumber.ExecuteNonQuery();
                //
                CommandOfUpdatingNumber.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { CommandOfUpdatingNumber.Connection.Close(); }
                catch (Exception E2)
                { this.RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении номера: {0}", E.Message));
            }
        }

        // Updating Date 
        public void UpdatingDate(int IDOfDate, DateTime NewDate)
        {
            //
            // Creating Updating
            //
            SqlCommand CommandOfUpdatingDate = new SqlCommand(
                "UPDATE Service SET Date_Service = @Date WHERE Id_Service = @ID;", ConnectionToBase);
            //
            CommandOfUpdatingDate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            CommandOfUpdatingDate.Parameters["@ID"].Value = IDOfDate;
            //
            CommandOfUpdatingDate.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime));
            CommandOfUpdatingDate.Parameters["@Date"].Value = NewDate;
            //
            // Updating Date
            //
            try
            {
                //
                CommandOfUpdatingDate.Connection.Open();
                //
                CommandOfUpdatingDate.ExecuteNonQuery();
                //
                CommandOfUpdatingDate.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { CommandOfUpdatingDate.Connection.Close(); }
                catch (Exception E2)
                { this.RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении даты: {0}", E.Message));
            }
        }

        // Increment Of Number
        public void IncrementOfNumber(int IDOfNumber)
        {
            //
            // Creating Increment
            //
            SqlCommand CommandOfUpdatingNumber = new SqlCommand(
                    "UPDATE Service SET Value = Value + 1 WHERE Id_Service = @ID;", ConnectionToBase);
            //
            CommandOfUpdatingNumber.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            CommandOfUpdatingNumber.Parameters["@ID"].Value = IDOfNumber;
            //
            // Increment Number
            //
            try
            {
                //
                CommandOfUpdatingNumber.Connection.Open();
                //
                CommandOfUpdatingNumber.ExecuteNonQuery();
                //
                CommandOfUpdatingNumber.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { CommandOfUpdatingNumber.Connection.Close(); }
                catch (Exception E2)
                { this.RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при приращении номера: {0}", E.Message));
            }
        }

        // Increment Of Number
        public void IncrementOfNumber(int IDOfNumber, DateTime NewDate)
        {
            //
        }

        #endregion


        // Exporting Of Modified Data
        /*public DataTable[] ExportingOfModifiedData(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification, 
            bool FillingOfDrugstores, 
            bool FillingOfProducts, 
            bool FillingOfPriceLists, 
            bool FillingOfDatesOfPriceLists)
        {
            //
            DataTable[] ListOfModifiedData = new DataTable[4]
            {
                new DataTable("Drugstores"), new DataTable("Products"), 
                new DataTable("PriceLists"), new DataTable("DatesOfPriceLists")
            };
            //
            // Creating Exporting Of Data
            //
            SqlCommand SelectionOfData = new SqlCommand("", ConnectionToBase);
            //
            SelectionOfData.Parameters.Add("@DateOfStarting", SqlDbType.DateTime);
            SelectionOfData.Parameters.Add("@DateOfEnding", SqlDbType.DateTime);
            SelectionOfData.Parameters["@DateOfStarting"].Value = DateOfStartingModification;
            SelectionOfData.Parameters["@DateOfEnding"].Value = DateOfEndingModification;
            //
            SqlDataAdapter FillingWithData = new SqlDataAdapter(SelectionOfData);
            //
            // Filling With Drugstores
            //
            if (FillingOfDrugstores)
            {
                //
                // Creating Text Of Inquiry
                //
                string TextOfInquiry = 
                    "SELECT Id_Pharmacy AS 'ID', Id_District AS 'ID_DI', Name_full AS 'Name', Addr AS 'Address', Phone AS 'Phone', Mail AS 'Mail', Web AS 'Site', Hours AS 'Schedule', Trans AS 'Transport', Is_deleted AS 'Deleting' " + 
                    "FROM Pharmacy " + 
                    "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
                //
                // Assignment Of Text Of Inquiry In Selection
                //
                SelectionOfData.CommandText = TextOfInquiry;
                //
                // Filling With Data
                //
                try
                {
                    FillingWithData.FillSchema(ListOfModifiedData[0], SchemaType.Source);
                    FillingWithData.Fill(ListOfModifiedData[0]);
                }
                catch (Exception E)
                {
                    RecordingInLogFile(
                      String.Format("ERROR Ошибка при заполнении таблицы изменений аптек: {0}", E.Message));
                }
            }
            //
            // Filling With Products
            //
            if (FillingOfProducts)
            {
                //
                // Creating Text Of Inquiry
                //
                string TextOfInquiry = 
                    "SELECT Id_Product AS 'ID', Id_product_group AS 'ID_PG', Name_full AS 'Name', Composition AS 'Composition', Date_upd AS 'Updating', Is_deleted AS 'Deleting' " + 
                    "FROM Product " + 
                    "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
                //
                // Assignment Of Text Of Inquiry In Selection
                //
                SelectionOfData.CommandText = TextOfInquiry;
                //
                // Filling With Data
                //
                try
                {
                    FillingWithData.FillSchema(ListOfModifiedData[1], SchemaType.Source);
                    FillingWithData.Fill(ListOfModifiedData[1]);
                }
                catch (Exception E)
                {
                    RecordingInLogFile(
                      String.Format("ERROR Ошибка при заполнении таблицы изменений продуктов: {0}", E.Message));
                }
            }
            //
            // Filling With PriceLists
            //
            if (FillingOfPriceLists)
            {
                //
                // Creating Text Of Inquiry
                //
                string TextOfInquiry = 
                    "SELECT Id_Pharmacy AS 'ID_PH', Id_Product AS 'ID_PR', Price AS 'Price', Date_upd AS 'Updating', " + 
                    "Is_privilege AS 'Preferential', Is_deleted AS 'Deleting' " + 
                    "FROM Price_List " + 
                    "WHERE (Date_upd BETWEEN @DateOfStarting AND @DateOfEnding);";
                //
                // Assignment Of Text Of Inquiry In Selection
                //
                SelectionOfData.CommandText = TextOfInquiry;
                //
                // Filling With Data
                //
                try
                {
                    FillingWithData.FillSchema(ListOfModifiedData[2], SchemaType.Source);
                    FillingWithData.Fill(ListOfModifiedData[2]);
                }
                catch (Exception E)
                {
                    RecordingInLogFile(
                      String.Format("ERROR Ошибка при заполнении таблицы изменений Прайс-Листов: {0}", E.Message));
                }
            }
            //
            // Filling With Dates Of PriceLists
            //
            if (FillingOfDatesOfPriceLists)
            {
                //
                // Creating Text Of Inquiry
                //
                string TextOfInquiry = 
                    "SELECT RIPL.ID_PH AS 'ID', HR.DateOfReception AS 'Date' " + 
                    "FROM HistoryOfReceptions AS HR, ReportsOfImportingOfPriceLists AS RIPL " + 
                    "WHERE ((HR.DateOfReception BETWEEN @DateOfStarting AND @DateOfEnding) AND " + 
                    "(HR.ID = RIPL.ID_HR) AND (RIPL.FullPriceList = 1) AND " + 
                    "((RIPL.CountNotConfirmed + RIPL.CountOfAdditions + RIPL.CountOfChanges + RIPL.CountOfDeletings) > 0));";
                //
                // Assignment Of Text Of Inquiry In Selection
                //
                SelectionOfData.CommandText = TextOfInquiry;
                //
                // Filling With Data
                //
                try
                {
                    FillingWithData.FillSchema(ListOfModifiedData[3], SchemaType.Source);
                    FillingWithData.Fill(ListOfModifiedData[3]);
                }
                catch (Exception E)
                {
                    RecordingInLogFile(
                      String.Format("ERROR Ошибка при заполнении таблицы дат обновлений: {0}", E.Message));
                }
            }
            //
            // Return
            //
            return ListOfModifiedData;
        }*/

    }
}