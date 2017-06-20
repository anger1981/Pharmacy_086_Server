using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PharmaceuticalInformation.BaseTypes;
using System.Linq;
using EntityFramework.Extensions;

namespace PharmaceuticalInformation.Updating
{
    public class UpdatingOfDataOfInformationForMsSQL : UpdatingOfDataOfInformation
    {

        #region ' Designer '

        public UpdatingOfDataOfInformationForMsSQL(string StringOfConnection)
            : base(StringOfConnection)
        {
            //
        }

        public UpdatingOfDataOfInformationForMsSQL(string StringOfConnection, string PathToLogFile)
            : base(StringOfConnection, PathToLogFile)
        {
            //
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

    }
}