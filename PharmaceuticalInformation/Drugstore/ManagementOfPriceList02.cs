using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//using System.Threading;
using System.Collections;
using MySql.Data.MySqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ManagementOfPriceList02 : BaseType
    {

        #region ' Fields '

        //
        // !!!
        //
        private string StringOfConnection;
        private MySqlConnection ConnectionToBase01;
        //private MySqlConnection ConnectionToBase02;
        private MySqlDataAdapter UpdatingOfPriceList;
        //
        private Reading.ReadingOfDataForMySQL ReadingOfInformation;
        //
        // Data Of PriceList
        //
        private DataTable PriceList;
        private StatusOfPriceList CurrentStatus;
        //
        // Filtering
        //
        private int[] _IDOfDisplayingOfDrugstores;
        private bool _EnabledFiltering = false;
        //
        //public delegate void ReturnOfEvent();
        //public event ReturnOfEvent LoadingPriceListIsCompleted;

        #endregion

        #region ' Designer '

        public ManagementOfPriceList02(
            string StringOfConnection, 
            Reading.ReadingOfDataForMySQL ReadingOfInformation)
            : this(StringOfConnection, ReadingOfInformation, "")
        {
            //
        }

        public ManagementOfPriceList02(
            string StringOfConnection, 
            Reading.ReadingOfDataForMySQL ReadingOfInformation, 
            string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            this.StringOfConnection = StringOfConnection;
            this.ReadingOfInformation = ReadingOfInformation;

            //
            PriceList = new DataTable("PriceList");
            _IDOfDisplayingOfDrugstores = new int[0];
            //
            // Creating Of Connection
            //
            try
            {
                ConnectionToBase01 = new MySqlConnection(StringOfConnection);
                try
                {
                    ConnectionToBase01.Open();
                    ConnectionToBase01.Close();
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
                "PL.Is_deleted AS 'Deleting', PL.Is_privilege AS 'Preferential', PL.Sent AS 'Sent' " +
                "FROM exp_price_list AS PL;",
                NewConnectionToBase);
            //
            /*  "SELECT PL.Id_Pharmacy AS 'ID_PH', PL.Id_Product AS 'ID_PR', P.Name_full AS 'Name', PL.Price AS 'Price', " +
                "PL.Is_deleted AS 'Deleting', PL.Is_privilege AS 'Preferential', PL.Is_all_price AS 'AllPrices', PL.Sent AS 'Sent' " +
                "FROM exp_price_list AS PL LEFT JOIN Product AS P " +
                "ON (PL.Id_Product = P.Id_Product);"*/
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
            // Reading Status Of PriceList
            //
            MySqlCommand GettingStatus = new MySqlCommand(
                "SELECT Value FROM service WHERE Id_Service = 6;", ConnectionToBase01);
            try
            {
                bool IsOpen = false;
                if (GettingStatus.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                if (!IsOpen)
                    GettingStatus.Connection.Open();
                //
                int ValueOfStatus = (int) GettingStatus.ExecuteScalar();
                if (ValueOfStatus == 1)
                    CurrentStatus = StatusOfPriceList.FullPriceList;
                else
                    CurrentStatus = StatusOfPriceList.SupplementingPriceList;
                //
                if (!IsOpen)
                    GettingStatus.Connection.Close();
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении статуса Прайс-Листа", E, false); }
            finally { GettingStatus.Connection.Close(); }
            /*
            bool AllPrices = false;
            if (PriceList.Rows.Count > 0)
                AllPrices = (bool)PriceList.Rows[0]["AllPrices"];
            //
            if (AllPrices)
                CurrentStatus = StatusOfPriceList.FullPriceList;
            else
                CurrentStatus = StatusOfPriceList.SupplementingPriceList;
            */
        }

        // Refresh Data Of PriceList
        public void RefreshData()
        {
            //
            FillingPriceList(false);
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

        // Getting PriceList
        public DataTable GettingPriceList()
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
        }

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
                        ConnectionToBase01);
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

        // Importing Prices In PriceList
        /*public void ImportingPricesInPriceList(DataTable NewPrices)
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
                        //
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
                /
            }
        }*/

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
                if (FindRow == null)
                {
                    //
                    DataRow NewPrice = PriceList.NewRow();
                    NewPrice["ID_PH"] = IDOfDrugstore;
                    NewPrice["ID_PR"] = IDOfProduct;
                    //
                    /*
                    DataRow NameOfProduct = ReadingOfInformation.GettingProducts().Rows.Find((object) IDOfProduct);
                    if (NameOfProduct != null)
                        if (NameOfProduct["Name"].ToString() != "")
                            NewPrice["Name"] = NameOfProduct["Name"];
                        else
                            NewPrice["Name"] = "";
                    */
                    //
                    NewPrice["Price"] = Price;
                    NewPrice["Deleting"] = false;
                    NewPrice["Preferential"] = false;
                    /*
                    bool AllPrices = false;
                    if (PriceList.Rows.Count > 0)
                        AllPrices = (bool)PriceList.Rows[0]["AllPrices"];
                    Console.WriteLine(AllPrices);
                    NewPrice["AllPrices"] = AllPrices;
                    */
                    NewPrice["Sent"] = false;
                    PriceList.Rows.Add(NewPrice);
                    //
                    // Updating
                    //
                    ExecutedUpdatingOfPriceList();
                    //
                    // Change Of Status Of PriceList ???
                    //
                    //ChangeOfStatusOfPriceList(CurrentStatus);
                }
                else
                { /* ??? */ }
            }
        }

        // Deleting Price
        public void DeletingPrice(int IDOfDrugstore, int IDOfProduct)
        {
            //
            // Searching Price
            //
            object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
            DataRow FindRow  = PriceList.Rows.Find(KeyOfRow);
            if (FindRow != null)
            {
                //
                FindRow.Delete();
                //
                // Updating
                //
                ExecutedUpdatingOfPriceList();
                //
                // Checking Of Count Prices
                //
                if (PriceList.Rows.Count == 0)
                {
                    //
                    // Change Of Status Of PriceList
                    //
                    CurrentStatus = StatusOfPriceList.SupplementingPriceList;
                    ChangeOfStatusOfPriceList(CurrentStatus);
                }
            }
        }

        // Deleting All Prices
        public void DeletingAllPrices()
        {
            //
            // Giving Status Of Deleting
            //
            foreach (DataRow CurrentPrice in PriceList.Rows)
            {
                CurrentPrice.Delete();
            }
            //
            PriceList.AcceptChanges();
            //
            // Updating
            //
            MySqlCommand DeletingPrices = new MySqlCommand("DELETE FROM exp_price_list;", ConnectionToBase01);
            try
            {
                bool IsOpen = false;
                if (DeletingPrices.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                if (!IsOpen)
                    DeletingPrices.Connection.Open();
                //
                DeletingPrices.ExecuteScalar();
                //
                if (!IsOpen)
                    DeletingPrices.Connection.Close();
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при удалении строк Прайс-листа", E, false); }
            finally { DeletingPrices.Connection.Close(); }
            //
            // COM
            //
            //ExecutedUpdatingOfPriceList();
            //TypeOfUpdating = "UP";
            //ExecutedThreadingsOfUpdatingOfPriceList();
            //
            // Change Of Status Of PriceList
            //
            CurrentStatus = StatusOfPriceList.SupplementingPriceList;
            ChangeOfStatusOfPriceList(CurrentStatus);
        }

        #region ' Giving Value Of Price '

        // Set Status Deleting
        public void SetStatusDeleting(int IDOfDrugstore, int IDOfProduct, bool NewStatus)
        {
            //
            GivingValueOfPrice(IDOfDrugstore, IDOfProduct, "Deleting", NewStatus);
        }

        // Set Status Preferential
        public void SetStatusPreferential(int IDOfDrugstore, int IDOfProduct, bool NewStatus)
        {
            //
            GivingValueOfPrice(IDOfDrugstore, IDOfProduct, "Preferential", NewStatus);
        }

        // Set Price
        public void SetPrice(int IDOfDrugstore, int IDOfProduct, decimal NewPrice)
        {
            //
            GivingValueOfPrice(IDOfDrugstore, IDOfProduct, "Price", NewPrice);
        }

        // Giving Value Of Price
        private void GivingValueOfPrice(int IDOfDrugstore, int IDOfProduct, string NameOfColum, object Value)
        {
            //
            // Searching Price
            //
            object[] KeyOfRow = new object[] { IDOfDrugstore, IDOfProduct };
            DataRow FindRow = PriceList.Rows.Find(KeyOfRow);
            if (FindRow != null)
            {
                //
                FindRow[NameOfColum] = Value;
                FindRow["Sent"] = false;
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
            get { return CurrentStatus; }
        }

        // Change Of Status Of PriceList
        public void ChangeOfStatusOfPriceList(StatusOfPriceList NewStatus)
        {
            //
            bool AllPrices = false;
            //
            // Change Of Status
            //
            CurrentStatus = NewStatus;
            if (CurrentStatus == StatusOfPriceList.FullPriceList)
                AllPrices = true;
            /*
            //
            // Updating AllPrices
            //
            foreach (DataRow CurrentPrice in PriceList.Rows)
                CurrentPrice["AllPrices"] = AllPrices;
            */
            //
            // Updating
            //
            //UPDATE exp_price_list SET Is_all_price = {0} WHERE Is_all_price <> {0};
            MySqlCommand UpdatingStatus = new MySqlCommand(
                String.Format("UPDATE service SET Value = {0} WHERE Id_Service = 6;", 
                AllPrices ? "1" : "0"),
                ConnectionToBase01);
            try
            {
                bool IsOpen = false;
                if (UpdatingStatus.Connection.State == ConnectionState.Open)
                    IsOpen = true;
                if(!IsOpen)
                    UpdatingStatus.Connection.Open();
                //
                UpdatingStatus.ExecuteScalar();
                //
                if(!IsOpen)
                    UpdatingStatus.Connection.Close();
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении статуса строк", E, false); }
            finally { UpdatingStatus.Connection.Close(); }
            //
        }

        //
        public enum StatusOfPriceList
        {
            FullPriceList,
            SupplementingPriceList
        }

        // Change Of Sent
        public void ChangeOfSent(bool NewSent)
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
                    ConnectionToBase01);
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
        }

        #endregion

        #region ' Services '

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
            //UpdatingOfPriceList.Update(
            UpdatingOfPriceList.ContinueUpdateOnError = true;
            try { UpdatingOfPriceList.Update(PriceList); }
            catch (Exception E)
            { ReturningMessageAboutError("Ошибка при обновление таблицы PriceList", E, false); }
            //
        }

        #endregion

    }
}