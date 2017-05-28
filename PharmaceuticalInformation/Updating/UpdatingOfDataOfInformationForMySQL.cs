using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections;
using PharmaceuticalInformation.BaseTypes;
//
using System.IO;

namespace PharmaceuticalInformation.Updating
{
    public class UpdatingOfDataOfInformationForMySQL : UpdatingOfDataOfInformation
    {

        #region ' Fields '

        //
        private string StringOfConnection = "";

        #endregion

        #region ' Designer '

        public UpdatingOfDataOfInformationForMySQL(string StringOfConnection)
            : base(StringOfConnection)
        {
            //
            this.StringOfConnection = StringOfConnection;
            StringsOfPrices = new Queue();
        }

        public UpdatingOfDataOfInformationForMySQL(string StringOfConnection, string PathToLogFile)
            : base(StringOfConnection, PathToLogFile)
        {
            //
            this.StringOfConnection = StringOfConnection;
            StringsOfPrices = new Queue();
            //PacketsOfPriceLists = new Queue();
        }

        #endregion

        #region ' Creating '

        protected override DbConnection CreatingConnection(string StringOfConnection)
        {
            // Return
            return new MySqlConnection(StringOfConnection);
        }

        protected override DbCommand CreatingCommand(string TextOfCommand, DbParameter[] ParametersOfCommand)
        {
            //
            DbCommand CreatedCommand = new MySqlCommand(TextOfCommand, (MySqlConnection)ConnectionToBase);
            //
            for (int i = 0; i <= ParametersOfCommand.GetUpperBound(0); i++)
                CreatedCommand.Parameters.Add(ParametersOfCommand[i]);
            CreatedCommand.CommandTimeout = 1000;
            // Return
            return CreatedCommand;
        }

        protected override DbDataAdapter CreatingDataAdapter()
        {
            // Return
            return new MySqlDataAdapter();
        }

        #endregion

        #region ' Updating '

        // Updating Of Pharmacy
        public override void UpdatingOfPharmacy(DataTable DataForDrugstores)
        {
            if (CheckingOfData(DataForDrugstores))
            {
                //
                // Creating Importing In Drugstores
                //
                DbParameter[] ParametersOfImporting = new DbParameter[10] { 
                    new MySqlParameter("ID",        MySqlDbType.Int16), 
                    new MySqlParameter("ID_DI",     MySqlDbType.Int16), 
                    new MySqlParameter("Name",      MySqlDbType.VarChar), 
                    new MySqlParameter("Address",   MySqlDbType.VarChar), 
                    new MySqlParameter("Phone",     MySqlDbType.VarChar), 
                    new MySqlParameter("Mail",      MySqlDbType.VarChar), 
                    new MySqlParameter("Site",      MySqlDbType.VarChar), 
                    new MySqlParameter("Schedule",  MySqlDbType.VarChar), 
                    new MySqlParameter("Transport", MySqlDbType.VarChar), 
                    new MySqlParameter("Deleting",  MySqlDbType.Bit) };
                //
                MySqlCommand ImportingInDrugstores =
                    (MySqlCommand)CreatingCommand("pharm66.UpdatingDrugstores", ParametersOfImporting);
                ImportingInDrugstores.CommandType = CommandType.StoredProcedure;
                //
                // Importing Data In Drugstores
                //
                ImportingData("Updating Table Of Drugstores", ImportingInDrugstores, DataForDrugstores);
            }
        }

        // Updating Of Groups Of Products
        public override void UpdatingOfGroupsOfProducts(DataTable DataForGroupsOfProducts)
        {
            if (CheckingOfData(DataForGroupsOfProducts))
            {
                //
                // Creating Importing In GroupsOfProducts
                //
                DbParameter[] ParametersOfImporting = new DbParameter[4] { 
                    new MySqlParameter("ID",             MySqlDbType.Int16), 
                    new MySqlParameter("Name",           MySqlDbType.VarChar), 
                    new MySqlParameter("DateOfUpdating", MySqlDbType.DateTime), 
                    new MySqlParameter("Deleting",       MySqlDbType.Bit) };
                //
                MySqlCommand ImportingInGroupsOfProducts =
                    (MySqlCommand)CreatingCommand("pharm66.UpdatingGroupsOfProducts", ParametersOfImporting);
                ImportingInGroupsOfProducts.CommandType = CommandType.StoredProcedure;
                //
                // Importing Data In GroupsOfProducts
                //
                ImportingData("Updating Table Of GroupsOfProducts", ImportingInGroupsOfProducts, DataForGroupsOfProducts);
            }
        }

        // Updating Of Products
        public override void UpdatingOfProducts(DataTable DataForProducts)
        {
            if (CheckingOfData(DataForProducts))
            {
                //
                // Creating Importing In Products
                //
                DbParameter[] ParametersOfImporting = new DbParameter[7] { 
                    new MySqlParameter("ID",          MySqlDbType.Int16), 
                    new MySqlParameter("ID_PG",       MySqlDbType.Int16), 
                    new MySqlParameter("Name",        MySqlDbType.VarChar), 
                    new MySqlParameter("Composition", MySqlDbType.VarChar), 
                    new MySqlParameter("Description", MySqlDbType.VarChar), 
                    new MySqlParameter("Updating",    MySqlDbType.DateTime), 
                    new MySqlParameter("Deleting",    MySqlDbType.Bit) };
                //
                MySqlCommand ImportingInProducts =
                    (MySqlCommand)CreatingCommand("pharm66.UpdatingOfProducts", ParametersOfImporting);
                ImportingInProducts.CommandType = CommandType.StoredProcedure;
                //
                // Importing Data In Products
                //
                ImportingData("Updating Table Of Products", ImportingInProducts, DataForProducts);
                //
                // !!!
                //
                DateTime DateOfUpdating = DateTime.Now;
                if (DataForProducts.Rows.Count > 0)
                    DateOfUpdating = (DateTime)DataForProducts.Rows[0]["Updating"];
                //
                foreach (DataRow CurrentProduct in DataForProducts.Rows)
                    if (((DateTime)CurrentProduct["Updating"]) > DateOfUpdating)
                        DateOfUpdating = ((DateTime)CurrentProduct["Updating"]);
                // 5
                DbCommand CommandOfUpdating = CreatingCommand(
                    "UPDATE Service SET Value = 0, Date_Service = @P1 WHERE Id_Service = 5;",
                    new DbParameter[1] { new MySqlParameter("@P1", MySqlDbType.DateTime) });
                CommandOfUpdating.CommandTimeout = 1000;
                //
                try
                {
                    CommandOfUpdating.Parameters["@P1"].Value = DateOfUpdating;
                    //
                    OpeningConnection(CommandOfUpdating.Connection);
                    CommandOfUpdating.ExecuteScalar();
                    ClosingConnection(CommandOfUpdating.Connection);
                }
                catch (Exception E)
                {
                    ClosingConnection(CommandOfUpdating.Connection);
                    ReturningMessageAboutError("Ошибка при обновлении даты обновления Продуктов", E, false);
                }
                //
            }
        }

        // Updating Of PriceList ARH
        public void UpdatingOfPriceListARH(DataTable DataForPriceList)
        {
            if (CheckingOfData(DataForPriceList))
            {
                //
                // Creating Importing In PriceList
                //
                DbParameter[] ParametersOfImporting = new DbParameter[6] {
                    new MySqlParameter("ID_PH",        MySqlDbType.Int16), 
                    new MySqlParameter("ID_PR",        MySqlDbType.Int16), 
                    new MySqlParameter("Price",        MySqlDbType.Decimal), 
                    new MySqlParameter("Updating",     MySqlDbType.DateTime), 
                    new MySqlParameter("Preferential", MySqlDbType.Bit), 
                    new MySqlParameter("Deleting",     MySqlDbType.Bit) };
                //
                MySqlCommand ImportingInPriceList = 
                    (MySqlCommand)CreatingCommand("pharm66.UpdatingPriceList", ParametersOfImporting);
                ImportingInPriceList.CommandType = CommandType.StoredProcedure;
                //
                // Importing Data In PriceList
                //
                ImportingData("Updating Table Of PriceList", ImportingInPriceList, DataForPriceList);
            }
        }

        #region ' UpdatingOfPriceList ARH '

        private bool SanctionOfUpdating = true;
        private Queue StringsOfPrices;
        private int CountOfExecutedPackages = 0;

