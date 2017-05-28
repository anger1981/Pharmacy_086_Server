using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Updating
{
    public class UpdatingOfDataOfInformationForMsSQL : UpdatingOfDataOfInformation
    {

        #region ' Fields '

        //
        //private string StringOfConnection1;

        #endregion

        #region ' Designer '

        public UpdatingOfDataOfInformationForMsSQL(string StringOfConnection)
            : base(StringOfConnection)
        {
            //
            //this.StringOfConnection = StringOfConnection;
        }

        public UpdatingOfDataOfInformationForMsSQL(string StringOfConnection, string PathToLogFile)
            : base(StringOfConnection, PathToLogFile)
        {
            //
            //this.StringOfConnection = StringOfConnection;
        }

        #endregion

        #region ' Creating '

        protected override DbConnection CreatingConnection(string StringOfConnection)
        {
            // Return
            return new SqlConnection(StringOfConnection);
        }

        protected override DbCommand CreatingCommand(string TextOfCommand, DbParameter[] ParametersOfCommand)
        {
            //
            DbCommand CreatedCommand = new SqlCommand(TextOfCommand, (SqlConnection)ConnectionToBase);
            //
            for (int i = 0; i <= ParametersOfCommand.GetUpperBound(0); i++)
                CreatedCommand.Parameters.Add(ParametersOfCommand[i]);
            // Return
            return CreatedCommand;
        }

        protected override DbDataAdapter CreatingDataAdapter()
        {
            // Return
            return new SqlDataAdapter();
        }

        #endregion

        #region ' Updating '

        public override void UpdatingOfPharmacy(DataTable DataForDrugstores)
        {
            if (CheckingOfData(DataForDrugstores))
            {
                //
                //DataForDrugstores = DataForDrugstores.Copy();
                //
                // Creating Command Of Reading Of Status Of Rows
                //
                DbParameter[] ParametersOfSelectionCommand = new DbParameter[1] { 
                    new SqlParameter("@P1", SqlDbType.Int, 0, "ID") };
                SetStatusOfRows(DataForDrugstores,
                    "", "EXISTS(SELECT Id_Pharmacy FROM Pharmacy WHERE Id_Pharmacy = @P1)", ParametersOfSelectionCommand);
                //
                // Creating Insert Command
                //
                DbParameter[] ParametersOfInsertingCommand = new DbParameter[10] { 
                    new SqlParameter("@P1", SqlDbType.Int,     0, "ID"), 
                    new SqlParameter("@P2", SqlDbType.Int,     0, "ID_DI"), 
                    new SqlParameter("@P3", SqlDbType.VarChar, 0, "Name"), 
                    new SqlParameter("@P4", SqlDbType.VarChar, 0, "Address"), 
                    new SqlParameter("@P5", SqlDbType.VarChar, 0, "Phone"), 
                    new SqlParameter("@P6", SqlDbType.VarChar, 0, "Mail"), 
                    new SqlParameter("@P7", SqlDbType.VarChar, 0, "Site"), 
                    new SqlParameter("@P8", SqlDbType.VarChar, 0, "Schedule"), 
                    new SqlParameter("@P9", SqlDbType.VarChar, 0, "Transport"), 
                    new SqlParameter("@P10",SqlDbType.Bit,     0, "Deleting") };
                //
                SqlCommand CommandOfInsertingPharmacy = (SqlCommand)CreatingCommand(
                    "SET IDENTITY_INSERT Pharmacy ON; " +
                    "INSERT INTO Pharmacy(Id_Pharmacy, Id_District, Name_full, Addr, Phone, Mail, Web, Hours, Trans, Is_deleted) " +
                    "VALUES(@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10); " +
                    "SET IDENTITY_INSERT Pharmacy OFF;",
                    ParametersOfInsertingCommand);
                //
                // Creating Update Command
                //
                DbParameter[] ParametersOfUpdatingCommand = new DbParameter[10] { 
                    new SqlParameter("@P1",  SqlDbType.Int,      0, "ID"), 
                    new SqlParameter("@P2",  SqlDbType.Int,      0, "ID_DI"), 
                    new SqlParameter("@P3",  SqlDbType.VarChar,  0, "Name"), 
                    new SqlParameter("@P4",  SqlDbType.VarChar,  0, "Address"), 
                    new SqlParameter("@P5",  SqlDbType.VarChar,  0, "Phone"), 
                    new SqlParameter("@P6",  SqlDbType.VarChar,  0, "Mail"), 
                    new SqlParameter("@P7",  SqlDbType.VarChar,  0, "Site"), 
                    new SqlParameter("@P8",  SqlDbType.VarChar,  0, "Schedule"), 
                    new SqlParameter("@P9",  SqlDbType.VarChar,  0, "Transport"), 
                    new SqlParameter("@P10", SqlDbType.Bit,      0, "Deleting") };
                //
                // new SqlParameter("@P11", SqlDbType.DateTime, 0, "Updating")  Date_upd = @P11
                SqlCommand CommandOfUpdatingPharmacy = (SqlCommand)CreatingCommand(
                    "UPDATE Pharmacy " +
                    "SET Id_District = @P2, Name_full = @P3, Addr = @P4, Phone = @P5, Mail = @P6, Web = @P7, Hours = @P8, Trans = @P9, Is_deleted = @P10 " + 
                    "WHERE Id_Pharmacy = @P1;",
                    ParametersOfUpdatingCommand);
                //
                // Assignment Of Commands
                //
                _UpdatingOfData.InsertCommand = CommandOfInsertingPharmacy;
                _UpdatingOfData.UpdateCommand = CommandOfUpdatingPharmacy;
                //
                // Updating
                //
                UpdateOfUpdatingData(DataForDrugstores, "Pharmacy");
            }
                /*
            else
                this.RecordingInLogFile("Не корректная структура таблицы Pharmacy");
            */
        }

        public override void UpdatingOfProducts(DataTable DataForProducts)
        {
            if (CheckingOfData(DataForProducts))
            {
                //
                DataForProducts = DataForProducts.Copy();
                //
                // Creating Command Of Reading Of Status Of Rows
                //
                DbParameter[] ParametersOfSelectionCommand = new DbParameter[1] { 
                    new SqlParameter("@P1", SqlDbType.Int, 0, "ID") };
                SetStatusOfRows(DataForProducts,
                    "", "EXISTS(SELECT Id_Product FROM Product WHERE Id_Product = @P1)", ParametersOfSelectionCommand);
                //
                // Creating Insert Command
                //
                DbParameter[] ParametersOfInsertingCommand = new DbParameter[7] { 
                    new SqlParameter("@P1", SqlDbType.Int,      0, "ID"), 
                    new SqlParameter("@P2", SqlDbType.Int,      0, "ID_PG"), 
                    new SqlParameter("@P3", SqlDbType.VarChar,  0, "Name"), 
                    new SqlParameter("@P4", SqlDbType.VarChar,  0, "Composition"), 
                    new SqlParameter("@P5", SqlDbType.VarChar,  0, "Description"), 
                    new SqlParameter("@P6", SqlDbType.DateTime, 0, "Updating"), 
                    new SqlParameter("@P7", SqlDbType.Bit,      0, "Deleting") };
                //
                SqlCommand CommandOfInsertingProduct = (SqlCommand)CreatingCommand(
                    "SET IDENTITY_INSERT Product ON; " +
                    "INSERT INTO Product(Id_Product, Id_product_group, Name_full, Composition, Description, Date_upd, Is_deleted) " +
                    "VALUES(@P1, @P2, @P3, @P4, @P5, @P6, @P7); " +
                    "SET IDENTITY_INSERT Product OFF;",
                    ParametersOfInsertingCommand);
                //
                // Creating Updating Command
                //
                DbParameter[] ParametersOfUpdatingCommand = new DbParameter[7] { 
                    new SqlParameter("@P1", SqlDbType.Int,      0, "ID"), 
                    new SqlParameter("@P2", SqlDbType.Int,      0, "ID_PG"), 
                    new SqlParameter("@P3", SqlDbType.VarChar,  0, "Name"), 
                    new SqlParameter("@P4", SqlDbType.VarChar,  0, "Composition"), 
                    new SqlParameter("@P5", SqlDbType.VarChar,  0, "Description"), 
                    new SqlParameter("@P6", SqlDbType.DateTime, 0, "Updating"), 
                    new SqlParameter("@P7", SqlDbType.Bit,      0, "Deleting") };
                //
                SqlCommand CommandOfUpdatingProduct = (SqlCommand)CreatingCommand(
                    "UPDATE Product " +
                    "SET Id_product_group = @P2, Name_full = @P3, Composition = @P4, Description = @P5, Date_upd = @P6, Is_deleted = @P7 " +
                    "WHERE Id_Product = @P1;",
                    ParametersOfUpdatingCommand);
                //
                // Assignment Of Commands
                //
                _UpdatingOfData.InsertCommand = CommandOfInsertingProduct;
                _UpdatingOfData.UpdateCommand = CommandOfUpdatingProduct;
                /*//
                // Updating
                //
                try { _UpdatingOfData.Update(DataForProducts); }
                catch (Exception E) { ReturningMessageAboutError("Ошибка при обновлении таблицы Product", E, false); }
                //
                // Clearing Of UpdatingOfData 
                //
                _UpdatingOfData.InsertCommand = null;
                _UpdatingOfData.UpdateCommand = null;*/
                //
                UpdateOfUpdatingData(DataForProducts, "Products");
            }
                /*
            else
                this.RecordingInLogFile("Не корректная структура таблицы Products");
            */
        }

        public override void UpdatingOfPriceList(DataTable DataForPriceList)
        {
            if (CheckingOfData(DataForPriceList))
            {
                //
                //DataForPriceList = DataForPriceList.Copy();
                //
                // Creating Command Of Reading Of Status Of Rows
                //
                DbParameter[] ParametersOfSelectionCommand = new DbParameter[3] {
                    new SqlParameter("@P1", SqlDbType.Int, 0, "ID_PH"), 
                    new SqlParameter("@P2", SqlDbType.Int, 0, "ID_PR"), 
                    new SqlParameter("@P3", SqlDbType.Bit, 0, "Deleting") };
                //
                RecordingInLogFile("Start Reading Status Of PriceList");
                //
                SetStatusOfRows(DataForPriceList,
                    "(@P3 = 1)",
                    "(EXISTS(SELECT * FROM Price_list WHERE ((ID_Pharmacy = @P1) AND (Id_Product = @P2))))",
                    ParametersOfSelectionCommand);
                //
                RecordingInLogFile("End Reading Status Of PriceList");
                //
                // Creating Insert Command
                //
                DbParameter[] ParametersOfInsertingCommand = new DbParameter[6] { 
                    new SqlParameter("@P1", SqlDbType.Int,      0, "ID_PH"), 
                    new SqlParameter("@P2", SqlDbType.Int,      0, "ID_PR"), 
                    new SqlParameter("@P3", SqlDbType.Decimal,  0, "Price"), 
                    new SqlParameter("@P4", SqlDbType.DateTime, 0, "Updating"), 
                    new SqlParameter("@P5", SqlDbType.Bit,      0, "Preferential"), 
                    new SqlParameter("@P6", SqlDbType.Bit,      0, "Deleting") };
                //
                SqlCommand CommandOfInsertingPriceList = (SqlCommand)CreatingCommand(
                    "IF ((EXISTS(SELECT * FROM Pharmacy WHERE ID_Pharmacy = @P1)) AND " +
                    "(EXISTS(SELECT * FROM Product WHERE ID_Product = @P2))) " +
                    "INSERT INTO Price_list(Id_Pharmacy, ID_Product, Price, Date_upd, Is_privilege, Is_deleted) " +
                    "VALUES (@P1, @P2, @P3, @P4, @P5, @P6);",
                    ParametersOfInsertingCommand);
                //
                // Creating Updating Command
                //
                DbParameter[] ParametersOfUpdatingCommand = new DbParameter[6] { 
                    new SqlParameter("@P1", SqlDbType.Int,      0, "ID_PH"), 
                    new SqlParameter("@P2", SqlDbType.Int,      0, "ID_PR"), 
                    new SqlParameter("@P3", SqlDbType.Decimal,  0, "Price"), 
                    new SqlParameter("@P4", SqlDbType.DateTime, 0, "Updating"), 
                    new SqlParameter("@P5", SqlDbType.Bit,      0, "Preferential"), 
                    new SqlParameter("@P6", SqlDbType.Bit,      0, "Deleting") };
                //
                SqlCommand CommandOfUpdatingPriceList = (SqlCommand)CreatingCommand(
                    "UPDATE Price_list " +
                    "SET Price = @P3, Date_upd = @P4, Is_privilege = @P5, Is_deleted = @P6 " +
                    "WHERE ((Id_Pharmacy = @P1) AND (Id_Product = @P2));",
                    ParametersOfUpdatingCommand);
                //
                // Creating Deleting Command
                //
                DbParameter[] ParametersOfDeletingCommand = new DbParameter[2] { 
                    new SqlParameter("@P1", SqlDbType.Int,     0, "ID_PH"), 
                    new SqlParameter("@P2", SqlDbType.Int,     0, "ID_PR") };
                //
                SqlCommand CommandOfDeletingPriceList = (SqlCommand)CreatingCommand(
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
                UpdateOfUpdatingData(DataForPriceList, "Price_list");
            }
                /*
            else
                this.RecordingInLogFile("Не корректная структура таблицы PriceList");
            */
        }

        public override void UpdatingOfDatesOfPriceList(DataTable DataForUpdating)
        {
            if (CheckingOfData(DataForUpdating))
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
                    new SqlParameter("@P1", SqlDbType.Int,      0, "ID"), 
                    new SqlParameter("@P2", SqlDbType.DateTime, 0, "Date") };
                //
                SqlCommand CommandOfUpdatingDatesOfPriceList = (SqlCommand)CreatingCommand(
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
        }

        private void UpdateOfUpdatingData(DataTable DataForUpdating, string TableName)
        {
            //
            // Updating
            //
            RecordingInLogFile(String.Format("Start Updating Table Of {0}", TableName));
            //
            int CountOfUpdating = 0;
            try { CountOfUpdating = _UpdatingOfData.Update(DataForUpdating); }
            catch (Exception E)
            { ReturningMessageAboutError(String.Format("Ошибка при обновлении таблицы {0}", TableName), E, false); }
            //
            RecordingInLogFile(String.Format("End Updating Table Of {0} ({1})", TableName, CountOfUpdating));
            //
            // Clearing Of UpdatingOfData 
            //
            _UpdatingOfData.InsertCommand = null;
            _UpdatingOfData.UpdateCommand = null;
            _UpdatingOfData.DeleteCommand = null;
        }

        public override void UpdatingDateAndNumberOfUpdatingARH(DateTime DateOfUpdating, int NumberOfExported)
        {
            //
            DbCommand CommandOfUpdating = CreatingCommand(
                "UPDATE Service SET Value = @P1, Date_Service = @P2 WHERE Id_Service = 9;",
                new DbParameter[2] { 
                        new SqlParameter("@P1", SqlDbType.Int), 
                        new SqlParameter("@P2", SqlDbType.DateTime) });
            CommandOfUpdating.CommandTimeout = 1000;
            //
            try
            {
                CommandOfUpdating.Parameters["@P1"].Value = NumberOfExported;
                CommandOfUpdating.Parameters["@P2"].Value = DateOfUpdating;
                CommandOfUpdating.Connection.Open();
                CommandOfUpdating.ExecuteScalar();
                CommandOfUpdating.Connection.Close();
            }
            catch (Exception E)
            {
                if (CommandOfUpdating.Connection.State == ConnectionState.Open)
                    CommandOfUpdating.Connection.Close();
                ReturningMessageAboutError("Ошибка при обновлении даты обновления", E, false);
            }
            //
        }

        #endregion

        #region ' Service '

        public override void ClearingOfTableOfPriceList()
        {
            //
            string[] TextOfCommandOfClearing =
                new string[2]
                { 
                    "DELETE FROM Price_List WHERE Id_Pharmacy IN (SELECT Id_Pharmacy FROM Pharmacy WHERE Is_deleted = 1);", 
                    "DELETE FROM Price_List WHERE Id_Product NOT IN(SELECT Id_Product FROM Product);" 
                };
            //
            ExecutingCommands(TextOfCommandOfClearing);
            //
        }

        protected override void SetStatusOfRows(DataTable TableForStatus,
            string TextOfCheckingOfDeleting, string TextOfCheckingOfExisting, DbParameter[] ParametersOfCommand)
        {
            //
            try
            {
                //
                TableForStatus.Columns.Add("TMP_Status", typeof(string));
                //
                TableForStatus.AcceptChanges();
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    CurrentRow.SetModified();
                //
                string TextOfExistingOfCommand = "";
                if (TextOfCheckingOfDeleting != "")
                    TextOfExistingOfCommand = String.Format(
                        "IF ({0}) SET @Status = 'REM' ELSE IF ({1}) SET @Status = 'MOD' ELSE SET @Status = 'ADD';",
                        TextOfCheckingOfDeleting, TextOfCheckingOfExisting);
                else
                    TextOfExistingOfCommand = String.Format(
                        "IF ({0}) SET @Status = 'MOD' ELSE SET @Status = 'ADD';",
                        TextOfCheckingOfExisting);
                //
                DbParameter[] ParametersOfExistingCommand = 
                    new DbParameter[ParametersOfCommand.Length + 1];
                for (int i = 0; i < ParametersOfCommand.Length; i++)
                    ParametersOfExistingCommand[i] = ParametersOfCommand[i];
                ParametersOfExistingCommand[ParametersOfCommand.Length] = 
                    new SqlParameter("@Status", SqlDbType.VarChar, 3, "TMP_Status");
                //
                DbCommand CommandOfExisting = CreatingCommand(TextOfExistingOfCommand, ParametersOfExistingCommand);
                CommandOfExisting.Parameters["@Status"].Direction = ParameterDirection.Output;
                SqlDataAdapter ReadingStatus = new SqlDataAdapter();
                ReadingStatus.UpdateCommand = (SqlCommand)CommandOfExisting;
                //
                ReadingStatus.Update(TableForStatus);
                //
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    switch (CurrentRow["TMP_Status"].ToString())
                    {
                        case "REM":
                            CurrentRow.Delete();
                            break;
                        case "ADD":
                            CurrentRow.SetAdded();
                            break;
                        case "MOD":
                            CurrentRow.SetModified();
                            break;
                    }
                //
                TableForStatus.Columns.Remove("TMP_Status");
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении статуса строк", E, false); }
        }

        #endregion

    }
}