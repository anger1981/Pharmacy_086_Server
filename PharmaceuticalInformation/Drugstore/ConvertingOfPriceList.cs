using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Drugstore
{
    public class ConvertingOfPriceList : BaseType
    {

        #region ' Fields '

        private string _MaskOfFullPriceList;
        private string _MaskOfReceipts;
        private string _MaskOfDeleting;
        private bool _UseOfIDOfPriceList;
        private int _IDOfPharmacy;

        #endregion

        #region ' Designer 

        public ConvertingOfPriceList()
            : this("")
        {
            //
        }

        public ConvertingOfPriceList(string PathToLogFile)
            : base(PathToLogFile)
        {
            //
        }

        #endregion

        #region ' Parameters Of Converting '

        public string MaskOfFullPriceList
        {
            get { return _MaskOfFullPriceList; }
            set { _MaskOfFullPriceList = value; }
        }

        public string MaskOfReceipts
        {
            get { return _MaskOfReceipts; }
            set { _MaskOfReceipts = value; }
        }

        public string MaskOfDeleting
        {
            get { return _MaskOfDeleting; }
            set { _MaskOfDeleting = value; }
        }

        public bool UseOfIDOfPriceList
        {
            get { return _UseOfIDOfPriceList; }
            set { _UseOfIDOfPriceList = value; }
        }

        public int IDOfPharmacy
        {
            get { return _IDOfPharmacy; }
            set { _IDOfPharmacy = value; }
        }

        #endregion

        #region ' Checking Of Masks '

        // Checking Of Masks
        public bool ConformityOfFileToMasks(string PathToFile)
        {
            //
            bool RecognitionOfMask = false;
            //
            string NameOfFile = Path.GetFileName(PathToFile);
            //
            if (_MaskOfFullPriceList.Length > 0)
                if (NameOfFile.ToLower().IndexOf(_MaskOfFullPriceList) >= 0)
                    RecognitionOfMask = true;
            //
            if (_MaskOfReceipts.Length > 0)
                if (NameOfFile.ToLower().IndexOf(_MaskOfReceipts) >= 0)
                    RecognitionOfMask = true;
            //
            if (_MaskOfDeleting.Length > 0)
                if (NameOfFile.ToLower().IndexOf(_MaskOfDeleting) >= 0)
                    RecognitionOfMask = true;
            //
            // Return
            //
            return RecognitionOfMask;
        }

        #endregion

        #region ' Converting '

        public DataTable Converting(string PathToFile)
        {
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            //int IDOfPharmacy = 108;
            //
            // Selection Of Type Of PriceList
            //
            bool RecognitionOfMask = false, AllPrices = false, Deleting = false;

            string NameOfFile = Path.GetFileName(PathToFile);
            //
            if (_MaskOfFullPriceList.Length > 0)
                if (NameOfFile.ToLower().IndexOf(_MaskOfFullPriceList) >= 0)
                {
                    RecognitionOfMask = true;
                    AllPrices = true;
                }
            if (_MaskOfReceipts.Length > 0)
                if (NameOfFile.ToLower().IndexOf(_MaskOfReceipts) >= 0)
                    RecognitionOfMask = true;
            if (_MaskOfDeleting.Length > 0)
                if (NameOfFile.ToLower().IndexOf(_MaskOfDeleting) >= 0)
                {
                    Deleting = true;
                    RecognitionOfMask = true;
                }
            //
            // Converting
            //
            if (RecognitionOfMask)
            {
                //
                RecordingInLogFile(String.Format("Converting Of File: {0}", PathToFile));
                //
                // Selection Of Type Of Price List
                //
                int IndexOfType = SelectionOfTypeOfPriceList(PathToFile);
                //
                // Selection Of Method Of Converting Of Price List
                //
                switch (IndexOfType)
                {
                    case -1:
                        RecordingInLogFile("Расширение не поддерживается");
                        break;
                    case 0:
                        RecordingInLogFile("Формат неподдерживается");
                        break;
                    case 1:
                        TableOfPrice = ConvertingOfPriceListOfType01_02(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 2:
                        TableOfPrice = ConvertingOfPriceListOfType02(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 3:
                        TableOfPrice = ConvertingOfPriceListOfType03_02(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 4:
                        TableOfPrice = ConvertingOfPriceListOfType04(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 5:
                        TableOfPrice = ConvertingOfPriceListOfType05(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 6:
                        TableOfPrice = ConvertingOfPriceListOfType06(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 7:
                        TableOfPrice = ConvertingOfPriceListOfType07(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 8:
                        TableOfPrice = ConvertingOfPriceListOfType08(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 9:
                        TableOfPrice = ConvertingOfPriceListOfType09(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                    case 10:
                        TableOfPrice = ConvertingOfPriceListOfType10(
                            PathToFile, _IDOfPharmacy, _UseOfIDOfPriceList, Deleting, AllPrices);
                        break;
                }
                //
                RecordingInLogFile(String.Format("Count Of Converted Rows: {0}", TableOfPrice.Rows.Count));
            }
            //
            // Return
            //
            return TableOfPrice;
        }

        private int SelectionOfTypeOfPriceList(string PathToFile)
        {
            //
            int IndexOfType = -1;
            //
            string Extension = Path.GetExtension(PathToFile).ToLower();
            //
            if ((Extension == ".txt") || (Extension == ".prs") ||
                (Extension == ".pos") || (Extension == ".ced") ||
                (Extension == ".xml"))
            {
                //
                IndexOfType = 0;
            }
            //
            if ((Extension == ".txt") || (Extension == ".prs") ||
                (Extension == ".pos") || (Extension == ".ced") )
            {
                //
                //IndexOfType = 0;
                //
                FileStream FS = new FileStream(PathToFile, FileMode.Open);
                StreamReader SR = new StreamReader(FS);
                string LineFromPrice = SR.ReadLine();
                SR.Close();
                FS.Dispose();
                //
                GC.SuppressFinalize(SR);
                GC.SuppressFinalize(FS);
                //
                // !!!
                //
                int DelimCount = 0;
                int EndIndex = LineFromPrice.IndexOf("\t", 0);//1
                while (EndIndex > -1)
                {
                    DelimCount++;
                    EndIndex = LineFromPrice.IndexOf("\t", EndIndex + 1);
                }
                //
                // !!!
                //
                if ((DelimCount == 4) || (DelimCount == 5))
                    IndexOfType = 1;
                //
                if ((LineFromPrice.Length == 44) || (LineFromPrice.Length == 45))
                    IndexOfType = 2;
                //
                if ((LineFromPrice.IndexOf("\"") >= 0) && (LineFromPrice.IndexOf(",") >= 0))
                    IndexOfType = 9;
                //
                if ((LineFromPrice.IndexOf(";") >= 0) && !(LineFromPrice.IndexOf("\"") >= 0) && 
                    !(LineFromPrice.IndexOf(",") >= 0))
                    IndexOfType = 10;
            }
            //
            if (Extension == ".xml")
            {
                //
                //IndexOfType = 0;
                //
                FileStream FS = new FileStream(PathToFile, FileMode.Open);
                StreamReader SR = new StreamReader(FS, Encoding.Default);
                string TextOfPriceList = SR.ReadToEnd();
                SR.Close();
                FS.Close();
                FS.Dispose();
                //
                GC.SuppressFinalize(SR);
                GC.SuppressFinalize(FS);
                //
                GC.Collect();
                GC.Collect();
                //
                if (TextOfPriceList != null)
                {
                    if (TextOfPriceList.IndexOf("<ДокументПрайс") >= 0)
                        IndexOfType = 3;
                    if (TextOfPriceList.IndexOf("<Выгрузка КодАптеки") >= 0)
                        IndexOfType = 4;
                    if (TextOfPriceList.IndexOf("<Документ КодАптеки") >= 0)
                        IndexOfType = 5;
                    if ((TextOfPriceList.IndexOf("<Документ КодАптеки") >= 0) &&
                        (TextOfPriceList.IndexOf("<Приход") >= 0))
                        IndexOfType = 7;
                    if (TextOfPriceList.IndexOf("<Документ Идентификатор") >= 0)
                        IndexOfType = 6;
                    if (TextOfPriceList.IndexOf("<ПрайсЛист КодАптеки") >= 0)
                        IndexOfType = 8;// (5)
                }
                //
                GC.SuppressFinalize(TextOfPriceList);
            }
            //
            // Return
            //
            return IndexOfType;
        }

        // Type 01
        private DataTable ConvertingOfPriceListOfType01(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 01"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            if (IDOfPharmacy > 0)
            {
                string sLine = "";
                FileStream FS = new FileStream(PathToFile, FileMode.Open);
                StreamReader SR = new StreamReader(FS);
                sLine = SR.ReadLine();
                //
                while ((sLine != null) && (sLine.Length > 1))
                {
                    //
                    int StartIndex = 0;
                    int EndIndex = 0;
                    int LineLenght = sLine.Length;
                    //
                    // ID Of Product
                    //
                    int Id_Product = 0;
                    EndIndex = sLine.IndexOf("\t", StartIndex + 1);
                    if (EndIndex < 0)
                        EndIndex = LineLenght;
                    string sField = sLine.Substring(StartIndex, (EndIndex - StartIndex));
                    sField = sField.Trim();
                    //Если не смогли перевести Id товара в число - переход на след. строку
                    if (!int.TryParse(sField, out Id_Product))
                    {
                        sLine = SR.ReadLine();
                        continue;
                    }
                    //Id_Product = uint.Parse(sField);
                    //
                    // Price
                    //
                    StartIndex = EndIndex;
                    EndIndex = sLine.IndexOf("\t", StartIndex + 1);
                    if (EndIndex < 0)
                        EndIndex = LineLenght;
                    sField = sLine.Substring(StartIndex, (EndIndex - StartIndex));
                    sField = sField.Trim();
                    //
                    decimal dPrice = Convert.ToDecimal(ReplaceSeparator(sField));
                    //
                    // ID Of Drugstore
                    //
                    if (UseOfIDOfPriceList)
                    {
                        StartIndex = EndIndex;
                        EndIndex = sLine.IndexOf("\t", StartIndex + 1);
                        if (EndIndex < 0)
                            EndIndex = LineLenght;
                        sField = sLine.Substring(StartIndex, (EndIndex - StartIndex));
                        sField = sField.Trim();
                        IDOfPharmacy = int.Parse(sField);//???
                    }
                    //
                    // Inserting
                    //
                    DataRow NewPrice = TableOfPrice.NewRow();
                    //
                    NewPrice[0] = IDOfPharmacy;
                    NewPrice[1] = Id_Product;
                    NewPrice[2] = dPrice;// Convert.ToDecimal(ReplaceSeparator(sField));
                    NewPrice[3] = Deleting;
                    NewPrice[4] = AllPrices;
                    //
                    TableOfPrice.Rows.Add(NewPrice);
                    //
                    // Next
                    //
                    sLine = SR.ReadLine();
                }
                SR.Close();
                FS.Close();
            }
            else Console.WriteLine("!! Не найден ID аптеки.");
            //
            return TableOfPrice;
        }

        // Type 01
        private DataTable ConvertingOfPriceListOfType01_02(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 01"));
            //
            DataTable PriceList = CreatingTableOfPriceList();
            //
            // Reading PirceList
            //
            string TextOfPriceList = "";
            //
            FileStream FS = new FileStream(PathToFile, FileMode.Open, FileAccess.Read);
            StreamReader SR = new StreamReader(FS, Encoding.Default);
            TextOfPriceList = SR.ReadToEnd();
            SR.Close();
            FS.Dispose();
            GC.SuppressFinalize(FS);
            //
            // !!!
            //
            if ((TextOfPriceList != "") && (TextOfPriceList != null))
            {
                //
                string[] RowsOfText = TextOfPriceList.Split('\n');
                //
                // !!!
                //
                foreach (string CurrentRow in RowsOfText)
                {
                    //
                    string[] ElementsOfRow = CurrentRow.Split('\t');
                    //
                    // !!!
                    //
                    if (ElementsOfRow.Length >= 3)
                    {
                        //
                        // ID Of Product
                        //
                        int IDOfProduct = 0;
                        //
                        if ((ElementsOfRow[0] != null) && (ElementsOfRow[0] != ""))
                        {
                            string StringOfIDOfProduct = ElementsOfRow[0].Trim();
                            try { IDOfProduct = Convert.ToInt32(StringOfIDOfProduct); }
                            catch { }
                        }
                        //
                        // !!!
                        //
                        if (IDOfProduct > 0)
                        {
                            //
                            bool SuccessfulConverting = true;
                            //
                            // Price
                            //
                            decimal Price = 0.0m;
                            //
                            if ((ElementsOfRow[1] != null) && (ElementsOfRow[1] != ""))
                            {
                                string StringOfPrice = ElementsOfRow[1].Trim();
                                StringOfPrice = ReplaceSeparator(StringOfPrice);
                                try { Price = Convert.ToDecimal(StringOfPrice); }
                                catch { SuccessfulConverting = false; }
                            }
                            //
                            // ID Of Drugstore
                            //
                            if (UseOfIDOfPriceList)
                                if ((ElementsOfRow[2] != null) && (ElementsOfRow[2] != ""))
                                {
                                    string StringOfIDOfDrugstore = ElementsOfRow[2].Trim();
                                    try { IDOfPharmacy = Convert.ToInt32(StringOfIDOfDrugstore); }
                                    catch { SuccessfulConverting = false; }
                                }
                            //
                            // Inserting
                            //
                            if (SuccessfulConverting &&
                                (IDOfProduct > 0) && (Price > 0.0m) && (IDOfPharmacy > 0))
                            {
                                DataRow NewPrice = PriceList.NewRow();
                                //
                                NewPrice[0] = IDOfPharmacy;
                                NewPrice[1] = IDOfProduct;
                                NewPrice[2] = Price;
                                NewPrice[3] = Deleting;
                                NewPrice[4] = AllPrices;
                                //
                                PriceList.Rows.Add(NewPrice);
                            }
                        }
                    }
                }
            }
            //
            // Return
            //
            return PriceList;
        }

        // Type 02
        private DataTable ConvertingOfPriceListOfType02(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 02"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            string sLine = "";
            FileStream FS = new FileStream(PathToFile, FileMode.Open, FileAccess.Read);
            StreamReader SR = new StreamReader(FS, Encoding.Default);
            sLine = SR.ReadLine();
            //
            //if ((sLine.Length == 44) || (sLine.Length == 45))
            {
                // Loading Of File Of Price List
                while ((sLine != null) && (sLine.Length > 1))
                {
                    //
                    string sField;
                    //
                    // ID Of Product
                    //
                    int Id_Product = 0;
                    //
                    try
                    {
                        sField = sLine.Substring(0, 10);
                        Id_Product = int.Parse(sField);
                    }
                    catch { }
                    //
                    // ID Of Drugstore
                    //
                    if (UseOfIDOfPriceList)
                    {
                        sField = sLine.Substring(10, 6);
                        IDOfPharmacy = int.Parse(sField);
                    }
                    //
                    // Price
                    //
                    sField = sLine.Substring(28, 10);
                    int iPrice = int.Parse(sField);
                    //Console.WriteLine(iPrice);
                    decimal dPrice = (decimal)iPrice / 100;
                    //
                    // Inserting
                    //
                    if (/*SuccessfulConverting &&*/
                                (Id_Product > 0) /*&& (dPrice > 0.0m)*/ && (IDOfPharmacy > 0))
                    {
                        //
                        DataRow NewPrice = TableOfPrice.NewRow();
                        //
                        NewPrice[0] = IDOfPharmacy;
                        NewPrice[1] = Id_Product;
                        NewPrice[2] = dPrice;
                        NewPrice[3] = Deleting;
                        NewPrice[4] = AllPrices;
                        //
                        TableOfPrice.Rows.Add(NewPrice);
                    }

                    sLine = SR.ReadLine();
                }
            }
            //
            SR.Close();
            FS.Close();
            //else Console.WriteLine("! Неизвестный формат файла.");
            //
            return TableOfPrice;
        }

        // Type 03
        private DataTable ConvertingOfPriceListOfType03(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 03"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);
            //
            foreach (DataRow drPharmacy in PricesForTransformation.Tables["Аптека"].Rows)
            {
                if (UseOfIDOfPriceList)
                    IDOfPharmacy = Convert.ToInt32(drPharmacy.ItemArray[1]);
                foreach (DataRow drProduct in drPharmacy.GetChildRows("Аптека_ТоварнаяПозиция"))
                {
                    //
                    DataRow NewPrice = TableOfPrice.NewRow();
                    //
                    NewPrice[0] = IDOfPharmacy;
                    NewPrice[1] = Convert.ToInt32(drProduct.ItemArray[0].ToString());
                    NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(drProduct.ItemArray[1].ToString()));
                    NewPrice[3] = Deleting;
                    NewPrice[4] = AllPrices;
                    //
                    TableOfPrice.Rows.Add(NewPrice);
                }
            }
            //
            return TableOfPrice;
        }

        // Type 03
        private DataTable ConvertingOfPriceListOfType03_02(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 03"));
            //
            DataTable PriceList = CreatingTableOfPriceList();
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);
            //
            if (PricesForTransformation.Tables.Contains("Аптека"))
                foreach (DataRow CurrentDrugstore in PricesForTransformation.Tables["Аптека"].Rows)
                {
                    //
                    // Getting ID
                    //
                    if (UseOfIDOfPriceList)
                    {
                        //"Код"
                        if((CurrentDrugstore[1] != null) && (CurrentDrugstore[1] != ""))
                            IDOfPharmacy = Convert.ToInt32(CurrentDrugstore[1]);
                    }
                    //
                    foreach (DataRow CurrentPrice in CurrentDrugstore.GetChildRows("Аптека_ТоварнаяПозиция"))
                    {
                        //
                        DataRow NewPrice = PriceList.NewRow();
                        //
                        NewPrice[0] = IDOfPharmacy;
                        //
                        // !!!
                        //
                        try
                        {
                            if (CurrentPrice.Table.Columns.Contains("КодТовара"))
                            { NewPrice[1] = Convert.ToInt32(CurrentPrice["КодТовара"].ToString()); }
                            else
                            { NewPrice[1] = Convert.ToInt32(CurrentPrice[0].ToString()); }
                        }
                        catch { NewPrice[1] = -1; }
                        //
                        // !!!
                        //
                        try
                        {
                            if (CurrentPrice.Table.Columns.Contains("Цена"))
                            { NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(CurrentPrice["Цена"].ToString())); }
                            else
                            { NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(CurrentPrice[1].ToString())); }
                        }
                        catch { NewPrice[2] = -1; }
                        //
                        NewPrice[3] = Deleting;
                        NewPrice[4] = AllPrices;
                        /*
                         * <КодТовара>19</КодТовара> 
                         * <Цена>116.00</Цена> 
                         */
                        //
                        PriceList.Rows.Add(NewPrice);
                    }
                }
            else
            {
                this.ReturningMessageAboutError(
                    "В Прайс-листе нет ценовых строк", new Exception("No Table Of Аптека"), false);
            }
            //
            // Return
            //
            return PriceList;
        }

        // Type 03
        private DataTable ConvertingOfPriceListOfType03_03(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 03B"));
            // Creating table of PriceList
            DataTable PriceList = CreatingTableOfPriceList();
            DataRow NewPrice = null;
            // Reading file of price-list in table
            System.Xml.XmlReader XML = System.Xml.XmlReader.Create(PathToFile);
            Boolean ProductIsOpen = false, IDOfProductIsOpen = false, PriceOfProductIsOpen = false;
            while (XML.Read())
            {
                /*
                //
                    // Getting ID
                    //
                    if (UseOfIDOfPriceList)
                    {
                        //"Код"
                        if ((CurrentDrugstore[1] != null) && (CurrentDrugstore[1] != ""))
                            IDOfPharmacy = Convert.ToInt32(CurrentDrugstore[1]);
                    }
                */
                // Opening and closing of price
                if (XML.Name.ToUpper() == "ТоварнаяПозиция".ToUpper())
                {
                    if (XML.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        ProductIsOpen = true;
                        // Creating price
                        NewPrice = PriceList.NewRow();
                        //
                        NewPrice[0] = IDOfPharmacy;
                        NewPrice[3] = Deleting;
                        NewPrice[4] = AllPrices;
                    }
                    else if (XML.NodeType == System.Xml.XmlNodeType.EndElement)
                    {
                        // Closing
                        ProductIsOpen = false;
                        // Additon price
                        PriceList.Rows.Add(NewPrice);
                    }
                }
                // Reading of price
                if (ProductIsOpen)
                {
                    // Opening and closing ID of product
                    if (XML.Name.ToUpper() == "КодТовара".ToUpper())
                    {
                        if (XML.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            IDOfProductIsOpen = true;
                        }
                        else if (XML.NodeType == System.Xml.XmlNodeType.EndElement)
                        {
                            IDOfProductIsOpen = false;
                        }
                    }
                    // Opening and closing of price
                    if (XML.Name.ToUpper() == "Цена".ToUpper())
                    {
                        if (XML.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            PriceOfProductIsOpen = true;
                        }
                        else if (XML.NodeType == System.Xml.XmlNodeType.EndElement)
                        {
                            PriceOfProductIsOpen = false;
                        }
                    }
                    // Reading ID of product
                    if (IDOfProductIsOpen && (XML.NodeType == System.Xml.XmlNodeType.Text))
                    {
                        try
                        {
                            //Console.WriteLine("ID of product: {0}", Convert.ToInt32(XML.Value));
                            NewPrice[1] = Convert.ToInt32(XML.Value);
                        }
                        catch { NewPrice[1] = -1; }
                    }
                    // Reading price of product
                    if (PriceOfProductIsOpen && (XML.NodeType == System.Xml.XmlNodeType.Text))
                    {
                        try
                        {
                            //Console.WriteLine("Price of product: {0}", Convert.ToDecimal(ReplaceSeparator(XML.Value)));
                            NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(XML.Value));
                        }
                        catch { NewPrice[2] = -1; }
                    }
                }
                /*
                 * <КодТовара>19</КодТовара> 
                 * <Цена>116.00</Цена> 
                */
            }
            //
            // Return
            //
            return PriceList;
        }

        // Type 03
        private DataTable ConvertingOfPriceListOfType03_02T(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 03"));
            //
            DataTable PriceList = CreatingTableOfPriceList();
            //
            try
            {
                // Reading file of price-list in table
                DataSet PricesForTransformation = new DataSet("XML");
                FileStream FS = new FileStream(PathToFile, FileMode.Open, FileAccess.Read);

                System.Xml.XmlReader X = System.Xml.XmlReader.Create(FS);
                RecordingInLogFile(X.XmlLang);
                RecordingInLogFile(X.Value);
                RecordingInLogFile(X.AttributeCount.ToString());
                RecordingInLogFile(X.LocalName);

                RecordingInLogFile("Read");
                //
                if (PricesForTransformation.Tables.Contains("Аптека"))
                {
                    //
                    //RecordingInLogFile(String.Format("Count of drugstores: {0}", PricesForTransformation.Tables["Аптека"].Rows.Count));
                    //
                    foreach (DataRow CurrentDrugstore in PricesForTransformation.Tables["Аптека"].Rows)
                    {
                        //
                        // Getting ID
                        //
                        if (UseOfIDOfPriceList)
                        {
                            //"Код"
                            if ((CurrentDrugstore[1] != null) && (CurrentDrugstore[1] != ""))
                                IDOfPharmacy = Convert.ToInt32(CurrentDrugstore[1]);
                        }
                        //
                        //RecordingInLogFile(String.Format("Count of prices: {0}", CurrentDrugstore.GetChildRows("Аптека_ТоварнаяПозиция").Length));
                        //
                        foreach (DataRow CurrentPrice in CurrentDrugstore.GetChildRows("Аптека_ТоварнаяПозиция"))
                        {
                            //
                            DataRow NewPrice = PriceList.NewRow();
                            //
                            NewPrice[0] = IDOfPharmacy;
                            //
                            // !!!
                            //
                            try
                            {
                                //RecordingInLogFile(String.Format("Value of ID product: {0}", CurrentPrice["КодТовара"].ToString()));
                                //
                                if (CurrentPrice.Table.Columns.Contains("КодТовара"))
                                { NewPrice[1] = Convert.ToInt32(CurrentPrice["КодТовара"].ToString()); }
                                else
                                { NewPrice[1] = Convert.ToInt32(CurrentPrice[0].ToString()); }
                                //
                                //RecordingInLogFile(String.Format("ID of product: {0}", NewPrice[1]));
                            }
                            catch { NewPrice[1] = -1; }
                            //
                            // !!!
                            //
                            try
                            {
                                //
                                if (CurrentPrice.Table.Columns.Contains("Цена"))
                                {
                                    NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(CurrentPrice["Цена"].ToString()));
                                }
                                else
                                {
                                    NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(CurrentPrice[1].ToString()));
                                }
                                //
                            }
                            catch { NewPrice[2] = -1; }
                            //
                            NewPrice[3] = Deleting;
                            NewPrice[4] = AllPrices;
                            /*
                             * <КодТовара>19</КодТовара> 
                             * <Цена>116.00</Цена> 
                             */
                            //
                            PriceList.Rows.Add(NewPrice);
                        }
                    }
                }
                else
                {
                    this.ReturningMessageAboutError(
                        "В Прайс-листе нет ценовых строк", new Exception("No Table Of Аптека"), false);
                }
            }
            catch (Exception E)
            {
                RecordingInLogFile(String.Format("ERROR: {0}", E.Message));
                throw E;
            }
            //
            // Return
            //
            return PriceList;
        }

        // Type 04
        private DataTable ConvertingOfPriceListOfType04(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 04"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);
            //
            foreach (DataRow drPharmacy in PricesForTransformation.Tables["Выгрузка"].Rows)
            {
                if (UseOfIDOfPriceList)
                    IDOfPharmacy = Convert.ToInt32(drPharmacy.ItemArray[1]);
                foreach (DataRow drPrice in drPharmacy.GetChildRows("Выгрузка_ПрайсЛист"))
                {
                    foreach (DataRow drProduct in drPrice.GetChildRows("ПрайсЛист_Запись"))
                    {
                        //
                        DataRow NewPrice = TableOfPrice.NewRow();
                        //
                        NewPrice[0] = IDOfPharmacy;
                        NewPrice[1] = Convert.ToInt32(drProduct.ItemArray[0].ToString());
                        NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(drProduct.ItemArray[1].ToString()));
                        NewPrice[3] = Deleting;
                        NewPrice[4] = AllPrices;
                        //
                        TableOfPrice.Rows.Add(NewPrice);
                    }
                }
            }
            return TableOfPrice;
        }

        // Type 05
        private DataTable ConvertingOfPriceListOfType05(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 05"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            int error_codes_count = 0;
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);
            //
            foreach (DataRow drPharmacy in PricesForTransformation.Tables["Документ"].Rows)
            {
                if (UseOfIDOfPriceList)
                    IDOfPharmacy = Convert.ToInt32(drPharmacy.ItemArray[1]);
                foreach (DataRow drPrice in drPharmacy.GetChildRows("Документ_ПрайсЛист"))
                {
                    foreach (DataRow drProduct in drPrice.GetChildRows("ПрайсЛист_Запись"))
                    {
                        string sField = Convert.ToString(drProduct.ItemArray[0]);
                        if ((sField.Length == 0) || (sField == "0"))
                        {
                            error_codes_count++;
                            continue;
                        }
                        int Id_Product = Convert.ToInt32(sField);
                        //
                        DataRow NewPrice = TableOfPrice.NewRow();
                        //
                        NewPrice[0] = IDOfPharmacy;
                        NewPrice[1] = Id_Product;
                        NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(drProduct.ItemArray[1].ToString()));
                        NewPrice[3] = Deleting;
                        NewPrice[4] = AllPrices;
                        //
                        TableOfPrice.Rows.Add(NewPrice);
                    }
                }
            }
            if (error_codes_count > 0)
            { Console.WriteLine("! Товаров с пустыми кодами: " + error_codes_count.ToString()); }
            //
            return TableOfPrice;
        }

        // Type 06
        private DataTable ConvertingOfPriceListOfType06(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 06"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);//, XmlReadMode.IgnoreSchema);
            //
            Console.WriteLine(
                "!ВНИМАНИЕ! Этот формат выгрузки 1С не предназначен для справки 086. \n" +
                "Коды товара могут быть не сопоставлены. Проверьте таблицу выгрузки на закладке 'Редактирование'.");
            //
            foreach (DataRow drPharmacy in PricesForTransformation.Tables["ТоварныеПозиции"].Rows)
            {
                foreach (DataRow drPrice in drPharmacy.GetChildRows("ТоварныеПозиции_ТоварнаяПозиция"))
                {
                    int Id_Product = 0;
                    foreach (DataRow drProduct in drPrice.GetChildRows("ТоварнаяПозиция_Товар"))
                    {
                        Id_Product = Convert.ToInt32(drProduct.ItemArray[0].ToString());
                    }
                    //
                    DataRow NewPrice = TableOfPrice.NewRow();
                    //
                    NewPrice[0] = IDOfPharmacy;
                    NewPrice[1] = Id_Product;
                    NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(drPrice.ItemArray[13].ToString()));
                    NewPrice[3] = Deleting;
                    NewPrice[4] = AllPrices;
                    //
                    TableOfPrice.Rows.Add(NewPrice);
                }
            }
            //
            return TableOfPrice;
        }

        // Type 07
        private DataTable ConvertingOfPriceListOfType07(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 07"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);
            //
            foreach (DataRow drPharmacy in PricesForTransformation.Tables["Документ"].Rows)
            {
                if (UseOfIDOfPriceList)
                    IDOfPharmacy = Convert.ToInt32(drPharmacy.ItemArray[1]);
                foreach (DataRow drPrice in drPharmacy.GetChildRows("Документ_Приход"))
                {
                    foreach (DataRow drProduct in drPrice.GetChildRows("Приход_Запись"))
                    {
                        DataRow NewPrice = TableOfPrice.NewRow();
                        //
                        NewPrice[0] = IDOfPharmacy;
                        NewPrice[1] = Convert.ToInt32(drProduct.ItemArray[0]);
                        NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(drProduct.ItemArray[1].ToString()));
                        NewPrice[3] = Deleting;
                        NewPrice[4] = AllPrices;
                        //
                        TableOfPrice.Rows.Add(NewPrice);
                    }
                }
            }
            //
            return TableOfPrice;
        }

        // Type 08 (From Type 05)
        private DataTable ConvertingOfPriceListOfType08(string PathToFile, int IDOfPharmacy,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 08"));
            //
            DataTable TableOfPrice = CreatingTableOfPriceList();
            //
            int error_codes_count = 0;
            //
            DataSet PricesForTransformation = new DataSet("XML");
            PricesForTransformation.ReadXml(PathToFile);
            //
            foreach (DataRow drPharmacy in PricesForTransformation.Tables["ПрайсЛист"].Rows)
            {
                if (UseOfIDOfPriceList)
                    IDOfPharmacy = Convert.ToInt32(drPharmacy.ItemArray[1]);
                foreach (DataRow drPrice in drPharmacy.GetChildRows("ПрайсЛист_Запись"))
                {
                    string sField = Convert.ToString(drPrice.ItemArray[0]);
                    if ((sField.Length == 0) || (sField == "0"))
                    {
                        error_codes_count++;
                        continue;
                    }
                    int Id_Product = Convert.ToInt32(sField);
                    //
                    DataRow NewPrice = TableOfPrice.NewRow();
                    //
                    NewPrice[0] = IDOfPharmacy;
                    NewPrice[1] = Id_Product;
                    //
                    string StringOfPrice = drPrice.ItemArray[1].ToString();
                    if (StringOfPrice != "")
                        NewPrice[2] = Convert.ToDecimal(ReplaceSeparator(StringOfPrice));
                    else
                        NewPrice[2] = 0;
                    //
                    NewPrice[3] = Deleting;
                    NewPrice[4] = AllPrices;
                    //
                    TableOfPrice.Rows.Add(NewPrice);
                }
            }
            if (error_codes_count > 0)
            { Console.WriteLine("! Товаров с пустыми кодами: " + error_codes_count.ToString()); }
            //
            return TableOfPrice;
        }

        // Type 09 (Zair 4)
        private DataTable ConvertingOfPriceListOfType09(string PathToFile, int IDOfDrugstore,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 09"));
            //
            DataTable PriceList = CreatingTableOfPriceList();
            //
            if (IDOfDrugstore > 0)
            {
                //
                // Reading Text Of PriceList File
                //
                string TextOfPriceList = "";
                //
                try
                {
                    //
                    FileStream FS = new FileStream(PathToFile, FileMode.Open, FileAccess.Read);
                    StreamReader SR = new StreamReader(FS, Encoding.Default);
                    //
                    try { TextOfPriceList = SR.ReadToEnd(); }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при чтении текста файла Прайса", E, true, false); }
                    //
                    try
                    {
                        SR.Close();
                        FS.Dispose();
                    }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при закрытии потоков файла Прайса", E, true, false); }
                    //
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при создании потоков файла Прайса", E, true, false); }
                //
                // Converting Text Of PriceList In List
                //
                if ((TextOfPriceList != "") && (TextOfPriceList != null))
                {
                    //
                    // Division Of Text Of PriceList On Rows
                    //
                    string[] RowsOfPriceList = TextOfPriceList.Split('\n');
                    //
                    if (RowsOfPriceList.Length > 0)
                    {
                        //
                        foreach (string CurrentRowOfPrice in RowsOfPriceList)
                        {
                            //
                            // Division Of Row Of Price On Parts
                            //
                            string[] ComponentsOfRowOfPrice =
                                CurrentRowOfPrice.Split(new string[1] { "\"" }, StringSplitOptions.None);
                            //
                            // !!!
                            //
                            if (ComponentsOfRowOfPrice.Length >= 7)
                            {
                                //
                                // ID Of Product
                                //
                                int IDOfProduct = 0;
                                //
                                string TextOfIDOfProduct = ComponentsOfRowOfPrice[1].Replace("\"", "");
                                IDOfProduct = Convert.ToInt32(TextOfIDOfProduct);
                                //int.TryParse(TextOfIDOfProduct, out IDOfProduct);
                                //
                                // Price Of Product
                                //
                                decimal PriceOfProduct = 0.0m;
                                //
                                string TextOfPrice = ComponentsOfRowOfPrice[6];
                                string[] ComponentsOfPrice = 
                                    TextOfPrice.Split(new string[1] { "," }, StringSplitOptions.None);
                                //
                                if (ComponentsOfPrice.Length >= 4) 
                                    PriceOfProduct = Convert.ToDecimal(ReplaceSeparator(ComponentsOfPrice[2]));
                                //
                                // Addition Of New Price In PriceList
                                //
                                DataRow NewPrice = PriceList.NewRow();
                                //
                                NewPrice[0] = IDOfDrugstore;
                                NewPrice[1] = IDOfProduct;
                                NewPrice[2] = PriceOfProduct;
                                NewPrice[3] = Deleting;
                                NewPrice[4] = AllPrices;
                                //
                                PriceList.Rows.Add(NewPrice);
                                //
                            }
                            else { this.RecordingInLogFile("ERROR Bad Row Of Price"); }
                        }
                    }
                    else { this.RecordingInLogFile("ERROR In Text Of File PriceList Count Of Rows is 0"); }
                }
                else { this.RecordingInLogFile("ERROR Text Of File PriceList is Null"); }
                //
            }
            else { this.RecordingInLogFile("ERROR ID Of Drugstore is 0"); }
            //
            // Return
            //
            return PriceList;
        }

        // Type 10 (Rainbow)
        private DataTable ConvertingOfPriceListOfType10(string PathToFile, int IDOfDrugstore,
            bool UseOfIDOfPriceList, bool Deleting, bool AllPrices)
        {
            //
            RecordingInLogFile(String.Format("Path To File={0} Type Of File={1}", PathToFile, "Type 10"));
            //
            DataTable PriceList = CreatingTableOfPriceList();
            //
            if (IDOfDrugstore > 0)
            {
                //
                // Reading Text Of PriceList File
                //
                string TextOfPriceList = "";
                //
                try
                {
                    //
                    FileStream FS = new FileStream(PathToFile, FileMode.Open, FileAccess.Read);
                    StreamReader SR = new StreamReader(FS, Encoding.Default);
                    //
                    try { TextOfPriceList = SR.ReadToEnd(); }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при чтении текста файла Прайс-Листа", E, true, false); }
                    //
                    try
                    {
                        SR.Close();
                        FS.Dispose();
                    }
                    catch (Exception E)
                    { this.ReturningMessageAboutError("Ошибка при закрытии потоков файла Прайс-Листа", E, true, false); }
                    //
                }
                catch (Exception E)
                { this.ReturningMessageAboutError("Ошибка при создании потоков файла Прайс-Листа", E, true, false); }
                //
                // Converting Text Of PriceList In List
                //
                if ((TextOfPriceList != String.Empty) && (TextOfPriceList != null))
                {
                    //
                    // Division Of Text Of PriceList On Rows
                    //
                    string[] RowsOfPriceList = TextOfPriceList.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    //
                    if (RowsOfPriceList.Length == 0)
                        this.RecordingInLogFile("ERROR In Text Of File PriceList Count Of Rows is 0");
                        //
                    foreach (string CurrentRowOfPrice in RowsOfPriceList)
                    {
                        //
                        // Division Of Row Of Price On Parts
                        //
                        string[] ComponentsOfRowOfPrice = 
                            CurrentRowOfPrice.Split(new string[1] { ";" }, StringSplitOptions.None);
                        //
                        // Addition Of Price
                        //
                        if (ComponentsOfRowOfPrice.Length >= 3)
                        {
                            //
                            bool SuccessfulGetting = true;
                            //
                            // Getting ID Of Drugstore
                            //
                            if (UseOfIDOfPriceList)
                            try { IDOfDrugstore = Convert.ToInt32(ComponentsOfRowOfPrice[0]); }
                            catch { SuccessfulGetting = false; }
                            //
                            // Getting ID Of Product
                            //
                            int IDOfProduct = 0;
                            //
                            try
                            {
                                string TextOfIDOfProduct = 
                                    (ComponentsOfRowOfPrice.Length == 4) ? 
                                    ComponentsOfRowOfPrice[1] : ComponentsOfRowOfPrice[2];
                                //
                                IDOfProduct = Convert.ToInt32(TextOfIDOfProduct);
                            }
                            catch { SuccessfulGetting = false; }
                            //
                            // Getting Price Of Product
                            //
                            decimal PriceOfProduct = 0.0m;
                            //
                            try
                            {
                                string TextOfPrice = 
                                    (ComponentsOfRowOfPrice.Length == 4) ? 
                                    ComponentsOfRowOfPrice[2] : ComponentsOfRowOfPrice[6];
                                //
                                PriceOfProduct = Convert.ToDecimal(ReplaceSeparator(TextOfPrice));
                            }
                            catch { SuccessfulGetting = false; }
                            //
                            // Addition Of New Price In PriceList
                            //
                            if (SuccessfulGetting)
                            {
                                //
                                DataRow NewPrice = PriceList.NewRow();
                                //
                                NewPrice[0] = IDOfDrugstore;
                                NewPrice[1] = IDOfProduct;
                                NewPrice[2] = PriceOfProduct;
                                NewPrice[3] = Deleting;
                                NewPrice[4] = AllPrices;
                                //
                                PriceList.Rows.Add(NewPrice);
                            }
                            else { this.RecordingInLogFile(String.Format("ERROR Not Correct Data Of Row Of Price ({0})", CurrentRowOfPrice)); }
                        }
                        else { this.RecordingInLogFile("ERROR Bad Row Of Price"); }
                    }
                }
                else { this.RecordingInLogFile("ERROR Text Of File PriceList is Null"); }
            }
            else { this.RecordingInLogFile("ERROR ID Of Drugstore is 0"); }
            //
            // Return
            //
            return PriceList;
        }        

        #endregion

        #region ' Service '

        private DataTable CreatingTableOfPriceList()
        {
            DataTable TableOfPrice = new DataTable("Prices");
            //
            TableOfPrice.Columns.Add("ID_PH", typeof(int));
            TableOfPrice.Columns.Add("ID_PR", typeof(int));
            TableOfPrice.Columns.Add("Price", typeof(decimal));
            TableOfPrice.Columns.Add("Deleting", typeof(bool));
            TableOfPrice.Columns.Add("AllPrices", typeof(bool));
            TableOfPrice.Columns.Add("Preferential", typeof(bool));
            //
            TableOfPrice.Columns["Preferential"].DefaultValue = false;
            //
            return TableOfPrice;
        }

        private string ReplaceSeparator(string StringOfProcessing)
        {
            //
            string SeparatorOfDecimalNumber =
                System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            //
            if (SeparatorOfDecimalNumber != ",")
                if (StringOfProcessing.IndexOf(",") >= 0)
                    StringOfProcessing = StringOfProcessing.Replace(",", SeparatorOfDecimalNumber);
            //
            if (SeparatorOfDecimalNumber != ".")
                if (StringOfProcessing.IndexOf(".") >= 0)
                    StringOfProcessing = StringOfProcessing.Replace(".", SeparatorOfDecimalNumber);
            //
            return StringOfProcessing;
        }

        #endregion

    }
}