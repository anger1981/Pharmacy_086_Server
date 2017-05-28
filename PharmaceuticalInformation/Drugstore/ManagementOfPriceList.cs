using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Threading;
using MySql.Data.MySqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ManagementOfPriceList : GeneralStorage
    {

        #region ' Fields '

        //
        // !!!
        //
        private string StringOfConnection;
        private MySqlConnection ConnectionToBase;
        private MySqlDataAdapter UpdatingOfPriceList;
        //
        private Reading.ReadingOfDataForMySQL ReadingOfInformation;
        //
        // Data Of PriceList
        //
        public DataTable PriceList;
        private ArrayList PriceListForSearching;
        //
        // Filtering
        //
        private int[] _IDOfDisplayingOfDrugstores;
        private bool _EnabledFiltering = false;

        #endregion

        #region ' Designer '

        public ManagementOfPriceList(
            string StringOfConnection, 
            Reading.ReadingOfDataForMySQL ReadingOfInformation)
            : this(StringOfConnection, ReadingOfInformation, "")
        {
            //
        }

        public ManagementOfPriceList(
            string StringOfConnection, 
            Reading.ReadingOfDataForMySQL ReadingOfInformation, 
            string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            this.StringOfConnection = StringOfConnection;
            this.ReadingOfInformation = ReadingOfInformation;
            //
            //UpdatingOfRows = new ArrayList();
            //UpdatingOfRows2 = new Queue();
            //ThreadingOfUpdating = new ArrayList();
            //_SizeOfPackageOfUpdating = 500;
            //
            // Initializing Fields Of Importing
            //
            //DataForImportingInPriceList = new DataTable();
            StringsOfPrices = new Queue();
            //
            // !!!
            //
            PriceList = new DataTable("PriceList");
            _IDOfDisplayingOfDrugstores = new int[0];
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
                { throw new Exception(String.Format("Ошибка при открытии подключения редактирования: {0}", E.Message)); }
            }
            catch (Exception E) 
            { throw new Exception(String.Format("Ошибка при создании подключения редактирования: {0}", E.Message)); }
            //
            // Initialize UpdatingOfPriceList
            //
            //InitializingUpdating();
            UpdatingOfPriceList = CreatingUpdating();
            //ConnectionToBase02 = new MySqlConnection(StringOfConnection);
            //
            // Filling With Data
            //
            FillingPriceList(true);
            //
        }

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
                "PL.Is_deleted AS 'Deleting', PL.Is_privilege AS 'Preferential' " + 
                "FROM exp_price_list AS PL;", 
                NewConnectionToBase);
            //
            //, PL.Sent AS 'Sent'
            /*  "SELECT PL.Id_Pharmacy AS 'ID_PH', PL.Id_Product AS 'ID_PR', P.Name_full AS 'Name', PL.Price AS 'Price', " +
                "PL.Is_deleted AS 'Deleting', PL.Is_privilege AS 'Preferential', PL.Is_all_price AS 'AllPrices', PL.Sent AS 'Sent' " +
                "FROM exp_price_list AS PL LEFT JOIN Product AS P " +
                "ON (PL.Id_Product = P.Id_Product);"*/
            //
            // Creating Command Of Inserting
            //
            MySqlCommand CommandOfInserting = new MySqlCommand(
                "INSERT INTO exp_price_list(Id_Pharmacy, Id_Product, Price, Is_deleted, Is_privilege, Sent) " +
                "VALUES(@P1, @P2, @P3, @P4, @P5, 0); ", NewConnectionToBase);
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
                "SET Price = @P3, Is_deleted = @P4, Is_privilege = @P5, Sent = 0 " +
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
            */
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

        #endregion

        #region ' Filling With Data '

        // Refresh Data Of PriceList
        public void RefreshData()
        {
            //
            FillingPriceList(false);
        }

        // Filling Of PriceList
        private void FillingPriceList(bool CreatingSchema)
        {
            //
            // Clearing PriceList
            //
            PriceList.Clear();
            PriceList.Dispose();
            PriceList = new DataTable("PriceList");
            GC.Collect();
            //
            // Filling With Data
            //
            FillingWithDataOfTable(PriceList, "PriceList", UpdatingOfPriceList, CreatingSchema);
            //
            //if (CreatingSchema)//PriceList.Columns["AllPrices"].AllowDBNull = true;
            //
            // Creation Of Compound Key
            //
            //PriceList.PrimaryKey = new DataColumn[] { PriceList.Columns["ID_PH"], PriceList.Columns["ID_PR"] };
            //
            try { PriceList.PrimaryKey = new DataColumn[] { PriceList.Columns["ID_PH"], PriceList.Columns["ID_PR"] }; }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(String.Format("Ошибка при создании составного ключа Exp_PL: {0}", E.Message));
                this.RecordingInLogFile("Зачистка Exp_PL");
                //
                // Removal Of Recurrences Of Exp_price_list
                //
                IntegrityOfData IOfD = new IntegrityOfData(StringOfConnection, this.PathToLogFile);
                //
                IOfD.RemovalOfRecurrencesOfPriceList("exp_price_list", true);
                //
                // !!!
                //
                try
                {
                    //
                    PriceList.Clear();
                    PriceList.Dispose();
                    PriceList = new DataTable("PriceList");
                    //
                    FillingWithDataOfTable(PriceList, "PriceList", UpdatingOfPriceList, CreatingSchema);
                    //
                    PriceList.PrimaryKey = 
                        new DataColumn[] { PriceList.Columns["ID_PH"], PriceList.Columns["ID_PR"] };
                }
                catch (Exception E2)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("Ошибка при создании составного ключа Exp_PL: {0}", E2.Message));
                    this.RecordingInLogFile("Очистка Exp_PL");
                    //
                    DeletingAllPrices();
                }
            }
            //
            // Addition Of Name Of Products
            //
            ReadingOfInformation.AdditionOfNameOfProducts(PriceList);
            //
            // Accept Changes
            //
            PriceList.AcceptChanges();
            //
            //Creating List Of Products
            //
            PriceListForSearching = CreatingListOfProducts(PriceList);
        }

        #endregion

        #region ' Settings Of Filtering '

        // ID Of Displaying Of Drugstores
        public int[] IDOfDisplayingOfDrugstores
        {
            get { return _IDOfDisplayingOfDrugstores; }
            set { if (value != null) _IDOfDisplayingOfDrugstores = value; }
        }

        // Enabled Filtering
        public bool EnabledFiltering
        {
            get { return _EnabledFiltering; }
            set { _EnabledFiltering = value; }
        }

        #endregion

        #region ' Getting Data '

        #region ' Getting PriceList '

        public DataTable GettingPriceList(
            string NameOfProduct, PharmaceuticalInformation.Reading.ReadingOfData.TypesOfFiltration TypeOfProduct, 
            bool Sold, bool EnableFilteringOfSold)
        {
            //
            // Filtering On Name
            //
            if (NameOfProduct == "")
                TypeOfProduct = PharmaceuticalInformation.Reading.ReadingOfData.TypesOfFiltration.NotSearch;
            //
            // Searching
            //
            DataTable ReturnPriceList = 
                SearchingProducts(
                this.PriceListForSearching, 
                "N", 
                NameOfProduct, TypeOfProduct);
            //
            // Filtering
            //
            DataView FilteringPriceList = new DataView();
            //
            // Filtering On Sold
            //
            if (ReturnPriceList.Columns.Contains("Deleting") && EnableFilteringOfSold)
            {
                //
                FilteringPriceList.Table = ReturnPriceList;
                //
                FilteringPriceList.RowFilter = String.Format("Deleting = {0}", (Sold) ? "0" : "1");
                //
                ReturnPriceList = FilteringPriceList.ToTable();
            }
            //
            // Filtering On Drugstores
            //
            if (_EnabledFiltering && (_IDOfDisplayingOfDrugstores.Length > 0))
            {
                //
                FilteringPriceList.Table = ReturnPriceList;
                //
                // Converting Numbers Of ID In Strings
                //
                string[] StringsOfIDsDrugstores = new string[_IDOfDisplayingOfDrugstores.Length];
                for (int i = 0; i < _IDOfDisplayingOfDrugstores.Length; i++)
                    StringsOfIDsDrugstores[i] = _IDOfDisplayingOfDrugstores[i].ToString();
                //
                // Joining Of Strings Of IDs
                //
                string StringOfIDsDrugstore = String.Join(", ", StringsOfIDsDrugstores);
                //
                // Creating Command Of Selection
                //
                FilteringPriceList.RowFilter = String.Format("ID_PH IN ({0})", StringOfIDsDrugstore);
                //
                ReturnPriceList = FilteringPriceList.ToTable();
                //
                // !!!
                //
            }
            //
            // Creating Primary Key (If Filtering IS Active)
            //
            try { Console.WriteLine(ReturnPriceList.PrimaryKey.Length); }
            catch (Exception E) { this.RecordingInLogFile(String.Format("ERROR Ошибка проверки PK: {0}", E.Message)); }
            //
            ReturnPriceList.PrimaryKey = 
                new DataColumn[] { ReturnPriceList.Columns["ID_PH"], ReturnPriceList.Columns["ID_PR"] };
            //
            //Console.WriteLine("B2 {0}", ReturnPriceList.Columns.Count);
            //
            // Addition Of Column AllPrices
            //
            if (!ReturnPriceList.Columns.Contains("AllPrices"))
            {
                ReturnPriceList.Columns.Add("AllPrices", typeof(bool));
                ReturnPriceList.Columns["AllPrices"].AllowDBNull = true;
            }
            //
            StatusOfPriceList CurrentStatus = GettingStatusOfPriceList;
            //
            foreach (DataRow CurrentPrice in ReturnPriceList.Rows)
                CurrentPrice["AllPrices"] = 
                    (CurrentStatus ==  StatusOfPriceList.FullPriceList) ? true : false;
            //
            if (ReturnPriceList.Columns.Contains("ID_PR"))
                ReturnPriceList.Columns["ID_PR"].SetOrdinal(0);
            if (ReturnPriceList.Columns.Contains("Name"))
                ReturnPriceList.Columns["Name"].SetOrdinal(1);
            if (ReturnPriceList.Columns.Contains("Price"))
                ReturnPriceList.Columns["Price"].SetOrdinal(2);
            //
            ReturnPriceList.AcceptChanges();
            //
            // Return
            //
            return ReturnPriceList;
        }

        #endregion

        // Getting List Names Of Drugstores
        public DataTable GettingListNamesOfDrugstores(int[] IDsOfDrugstores)
        {
            DataTable ListNamesOfDrugstores = new DataTable("Pharmacy");
            //
            if (IDsOfDrugstores.Length > 0)
            {
                //
                // Converting Numbers Of ID In Strings
                //
                string[] StringsOfIDsDrugstores = new string[IDsOfDrugstores.Length];
                for (int i = 0; i < IDsOfDrugstores.Length; i++)
                    StringsOfIDsDrugstores[i] = IDsOfDrugstores[i].ToString();
                //
                // Joining Of Strings Of IDs
                //
                string StringOfIDsDrugstore = String.Join(", ", StringsOfIDsDrugstores);
                //
                // Creating Command Of Selection
                //
                MySqlCommand CommandOfSelection = 
                    new MySqlCommand(
                        String.Format(
                        "SELECT Id_Pharmacy AS 'ID', Name_full AS 'Name', Addr AS 'Address' FROM Pharmacy " + 
                        "WHERE ((Is_deleted = 0)  AND (Id_Pharmacy IN ({0})))", 
                        StringOfIDsDrugstore), 
                        ConnectionToBase);
                //
                // Creating Getting Drugstore
                //
                MySqlDataAdapter GettingDrugstore = new MySqlDataAdapter(CommandOfSelection);
                //
                // Filling
                //
                FillingWithDataOfTable(ListNamesOfDrugstores, "Pharmacy", GettingDrugstore, true);
                //
                //ListNamesOfDrugstores.Columns["ID"].DataType = typeof(int); ERROR
            }
            //
            // Return
            //
            return ListNamesOfDrugstores;
        }

        #endregion

        #region ' Getting Reading Of Information '

        // Getting Reading Of Information
        public Reading.ReadingOfDataForMySQL GettingReadingOfInformation
        {
            get { return ReadingOfInformation; }
        }

        #endregion

        #region ' Management Of Prices '

        // Exists Price
        public bool ExistsPrice(int IDOfDrugstore, int IDOfProduct)
        {
            //
            bool ExistsPrice = false;
            //
            if ((IDOfDrugstore > 0) && (IDOfProduct > 0))
            {
                //
                // Searching Price
                //
                object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
                DataRow FindRow = PriceList.Rows.Find(KeyOfRow);
                //
                if (FindRow != null)
                    ExistsPrice = true;
            }
            //
            // Return
            //
            return ExistsPrice;
        }

        // Addition Of Price
        public void AdditionOfPrice(int IDOfDrugstore, int IDOfProduct, Decimal Price)
        {
            if ((IDOfDrugstore > 0) && (IDOfProduct > 0))
            {
                //
                // Searching Price
                //
                object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
                DataRow FindRow = PriceList.Rows.Find(KeyOfRow);
                //
                // Addition Of Price
                //
                if (FindRow == null)
                {
                    //
                    // !!!
                    //
                    DataRow NewPrice = PriceList.NewRow();
                    //
                    NewPrice["ID_PH"] = IDOfDrugstore;
                    NewPrice["ID_PR"] = IDOfProduct;
                    //
                    // Addition Of Name
                    //
                    DataTable DataOfProduct = ReadingOfInformation.GettingDetailsOfProduct(IDOfProduct);
                    if (DataOfProduct != null)
                        if (DataOfProduct.Rows[0]["Name"].ToString() != "")
                            NewPrice["Name"] = DataOfProduct.Rows[0]["Name"];
                        else
                            NewPrice["Name"] = "";
                    //
                    NewPrice["Price"] = Price;
                    NewPrice["Deleting"] = false;
                    NewPrice["Preferential"] = false;
                    //
                    PriceList.Rows.Add(NewPrice);
                    //
                    // Updating
                    //
                    ExecutedUpdatingOfPriceList();
                    //
                    //Creating List Of Products
                    //
                    PriceListForSearching = CreatingListOfProducts(PriceList);
                    //
                    // Recording
                    //
                    this.RecordingInLogFile(String.Format("Addition Of Product: {0}", IDOfProduct));
                    this.RecordingInLogFile("");
                }
                else
                {
                    /* ??? */
                }
            }
        }

        // Deleting Price
        public void DeletingPrice(int IDOfDrugstore, int IDOfProduct)
        {
            if ((IDOfDrugstore > 0) && (IDOfProduct > 0))
            {
                //
                // Searching Price
                //
                object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
                DataRow FindRow = PriceList.Rows.Find(KeyOfRow);
                //
                // !!!
                //
                if (FindRow != null)
                {
                    //
                    // Deleting
                    //
                    FindRow.Delete();
                    //
                    // Updating
                    //
                    ExecutedUpdatingOfPriceList();
                    //
                    // Creating List Of Products
                    //
                    PriceListForSearching = CreatingListOfProducts(PriceList);
                    //
                    // Recording
                    //
                    this.RecordingInLogFile(String.Format("Deleting Of Product: {0}", IDOfProduct));
                    this.RecordingInLogFile("");
                }
            }
        }

        // All Prices Of Is Sold
        public void AllPricesOfIsSold()
        {
            //
            // !!!
            //
            foreach (DataRow CurrentPrice in PriceList.Rows)
            { CurrentPrice["Deleting"] = true; }
            //
            PriceList.AcceptChanges();
            //
            // Updating
            //
            MySqlCommand SalePrices = new MySqlCommand("UPDATE exp_price_list SET Is_deleted = 1;", ConnectionToBase);
            try
            {
                //
                bool IsOpen = false;
                if (SalePrices.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                //
                if (!IsOpen)
                    OpeningConnection(SalePrices.Connection);
                //
                SalePrices.ExecuteScalar();
                //
                if (!IsOpen)
                    ClosingConnection(SalePrices.Connection);
                //
                // Recording
                //
                this.RecordingInLogFile("All Prices Of Is Sold");
                this.RecordingInLogFile("");
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении строк Прайс-листа", E, false); }
            finally { ClosingConnection(SalePrices.Connection); }
        }

        // Deleting All Prices
        public void DeletingAllPrices()
        {
            //
            // Deleting All Prices
            //
            MySqlCommand DeletingPrices = new MySqlCommand("DELETE FROM exp_price_list;", ConnectionToBase);
            try
            {
                //
                bool IsOpen = false;
                if (DeletingPrices.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                //
                if (!IsOpen)
                    OpeningConnection(DeletingPrices.Connection);
                //
                DeletingPrices.ExecuteScalar();
                //
                if (!IsOpen)
                    ClosingConnection(DeletingPrices.Connection);
                //
                // Recording
                //
                this.RecordingInLogFile("Deleting All Prices");
                this.RecordingInLogFile("");
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при удалении строк Прайс-листа", E, false); }
            finally { ClosingConnection(DeletingPrices.Connection); }
            //
            // Refresh Data Of PriceList
            //
            RefreshData();
        }

        #region ' Giving Value Of Price '

        // Set Status Deleting
        public void SetStatusDeleting(int IDOfDrugstore, int IDOfProduct, bool NewStatus)
        {
            //
            GivingValueOfPrice(IDOfDrugstore, IDOfProduct, "Deleting", NewStatus);
            //
            // Recording
            //
            this.RecordingInLogFile(String.Format("Status Deleting Of Product: {0}", IDOfProduct));
            this.RecordingInLogFile("");
        }

        // Set Status Preferential
        public void SetStatusPreferential(int IDOfDrugstore, int IDOfProduct, bool NewStatus)
        {
            //
            GivingValueOfPrice(IDOfDrugstore, IDOfProduct, "Preferential", NewStatus);
            //
            // Recording
            //
            this.RecordingInLogFile(String.Format("Status Preferential Of Product: {0}", IDOfProduct));
            this.RecordingInLogFile("");
        }

        // Set Price
        public void SetPrice(int IDOfDrugstore, int IDOfProduct, decimal NewPrice)
        {
            //
            GivingValueOfPrice(IDOfDrugstore, IDOfProduct, "Price", NewPrice);
            //
            // Recording
            //
            this.RecordingInLogFile(String.Format("Change Of Price On Product: {0}, New Price: {1}", IDOfProduct, NewPrice));
            this.RecordingInLogFile("");
        }

        // Giving Value Of Price
        private void GivingValueOfPrice(int IDOfDrugstore, int IDOfProduct, string NameOfColum, object Value)
        {
            //
            // Searching Price
            //
            object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
            DataRow FindRow = PriceList.Rows.Find(KeyOfRow);
            //
            // !!!
            //
            if (FindRow != null)
            {
                //
                // !!!
                //
                FindRow[NameOfColum] = Value;
                //
                // Updating
                //
                ExecutedUpdatingOfPriceList();
            }
        }

        #endregion

        #endregion

        #region ' Status Of PriceList '

        // Getting Status Of PriceList
        public StatusOfPriceList GettingStatusOfPriceList
        {
            get
            {
                //
                // Reading Status Of PriceList
                //
                StatusOfPriceList CurrentStatus = StatusOfPriceList.SupplementingPriceList;
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
                        CurrentStatus = StatusOfPriceList.FullPriceList;
                    else
                        CurrentStatus = StatusOfPriceList.SupplementingPriceList;
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

        // Change Of Status Of PriceList
        public void ChangeOfStatusOfPriceList(StatusOfPriceList NewStatus)
        {
            //
            // Updating Status Of PriceList
            //
            MySqlCommand UpdatingStatus = new MySqlCommand(
                String.Format("UPDATE service SET Value = {0} WHERE Id_Service = 6;", 
                (NewStatus == StatusOfPriceList.FullPriceList) ? "1" : "0"), ConnectionToBase);
            try
            {
                //
                bool IsOpen = false;
                //
                if (UpdatingStatus.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                //
                if (!IsOpen)
                    OpeningConnection(UpdatingStatus.Connection);
                //
                UpdatingStatus.ExecuteScalar();
                //
                if (!IsOpen)
                    ClosingConnection(UpdatingStatus.Connection);
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении статуса Прайс-Листа", E, false); }
            finally { ClosingConnection(UpdatingStatus.Connection); }
        }

        //
        public enum StatusOfPriceList
        {
            FullPriceList,
            SupplementingPriceList
        }

        #endregion

        #region ' Service '

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

        // Executed Updating Of PriceList
        private void ExecutedUpdatingOfPriceList()
        {
            //
            // Updating Of Data
            //
            UpdatingOfPriceList.ContinueUpdateOnError = true;
            try { UpdatingOfPriceList.Update(PriceList); }
            catch (Exception E)
            { ReturningMessageAboutError("Ошибка при обновление таблицы PriceList", E, false); }
            //
        }

        #endregion

        #region ' Importing Prices In PriceList '

        //
        public PharmaceuticalInformation.Drugstore.ManagementOfComparisons ManagementOfComparisons;
        //
        //private string StringOfConnection = "";
        private StatusOfPriceList _StatusOfImporting = StatusOfPriceList.SupplementingPriceList;
        private string _MessageOfConverting = "";
        private DataTable DataForImportingInPriceList = new DataTable();
        private bool SanctionOfImporting = true;
        private Queue StringsOfPrices;
        private int CountOfExecutedPackages = 0;
        //
        public delegate void ReturnOfEvent(string Str, int Count01, int Count02);
        public event ReturnOfEvent LoadingPriceListIsCompleted;

        // Status Of Importing
        public StatusOfPriceList StatusOfImporting
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
            _StatusOfImporting = StatusOfPriceList.SupplementingPriceList;
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
                    if (_StatusOfImporting == StatusOfPriceList.FullPriceList)
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
                            if (_StatusOfImporting == StatusOfPriceList.FullPriceList)
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
                            if (_StatusOfImporting == StatusOfPriceList.FullPriceList)
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
                        if (_StatusOfImporting == StatusOfPriceList.FullPriceList)
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
                            new System.Threading.ThreadStart(ImportingPricesInPriceList)).Start();
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
                    RecordingInLogFile(String.Format("ERROR Ошибка при импортировании цен Прайс-Листа: {0}", E.Message));
                    //
                    // !!!
                    //
                    if (_StatusOfImporting == StatusOfPriceList.SupplementingPriceList)
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

        // Getting PriceList
        /*public DataTable GettingPriceList()
        {
            //
            DataTable ReturnPriceList = new DataTable();
            //
            // Filtering
            //
            // Creating Filtering
            //
            DataView FilteringPriceList = new DataView(PriceList);
            //
            Console.WriteLine(_EnabledFiltering);
            //
            if (_EnabledFiltering && (_IDOfDisplayingOfDrugstores.Length > 0))
            {
                //
                // Converting Numbers Of ID In Strings
                //
                string[] StringsOfIDsDrugstores = new string[_IDOfDisplayingOfDrugstores.Length];
                for (int i = 0; i < _IDOfDisplayingOfDrugstores.Length; i++)
                    StringsOfIDsDrugstores[i] = _IDOfDisplayingOfDrugstores[i].ToString();
                //
                // Joining Of Strings Of IDs
                //
                string StringOfIDsDrugstore = String.Join(", ", StringsOfIDsDrugstores);
                //
                // Creating Command Of Selection
                //
                FilteringPriceList.RowFilter = String.Format("ID_PH IN ({0})", StringOfIDsDrugstore);
            }
            //
            // Creating Copy
            //
            ReturnPriceList = FilteringPriceList.ToTable();
            //
            // Creating Primary Key
            //
            ReturnPriceList.PrimaryKey = 
                new DataColumn[] { ReturnPriceList.Columns["ID_PH"], ReturnPriceList.Columns["ID_PR"] };
            //
            // Addition Of Column AllPrices
            //
            ReturnPriceList.Columns.Add("AllPrices", typeof(bool));
            ReturnPriceList.Columns["AllPrices"].AllowDBNull = true;
            //
            foreach (DataRow CurrentPrice in ReturnPriceList.Rows)
                CurrentPrice["AllPrices"] = 
                    (GettingStatusOfPriceList == CurrentStatus) ? true : false;
            //
            // Addition Of Name Of Products
            //
            ReadingOfInformation.AdditionOfNameOfProducts(ReturnPriceList);
            //
            if (ReturnPriceList.Columns.Contains("ID_PR"))
                ReturnPriceList.Columns["ID_PR"].SetOrdinal(0);
            if (ReturnPriceList.Columns.Contains("Name"))
                ReturnPriceList.Columns["Name"].SetOrdinal(1);
            if (ReturnPriceList.Columns.Contains("Price"))
                ReturnPriceList.Columns["Price"].SetOrdinal(2);
            //
            ReturnPriceList.AcceptChanges();
            //
            // Return
            //
            return ReturnPriceList;
        }*/
        // Change Of Sent (Not Working)
        /*public void ChangeOfSent(bool NewSent)
        {
            //
            // Updating Sent
            //
            foreach (DataRow CurrentPrice in PriceList.Rows)
                if(((bool)CurrentPrice["Sent"]) != NewSent)
                    CurrentPrice["Sent"] = NewSent;
            //
            PriceList.AcceptChanges();
            //
            // Updating
            //
            MySqlCommand UpdatingSent = 
                new MySqlCommand(String.Format("UPDATE exp_price_list SET Sent = {0};", NewSent ? "1" : "0"), 
                    ConnectionToBase);
            try
            {
                bool IsOpen = false;
                if (UpdatingSent.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                if (!IsOpen)
                    UpdatingSent.Connection.Open();
                //
                UpdatingSent.ExecuteScalar();
                //
                if (!IsOpen)
                    UpdatingSent.Connection.Close();
            }
            catch (Exception E)
            { ReturningMessageAboutError("Ошибка при обновлении статуса оптравки Прайс-Листа", E, false); }
            finally { UpdatingSent.Connection.Close(); }
            //
            // COM
            //
            //ExecutedUpdatingOfPriceList();
            //
        }*/
        /*
          
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
                    MySqlCommand Executing = new MySqlCommand("SELECT COUNT(*) FROM Matching", ConnectionToBase01);
                    ConnectionToBase01.Open();
                    int CountOfComparisons = Convert.ToInt32(Executing.ExecuteScalar());
                    ConnectionToBase01.Close();
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
                            ConnectionToBase01);
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
        */

    }
}