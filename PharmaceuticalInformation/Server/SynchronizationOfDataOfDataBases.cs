using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Server
{
    public class SynchronizationOfDataOfDataBases : BaseType
    {

        #region ' Fields '

        //
        private SqlConnection ConnectionToHelp;
        private SqlConnection ConnectionToIS;
        //
        private ExportingOfData ExportingOfHelp;
        private Updating.UpdatingOfDataOfInformationForMsSQL UpdatingOfHelp;
        //
        private ExportingOfData ExportingOfIS;
        private ImportingOfData ImportingOfIS;

        #endregion

        #region ' Designer '

        public SynchronizationOfDataOfDataBases(string StringOfConnectionToHelp, string StringOfConnectionToIS, 
            string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Creating Of Connection To Help
            //
            try
            {
                ConnectionToHelp = new SqlConnection(StringOfConnectionToHelp);
                try
                {
                    ConnectionToHelp.Open();
                    ConnectionToHelp.Close();
                }
                catch (Exception E)
                { throw new Exception(String.Format("Ошибка при открытии подключения к БД Справки: {0}", E.Message)); }
            }
            catch (Exception E)
            { throw new Exception(String.Format("Ошибка при создании подключения к БД Справки: {0}", E.Message)); }
            //
            // Creating Of Connection To IS
            //
            try
            {
                ConnectionToIS = new SqlConnection(StringOfConnectionToIS);
                try
                {
                    ConnectionToIS.Open();
                    ConnectionToIS.Close();
                }
                catch (Exception E)
                { throw new Exception(String.Format("Ошибка при открытии подключения к БД ИС: {0}", E.Message)); }
            }
            catch (Exception E)
            { throw new Exception(String.Format("Ошибка при создании подключения к БД ИС: {0}", E.Message)); }
            //
            // Initializing Of Transfer
            //
            ExportingOfHelp = new ExportingOfData(StringOfConnectionToHelp, this.PathToLogFile);
            UpdatingOfHelp = new Updating.UpdatingOfDataOfInformationForMsSQL(StringOfConnectionToHelp, this.PathToLogFile);
            //
            // Initializing Of IS
            //
            ExportingOfIS = new ExportingOfData(StringOfConnectionToIS, this.PathToLogFile);
            ImportingOfIS = new ImportingOfData(StringOfConnectionToIS, this.PathToLogFile);
            //
            // Hiding Messages
            //
            this.ShowingMessages = false;
            ExportingOfHelp.ShowingMessages = false;
            UpdatingOfHelp.ShowingMessages = false;
            ExportingOfIS.ShowingMessages = false;
            ImportingOfIS.ShowingMessages = false;
        }

        #endregion

        #region ' Synchronization '

        // Importing Modifications
        public void ImportingModifications()
        {
            //
            //RecordingInLogFile("Checking Of Importing Modifications");
            //
            DataSet ModifiedData = ExportingOfHelp.ExportingOfModifications();
            //
            if ((ModifiedData.Tables["Pharmacy"].Rows.Count > 0) ||
                (ModifiedData.Tables["Products"].Rows.Count > 0) ||
                (ModifiedData.Tables["PriceList"].Rows.Count > 0) ||
                (ModifiedData.Tables["IDsOfModifications"].Rows.Count > 0))
            {
                //
                RecordingInLogFile("Checking Of Importing Modifications");
                //
                ImportingOfIS.ImportingDataFromServiceOfHelp(ModifiedData);
                //
                ExportingOfHelp.ClearingOfModifications(ModifiedData);
                //
                this.RecordingInLogFile("");
                this.RecordingInLogFile("");
            }
        }

        // Exporting Data Of Updating
        public void ExportingDataOfUpdating()
        {
            //
            //RecordingInLogFile("Checking Of Exporting For Help");
            //
            DataSet DS = ExportingOfIS.Exporting();
            //
            DataSet[] DSs = new DataSet[1] { DS };
            //
            DS = UpdatingOfHelp.AssociationDateSet(DSs);
            //
            if ((DS.Tables["Pharmacy"].Rows.Count > 0) ||
                (DS.Tables["GroupsOfProducts"].Rows.Count > 0) ||
                (DS.Tables["Products"].Rows.Count > 0) ||
                (DS.Tables["PriceList"].Rows.Count > 0) ||
                (DS.Tables["FullUpdatingOfDates"].Rows.Count > 0))
            {
                //
                RecordingInLogFile("Checking Of Exporting For Help");
                //
                UpdatingOfHelp.UpdatingOfData(DS);
                //
                this.RecordingInLogFile("");
                this.RecordingInLogFile("");
            }
        }

        #endregion

    }
}