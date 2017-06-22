using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Service
{
    public class WorkingWithFiles : BaseType
    {

        #region ' Fields '

        //
        
        #endregion

        #region ' Designer '

        public WorkingWithFiles()
        {
            //
        }

        public WorkingWithFiles(string PathToLogFile)
            : base(PathToLogFile)
        {
            //
        }

        #endregion

        public string CreatingNameOfFile(string BaseName, DateTime Date)
        {
            string NameOfFile = String.Format(
                "{0}_{1}", BaseName,
                String.Format("{0}{1}{2}_{3}{4}{5}",
                Date.Year, Date.Month.ToString("00"), Date.Day.ToString("00"),
                Date.Hour.ToString("00"), Date.Minute.ToString("00"), Date.Second.ToString("00")));
            return NameOfFile;
        }

        public bool Saving(System.Data.DataSet DataOfSaving, System.IO.Stream StreamOfSaving)
        {
            bool ResultOfOperation = true;
            try
            {
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().
                  Serialize(StreamOfSaving, DataOfSaving);
            }
            catch (Exception E) { ResultOfOperation = false; ReturningMessageAboutError("Ошибка при сохранении данны в поток", E, false); }
            // Return
            return ResultOfOperation;
        }

        public System.Data.DataSet Loading(System.IO.Stream StreamOfLoading)
        {
            System.Data.DataSet LoadingDataSet = new System.Data.DataSet();
            try
            {
                LoadingDataSet = (System.Data.DataSet)
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(StreamOfLoading);
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при извлечении данных из потока", E, false); }
            //
            return LoadingDataSet;
        }

        public bool ArchivingFile(string PathToArchivingProgram, string PathToArchive, string PathToFile)
        {
            bool ResultOfOperation = true;
            try
            {
                System.Diagnostics.Process Processing = new System.Diagnostics.Process();
                Processing.StartInfo.CreateNoWindow = true;
                Processing.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Processing.StartInfo.Arguments =
                    String.Format(" a -ep -inul \"{0}\" \"{1}\"", PathToArchive, PathToFile);
                Processing.StartInfo.FileName = PathToArchivingProgram;
                Processing.Start();
                Processing.Close();
            }
            catch (Exception E)
            {
                ResultOfOperation = false;
                ReturningMessageAboutError(
                    String.Format("{0}: {1}", "Ошибка при архивации файла", PathToFile), E, false);
            }
            // Return
            return ResultOfOperation;
        }

        public bool ExtractionFromArchive(string PathToArchivingProgram, string PathToArchive, string PathToExtraction)
        {
            bool ResultOfOperation = true;
            try
            {
                this.RecordingInLogFile("extract arc begin");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments =
                    String.Format(" e -y \"{0}\" \"{1}\"", PathToArchive, PathToExtraction);
                process.StartInfo.FileName = PathToArchivingProgram;
                process.Start();
                process.Close();
                this.RecordingInLogFile("extract arc end");
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(String.Format("{0}: {1}", "Ошибка при извлечении из архива", PathToArchive, E.Message));
                ResultOfOperation = false;
                ReturningMessageAboutError(
                    String.Format("{0}: {1}", "Ошибка при извлечении из архива", PathToArchive), E, false);
            }
            // Return
            return ResultOfOperation;
        }

        // Is Access File
        public bool IsAccessFile(string NameOfFile)
        {
            try
            {
                FileStream FS = new FileStream(NameOfFile, FileMode.Open, FileAccess.ReadWrite);
                try
                {
                    FS.Close();
                    FS.Dispose();
                }
                catch { return false; }
            }
            catch { return false; }
            return true;
        }

        // Is Access File 02
        public bool IsAccessFile02(string NameOfFile)
        {
            //
            // !!!
            //
            try
            {
                FileStream FS = new FileStream(NameOfFile, FileMode.Open, FileAccess.ReadWrite);
                try
                {
                    FS.Close();
                    FS.Dispose();
                }
                catch { return false; }
            }
            catch { return false; }
            //
            // !!!
            //
            try
            {
                FileStream FS = new FileInfo(NameOfFile).Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Delete);
                try
                {
                    FS.Close();
                    FS.Dispose();
                }
                catch { return false; }
            }
            catch { return false; }
            //
            return true;
        }

        // Deleting File
        public bool DeletingFile(string PathToFile)
        {
            bool ResultOfOperation = true;
            try { System.IO.File.Delete(PathToFile); }
            catch (Exception E)
            {
                ResultOfOperation = false;
                ReturningMessageAboutError(
                    String.Format("{0}: {1}", "Ошибка при удалении файла", PathToFile), E, false);
            }
            // Return
            return ResultOfOperation;
        }

        #region ' Waiting '

        public void WaitingOfExists(string PathToFile, int Limit)
        {
            int AspirationToLimit = 0;
            while ((AspirationToLimit++ < Limit) && (!System.IO.File.Exists(PathToFile)))
                System.Threading.Thread.Sleep(50);
        }

        public void WaitingOfIsAccess(string PathToFile, int Limit)
        {
            int AspirationToLimit = 0;
            while ((AspirationToLimit++ < Limit) && (!IsAccessFile(PathToFile)))
                System.Threading.Thread.Sleep(50);
        }

        #endregion

    }
}