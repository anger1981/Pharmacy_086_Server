using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Updating
{
    public abstract class UpdatingOfDataOfInformation : BaseType
    {

        #region ' Fields '

        // Connection To Base
        protected System.Data.Common.DbConnection ConnectionToBase;
        // Updating Of Data
        protected System.Data.Common.DbDataAdapter _UpdatingOfData;

        #endregion

        #region ' Designer '

        public UpdatingOfDataOfInformation(string StringOfConnection)
        {
            //
            // Creating Of Connection
            //
            try
            {
                ConnectionToBase = CreatingConnection(StringOfConnection);
                try
                {
                    ConnectionToBase.Open();
                    ConnectionToBase.Close();
                }
                catch (Exception E) { throw new Exception(String.Format("Ошибка при открытии подключения обновления: {0}", E)); }
            }
            catch (Exception E) { throw new Exception(String.Format("Ошибка при создании подключения обновления: {0}", E)); }
            //
            // Initializing UpdatingOfData
            //
            _UpdatingOfData = CreatingDataAdapter();
        }

        public UpdatingOfDataOfInformation(string StringOfConnection, string PathToLogFile)
            : this(StringOfConnection)
        {
            this.PathToLogFile = PathToLogFile;
        }

        #endregion

        #region ' Creating '

        // Clear
        abstract protected DbConnection CreatingConnection(string StringOfConnection);

        // Clear
        abstract protected DbCommand CreatingCommand(string TextOfCommand, DbParameter[] ParametersOfCommand);

        // Clear
        abstract protected DbDataAdapter CreatingDataAdapter();

        #endregion

        #region ' Updating '

        // Filled
        public virtual void UpdatingOfData(DataSet DataOfUpdating)
        {
            //
            string[] NamesOfTables = new string[6] 
            { "District", "Pharmacy", "GroupsOfProducts", "Products", "PriceList", "FullUpdatingOfDates" };
            //
            if (DataOfUpdating != null)
                if (DataOfUpdating.Tables.Contains("DateOfExported") && 
                    DataOfUpdating.Tables.Contains("NumberOfExported"))
                    if ((DataOfUpdating.Tables["DateOfExported"].Rows.Count > 0) && 
                        (DataOfUpdating.Tables["DateOfExported"].Columns.Count > 0) &&
                        (DataOfUpdating.Tables["NumberOfExported"].Rows.Count > 0) &&
                        (DataOfUpdating.Tables["NumberOfExported"].Columns.Count > 0))
                    {
                        foreach (DataTable CurrentTable in DataOfUpdating.Tables)
                        {
                            /*
                            RecordingInLogFile(
                                   String.Format("TableName={0} Rows={1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                            */
                            if (CurrentTable.TableName == "NumberOfExported")
                                RecordingInLogFile(String.Format("Number Of Exported={0} (String Of Numbers={1})",
                                    CurrentTable.Rows[0]["NumberOfExported"], 
                                    CurrentTable.Rows[0]["StringOfNumbers"]));
                            //
                            if (CurrentTable.TableName == "DateOfExported")
                                RecordingInLogFile(String.Format("DateOfExported={0}", CurrentTable.Rows[0][0]));
                        }

                        //
                        DateTime DateOfExported = new DateTime(0);
                        int NumberOfExported = 0;
                        try
                        { 
                            DateOfExported = ((DateTime)DataOfUpdating.Tables["DateOfExported"].Rows[0][0]);
                            //
                            NumberOfExported = ((int)DataOfUpdating.Tables["NumberOfExported"].Rows[0]["NumberOfExported"]);  
                        }
                        catch (Exception E) { ReturningMessageAboutError("Ошибка при извлечении даты экспортирования", E, false); }
                        //
                        if ((DateOfExported.Ticks != 0) && (NumberOfExported > 0))
                        {
                            foreach (string CurrentTableName in NamesOfTables)
                                if (DataOfUpdating.Tables.Contains(CurrentTableName))
                                    switch (CurrentTableName)
                                    {
                                        case "District":
                                            UpdatingOfDistricts(DataOfUpdating.Tables[CurrentTableName]);
                                            break;
                                        case "Pharmacy":
                                            UpdatingOfPharmacy(DataOfUpdating.Tables[CurrentTableName]);
                                            break;
                                        case "GroupsOfProducts":
                                            UpdatingOfGroupsOfProducts(DataOfUpdating.Tables[CurrentTableName]);
                                            break;
                                        case "Products":
                                            UpdatingOfProducts(DataOfUpdating.Tables[CurrentTableName]);
                                            break;
                                        case "PriceList":
                                            UpdatingOfPriceList(DataOfUpdating.Tables[CurrentTableName]);
                                            break;
                                        case "FullUpdatingOfDates":
                                            UpdatingOfDatesOfPriceList(DataOfUpdating.Tables[CurrentTableName]);
                                            break;
                                    }
                            //
                            ClearingOfTableOfPriceList();
                            //
                            //this.RecordingInLogFile("");
                            //
                            UpdatingDateAndNumberOfUpdatingARH(DateOfExported, NumberOfExported);
                        }
                    }
        }

        // Filled
        public virtual void UpdatingOfData02(DataSet DataOfUpdating)
        {
            //
            if (DataOfUpdating != null)
            {
                //
                // Reading Data Of Pack
                //
                DateTime DateOfExported = new DateTime(0);
                int NumberOfExported = 0;
                //
                if (DataOfUpdating.Tables.Contains("DateOfExported") &&
                    DataOfUpdating.Tables.Contains("NumberOfExported"))
                    if ((DataOfUpdating.Tables["DateOfExported"].Rows.Count > 0) &&
                        (DataOfUpdating.Tables["DateOfExported"].Columns.Count > 0) &&
                        (DataOfUpdating.Tables["NumberOfExported"].Rows.Count > 0) &&
                        (DataOfUpdating.Tables["NumberOfExported"].Columns.Count > 0))
                    {
                        //
                        try
                        {
                            //
                            DateOfExported = ((DateTime)DataOfUpdating.Tables["DateOfExported"].Rows[0][0]);
                            //
                            NumberOfExported = ((int)DataOfUpdating.Tables["NumberOfExported"].Rows[0]["DateOfExported"]);
                        }
                        catch (Exception E) { ReturningMessageAboutError("Ошибка при извлечении даты экспортирования", E, false); }
                    }
                //
                // !!!
                //
                RecordingInLogFile(
                    String.Format("Number Of Pack={0} Date Of Pack={1}", NumberOfExported, DateOfExported));
                //
                // Updating Tables
                //
                if ((DateOfExported.Ticks != 0) && (NumberOfExported > 0))
                {
                    //
                    foreach (DataTable CurrentTable in DataOfUpdating.Tables)
                        switch (CurrentTable.TableName)
                        {
                            case "District":
                                UpdatingOfDistricts(CurrentTable);
                                break;
                            case "Pharmacy":
                                UpdatingOfPharmacy(CurrentTable);
                                break;
                            case "GroupsOfProducts":
                                UpdatingOfGroupsOfProducts(CurrentTable);
                                break;
                            case "Products":
                                UpdatingOfProducts(CurrentTable);
                                break;
                            /*case "PriceList":
                                UpdatingOfPriceList(CurrentTable);
                                break;*/
                            /*case "FullUpdatingOfDates":
                                UpdatingOfDatesOfPriceList(CurrentTable);
                                break;*/
                            /*case "CountOfExported":
                                UpdatingOfPublishedOfAnnouncements(CurrentTable);
                                break;*/
                            case "Announcements":
                                UpdatingOfPublishedOfAnnouncements(CurrentTable);
                                break;
                        }
                    //
                    //ClearingOfTableOfPriceList();
                    //
                    this.RecordingInLogFile("");
                    //
                    //UpdatingDateAndNumberOfUpdating(DateOfExported, NumberOfExported);
                    UpdatingNumberOfUpdating(NumberOfExported);
                }
            }
        }

        // Clear
        public virtual void UpdatingOfDistricts(DataTable DataForDistricts)
        {
            //
        }

        // Clear
        public virtual void UpdatingOfPharmacy(DataTable DataForDrugstores)
        {
            //
        }

        // Clear
        public virtual void UpdatingOfGroupsOfProducts(DataTable DataForGroupsOfProducts)
        {
            //
        }

        // Clear
        public virtual void UpdatingOfProducts(DataTable DataForProducts)
        {
            //
        }

        // Clear
        public virtual void UpdatingOfPriceList(DataTable DataForPriceList)
        {
            //
        }

        // Empty For Overriding
        public virtual void UpdatingOfPriceLists(string PathToFileOfPriceLists, int NumberOfUpdatingOfPriceLists)
        {
            //
        }

        // Clear
        public virtual void UpdatingOfDatesOfPriceList(DataTable DataForUpdating)
        {
            //
        }

        // Empty For Overriding
        public virtual void UpdatingOfDatesOfPriceList02(DataTable DatesForUpdating)
        {
            //
        }

        // Clear
        public virtual void UpdatingOfPublishedOfAnnouncements(DataTable DataForUpdating)
        {
            //
        }

        // Clear
        public virtual void UpdatingDateAndNumberOfUpdatingARH(DateTime DateOfUpdating, int NumberOfExported)
        {
            //
        }

        // Empty For Overriding
        public virtual void UpdatingDateOfActual(DateTime DateOfActual)
        {
            //
        }

        // Empty For Overriding
        public virtual void UpdatingNumberOfUpdating(int NumberOfUpdating)
        {
            //
        }

        // Empty For Overriding
        public virtual void UpdatingNumberOfUpdatingPriceLists(int NumberOfUpdating)
        {
            //
        }

        #endregion

        #region ' Service '

        // Checking Of Data Of Filled
        protected virtual bool CheckingOfData(DataTable DataForChecking)
        {
            //
            bool ResultOfChecking = false;
            //
            string[] NamesOfTables = new string[7] 
            { "Pharmacy", "GroupsOfProducts", "Products", "PriceList", "FullUpdatingOfDates", "CountOfExported", "Announcements" };
            //
            if (DataForChecking != null)
            {
                string NameOfChecking = "";
                //
                foreach (string CurrentTableName in NamesOfTables)
                    if (CurrentTableName == DataForChecking.TableName)
                        NameOfChecking = CurrentTableName;
                //
                if (NameOfChecking != "")
                {
                    //
                    DataForChecking.AcceptChanges();
                    //
                    if (DataForChecking.Rows.Count > 0)
                        ResultOfChecking = true;
                    else
                        ResultOfChecking = false;
                }
                else
                    ResultOfChecking = false;
            }
            else
                ResultOfChecking = true;
            //
            // Return
            //
            return ResultOfChecking;
        }

        // Clear
        public virtual void ClearingOfTableOfPriceList()
        {
            //
        }

        // Filled
        public void ExecutingCommands(string[] TextOfCommands)
        {
            //
            DbCommand CommandOfClearing = CreatingCommand("", new DbParameter[0]);
            CommandOfClearing.CommandTimeout = 1000;
            //
            foreach (string CurrentTextOfCommand in TextOfCommands)
            {
                CommandOfClearing.CommandText = CurrentTextOfCommand;
                //
                try
                {
                    CommandOfClearing.Connection.Open();
                    //
                    try { CommandOfClearing.ExecuteNonQuery(); }
                    catch (Exception E) { ReturningMessageAboutError("Ошибка при зачистке таблицы PriceList", E, false); }
                    //
                    CommandOfClearing.Connection.Close();
                }
                catch (Exception E)
                {
                    //
                    if (CommandOfClearing.Connection.State == ConnectionState.Open)
                        CommandOfClearing.Connection.Close();
                    //
                    ReturningMessageAboutError("Ошибка при открытии подключения зачистки", E, false);
                }
            }
            //
        }

        // Filled
        public DataSet AssociationDateSet(DataSet[] ListForAssociation)
        {
            if (ListForAssociation.Length > 0)
            {
                //
                DataSet ReturnAssociationData = ListForAssociation[0].Copy();
                for (int i = 0; i < ReturnAssociationData.Tables.Count; i++)
                {
                    ReturnAssociationData.Tables[i].Clear();
                    ReturnAssociationData.Tables[i].AcceptChanges();
                }
                //
                ReturnAssociationData.Tables["NumberOfExported"].Columns["DateOfExported"].ColumnName = "NumberOfExported2";
                ReturnAssociationData.Tables["NumberOfExported"].Columns.Add("StringOfNumbers", typeof(string));
                ReturnAssociationData.Tables["NumberOfExported"].Columns.Remove("NumberOfExported2");
                ReturnAssociationData.Tables["NumberOfExported"].Columns.Add("NumberOfExported", typeof(int));
                ReturnAssociationData.Tables["NumberOfExported"].Rows.Add("", 0);
                //
                ReturnAssociationData.Tables["DateOfExported"].Rows.Add(DateTime.Now);
                //
                // Creating Constraints
                //
                ReturnAssociationData.Tables["Pharmacy"].Columns["ID"].ReadOnly = false;
                DataColumn[] KeyOfDrugstore =
                    new DataColumn[] { ReturnAssociationData.Tables["Pharmacy"].Columns["ID"] };
                ReturnAssociationData.Tables["Pharmacy"].PrimaryKey = KeyOfDrugstore;
                //
                ReturnAssociationData.Tables["Products"].Columns["ID"].ReadOnly = false;
                DataColumn[] KeyOfProduct =
                    new DataColumn[] { ReturnAssociationData.Tables["Products"].Columns["ID"] };
                ReturnAssociationData.Tables["Products"].PrimaryKey = KeyOfProduct;
                //
                ReturnAssociationData.Tables["GroupsOfProducts"].Columns["ID"].ReadOnly = false;
                DataColumn[] KeyOfProductGroup =
                    new DataColumn[] { ReturnAssociationData.Tables["GroupsOfProducts"].Columns["ID"] };
                ReturnAssociationData.Tables["GroupsOfProducts"].PrimaryKey = KeyOfProductGroup;
                //
                ReturnAssociationData.Tables["FullUpdatingOfDates"].Columns["ID"].ReadOnly = false;
                DataColumn[] KeyOfFullUpdatingOfDates =
                    new DataColumn[] { ReturnAssociationData.Tables["FullUpdatingOfDates"].Columns["ID"] };
                ReturnAssociationData.Tables["FullUpdatingOfDates"].PrimaryKey = KeyOfFullUpdatingOfDates;
                //
                DataColumn[] KeyOfPrice =
                    new DataColumn[] { 
                        ReturnAssociationData.Tables["PriceList"].Columns["ID_PH"], 
                        ReturnAssociationData.Tables["PriceList"].Columns["ID_PR"] };
                ReturnAssociationData.Tables["PriceList"].PrimaryKey = KeyOfPrice;
                //
                ReturnAssociationData.Tables["FullUpdatingOfDates"].Columns["Date"].ReadOnly = false;
                //
                // Filling Of DataSet Of Returning
                //
                for (int i = 0; i < ListForAssociation.Length; i++)
                {
                    //
                    // Summing Of Numbers And Date
                    //
                    ReturnAssociationData.Tables["NumberOfExported"].Rows[0]["StringOfNumbers"] +=
                        " " + ListForAssociation[i].Tables["NumberOfExported"].Rows[0]["DateOfExported"].ToString();
                    //
                    ReturnAssociationData.Tables["NumberOfExported"].Rows[0]["NumberOfExported"] = 
                        (int) ListForAssociation[i].Tables["NumberOfExported"].Rows[0]["DateOfExported"];
                    //
                    ReturnAssociationData.Tables["DateOfExported"].Rows[0]["DateOfExported"] =
                        ListForAssociation[i].Tables["DateOfExported"].Rows[0]["DateOfExported"];
                    //
                    // Summing Data
                    //
                    for (int i2 = 0; i2 < ListForAssociation[i].Tables.Count; i2++)
                    {
                        DataTable CurrentTable = ListForAssociation[i].Tables[i2];
                        if ((CurrentTable.TableName != "DateOfExported") && (CurrentTable.TableName != "NumberOfExported") &&
                            (CurrentTable.TableName != "CountOfExported"))
                            for (int i3 = 0; i3 < CurrentTable.Rows.Count; i3++)
                            {
                                //
                                DataRow RowInTable = null;
                                // Exists Row
                                if (CurrentTable.TableName == "PriceList")
                                {
                                    int IDOfDrugstore = (int)CurrentTable.Rows[i3]["ID_PH"];
                                    int IDOfProduct = (int)CurrentTable.Rows[i3]["ID_PR"];
                                    object[] KeyRow = new object[] { IDOfDrugstore, IDOfProduct };
                                    RowInTable =
                                        ReturnAssociationData.Tables[CurrentTable.TableName].Rows.Find(KeyRow);
                                }
                                else
                                {
                                    RowInTable =
                                        ReturnAssociationData.Tables[CurrentTable.TableName].Rows.Find(
                                        (int)CurrentTable.Rows[i3][0]);
                                }
                                //
                                if (RowInTable == null)
                                {
                                    //
                                    DataRow NewRow = ReturnAssociationData.Tables[CurrentTable.TableName].NewRow();
                                    NewRow.ItemArray = CurrentTable.Rows[i3].ItemArray;
                                    ReturnAssociationData.Tables[CurrentTable.TableName].Rows.Add(NewRow);
                                }
                                else
                                { RowInTable.ItemArray = CurrentTable.Rows[i3].ItemArray; }
                                //
                            }
                    }
                }
                // Return
                return ReturnAssociationData;
            }
            else
                // Retrun
                return null;
        }

        // Filled
        public virtual int[] FilteringOnNumberOfExported(int[] NumbersOfExported)
        {
            //
            DbCommand CommandOfGettingNumber = CreatingCommand(
                "SELECT Value FROM Service WHERE Id_Service = 9;", new DbParameter[0]);
            //
            int NumberOfUpdating = -1;
            ArrayList _NumbersForUpdating = new ArrayList();
            int[] NumbersForUpdating = new int[0];
            //
            try
            {
                CommandOfGettingNumber.Connection.Open();
                NumberOfUpdating = (int) CommandOfGettingNumber.ExecuteScalar();
                CommandOfGettingNumber.Connection.Close();
            }
            catch (Exception E) 
            {
                //
                if (CommandOfGettingNumber.Connection.State == ConnectionState.Open)
                    CommandOfGettingNumber.Connection.Close();
                ReturningMessageAboutError("Ошибка при чтении номера импортирования", E, false);
            }
            //
            if (NumberOfUpdating >= 0)
                foreach (int CurrentNumber in NumbersOfExported)
                    if (CurrentNumber > NumberOfUpdating)
                        _NumbersForUpdating.Add(CurrentNumber);
            //
            _NumbersForUpdating.Sort();
            NumbersForUpdating = (int[]) _NumbersForUpdating.ToArray(typeof(int));
            // Return
            return NumbersForUpdating;
        }

        // Filled
        public virtual int[] FilteringOnNumberOfExported02(int[] NumbersOfExported)
        {
            //
            // Creating
            //
            DbCommand CommandOfGettingNumber = CreatingCommand(
                "SELECT Value FROM Service WHERE Id_Service = 9;", new DbParameter[0]);
            //
            int NumberOfUpdating = -1;
            ArrayList _NumbersForUpdating = new ArrayList();
            int[] NumbersForUpdating = new int[0];
            //
            // Getting Number Of Updating
            //
            try
            {
                //
                CommandOfGettingNumber.Connection.Open();
                //
                NumberOfUpdating = (int)CommandOfGettingNumber.ExecuteScalar();
                //
                CommandOfGettingNumber.Connection.Close();
            }
            catch (Exception E)
            {
                //
                if (CommandOfGettingNumber.Connection.State == ConnectionState.Open)
                    CommandOfGettingNumber.Connection.Close();
                //
                ReturningMessageAboutError("Ошибка при чтении номера импортирования", E, false);
            }
            //
            // Filtering
            //
            if (NumberOfUpdating >= 0)
                foreach (int CurrentNumber in NumbersOfExported)
                    if ((CurrentNumber > NumberOfUpdating) && (CurrentNumber > 15390))
                        _NumbersForUpdating.Add(CurrentNumber);
            //
            // Sorting
            //
            _NumbersForUpdating.Sort();
            NumbersForUpdating = (int[])_NumbersForUpdating.ToArray(typeof(int));
            //
            // Return
            //
            return NumbersForUpdating;
        }

        // Filled
        public virtual int FilteringOfListsOfExportedPriceLists(int[] ListOfNumbersOfUpdatingOfPriceLists)
        {
            //
            // Getting Number Of Updating Of PriceLists
            //
            int NumberOfUpdating = -1;
            //
            DbCommand CommandOfGettingNumber = CreatingCommand(
                "SELECT Value FROM Service WHERE Id_Service = 4;", new DbParameter[0]);
            //
            try
            {
                //
                CommandOfGettingNumber.Connection.Open();
                //
                NumberOfUpdating = (int)CommandOfGettingNumber.ExecuteScalar();
                //
                CommandOfGettingNumber.Connection.Close();
            }
            catch (Exception E)
            {
                //
                if (CommandOfGettingNumber.Connection.State == ConnectionState.Open)
                    CommandOfGettingNumber.Connection.Close();
                //
                this.RecordingInLogFile(String.Format("ERROR Ошибка при чтении номера обновления Прайс-Листов: {0}", E.Message));
            }
            //
            // Filtering
            //
            ArrayList ListOfGreaterNumbersOfUpdating = new ArrayList();
            //
            foreach (int CurrentNumber in ListOfNumbersOfUpdatingOfPriceLists)
                if (CurrentNumber > NumberOfUpdating)
                    ListOfGreaterNumbersOfUpdating.Add(CurrentNumber);
            //
            if (ListOfGreaterNumbersOfUpdating.Count > 0)
            {
                //
                ListOfGreaterNumbersOfUpdating.Sort();
                //
                NumberOfUpdating = 
                    (int)ListOfGreaterNumbersOfUpdating[ListOfGreaterNumbersOfUpdating.Count - 1];
            }
            else
            { NumberOfUpdating = -1; }
            //
            // Return
            //
            return NumberOfUpdating;
        }

        // Clear
        protected virtual void SetStatusOfRows(DataTable TableForStatus,
            string TextOfCheckingOfDeleting, string TextOfCheckingOfExisting, DbParameter[] ParametersOfCommand)
        {
            //
        }

        #endregion

        #region ' ??? '

        protected DataTable GeneratingTable(string NameOfTable)
        {
            return new DataTable(NameOfTable);
        }

        #endregion

    }
}