        // Updating Of PriceList
        public override void UpdatingOfPriceList(DataTable DataForPriceList)
        {
            //
            // Checking Data
            //
            bool CheckingData = false;
            //
            if (CheckingOfData(DataForPriceList))
                CheckingData = true;
            //
            // Updating
            //
            if (SanctionOfUpdating && CheckingData)
            {
                try
                {
                    //
                    // Blocking Entering To UpdatingOfPriceList
                    //
                    SanctionOfUpdating = false;
                    //
                    // Creating Packages Of Prices (Max 50)
                    //
                    ArrayList PackagesOfPrices = new ArrayList();
                    DataTable NewPackage = new DataTable("NewPackage");
                    int SizeOfPackage = 0;
                    //
                    if (DataForPriceList.Rows.Count < 5000)
                        SizeOfPackage = 100; // 50 Connections
                    else
                        SizeOfPackage = 500;
                    //
                    for (int i = 0; i < DataForPriceList.Rows.Count; i++)
                    {
                        //
                        if ((NewPackage.Rows.Count == 0) || (NewPackage.Rows.Count == SizeOfPackage))
                        {
                            NewPackage = DataForPriceList.Clone();
                            NewPackage.TableName = String.Format("Package{0}", PackagesOfPrices.Count + 1);
                        }
                        //
                        if (NewPackage.Rows.Count < SizeOfPackage)
                        { NewPackage.Rows.Add(DataForPriceList.Rows[i].ItemArray); }
                        //
                        if ((NewPackage.Rows.Count == SizeOfPackage) || (i == (DataForPriceList.Rows.Count - 1)))
                        {
                            if (PackagesOfPrices.Count <= 50)
                                PackagesOfPrices.Add(NewPackage);
                            else
                            {
                                RecordingInLogFile(
                                    String.Format(
                                    "Error Превышен размер очереди PackagesOfPrices ({0})", NewPackage.TableName));
                            }
                        }
                    }
                    //
                    // Generation Of Strings Of Prices
                    //
                    StringsOfPrices.Clear();
                    //
                    foreach (DataTable CurrentPackage in PackagesOfPrices)
                    {
                        //
                        string NewStringOfPrices = "";
                        //
                        foreach (DataRow CurrentPrice in CurrentPackage.Rows)
                        {
                            //
                            NewStringOfPrices +=
                                String.Format(
                                "CALL UpdatingPriceList({0}, {1}, {2}, '{3}-{4}-{5}', {6}, {7}); ", 
                                CurrentPrice["ID_PH"], 
                                CurrentPrice["ID_PR"], 
                                CurrentPrice["Price"].ToString().Replace(",", "."), 
                                ((DateTime)CurrentPrice["Updating"]).Year, 
                                ((DateTime)CurrentPrice["Updating"]).Month, 
                                ((DateTime)CurrentPrice["Updating"]).Day, 
                                (((bool)CurrentPrice["Preferential"]) == true) ? "1" : "0", 
                                (((bool)CurrentPrice["Deleting"]) == true) ? "1" : "0");
                        }
                        //
                        if(NewStringOfPrices != "")
                            StringsOfPrices.Enqueue(NewStringOfPrices);
                    }
                    //
                    // Starting Threads Of Updating
                    //
                    CountOfExecutedPackages = 0;
                    //
                    RecordingInLogFile(
                        String.Format("Starting Updating Table Of PriceList ({0})", DataForPriceList.Rows.Count));
                    //
                    int CountOfStartingThreads = StringsOfPrices.Count;
                    for (int i = 0; i < CountOfStartingThreads; i++)
                    {
                        //
                        new System.Threading.Thread(
                            new System.Threading.ThreadStart(ImportingPricesInPriceList)).Start();
                        //
                        if (SizeOfPackage == 500)
                            System.Threading.Thread.Sleep(3000);
                        else if (SizeOfPackage == 100)
                            System.Threading.Thread.Sleep(108);
                    }
                    //
                    // Waiting
                    //
                    while (!(CountOfStartingThreads == CountOfExecutedPackages))
                    { System.Threading.Thread.Sleep(500); }
                    //
                    RecordingInLogFile("Stoping Updating Table Of PriceList");
                    //
                    // UnBlocking Entering To UpdatingOfPriceList
                    //
                    SanctionOfUpdating = true;
                }
                catch (Exception E)
                {
                    //
                    RecordingInLogFile(String.Format("ERROR Ошибка в методе UpdatingOfPriceList: {0}", E.Message));
                    //
                    // UnBlocking Entering To UpdatingOfPriceList
                    //
                    SanctionOfUpdating = true;
                }
            }
            else
            {
                //
                if (CheckingData == false)
                { RecordingInLogFile(("ERROR Данные для метода UpdatingOfPriceList не прошли проверку")); }
                //
                if (SanctionOfUpdating == false)
                { RecordingInLogFile(("ERROR Вход в метод UpdatingOfPriceList не разрешен")); }
            }
        }

        // Importing Prices In PriceList
        private void ImportingPricesInPriceList()
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
                    RecordingInLogFile(String.Format("ERROR Ошибка при импортировании цен пакета: {0}", E.Message));
                    //
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
                                  String.Format("ERROR Ошибка при импортировании цены {0}: {1}", 
                                  CurrentPrice, E2.Message));
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

        #endregion

        #region ' Updating Of PriceLists '

        private bool SanctionOfImportingPriceLists = true;
        private int CountOfExecutingOfPackets = 0;
        private int CountOfExecutedPackagesPriceLists = 0;
        private string PriceListsOfPacket = "";

