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
        //private Updating.UpdatingOfDataOfInformationForMsSQL UpdatingOfHelp;
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
            //UpdatingOfHelp = new Updating.UpdatingOfDataOfInformationForMsSQL(StringOfConnectionToHelp, this.PathToLogFile);
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
            //UpdatingOfHelp.ShowingMessages = false;
            ExportingOfIS.ShowingMessages = false;
            ImportingOfIS.ShowingMessages = false;
        }

        #endregion

    }
}