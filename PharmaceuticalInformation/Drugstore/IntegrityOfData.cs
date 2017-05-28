using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class IntegrityOfData : BaseType
    {

        #region ' Fields '

        private string StringOfConnection;
        private MySqlConnection ConnectionToBase;

        #endregion

        #region ' Designer '

        public IntegrityOfData(string StringOfConnection)
            : this(StringOfConnection, "")
        {
            //
        }

        public IntegrityOfData(string StringOfConnection, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initialize Fields
            //
            this.StringOfConnection = StringOfConnection;
            //
            // Creating Of Connection
            //
            try
            {
                ConnectionToBase = new MySqlConnection(StringOfConnection);
                try
                {
                    ConnectionToBase.Open();
                    ConnectionToBase.Close();
                }
                catch (Exception E) 
                { throw new Exception(String.Format("Ошибка при открытии подключения целостности: {0}", E)); }
            }
            catch (Exception E) 
            { throw new Exception(String.Format("Ошибка при создании подключения целостности: {0}", E)); }
            //
        }

        #endregion

        #region ' Correction Of Data Base '

        //
        public void CorrectionOfDataBase()
        {
            //
            // Creating Table For Comparisons
            //
            CreatingTableForComparisons();
            //
            // Removal Of Recurrences Of PriceList (Много времени)
            //
            RemovalOfRecurrencesOfPriceList("Price_List", false);
            //
            // Creating Compound ID For Price_Lists
            //
            CreatingCompoundID("Price_List");
            //
            // Removal Of Recurrences Of Exported PriceList
            //
            RemovalOfRecurrencesOfPriceList("exp_price_list", false); /// !!!
            //
            // Creating Compound ID For exp_price_list
            //
            CreatingCompoundID("exp_price_list");  /// !!!
            //
            // Modification Of Table Of Service 01
            //
            //ModificationOfTableOfService_01();
            //
            // Modification Of Table Of ExpPriceList
            //
            //ModificationOfTableOfExpPriceList();
            //
            // Modificaton Of Group Of Products
            //
            //ModificatonOfGroupOfProducts();
            //
            // Creating Stored Procedures
            //
            CreatingStoredProcedures();
        }

        // Creating Table For Comparisons
        private void CreatingTableForComparisons()
        {
            //
            // Checking Existence Of Table Of Comparisons
            //
            bool ExistenceOfTableOfComparisons = false;
            //
            // Creating Checking
            //
            string TextOfCommandOfChecking = 
                "SELECT COUNT(*) FROM Information_Schema.Tables WHERE Table_Name = 'Comparisons';";
            MySqlCommand CommandOfChecking = new MySqlCommand(TextOfCommandOfChecking, ConnectionToBase);
            //
            // Checking
            //
            int CountOfTables = 0;
            //
            try
            {
                //
                OpeningConnection(CommandOfChecking.Connection);
                //
                CountOfTables = Convert.ToInt32(CommandOfChecking.ExecuteScalar());
                //
                ClosingConnection(CommandOfChecking.Connection);
            }
            catch (Exception E)
            {
                //
                try { ClosingConnection(CommandOfChecking.Connection); }
                catch (Exception E2)
                { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                //
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при проверке существования таблицы Comparisons: {0}", E.Message));
            }
            //
            if (CountOfTables > 0) ExistenceOfTableOfComparisons = true;
            //
            // Creating Of Table Of Comparisons
            //
            bool CreatingTableOfComparisons = false;
            //
            if (!ExistenceOfTableOfComparisons)
            {
                //
                // Creating Creating
                //
                string TextOfCommandOfCreating = 
                    "CREATE TABLE Comparisons" + 
                    "( " + 
                    "IDOfExternalProduct   INT(10)       NOT NULL PRIMARY KEY, " + 
                    "NameOfExternalProduct VARCHAR(255)  NOT NULL, " + 
                    "IDOfProduct           INT(10)       NOT NULL, " + 
                    "ExistsInPriceList     BOOL          NOT NULL, " + 
                    "Compare               BOOL          NOT NULL " + 
                    ") " + 
                    "DEFAULT CHARSET = cp1251;";
                //  -- NameOfProduct         VARCHAR (255) NOT NULL,
                MySqlCommand CommandOfCreating = new MySqlCommand(TextOfCommandOfCreating, ConnectionToBase);
                //
                // Creating
                //
                try
                {
                    //
                    OpeningConnection(CommandOfCreating.Connection);
                    //
                    CommandOfCreating.ExecuteNonQuery();
                    //
                    ClosingConnection(CommandOfCreating.Connection);
                    //
                    CreatingTableOfComparisons = true;
                }
                catch (Exception E)
                {
                    //
                    try { ClosingConnection(CommandOfCreating.Connection); }
                    catch (Exception E2)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                    //
                    RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании таблицы Comparisons: {0}", E.Message));
                }
            }
            //
            // Filling Of Table Of Comparisons Of List Of External Products
            //
            if (CreatingTableOfComparisons)
            {
                //
                // Creating Filling
                //
                string TextOfCommandOfFilling = 
                    "INSERT " + 
                    "INTO Comparisons (IDOfExternalProduct, NameOfExternalProduct, IDOfProduct, ExistsInPriceList, Compare) " + 
                    "SELECT " + 
                    "Id_client_product, Name_full, 0, FALSE, TRUE " + 
                    "FROM " + 
                    "Client_ref;";
                MySqlCommand CommandOfFilling = new MySqlCommand(TextOfCommandOfFilling, ConnectionToBase);
                //
                // Filling
                //
                try
                {
                    //
                    OpeningConnection(CommandOfFilling.Connection);
                    //
                    CommandOfFilling.ExecuteNonQuery();
                    //
                    ClosingConnection(CommandOfFilling.Connection);
                }
                catch (Exception E)
                {
                    //
                    try { ClosingConnection(CommandOfFilling.Connection); }
                    catch (Exception E2)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                    //
                    RecordingInLogFile(
                        String.Format("ERROR Ошибка при заполнении таблицы Comparisons списком продуктов: {0}", E.Message));
                }
            }
            //
            // Filling Of Table Of Comparisons Of List Of Compared Products
            //
            if (CreatingTableOfComparisons)
            {
                //
                // Creating Filling
                //
                string TextOfCommandOfFilling = 
                    "UPDATE " + 
                    "Comparisons AS C, Matching AS M " + 
                    "SET " + 
                    "C.IDOfProduct = M.Id_product " + 
                    "WHERE " + 
                    "C.IDOfExternalProduct = M.Id_client_product;";
                MySqlCommand CommandOfFilling = new MySqlCommand(TextOfCommandOfFilling, ConnectionToBase);
                //
                // Filling
                //
                try
                {
                    //
                    OpeningConnection(CommandOfFilling.Connection);
                    //
                    CommandOfFilling.ExecuteNonQuery();
                    //
                    ClosingConnection(CommandOfFilling.Connection);
                }
                catch (Exception E)
                {
                    //
                    try { ClosingConnection(CommandOfFilling.Connection); }
                    catch (Exception E2)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения: {0}", E2.Message)); }
                    //
                    RecordingInLogFile(
                        String.Format("ERROR Ошибка при заполнении таблицы Comparisons сопоставлениями: {0}", E.Message));
                }
            }
        }

        //
        private void ModificationOfTableOfService_01()
        {
            //
            // Initialize Correcting Instructions
            //
            MySqlCommand CommandOfChecking =
                new MySqlCommand(
                    "SELECT COUNT(*) FROM Service " +
                    "WHERE ((Id_Service = 9) AND NOT((Year(Date_Service) = '2010') AND (Month(Date_Service) = '08') AND " +
                    "(Day(Date_Service) = '13')))", ConnectionToBase);
            //
            MySqlCommand CommandOfModification = new MySqlCommand("", ConnectionToBase);
            //
            string[] CorrectingInstructions = new string[4]
            {
                "UPDATE Service SET Value = '0';", 
                "INSERT INTO Service (Id_Service, Name_short, Value, Date_Service) VALUES (9, 'DateOfUpdating', 900, '2010-10-15');", 
                "ALTER TABLE Service MODIFY COLUMN Value INT NOT NULL;", 
                "UPDATE Service SET Value = 200 WHERE (Id_Service = 9);", 
            };
            //
            ConnectionToBase.Open();
            int CountOfRows = Convert.ToInt32(CommandOfChecking.ExecuteScalar());
            ConnectionToBase.Close();
            //
            if (CountOfRows != 1)
            {
                //
                this.RecordingInLogFile("CHANGE Изменение структуры Базы Данных");
                ConnectionToBase.Open();
                foreach (string CurrentTextOfCommand in CorrectingInstructions)
                {
                    //
                    CommandOfModification.CommandText = CurrentTextOfCommand;
                    //
                    try { CommandOfModification.ExecuteNonQuery(); }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при изменении структуры БД", E, true); }
                }
                ConnectionToBase.Close();
            }
        }

        //
        private void ModificationOfTableOfExpPriceList()
        {
            //
            // Initialize Correcting Instructions
            //
            MySqlCommand CommandOfChecking =
                new MySqlCommand("SHOW COLUMNS FROM exp_price_list;", ConnectionToBase);
            MySqlDataAdapter Checking = new MySqlDataAdapter(CommandOfChecking);
            //
            MySqlCommand CommandOfModification = new MySqlCommand("", ConnectionToBase);
            //
            string[] CorrectingInstructions = new string[2]
            {
                "ALTER TABLE exp_price_list ADD COLUMN Sent BOOL NOT NULL;", 
                "UPDATE exp_price_list SET Sent = 0;"
            };
            //
            DataTable SchemaOfExpPriceList = new DataTable("ExpPriceList");
            Checking.Fill(SchemaOfExpPriceList);
            //
            if (SchemaOfExpPriceList.Rows.Count == 8)
            {
                //
                this.RecordingInLogFile("CHANGE Изменение структуры Базы Данных");
                ConnectionToBase.Open();
                foreach (string CurrentTextOfCommand in CorrectingInstructions)
                {
                    //
                    CommandOfModification.CommandText = CurrentTextOfCommand;
                    //
                    try { CommandOfModification.ExecuteNonQuery(); }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при изменении структуры БД", E, true); }
                }
                ConnectionToBase.Close();
            }
        }

        //
        private void ModificatonOfGroupOfProducts()
        {
            //
            // Initialize Correcting Instructions
            //
            MySqlCommand CommandOfChecking =
                new MySqlCommand(
                    "SELECT COUNT(*) FROM Product_group WHERE ((Id_Product_group = 108) AND (Is_deleted = 0));", 
                    ConnectionToBase);
            //
            MySqlCommand CommandOfModification = new MySqlCommand("", ConnectionToBase);
            //
            string[] CorrectingInstructions = new string[4]
            {
                "INSERT INTO Product_Group(Id_product_group, Name_full, Date_upd, Is_deleted) VALUE (108, 'Сборка', NULL, 0);", 
                "UPDATE Product_Group SET Is_deleted = 0 WHERE Id_product_group = 108;", 
                "DELETE FROM Product_Group WHERE ID_PRODUCT_GROUP IN (972, 973, 974, 975, 976);", 
                "UPDATE Product SET Id_product_group = 108 WHERE Id_product_group = 0;"
            };
            //
            ConnectionToBase.Open();
            int CountOfRows = Convert.ToInt32(CommandOfChecking.ExecuteScalar());
            ConnectionToBase.Close();
            //
            if (CountOfRows == 0)
            {
                //
                this.RecordingInLogFile("CHANGE Изменение структуры Базы Данных");
                ConnectionToBase.Open();
                foreach (string CurrentTextOfCommand in CorrectingInstructions)
                {
                    //
                    CommandOfModification.CommandText = CurrentTextOfCommand;
                    //
                    try { CommandOfModification.ExecuteNonQuery(); }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при изменении структуры БД", E, true); }
                }
                ConnectionToBase.Close();
            }
        }

        // Removal Of Recurrences Of PriceList
        public void RemovalOfRecurrencesOfPriceList(string NameOfTable, bool Compulsorily)
        {
            //
            // Initializing Checking Command
            //
            MySqlCommand CommandOfChecking =
                new MySqlCommand(
                    "SELECT COUNT(*) FROM Service " +
                    "WHERE ((Id_Service = 9) AND NOT((Year(Date_Service) = '2010') AND (Month(Date_Service) = '08') AND " + 
                    "(Day(Date_Service) = '13')))", ConnectionToBase);
            //
            ConnectionToBase.Open();
            int CountOfRows = Convert.ToInt32(CommandOfChecking.ExecuteScalar());
            ConnectionToBase.Close();
            //
            CommandOfChecking.CommandText = 
                "SELECT COUNT(*) FROM Price_List AS PL " + 
                "WHERE (SELECT COUNT(*) FROM Price_List AS PL2 " + 
                "WHERE PL.Id_Pharmacy = PL2.Id_Pharmacy AND PL.Id_Product = PL2.Id_Product) > 1 "+
                "ORDER BY PL.Id_Pharmacy, PL.ID_Product, PL.Price;";
            //
            //ConnectionToBase.Open();
            //int CountOfRecurrences = Convert.ToInt32(CommandOfChecking.ExecuteScalar());
            //ConnectionToBase.Close();
            //
            //SELECT * FROM Pharm66.Price_List WHERE Id_Pharmacy = Id_Product
            //
            if ((CountOfRows != 1) || Compulsorily)// || (CountOfRecurrences > 0))
            {
                //
                // Formating Text Of Selection
                //
                string TextOfSelectionCommand =
                    String.Format(
                    "SELECT PL.Id_Price_List AS 'ID', PL.Id_Pharmacy AS 'ID_PH', PL.ID_Product AS 'ID_PR' " +
                    "FROM {0} AS PL " +
                    "WHERE (SELECT COUNT(*) FROM {0} AS PL2 " +
                    "WHERE PL.Id_Pharmacy = PL2.Id_Pharmacy AND PL.Id_Product = PL2.Id_Product) > 1 " +
                    "ORDER BY PL.Id_Pharmacy, PL.ID_Product, PL.Price;", NameOfTable);
                //
                // Initialize UpdatingPriceList
                //
                MySqlCommand CommandOfSelection = new MySqlCommand(TextOfSelectionCommand, ConnectionToBase);
                //
                MySqlCommand CommandOfDeleting = new MySqlCommand(
                    String.Format("DELETE FROM {0} WHERE Id_Price_list = @P1;", NameOfTable),
                    ConnectionToBase);
                CommandOfDeleting.Parameters.Add("@P1", MySqlDbType.UInt32, 0, "ID");
                //
                MySqlDataAdapter UpdatingPriceList = new MySqlDataAdapter(CommandOfSelection);
                UpdatingPriceList.DeleteCommand = CommandOfDeleting;
                //
                // Filling PriceList
                //
                DataTable PriceListForClearing = new DataTable();
                UpdatingPriceList.FillSchema(PriceListForClearing, SchemaType.Source);
                UpdatingPriceList.Fill(PriceListForClearing);
                //
                // Deleting Of Recurrences
                //
                // Getting IDs Of Drugstores
                //
                DataTable TableOfIDsOfDrugstores = new DataView(PriceListForClearing).ToTable(true, "ID_PH");
                int[] IDsOfDrugstores = new int[TableOfIDsOfDrugstores.Rows.Count];
                for (int i = 0; i < IDsOfDrugstores.Length; i++)
                    IDsOfDrugstores[i] = Convert.ToInt32(TableOfIDsOfDrugstores.Rows[i]["ID_PH"]);
                //
                // Importing Of Drugstores
                //
                DataView GettingDrugstore = new DataView(PriceListForClearing);
                for (int i = 0; i < IDsOfDrugstores.Length; i++)
                {
                    //
                    // Getting Prices Of Drugstore
                    //
                    GettingDrugstore.RowFilter = String.Format("ID_PH={0}", IDsOfDrugstores[i]);
                    DataTable PricesOfDrugstore = GettingDrugstore.ToTable("PricesOfDrugstore");
                    PricesOfDrugstore.AcceptChanges();
                    //
                    // Addition Of TMP Of ID Of Prices
                    //
                    PricesOfDrugstore.PrimaryKey = new DataColumn[1] { PricesOfDrugstore.Columns["ID"] };
                    //
                    // Filling ID Of Prices And ID_PR Of Products
                    //
                    UInt32[,] IDAndIDPROfPrices = new UInt32[PricesOfDrugstore.Rows.Count, 2];
                    for (int i2 = 0; i2 <= IDAndIDPROfPrices.GetUpperBound(0); i2++)
                    {
                        IDAndIDPROfPrices[i2, 0] = (UInt32)PricesOfDrugstore.Rows[i2]["ID"];
                        IDAndIDPROfPrices[i2, 1] = (UInt32)PricesOfDrugstore.Rows[i2]["ID_PR"];
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
                    int CountOfDeleting = 0;
                    foreach (DataRow CurRow in PricesOfDrugstore.Rows)
                        if (CurRow.RowState == DataRowState.Deleted)
                            CountOfDeleting++;
                    this.RecordingInLogFile(
                        String.Format("Удаление повторяющихся строк в таблице {0}, Повторений: {1}, Удалений: {2}",
                        NameOfTable, PricesOfDrugstore.Rows.Count, CountOfDeleting));
                    //
                    // Clearing PriceList
                    //
                    UpdatingPriceList.Update(PricesOfDrugstore);
                }
            }
        }

        // Creating Stored Procedures
        private void CreatingStoredProcedures()
        {
            //
            // Formatting Text Of Stored Procedures
            //
            //string[] TextsOfStoredProcedures = new string[9];
            ArrayList TextsOfStoredProcedures = new ArrayList();
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.UpdatingDrugstores(IN ID INT, IN ID_DI INT, IN Name VARBINARY (255), IN Address VARBINARY (255), IN Phone VARBINARY (127), IN Mail VARBINARY (63), IN Site VARBINARY (127), IN Schedule VARBINARY (255), IN Transport VARBINARY (255), IN Deleting BINARY) " + 
                "BEGIN " + 
                "" + 
                "IF (Deleting = 1) THEN " + 
                "" + 
                "UPDATE " + 
                "Pharmacy " + 
                "SET " + 
                "Is_deleted = 1 " + 
                "WHERE " + 
                "(Id_Pharmacy = ID); " + 
                "" + 
                "DELETE " + 
                "FROM "+ 
                "Price_List " + 
                "WHERE " + 
                "Id_Pharmacy = ID; " + 
                "" + 
                "ELSE " + 
                "" + 
                "IF (EXISTS (SELECT " + 
                "* " + 
                "FROM " + 
                "Pharmacy AS D " + 
                "WHERE " + 
                "(D.Id_Pharmacy = ID))) THEN " + 
                "" + 
                "UPDATE " + 
                "Pharmacy AS D " + 
                "SET " +
                "D.Name_full = Name, D.Addr = Address, D.Phone = Phone, D.Mail = Mail, D.Web = Site, D.Hours = Schedule, D.Trans = Transport, D.Id_District = ID_DI, D.Is_deleted = Deleting " + 
                "WHERE " + 
                "(D.Id_Pharmacy = ID); " + 
                "" + 
                "ELSE " + 
                "" + 
                "INSERT " + 
                "INTO Pharmacy (Id_Pharmacy, Name_full, Addr, Phone, Mail, Web, Hours, Trans, Id_District, Is_deleted) " + 
                "VALUES (ID, Name, Address, Phone, Mail, Site, Schedule, Transport, ID_DI, Deleting); " + 
                "END IF; " + 
                "END IF; " + 
                "END");
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.UpdatingGroupsOfProducts(IN ID INT, IN Name VARBINARY (255), IN DateOfUpdating DATETIME, IN Deleting BINARY) " + 
                "BEGIN " + 
                "" + 
                "IF (Deleting = 1) THEN " + 
                "UPDATE " + 
                "Product_group " + 
                "SET " + 
                "Is_deleted = 1 " + 
                "WHERE " + 
                "(Id_product_group = ID); " + 
                "" + 
                "ELSE " + 
                "" + 
                "IF (EXISTS (SELECT " + 
                "* " + 
                "FROM " + 
                "Product_group AS PG " + 
                "WHERE " + 
                "(PG.Id_product_group = ID))) THEN " + 
                "" + 
                "UPDATE " + 
                "Product_group AS PG " + 
                "SET " + 
                "PG.Id_product_group = ID, PG.Name_full = Name, PG.Date_upd = DateOfUpdating, PG.Is_deleted = Deleting " + 
                "WHERE " + 
                "(PG.Id_product_group = ID); " + 
                "" + 
                "ELSE " + 
                "" + 
                "INSERT " + 
                "INTO Product_group (Id_product_group, Name_full, Date_upd, Is_deleted) " + 
                "VALUES (ID, Name, DateOfUpdating, Deleting); " + 
                "" + 
                "END IF; " + 
                "END IF; " + 
                "END");
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.UpdatingOfProducts(IN ID INT, IN ID_PG INT, IN Name VARBINARY (255), IN Composition VARBINARY (255), IN Description VARBINARY (65535), IN Updating DATETIME, IN Deleting BINARY) " + 
                "BEGIN " + 
                "" + 
                "IF (Deleting = 1) THEN " + 
                "UPDATE " + 
                "Product " + 
                "SET " + 
                "Is_deleted = 1 " + 
                "WHERE " + 
                "(Id_Product = ID); " + 
                "" + 
                "ELSE " + 
                "" + 
                "IF (EXISTS (SELECT " + 
                "* " + 
                "FROM " + 
                "Product AS P " + 
                "WHERE " + 
                "(P.Id_Product = ID))) THEN " + 
                "" + 
                "UPDATE " + 
                "Product AS P " + 
                "SET " + 
                "P.Id_product_group = ID_PG, P.Name_full = Name, P.Composition = Composition, P.Description = Description, P.Date_upd = Updating, P.Is_deleted = Deleting " + 
                "WHERE " + 
                "(P.Id_Product = ID); " + 
                "" + 
                "ELSE " + 
                "" + 
                "INSERT " + 
                "INTO Product (Id_Product, Id_product_group, Name_full, Composition, Description, Date_upd, Is_deleted) " + 
                "VALUES (ID, ID_PG, Name, Composition, Description, Updating, Deleting); " + 
                "" + 
                "END IF; " + 
                "END IF; " + 
                "END");
            //
            TextsOfStoredProcedures.Add("DROP PROCEDURE UpdatingPriceList;");
            //
            /*
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.UpdatingPriceList(IN ID_PH INT, IN ID_PR INT, IN Price DECIMAL (12, 3), IN Updating DATETIME, IN Preferential BINARY, IN Deleting BINARY) " + 
                "BEGIN " + 
                "" + 
                "IF (Deleting = 1) THEN " + 
                "" + 
                "DELETE " + 
                "FROM " + 
                "Price_List " + 
                "WHERE " + 
                "((Id_Pharmacy = ID_PH) AND (Id_Product = ID_PR)); " + 
                "" + 
                "ELSE " + 
                "" + 
                "IF (EXISTS (SELECT " + 
                "* " + 
                "FROM " + 
                "Price_List AS PL " + 
                "WHERE " + 
                "((PL.Id_Pharmacy = ID_PH) AND (PL.Id_Product = ID_PR)))) THEN " + 
                "" + 
                "UPDATE " + 
                "Price_list AS PL " + 
                "SET " + 
                "PL.Price = Price, PL.Date_upd = Updating, PL.Is_privilege = Preferential, PL.Is_deleted = Deleting " + 
                "WHERE " + 
                "((PL.Id_Pharmacy = ID_PH) AND (PL.Id_Product = ID_PR));" + 
                "" + 
                "ELSE " + 
                "" + 
                "INSERT " + 
                "INTO Price_list (Id_Pharmacy, ID_Product, Price, Date_upd, Is_privilege, Is_deleted) " + 
                "VALUES (ID_PH, ID_PR, Price, Updating, Preferential, Deleting); " + 
                "" + 
                "END IF; " + 
                "END IF; " + 
                "END");
            */
            //
            TextsOfStoredProcedures.Add("DROP PROCEDURE UpdatingDatesOfPriceList;");
            //
            /*
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.UpdatingDatesOfPriceList(IN ID INT, IN `Date` DATETIME) " + 
                "BEGIN " + 
                "" + 
                "UPDATE " + 
                "Price_list AS PL " + 
                "SET " + 
                "PL.Date_upd = Date " + 
                "WHERE " + 
                "(PL.Id_Pharmacy = ID); " + 
                "" + 
                "UPDATE " + 
                "Pharmacy AS P " + 
                "SET " + 
                "P.Date_upd = Date " + 
                "WHERE " + 
                "(P.Id_Pharmacy = ID); " + 
                "" + 
                "END");
            */
            //
            TextsOfStoredProcedures.Add(
                "CREATE TABLE Pharm66.Announcements( " + 
                "ID_PH          INT (10)      NOT NULL, " + 
                "ID             INT (10)      NOT NULL, " + 
                "Caption        VARCHAR (255) NULL, " + 
                "Text           LONGTEXT      NULL, " + 
                "DateOfUpdating DATETIME      NULL, " +
                "PRIMARY KEY (`ID_PH`,`ID`)) " + 
                "DEFAULT CHARSET = cp1251;");
            //
            TextsOfStoredProcedures.Add("SET GLOBAL MAX_ALLOWED_PACKET = 4294967296;");
            //
            TextsOfStoredProcedures.Add("DROP PROCEDURE UpdatingOfAnnouncements;");
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.UpdatingOfAnnouncements(IN ID_PH INT, IN ID INT, IN Caption VARBINARY (255), IN `Text` LONGTEXT, IN Published BINARY, IN DateOfUpdating DATETIME) " + 
                "BEGIN " + 
                "IF (Published = 0) THEN " + 
                "DELETE " + 
                "FROM " + 
                "Announcements " + 
                "WHERE " + 
                "((Announcements.ID_PH = ID_PH) AND (Announcements.ID = ID)); " + 
                "ELSE " + 
                "IF (EXISTS (SELECT " + 
                "* " + 
                "FROM " + 
                "Announcements AS A " + 
                "WHERE " + 
                "((A.ID_PH = ID_PH) AND (A.ID = ID)))) THEN " + 
                "UPDATE " + 
                "Announcements AS A " + 
                "SET " + 
                "A.Caption = Caption, A.Text = `Text`, A.DateOfUpdating = DateOfUpdating " + 
                "WHERE " + 
                "((A.ID_PH = ID_PH) AND (A.ID = ID)); " + 
                "ELSE " + 
                "INSERT " + 
                "INTO Announcements (ID_PH, ID, Caption, `Text`, DateOfUpdating) " + 
                "VALUES (ID_PH, ID, Caption, Text, DateOfUpdating); " + 
                "END IF; " + 
                "END IF; " + 
                "END");
            //
            TextsOfStoredProcedures.Add("DELETE FROM Pharmacy WHERE Is_deleted = 1;");
            //
            // Procedure Of Importing In PriceList
            //
            TextsOfStoredProcedures.Add("DROP PROCEDURE ImportingInPriceList;");
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE pharm66.ImportingInPriceList(IN ID_PH INT, IN ID_PR INT, IN Price DECIMAL (12, 3), IN Preferential BINARY, IN Deleting BINARY) " + 
                "BEGIN " + 
                "IF " + 
                "(EXISTS " + 
                "(SELECT * FROM exp_price_list AS EPL WHERE ((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR)))) " + 
                "THEN " + 
                "IF " + 
                "(EXISTS " + 
                "(SELECT * FROM exp_price_list AS EPL " + 
                "WHERE (((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR)) OR (EPL.Price <> Price) OR " + 
                "(EPL.Is_privilege <> Preferential) OR (EPL.Is_deleted <> Deleting)))) " + 
                "THEN " + 
                "UPDATE " + 
                "exp_price_list AS EPL " + 
                "SET " + 
                "EPL.Price = Price, EPL.Is_privilege = Preferential, EPL.Is_deleted = Deleting, EPL.Sent = 0 " + 
                "WHERE " + 
                "((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR)); " + 
                "END IF;" + 
                "ELSE " + 
                "INSERT " + 
                "INTO exp_price_list (Id_Pharmacy, ID_Product, Price, Is_privilege, Is_deleted, Sent) " + 
                "VALUES (ID_PH, ID_PR, Price, Preferential, Deleting, 0); " + 
                "END IF; " + 
                "END");
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE Pharm66.ImportingInComparisonsOfProducts(IN ID INT, IN Name VARBINARY (255))" + 
                "BEGIN" + 
                "  IF" + 
                "    (EXISTS (SELECT * FROM Comparisons AS C WHERE (C.IDOfExternalProduct = ID)))" + 
                "  THEN" + 
                "    UPDATE" + 
                "      Comparisons AS C" + 
                "    SET" + 
                "      C.NameOfExternalProduct = Name" + 
                "    WHERE" + 
                "      (C.IDOfExternalProduct = ID);" + 
                "  ELSE" + 
                "    INSERT INTO Comparisons (IDOfExternalProduct, NameOfExternalProduct, IDOfProduct, ExistsInPriceList, Compare)" + 
                "    VALUES (ID, Name, 0 , 0, 0);" + 
                "  END IF;" + 
                "END");
            //
            TextsOfStoredProcedures.Add(
                "CREATE PROCEDURE Pharm66.ImportingInComparisonsOfExistsIDs(IN ID INT)" + 
                "BEGIN" + 
                "  IF" + 
                "    (EXISTS (SELECT * FROM Comparisons AS C WHERE (C.IDOfExternalProduct = ID)))" + 
                "  THEN" + 
                "    UPDATE" + 
                "      Comparisons AS C" + 
                "    SET" + 
                "      C.ExistsInPriceList = 1" + 
                "    WHERE" + 
                "      ((C.IDOfExternalProduct = ID) AND (C.Compare = 0));" + 
                "  ELSE" + 
                "    INSERT INTO Comparisons (IDOfExternalProduct, NameOfExternalProduct, IDOfProduct, ExistsInPriceList, Compare)" + 
                "    VALUES (ID, '', 0, 1, 0);" + 
                "  END IF;" + 
                "END");
            //
            //DROP INDEX Idx_product_Id_product_group ON Pharm66.Product;
            //DROP INDEX index1 ON Pharm66.Product;
            //
            // Creating Command Of Creating
            //
            MySqlCommand CommandOfCreating = new MySqlCommand("", ConnectionToBase);
            //
            // Creating Stored Procedures
            //
            try
            {
                //
                OpeningConnection(CommandOfCreating.Connection);
                //
                this.RecordingInLogFile("Starting Creating SP");
                //
                foreach (string CurrentTextOfCreating in TextsOfStoredProcedures)
                {
                    //
                    CommandOfCreating.CommandText = CurrentTextOfCreating;
                    //
                    try { CommandOfCreating.ExecuteNonQuery(); }
                    catch {  }
                    /*catch (Exception E)
                    { this.ReturningMessageAboutError("ERROR Ошибка при создании SP", E, true); }*/
                }
                //
                this.RecordingInLogFile("Stoping Creating SP");
                //
                ClosingConnection(CommandOfCreating.Connection);
                //
            }
            catch (Exception E) { this.ReturningMessageAboutError("ERROR Ошибка при создании SP", E, true); }
        }

        // Creating Compound ID
        private void CreatingCompoundID(string NameOfTable)
        {
            //
            // Creating Command Of Modification
            //
            MySqlCommand CommandOfModification = new MySqlCommand("", ConnectionToBase);
            CommandOfModification.CommandTimeout = 100000;
            //
            try
            {
                //
                OpeningConnection(CommandOfModification.Connection);
                //
                // Checking Sanction Of Modification 
                //
                bool SanctionOfModification = false;
                //
                CommandOfModification.CommandText = 
                    String.Format("SELECT COUNT(*) FROM Pharm66.{0} WHERE ID_Price_List IS NULL;", 
                    NameOfTable);
                //
                try
                {
                    if(Convert.ToInt32(CommandOfModification.ExecuteScalar()) == 0) 
                        SanctionOfModification = true;
                }
                catch (Exception E)
                {
                    this.RecordingInLogFile(
                      String.Format("ERROR Ошибка при проверки факта модификации ID Of {0}: {1}", 
                      NameOfTable, E.Message));
                }
                //
                // Modification Of IDs
                //
                if (SanctionOfModification)
                {
                    //
                    // !!!
                    //
                    if (NameOfTable == "Price_List")
                    { this.ReturningMessageOfInformation(
                        "Идет замена ключей, дождитесь появления окна программы.", true); }
                    //
                    // !!!
                    //
                    this.RecordingInLogFile(String.Format("Starting Creating ID Of {0}", NameOfTable));
                    //
                    // Removing Primary Key
                    //
                    CommandOfModification.CommandText = 
                        String.Format(
                        "ALTER TABLE Pharm66.{0} DROP PRIMARY KEY, CHANGE Id_Price_list Id_Price_list INT (10);", 
                        NameOfTable);
                    //
                    try { CommandOfModification.ExecuteNonQuery(); }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                          String.Format("ERROR Ошибка при удалении ID Of {0}: {1}", NameOfTable, E.Message));
                    }
                    //
                    // Creating Primary Key
                    //
                    CommandOfModification.CommandText = 
                        String.Format(
                        "ALTER TABLE Pharm66.{0} ADD PRIMARY KEY (Id_Product ASC, Id_Pharmacy ASC);",
                        NameOfTable);
                    //
                    try { CommandOfModification.ExecuteNonQuery(); }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                          String.Format("ERROR Ошибка при создании ID Of {0}: {1}", NameOfTable, E.Message));
                    }
                    //
                    // Modification Of Indexes (Price_Lists)
                    //
                    if (NameOfTable == "Price_List")
                    {
                        //
                        // Removing Indexes
                        //
                        CommandOfModification.CommandText = 
                            "DROP INDEX Idx_price_list_Id_Pharmacy ON Pharm66.price_list; " +
                            "DROP INDEX Idx_price_list_Id_Product ON Pharm66.price_list; " +
                            "DROP INDEX Idx_price_list_Pharmacy_Product ON Pharm66.price_list;";
                        //
                        try { CommandOfModification.ExecuteNonQuery(); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                                String.Format(
                                "ERROR Ошибка при удалении индексов {0}: {1}", NameOfTable, E.Message));
                        }
                        //
                        // Creating Index
                        //
                        CommandOfModification.CommandText =
                            "CREATE INDEX IndexOfPriceLists ON Pharm66.price_list (Id_Product ASC, Id_Pharmacy ASC);";
                        //
                        try { CommandOfModification.ExecuteNonQuery(); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                              String.Format("ERROR Ошибка при создании индекса {0}: {1}", NameOfTable, E.Message));
                        }
                    }
                    //
                    // Modification Of Indexes (exp_price_list)
                    //
                    if (NameOfTable == "exp_price_list")
                    {
                        //
                        // Removing Indexes
                        //
                        CommandOfModification.CommandText =
                            "DROP INDEX FK_pharmacy_product_product ON Pharm66.exp_price_list; " + 
                            "DROP INDEX FK_pharmacy_product_pharmacy ON Pharm66.exp_price_list;";
                        //
                        try { CommandOfModification.ExecuteNonQuery(); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                                String.Format(
                                "ERROR Ошибка при удалении индексов {0}: {1}", NameOfTable, E.Message));
                        }
                        //
                        // Creating Index
                        //
                        CommandOfModification.CommandText = 
                            "CREATE INDEX IndexOfPriceList ON Pharm66.exp_price_list (Id_Product ASC, Id_Pharmacy ASC);";
                        //
                        try { CommandOfModification.ExecuteNonQuery(); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                              String.Format("ERROR Ошибка при создании индекса {0}: {1}", NameOfTable, E.Message));
                        }
                    }
                    //
                    // Zeroing Of ID (Price_Lists)
                    //
                    if (NameOfTable == "Price_List")
                    {
                        //
                        // Updating Id_Price_list
                        //
                        CommandOfModification.CommandText = 
                            "UPDATE Pharm66.Price_List SET Id_Price_list = NULL WHERE Id_Product = 1;";
                        //
                        try { CommandOfModification.ExecuteNonQuery(); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                              String.Format("ERROR Ошибка при занулении ID Of {0}: {1}", NameOfTable, E.Message));
                        }
                    }
                    //
                    // Zeroing Of ID (exp_price_list)
                    //
                    if (NameOfTable == "exp_price_list")
                    {
                        //
                        // Updating exp_price_list
                        //
                        CommandOfModification.CommandText = 
                            "UPDATE Pharm66.exp_price_list SET Id_Price_list = NULL WHERE Id_Product = 60869;";
                        //
                        try { CommandOfModification.ExecuteNonQuery(); }
                        catch (Exception E)
                        {
                            this.RecordingInLogFile(
                              String.Format("ERROR Ошибка при занулении ID Of {0}: {1}", NameOfTable, E.Message));
                        }
                    }
                    //
                    // !!!
                    //
                    this.RecordingInLogFile(string.Format("Stoping Creating ID Of {0}", NameOfTable));
                }
                //
                ClosingConnection(CommandOfModification.Connection);
            }
            catch (Exception E) { this.ReturningMessageAboutError("ERROR Ошибка при создании IDs", E, true); }
        }

        /*
         * ALTER TABLE TestingPH.PL
  DROP PRIMARY KEY,
  CHANGE ID ID INT (10),
  ADD PRIMARY KEY (ID);
ALTER TABLE TestingPH.PL
  DROP PRIMARY KEY,
  CHANGE ID ID INT (10);
ALTER TABLE TestingPH.PL
  DROP PRIMARY KEY;
ALTER TABLE TestingPH.PL
  DROP COLUMN ID;
ALTER TABLE TestingPH.PL
  ADD PRIMARY KEY (ID_PR ASC, ID_DR ASC);
         * 
         * SELECT
  *
FROM
  Testingph.pl;
         * 
         * 
         * INSERT
INTO Testingph.pl (ID_PR, ID_DR)
VALUES (1, 1);
INSERT
INTO Testingph.pl (ID_PR, ID_DR)
VALUES (1, 2);
INSERT
INTO Testingph.pl (ID_PR, ID_DR)
VALUES (1, 3);
INSERT
INTO Testingph.pl (ID_PR, ID_DR)
VALUES (2, 1);
INSERT
INTO Testingph.pl (ID_PR, ID_DR)
VALUES (2, 2);
INSERT
INTO Testingph.pl (ID_PR, ID_DR)
VALUES (2, 3);
         * 
         * 

         * 
         * 
         * CREATE TABLE TestingPH.PL
  (
  ID INT(10) NOT NULL,
  ID_PR INT(10) NOT NULL,
  ID_DR INT(10) NOT NULL
  );
         * 
         * 
        */

        #endregion

        #region ' Management Of Connection '

        // Opening Connection
        private void OpeningConnection(MySqlConnection Connection)
        {
            //
            if (Connection != null)
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
        }

        // Closing Connection
        private void ClosingConnection(MySqlConnection Connection)
        {
            //
            if (Connection != null)
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
        }

        #endregion

    }
}