        // Updating Of PriceLists
        public override void UpdatingOfPriceLists(string PathToFileOfPriceLists, int NumberOfUpdatingOfPriceLists)
        {
            //
            // Importing Of PriceLists
            //
            if (SanctionOfImportingPriceLists)
            {
                try
                {
                    //
                    // Blocking Entering To ImportingPriceListInDataBase
                    //
                    SanctionOfImportingPriceLists = false;
                    //
                    RecordingInLogFile("Starting Importing PriceLists");
                    //
                    // Checking Of File Of PriceLists
                    //
                    RecordingInLogFile("Starting Checking Of File Of PriceLists");
                    //
                    bool CheckedFileOfPriceLists = false;
                    //
                    try
                    {
                        //
                        // Creating Streams Of Reading
                        //
                        FileStream FS = new FileStream(PathToFileOfPriceLists, FileMode.Open, FileAccess.Read);
                        StreamReader SR = new StreamReader(FS);
                        //
                        // Checking Of Reading
                        //
                        try
                        {
                            //
                            SR.ReadLine();
                            //
                            CheckedFileOfPriceLists = true;
                        }
                        catch (Exception E)
                        { RecordingInLogFile(String.Format("ERROR Ошибка при проверочном чтении файла обновления Прайс-листов: {0}", E.Message)); }
                        //
                        // Closing And Clearing Streams
                        //
                        try
                        {
                            //
                            SR.Close();
                            SR.Dispose();
                            //
                            //GC.SuppressFinalize(FS);
                            //
                            FS.Dispose();
                            //
                            System.Threading.Thread.Sleep(1108);
                            //
                            GC.Collect();
                        }
                        catch (Exception E)
                        {
                            //
                            RecordingInLogFile(
                                String.Format("ERROR Ошибка при закрытии проверочных потоков файла обновления Прайс-листов: {0}", E.Message));
                            //
                            CheckedFileOfPriceLists = false;
                        }
                    }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при создании проверочных потоков файла обновления Прайс-листов: {0}", E.Message)); }
                    //
                    RecordingInLogFile("Stoping Checking Of File Of PriceLists");
                    //
                    // Clearing Of PriceLists
                    //
                    if (CheckedFileOfPriceLists)
                    {
                        //
                        RecordingInLogFile("Starting Clearing Of PriceLists");
                        //
                        // Creating Command Of Clearing
                        //
                        MySqlCommand CommandOfClearing = 
                            new MySqlCommand("TRUNCATE TABLE Price_List;", new MySqlConnection(StringOfConnection));
                        CommandOfClearing.CommandTimeout = 1000;
                        //
                        // Clearing Of PriceLists
                        //
                        try
                        {
                            //
                            OpeningConnection(CommandOfClearing.Connection);
                            //
                            // Executing Of Clearing
                            //
                            try
                            { CommandOfClearing.ExecuteScalar(); }
                            catch (Exception E)
                            {
                                //
                                RecordingInLogFile(
                                    String.Format("ERROR Ошибка при очистке таблицы Прайс-Листов: {0}", E.Message));
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
                                String.Format(
                                "ERROR Ошибка при Открытии/Закрытии подключения очистки Прайс-Листов: {0}", E.Message));
                            //
                            ClosingConnection(ConnectionToBase);
                        }
                        //
                        RecordingInLogFile("Stoping Clearing Of PriceLists");
                    }
                    //
                    // Creating Packages Of PriceLists
                    //
                    if (CheckedFileOfPriceLists)
                    {
                        //
                        RecordingInLogFile("Starting Threads Of Importing Of Packages Of PriceLists");
                        //
                        // Initializing Importing
                        //
                        int SizeOfPackage = 2000;
                        int CountPricesInCurrentPacket = 0;
                        //
                        DataTable DatesOfPriceList = new DataTable("DatesOfPriceList");
                        DatesOfPriceList.Columns.Add("ID", typeof(int));
                        DatesOfPriceList.Columns.Add("Date", typeof(string));
                        //
                        // Initializing String Of Packet
                        //
                        StringBuilder NewStringOfPrices = new StringBuilder();
                        NewStringOfPrices.Append("INSERT INTO Price_List(Id_Pharmacy, Id_Product, Price, Date_upd, Is_privilege, Is_deleted) VALUES ");
                        //
                        // Reading Of PriceLists
                        //
                        try
                        {
                            //
                            // Creating Streams Of Reading
                            //
                            FileStream FS = new FileStream(PathToFileOfPriceLists, FileMode.Open, FileAccess.Read);
                            StreamReader SR = new StreamReader(FS);
                            //
                            // Creating And Starting Packets
                            //
                            string DateOfPriceListOfDrugstore = "";
                            //
                            try
                            {
                                while (SR.EndOfStream != true)
                                {
                                    //
                                    // Reading Of Data
                                    //
                                    string TextOfRowFromFile = SR.ReadLine();
                                    //
                                    // Addition Of Price In Packet
                                    //
                                    if (CountPricesInCurrentPacket < SizeOfPackage)
                                    {
                                        //
                                        // !!!
                                        //
                                        if (TextOfRowFromFile != null)
                                        {
                                            //
                                            if (TextOfRowFromFile.IndexOf(":") != -1)
                                            {
                                                //
                                                string[] ElementsOfDate = TextOfRowFromFile.Split(',');
                                                //
                                                if (ElementsOfDate.Length >= 2)
                                                {
                                                    //
                                                    DateOfPriceListOfDrugstore = ElementsOfDate[1];
                                                    //
                                                    DatesOfPriceList.Rows.Add(
                                                        Convert.ToInt32(ElementsOfDate[0]), 
                                                        ElementsOfDate[1]);
                                                }
                                            }
                                            else
                                            {
                                                //
                                                string[] ElementsOfPrice = TextOfRowFromFile.Split(
                                                    new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                //
                                                if (ElementsOfPrice.Length >= 4)
                                                {
                                                    NewStringOfPrices.Append(
                                                        String.Format("({0}, {1}, {2}, '{3}', {4}, 0)",
                                                        ElementsOfPrice[0], ElementsOfPrice[1], ElementsOfPrice[2],
                                                        DateOfPriceListOfDrugstore, ElementsOfPrice[3]));
                                                }
                                                //
                                                // Addition Of Separator
                                                //
                                                if ((CountPricesInCurrentPacket < (SizeOfPackage - 1)) &&
                                                    (SR.EndOfStream != true))
                                                    NewStringOfPrices.Append(", ");
                                                //
                                                // Increment Of Count Of Prices In Packet
                                                //
                                                CountPricesInCurrentPacket++;
                                            }
                                        }
                                    }
                                    //
                                    // Starting Packet
                                    //
                                    if ((CountPricesInCurrentPacket == SizeOfPackage) ||
                                        (SR.EndOfStream == true))
                                    {
                                        //
                                        // Moving Prices
                                        //
                                        PriceListsOfPacket = NewStringOfPrices.ToString();
                                        //
                                        // Starting Thread
                                        //
                                        new System.Threading.Thread(
                                            new System.Threading.ThreadStart(ImportingPricesInPriceLists)).Start();
                                        //
                                        // Increment Of Count Of Starting Threads
                                        //
                                        CountOfExecutingOfPackets++;
                                        //
                                        // Waiting
                                        //
                                        System.Threading.Thread.Sleep(1108);
                                        //
                                        // Waiting Of Executing (No More 10 Streams)
                                        //
                                        int CountOfWaiting = 0;
                                        //
                                        while (!((CountOfExecutingOfPackets - CountOfExecutedPackagesPriceLists) < 10))
                                        {
                                            //
                                            System.Threading.Thread.Sleep(500);
                                            //
                                            CountOfWaiting++;
                                            //
                                            if (CountOfWaiting == 5)
                                            {
                                                //
                                                CountOfWaiting = 0;
                                                //
                                                //RecordingInLogFile(String.Format("CountOfExecutingOfPackets: {0}", CountOfExecutingOfPackets));
                                                //
                                                RecordingInLogFile(String.Format("Excess Of Expectation: {0}", CountOfExecutingOfPackets));
                                            }
                                        }
                                        //
                                        // Initializing String Of Packet
                                        //
                                        //GC.SuppressFinalize(NewStringOfPrices);
                                        NewStringOfPrices = new StringBuilder();
                                        NewStringOfPrices.Append("INSERT INTO Price_List(Id_Pharmacy, Id_Product, Price, Date_upd, Is_privilege, Is_deleted) VALUES ");
                                        //
                                        CountPricesInCurrentPacket = 0;
                                        //
                                        GC.Collect();
                                    }
                                }
                            }
                            catch (Exception E)
                            { RecordingInLogFile(String.Format("ERROR Ошибка при чтении цен файла обновления Прайс-листов и создании пакетов обновления: {0}", E.Message)); }
                            //
                            // Closing And Clearing Streams
                            //
                            try
                            {
                                //
                                SR.Close();
                                SR.Dispose();
                                //
                                //GC.SuppressFinalize(FS);
                                //
                                FS.Dispose();
                                //
                                System.Threading.Thread.Sleep(1108);
                                //
                                GC.Collect();
                            }
                            catch (Exception E)
                            { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии потоков чтения файла обновления Прайс-листов: {0}", E.Message)); }
                        }
                        catch (Exception E)
                        { RecordingInLogFile(String.Format("ERROR Ошибка при создании потоков чтения файла обновления Прайс-листов: {0}", E.Message)); }
                        //
                        // Getting Maximum Date Of PriceLists
                        //
                        DateTime MaximumDateOfPriceLists = new DateTime(1947, 7, 2);
                        //
                        foreach (DataRow CurrentDate in DatesOfPriceList.Rows)
                        {
                            try
                            {
                                //
                                DateTime NewDate = Convert.ToDateTime(CurrentDate["Date"]);
                                //
                                if (NewDate > MaximumDateOfPriceLists)
                                    MaximumDateOfPriceLists = NewDate;
                            }
                            catch (Exception E)
                            {RecordingInLogFile(String.Format("Ошибка при конвертации даты Прайс-Листа: {0}", E.Message)); } 
                        }
                        //
                        // Updating Date Of Actual
                        //
                        UpdatingDateOfActual(MaximumDateOfPriceLists);
                        //
                        // Updating Of Dates Of PriceList
                        //
                        UpdatingOfDatesOfPriceList02(DatesOfPriceList);
                        //
                        // Updating Number Of Updating PriceLists
                        //
                        UpdatingNumberOfUpdatingPriceLists(NumberOfUpdatingOfPriceLists);
                    }
                    //
                    // Waiting Of End
                    //
                    while (!(CountOfExecutingOfPackets == CountOfExecutedPackagesPriceLists))
                    { System.Threading.Thread.Sleep(500); }
                    //
                    // UnBlocking Entering To ImportingPriceListInDataBase
                    //
                    SanctionOfImportingPriceLists = true;
                    //
                    RecordingInLogFile("Stoping Threads Of Importing Of Packages Of PriceLists");
                }
                catch (Exception E)
                {
                    //
                    RecordingInLogFile(
                        String.Format("ERROR Ошибка в методе UpdatingOfPriceLists: {0}", E.Message));
                    //
                    // UnBlocking Entering To ImportingPriceListInDataBase
                    //
                    SanctionOfImportingPriceLists = true;
                    //
                    RecordingInLogFile("Stoping Importing PriceLists");
                }
            }
            else
            {
                //
                if (SanctionOfImportingPriceLists == false)
                { RecordingInLogFile(("ERROR Вход в метод UpdatingOfPriceLists не разрешен")); }
            }
            //
            //RecordingInLogFile("");
        }

        // Importing Prices In PriceLists
        private void ImportingPricesInPriceLists()
        {
            //
            // Importing Strings Of Prices
            //
            MySqlCommand CommandOfImporting = 
                new MySqlCommand("", new MySqlConnection(StringOfConnection));
            CommandOfImporting.CommandTimeout = 100000;
            CommandOfImporting.CommandText = PriceListsOfPacket;
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
                { RecordingInLogFile(String.Format("ERROR Ошибка при импортировании цен пакета Прайс-Листов: {0}", E.Message)); }
                //
                //RecordingInLogFile(String.Format("Stoping Executing: {0}", KeyOfThread));
                //
                ClosingConnection(CommandOfImporting.Connection);
            }
            catch (Exception E)
            {
                RecordingInLogFile(
                    String.Format(
                    "ERROR Ошибка при Открытии/Закрытии подключения (ImportingPricesInPriceLists): {0}", E.Message));
            }
            //
            GC.Collect();
            //
            // Increment Of Count Of Executed Packages
            //
            CountOfExecutedPackagesPriceLists++;
        }

        #endregion

        // Updating Of Dates Of PriceList
        public override void UpdatingOfDatesOfPriceList(DataTable DataForUpdating)
        {
            if (CheckingOfData(DataForUpdating))
            {
                //
                // Creating Importing In DatesOfPriceList
                //
                DbParameter[] ParametersOfImporting = new DbParameter[2] { 
                    new MySqlParameter("ID",   MySqlDbType.Int16), 
                    new MySqlParameter("Date", MySqlDbType.DateTime)  };
                //
                MySqlCommand ImportingInDatesOfPriceList =
                    (MySqlCommand)CreatingCommand("pharm66.UpdatingDatesOfPriceList", ParametersOfImporting);
                ImportingInDatesOfPriceList.CommandType = CommandType.StoredProcedure;
                //
                // Importing Data In DatesOfPriceList
                //
                ImportingData("Updating Dates Of Table Of PriceList", ImportingInDatesOfPriceList, DataForUpdating);
            }
        }

        // Updating Of Dates Of PriceList02
        public override void UpdatingOfDatesOfPriceList02(DataTable DatesForUpdating)
        {
            //
            // Checking Data
            //
            bool CheckedData = true;
            //
            if ((!DatesForUpdating.Columns.Contains("ID")) || (!DatesForUpdating.Columns.Contains("Date")))
                CheckedData = false;
            //
            // Updating Dates Of Drugstores
            //
            if (CheckedData)
            {
                try
                {
                    //
                    // Creating Clearing Of Dates
                    //
                    MySqlCommand CommandOfClearingOfDates =
                        new MySqlCommand(
                            "UPDATE Pharmacy SET Date_upd = null;", new MySqlConnection(StringOfConnection));
                    CommandOfClearingOfDates.CommandTimeout = 1000;
                    //
                    // Clearing Of Dates Of Drugstore
                    //
                    try
                    {
                        //
                        OpeningConnection(CommandOfClearingOfDates.Connection);
                        //
                        CommandOfClearingOfDates.ExecuteScalar();
                        //
                        ClosingConnection(CommandOfClearingOfDates.Connection);
                    }
                    catch (Exception E)
                    {
                        //
                        ClosingConnection(CommandOfClearingOfDates.Connection);
                        //
                        RecordingInLogFile(String.Format("ERROR Ошибка при очистке аптечных даты актуальности: {0}", E.Message));
                    }
                    //
                    // Change Of Status Of Changes
                    //
                    DatesForUpdating.AcceptChanges();
                    //
                    for (int i = 0; i < DatesForUpdating.Rows.Count; i++)
                        DatesForUpdating.Rows[i].SetModified();
                    //
                    // Creating Command Of Updating Dates Of Drugstores
                    //
                    DbParameter[] ParametersOfUpdating = new DbParameter[2] { 
                        new MySqlParameter("ID",   MySqlDbType.Int16, 0, "ID"), 
                        new MySqlParameter("Date", MySqlDbType.String, 0, "Date") };
                    //
                    MySqlCommand UpdatingDatesOfDrugstores = 
                        (MySqlCommand)CreatingCommand(
                        "UPDATE Pharmacy SET Date_upd = @Date WHERE ID_Pharmacy = @ID;", ParametersOfUpdating);
                    //
                    MySqlDataAdapter UpdatingDates = new MySqlDataAdapter();
                    UpdatingDates.UpdateCommand = UpdatingDatesOfDrugstores;
                    UpdatingDates.ContinueUpdateOnError = true;
                    //
                    // Updating
                    //
                    UpdatingDates.Update(DatesForUpdating);
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка в методе UpdatingOfDatesOfPriceList02: {0}", E.Message)); }
            }
        }

        // Updating Of Published Of Announcements
        public override void UpdatingOfPublishedOfAnnouncements(DataTable DataForUpdating)
        {
            if (CheckingOfData(DataForUpdating))
            {
                //
                // Creating Importing In DatesOfPriceList
                //
                DbParameter[] ParametersOfImporting = new DbParameter[6] { 
                    new MySqlParameter("ID_PH",          MySqlDbType.Int16), 
                    new MySqlParameter("ID",             MySqlDbType.Int16),
                    new MySqlParameter("Caption",        MySqlDbType.VarChar),
                    new MySqlParameter("Text",           MySqlDbType.LongText),
                    new MySqlParameter("Published",      MySqlDbType.Bit),
                    new MySqlParameter("DateOfUpdating", MySqlDbType.DateTime)
                };
                //VarChar
                //
                MySqlCommand ImportingInAnnouncements =
                    (MySqlCommand)CreatingCommand("pharm66.UpdatingOfAnnouncements", ParametersOfImporting);
                ImportingInAnnouncements.CommandType = CommandType.StoredProcedure;
                //
                // Importing Data In DatesOfPriceList
                //
                ImportingData("Updating Table Of Announcements", ImportingInAnnouncements, DataForUpdating);
            }
        }

        // Importing Data
        private void ImportingData(
            string NameOfUpdating, MySqlCommand CommandOfImporting, DataTable TableOfImporting)
        {
            //
            // Importing Data
            //
            try
            {
                OpeningConnection(CommandOfImporting.Connection);
                //
                RecordingInLogFile(String.Format("Starting {0} ({1})", 
                    NameOfUpdating, TableOfImporting.Rows.Count));
                //
                foreach (DataRow CurrentRow in TableOfImporting.Rows)
                {
                    //
                    foreach (DataColumn CurrentColumn in TableOfImporting.Columns)
                        if (CommandOfImporting.Parameters.Contains(CurrentColumn.ColumnName))
                            CommandOfImporting.Parameters[CurrentColumn.ColumnName].Value =
                                CurrentRow[CurrentColumn.ColumnName];
                    //
                    CommandOfImporting.ExecuteScalar();
                    //
                }
                //
                RecordingInLogFile(String.Format("Stoping {0}", NameOfUpdating));
                //
                ClosingConnection(CommandOfImporting.Connection);
                //
            }
            catch (Exception E)
            {
                //
                ClosingConnection(CommandOfImporting.Connection);
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при импортировании данных таблицы {0}: {1}",
                    TableOfImporting.TableName, E.Message));
            }
        }

