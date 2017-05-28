using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using MySql.Data.MySqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ManagementOfComparisons : GeneralStorage
    {

        #region ' Fields '

        //
        private string StringOfConnection;
        private MySqlConnection ConnectionToBase;
        //
        private MySqlCommand CommandForExecuting;
        //
        private MySqlCommand CommandOfSelection;
        private MySqlDataAdapter FillingWithData;
        //
        private DataTable ListOfComparisons;
        private ArrayList ListOfComparisonsForSearching;

        // IDOfExternalProduct 
        // NameOfExternalProduct 
        // IDOfProduct 
        // NameOfProduct
        // ExistsInPriceList
        // Compare

        #endregion


        #region ' Designer '

        public ManagementOfComparisons(string StringOfConnection)
            : this(StringOfConnection, "")
        {
            //
        }

        public ManagementOfComparisons(string StringOfConnection, string PathToLogFile)
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
                    String.Format("Ошибка при создании подключения ManagementOfComparisons: {0}", E.Message));
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
                      String.Format("Ошибка при проверке подключения ManagementOfComparisons: {0}", E.Message));
                }
            //
            // Initializing Executing
            //
            CommandForExecuting = new MySqlCommand("", ConnectionToBase);
            CommandForExecuting.CommandTimeout = 1000;
            //
            // Initializing Filling With Data
            //
            CommandOfSelection = new MySqlCommand("", ConnectionToBase);
            CommandOfSelection.CommandTimeout = 1000;
            FillingWithData = new MySqlDataAdapter(CommandOfSelection);
            //
            // Initializing List Of Comparisons
            //
            ListOfComparisons = new DataTable("ListOfComparisons");
            ListOfComparisonsForSearching = new ArrayList();
        }

        #endregion


        #region ' Refreshing List Of Comparisons '

        // Refreshing List Of Comparisons
        public void RefreshingListOfComparisons()
        {
            //
            // Creating Text Of Selection Of Comparisons
            //
            string TextOfCommandOfFilling = 
                "SELECT " + 
                "C.IDOfExternalProduct, C.NameOfExternalProduct, " + 
                "C.IDOfProduct, P.Name_full AS 'NameOfProduct', " + 
                "C.ExistsInPriceList, C.Compare " + 
                "FROM " + 
                "Comparisons AS C " + 
                "LEFT JOIN " + 
                "Product AS P " + 
                "ON C.IDOfProduct = P.ID_Product " + 
                "ORDER BY NameOfExternalProduct;";
            //
            // Clearing List Of Comparisons
            //
            ListOfComparisons.Clear();
            ListOfComparisons.Dispose();
            //
            // Filling List Of Comparisons With Data
            //
            ListOfComparisons = 
                FillingWithDataOfTable(TextOfCommandOfFilling, "ListOfComparisons", true);
            //
            // Clearing Null Values Of Names Of Products
            //
            foreach (DataRow CurrentRow in ListOfComparisons.Rows)
                if (CurrentRow["NameOfProduct"] is DBNull)
                    CurrentRow["NameOfProduct"] = "";
            //
            ListOfComparisons.AcceptChanges();
            //
            // Creating Primary Key
            //
            if (ListOfComparisons.Columns.Contains("IDOfExternalProduct"))
            { 
                ListOfComparisons.PrimaryKey = 
                    new DataColumn[] { ListOfComparisons.Columns["IDOfExternalProduct"] };
            }
            //
            // Creating List Of Comparisons For Searching
            //
            CreatingListOfComparisonsForSearching();
        }

        // Creating List Of Comparisons For Searching
        private void CreatingListOfComparisonsForSearching()
        {
            //
            // Renaming Columns
            //
            ListOfComparisons.Columns[1].ColumnName = "Name";
            ListOfComparisons.Columns[3].ColumnName = "MNN";
            //
            // Creating List Of Comparisons For Searching
            //
            ListOfComparisonsForSearching.Clear();
            //
            ListOfComparisonsForSearching = CreatingListOfProducts(ListOfComparisons);
            //
            // Renaming Columns
            //
            ListOfComparisons.Columns[1].ColumnName = "NameOfExternalProduct";
            ListOfComparisons.Columns[3].ColumnName = "NameOfProduct";
        }

        #endregion


        #region ' Editing List Of Comparisons '

        // Comparison Of Products
        public void ComparisonOfProducts(int IDOfExternalProduct, int IDOfProduct, string NameOfProduct)
        {
            //
            // Checking Data
            //
            bool CorrectData = false;
            //
            if ((IDOfExternalProduct >= 0) && 
                (IDOfProduct > 0) && 
                (NameOfProduct.Length > 0))
            { CorrectData = true; }
            //
            // Getting Comparison For Changing
            //
            DataRow ComparisonForChanging = null;
            //
            if (CorrectData)
                ComparisonForChanging = ListOfComparisons.Rows.Find(IDOfExternalProduct);
            //
            // Processing Of Comparison
            //
            if (ComparisonForChanging != null)
            {
                //
                // Updating List Of Comparisons
                //
                ComparisonForChanging["IDOfProduct"] = IDOfProduct;
                ComparisonForChanging["NameOfProduct"] = (NameOfProduct.Length == 0) ? " " : NameOfProduct;
                //
                ComparisonForChanging.AcceptChanges();
                //
                // Creating List Of Comparisons For Searching
                //
                CreatingListOfComparisonsForSearching();
                //
                // Creating Text Of Updating Comparison (In DataBase)
                //
                string TextOfUpdatingComparison = 
                    String.Format(
                    "UPDATE Comparisons SET IDOfProduct = {1}, ExistsInPriceList = 0, Compare = 1 WHERE (IDOfExternalProduct = {0});", 
                    IDOfExternalProduct, IDOfProduct);
                //
                // Executing Command
                //
                ExecutingCommand(TextOfUpdatingComparison);
            }
        }

        // Removing Comparison Of Products
        public void RemovingComparisonOfProducts(int IDOfExternalProduct)
        {
            //
            // Checking Data
            //
            bool CorrectData = false;
            //
            if (IDOfExternalProduct >= 0)
            { CorrectData = true; }
            //
            // Getting Comparison For Changing
            //
            DataRow ComparisonForChanging = null;
            //
            if (CorrectData)
                ComparisonForChanging = ListOfComparisons.Rows.Find(IDOfExternalProduct);
            //
            // Processing Of Comparison
            //
            if (ComparisonForChanging != null)
            {
                //
                // Updating List Of Comparisons
                //
                ComparisonForChanging["IDOfProduct"] = 0;
                ComparisonForChanging["NameOfProduct"] = "";
                //
                ComparisonForChanging.AcceptChanges();
                //
                // Creating List Of Comparisons For Searching
                //
                CreatingListOfComparisonsForSearching();
                //
                // Creating Text Of Updating Comparison (In DataBase)
                //
                string TextOfUpdatingComparison = 
                    String.Format(
                    "UPDATE Comparisons SET IDOfProduct = 0, Compare = 0 WHERE (IDOfExternalProduct = {0});", 
                    IDOfExternalProduct);
                //
                // Executing Command
                //
                ExecutingCommand(TextOfUpdatingComparison);
            }
        }

        // Removing All Comparison Of Products 
        public void RemovingAllComparisonOfProducts()
        {
            //
            // Updating List Of Comparisons
            //
            foreach (DataRow CurrentComparison in ListOfComparisons.Rows)
            {
                CurrentComparison["IDOfProduct"] = 0;
                CurrentComparison["NameOfProduct"] = "";
            }
            //
            ListOfComparisons.AcceptChanges();
            //
            // Creating List Of Comparisons For Searching
            //
            CreatingListOfComparisonsForSearching();
            //
            // Creating Text Of Updating Comparison (In DataBase)
            //
            string TextOfUpdatingComparison = 
                "UPDATE Comparisons SET IDOfProduct = 0, Compare = 0;";
            //
            // Executing Command
            //
            ExecutingCommand(TextOfUpdatingComparison);
        }

        // Importing Of Existing In PriceList IDs
        // Importing IDs Of Products Of PriceList
        // List Of IDs Of Products
        public void ImportingOfExistingInPriceListIDs(ArrayList ListOfExistingIDs)
        {
            //
            // Checking Data
            //
            bool CorrectData = false;
            //
            if (ListOfExistingIDs.Count > 0)
            {
                //
                CorrectData = true;
                //
                for (int i = 0; i < ListOfExistingIDs.Count; i++)
                    if (ListOfExistingIDs[i].GetType() != typeof(int))
                        CorrectData = false;
            }
            //
            // Processing Of Comparisons
            //
            if (CorrectData)
            {
                //
                // Creating Command Of Importing
                //
                MySqlCommand CommandOfImporting = new MySqlCommand("", ConnectionToBase);
                CommandOfImporting.CommandText = "pharm66.ImportingInComparisonsOfExistsIDs";
                //                                ImportingInComparisonsOfExistsIDs
                CommandOfImporting.CommandType = CommandType.StoredProcedure;
                //
                // Addition Of Parameter Of ID
                //
                CommandOfImporting.Parameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32));
                //
                // Importing Of IDs
                //
                try
                {
                    //
                    CommandOfImporting.Connection.Open();
                    //
                    for (int i = 0; i < ListOfExistingIDs.Count; i++)
                    {
                        //
                        CommandOfImporting.Parameters["@ID"].Value = (int)ListOfExistingIDs[i];
                        //
                        CommandOfImporting.ExecuteScalar();
                    }
                    //
                    CommandOfImporting.Connection.Close();
                }
                catch (Exception E)
                {
                    //
                    try { CommandOfImporting.Connection.Close(); }
                    catch (Exception E2)
                    {
                        RecordingInLogFile(
                          String.Format("ERROR Ошибка при закрытии подключения ManagementOfComparisons: {0}", E2.Message));
                    }
                    //
                    RecordingInLogFile(
                        String.Format(
                        "ERROR Ошибка при выполнении импортирования отметки наличия в Comparisons: {0}", E.Message));
                }
            }
        }

        //Auto Selection (Null)
        public DataTable AutoSelection(
            PharmaceuticalInformation.Reading.ReadingOfData ReadingOfData, 
            string NameOfExternalProduct)
        {
            //
            DataTable FindResults = new DataTable("FindResults");
            //
            // Return
            //
            return FindResults;
        }

        #endregion


        #region ' Editing List Of Products Of Drugstore '

        // Addition Of Product Of Drugstore
        // Empty
        public void AdditionOfProductOfDrugstore(int IDOfExternalProduct, string NameOfExternalProduct)
        {
            //
        }

        // Deleting Product Of Drugstore
        // Empty
        public void DeletingProductOfDrugstore(int IDOfExternalProduct)
        {
            //
        }

        // Importing Products Of Drugstore
        public void ImportingProductsOfDrugstore(DataTable ListOfImportingProducts)
        {
            //
            // Checking Data
            //
            bool ListOfImportingProductsIsCorrect = false;
            //
            if (ListOfImportingProducts != null)
            {
                //
                ListOfImportingProducts.AcceptChanges();
                //
                if (ListOfImportingProducts.Rows.Count > 0)
                    ListOfImportingProductsIsCorrect = true;
                //
                if (!ListOfImportingProducts.Columns.Contains("ID") || 
                    !ListOfImportingProducts.Columns.Contains("Name"))
                    ListOfImportingProductsIsCorrect = false;
            }
            //
            // Importing List
            //
            if (ListOfImportingProductsIsCorrect)
            {
                //
                // Creating Command Of Importing
                //
                MySqlCommand CommandOfImporting = new MySqlCommand("", ConnectionToBase);
                CommandOfImporting.CommandText = "pharm66.ImportingInComparisonsOfProducts";
                CommandOfImporting.CommandType = CommandType.StoredProcedure;
                //
                // Addition Of Parameter Of ID
                //
                CommandOfImporting.Parameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32));
                //
                // Addition Of Parameter Of Name
                //
                CommandOfImporting.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.VarChar));
                //
                // Importing Of Products
                //
                try
                {
                    //
                    CommandOfImporting.Connection.Open();
                    //
                    foreach (DataRow CurrentProduct in ListOfImportingProducts.Rows)
                    {
                        //
                        CommandOfImporting.Parameters["@ID"].Value = (int)CurrentProduct["ID"];
                        CommandOfImporting.Parameters["@Name"].Value = (string)CurrentProduct["Name"];
                        //
                        CommandOfImporting.ExecuteScalar();
                    }
                    //
                    CommandOfImporting.Connection.Close();
                }
                catch (Exception E)
                {
                    //
                    try { CommandOfImporting.Connection.Close(); }
                    catch (Exception E2)
                    {
                        RecordingInLogFile(
                          String.Format("ERROR Ошибка при закрытии подключения ManagementOfComparisons: {0}", E2.Message));
                    }
                    //
                    RecordingInLogFile(
                        String.Format("ERROR Ошибка при выполнении импортировании продуктов в Comparisons: {0}", E.Message));
                }
                //
                // Refreshing List Of Comparisons
                //
                RefreshingListOfComparisons();
            }
        }

        // Deleting All Products Of Drugstore
        public void DeletingAllProductsOfDrugstore()
        {
            //
            // Creating Text Of Updating Comparison (In DataBase)
            //
            string TextOfUpdatingComparison = 
                "DELETE FROM Comparisons WHERE (IDOfProduct = 0);" + 
                "UPDATE Comparisons SET NameOfExternalProduct = '';";
            //
            // Executing Command
            //
            ExecutingCommand(TextOfUpdatingComparison);
            //
            // Refreshing List Of Comparisons
            //
            RefreshingListOfComparisons();
        }

        #endregion


        #region ' Converting And Writing List Of Products Of Drugstore '

        // Creating List Of Products
        private DataTable CreatingListOfProducts()
        {
            //
            // Creating ListOfProducts
            //
            DataTable ListOfProducts = new DataTable("Products");
            //
            // Addition Of Columns
            //
            ListOfProducts.Columns.Add("ID", typeof(int));
            ListOfProducts.Columns.Add("Name", typeof(string));
            //
            // Creating Primary Key
            //
            ListOfProducts.PrimaryKey = new DataColumn[] { ListOfProducts.Columns[0] };
            //
            // Return
            //
            return ListOfProducts;
        }

        // Exporting List Of Products In XML
        public void ExportingListOfProductsInXML(
            string PathToExportedFile, 
            DataTable ListOfProductsForExporting)
        {
            //
            // Checking Of Conditions 
            //
            bool SanctionOfWriting = true;
            //
            if ((PathToExportedFile == null) || (PathToExportedFile == ""))
                SanctionOfWriting = false;
            else if (PathToExportedFile.Length == 0)
                SanctionOfWriting = false;
            //
            if (ListOfProductsForExporting == null)
                SanctionOfWriting = false;
            else
                if ((ListOfProductsForExporting.Rows.Count == 0) || (ListOfProductsForExporting.Columns.Count == 0))
                    SanctionOfWriting = false;
            //
            // Writing In XML File
            //
            if (SanctionOfWriting)
            {
                //
                // Settings Of Writing
                //
                System.Xml.XmlWriterSettings Settings = new System.Xml.XmlWriterSettings();
                Settings.Indent = true;
                Settings.NewLineOnAttributes = true;
                Settings.Encoding = Encoding.Default;
                // Starting
                //
                // Initializing Writing (XmlWriter)
                //
                System.Xml.XmlWriter Writing = System.Xml.XmlWriter.Create(PathToExportedFile, Settings);
                //
                Writing.WriteStartDocument();
                // Writing Document (Документ)
                Writing.WriteStartElement("Документ");
                // Writing Element (СписокПрепаратов)
                Writing.WriteStartElement("СписокПрепаратов");
                //
                DateTime DateOfUnloading = DateTime.Now;
                Writing.WriteAttributeString("ДатаВыгрузки", DateOfUnloading.ToString());
                // Writing Products
                foreach (DataRow CurrentRow in ListOfProductsForExporting.Rows)
                {
                    Writing.WriteStartElement("Препарат");
                    Writing.WriteAttributeString("Код", CurrentRow["ID"].ToString());
                    Writing.WriteString(CurrentRow["Name"].ToString());
                    Writing.WriteEndElement();
                }
                // Ending
                Writing.WriteEndElement();  // (Документ)
                Writing.WriteEndDocument(); // (СписокПрепаратов)
                // Closing
                //Writing.Flush();
                Writing.Close();
            }
        }

        // Converting Of Drug List
        public DataTable ConvertingOfListOfProductsOfDrugstore(string PathToListOfProductsOfDrugstore)
        {
            //
            DataTable ListOfProducts = CreatingListOfProducts();
            //
            if (System.IO.File.Exists(PathToListOfProductsOfDrugstore))
            {
                //
                if (System.IO.Path.GetExtension(PathToListOfProductsOfDrugstore).ToLower() == ".xml")
                    ListOfProducts = ConvertingListOfXML(PathToListOfProductsOfDrugstore);
                //
                if (System.IO.Path.GetExtension(PathToListOfProductsOfDrugstore).ToLower() == ".txt")
                    ListOfProducts = ConvertingListOfTXT(PathToListOfProductsOfDrugstore);
            }
            //
            // Return
            //
            return ListOfProducts;
        }

        // Converting List Of TXT
        private DataTable ConvertingListOfTXT(string PathToFileOfList)
        {
            //
            // Reading File
            //
            string TextOfDrugList = "";
            //
            System.IO.FileStream FS = new System.IO.FileStream(PathToFileOfList, System.IO.FileMode.Open);
            System.IO.StreamReader SR = new System.IO.StreamReader(FS, Encoding.Default);
            //
            TextOfDrugList = SR.ReadToEnd();
            //
            SR.Close();
            FS.Close();
            FS.Dispose();
            //
            // Converting Data
            //
            DataTable ListOfDrugList = CreatingListOfProducts();
            //
            string[] LinesOfDrugList = TextOfDrugList.Split('\n');
            //
            if (LinesOfDrugList.Length > 0)
            {
                //
                int IndexOfResult = 0, CountOfTab = 0;
                //
                while (IndexOfResult > -1)
                {
                    IndexOfResult = LinesOfDrugList[0].IndexOf('\t', ++IndexOfResult);
                    CountOfTab++;
                }
                //
                if (CountOfTab > 2)
                {
                    for (int i = 0; i < LinesOfDrugList.GetUpperBound(0); i++)
                    {
                        string[] SubLines = LinesOfDrugList[i].Split('\t');
                        if (SubLines.Length > 2)
                            if ((SubLines[0] != "") && (SubLines[1] != ""))
                            {
                                //
                                DataRow NewRow = ListOfDrugList.NewRow();
                                //
                                NewRow["ID"] = Convert.ToInt32(SubLines[0]);
                                NewRow["Name"] = SubLines[1];
                                //
                                ListOfDrugList.Rows.Add(NewRow);
                            }
                    }
                }
            }
            /*
            if (LinesOfDrugList.Length > 0)
            {
                //
                int IndexOfResult = 0, CountOfTab = 0;
                //
                while (IndexOfResult > -1)
                {
                    IndexOfResult = LinesOfDrugList[0].IndexOf(',', ++IndexOfResult);
                    CountOfTab++;
                }
                //
                if (CountOfTab >= 1)
                {
                    for (int i = 0; i < LinesOfDrugList.GetUpperBound(0); i++)
                    {
                        string[] SubLines = LinesOfDrugList[i].Split(',');
                        if (SubLines.Length >= 2)
                            if ((SubLines[0] != "") && (SubLines[1] != ""))
                            {
                                //
                                DataRow NewRow = ListOfDrugList.NewRow();
                                //
                                NewRow["ID"] = Convert.ToInt32(SubLines[0]);
                                NewRow["Name"] = SubLines[1];
                                //
                                ListOfDrugList.Rows.Add(NewRow);
                            }
                    }
                }
            }
            */
            //
            // Return
            //
            return ListOfDrugList;
        }

        // Converting List Of XML
        private DataTable ConvertingListOfXML(string PathToFileOfList)
        {
            //
            // Initializing Reading (XmlReader)
            //
            System.Xml.XmlReader Reading = System.Xml.XmlReader.Create(PathToFileOfList);
            DataTable TableToFilling = CreatingListOfProducts();
            //
            // Reading Data
            //
            //Товар
            while (Reading.Read())
                if (Reading.NodeType == System.Xml.XmlNodeType.Element)
                    if (Reading.Name == "Препарат")
                    {
                        string IDOfProduct_1 = Reading.GetAttribute("Код");
                        //string IDOfProduct_2 = Reading.GetAttribute("КодСвязи");
                        string NameOfProduct = Reading.ReadString();
                        //if ((IDOfProduct_1 != "") && (IDOfProduct_2 != "") && (NameOfProduct != ""))
                        if ((IDOfProduct_1 != "") && (NameOfProduct != ""))
                        {
                            int ID = Convert.ToInt32(IDOfProduct_1);
                            if (!TableToFilling.Rows.Contains(ID))
                            {
                                DataRow NewProduct = TableToFilling.NewRow();
                                NewProduct["ID"] = ID;
                                //NewProduct["ID_2"] = Convert.ToInt32(IDOfProduct_2);
                                NewProduct["Name"] = NameOfProduct;
                                TableToFilling.Rows.Add(NewProduct);
                            }
                            //Console.WriteLine("ID_1={0} ID_2={1} Name={2}", IDOfProduct_1, IDOfProduct_2, NameOfProduct);
                        }
                    }
            //
            // Closing Reading
            //
            Reading.Close();
            //
            // Sorting
            //
            TableToFilling.DefaultView.Sort = "ID ASC";
            TableToFilling = TableToFilling.DefaultView.ToTable();
            // Help
            // Reading.AttributeCount       // Возвращает количество атрибутов элемента.
            // Reading.GetAttribute("Str")  // Возвращает атрибут элемента.
            // Reading.ReadString()         // Читает строку в элементе.
            // Reading.ReadElementString()  // Переходит на следующий элемент и читает строку.
            //
            // Return
            //
            return TableToFilling;
        }

        #endregion


        #region ' Transfer Of Comparisons '

        // Exporting Comparisons
        // Empty
        public DataTable ExportingComparisons()
        {
            //
            DataTable ExportedComparisons = new DataTable("ExportedComparisons");
            //
            // Return
            //
            return ExportedComparisons;
        }

        // Importing Comparisons
        // Empty
        public void ImportingComparisons(DataTable ImportedComparisons)
        {
            //
        }

        #endregion


        #region ' Getting List Of Comparisons '

        // Getting List Of Comparisons
        public DataTable GettingListOfComparisons()
        {
            //
            // Return
            //
            return GettingListOfComparisons(
                "", PharmaceuticalInformation.Reading.ReadingOfData.TypesOfFiltration.NotSearch, 
                "", PharmaceuticalInformation.Reading.ReadingOfData.TypesOfFiltration.NotSearch, 
                false, false, 
                false, false, 
                false, false);
        }

        // Getting List Of Comparisons
        public DataTable GettingListOfComparisons(
            string NameOfExternalProduct, Reading.ReadingOfData.TypesOfFiltration TypeNameOfExternalProduct, 
            string NameOfProduct, Reading.ReadingOfData.TypesOfFiltration TypeNameOfProduct)
        {
            //
            // Return
            //
            return GettingListOfComparisons(
                NameOfExternalProduct, TypeNameOfExternalProduct, 
                NameOfProduct, TypeNameOfProduct, 
                false, false, 
                false, false, 
                false, false);
        }

        // Getting List Of Comparisons
        public DataTable GettingListOfComparisons(
            string NameOfExternalProduct, Reading.ReadingOfData.TypesOfFiltration TypeNameOfExternalProduct, 
            string NameOfProduct, Reading.ReadingOfData.TypesOfFiltration TypeNameOfProduct, 
            bool Associated, bool EnableFilteringOfAssociated, 
            bool ExistsInPriceList, bool EnableFilteringOfExistsInPriceList, 
            bool Compare, bool EnableFilteringOfCompare)
        {
            //
            // Searching
            //
            DataTable ListOfComparisons = new DataTable("ListOfComparisons");
            //
            // Searching On Name
            //
            //Console.WriteLine("A");
            //
            if (NameOfExternalProduct == "")
                TypeNameOfExternalProduct = Reading.ReadingOfData.TypesOfFiltration.NotSearch;
            //
            ListOfComparisons = 
                SearchingProducts(
                this.ListOfComparisonsForSearching, "N", NameOfExternalProduct, TypeNameOfExternalProduct);
            //
            // Searching On MNN
            //
            //Console.WriteLine("B");
            //
            if (ListOfComparisons.Rows.Count > 0)
            {
                ArrayList ListOfComparisonsForSearching = new ArrayList();
                //
                // Renaming Columns
                //

                ListOfComparisons.Columns[1].ColumnName = "Name";
                ListOfComparisons.Columns[3].ColumnName = "MNN";
                //
                // Creating List Of Comparisons For Searching
                //
                ListOfComparisonsForSearching = CreatingListOfProducts(ListOfComparisons);
                //
                // Renaming Columns
                //
                ListOfComparisons.Columns[1].ColumnName = "NameOfExternalProduct";
                ListOfComparisons.Columns[3].ColumnName = "NameOfProduct";

                //
                if (NameOfProduct == "")
                    TypeNameOfProduct = Reading.ReadingOfData.TypesOfFiltration.NotSearch;
                //
                DataTable NewListOfComparisons;
                //
                NewListOfComparisons =
                    SearchingProducts(
                    ListOfComparisonsForSearching, "M", NameOfProduct, TypeNameOfProduct);
                //
                // Clearing List Of Comparisons
                //
                ListOfComparisons.Clear();
                ListOfComparisons.Dispose();
                //
                ListOfComparisons = NewListOfComparisons;
            }
            //
            // Filtering
            //
            DataView FilteringComparisons = new DataView();
            //
            // Filtering On Associated
            //
            if (ListOfComparisons.Columns.Contains("IDOfProduct") && (EnableFilteringOfAssociated))
            {
                FilteringComparisons.Table = ListOfComparisons;
                //
                FilteringComparisons.RowFilter = String.Format("IDOfProduct {0} 0", (Associated) ? ">" : "=");
                //
                ListOfComparisons = FilteringComparisons.ToTable();
            }
            //
            // Filtering On ExistsInPriceList
            //
            if (ListOfComparisons.Columns.Contains("ExistsInPriceList") && (EnableFilteringOfExistsInPriceList))
            {
                FilteringComparisons.Table = ListOfComparisons;
                //
                FilteringComparisons.RowFilter = String.Format("ExistsInPriceList = {0}", (ExistsInPriceList) ? "1" : "0");
                //
                ListOfComparisons = FilteringComparisons.ToTable();
            }
            //
            // Filtering On Compare
            //
            if (ListOfComparisons.Columns.Contains("Compare") && (EnableFilteringOfCompare))
            {
                FilteringComparisons.Table = ListOfComparisons;
                //
                FilteringComparisons.RowFilter = String.Format("Compare = {0}", (Compare) ? "1" : "0");
                //
                ListOfComparisons = FilteringComparisons.ToTable();
            }
            //
            // !!!
            //
            ListOfComparisons.AcceptChanges();
            //
            // Creating Primary Key
            //
            ListOfComparisons.PrimaryKey = 
                new DataColumn[] { ListOfComparisons.Columns["IDOfExternalProduct"] };
            //
            // Return
            //
            return ListOfComparisons;
        }

        // Getting Count Of Associated
        public int GettingCountOfAssociated
        {
            get
            {
                //
                // Creating Text Of Selection Of Count Of Associated
                //
                string TextOfSelectionOfCount = 
                    "SELECT COUNT(*) FROM Comparisons WHERE (IDOfProduct > 0)";
                //
                // Executing Command
                //
                object ValueOfCount = ExecutingCommand(TextOfSelectionOfCount);
                //
                // Converting Value
                //
                int CountOfAssociated = -1;
                //
                try { CountOfAssociated = Convert.ToInt32(ValueOfCount); }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при конвертации количества строк : {0}", E.Message)); }
                //
                // Return
                //
                return CountOfAssociated;
            }
        }

        // Getting Count Of Associated And Compare
        public int GettingCountOfAssociatedAndCompare
        {
            get
            {
                //
                // Creating Text Of Selection Of Count Of Associated And Compare
                //
                string TextOfSelectionOfCount = 
                    "SELECT COUNT(*) FROM Comparisons WHERE ((IDOfProduct > 0) AND (Compare = 1))";
                //
                // Executing Command
                //
                object ValueOfCount = ExecutingCommand(TextOfSelectionOfCount);
                //
                // Converting Value
                //
                int CountOfAssociatedAndCompare = -1;
                //
                try { CountOfAssociatedAndCompare = Convert.ToInt32(ValueOfCount); }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при конвертации количества строк : {0}", E.Message)); }
                //
                // Return
                //
                return CountOfAssociatedAndCompare;
            }
        }

        #endregion


        #region ' Converting IDs Of Products '

        // Converting IDs Of Products
        public void ConvertingIDsOfProducts(DataTable ListOfIDsOfProducts, string NameOfColumn)
        {
            //
            // Checking List Of IDs Of Products
            //
            bool ListOfIDsOfProductsIsCorrect = false;
            //
            if (ListOfIDsOfProducts != null)
            {
                //
                ListOfIDsOfProducts.AcceptChanges();
                //
                if (ListOfIDsOfProducts.Rows.Count > 0)
                    ListOfIDsOfProductsIsCorrect = true;
                //
                if (NameOfColumn.Length == 0)
                    ListOfIDsOfProductsIsCorrect = false;
            }
            //
            // Checking Of Existence Of Column
            //
            bool ExistenceOfColumn = false;
            //
            if (ListOfIDsOfProductsIsCorrect)
                if (ListOfIDsOfProducts.Columns.Contains(NameOfColumn))
                    ExistenceOfColumn = true;
            //
            // Checking Of Existence Of Comparisons
            //
            bool ExistenceOfComparisons = false;
            //
            if (ExistenceOfColumn)
            {
                //
                int CountOfComparisons = this.GettingCountOfAssociatedAndCompare;
                //
                if (CountOfComparisons > 0)
                    ExistenceOfComparisons = true;
            }
            //
            // Creating List Of Changes Of IDs Of Products
            //
            DataTable ListOfChangesOfIDsOfProducts = new DataTable("ListOfComparisons");
            //
            if (ExistenceOfComparisons)
            {
                //
                // Filling List Of Comparisons With Data
                //
                string TextOfCommandOfFilling = 
                    "SELECT " + 
                    "C.IDOfExternalProduct, C.IDOfProduct " + 
                    "FROM " + 
                    "Comparisons AS C " + 
                    "WHERE (C.Compare = 1);";
                //
                ListOfChangesOfIDsOfProducts = 
                    FillingWithDataOfTable(TextOfCommandOfFilling, "ListOfComparisons", true);
                //
                //ListOfChangesOfIDsOfProducts.PrimaryKey = new DataColumn[1] { Comparisons.Columns["IDOfExternalProduct"] };
            }
            //
            // Changing Of IDs Of Products In List Of Products
            //
            //if (ExistenceOfComparisons)
            if (ListOfChangesOfIDsOfProducts.Rows.Count > 0)
            {
                //
                // Recording In Log File
                //
                RecordingInLogFile("Usage Of List Of Comparisons");
                //
                // Changing Of IDs
                //
                ArrayList ListNotConvertingIDs = new ArrayList();
                int CountOfConvertingElements = 0, CountOfNotConvertingElements = 0;
                //
                foreach (DataRow CurrentElementOfList in ListOfIDsOfProducts.Rows)
                {
                    //
                    // Converting ID Of Current Element
                    //
                    int CurrentIDOfProduct = -1;
                    //
                    try { CurrentIDOfProduct = Convert.ToInt32(CurrentElementOfList[NameOfColumn]); }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при конвертации ID: {0}", E.Message)); }
                    //
                    // !!!
                    //
                    if (CurrentIDOfProduct > -1)
                    {
                        //
                        // Getting New ID Of Product
                        //
                        int NewIDOfProduct = 0;
                        //
                        DataRow ChangeOfID = ListOfChangesOfIDsOfProducts.Rows.Find(CurrentIDOfProduct);
                        if (ChangeOfID != null)
                            NewIDOfProduct = (int) ChangeOfID["IDOfProduct"];
                        //
                        // !!!
                        //
                        if (NewIDOfProduct > 0)
                        {
                            //
                            CurrentElementOfList[NameOfColumn] = NewIDOfProduct;
                            //
                            CountOfConvertingElements++;
                        }
                        else
                        {
                            //
                            ListNotConvertingIDs.Add(CurrentIDOfProduct);
                            //
                            CurrentElementOfList.Delete();
                            //
                            CountOfNotConvertingElements++;
                        }
                    }
                }
                //
                // Importing Of Existing In PriceList Of IDs
                //
                ImportingOfExistingInPriceListIDs(ListNotConvertingIDs);
                //
                // Recording In Log File
                //
                RecordingInLogFile(
                    String.Format("Count Of Converting Elements: {0}, Count Of Not Converting Elements: {1}", 
                    CountOfConvertingElements, CountOfNotConvertingElements));
            }
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

        // Executing Command
        private object ExecutingCommand(string TextOfCommand)
        {
            //
            object ReturnedValue = null;
            //
            // Text Of Command For Executing
            //
            CommandForExecuting.CommandText = TextOfCommand;
            //
            // Executing
            //
            try
            {
                //
                CommandForExecuting.Connection.Open();
                //
                ReturnedValue = CommandForExecuting.ExecuteScalar();
                //
                CommandForExecuting.Connection.Close();
            }
            catch (Exception E)
            {
                //
                try { CommandForExecuting.Connection.Close(); }
                catch (Exception E2)
                { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии подключения ManagementOfComparisons: {0}", E2.Message)); }
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при выполнении команды на таблицу Comparisons: ", E.Message));
            }
            //
            // Clearing Text Of Command
            //
            CommandForExecuting.CommandText = "";
            //
            // Return
            //
            return ReturnedValue;
        }

        #endregion


        #region ' Archive '

        /*
        //
            // Initializing Of Deleting
            //
            MySqlCommand CommandOfDeleting = new MySqlCommand("DELETE FROM client_ref;", ConnectionToBase);
            //
            // Deleting Products
            //
            try
            {
                CommandOfDeleting.Connection.Open();
                CommandOfDeleting.ExecuteNonQuery();
                CommandOfDeleting.Connection.Close();
            }
            catch (Exception E)
            {
                CommandOfDeleting.Connection.Close();
                ReturningMessageAboutError("Ошибка пир очистке таблицы client_ref", E, false);
            }
        */
        /*
        //
            // Initializing UpdatingProducts
            //
            MySqlDataAdapter UpdatingProducts = new MySqlDataAdapter();
            //
            // Creating Command Of Inserting
            //
            MySqlCommand CommandOfInserting =
                new MySqlCommand("INSERT INTO client_ref(Id_client_product, Name_full) VALUES(@1, @2);",
                    ConnectionToBase);
            //
            // Addition Of Parameters
            //
            // Parameter 1
            //
            MySqlParameter Parameter_1 = new MySqlParameter();
            Parameter_1.ParameterName = "@1";
            Parameter_1.MySqlDbType = MySqlDbType.Int32;
            Parameter_1.SourceColumn = "ID";
            CommandOfInserting.Parameters.Add(Parameter_1);
            //
            // Parameter 2
            //
            MySqlParameter Parameter_2 = new MySqlParameter();
            Parameter_2.ParameterName = "@2";
            Parameter_2.MySqlDbType = MySqlDbType.VarChar;
            Parameter_2.SourceColumn = "Name";
            CommandOfInserting.Parameters.Add(Parameter_2);
            UpdatingProducts.InsertCommand = CommandOfInserting;
            //
            // Processing Of Status Of Rows
            //
            InsertedProducts.AcceptChanges();
            //
            foreach (DataRow CurrentRow in InsertedProducts.Rows)
                if (CurrentRow.RowState == DataRowState.Unchanged)
                    CurrentRow.SetAdded();
            //
            // Updating
            //
            UpdatingProducts.Update(InsertedProducts);
        */
        /*
            if (dGV_Comparisons.Columns.Contains("IDOfProductOfInformation"))
                foreach (DataRow CurrentComparsion in ((DataTable)dGV_Comparisons.DataSource).Rows)
                    if (((UInt32)CurrentComparsion["IDOfProductOfInformation"]) != 0)
                        CountOfComparison++;
            */
        //
        // Initializing Comparisons
        //
        //InitializingComparisons();
        /*
        // Initializing Comparisons
        private void InitializingComparisons()
        {
            //
            // Initializing UpdatingOfListOfComparisons
            //
            UpdatingOfListOfComparisons = new MySqlDataAdapter();
            //
            // Creating Command Of Selection
            //
            MySqlCommand CommandOfSelection = new MySqlCommand(
                "SELECT " + 
                "C.IDOfExternalProduct, C.NameOfExternalProduct, " + 
                "C.IDOfProduct, P.Name_full AS 'NameOfProduct', " + 
                "C.ExistsInPriceList, C.Compare " + 
                "FROM " + 
                "Pharm66.Comparisons AS C " + 
                "LEFT JOIN " + 
                "Pharm66.Product AS P " + 
                "ON C.IDOfProduct = P.ID_Product " + 
                "ORDER BY NameOfExternalProduct;", ConnectionToBase);

            //
            // Creating Command Of Inserting
            //
            MySqlCommand CommandOfInserting = new MySqlCommand(
                "INSERT INTO matching(Id_client_product, Id_product) VALUES(@P1, @P2);", ConnectionToBase);
            //
            // Creating And Addition Of Parameters
            //
            object[,] ParametersOfInserting = new object[2, 3] { 
            { "@P1", "ID_PC" ,       MySqlDbType.UInt32 },  { "@P2", "ID_PR",     MySqlDbType.UInt32 } };
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
                "UPDATE matching " + 
                "SET Id_product = @P2 " +
                "WHERE (Id_client_product = @P1);", ConnectionToBase);
            //
            // Creating And Addition Of Parameters
            //
            object[,] ParametersOfUpdating = new object[2, 3]
            { { "@P1", "ID_PC",      MySqlDbType.UInt32 }, { "@P2", "ID_PR",     MySqlDbType.UInt32 } };
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
                "DELETE FROM matching WHERE (Id_client_product = @P1);", ConnectionToBase);
            //
            // Creating And Addition Of Parameters
            //
            object[,] ParametersOfDeleting = new object[2, 3] { 
            { "@P1", "ID_PC",        MySqlDbType.UInt32 }, { "@P2", "ID_PR",     MySqlDbType.UInt32 }};
            for (int i = 0; i <= ParametersOfDeleting.GetUpperBound(0); i++)
            {
                MySqlParameter NewParameter = new MySqlParameter();
                NewParameter.ParameterName = ParametersOfDeleting[i, 0].ToString();
                NewParameter.SourceColumn = ParametersOfDeleting[i, 1].ToString();
                NewParameter.MySqlDbType = (MySqlDbType)ParametersOfDeleting[i, 2];
                //NewParameter.Size = 0;
                CommandOfDeleting.Parameters.Add(NewParameter);
            }
            //
            // Assignment Of Commands
            //
            UpdatingOfListOfComparisons.SelectCommand = CommandOfSelection;
            UpdatingOfListOfComparisons.InsertCommand = CommandOfInserting;
            UpdatingOfListOfComparisons.UpdateCommand = CommandOfUpdating;
            UpdatingOfListOfComparisons.DeleteCommand = CommandOfDeleting;
        }
        */
        //
        //private MySqlDataAdapter UpdatingOfListOfComparisons;
        /*
        public void RemovingAllComparisonOfProducts()
        {
            //
            foreach (DataRow CurrentComparison in ListOfComparisons.Rows)
            {
                CurrentComparison["ID_PR"] = 0;
                CurrentComparison["InformationName"] = "";
            }
            //
            ListOfComparisons.AcceptChanges();
            //
            string TextOfCommand = UpdatingOfListOfComparisons.DeleteCommand.CommandText;
            //
            try
            {
                //
                UpdatingOfListOfComparisons.DeleteCommand.CommandText = "DELETE FROM matching";
                //
                UpdatingOfListOfComparisons.DeleteCommand.Connection.Open();
                UpdatingOfListOfComparisons.DeleteCommand.ExecuteScalar();
                UpdatingOfListOfComparisons.DeleteCommand.Connection.Close();
            }
            catch (Exception E)
            {
                UpdatingOfListOfComparisons.DeleteCommand.Connection.Close();
                ReturningMessageAboutError("Ошибка при обновление таблицы Comparisons", E, false);
            }
            //
            UpdatingOfListOfComparisons.DeleteCommand.CommandText = TextOfCommand;
        }
        */

        /*
        // Removing Comparison Of Products
        public void RemovingComparison(int IDOfProductOfClient)
        {
            if ((IDOfProductOfClient > 0))
            {
                //
                // Searching Comparison
                //
                DataRow FoundComparison = ListOfComparisons.Rows.Find(IDOfProductOfClient);
                //
                if (FoundComparison != null)
                {
                    //
                    FoundComparison["ID_PR"] = 0;
                    FoundComparison["InformationName"] = "";
                    //
                    FoundComparison.AcceptChanges();
                    //
                    try
                    {
                        //
                        UpdatingOfListOfComparisons.DeleteCommand.Parameters["@P1"].Value = IDOfProductOfClient;
                        //
                        UpdatingOfListOfComparisons.DeleteCommand.Connection.Open();
                        UpdatingOfListOfComparisons.DeleteCommand.ExecuteScalar();
                        UpdatingOfListOfComparisons.DeleteCommand.Connection.Close();
                    }
                    catch (Exception E)
                    {
                        UpdatingOfListOfComparisons.DeleteCommand.Connection.Close();
                        ReturningMessageAboutError("Ошибка при обновление таблицы Comparisons", E, false);
                    }
                }
                else
                {  }
            }
        }
        */
        //int CurrentIDOfProduct = Convert.ToInt32(FoundComparison["ID_PR"]);
        /**/
        //
        /*if (CurrentIDOfProduct == 0)
            FoundComparison.SetAdded();
        else
            FoundComparison.SetModified();*/
        //
        //Console.WriteLine(FoundComparison.RowState);
        //
        // Updating
        //
        //ExecutedUpdatingOfComparisons();
        /*
        // Executed Updating Of PriceList
        private void ExecutedUpdatingOfComparisons()
        {
            //
            // Updating Of Data
            //
            try { UpdatingOfListOfComparisons.Update(ListOfComparisons); }
            catch (Exception E)
            { ReturningMessageAboutError("Ошибка при обновление таблицы Comparisons", E, false); }
            //
        }
        */
        // Comparison Of Products
        /*public void AppointOfComparison(
            int IDOfProductOfClient, int IDOfProductOfInformation, string NameOfProductOfInformation)
        {
            if ((IDOfProductOfClient > 0) && (IDOfProductOfInformation > 0) &&
                (NameOfProductOfInformation.Length > 0))
            {
                //
                // Searching Comparison
                //
                DataRow FoundComparison = ListOfComparisons.Rows.Find(IDOfProductOfClient);
                //
                if (FoundComparison != null)
                {
                    //
                    int CurrentIDOfProduct = Convert.ToInt32(FoundComparison["ID_PR"]);
                    //
                    FoundComparison["ID_PR"] = IDOfProductOfInformation;
                    FoundComparison["InformationName"] = NameOfProductOfInformation;
                    //
                    FoundComparison.AcceptChanges();
                    //
                    if (CurrentIDOfProduct == 0)
                        FoundComparison.SetAdded();
                    else
                        FoundComparison.SetModified();
                    //
                    //Console.WriteLine(FoundComparison.RowState);
                    //
                    // Updating
                    //
                    ExecutedUpdatingOfComparisons();
                }
                else
                { }
            }
        }*/
        //
        // Filling Comparisons
        //
        //FillingComparisons(true);
        //
        // Refreshing List Of Products
        //
        //RefreshingListOfProducts(true);
        // Filling With Data Of Table
        /*private void FillingWithDataOfTable2(
            DataTable FilledTable, string TableName, MySqlDataAdapter GettingData, bool CreatingSchema)
        {
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
            //
        }*/
        // Refresh Data
        /*public void RefreshData()
        {
            //
            FillingComparisons(false);
        }*/
        /*
                "SELECT ListOfNamesAndComparisons.*, ListOfProducts.Name_full AS 'NameOfProductOfInformation' FROM " + 
                "  ( " + 
                "    SELECT" + 
                "      ListOfNamesOfDrugstore.Id_client_product AS 'IDOfProductOfDrugstore', " + 
                "      ListOfNamesOfDrugstore.Name_full AS 'NameOfProductOfDrugstore', " + 
                "      ListOfComparisons.Id_Product AS 'IDOfProductOfInformation' " + 
                "    FROM " + 
                "      Pharm66.Client_Ref AS ListOfNamesOfDrugstore " + 
                "    LEFT JOIN" + 
                "      Pharm66.Matching AS ListOfComparisons " + 
                "    ON (ListOfNamesOfDrugstore.Id_client_product = ListOfComparisons.Id_client_product) " + 
                "  ) AS ListOfNamesAndComparisons " + 
                "LEFT JOIN " + 
                "  Pharm66.Product AS ListOfProducts " + 
                "ON (ListOfNamesAndComparisons.IDOfProductOfInformation = ListOfProducts.Id_Product) " + 
                "ORDER BY NameOfProductOfDrugstore;", 
            */
        /*"SELECT " +
            "CR.Id_client_product AS 'ID_PC', CR.Name_full AS 'ClientName', " +
            "MP.Id_Product AS 'ID_PR', MP.Name_full AS 'InformationName' " +
            "FROM " +
            "Client_Ref AS CR " +
            "LEFT JOIN " +
            "(SELECT MA.Id_client_product, P.Id_Product, P.Name_full FROM Matching AS MA, Product AS P " +
            "WHERE " +
            "(MA.Id_product = P.Id_Product)) AS MP " +
            "ON " +
            "(CR.Id_client_product = MP.Id_client_product);",
        */

        // Getting Comparisons
        /*public DataTable GettingComparisonsARH(
            string NameOfProductOfDrugstore, Reading.ReadingOfData.TypesOfFiltration TypeOfFiltrationDrugstore,
            string NameOfProductOfInformation, Reading.ReadingOfData.TypesOfFiltration TypeOfFiltrationInformation)
        {
            //
            DataTable ReturnComparisons = new DataTable();
            //
            // Filtering
            //
            // Creating Filtering
            //
            DataView FilteringComparisons = new DataView(ListOfComparisons);
            //
            //if (_EnabledFiltering)
            //
            ArrayList ListOfConditions = new ArrayList();
            //
            if (TypeOfFiltrationDrugstore != Reading.ReadingOfData.TypesOfFiltration.NotSearch)
                if (NameOfProductOfDrugstore.Length > 0)
                {
                    ListOfConditions.Add(
                        String.Format("(ClientName LIKE '{0}{1}%')",
                        TypeOfFiltrationDrugstore == Reading.ReadingOfData.TypesOfFiltration.InText ? "%" : "",
                        NameOfProductOfDrugstore));
                }
            //
            if (TypeOfFiltrationInformation != Reading.ReadingOfData.TypesOfFiltration.NotSearch)
                if (NameOfProductOfInformation.Length > 0)
                {
                    ListOfConditions.Add(
                        String.Format("(InformationName LIKE '{0}{1}%')",
                        TypeOfFiltrationInformation == Reading.ReadingOfData.TypesOfFiltration.InText ? "%" : "",
                        NameOfProductOfInformation));
                }
            //
            // Joining Strings Of Conditions In General String
            //
            if (ListOfConditions.Count > 0)
            {
                string[] StringsOfConditions = (string[])ListOfConditions.ToArray(typeof(string));
                string StringOfCondition = String.Join(" AND ", StringsOfConditions);
                //
                // Creating Command Of Selection
                //
                FilteringComparisons.RowFilter = StringOfCondition;
            }
            //
            // Creating Copy
            //
            ReturnComparisons = FilteringComparisons.ToTable();
            ReturnComparisons.PrimaryKey =
                new DataColumn[] { ReturnComparisons.Columns["IDOfProductOfDrugstore"] };
            //
            // Return
            //
            return ReturnComparisons;
        }*/
        //
        /*private DataTable ReadingProductsXML(string PathOfReadingDataXML)
        {
            // Reading
            DataTable Products = new DataTable();
            Products.ReadXmlSchema(PathOfReadingDataXML + ".xsd");
            Products.ReadXml(PathOfReadingDataXML + ".xml");
            // Return
            return Products;
        }*/

        //
        /*private void WritingProducts(string PathOfWritingData, DataTable TableOfProducts)
        {
            // Writing Data
            TableOfProducts.WriteXmlSchema(PathOfWritingData + ".xsd");
            TableOfProducts.WriteXml(PathOfWritingData + ".xml");
        }*/
        //
        //_EnabledFiltering = true;
        //
        /*_NameOfProductOfDrugstore = "";
        _NameOfProductOfInformation = "";
        //_NameOfProductOfDrugList = "";
        //
        _TypeOfFiltrationDrugstore = TypeOfFiltration.NoFiltrationString;
        _TypeOfFiltrationInformation = TypeOfFiltration.NoFiltrationString;*/
        //_TypeOfFiltrationDrugList = TypeOfFiltration.NoFiltrationString;

        //#region ' Settings Of Filtering '

        //
        //private bool _EnabledFiltering;
        //private string _NameOfProductOfDrugstore;
        /*private TypeOfFiltration _TypeOfFiltrationDrugstore;
        private string _NameOfProductOfInformation;
        private TypeOfFiltration _TypeOfFiltrationInformation;*/

        /*public bool EnabledFiltering
        {
            get { return _EnabledFiltering; }
            set { _EnabledFiltering = value; }
        }*/

        // Name Of Product Of Drugstore
        /*public string NameOfProductOfDrugstore
        {
            get { return _NameOfProductOfDrugstore; }
            set { if(value != null) _NameOfProductOfDrugstore = value; }
        }*/

        // Type Of Filtration Drugstore
        /*public TypeOfFiltration TypeOfFiltrationDrugstore
        {
            get { return _TypeOfFiltrationDrugstore; }
            set { _TypeOfFiltrationDrugstore = value; }
        }*/

        // Name Of Product Of Information
        /*public string NameOfProductOfInformation
        {
            get { return _NameOfProductOfInformation; }
            set { if (value != null) _NameOfProductOfInformation = value; }
        }*/

        // Type Of Filtration In formation
        /*public TypeOfFiltration TypeOfFiltrationInformation
        {
            get { return _TypeOfFiltrationInformation; }
            set { _TypeOfFiltrationInformation = value; }
        }*/

        //#endregion
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
        //#region ' Type Of Filtration '

        /*public enum TypeOfFiltration
        {
            InBeginning,
            InString,
            NoFiltrationString
        }*/

        //#endregion

        #endregion

    }
}