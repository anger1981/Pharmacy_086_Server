using System;
using System.Collections.Generic;
using System.Text;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Service
{
    public class Reportings : BaseType
    {

        #region ' Fields '

        //

        #endregion

        #region ' Designer 

        public Reportings(string PathToLogFile)
            : base(PathToLogFile)
        {
            //
        }

        #endregion

        #region ' Getting '

        // Getting Path To Log File
        public string GettingPathToLogFile
        {
            get { return this.PathToLogFile; }
        }

        #endregion

        #region ' Messages '

        // Returning Message About Error
        public new void ReturningMessageAboutError(string TextOfCircumstances, Exception Exc, bool Fatal)
        {
            //
            base.ReturningMessageAboutError(TextOfCircumstances, Exc, Fatal);
        }

        // Returning Message Of Information
        public new void ReturningMessageOfInformation(string TextOfMessage)
        {
            //
            base.ReturningMessageOfInformation(TextOfMessage);
        }

        #endregion

        #region ' Recording '

        // Recording In Log
        public void RecordingInLog(string TextOfRecord)
        {
            //
            base.RecordingInLogFile(TextOfRecord);
        }

        #endregion

    }
}