        // Updating Date And Number Of Updating
        public override void UpdatingDateAndNumberOfUpdatingARH(DateTime DateOfUpdating, int NumberOfExported)
        {
            // 5
            DbCommand CommandOfUpdating = CreatingCommand(
                "UPDATE Service SET Value = @P1, Date_Service = @P2 WHERE Id_Service IN (2, 3, 9);",
                new DbParameter[2] { 
                        new MySqlParameter("@P1", MySqlDbType.Int16), 
                        new MySqlParameter("@P2", MySqlDbType.DateTime) });
            CommandOfUpdating.CommandTimeout = 1000;
            //
            try
            {
                CommandOfUpdating.Parameters["@P1"].Value = NumberOfExported;
                CommandOfUpdating.Parameters["@P2"].Value = DateOfUpdating;
                OpeningConnection(CommandOfUpdating.Connection);
                CommandOfUpdating.ExecuteScalar();
                ClosingConnection(CommandOfUpdating.Connection);
            }
            catch (Exception E)
            {
                ClosingConnection(CommandOfUpdating.Connection);
                ReturningMessageAboutError("Ошибка при обновлении даты обновления", E, false);
            }
            //
        }

        // Updating Date Of Actual
        public override void UpdatingDateOfActual(DateTime DateOfActual)
        {
            //
            // !!!
            //
            DbCommand CommandOfUpdating = CreatingCommand(
                "UPDATE Service SET Date_Service = @P1 WHERE Id_Service = 9;", 
                new DbParameter[1] { new MySqlParameter("@P1", MySqlDbType.DateTime) });
            CommandOfUpdating.CommandTimeout = 1000;
            //
            // !!!
            //
            try
            {
                //
                CommandOfUpdating.Parameters["@P1"].Value = DateOfActual;
                //
                OpeningConnection(CommandOfUpdating.Connection);
                //
                CommandOfUpdating.ExecuteScalar();
                //
                ClosingConnection(CommandOfUpdating.Connection);
            }
            catch (Exception E)
            {
                //
                ClosingConnection(CommandOfUpdating.Connection);
                //
                //ReturningMessageAboutError("Ошибка при обновлении даты обновления", E, false);
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении даты актуальности: {0}", E.Message));
                RecordingInLogFile("");
            }
        }

