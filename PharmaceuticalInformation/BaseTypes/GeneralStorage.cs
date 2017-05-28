using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace PharmaceuticalInformation.BaseTypes
{
    public class GeneralStorage : BaseType
    {

        #region ' Fields '

        //protected static DataTable ListOfProducts;
        protected SortingOfList Sorting;

        #endregion

        #region ' Designer '

        public GeneralStorage()
            : base()
        {
            //
            Sorting = new SortingOfList();
        }

        public GeneralStorage(string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            Sorting = new SortingOfList();
        }

        #endregion

        #region ' Filtering List Of Products '

        #region ' Creating List Of Products '

        // Creating List Of Products
        protected ArrayList CreatingListOfProducts(DataTable Products)
        {
            //Console.WriteLine("CL");
            //
            // Initializing Of List Products
            //
            ArrayList ListOfProducts = new ArrayList();
            //
            // Filling Of List Products
            //
            foreach (DataRow CurrentProduct in Products.Rows)
            {
                if (!(CurrentProduct["Name"] is DBNull) && (CurrentProduct["Name"] != null) &&
                    (CurrentProduct["Name"].GetType() == typeof(string)))
                    if (((string)CurrentProduct["Name"]).Length > 0)
                    { ListOfProducts.Add(new ElementOfSorting(CurrentProduct)); }
            }
            //
            // Addition Of Symbols Of Marking
            //
            string CurrentChar = "";
            SortingOfList Sorting = new SortingOfList();
            //
            // Addition Of Marking Of Names
            //
            Sorting.TypeOfSorting = "N";
            ListOfProducts.Sort(Sorting);
            //
            for (int i = 0; i < ListOfProducts.Count; i++)
            {
                //
                string NewChar = ((ElementOfSorting)ListOfProducts[i]).Name[0].ToString();
                //
                if (CurrentChar != NewChar)
                {
                    //
                    CurrentChar = NewChar;
                    //
                    ListOfProducts.Insert(i, new ElementOfSorting(CurrentChar, "N"));
                }
            }
            //
            // Addition Of Marking Of MNN
            //
            Sorting.TypeOfSorting = "M";
            ListOfProducts.Sort(Sorting);
            //
            CurrentChar = "";
            //
            for (int i = 0; i < ListOfProducts.Count; i++)
            {
                //
                // !!!
                //
                string NewChar = "", MNN = ((ElementOfSorting)ListOfProducts[i]).MNN;
                if (MNN.Length > 1)
                    NewChar = MNN[0].ToString();
                //
                if (CurrentChar != NewChar)
                {
                    //
                    CurrentChar = NewChar;
                    //
                    ListOfProducts.Insert(i, new ElementOfSorting(CurrentChar, "M"));
                }
            }
            //
            // Sorting Of List Of Products
            //
            Sorting.TypeOfSorting = "N";
            ListOfProducts.Sort(Sorting);
            //
            // Return
            //
            return ListOfProducts;
        }

        // Creating List Of Products 02
        protected ArrayList CreatingListOfProducts02(
            DataTable ListOfProductsForCreating, TypeOfList TypeOfCreatingList)
        {
            //Console.WriteLine("CL");
            //
            // Initializing Of List Products
            //
            ArrayList ListOfProducts = new ArrayList();
            //
            // Filling Of List Products
            //
            foreach (DataRow CurrentProduct in ListOfProductsForCreating.Rows)
            {
                if (!(CurrentProduct["Name"] is DBNull) && (CurrentProduct["Name"] != null) &&
                    (CurrentProduct["Name"].GetType() == typeof(string)))
                    if (((string)CurrentProduct["Name"]).Length > 0)
                    { ListOfProducts.Add(new ElementOfSorting(CurrentProduct)); }
            }
            //
            // Addition Of Symbols Of Marking
            //
            string CurrentChar = "";
            SortingOfList Sorting = new SortingOfList();
            //
            // Addition Of Marking Of Names
            //
            Sorting.TypeOfSorting = "N";
            ListOfProducts.Sort(Sorting);
            //
            for (int i = 0; i < ListOfProducts.Count; i++)
            {
                //
                string NewChar = ((ElementOfSorting)ListOfProducts[i]).Name[0].ToString();
                //
                if (CurrentChar != NewChar)
                {
                    //
                    CurrentChar = NewChar;
                    //
                    ListOfProducts.Insert(i, new ElementOfSorting(CurrentChar, "N"));
                }
            }
            //
            // Addition Of Marking Of MNN
            //
            Sorting.TypeOfSorting = "M";
            ListOfProducts.Sort(Sorting);
            //
            CurrentChar = "";
            //
            for (int i = 0; i < ListOfProducts.Count; i++)
            {
                //
                // !!!
                //
                string NewChar = "", MNN = ((ElementOfSorting)ListOfProducts[i]).MNN;
                if (MNN.Length > 1)
                    NewChar = MNN[0].ToString();
                //
                if (CurrentChar != NewChar)
                {
                    //
                    CurrentChar = NewChar;
                    //
                    ListOfProducts.Insert(i, new ElementOfSorting(CurrentChar, "M"));
                }
            }
            //
            // Sorting Of List Of Products
            //
            Sorting.TypeOfSorting = "N";
            ListOfProducts.Sort(Sorting);
            //
            // Return
            //
            return ListOfProducts;
        }

        // Type Of List
        public enum TypeOfList
        {
            FullListOfProducts = 0, 
            OnlyListOfVisibleProducts
        }

        #endregion

        // Searching Products
        protected DataTable SearchingProducts(
            ArrayList ListOfProductsForSearching, 
            string TypeOfSearching, string RequiredString, Reading.ReadingOfData.TypesOfFiltration TypeOfFiltration)
        {
            //
            bool CheckedData = true;
            DataTable ResultOfSearching = new DataTable("ResultOfProducts");
            //
            // Checking Entering Data
            //
            if (!((TypeOfSearching == "N") || (TypeOfSearching == "M")))
                CheckedData = false;
            if (RequiredString == null)
                CheckedData = false;
            //(RequiredString == "") || 
            if (ListOfProductsForSearching.Count == 0)
                CheckedData = false;
            //
            // !!!
            //
            //Console.WriteLine("{0} {1} {2} {3} {4}",CheckedData, TypeOfSearching, ListOfProducts.Rows.Count, RequiredString, TypeOfFiltration);
            //
            if (CheckedData)
            {
                //
                // Creating Copy Of Table 
                //
                foreach (ElementOfSorting CurrentItem in ListOfProductsForSearching)
                    if (CurrentItem.ContainsRow)
                    { ResultOfSearching = CurrentItem.RowOfBase.Table.Clone(); break; }
                //
                // !!!
                //
                if (TypeOfSearching == "N")
                {
                    //
                    if (TypeOfFiltration == Reading.ReadingOfData.TypesOfFiltration.InBeginning)
                    {
                        //
                        // Searching In Beginning Name
                        //
                        ArrayList FoundProducts = 
                            SearchingInBeginning(ListOfProductsForSearching, RequiredString, TypeOfSearching);
                        //
                        // Creating Copy Of Table And Filling With Data
                        //
                        if (FoundProducts.Count > 0)
                        {
                            //
                            foreach (ElementOfSorting CurrentProduct in FoundProducts)
                                if (CurrentProduct.RowOfBase != null)
                                    ResultOfSearching.Rows.Add(CurrentProduct.RowOfBase.ItemArray);
                        }
                    }
                    else if (TypeOfFiltration == Reading.ReadingOfData.TypesOfFiltration.InText)
                    {
                        //
                        // Filtering On Name
                        //
                        //ResultOfSearching = SearchingInText(ListOfProducts, RequiredString, "Name");
                        DataTable ProductsFromList = new DataTable();
                        foreach (ElementOfSorting CurrentElement in ListOfProductsForSearching)
                            if (CurrentElement.RowOfBase != null)
                            {
                                ProductsFromList = CurrentElement.RowOfBase.Table;
                                break;
                            }
                        //
                        if (ListOfProductsForSearching.Count > 0)
                            ResultOfSearching = SearchingInText(ProductsFromList, RequiredString, "Name");
                        //ListOfProductsForSearching
                    }
                    else if (TypeOfFiltration == Reading.ReadingOfData.TypesOfFiltration.NotSearch)
                    {
                        //
                        foreach (ElementOfSorting CurrentElement in ListOfProductsForSearching)
                            if (CurrentElement.ContainsRow)
                            { ResultOfSearching.Rows.Add(CurrentElement.RowOfBase.ItemArray); }
                    }
                }
                else if (TypeOfSearching == "M")
                {
                    //
                    if (TypeOfFiltration == Reading.ReadingOfData.TypesOfFiltration.InBeginning)
                    {
                        //
                        // Searching In Beginning MNN
                        //
                        ArrayList FoundProducts =
                            SearchingInBeginning(ListOfProductsForSearching, RequiredString, TypeOfSearching);
                        //
                        // Creating Copy Of Table And Filling With Data
                        //
                        if (FoundProducts.Count > 0)
                        {
                            //
                            ResultOfSearching = ((ElementOfSorting)FoundProducts[0]).RowOfBase.Table.Clone();
                            //
                            foreach (ElementOfSorting CurrentProduct in FoundProducts)
                                ResultOfSearching.Rows.Add(CurrentProduct.RowOfBase.ItemArray);
                        }
                    }
                    else if (TypeOfFiltration == Reading.ReadingOfData.TypesOfFiltration.InText)
                    {
                        //
                        // Filtering On MNN
                        //
                        //ResultOfSearching = SearchingInText(ListOfProducts, RequiredString, "MNN");
                        if (ListOfProductsForSearching.Count > 0)
                            ResultOfSearching =
                                SearchingInText(((ElementOfSorting)ListOfProductsForSearching[0]).RowOfBase.Table,
                                RequiredString, "MNN");
                    }
                    else if (TypeOfFiltration == Reading.ReadingOfData.TypesOfFiltration.NotSearch)
                    {
                        //
                        foreach (ElementOfSorting CurrentElement in ListOfProductsForSearching)
                            if (CurrentElement.ContainsRow)
                            { ResultOfSearching.Rows.Add(CurrentElement.RowOfBase.ItemArray); }
                    }
                }
            }
            /*else
                ResultOfSearching = ListOfProducts;*/
            //
            // Return
            //
            return ResultOfSearching;
        }

        // Searching Product
        protected ArrayList SearchingInBeginning(
            ArrayList ListOfProductsForSearching, string FragmentOfName, string TypeOfSearching)
        {
            //
            ArrayList ListOfResults = new ArrayList();
            //
            // !!!
            //
            if (FragmentOfName.Length > 0)
            {
                //
                string InitialSymbol = FragmentOfName[0].ToString();
                //
                string[] SymbolsForSelection;
                //
                // !!!
                //
                if (InitialSymbol.ToUpper() != InitialSymbol.ToLower())
                    SymbolsForSelection = new string[2] { InitialSymbol.ToUpper(), InitialSymbol.ToLower() };
                else
                    SymbolsForSelection = new string[1] { FragmentOfName[0].ToString() };
                //
                // Sorting Of List Of Products
                //
                if (Sorting.TypeOfSorting != TypeOfSearching)
                {
                    Sorting.TypeOfSorting = TypeOfSearching;
                    ListOfProductsForSearching.Sort(Sorting);
                    Console.WriteLine("SS");
                }
                //
                // !!!
                //
                ArrayList ListForSearching = new ArrayList();
                //
                for (int i = 0; i < SymbolsForSelection.Length; i++)
                {
                    //
                    int IndexOfStartingSearching =
                        ListOfProductsForSearching.BinarySearch(
                        new ElementOfSorting(SymbolsForSelection[i], TypeOfSearching), Sorting);
                    //
                    if (IndexOfStartingSearching >= 0)
                    {
                        //
                        int CurrentIndex = IndexOfStartingSearching + 1;
                        //
                        while (CurrentIndex < ListOfProductsForSearching.Count)
                        {
                            //
                            ListForSearching.Add(ListOfProductsForSearching[CurrentIndex]);
                            //
                            CurrentIndex++;
                            //
                            if (CurrentIndex == ListOfProductsForSearching.Count)
                                break;
                            else
                            {
                                //
                                string ValueOfElement = "";
                                //
                                if (TypeOfSearching == "N")
                                    ValueOfElement =
                                        ((ElementOfSorting)ListOfProductsForSearching[CurrentIndex]).Name;
                                else if (TypeOfSearching == "M")
                                    ValueOfElement =
                                        ((ElementOfSorting)ListOfProductsForSearching[CurrentIndex]).MNN;
                                //
                                //if (ValueOfElement.Length == 1)
                                if (((ElementOfSorting)ListOfProductsForSearching[CurrentIndex]).RowOfBase == null)
                                    break;
                            }
                        }
                    }
                }
                //
                // !!!
                //
                if (FragmentOfName.Length > 1)
                {
                    foreach (ElementOfSorting CurrentElement in ListForSearching)
                    {
                        //
                        string ValueOfElement = "";
                        //
                        if (TypeOfSearching == "N")
                            ValueOfElement = CurrentElement.Name;
                        else if (TypeOfSearching == "M")
                            ValueOfElement = CurrentElement.MNN;
                        //
                        int IndexOfResult = -1;
                        //
                        if (FragmentOfName.Length <= ValueOfElement.Length)
                            IndexOfResult = ValueOfElement.ToUpper().IndexOf(
                                FragmentOfName.ToUpper(), 0, FragmentOfName.Length);
                        //
                        if (IndexOfResult == 0)
                        {
                            ListOfResults.Add(CurrentElement);
                        }
                    }
                }
                else
                {
                    ListOfResults.AddRange(ListForSearching);
                }
            }
            //
            // Return
            //
            return ListOfResults;
        }

        // Searching In Text
        protected DataTable SearchingInText(DataTable ListOfProducts, string RequiredString, string NameOfColumn)
        {
            //
            DataView FilteringProducts = new DataView();
            //
            // Checking
            //
            RequiredString = RequiredString.Replace("%", "");
            RequiredString = RequiredString.Replace("'", "");
            //
            // !!!
            //
            //int MinimalLength = (_MinimalLengthOfString <= 3) ? 3 : _MinimalLengthOfString;
            //
            if ((RequiredString.Length >= 3) && (RequiredString.Length < 30))
            {
                FilteringProducts.Table = ListOfProducts;
                //
                FilteringProducts.RowFilter = String.Format(
                    "{0} LIKE '%{1}%'", NameOfColumn, RequiredString);
                //
                ListOfProducts = FilteringProducts.ToTable();
                //
                ListOfProducts.AcceptChanges();
            }
            //
            // Return
            //
            return ListOfProducts;
        }

        #region ' Sorting '

        // Sorting Of List
        protected class SortingOfList : IComparer
        {

            #region ' Fields '

            //
            private string _TypeOfSorting;

            #endregion

            #region ' Designer '

            public SortingOfList()
            {
                //
                // !!!
                //
                this._TypeOfSorting = "N";
            }

            #endregion

            #region ' Settings '

            // Type Of Sorting
            public string TypeOfSorting
            {
                get { return _TypeOfSorting; }
                set { _TypeOfSorting = value; }
            }

            #endregion

            #region ' Comparing '

            // Compare
            public int Compare(object Object01, object Object02)
            {
                //
                // Converting Objects
                //
                ElementOfSorting Element01 = (ElementOfSorting)Object01;
                ElementOfSorting Element02 = (ElementOfSorting)Object02;
                //
                // Comparing
                //
                int Result = 0;
                //
                if (_TypeOfSorting == "N")
                    Result = String.Compare(Element01.Name, Element02.Name, StringComparison.Ordinal);
                else if (_TypeOfSorting == "M")
                    Result = String.Compare(Element01.MNN, Element02.MNN, StringComparison.Ordinal);
                //String.Compare(String01, String02, false);
                //
                // Return
                //
                return Result;
            }

            #endregion

        }

        // Element Of Sorting
        protected class ElementOfSorting
        {

            #region ' Fields '

            //
            public DataRow RowOfBase;
            public string Name;
            public string MNN;

            #endregion

            #region ' Designer '

            public ElementOfSorting(DataRow RowOfBase)
            {
                //
                // !!!
                //
                this.RowOfBase = RowOfBase;
                this.Name = "";
                this.MNN = "";
                //
                // Reception Of Value Of Name
                //
                if (RowOfBase.Table.Columns.Contains("Name"))
                    if (!(RowOfBase["Name"] is DBNull) && (RowOfBase["Name"] != null) &&
                        (RowOfBase["Name"].GetType() == typeof(string)))
                        if (((string)RowOfBase["Name"]).Length > 0)
                            this.Name = RowOfBase["Name"].ToString();
                //
                // Reception Of Value Of MNN
                //
                if (RowOfBase.Table.Columns.Contains("MNN"))
                    if (!(RowOfBase["MNN"] is DBNull) && (RowOfBase["MNN"] != null) &&
                        (RowOfBase["MNN"].GetType() == typeof(string)))
                        if (((string)RowOfBase["MNN"]).Length > 0)
                            this.MNN = RowOfBase["MNN"].ToString();
                //
            }

            public ElementOfSorting(string TextOfMarker, string TypeOfMarker)
            {
                //
                // !!!
                //
                if (TypeOfMarker == "N")
                {
                    this.Name = TextOfMarker;
                    this.MNN = "";
                }
                else if (TypeOfMarker == "M")
                {
                    this.Name = "";
                    this.MNN = TextOfMarker;
                }
            }

            #endregion

            #region ' Contains '

            // Contains Row
            public bool ContainsRow
            {
                get { return (RowOfBase != null) ? true : false; }
            }

            #endregion

        }

        #endregion

        #endregion

    }
}