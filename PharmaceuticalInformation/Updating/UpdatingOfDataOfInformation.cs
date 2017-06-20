using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using PharmaceuticalInformation.BaseTypes;
using Test_pharm_server;

namespace PharmaceuticalInformation.Updating
{
    public abstract class UpdatingOfDataOfInformation : BaseType
    {

        #region ' Fields '

        //// Connection To Base
        //protected System.Data.Common.DbConnection ConnectionToBase;
        //// Updating Of Data
        //protected System.Data.Common.DbDataAdapter _UpdatingOfData;

        protected PhrmInfTESTEntities PhrmInf;

        #endregion

        #region ' Designer '

        public UpdatingOfDataOfInformation(string StringOfConnection)
        {
            //
            // Creating Of Connection
            //
            try
            {
                //ConnectionToBase = CreatingConnection(StringOfConnection);
                try
                {
                    PhrmInf = new PhrmInfTESTEntities(StringOfConnection);
                    //ConnectionToBase.Open();
                    //ConnectionToBase.Close();
                }
                catch (Exception E) { throw new Exception(String.Format("Ошибка при открытии подключения обновления: {0}", E)); }
            }
            catch (Exception E) { throw new Exception(String.Format("Ошибка при создании подключения обновления: {0}", E)); }
            //
            // Initializing UpdatingOfData
            //
            //_UpdatingOfData = CreatingDataAdapter();
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

        //// Checking Of Data Of Filled
        //protected virtual bool CheckingOfData(DataTable DataForChecking)
        //{
        //    //
        //    bool ResultOfChecking = false;
        //    //
        //    string[] NamesOfTables = new string[7] 
        //    { "Pharmacy", "GroupsOfProducts", "Products", "PriceList", "FullUpdatingOfDates", "CountOfExported", "Announcements" };
        //    //
        //    if (DataForChecking != null)
        //    {
        //        string NameOfChecking = "";
        //        //
        //        foreach (string CurrentTableName in NamesOfTables)
        //            if (CurrentTableName == DataForChecking.TableName)
        //                NameOfChecking = CurrentTableName;
        //        //
        //        if (NameOfChecking != "")
        //        {
        //            //
        //            DataForChecking.AcceptChanges();
        //            //
        //            if (DataForChecking.Rows.Count > 0)
        //                ResultOfChecking = true;
        //            else
        //                ResultOfChecking = false;
        //        }
        //        else
        //            ResultOfChecking = false;
        //    }
        //    else
        //        ResultOfChecking = true;
        //    //
        //    // Return
        //    //
        //    return ResultOfChecking;
        //}

        // Clear
        public virtual void ClearingOfTableOfPriceList()
        {
            //
        }

        //// Filled
        //public void ExecutingCommands(string[] TextOfCommands)
        //{
        //    //
        //    DbCommand CommandOfClearing = CreatingCommand("", new DbParameter[0]);
        //    CommandOfClearing.CommandTimeout = 1000;
        //    //
        //    foreach (string CurrentTextOfCommand in TextOfCommands)
        //    {
        //        CommandOfClearing.CommandText = CurrentTextOfCommand;
        //        //
        //        try
        //        {
        //            CommandOfClearing.Connection.Open();
        //            //
        //            try { CommandOfClearing.ExecuteNonQuery(); }
        //            catch (Exception E) { ReturningMessageAboutError("Ошибка при зачистке таблицы PriceList", E, false); }
        //            //
        //            CommandOfClearing.Connection.Close();
        //        }
        //        catch (Exception E)
        //        {
        //            //
        //            if (CommandOfClearing.Connection.State == ConnectionState.Open)
        //                CommandOfClearing.Connection.Close();
        //            //
        //            ReturningMessageAboutError("Ошибка при открытии подключения зачистки", E, false);
        //        }
        //    }
        //    //
        //}

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