        // Updating Number Of Updating
        public override void UpdatingNumberOfUpdating(int NumberOfUpdating)
        {
            //
            // !!!
            //
            DbCommand CommandOfUpdating = CreatingCommand(
                "UPDATE Service SET Value = @P1 WHERE Id_Service = 9;", 
                new DbParameter[1] { new MySqlParameter("@P1", MySqlDbType.Int16) });
            CommandOfUpdating.CommandTimeout = 1000;
            //
            // !!!
            //
            try
            {
                //
                CommandOfUpdating.Parameters["@P1"].Value = NumberOfUpdating;
                //
                OpeningConnection(CommandOfUpdating.Connection);
                //
                CommandOfUpdating.ExecuteScalar();
                //
                ClosingConnection(CommandOfUpdating.Connection);
            }
            catch (Exception E)
            {
                //
                ClosingConnection(CommandOfUpdating.Connection);
                //
                //ReturningMessageAboutError("Ошибка при обновлении номера обновления", E, false);
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении номера обновления: {0}", E.Message));
                RecordingInLogFile("");
            }
        }

        // Updating Number Of Updating
        public override void UpdatingNumberOfUpdatingPriceLists(int NumberOfUpdating)
        {
            //
            // !!!
            //
            DbCommand CommandOfUpdating = CreatingCommand(
                "UPDATE Service SET Value = @P1 WHERE Id_Service = 4;",
                new DbParameter[1] { new MySqlParameter("@P1", MySqlDbType.Int16) });
            CommandOfUpdating.CommandTimeout = 1000;
            //
            // !!!
            //
            try
            {
                //
                CommandOfUpdating.Parameters["@P1"].Value = NumberOfUpdating;
                //
                OpeningConnection(CommandOfUpdating.Connection);
                //
                CommandOfUpdating.ExecuteScalar();
                //
                ClosingConnection(CommandOfUpdating.Connection);
            }
            catch (Exception E)
            {
                //
                ClosingConnection(CommandOfUpdating.Connection);
                //
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении номера обновления Прайс-Листов: {0}", E.Message));
                RecordingInLogFile("");
            }
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

        #region ' Archive '

        /*
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
        */
        //
        // Getting List Of Deleting ID Of Drugstores
        //
        /*
        DataTable ListOfDeletingIDsOfDrugstores = new DataTable("ListOfDeletingIDsOfDrugstores");
        //
        MySqlCommand SelectionOfIDs = 
            new MySqlCommand(
                "SELECT DISTINCT ID_Pharmacy AS 'ID' FROM Price_List;", 
                new MySqlConnection(StringOfConnection));
        MySqlDataAdapter GettingListOfDeleting = new MySqlDataAdapter(SelectionOfIDs);
        //
        GettingListOfDeleting.FillSchema(ListOfDeletingIDsOfDrugstores, SchemaType.Source);
        GettingListOfDeleting.Fill(ListOfDeletingIDsOfDrugstores);
        */
        /*
CommandOfClearing.CommandText = 
String.Format(
"DELETE FROM Price_List WHERE ID_Pharmacy = {0}", CurrentID["ID"]);
//
RecordingInLogFile(String.Format("Starting Deleting PL {0}", CurrentID["ID"]));
*/
        /*NewStringOfPrices = 
"INSERT INTO Price_List(Id_Pharmacy, Id_Product, Price, Date_upd, Is_privilege, Is_deleted) VALUES ";*/
        /*if ((CountPricesInCurrentPacket == SizeOfPackage) ||
    (i == (PriceListsForImporting.Length - 1)))*/
        /*
        if ((CountPricesInCurrentPacket < (SizeOfPackage - 1)) && 
            (i != (PriceListsForImporting.Length - 1)))
            NewStringOfPrices += ", ";
        */
        //
        //
        //string[] ElementsOfPrice = new string[0];
        //
        //
        // Importing PriceLists From File
        //
        // Starting Threads Of Importing PriceLists
        //
        /*RecordingInLogFile(
            String.Format("Starting Threads Importing PriceLists ({0})", PacketsOfPriceLists.Count));*/
        /*
         * //RecordingInLogFile(String.Format("Count Of Read Prices: {0}", PriceListsForImporting.Length));
        int CountOfStartingThreads = PacketsOfPriceLists.Count;
        //
        CountOfExecutedPackagesPriceLists = 0;
        //
        for (int i = 0; i < CountOfStartingThreads; i++)
        {
            //
            new System.Threading.Thread(
                new System.Threading.ThreadStart(ImportingPricesInPriceLists)).Start();
            //
            //RecordingInLogFile(String.Format("Thread ({0})", i));
            //
            System.Threading.Thread.Sleep(1000);
            //
            // If in Queue big 10 Packet
            //
            if (CountOfStartingThreads > 10)
            {
                //
                // Waiting Of Executing
                //
                while (!(((i + 1) - CountOfExecutedPackagesPriceLists) < 10))
                { System.Threading.Thread.Sleep(500); }
            }
        }
        */
        /*
                        try
                        {
                            //
                            // !!!
                            //
                            try
                            {
                                //
                                string StringOfPriceLists = SR.ReadToEnd();
                                //
                                PriceListsForImporting =
                                    StringOfPriceLists.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                //
                                StringOfPriceLists = "";
                            }
                            catch (Exception E)
                            { RecordingInLogFile(String.Format("ERROR Ошибка при чтении Прайс-листов из файла: {0}", E.Message)); }
                        }
                        catch (Exception E)
                        { RecordingInLogFile(String.Format("ERROR Ошибка при создании потоков файла Прайс-листов: {0}", E.Message)); }
                        */
        /*
        DataTable DividedDataForUpdating = DataForUpdating.Clone();
        DividedDataForUpdating.TableName = "ListOfAnnouncements";
        DividedDataForUpdating.PrimaryKey = null;
        //
        // 1 000 000
        foreach (DataRow CurrentAnnouncement in DataForUpdating.Rows)
        {
            if (((string)CurrentAnnouncement["Text"]).Length > 1000000)
            {
                //
                string TextOfAnnouncement = CurrentAnnouncement["Text"].ToString();
                //
                while (TextOfAnnouncement.Length > 0)
                {
                    //
                    int LengthOfPart = 
                        (TextOfAnnouncement.Length > 500000) ? 500000 : TextOfAnnouncement.Length;
                    string PartOfTextOfAnnouncement = TextOfAnnouncement.Substring(0, LengthOfPart);
                    TextOfAnnouncement = TextOfAnnouncement.Remove(0, LengthOfPart);
                    //
                    DividedDataForUpdating.Rows.Add(
                        CurrentAnnouncement["ID_PH"],
                        CurrentAnnouncement["ID"],
                        CurrentAnnouncement["Caption"],
                        PartOfTextOfAnnouncement,
                        CurrentAnnouncement["Published"],
                        CurrentAnnouncement["DateOfUpdating"]);
                }
            }
            else
            {
                DividedDataForUpdating.Rows.Add(CurrentAnnouncement.ItemArray);
            }
        }
        //
        // !!!
        //
        foreach (DataRow CurA in DividedDataForUpdating.Rows)
        {
            Console.WriteLine(CurA["Text"].ToString().Length);
            //Console.WriteLine(CurA["Text"]);
        }
        */
        /*
                ImportingInDatesOfPriceList.Connection.Open();
                //
                RecordingInLogFile("Starting Updating Dates Of Table Of PriceList");
                //
                foreach (DataRow CurrentDate in DataForUpdating.Rows)
                {
                    //
                    ImportingInDatesOfPriceList.Parameters["ID"].Value =   CurrentDate["ID"];
                    ImportingInDatesOfPriceList.Parameters["Date"].Value =  CurrentDate["Date"];
                    //
                    ImportingInDatesOfPriceList.ExecuteScalar();
                    //
                }
                //
                RecordingInLogFile("Stoping Updating Table Of GroupsOfProducts");
                //
                ImportingInDatesOfPriceList.Connection.Close();
                */
        /*
                ImportingInPriceList.Connection.Open();
                //
                RecordingInLogFile("Starting Updating Table Of PriceList");
                //
                foreach (DataRow CurrentPrice in DataForPriceList.Rows)
                {
                    //
                    ImportingInPriceList.Parameters["ID_PH"].Value =        CurrentPrice["ID_PH"];
                    ImportingInPriceList.Parameters["ID_PR"].Value =        CurrentPrice["ID_PR"];
                    ImportingInPriceList.Parameters["Price"].Value =        CurrentPrice["Price"];
                    ImportingInPriceList.Parameters["Updating"].Value =     CurrentPrice["Updating"];
                    ImportingInPriceList.Parameters["Preferential"].Value = CurrentPrice["Preferential"];
                    ImportingInPriceList.Parameters["Deleting"].Value =     CurrentPrice["Deleting"];
                    //
                    ImportingInPriceList.ExecuteScalar();
                    //
                }
                //
                RecordingInLogFile("Stoping Updating Table Of PriceList");
                //
                ImportingInPriceList.Connection.Close();
                //
                */
        /*
                ImportingInDrugstores.Connection.Open();
                //
                RecordingInLogFile("Starting Updating Table Of Drugstores");
                //
                foreach (DataRow CurrentDrugstore in DataForDrugstores.Rows)
                {
                    //
                    ImportingInDrugstores.Parameters["ID"].Value =        CurrentDrugstore["ID"];
                    ImportingInDrugstores.Parameters["ID_DI"].Value =     CurrentDrugstore["ID_DI"];
                    ImportingInDrugstores.Parameters["Name"].Value =      CurrentDrugstore["Name"];
                    ImportingInDrugstores.Parameters["Address"].Value =   CurrentDrugstore["Address"];
                    ImportingInDrugstores.Parameters["Phone"].Value =     CurrentDrugstore["Phone"];
                    ImportingInDrugstores.Parameters["Mail"].Value =      CurrentDrugstore["Mail"];
                    ImportingInDrugstores.Parameters["Site"].Value =      CurrentDrugstore["Site"];
                    ImportingInDrugstores.Parameters["Schedule"].Value =  CurrentDrugstore["Schedule"];
                    ImportingInDrugstores.Parameters["Transport"].Value = CurrentDrugstore["Transport"];
                    ImportingInDrugstores.Parameters["Deleting"].Value =  CurrentDrugstore["Deleting"];
                    //
                    ImportingInDrugstores.ExecuteScalar();
                    //
                }
                //
                RecordingInLogFile("Stoping Updating Table Of Drugstores");
                //
                ImportingInDrugstores.Connection.Close();
                */
        /*
                ImportingInGroupsOfProducts.Connection.Open();
                //
                RecordingInLogFile("Starting Updating Table Of GroupsOfProducts");
                //
                foreach (DataRow CurrentGroup in DataForGroupsOfProducts.Rows)
                {
                    //
                    ImportingInGroupsOfProducts.Parameters["ID"].Value =             CurrentGroup["ID"];
                    ImportingInGroupsOfProducts.Parameters["Name"].Value =           CurrentGroup["Name"];
                    ImportingInGroupsOfProducts.Parameters["DateOfUpdating"].Value = CurrentGroup["DateOfUpdating"];
                    ImportingInGroupsOfProducts.Parameters["Deleting"].Value =       CurrentGroup["Deleting"];
                    //
                    ImportingInGroupsOfProducts.ExecuteScalar();
                    //
                }
                //
                RecordingInLogFile("Stoping Updating Table Of GroupsOfProducts");
                //
                ImportingInGroupsOfProducts.Connection.Close();
                //
                */
        /*
                ImportingInProducts.Connection.Open();
                //
                RecordingInLogFile("Starting Updating Table Of Products");
                //
                foreach (DataRow CurrentProduct in DataForProducts.Rows)
                {
                    //
                    ImportingInProducts.Parameters["ID"].Value =          CurrentProduct["ID"];
                    ImportingInProducts.Parameters["ID_PG"].Value =       CurrentProduct["ID_PG"];
                    ImportingInProducts.Parameters["Name"].Value =        CurrentProduct["Name"];
                    ImportingInProducts.Parameters["Composition"].Value = CurrentProduct["Composition"];
                    ImportingInProducts.Parameters["Description"].Value = CurrentProduct["Description"];
                    ImportingInProducts.Parameters["Updating"].Value =    CurrentProduct["Updating"];
                    ImportingInProducts.Parameters["Deleting"].Value =    CurrentProduct["Deleting"];
                    //
                    ImportingInProducts.ExecuteScalar();
                    //
                }
                //
                RecordingInLogFile("Stoping Updating Table Of Products");
                //
                ImportingInProducts.Connection.Close();
                //
                */
        /*

        #region ' Service '

        public override void ClearingOfTableOfPriceList()
        {
            //
            //"UPDATE product SET Is_available = 1 WHERE (Is_available = 0) and Id_Product in (Select Id_Product from price_list where Is_deleted = 0)",
            //"UPDATE product SET Is_available = 0 WHERE (Is_available = 1) and Id_Product not in (Select Id_Product from price_list where Is_deleted = 0)"
            //
            /*
            string[] TextOfCommandOfClearing =
                new string[1]
                { 
                    "DELETE FROM Price_List " + 
                    "WHERE Id_Pharmacy IN (SELECT Id_Pharmacy FROM Pharmacy WHERE Is_deleted = 1);"
                };
            //"DELETE FROM Price_List WHERE Id_Product NOT IN(SELECT Id_Product FROM Product);",
            //
            RecordingInLogFile("Clearing Start");
            //
            ExecutingCommands(TextOfCommandOfClearing);
            //
            RecordingInLogFile("Clearing Stop");
            8/
            //
        }

        protected override void SetStatusOfRows(DataTable TableForStatus,
            string TextOfCheckingOfDeleting, string TextOfCheckingOfExisting, DbParameter[] ParametersOfCommand)
        {
            try
            {
                //
                // Status Of Rows Unchanged
                //
                TableForStatus.AcceptChanges();
                //
                // Getting IDs Of Table
                //
                DbCommand CommandOfExisting = CreatingCommand(TextOfCheckingOfExisting, ParametersOfCommand);
                DataTable IDsOfTable = new DataTable();
                MySqlDataAdapter ReadingIDs = new MySqlDataAdapter((MySqlCommand)CommandOfExisting);
                ReadingIDs.FillSchema(IDsOfTable, SchemaType.Source);
                ReadingIDs.Fill(IDsOfTable);
                //
                // Creating Keys
                //
                DataColumn[] ColumnsOfKey = new DataColumn[IDsOfTable.Columns.Count];
                string[] NamesOfColumnsOfKey = new string[IDsOfTable.Columns.Count];
                object[] KeyToExist = new object[IDsOfTable.Columns.Count];
                //
                for (int i = 0; i < ColumnsOfKey.Length; i++)
                    ColumnsOfKey[i] = IDsOfTable.Columns[i];
                IDsOfTable.PrimaryKey = ColumnsOfKey;
                //
                int Increment = 0;
                foreach (DataColumn CurrentColumn in ColumnsOfKey)
                {
                    if (ColumnsOfKey.Length == 2)
                    {
                        if (CurrentColumn.ColumnName == "Id_Pharmacy")
                            NamesOfColumnsOfKey[Increment++] = "ID_PH";
                        else if (CurrentColumn.ColumnName == "Id_Product")
                            NamesOfColumnsOfKey[Increment++] = "ID_PR";
                        else
                            NamesOfColumnsOfKey[Increment++] = CurrentColumn.ColumnName;
                    }
                    else
                    {
                        if (CurrentColumn.ColumnName == "Id_Pharmacy")
                            NamesOfColumnsOfKey[Increment++] = "ID";
                        else if (CurrentColumn.ColumnName == "Id_Product")
                            NamesOfColumnsOfKey[Increment++] = "ID";
                        else if (CurrentColumn.ColumnName == "Id_product_group")
                            NamesOfColumnsOfKey[Increment++] = "ID";
                        else
                            NamesOfColumnsOfKey[Increment++] = CurrentColumn.ColumnName;
                    }
                }
                //
                // Set Status Of Rows Is Deleted
                //
                if (TextOfCheckingOfDeleting != "")
                {
                    DataView FilteringOfStatus = new DataView(TableForStatus);
                    FilteringOfStatus.RowFilter = TextOfCheckingOfDeleting;
                    while (FilteringOfStatus.Count > 0)
                        FilteringOfStatus[0].Row.Delete();
                }
                //
                // Set Status Of Rows Is Added And Modified
                //
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    if (CurrentRow.RowState == DataRowState.Unchanged)
                    {
                        //
                        Increment = 0;
                        foreach (string CurrentName in NamesOfColumnsOfKey)
                            KeyToExist[Increment++] = CurrentRow[CurrentName];
                        //
                        if (IDsOfTable.Rows.Contains(KeyToExist))
                            CurrentRow.SetModified();
                        else
                            CurrentRow.SetAdded();
                    }
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении статуса строк", E, false); }
            
        }

        private void ReadingStatus(DataTable DT)
        {
            //
            int S_M = 0, S_A = 0, S_R = 0, S_U = 0;
            for (int i = 0; i < DT.Rows.Count; i++)
                if (DT.Rows[i].RowState == DataRowState.Added)
                    S_A++;
                else if (DT.Rows[i].RowState == DataRowState.Deleted)
                    S_R++;
                else if (DT.Rows[i].RowState == DataRowState.Modified)
                    S_M++;
                else if (DT.Rows[i].RowState == DataRowState.Unchanged)
                    S_U++;
            RecordingInLogFile(
                String.Format("CountR={4} Add={0} Rem={1} Mod={2} Unc={3}", 
                S_A, S_R, S_M, S_U, DT.Rows.Count));
        }

        #endregion

        private void UpdatingOfPharmacyARH(DataTable DataForPharmacy)
        {
            if (CheckingOfData(DataForPharmacy))
            {
                //
                if (DataForPharmacy.Rows.Count > 0)
                {
                    //
                    DataForPharmacy = DataForPharmacy.Copy();
                    //
                    // Installation Of Status Of Rows
                    //
                    SetStatusOfRows(DataForPharmacy,
                        "", "SELECT Id_Pharmacy FROM Pharmacy;", new DbParameter[0]);
                    //
                    // Creating Insert Command
                    //
                    DbParameter[] ParametersOfInsertingCommand = new DbParameter[10] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,   0, "ID"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,   0, "ID_DI"), 
                    new MySqlParameter("@P3", MySqlDbType.VarChar, 0, "Name"), 
                    new MySqlParameter("@P4", MySqlDbType.VarChar, 0, "Address"), 
                    new MySqlParameter("@P5", MySqlDbType.VarChar, 0, "Phone"), 
                    new MySqlParameter("@P6", MySqlDbType.VarChar, 0, "Mail"), 
                    new MySqlParameter("@P7", MySqlDbType.VarChar, 0, "Site"), 
                    new MySqlParameter("@P8", MySqlDbType.VarChar, 0, "Schedule"), 
                    new MySqlParameter("@P9", MySqlDbType.VarChar, 0, "Transport"), 
                    new MySqlParameter("@P10",MySqlDbType.Bit,     0, "Deleting") };
                    //
                    MySqlCommand CommandOfInsertingPharmacy = (MySqlCommand)CreatingCommand(
                        "INSERT INTO Pharmacy(Id_Pharmacy, Id_District, Name_full, Addr, Phone, Mail, Web, Hours, Trans, Is_deleted) " +
                        "VALUES(@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10); ",
                        ParametersOfInsertingCommand);
                    //
                    // Creating Update Command
                    //
                    DbParameter[] ParametersOfUpdatingCommand = new DbParameter[10] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,   0, "ID"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,   0, "ID_DI"), 
                    new MySqlParameter("@P3", MySqlDbType.VarChar, 0, "Name"), 
                    new MySqlParameter("@P4", MySqlDbType.VarChar, 0, "Address"), 
                    new MySqlParameter("@P5", MySqlDbType.VarChar, 0, "Phone"), 
                    new MySqlParameter("@P6", MySqlDbType.VarChar, 0, "Mail"), 
                    new MySqlParameter("@P7", MySqlDbType.VarChar, 0, "Site"), 
                    new MySqlParameter("@P8", MySqlDbType.VarChar, 0, "Schedule"), 
                    new MySqlParameter("@P9", MySqlDbType.VarChar, 0, "Transport"), 
                    new MySqlParameter("@P10",MySqlDbType.Bit,     0, "Deleting") };
                    //
                    MySqlCommand CommandOfUpdatingPharmacy = (MySqlCommand)CreatingCommand(
                        "UPDATE Pharmacy " +
                        "SET Id_District = @P2, Name_full = @P3, Addr = @P4, Phone = @P5, Mail = @P6, Web = @P7, Hours = @P8, Trans = @P9, Is_deleted = @P10 " +
                        "WHERE Id_Pharmacy = @P1;",
                        ParametersOfUpdatingCommand);
                    //
                    // Assignment Of Commands
                    //
                    _UpdatingOfData.InsertCommand = CommandOfInsertingPharmacy;
                    _UpdatingOfData.UpdateCommand = CommandOfUpdatingPharmacy;
                    /8//
                    // Updating
                    //
                    try { _UpdatingOfData.Update(DataForPharmacy); }
                    catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении таблицы Pharmacy", E, false); }
                    //
                    // Clearing Of UpdatingOfData 
                    //
                    _UpdatingOfData.InsertCommand = null;
                    _UpdatingOfData.UpdateCommand = null;8/
                    //
                    UpdateOfUpdatingData(DataForPharmacy, "Pharmacy");
                }
            }
        }

        private void UpdatingOfProductsARH(DataTable DataForProducts)
        {
            if (CheckingOfData(DataForProducts))
            {
                //
                if (DataForProducts.Rows.Count > 0)
                {
                    //
                    DataForProducts = DataForProducts.Copy();
                    //
                    // !!!
                    //
                    DateTime DateOfUpdating = DateTime.Now;
                    if (DataForProducts.Rows.Count > 0)
                        DateOfUpdating = (DateTime)DataForProducts.Rows[0]["Updating"];
                    //
                    foreach (DataRow CurrentProduct in DataForProducts.Rows)
                        if (((DateTime)CurrentProduct["Updating"]) > DateOfUpdating)
                            DateOfUpdating = ((DateTime)CurrentProduct["Updating"]);
                    //
                    // Installation Of Status Of Rows
                    //
                    SetStatusOfRows(DataForProducts,
                        "(Deleting = 1)",
                        "SELECT Id_Product FROM Product; ", new MySqlParameter[0]);
                    //
                    // Creating Insert Command
                    //
                    DbParameter[] ParametersOfInsertingCommand = new DbParameter[7] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,    0, "ID"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,    0, "ID_PG"), 
                    new MySqlParameter("@P3", MySqlDbType.VarChar,  0, "Name"), 
                    new MySqlParameter("@P4", MySqlDbType.VarChar,  0, "Composition"), 
                    new MySqlParameter("@P5", MySqlDbType.VarChar,  0, "Description"), 
                    new MySqlParameter("@P6", MySqlDbType.DateTime, 0, "Updating"), 
                    new MySqlParameter("@P7", MySqlDbType.Bit,      0, "Deleting") };
                    //
                    MySqlCommand CommandOfInsertingProduct = (MySqlCommand)CreatingCommand(
                        "INSERT INTO Product(Id_Product, Id_product_group, Name_full, Composition, Description, Date_upd, Is_deleted) " +
                        "VALUES(@P1, @P2, @P3, @P4, @P5, @P6, @P7); ",
                        ParametersOfInsertingCommand);
                    //
                    // Creating Updating Command
                    //
                    DbParameter[] ParametersOfUpdatingCommand = new DbParameter[7] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,    0, "ID"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,    0, "ID_PG"), 
                    new MySqlParameter("@P3", MySqlDbType.VarChar,  0, "Name"), 
                    new MySqlParameter("@P4", MySqlDbType.VarChar,  0, "Composition"), 
                    new MySqlParameter("@P5", MySqlDbType.VarChar,  0, "Description"), 
                    new MySqlParameter("@P6", MySqlDbType.DateTime, 0, "Updating"), 
                    new MySqlParameter("@P7", MySqlDbType.Bit,      0, "Deleting") };
                    //
                    MySqlCommand CommandOfUpdatingProduct = (MySqlCommand)CreatingCommand(
                        "UPDATE Product " +
                        "SET Id_product_group = @P2, Name_full = @P3, Composition = @P4, Description = @P5, Date_upd = @P6, Is_deleted = @P7 " +
                        "WHERE Id_Product = @P1;",
                        ParametersOfUpdatingCommand);
                    //
                    // Creating Deleting Command
                    //
                    DbParameter[] ParametersOfDeletingCommand = new DbParameter[1] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,     0, "ID") };
                    //
                    MySqlCommand CommandOfDeletingProduct = (MySqlCommand)CreatingCommand(
                        "DELETE FROM Product WHERE (Id_Product = @P1);",
                        ParametersOfDeletingCommand);
                    //
                    // Assignment Of Commands
                    //
                    _UpdatingOfData.InsertCommand = CommandOfInsertingProduct;
                    _UpdatingOfData.UpdateCommand = CommandOfUpdatingProduct;
                    _UpdatingOfData.DeleteCommand = CommandOfDeletingProduct;
                    //
                    UpdateOfUpdatingData(DataForProducts, "Product");
                    //
                    // !!!
                    //
                    // 5
                    DbCommand CommandOfUpdating = CreatingCommand(
                        "UPDATE Service SET Value = 0, Date_Service = @P1 WHERE Id_Service = 5;",
                        new DbParameter[1] { new MySqlParameter("@P1", MySqlDbType.DateTime) });
                    CommandOfUpdating.CommandTimeout = 1000;
                    //
                    try
                    {
                        CommandOfUpdating.Parameters["@P1"].Value = DateOfUpdating;
                        //CommandOfUpdating.Parameters["@P2"].Value = DateOfUpdating;
                        CommandOfUpdating.Connection.Open();
                        CommandOfUpdating.ExecuteScalar();
                        CommandOfUpdating.Connection.Close();
                    }
                    catch (Exception E)
                    {
                        if (CommandOfUpdating.Connection.State == ConnectionState.Open)
                            CommandOfUpdating.Connection.Close();
                        ReturningMessageAboutError("Ошибка при обновлении даты обновления Продуктов", E, false);
                    }
                    //
                    /8//
                    // Updating
                    //
                    try { _UpdatingOfData.Update(DataForProducts); }
                    catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении таблицы Product", E, false); }
                    //
                    // Clearing Of UpdatingOfData 
                    //
                    _UpdatingOfData.InsertCommand = null;
                    _UpdatingOfData.UpdateCommand = null;8/
                }
            }
        }

        private void UpdatingOfPriceListARH(DataTable DataForPriceList)
        {
            if (CheckingOfData(DataForPriceList))
            {
                //
                DataForPriceList = DataForPriceList.Copy();
                //
                // Installation Of Status Of Rows
                //
                RecordingInLogFile("Start Reading Status Of PriceList");
                //
                SetStatusOfRows(DataForPriceList,
                    "(Deleting = 1)", "SELECT Id_Pharmacy, Id_Product FROM Price_List;",
                    new DbParameter[0]);
                //
                RecordingInLogFile("End Reading Status Of PriceList");
                //
                ReadingStatus(DataForPriceList);
                //
                // Creating Insert Command
                //
                DbParameter[] ParametersOfInsertingCommand = new DbParameter[6] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,    0, "ID_PH"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,    0, "ID_PR"), 
                    new MySqlParameter("@P3", MySqlDbType.Decimal,  0, "Price"), 
                    new MySqlParameter("@P4", MySqlDbType.DateTime, 0, "Updating"), 
                    new MySqlParameter("@P5", MySqlDbType.Bit,      0, "Preferential"), 
                    new MySqlParameter("@P6", MySqlDbType.Bit,      0, "Deleting") };
                //
                MySqlCommand CommandOfInsertingPriceList = (MySqlCommand)CreatingCommand(
                    "INSERT INTO Price_list(Id_Pharmacy, ID_Product, Price, Date_upd, Is_privilege, Is_deleted) " +
                    "VALUES (@P1, @P2, @P3, @P4, @P5, @P6);",
                    ParametersOfInsertingCommand);
                //
                // Creating Updating Command
                //
                DbParameter[] ParametersOfUpdatingCommand = new DbParameter[6] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,    0, "ID_PH"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,    0, "ID_PR"), 
                    new MySqlParameter("@P3", MySqlDbType.Decimal,  0, "Price"), 
                    new MySqlParameter("@P4", MySqlDbType.DateTime, 0, "Updating"), 
                    new MySqlParameter("@P5", MySqlDbType.Bit,      0, "Preferential"), 
                    new MySqlParameter("@P6", MySqlDbType.Bit,      0, "Deleting") };
                //
                MySqlCommand CommandOfUpdatingPriceList = (MySqlCommand)CreatingCommand(
                    "UPDATE Price_list " +
                    "SET Price = @P3, Date_upd = @P4, Is_privilege = @P5, Is_deleted = @P6 " +
                    "WHERE ((Id_Pharmacy = @P1) AND (Id_Product = @P2));",
                    ParametersOfUpdatingCommand);
                //
                // Creating Deleting Command
                //
                DbParameter[] ParametersOfDeletingCommand = new DbParameter[2] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,    0, "ID_PH"), 
                    new MySqlParameter("@P2", MySqlDbType.Int16,    0, "ID_PR"),  };
                //
                MySqlCommand CommandOfDeletingPriceList = (MySqlCommand)CreatingCommand(
                    "DELETE FROM Price_List WHERE ((Id_Pharmacy = @P1) AND (Id_Product = @P2));",
                    ParametersOfDeletingCommand);
                //
                // Assignment Of Commands
                //
                _UpdatingOfData.ContinueUpdateOnError = true;
                _UpdatingOfData.InsertCommand = CommandOfInsertingPriceList;
                _UpdatingOfData.UpdateCommand = CommandOfUpdatingPriceList;
                _UpdatingOfData.DeleteCommand = CommandOfDeletingPriceList;
                //
                // Updating
                //
                /*RecordingInLogFile(String.Format("Start Updating Table Of {0}", "Price_List"));
                //
                try { _UpdatingOfData.Update(DataForPriceList); }
                catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении таблицы Price_list", E, false); }
                //
                RecordingInLogFile(String.Format("End Updating Table Of {0}", "Price_List"));
                //
                ReadingStatus(DataForPriceList);
                //
                // Clearing Of UpdatingOfData 
                //
                _UpdatingOfData.InsertCommand = null;
                _UpdatingOfData.UpdateCommand = null;8/
                //
                //RecordingInLogFile(String.Format("Start Updating Table Of {0}", "Price_List"));
                //
                UpdateOfUpdatingData(DataForPriceList, "Price_list");
                //
                ReadingStatus(DataForPriceList);
                //
                //
                //RecordingInLogFile(String.Format("End Updating Table Of {0}", "Price_List"));
                //
            }
        }

        private void UpdatingOfDatesOfPriceListARH(DataTable DataForUpdating)
        {
            //
            // Status Of Modified
            //
            DataForUpdating.AcceptChanges();
            foreach (DataRow CurrentRow in DataForUpdating.Rows)
                CurrentRow.SetModified();
            //
            // Creating Updating Command
            //
            DbParameter[] ParametersOfUpdatingCommand = new DbParameter[2] { 
                    new MySqlParameter("@P1", MySqlDbType.Int16,    0, "ID"), 
                    new MySqlParameter("@P2", MySqlDbType.DateTime, 0, "Date") };
            //
            MySqlCommand CommandOfUpdatingDatesOfPriceList = (MySqlCommand)CreatingCommand(
                "UPDATE Price_list " +
                "SET Date_upd = @P2 " +
                "WHERE (Id_Pharmacy = @P1);",
                ParametersOfUpdatingCommand);
            //
            // Assignment Of Commands
            //
            _UpdatingOfData.ContinueUpdateOnError = true;
            _UpdatingOfData.UpdateCommand = CommandOfUpdatingDatesOfPriceList;
            //
            // Updating
            //
            UpdateOfUpdatingData(DataForUpdating, "Price_list");
        }

        private void UpdateOfUpdatingData(DataTable DataForUpdating, string TableName)
        {
            //
            // Updating
            //
            RecordingInLogFile(String.Format("Start A Updating Table Of {0}", TableName));
            //
            try { _UpdatingOfData.Update(DataForUpdating); }
            catch (Exception E)
            { ReturningMessageAboutError(String.Format("Ошибка при обновлении таблицы {0}", TableName), E, false); }
            //
            RecordingInLogFile(String.Format("End A Updating Table Of {0}", TableName));
            //
            // Clearing Of UpdatingOfData 
            //
            _UpdatingOfData.InsertCommand = null;
            _UpdatingOfData.UpdateCommand = null;
            _UpdatingOfData.DeleteCommand = null;
        }

        */

        #endregion

    }
}