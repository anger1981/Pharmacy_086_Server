using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading;
using System.Collections;
using MySql.Data.MySqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ImportingPricesInPriceList : BaseType
    {

        #region ' Fields '

        //
        private string StringOfConnection;
        private MySqlConnection ConnectionToBase;
        //
        private MySqlCommand CommandOfSelection;
        private MySqlDataAdapter FillingWithData;
        //
        private PharmaceuticalInformation.Drugstore.ManagementOfComparisons ManagementOfComparisons;
        //
        //private string StringOfConnection = "";
        private ManagementOfPriceList.StatusOfPriceList _StatusOfImporting;
        private string _MessageOfConverting = "";
        private DataTable DataForImportingInPriceList;
        private bool SanctionOfImporting = true;
        private Queue StringsOfPrices;
        private int CountOfExecutedPackages = 0;
        //
        public delegate void ReturnOfEvent(string Str, int Count01, int Count02);
        public event ReturnOfEvent LoadingPriceListIsCompleted;


        #endregion

        #region ' Designer '

        public ImportingPricesInPriceList(
            string StringOfConnection, 
            PharmaceuticalInformation.Drugstore.ManagementOfComparisons ManagementOfComparisons)
            : this(StringOfConnection, ManagementOfComparisons, "")
        {
            //
        }

        public ImportingPricesInPriceList(
            string StringOfConnection, 
            PharmaceuticalInformation.Drugstore.ManagementOfComparisons ManagementOfComparisons, 
            string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            this.StringOfConnection = StringOfConnection;
            //
            // Creating Of Connection
            //
            try { ConnectionToBase = new MySqlConnection(this.StringOfConnection); }
            catch (Exception E)
            {
                throw new Exception(
                    String.Format("Ошибка при создании подключения редактирования Сопоставлений: {0}", E.Message));
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
                      String.Format("Ошибка при открытии подключения редактирования Сопоставлений: {0}", E.Message));
                }
            //
            // Initializing Fields
            //
            this.ManagementOfComparisons = ManagementOfComparisons;
            //
            _StatusOfImporting = ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList;
            DataForImportingInPriceList = new DataTable();
            StringsOfPrices = new Queue();
            //
            // Initializing Filling With Data
            //
            CommandOfSelection = new MySqlCommand("", ConnectionToBase);
            CommandOfSelection.CommandTimeout = 1000;
            FillingWithData = new MySqlDataAdapter(CommandOfSelection);
        }

        #endregion

        // Status Of Importing
        public ManagementOfPriceList.StatusOfPriceList StatusOfImporting
        {
            get { return _StatusOfImporting; }
            set { _StatusOfImporting = value; }
        }

        // Messages Of Converting
        public string MessageOfConverting
        {
            get { return MessageOfConverting; }
            set { _MessageOfConverting = value; }
        }

        // Getting Count For Importing
        public int GettingCountForImporting
        {
            get { return DataForImportingInPriceList.Rows.Count; }
        }

        // Reset Of Data For Importing In PriceList
        public void ResetDataForImportingInPriceList()
        {
            //
            DataForImportingInPriceList.Clear();
            //
            _StatusOfImporting = ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList;
        }

        // Importing Prices In PriceList
        public void AdditionOfPriceListForImporting(DataTable NewPrices)
        {
            //
            // Checking Of Table
            //
            bool CorrectTable = true;
            string[] ContainsColumns = new string[6] { "ID_PH", "ID_PR", "Price", "Deleting", "Preferential", "AllPrices" };
            foreach (string CurrentColumns in ContainsColumns)
                if (!NewPrices.Columns.Contains(CurrentColumns))
                    CorrectTable = false;
            //
            // Addition And Association Of PriceLists
            //
            if (CorrectTable && SanctionOfImporting)
            {
                //
                // Converting IDs Of Products
                //
                if (ManagementOfComparisons != null)
                {
                    //
                    ManagementOfComparisons.ConvertingIDsOfProducts(NewPrices, "ID_PR");
                    //
                    NewPrices.AcceptChanges();
                }
                /*
                //
                // Comparisons
                //
                try
                {
                    //
                    // Checking Presence Of Comparisons
                    //
                    MySqlCommand Executing = new MySqlCommand("SELECT COUNT(*) FROM Matching", ConnectionToBase);
                    ConnectionToBase.Open();
                    int CountOfComparisons = Convert.ToInt32(Executing.ExecuteScalar());
                    ConnectionToBase.Close();
                    //
                    if (CountOfComparisons > 0)
                    {
                        //
                        RecordingInLogFile("Usage Of Table Of Comparisons");
                        //
                        // Getting Comparisons
                        //
                        DataTable Comparisons = new DataTable("Comparisons");
                        MySqlCommand SelectionOfComparisons = new MySqlCommand(
                            "SELECT Id_client_product AS 'ID_PC', Id_product AS 'ID_PR' FROM Matching;",
                            ConnectionToBase);
                        MySqlDataAdapter GettingComparisons = new MySqlDataAdapter(SelectionOfComparisons);
                        FillingWithDataOfTable(Comparisons, "Comaprisons", GettingComparisons, true);
                        //
                        // Converting Of Codes
                        //
                        int CountOfConverting = 0, CountOfDeleting = 0;
                        //
                        if (Comparisons.Rows.Count > 0)
                        {
                            //
                            NewPrices.AcceptChanges();
                            //
                            Comparisons.PrimaryKey = new DataColumn[1] { Comparisons.Columns["ID_PC"] };
                            //
                            foreach (DataRow CurrentPrice in NewPrices.Rows)
                            {
                                //
                                DataRow FindRow = Comparisons.Rows.Find(CurrentPrice["ID_PR"]);
                                if (FindRow != null)
                                { CurrentPrice["ID_PR"] = FindRow["ID_PR"]; CountOfConverting++; }
                                else
                                { CurrentPrice.Delete(); CountOfDeleting++; }
                            }
                        }
                        //
                        int CountOfPrices = NewPrices.Rows.Count;
                        //
                        NewPrices.AcceptChanges();
                        //
                        RecordingInLogFile(
                            String.Format("Count Of Prices: {0} Count Of Converting: {1}, Count Of Deleting: {2}", 
                            CountOfPrices, CountOfConverting, CountOfDeleting));
                    }
                }
                catch (Exception E)
                {
                    //
                    ReturningMessageAboutError("Ошибка при конвертации кодов продуктов", E, true);
                    //
                    ClosingConnection(ConnectionToBase);
                }
                */
                //
                // Association Of PriceLists
                //
                if (DataForImportingInPriceList.Rows.Count == 0)
                    DataForImportingInPriceList = NewPrices.Clone();
                //
                foreach (DataRow CurrentPrice in NewPrices.Rows)
                { DataForImportingInPriceList.Rows.Add(CurrentPrice.ItemArray); }

            }
            else
            {
                //
                if (SanctionOfImporting == false)
                { RecordingInLogFile("ERROR Добавление невозможно идет импорт"); }
                //
                if (CorrectTable == false)
                { RecordingInLogFile("ERROR Error In table NewPrices there are no necessary columns"); }
            }
        }

        // Executed Threadings Of Updating Of PriceList
        public void ImportingPriceListInDataBase()
        {
            //
            // Importing
            //
            if (SanctionOfImporting)
            {
                try
                {
                    //
                    // Blocking Entering To ImportingPriceListInDataBase
                    //
                    SanctionOfImporting = false;
                    //
                    DataForImportingInPriceList.AcceptChanges();
                    //
                    // Clearing Of Repeatings Of Prices And Saving Maximal Values
                    //
                    int CountOfRepeatings = 0;
                    //
                    if ((DataForImportingInPriceList.Columns.Contains("ID_PH")) &&
                        (DataForImportingInPriceList.Columns.Contains("ID_PR")))
                    {
                        //
                        // Creating Table Of Unique Prices
                        //
                        DataTable ClearingPriceList = DataForImportingInPriceList.Clone();
                        ClearingPriceList.PrimaryKey =
                            new DataColumn[2]
                        {
                            ClearingPriceList.Columns["ID_PH"],
                            ClearingPriceList.Columns["ID_PR"]
                        };
                        //
                        // Filling Of Table Unique Prices And Definition Of Maximal
                        //
                        foreach (DataRow CurrentPrice in DataForImportingInPriceList.Rows)
                        {
                            //
                            DataRow FindPrice =
                                ClearingPriceList.Rows.Find(
                                new object[2] { CurrentPrice["ID_PH"], CurrentPrice["ID_PR"] });
                            //
                            if (FindPrice == null)
                            {
                                //
                                ClearingPriceList.Rows.Add(CurrentPrice.ItemArray);
                            }
                            else
                            {
                                //
                                // Deleting
                                //
                                if (((bool)CurrentPrice["Deleting"]) == true)
                                    FindPrice["Deleting"] = true;
                                //
                                // Price
                                //
                                try
                                {
                                    decimal NewValue = ((decimal)CurrentPrice["Price"]);
                                    decimal CurrentValue = ((decimal)FindPrice["Price"]);
                                    //
                                    // > <
                                    if (NewValue < CurrentValue)
                                        FindPrice["Price"] = NewValue;
                                    //
                                    if (((bool)CurrentPrice["Deleting"]) == false)
                                        CountOfRepeatings++;
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Error Of Updating Of Price Of Clearing ({0})",
                                        E.Message));
                                }
                            }
                        }
                        //
                        DataForImportingInPriceList = ClearingPriceList;
                    }
                    //
                    // Recording Count Of Rows For Importing
                    //
                    this.RecordingInLogFile(
                        String.Format("Count Of Rows For Importing: {0}", DataForImportingInPriceList.Rows.Count));
                    this.RecordingInLogFile("");
                    //
                    // Creating Packages Of Prices
                    //
                    int SizeOfPackage = 0;
                    //
                    StringsOfPrices.Clear();
                    //
                    if (DataForImportingInPriceList.Rows.Count <= 3000)
                        SizeOfPackage = 300;
                    else if (DataForImportingInPriceList.Rows.Count <= 5000)
                        SizeOfPackage = 500;
                    else if (DataForImportingInPriceList.Rows.Count <= 10000)
                        SizeOfPackage = 1000;
                    else if (DataForImportingInPriceList.Rows.Count > 10000)
                        SizeOfPackage = 2000;
                    //
                    // !!!
                    //
                    string NewStringOfPrices = "";
                    int CountPricesInCurrentPacket = 0;
                    //
                    // !!!
                    //
                    if (_StatusOfImporting == ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                    {
                        NewStringOfPrices =
                            "INSERT INTO exp_price_list(Id_Pharmacy, Id_Product, Price, Is_privilege, Is_deleted, Sent) VALUES ";
                    }
                    //
                    for (int i = 0; i < DataForImportingInPriceList.Rows.Count; i++)
                    {
                        //
                        if (CountPricesInCurrentPacket < SizeOfPackage)
                        {
                            //
                            // !!!
                            //
                            if (_StatusOfImporting == ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                            {
                                //
                                NewStringOfPrices +=
                                    String.Format(
                                    "({0}, {1}, {2}, {3}, {4}, 0)",
                                    DataForImportingInPriceList.Rows[i]["ID_PH"],
                                    DataForImportingInPriceList.Rows[i]["ID_PR"],
                                    DataForImportingInPriceList.Rows[i]["Price"].ToString().Replace(",", "."),
                                    (((bool)DataForImportingInPriceList.Rows[i]["Preferential"]) == true) ? "1" : "0",
                                    (((bool)DataForImportingInPriceList.Rows[i]["Deleting"]) == true) ? "1" : "0");
                                //
                                if ((CountPricesInCurrentPacket < (SizeOfPackage - 1)) &&
                                    (i != (DataForImportingInPriceList.Rows.Count - 1)))
                                    NewStringOfPrices += ", ";
                            }
                            else
                            {
                                //
                                NewStringOfPrices +=
                                    String.Format(
                                    "CALL ImportingInPriceList({0}, {1}, {2}, {3}, {4}); ",
                                    DataForImportingInPriceList.Rows[i]["ID_PH"],
                                    DataForImportingInPriceList.Rows[i]["ID_PR"],
                                    DataForImportingInPriceList.Rows[i]["Price"].ToString().Replace(",", "."),
                                    (((bool)DataForImportingInPriceList.Rows[i]["Preferential"]) == true) ? "1" : "0",
                                    (((bool)DataForImportingInPriceList.Rows[i]["Deleting"]) == true) ? "1" : "0");
                            }
                            //
                            // !!!
                            //
                            CountPricesInCurrentPacket++;
                        }
                        //
                        // !!!
                        //
                        if ((CountPricesInCurrentPacket == SizeOfPackage) ||
                            (i == (DataForImportingInPriceList.Rows.Count - 1)))
                        {
                            //
                            StringsOfPrices.Enqueue(NewStringOfPrices);
                            //
                            NewStringOfPrices = "";
                            if (_StatusOfImporting == ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                            {
                                NewStringOfPrices =
                                    "INSERT INTO exp_price_list(Id_Pharmacy, Id_Product, Price, Is_privilege, Is_deleted, Sent) VALUES ";
                            }
                            //
                            CountPricesInCurrentPacket = 0;
                            //
                            Thread.Sleep(10);
                        }
                    }
                    //
                    // Clearing Of Price List
                    //
                    if (DataForImportingInPriceList.Rows.Count > 0)
                    {
                        //
                        // !!! ???
                        //
                        MySqlCommand CommandOfClearing =
                                new MySqlCommand("", new MySqlConnection(StringOfConnection));
                        CommandOfClearing.CommandTimeout = 1000;
                        //
                        if (_StatusOfImporting == ManagementOfPriceList.StatusOfPriceList.FullPriceList)
                        { CommandOfClearing.CommandText = "DELETE FROM exp_price_list;"; }
                        else
                        { CommandOfClearing.CommandText = "DELETE FROM exp_price_list WHERE (Is_deleted = 1);"; }
                        //
                        // !!!
                        //
                        try
                        {
                            //
                            OpeningConnection(CommandOfClearing.Connection);
                            //
                            try
                            { CommandOfClearing.ExecuteScalar(); }
                            catch (Exception E)
                            {
                                //
                                //RecordingInLogFile(String.Format("ERROR Ошибка при зачистке Прайс-Листа: {0}", E.Message));
                                ReturningMessageAboutError("Ошибка при зачистке Прайс-Листа", E, true);
                                //
                                ClosingConnection(ConnectionToBase);
                            }
                            //
                            ClosingConnection(CommandOfClearing.Connection);
                        }
                        catch (Exception E)
                        {
                            //
                            RecordingInLogFile(
                              String.Format("ERROR Ошибка при Открытии/Закрытии подключения: {0}", E.Message));
                            //
                            ClosingConnection(ConnectionToBase);
                        }
                    }
                    //
                    // Starting Threads Of Importing
                    //
                    /*RecordingInLogFile(
                        String.Format("Starting Importing PriceList In DataBase ({0})", DataForImportingInPriceList.Rows.Count));*/
                    //
                    int CountOfStartingThreads = StringsOfPrices.Count;
                    //
                    CountOfExecutedPackages = 0;
                    //
                    for (int i = 0; i < CountOfStartingThreads; i++)
                    {
                        //
                        new System.Threading.Thread(
                            new System.Threading.ThreadStart(ImportingPricesInPriceList2)).Start();
                        //
                        //RecordingInLogFile(String.Format("Thread ({0})", i));
                        //
                        if (SizeOfPackage == 300)
                            System.Threading.Thread.Sleep(108);
                        else if (SizeOfPackage == 500)
                            System.Threading.Thread.Sleep(108);
                        else if (SizeOfPackage == 1000)
                            System.Threading.Thread.Sleep(1000);
                        else if (SizeOfPackage == 2000)
                            System.Threading.Thread.Sleep(1000);
                        //
                        // If in Queue big 10 Packet
                        //
                        if (CountOfStartingThreads > 10)
                        {
                            //
                            // Waiting Of Executing
                            //
                            while (!(((i + 1) - CountOfExecutedPackages) < 10))
                            { System.Threading.Thread.Sleep(500); }
                        }
                    }
                    //
                    // Waiting Of End
                    //
                    while (!(CountOfStartingThreads == CountOfExecutedPackages))
                    { System.Threading.Thread.Sleep(500); }
                    //
                    //RecordingInLogFile("Stoping Importing PriceList In DataBase");
                    //
                    LoadingPriceListIsCompleted(_MessageOfConverting, DataForImportingInPriceList.Rows.Count, CountOfRepeatings);
                    //
                    _MessageOfConverting = "";
                    //
                    ResetDataForImportingInPriceList();
                    //
                    // Refresh Data Of PriceList
                    //
                    //RefreshData();
                    //
                    // UnBlocking Entering To ImportingPriceListInDataBase
                    //
                    SanctionOfImporting = true;
                }
                catch (Exception E)
                {
                    //
                    //RecordingInLogFile(String.Format("ERROR Ошибка в методе ImportingPriceListInDataBase: {0}", E.Message));
                    ReturningMessageAboutError("ERROR Ошибка в методе ImportingPriceListInDataBase", E, true);
                    //
                    ResetDataForImportingInPriceList();
                    //
                    LoadingPriceListIsCompleted(_MessageOfConverting, 0, 0);
                    //
                    _MessageOfConverting = "";
                    //
                    // Refresh Data Of PriceList
                    //
                    //RefreshData();
                    //
                    // UnBlocking Entering To ImportingPriceListInDataBase
                    //
                    SanctionOfImporting = true;
                }
            }
            else
            {
                //
                if (SanctionOfImporting == false)
                { RecordingInLogFile(("ERROR Вход в метод ImportingPriceListInDataBase не разрешен")); }
            }
        }

        // Getting Of Executing Importing
        public bool GettingOfExecutingImporting
        {
            get
            {
                return false;
            }
        }

        // Getting PriceList For Importing
        public DataTable GettingPriceListForImporting()
        {
            //
            // Creating Copy PriceList
            //
            DataTable PriceListForImporting = new DataTable("PriceList");
            //
            PriceListForImporting = PriceListForImporting.Copy();
            //
            // Addition Of Column AllPrices
            //
            if (!PriceListForImporting.Columns.Contains("AllPrices"))
            {
                //
                PriceListForImporting.Columns.Add("AllPrices", typeof(bool));
                PriceListForImporting.Columns["AllPrices"].AllowDBNull = true;
                //
                ManagementOfPriceList.StatusOfPriceList CurrentStatus = StatusOfImporting;
                //
                foreach (DataRow CurrentPrice in PriceListForImporting.Rows)
                    CurrentPrice["AllPrices"] = 
                        (CurrentStatus == ManagementOfPriceList.StatusOfPriceList.FullPriceList) ? true : false;
            }
            //
            // Return
            //
            return PriceListForImporting;
        }

        // Can Reading PriceList From DataBase
        public bool CanReadingPriceListFromDataBase
        {
            get
            {
                //
                // Creating Text Of Checking
                //
                string TextOfChecking = "SELECT * FROM Service WHERE (1=2);";
                //
                // Getting Count Of Columns
                //
                DataTable CountOfColumns = new DataTable("CountOfColumns");
                //
                CountOfColumns = FillingWithDataOfTable(TextOfChecking, "CountOfColumns", true);
                //
                // Checking
                //
                bool ResultOfChecking = false;
                //
                if (CountOfColumns.Columns.Count > 0)
                    ResultOfChecking = true;
                //
                // Return
                //
                return ResultOfChecking;
            }
        }

        // Getting Status Of PriceList
        public ManagementOfPriceList.StatusOfPriceList GettingStatusOfPriceList
        {
            get
            {
                //
                // Reading Status Of PriceList
                //
                ManagementOfPriceList.StatusOfPriceList CurrentStatus = 
                    ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList;
                //
                MySqlCommand GettingStatus = new MySqlCommand(
                    "SELECT Value FROM service WHERE Id_Service = 6;", ConnectionToBase);
                try
                {
                    //
                    bool IsOpen = false;
                    if (GettingStatus.Connection.State == ConnectionState.Open)
                        IsOpen = true;
                    //
                    if (!IsOpen)
                        OpeningConnection(GettingStatus.Connection);
                    //
                    int ValueOfStatus = (int)GettingStatus.ExecuteScalar();
                    //
                    if (ValueOfStatus == 1)
                        CurrentStatus = ManagementOfPriceList.StatusOfPriceList.FullPriceList;
                    else
                        CurrentStatus = ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList;
                    //
                    if (!IsOpen)
                        ClosingConnection(GettingStatus.Connection);
                }
                catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении статуса Прайс-Листа", E, false); }
                finally { ClosingConnection(GettingStatus.Connection); }
                //
                // Return
                //
                return CurrentStatus;
            }
        }

        // Getting PriceList From DataBase
        public DataTable GettingPriceListFromDataBase()
        {
            //
            DataTable PriceListFromDataBase = new DataTable("PriceList");
            //
            // Creating Text Of Command Of Selection
            //
            string TextOfCommandOfFilling = 
                "SELECT EPL.Id_Pharmacy AS 'ID_PH', EPL.Id_Product AS 'ID_PR', EPL.Price AS 'Price', " + 
                "EPL.Is_deleted AS 'Deleting', EPL.Is_privilege AS 'Preferential' " + 
                "FROM exp_price_list AS EPL;";
            //
            // Filling
            //
            PriceListFromDataBase = FillingWithDataOfTable(TextOfCommandOfFilling, "PriceList", true);
            //
            // Addition Of Column AllPrices
            //
            PriceListFromDataBase.Columns.Add("AllPrices", typeof(bool));
            PriceListFromDataBase.Columns["AllPrices"].AllowDBNull = true;
            //
            ManagementOfPriceList.StatusOfPriceList CurrentStatus = GettingStatusOfPriceList;
            //
            foreach (DataRow CurrentPrice in PriceListFromDataBase.Rows)
                CurrentPrice["AllPrices"] = 
                    (CurrentStatus == ManagementOfPriceList.StatusOfPriceList.FullPriceList) ? true : false;
            //
            // Return
            //
            return PriceListFromDataBase;
        }

        // Getting Dates Of Transfer
        public DataTable GettingDatesOfTransfer()
        {
            //
            DataTable DatesOfTransfer = new DataTable("DatesOfTransfer");
            //
            // Creating Text Of Command Of Selection
            //
            string TextOfCommandOfFilling = 
                "SELECT S.Id_Service AS 'ID', S.Name_short AS 'Name', S.Value AS 'Value', S.Date_Service AS 'Date' " + 
                "FROM Service AS S " + 
                "ORDER BY S.Id_Service;";
            //
            // Filling
            //
            DatesOfTransfer = FillingWithDataOfTable(TextOfCommandOfFilling, "DatesOfTransfer", true);
            //
            // Return
            //
            return DatesOfTransfer;
        }

        // Importing Prices In PriceList
        private void ImportingPricesInPriceList2()
        {
            //
            // Importing Strings Of Prices
            //
            MySqlCommand CommandOfImporting =
                new MySqlCommand("", new MySqlConnection(StringOfConnection));
            CommandOfImporting.CommandTimeout = 100000;
            CommandOfImporting.CommandText = (string)StringsOfPrices.Dequeue();
            //
            try
            {
                //
                //int KeyOfThread = new Random().Next(1, 100);
                //
                OpeningConnection(CommandOfImporting.Connection);
                //
                //RecordingInLogFile(String.Format("Starting Executing: {0} {1}", KeyOfThread, CommandOfImporting.CommandText.Length));
                //
                try
                { CommandOfImporting.ExecuteScalar(); }
                catch (Exception E)
                {
                    //
                    RecordingInLogFile(String.Format("ERROR Ошибка при импортировании цен Прайс-Листа: {0}", E.Message));
                    //
                    // !!!
                    //
                    if (_StatusOfImporting == ManagementOfPriceList.StatusOfPriceList.SupplementingPriceList)
                    {
                        string StringOfPrices = CommandOfImporting.CommandText;
                        string[] Prices = new string[0];
                        Prices = StringOfPrices.Split(';');
                        //
                        RecordingInLogFile(
                            String.Format(
                            "Length Of String: {0} Count Of Prices Of String: {1}",
                            StringOfPrices.Length, Prices.Length));
                        //
                        foreach (string CurrentPrice in Prices)
                            if (CurrentPrice.Length > 1)
                            {
                                //
                                CommandOfImporting.CommandText = CurrentPrice;
                                try { CommandOfImporting.ExecuteScalar(); }
                                catch (Exception E2)
                                {
                                    RecordingInLogFile(
                                      String.Format("ERROR Ошибка при импортировании цены Прайс-Листа {0}: {1} {2}",
                                      CurrentPrice, E2.Message, CommandOfImporting.Connection.State.ToString()));
                                }
                            }
                    }
                }
                //
                //RecordingInLogFile(String.Format("Stoping Executing: {0}", KeyOfThread));
                //
                ClosingConnection(CommandOfImporting.Connection);
            }
            catch (Exception E)
            { RecordingInLogFile(String.Format("ERROR Ошибка при Открытии/Закрытии подключения: {0}", E.Message)); }
            //
            // !!!
            //
            CountOfExecutedPackages++;
        }

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

        #region ' Services '

        // Filling With Data Of Table
        private DataTable FillingWithDataOfTable(string TextOfCommandOfFilling, string NameOfTable, bool CreatingSchema)
        {
            //
            // Creating Return Table
            //
            DataTable FillingTable = new DataTable(NameOfTable);
            //
            // Text Of Command Of Filling
            //
            CommandOfSelection.CommandText = TextOfCommandOfFilling;
            //CommandOfSelection.CommandTimeout = 0;
            //
            // Filling With Data
            //
            try
            {
                if (CreatingSchema)
                    FillingWithData.FillSchema(FillingTable, SchemaType.Source);
                FillingWithData.Fill(FillingTable);
            }
            catch (Exception E)
            { this.ReturningMessageAboutError(String.Format("Ошибка при заполнении таблицы: {0}", NameOfTable), E, true); }
            //
            // Return
            //
            return FillingTable;
        }

        #endregion


        #region ' Archive '

        /*
        private string StringOfConnection;
        private MySqlConnection ConnectionToBase;
        //
        // Data Of PriceList
        //
        private DataTable PriceList;
        //
        // !!!
        //
        //private ArrayList UpdatingOfRows;
        private Queue UpdatingOfRows2;
        private ArrayList ThreadingOfUpdating;
        //private string NameOfEndThread = "";
        private int CountOfEndThread = 0;
        private int _SizeOfPackageOfUpdating = 0;
        //
        public delegate void ReturnOfEvent();
        public event ReturnOfEvent LoadingPriceListIsCompleted;
        */

        /*
        //
            PriceList = new DataTable("PriceList");
            ThreadingOfUpdating = new ArrayList();
            UpdatingOfRows2 = new Queue();
            _SizeOfPackageOfUpdating = 500;
        */

        /*
        // Creating Updating
        private MySqlDataAdapter CreatingUpdating()
        {
            //
            MySqlConnection NewConnectionToBase = new MySqlConnection(StringOfConnection);
            //
            // Initialize SqlDataAdapter
            //
            MySqlDataAdapter NewDataAdapter = new MySqlDataAdapter();
            //
            // Creating Command Of Selection
            //
            MySqlCommand CommandOfSelection = new MySqlCommand(
                "SELECT PL.Id_Pharmacy AS 'ID_PH', PL.Id_Product AS 'ID_PR', PL.Price AS 'Price', " +
                "PL.Is_deleted AS 'Deleting', PL.Is_privilege AS 'Preferential', PL.Sent AS 'Sent' " +
                "FROM exp_price_list AS PL;",
                NewConnectionToBase);
            //
            /*  "SELECT PL.Id_Pharmacy AS 'ID_PH', PL.Id_Product AS 'ID_PR', P.Name_full AS 'Name', PL.Price AS 'Price', " +
                "PL.Is_deleted AS 'Deleting', PL.Is_privilege AS 'Preferential', PL.Is_all_price AS 'AllPrices', PL.Sent AS 'Sent' " +
                "FROM exp_price_list AS PL LEFT JOIN Product AS P " +
                "ON (PL.Id_Product = P.Id_Product);"8/
            //
            // Creating Command Of Inserting
            //
            MySqlCommand CommandOfInserting = new MySqlCommand(
                "INSERT INTO exp_price_list(Id_Pharmacy, Id_Product, Price, Is_deleted, Is_privilege, Sent) " +
                "VALUES(@P1, @P2, @P3, @P4, @P5, @P6); ", NewConnectionToBase);
            //
            // Creating And Addition Of Parameters
            //
            object[,] ParametersOfInserting = new object[6, 3] { 
            { "@P1", "ID_PH" ,       MySqlDbType.UInt32 },  { "@P2", "ID_PR",     MySqlDbType.UInt32 }, 
            { "@P3", "Price",        MySqlDbType.Decimal }, { "@P4", "Deleting",  MySqlDbType.Bit }, 
            { "@P5", "Preferential", MySqlDbType.Bit },     { "@P6", "Sent",      MySqlDbType.Bit } };
            for (int i = 0; i <= ParametersOfInserting.GetUpperBound(0); i++)
            {
                MySqlParameter NewParameter = new MySqlParameter();
                NewParameter.ParameterName = ParametersOfInserting[i, 0].ToString();
                NewParameter.SourceColumn = ParametersOfInserting[i, 1].ToString();
                NewParameter.MySqlDbType = (MySqlDbType)ParametersOfInserting[i, 2];
                //NewParameter.Size = 0;
                CommandOfInserting.Parameters.Add(NewParameter);
            }
            //
            // Creating Command Of Updating
            //
            MySqlCommand CommandOfUpdating = new MySqlCommand(
                "UPDATE exp_price_list " +
                "SET Price = @P3, Is_deleted = @P4, Is_privilege = @P5, Sent = @P6 " +
                "WHERE ((Id_Pharmacy = @P1) AND (Id_Product = @P2))", NewConnectionToBase);
            //
            // Creating And Addition Of Parameters
            //
            object[,] ParametersOfUpdating = new object[6, 3] { 
            { "@P1", "ID_PH",        MySqlDbType.UInt32 }, { "@P2", "ID_PR",     MySqlDbType.UInt32 }, 
            { "@P3", "Price",        MySqlDbType.Decimal}, { "@P4", "Deleting",  MySqlDbType.Bit }, 
            { "@P5", "Preferential", MySqlDbType.Bit },    { "@P6", "Sent",      MySqlDbType.Bit } };
            for (int i = 0; i <= ParametersOfUpdating.GetUpperBound(0); i++)
            {
                MySqlParameter NewParameter = new MySqlParameter();
                NewParameter.ParameterName = ParametersOfUpdating[i, 0].ToString();
                NewParameter.SourceColumn = ParametersOfUpdating[i, 1].ToString();
                NewParameter.MySqlDbType = (MySqlDbType)ParametersOfUpdating[i, 2];
                //NewParameter.Size = 0;
                CommandOfUpdating.Parameters.Add(NewParameter);
            }
            //
            // Creating Command Of Deleting
            //
            MySqlCommand CommandOfDeleting = new MySqlCommand(
                "DELETE FROM exp_price_list " +
                "WHERE ((Id_Pharmacy = @P1) AND (Id_Product = @P2))", NewConnectionToBase);
            //
            // Creating And Addition Of Parameters
            //
            object[,] ParametersOfDeleting = new object[2, 3] { 
            { "@P1", "ID_PH",        MySqlDbType.UInt32 }, { "@P2", "ID_PR",     MySqlDbType.UInt32 }};
            for (int i = 0; i <= ParametersOfDeleting.GetUpperBound(0); i++)
            {
                MySqlParameter NewParameter = new MySqlParameter();
                NewParameter.ParameterName = ParametersOfDeleting[i, 0].ToString();
                NewParameter.SourceColumn = ParametersOfDeleting[i, 1].ToString();
                NewParameter.MySqlDbType = (MySqlDbType)ParametersOfDeleting[i, 2];
                //NewParameter.Size = 0;
                CommandOfDeleting.Parameters.Add(NewParameter);
            }
            /*
            //
            // Mapping
            //
            System.Data.Common.DataTableMapping Mapping = 
               new System.Data.Common.DataTableMapping("exp_price_list", "Prices");
            Mapping.ColumnMappings.Add("Id_Pharmacy", "ID_PH");
            Mapping.ColumnMappings.Add("Id_Product", "ID_PR");
            Mapping.ColumnMappings.Add("Price", "Price");
            Mapping.ColumnMappings.Add("Is_deleted", "Deleting");
            Mapping.ColumnMappings.Add("Is_privilege", "Preferential");
            Mapping.ColumnMappings.Add("Is_all_price", "AllPrices");
            //GettingData.TableMappings.Add(Mapping);
            ReturnPriceList.TableName = "Prices";
            8/
            //
            // Assignment Of Commands
            //
            NewDataAdapter.SelectCommand = CommandOfSelection;
            NewDataAdapter.InsertCommand = CommandOfInserting;
            NewDataAdapter.UpdateCommand = CommandOfUpdating;
            NewDataAdapter.DeleteCommand = CommandOfDeleting;
            //
            NewDataAdapter.ContinueUpdateOnError = true;
            //
            // Return
            //
            return NewDataAdapter;
        }

        // Importing Prices In PriceList
        public void ImportingPricesInPriceList(DataTable NewPrices)
        {
            //
            // Checking Of Table
            //
            bool CorrectTable = true;
            string[] ContainsColumns = new string[6] { "ID_PH", "ID_PR", "Price", "Deleting", "Preferential", "AllPrices" };
            foreach (string CurrentColumns in ContainsColumns)
                if (!NewPrices.Columns.Contains(CurrentColumns))
                    CorrectTable = false;
            //
            // Addition And Updating Prices In PriceList
            //
            if (CorrectTable)
            {
                //
                // Comparisons
                //
                try
                {
                    //
                    // Checking Presence Of Comparisons
                    //
                    MySqlCommand Executing = new MySqlCommand("SELECT COUNT(*) FROM Matching", ConnectionToBase);
                    ConnectionToBase.Open();
                    int CountOfComparisons = Convert.ToInt32(Executing.ExecuteScalar());
                    ConnectionToBase.Close();
                    //
                    if (CountOfComparisons > 0)
                    {
                        //
                        RecordingInLogFile("Usage Of Table Of Comparisons");
                        //
                        // Getting Comparisons
                        //
                        DataTable Comparisons = new DataTable("Comparisons");
                        MySqlCommand SelectionOfComparisons = new MySqlCommand(
                            "SELECT Id_client_product AS 'ID_PC', Id_product AS 'ID_PR' FROM Matching;",
                            ConnectionToBase);
                        MySqlDataAdapter GettingComparisons = new MySqlDataAdapter(SelectionOfComparisons);
                        FillingWithDataOfTable(Comparisons, "Comaprisons", GettingComparisons, true);
                        //
                        // Converting Of Codes
                        //
                        int CountOfConverting = 0, CountOfDeleting = 0;
                        //
                        if (Comparisons.Rows.Count > 0)
                        {
                            //
                            NewPrices.AcceptChanges();
                            //
                            Comparisons.PrimaryKey = new DataColumn[1] { Comparisons.Columns["ID_PC"] };
                            //
                            foreach (DataRow CurrentPrice in NewPrices.Rows)
                            {
                                //
                                DataRow FindRow = Comparisons.Rows.Find(CurrentPrice["ID_PR"]);
                                if (FindRow != null)
                                { CurrentPrice["ID_PR"] = FindRow["ID_PR"]; CountOfConverting++; }
                                else
                                { CurrentPrice.Delete(); CountOfDeleting++; }
                            }
                        }
                        //
                        NewPrices.AcceptChanges();
                        //
                        RecordingInLogFile(
                            String.Format("Count Of Converting:{0}, Count Of Deleting: {1}",
                            CountOfConverting, CountOfDeleting));
                    }
                }
                catch (Exception E)
                { ReturningMessageAboutError("Ошибка при конвертации кодов продуктов", E, true); }
                //
                // Importing Price In PriceList
                //
                //DataTable Products = ReadingOfInformation.GettingProducts();
                //
                foreach (DataRow CurrentPrice in NewPrices.Rows)
                {
                    //
                    DataRow RowInTable = null;
                    //
                    // Exists Row
                    //
                    int IDOfDrugstore = (int)CurrentPrice["ID_PH"];
                    int IDOfProduct = (int)CurrentPrice["ID_PR"];
                    object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
                    RowInTable = PriceList.Rows.Find(KeyOfRow);
                    //
                    // Addition Or Updating Of Price
                    //
                    if (RowInTable == null)
                    {
                        //
                        DataRow NewRow = PriceList.NewRow();
                        NewRow["ID_PH"] = CurrentPrice["ID_PH"];
                        NewRow["ID_PR"] = CurrentPrice["ID_PR"];
                        //
                        /*
                        DataRow NameOfProduct = Products.Rows.Find((object)IDOfProduct);
                        if (NameOfProduct != null)
                            if (NameOfProduct["Name"].ToString() != "")
                                NewRow["Name"] = NameOfProduct["Name"];
                            else
                                NewRow["Name"] = "";
                        8/
                        //
                        NewRow["Price"] = CurrentPrice["Price"];
                        NewRow["Deleting"] = CurrentPrice["Deleting"];
                        NewRow["Preferential"] = CurrentPrice["Preferential"];
                        NewRow["Sent"] = false;
                        //
                        //NewRow["AllPrices"] = CurrentPrice["AllPrices"];
                        PriceList.Rows.Add(NewRow);
                        //throw new Exception();
                    }
                    else
                    {
                        RowInTable["ID_PH"] = CurrentPrice["ID_PH"];
                        RowInTable["ID_PR"] = CurrentPrice["ID_PR"];
                        RowInTable["Price"] = CurrentPrice["Price"];
                        RowInTable["Deleting"] = CurrentPrice["Deleting"];
                        //RowInTable["Preferential"] = CurrentPrice["Preferential"];
                        //RowInTable["AllPrices"] = CurrentPrice["AllPrices"];
                        RowInTable["Sent"] = false;
                    }
                    //
                }
                //
                RecordingInLogFile(
                    String.Format("Count Of Modifications Of Rows {0}", PriceList.GetChanges().Rows.Count));
                //
                // Updating
                //
                //ExecutedUpdatingOfPriceList();
                //ExecutedThreadingsOfUpdatingOfPriceList();
                //
                // Change Of Status Of PriceList
                //
                /*
                if ((bool)NewPrices.Rows[0]["AllPrices"])
                    CurrentStatus = StatusOfPriceList.FullPriceList;
                else
                    CurrentStatus = StatusOfPriceList.SupplementingPriceList;
                //
                ChangeOfStatusOfPriceList(CurrentStatus);
                8/
            }
        }

        // Size Of Package Of Updating
        public int SizeOfPackageOfUpdating
        {
            get { return _SizeOfPackageOfUpdating; }
            set { _SizeOfPackageOfUpdating = value; }
        }

        // Executed Threadings Of Updating Of PriceList
        public void ExecutedThreadingsOfUpdatingOfPriceList()
        {
            //
            if (ThreadingOfUpdating.Count == 0)
            {
                int CountOfPackage = 0;
                //ArrayList NewPackage = new ArrayList();
                DataTable PackagePriceList = PriceList.Clone();
                //
                // Creating Of Packages
                //
                for (int i = 0; i < PriceList.Rows.Count; i++)
                    if ((PriceList.Rows[i].RowState == DataRowState.Added) ||
                        (PriceList.Rows[i].RowState == DataRowState.Modified) ||
                        (PriceList.Rows[i].RowState == DataRowState.Deleted))
                    {
                        CountOfPackage++;
                        //NewPackage.Add(PriceList.Rows[i]);
                        if (PriceList.Rows[i].RowState != DataRowState.Deleted)
                        {
                            DataRowState Status = PriceList.Rows[i].RowState;
                            PackagePriceList.Rows.Add(PriceList.Rows[i].ItemArray);
                            PriceList.Rows[i].AcceptChanges();
                            //
                            if (PackagePriceList.Rows.Count > 0)
                            {
                                PackagePriceList.Rows[PackagePriceList.Rows.Count - 1].AcceptChanges();
                                //
                                if (Status == DataRowState.Added)
                                    PackagePriceList.Rows[PackagePriceList.Rows.Count - 1].SetAdded();
                                else
                                    PackagePriceList.Rows[PackagePriceList.Rows.Count - 1].SetModified();
                            }
                        }
                        if ((CountOfPackage == _SizeOfPackageOfUpdating) || (i == (PriceList.Rows.Count - 1)))
                        {
                            //
                            CountOfPackage = 0;
                            //
                            //if (NewPackage.Count > 0)
                            if (PackagePriceList.Rows.Count > 0)
                            {
                                //
                                UpdatingOfRows2.Enqueue(PackagePriceList.Copy());
                                //UpdatingOfRows2.Enqueue(((DataRow[])NewPackage.ToArray(typeof(DataRow))));
                            }
                            //UpdatingOfRows.Add(((DataRow[])NewPackage.ToArray(typeof(DataRow))));
                            //
                            //NewPackage.Clear();
                            PackagePriceList.Clear();
                        }
                    }
                //
                // Starting Of Processing Updating
                //
                //int CountOfStarting = UpdatingOfRows.Count;
                int CountOfStarting = UpdatingOfRows2.Count;
                //
                /*
                if (ConnectionToBase02.State != ConnectionState.Open)
                    ConnectionToBase02.Open();
                8/
                //
                for (int i = 0; i < CountOfStarting; i++)
                {
                    //
                    Thread NewThreadOfUpdating = new Thread(new ThreadStart(ExecutedUpdatingOfPriceList02));
                    NewThreadOfUpdating.Name = String.Format("Updating_{0}", i);
                    ThreadingOfUpdating.Add(NewThreadOfUpdating);
                    NewThreadOfUpdating.Start();
                    Thread.Sleep(18);
                    //
                }
            }
        }

        // Executed Updating Of PriceList
        private void ExecutedUpdatingOfPriceList02()
        {
            //if (UpdatingOfRows.Count > 0)
            if (UpdatingOfRows2.Count > 0)
            {
                //DataRow[] RowsForUpdating = (DataRow[]) UpdatingOfRows[0];
                //UpdatingOfRows.RemoveAt(0);
                //DataRow[] RowsForUpdating = (DataRow[])UpdatingOfRows2.Dequeue();
                DataTable RowsForUpdating = (DataTable)UpdatingOfRows2.Dequeue();
                //
                // Updating Of Data
                //
                //ConnectionToBase02
                MySqlDataAdapter UpdatingRows = CreatingUpdating();
                try { UpdatingRows.Update(RowsForUpdating); }
                catch (Exception E)
                { ReturningMessageAboutError("Ошибка при обновление таблицы PriceList Thred", E, false); }
                //
                // Checking Of End Thread
                //
                CountOfEndThread++;
                //
                /*int CountOfRunningUpdating = 0;
                foreach (Thread CurrentThred in ThreadingOfUpdating)
                    if (CurrentThred.ThreadState == ThreadState.Running)
                        CountOfRunningUpdating++;8/
                //if (CountOfRunningUpdating <= 2)
                //if(NameOfEndThread == Thread.CurrentThread.Name)
                //
                if (ThreadingOfUpdating.Count == CountOfEndThread) // ??? ???
                {
                    //
                    CountOfEndThread = 0;
                    ThreadingOfUpdating.Clear();
                    //ConnectionToBase02.Close();
                    //
                    LoadingPriceListIsCompleted();
                    //
                    Console.WriteLine("End {0}", ThreadingOfUpdating.Count);
                }
            }
        }

        // Filling With Data Of Table
        private void FillingWithDataOfTable(DataTable FilledTable, string TableName, MySqlDataAdapter GettingData, bool CreatingSchema)
        {
            // Creating Return Table
            //DataTable ReceivedData = new DataTable(TableName);
            //
            // Filling Of Data
            //
            try
            {
                if (CreatingSchema)
                    GettingData.FillSchema(FilledTable, SchemaType.Source);
                GettingData.Fill(FilledTable);
            }
            catch (Exception E)
            { ReturningMessageAboutError(String.Format("Ошибка при заполнении таблицы {0}", TableName), E, false); }
            // Return
            //return ReceivedData;
        }
        */

        #endregion

    }
}