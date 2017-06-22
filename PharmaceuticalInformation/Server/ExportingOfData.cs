using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using PharmaceuticalInformation.BaseTypes;
using System.Linq;
using EntityFramework.Extensions;
using ServerOfSystem.PharmaceuticalInformation.DataTools;
using System.Data.Linq.Mapping;
using System.Reflection;
using ServerOfSystem.PharmaceuticalInformation.Interfaces;
using ServerOfSystem.PharmaceuticalInformation.Infrastructure;
using Ninject;

namespace PharmaceuticalInformation.Server
{
    public class ExportingOfData : BaseType
    {

        #region ' Fields '

        private IPharmacyInformation IPhrmInf;

        private LocalDataContext LDC;

        public class PriceListGlobal
        {
            public int ID_DR { get; set; }
            public int ID_PR { get; set; }
            public decimal Price { get; set; }
            public DateTime Actual { get; set; }
            public bool Preferential { get; set; }
        }

        private string StringOfConnection;

        #endregion
        
        #region ' Designer '

        public ExportingOfData(IPharmacyInformation _IPhrmInf, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Creating Of Connection
            //
            try
            {
                IPhrmInf = _IPhrmInf;

                LDC = new LocalDataContext(IPhrmInf.EFPhrmInf.Database.Connection);
            }
            catch (Exception E)
            {
                throw new Exception(
                    String.Format("Ошибка при создании подключения Экспорта данных: {0}", E.Message));
            }
        }

        #endregion


        #region ' Exporting Of Data '

        // Exporting Of Data
        public DataSet Exporting()
        {
            //
            // Initialize ExportedDataSet
            //
            DataSet ExportedDataSet = new DataSet("ExportedData");
            ExportedDataSet.RemotingFormat = SerializationFormat.Binary;
            //
            // Selections For Exporting Data
            //

            //
            // Date Of Exporting
            bool GettingOfData = false;
            int CountOfExported = 0;
            DateTime DateOfExported = new DateTime(1947, 07, 02);
            DateTime LastDateOfExported = new DateTime(1947, 07, 02);
            //
            try
            {
                DateOfExported = LDC.GetSystemDate();  // Convert.ToDateTime(GettingData.ExecuteScalar());
                LastDateOfExported = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 8).Select(s => s.Date_Service).Max();

                int NumberOfExported = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 8).Select(s => s.Value).Max(); //Convert.ToInt32(GettingData.ExecuteScalar());
                //
                ExportedDataSet.Tables.Add("DateOfExported");
                ExportedDataSet.Tables["DateOfExported"].Columns.Add("DateOfExported", typeof(DateTime));
                ExportedDataSet.Tables["DateOfExported"].Rows.Add(DateOfExported);
                ExportedDataSet.Tables["DateOfExported"].AcceptChanges();
                //
                ExportedDataSet.Tables.Add("NumberOfExported");
                ExportedDataSet.Tables["NumberOfExported"].Columns.Add("DateOfExported", typeof(int));
                ExportedDataSet.Tables["NumberOfExported"].Rows.Add(++NumberOfExported);
                ExportedDataSet.Tables["NumberOfExported"].AcceptChanges();

                GettingOfData = true;
            }
            catch (Exception E) 
            { ReturningMessageAboutError("Ошибка при получении даты или номера экспорта", E, true); }
            //
            // Reading Data
            //
            if (GettingOfData)
            {
                DataTable DT_Pharmacy = new DataTable("Pharmacy");
                DataTable DT_GroupsOfProducts = new DataTable("GroupsOfProducts");
                DataTable DT_Products = new DataTable("Products");
                DataTable DT_Announcements = new DataTable("Announcements");

                try
                {
                    IPhrmInf.EFPhrmInf.Pharmacies.Where(ph => ph.Date_upd >= LastDateOfExported && ph.Date_upd <= DateOfExported)                
                        .Select(ph => new
                            {
                                ID = ph.Id_Pharmacy,
                                ID_DI = ph.Id_District,
                                Name = ph.Name_full,
                                Address = ph.Addr,
                                Phone = ph.Phone,
                                Mail = ph.Mail,
                                Site = ph.Web,
                                Schedule = ph.Hours,
                                Transport = ph.Trans,
                                Deleting = ph.Is_deleted
                            }).Fill(ref DT_Pharmacy);
                }
                catch (Exception E)
                {
                    ReturningMessageAboutError(
                        String.Format("Ошибка при чтении данных из таблицы Pharmacy"), E, false);
                }

                try
                {
                    IPhrmInf.EFPhrmInf.Product_group.Where(pg => pg.Date_upd >= LastDateOfExported && pg.Date_upd <= DateOfExported)                
                        .Select(pg => new
                            {
                                ID = pg.Id_product_group,
                                Name = pg.Name_full,
                                DateOfUpdating = pg.Date_upd,
                                Deleting = pg.Is_deleted
                            }).Fill(ref DT_GroupsOfProducts);
                }
                catch (Exception E)
                {
                    ReturningMessageAboutError(
                        String.Format("Ошибка при чтении данных из таблицы Product_group"), E, false);
                }

                try
                {
                    IPhrmInf.EFPhrmInf.Products.Where(pr => pr.Date_upd >= LastDateOfExported && pr.Date_upd <= DateOfExported)                
                        .Select(pr => new
                            {
                                ID = pr.Id_Product,
                                ID_PG = pr.Id_product_group,
                                Name = pr.Name_full,
                                Composition = pr.Composition,
                                Description = pr.Description,
                                Updating = pr.Date_upd,
                                Deleting = pr.Is_deleted
                            }).Fill(ref DT_Products);
                }
                catch (Exception E)
                {
                    ReturningMessageAboutError(
                        String.Format("Ошибка при чтении данных из таблицы Product"), E, false);
                }

                try
                {
                    IPhrmInf.EFPhrmInf.Announcements.Where(a => a.DateOfUpdating >= LastDateOfExported && a.DateOfUpdating <= DateOfExported)                
                        .Select(a => new
                            {
                                ID_PH = a.ID_PH,
                                ID = a.ID,
                                Caption = a.Caption,
                                Text = a.Text,
                                Published = a.Published,
                                DateOfUpdating = a.DateOfUpdating
                            }).Fill(ref DT_Announcements);
                }
                catch (Exception E)
                {
                    ReturningMessageAboutError(
                        String.Format("Ошибка при чтении данных из таблицы Announcement"), E, false);
                }

                ExportedDataSet.Tables.Add(DT_Pharmacy);
                ExportedDataSet.Tables.Add(DT_GroupsOfProducts);
                ExportedDataSet.Tables.Add(DT_Products);
                ExportedDataSet.Tables.Add(DT_Announcements);

            }
            //
            // Return
            //
                return ExportedDataSet;
        }

        public bool UpdatingDateOfExporting(DateTime DateOfExported)
        {
            //
            bool ResultOfUpdating = true;
            //
            // Executing Updating
            //
            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 8)
                    .Update(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Date_Service = DateOfExported });
            }
            catch (Exception E)
            {
                ResultOfUpdating = false;
                ReturningMessageAboutError(
                    String.Format("Ошибка при обновлении даты экспортирования {0}", DateOfExported), E, false);
            }
            //
            // Return
            //
            return ResultOfUpdating;
        }

        public bool IncrementOfNumberOfExported()
        {
            //
            bool ResultOfExecuting = true;
            //
            // Executing Increment
            //
            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 8)
                    .Update(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Value = s.Value + 1 });
            }
            catch (Exception E)
            {
                ResultOfExecuting = false;
                ReturningMessageAboutError("Ошибка при обновлении номера экспорирования", E, false);
            }
            //
            // Return
            //
            return ResultOfExecuting;
        }

        #endregion


        #region ' Exporting Of PriceLists '

        // Checking Of Existence Exported Prices
        public bool CheckingOfExistenceExportedPrices()
        {
            //
            bool ExistenceExportedPrices = false;
            //
            // Getting Count Of Exported Prices
            //
            int CountOfExportedPrices = -1;
            //
            try
            {
                CountOfExportedPrices = IPhrmInf.EFPhrmInf.price_list
                    .Where(p => p.Date_upd > IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 6).Select(s => s.Date_Service).Max()).Count();
            }
            catch (Exception E)
            {
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при проверке наличия экспортных Прайс-Листов: {0}", E.Message));
            }
            //
            // Checking Existence
            //
            if (CountOfExportedPrices > 0)
                ExistenceExportedPrices = true;
            //
            // Return
            //
            return ExistenceExportedPrices;
        }

        // Getting Number Of Exported PriceLists
        public int GettingNumberOfExportedPriceLists()
        {
            //
            int NumberOfExported = -1;
            //
            // Getting Number
            //
            try
            {
                NumberOfExported = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 6).Select(s => s.Value).Max();
            }
            catch (Exception E)
            {
                //
                NumberOfExported = -1;
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при получении номера экспортирования Прайс-Листов: {0}", E.Message));
            }
            //
            // Return
            //
            return NumberOfExported;
        }

        // Getting Date Of Exported PriceLists
        public DateTime GettingDateOfExportedPriceLists()
        {
            //
            DateTime DateOfExported = new DateTime(1947, 07, 02);
            //
            // Getting Date
            //
            try
            {
                DateOfExported = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 6).Select(s => s.Date_Service).Max();
            }
            catch (Exception E)
            {
                //
                DateOfExported = new DateTime(1947, 07, 02);
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при получении даты экспортирования Прайс-Листов: {0}", E.Message));
            }
            //
            // Return
            //
            return DateOfExported;
        }

        // Getting Date Of Updating PriceLists
        public DateTime GettingDateOfUpdatingPriceLists()
        {
            //
            DateTime DateOfExported = new DateTime(1947, 07, 02);
            //
            // Getting Date
            //
            try
            {
                DateOfExported = IPhrmInf.EFPhrmInf.price_list.Select(p => p.Date_upd).Max();
            }
            catch (Exception E)
            {
                //
                DateOfExported = new DateTime(1947, 07, 02);
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при получении даты экспортирования Прайс-Листов: {0}", E.Message));
            }
            //
            // Return
            //
            return DateOfExported;
        }

        // Exporting Of PriceLists
        public void ExportingOfPriceLists(string PathToFileOfExportingPriceLists)
        {
            //
            RecordingInLogFile("Starting Exporting Of PriceLists");
            //
            // Filling Of PriceLists
            //
            RecordingInLogFile("Starting Filling Of Exported PriceLists");
            //
            DataTable ExportedPriceLists = new DataTable("PriceLists");
            bool ResultOfFilling = true;

            try
            {
                IEnumerable<PriceListGlobal> ExportedPriceListsIEn = IPhrmInf.EFPhrmInf.price_list.Where(pl => !pl.Is_deleted).Select(pl => new PriceListGlobal
                {
                    ID_DR = pl.Id_Pharmacy
                    ,
                    ID_PR = pl.Id_Product,
                    Price = (decimal)pl.Price,
                    Actual = pl.Actual,
                    Preferential = pl.Is_privilege
                });

                List<PriceListGlobal> lPLG = ExportedPriceListsIEn.ToList();
                lPLG.Sort((x, y) => 10 * Math.Sign(x.ID_DR - y.ID_DR) + Math.Sign(x.ID_PR - y.ID_PR));
                lPLG.Fill(ref ExportedPriceLists);
            }
            catch (Exception E)
            { 
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при заполнении таблицы экспортируемый Прайс-Листов: {0}", E.Message));
                //
                ResultOfFilling = false;
            }
            //
            RecordingInLogFile("Stoping Filling Of Exported PriceLists");
            //
            // Exporting To File Of PriceLists
            //
            if (ResultOfFilling)
            {
                //
                RecordingInLogFile("Starting Exporting To File Of PriceLists");
                //
                try
                {
                    //
                    // Creating Of Streams
                    //
                    FileStream FS = 
                        new FileStream(PathToFileOfExportingPriceLists, FileMode.Create, FileAccess.ReadWrite);
                    //
                    StreamWriter SW = new StreamWriter(FS);
                    //
                    // Writing Prices And Dates
                    //
                    try
                    {
                        //
                        int IDOfDrugstore = 0;
                        //
                        foreach (DataRow CurrentPrice in ExportedPriceLists.Rows)
                        {
                            //
                            // Writing Date
                            //
                            if (IDOfDrugstore != ((int)CurrentPrice["ID_DR"]))
                            {
                                //
                                IDOfDrugstore = ((int)CurrentPrice["ID_DR"]);
                                //
                                SW.WriteLine(
                                    String.Format("{0},{1}", 
                                    IDOfDrugstore, ((DateTime)CurrentPrice["Actual"]).ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                            //
                            // Writing Price
                            //
                            string NewStringOfPrices = 
                                String.Format(
                                "{0},{1},{2},{3}", 
                                CurrentPrice["ID_DR"], 
                                CurrentPrice["ID_PR"], 
                                CurrentPrice["Price"].ToString().Replace(",", "."), 
                                (((bool)CurrentPrice["Preferential"]) == true) ? "1" : "0");
                            //
                            SW.WriteLine(NewStringOfPrices);
                        }
                    }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при записи цена в файл Прайс-Листов: {0}", E.Message)); }
                    //
                    // Clearing Of ExportedPriceLists
                    //
                    ExportedPriceLists.Clear();
                    ExportedPriceLists.Dispose();
                    //
                    // Closing And Clearing Streams
                    //
                    try
                    {
                        //
                        SW.Close();
                        SW.Dispose();
                        //
                        //GC.SuppressFinalize(FS);
                        //
                        FS.Close();
                        FS.Dispose();
                        //
                        System.Threading.Thread.Sleep(1108);
                        //
                        GC.Collect();
                    }
                    catch (Exception E)
                    { RecordingInLogFile(String.Format("ERROR Ошибка при закрытии потоков записи файла экспортируемых Прайс-Листов: {0}", E.Message)); }
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при создании потоков записи файла экспортируемых Прайс-Листов: {0}", E.Message)); }
                //
                RecordingInLogFile("Stoping Exporting To File Of PriceLists");
            }
            //
            System.Threading.Thread.Sleep(108);
            //
            GC.Collect();
            //
            RecordingInLogFile("Stoping Exporting Of PriceLists");
        }

        // Increment Of Number Of Exported PriceLists
        public bool IncrementOfNumberOfExportedPriceLists(DateTime DateOfExported)
        {
            //
            bool ResultOfUpdating = true;
            //
            // Updating Number And Date Of Exported PriceLists
            //
            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 6)
                    .Update(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Value = s.Value + 1, Date_Service = DateOfExported });
            }
            catch (Exception E)
            {
                //
                ResultOfUpdating = false;
                //
                RecordingInLogFile(String.Format("ERROR Ошибка при обновлении номера экспорирования Прайс-Листов: {0}", E.Message));
            }
            //
            // Return
            //
            return ResultOfUpdating;
        }

        #endregion


        #region ' Exporting Of Modifications '

        // Clearing Of Modifications
        public void ClearingOfModifications(DataSet Modifications)
        {
            //
            if (Modifications != null)
                if (Modifications.Tables.Contains("IDsOfModifications"))
                {
                    //
                    DataTable IDsOfModifications = Modifications.Tables["IDsOfModifications"].Copy();
                    //
                    try
                    {
                        foreach(DataRow del_row in IDsOfModifications.Rows)
                            IPhrmInf.EFPhrmInf.C__ModifiedData.Where(md => Convert.ToInt32(del_row["IDOfModification"]) == md.ID).Delete();   /// JoinDataTable(ref IDsOfModifications, md.ID)).Delete();                 
                    }
                    catch (Exception E) 
                    { this.RecordingInLogFile(String.Format("Ошибка при удалении ID изменений: {0}", E.Message)); }
                }
        }
        #endregion


        #region ' Exporting Of Scripts Of Data '

        // Getting Current Date
        public DateTime GettingCurrentDate()
        {
            //
            // Creating Connection
            //
            DateTime CurrentDate = DateTime.MinValue;
            try
            {
                CurrentDate = LDC.GetSystemDate();
            }
            catch (Exception E) { this.RecordingInLogFile(String.Format("{0}: {1}", "Ошибка при создании подключения", E.Message)); }
            //
            // Getting Current Date
            //
            //
            // Return
            //
            return CurrentDate;
        }

        // Updating Date Of Exporting Of Scripts
        public bool UpdatingDateOfExportingOfScripts(DateTime DateOfExported)
        {
            //
            bool ResultOfOperation = true;

            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 10)
                    .UpdateAsync(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Date_Service = DateOfExported });
            }
            catch (Exception E)
            {
                ResultOfOperation = false;
                ReturningMessageAboutError(
                    String.Format("Ошибка при обновлении даты экспортирования {0}", DateOfExported), E, false);
            }
            //
            // Return
            //
            return ResultOfOperation;
        }

        #region ' Exporting Of Scripts Of Data '

        // Exporting Of Scripts Of Data
        public string ExportingOfScriptsOfData(DateTime DateOfExported)
        {
            //
            DataTable PriceListForExporting = new DataTable();            
            //
            // Filling Of Data
            //
            try
            {
                IPhrmInf.EFPhrmInf.price_list.Where(p => p.Date_upd >= IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 10)
                .Select(s => s.Date_Service).Max() && p.Date_upd <= DateOfExported)
                .Fill(ref PriceListForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(String.Format("{0}: {1}", "Ошибка при заполнении таблицы PriceList", E.Message));
                return "";
            }
            //
            // Scaning Of PriceList For Exporting
            //
            string TextOfCommands = "";
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            foreach (DataRow CurRow in PriceListForExporting.Rows)
            {
                //
                // Creating Text Of Command Of Deleting
                //
                TextOfCommands +=
                    String.Format(
                    "DELETE FROM {0}.Price_List WHERE (Id_Pharmacy = {1} AND Id_Product = {2});",
                    "Pharm66",
                    ((System.Int32)CurRow["Id_Pharmacy"]),
                    ((System.Int32)CurRow["Id_Product"]));
                TextOfCommands += Paragraph;
                //
                // Creating Text Of Command Of Inserting
                //
                if (((System.Boolean)CurRow["Is_deleted"]) != true)
                {
                    TextOfCommands +=
                        String.Format(
                        "INSERT INTO {0}.Price_List " +
                        "(Id_Pharmacy, Id_Product, Price, Date_upd, Is_deleted, Is_privilege) " +
                        Paragraph + "VALUES ({1}, {2}, {3}, '{4}', {5}, {6});",
                        "Pharm66",
                        Convert.ToInt32(CurRow["Id_Pharmacy"]), Convert.ToInt32(CurRow["Id_Product"]),
                        GettingStringOfDecimal(CurRow["Price"]),
                        GettingStringOfDate(((System.DateTime)CurRow["Date_upd"])),
                        GettingStringOfBoolean(((System.Boolean)CurRow["Is_deleted"])),
                        GettingStringOfBoolean(((System.Boolean)CurRow["Is_privilege"])));
                    TextOfCommands += Paragraph;
                    //
                }
                TextOfCommands += Paragraph;
            }
            //
            // Return
            //
            return TextOfCommands;
        }

        // Exporting Of Scripts Of Data 02
        public void ExportingOfScriptsOfData02(DateTime DateOfExported, string PathOfFileSQL)
        {
            //
            bool SuccessfulFilling = true;
            DataTable PriceListForExporting = new DataTable();
            //
            // Filling Of Data
            //
            try
            {
                IPhrmInf.EFPhrmInf.price_list.Where(p => p.Date_upd >= IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 10)
                .Select(s => s.Date_Service).Max() && p.Date_upd <= DateOfExported)
                .Fill(ref PriceListForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при заполнении таблицы PriceList: {0}", E.Message));
                SuccessfulFilling = false;
            }
            //
            // Creating File Of Exporting Scripts
            //
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Checking
            //
            bool SuccessfulChecking = false;
            //
            if (SuccessfulFilling)
                if (PriceListForExporting.Rows.Count > 0)
                    SuccessfulChecking = true;
            //
            if (SuccessfulChecking)
                try
                {
                    //
                    // Creating Streams Of File
                    //
                    System.IO.FileStream FS = new System.IO.FileStream(
                        PathOfFileSQL, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(FS);
                    //
                    // Writing Scripts In File
                    //
                    try
                    {
                        foreach (DataRow CurRow in PriceListForExporting.Rows)
                        {
                            //
                            // Creating Text Of Command Of Deleting
                            //
                            string TextOfCommandDeleting =
                                String.Format(
                                "DELETE FROM {0}.Price_List WHERE (Id_Pharmacy = {1} AND Id_Product = {2});",
                                "Pharm66",
                                ((System.Int32)CurRow["Id_Pharmacy"]),
                                ((System.Int32)CurRow["Id_Product"]));
                            //
                            SW.Write(TextOfCommandDeleting + Paragraph);
                            //
                            // Creating Text Of Command Of Inserting
                            //
                            if (((System.Boolean)CurRow["Is_deleted"]) != true)
                            {
                                string TextOfCommandInserting =
                                    String.Format(
                                    "INSERT INTO {0}.Price_List " +
                                    "(Id_Pharmacy, Id_Product, Price, Date_upd, Is_deleted, Is_privilege) " +
                                    Paragraph + "VALUES ({1}, {2}, {3}, '{4}', {5}, {6});",
                                    "Pharm66",
                                    Convert.ToInt32(CurRow["Id_Pharmacy"]), Convert.ToInt32(CurRow["Id_Product"]),
                                    GettingStringOfDecimal(CurRow["Price"]),
                                    GettingStringOfDate(((System.DateTime)CurRow["Date_upd"])),
                                    GettingStringOfBoolean(((System.Boolean)CurRow["Is_deleted"])),
                                    GettingStringOfBoolean(((System.Boolean)CurRow["Is_privilege"])));
                                //
                                SW.Write(TextOfCommandInserting + Paragraph);
                                //
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                                String.Format("ERROR Ошибка при записи в файл: {0}: {1}", PathOfFileSQL, E.Message));
                    }
                    //
                    // Closing Streams
                    //
                    try
                    {
                        SW.Close();
                        FS.Close();
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при закрытии файла: {0}: {1}", PathOfFileSQL, E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании файла: {0}: {1}", PathOfFileSQL, E.Message));
                    //
                    // Deleting File
                    //
                    try
                    {
                        if (File.Exists(PathOfFileSQL))
                            File.Delete(PathOfFileSQL);
                    }
                    catch (Exception E2)
                    {
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при удалении файла: {0}: {1}", PathOfFileSQL, E2.Message));
                    }
                }
            //
        }

        // Exporting Of Scripts Of Data 03
        public DataTable ExportingOfScriptsOfData03(DateTime DateOfExported, string PathToFileOfScripts)
        {
            //
            // Initializing Of Variables
            //
            bool SuccessfulFilling = true;
            DateTime MaxDateService = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == 10).Select(s => s.Date_Service).Max();
            //
            DataTable InformationOfExporting = new DataTable("InformationOfExporting");
            InformationOfExporting.Columns.Add("Key", typeof(string));
            InformationOfExporting.Columns.Add("Value", typeof(object));
            InformationOfExporting.PrimaryKey = new DataColumn[1] { InformationOfExporting.Columns["Key"] };
            //
            DataTable PricesForExporting = new DataTable("PricesForExporting");
            DataTable DatesForExporting = new DataTable("DatesForExporting");            
            //
            // Filling Of Data
            //
            try
            {
                IPhrmInf.EFPhrmInf.price_list.Where(p => p.Date_upd >= MaxDateService && p.Date_upd <= DateOfExported)
                    .Select(p => new
                        {
                            ID_PH = p.Id_Pharmacy,
                            ID_PR = p.Id_Product,
                            Price = p.Price,
                            Updating = p.Date_upd,
                            Preferential = p.Is_privilege,
                            Deleting = p.Is_deleted
                        }).Fill(ref PricesForExporting);
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при заполнении таблицы PricesForExporting: {0}", E.Message));
                SuccessfulFilling = false;
            }
            //
            try
            {
                IPhrmInf.EFPhrmInf.HistoryOfReceptions.Where(hr => hr.DateOfReception >= MaxDateService && hr.DateOfReception <= DateOfExported)
                    .Join
                    (
                        IPhrmInf.EFPhrmInf.ReportsOfImportingOfPriceLists
                        .Where(ripl => ripl.FullPriceList && ((ripl.CountNotConfirmed + ripl.CountOfAdditions + ripl.CountOfChanges + ripl.CountOfDeletings) > 0))
                        , hr => hr.ID, ripl => ripl.ID_HR 
                        , (hr, ripl) => new { ID =  ripl.ID_PH, Date = hr.DateOfReception}
                    ).Fill(ref DatesForExporting); 
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при заполнении таблицы DatesForExporting: {0}", E.Message));
                SuccessfulFilling = false;
            }
            //
            // Checking
            //
            if ((PricesForExporting.Rows.Count == 0) && (DatesForExporting.Rows.Count == 0))
                SuccessfulFilling = false;
            //
            // Filling Of Information Of Exporting
            //
            if (SuccessfulFilling)
            {
                //
                InformationOfExporting.Rows.Add("CountOfPrices", PricesForExporting.Rows.Count);
                InformationOfExporting.Rows.Add("CountOfChanges", PricesForExporting.Select("Deleting <> 1").Length);
                InformationOfExporting.Rows.Add("CountOfDeleting", PricesForExporting.Select("Deleting =  1").Length);
                //
                InformationOfExporting.Rows.Add("CountOfFullDates", DatesForExporting.Rows.Count);
                //
                //ConnectionToBase.Open();
                InformationOfExporting.Rows.Add("DateOfStart", MaxDateService);
                InformationOfExporting.Rows.Add("DateOfEnd", DateOfExported);

                //
                // Creating File Of Exporting Scripts
                //
                try
                {
                    //
                    string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
                    //
                    // Creating Streams Of File
                    //
                    System.IO.FileStream FS = new System.IO.FileStream(
                        PathToFileOfScripts, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    System.IO.StreamWriter SW = new System.IO.StreamWriter(FS, Encoding.Default);
                    //
                    // Writing Scripts In File
                    //
                    try
                    {
                        //
                        foreach (DataRow CurrentPrice in PricesForExporting.Rows)
                        {
                            //
                            // Creating Text Of Command Of Deleting
                            //
                            string TextOfCommandOfDeleting =
                                String.Format(
                                "DELETE FROM Pharm66.Price_List WHERE (Id_Pharmacy = {1} AND Id_Product = {2});{0}",
                                Paragraph, ((int)CurrentPrice["ID_PH"]), ((int)CurrentPrice["ID_PR"]));
                            //
                            SW.Write(TextOfCommandOfDeleting);
                            //
                            // Creating Text Of Command Of Inserting
                            //
                            if (((bool)CurrentPrice["Deleting"]) != true)
                            {
                                string TextOfCommandOfInserting =
                                    String.Format(
                                    "INSERT INTO Pharm66.Price_List " +
                                    "(Id_Pharmacy, Id_Product, Price, Date_upd, Is_deleted, Is_privilege) {0}" +
                                    "VALUES ({1}, {2}, {3}, '{4}', 0, {6});{0}",
                                    Paragraph, ((int)CurrentPrice["ID_PH"]), ((int)CurrentPrice["ID_PR"]),
                                    GettingStringOfDecimal(CurrentPrice["Price"]),
                                    GettingStringOfDate(((System.DateTime)CurrentPrice["Updating"])),
                                    GettingStringOfBoolean(((System.Boolean)CurrentPrice["Deleting"])),
                                    GettingStringOfBoolean(((System.Boolean)CurrentPrice["Preferential"])));
                                //
                                SW.Write(TextOfCommandOfInserting);
                                //
                            }
                        }
                        //
                        foreach (DataRow CurrentDate in DatesForExporting.Rows)
                        {
                            //
                            // Creating Text Of Command Of Updating
                            //
                            string TextOfCommandOfUpdating =
                                String.Format(
                                "UPDATE Pharm66.Price_List SET Date_upd = '{2}' WHERE Id_Pharmacy = {1};{0}",
                                Paragraph,
                                ((int)CurrentDate["ID"]), GettingStringOfDate(((DateTime)CurrentDate["Date"])));
                            //
                            SW.Write(TextOfCommandOfUpdating);
                            //
                        }
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                                String.Format("ERROR Ошибка при записи в файл: {0}: {1}", PathToFileOfScripts, E.Message));
                    }
                    //
                    // Closing Streams
                    //
                    try
                    {
                        SW.Close();
                        FS.Close();
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при закрытии файла: {0}: {1}", PathToFileOfScripts, E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании файла: {0}: {1}", PathToFileOfScripts, E.Message));
                    //
                    // Deleting File
                    //
                    try
                    {
                        if (File.Exists(PathToFileOfScripts))
                            File.Delete(PathToFileOfScripts);
                    }
                    catch (Exception E2)
                    {
                        //
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при удалении файла: {0}: {1}", PathToFileOfScripts, E2.Message));
                    }
                }
            }
            //
            // Return
            //
            return InformationOfExporting;
        }

        #endregion

        #endregion


        #region ' Exporting Updatings For Drugstore '

        // Exporting Updatings For Drugstore
        public DataSet ExportingUpdatingsForDrugstore()
        {
            //
            // Return
            //
            return new DataSet("UpdatingsForDrugstore");
        }

        #endregion


        #region ' Exporting Updatings PriceLists For Drugstore '

        //

        #endregion


        #region ' Exporting Updatings For Sites '


        #region ' Creating Scripts Of Updating '

        // Exporting Scripts Of Updating
        public void ExportingScriptsOfUpdating(
            string PathToFileOfScripts,
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
        }

        // Creating Scripts Of Updating
        public void CreatingScriptsOfUpdating(DataSet DataForCreatingScripts, string PathToFileOfScripts)
        {
            //
        }

        // Creating Scripts Of Updating
        public string CreatingScriptsOfUpdating(DataSet DataForCreatingScripts)
        {
            //
            // Return;
            //
            return "";
        }

        #endregion


        #region ' Creating Of Executing Of Stored Procedures '

        // Exporting Stored Procedures Of Updating
        public void ExportingStoredProceduresOfUpdating(
            string PathToFileOfScripts, 
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Exporting Of Modified Data
            //
            DataTable[] DataForExporting = new DataTable[4];
            //
            DataForExporting[0] = 
                GettingModificationsOfDrugstores(DateOfStartingModification, DateOfEndingModification);
            DataForExporting[1] = 
                GettingModificationsOfProducts(DateOfStartingModification, DateOfEndingModification);
            DataForExporting[2] = 
                GettingModificationsOfPriceLists(DateOfStartingModification, DateOfEndingModification);
            DataForExporting[3] = 
                GettingModificationsOfDatesOfPriceLists(DateOfStartingModification, DateOfEndingModification);
            //
            // Checking Of Result Of Exporting
            //
            bool CorrectResultOfExporting = true;
            //
            if (DataForExporting.Length == 0)
                CorrectResultOfExporting = false;
            //
            // !!!
            //
            if (CorrectResultOfExporting)
            { }
            //
            // Reporting Of Result Of Exporting 
            //
            if (CorrectResultOfExporting)
            {
                //
                // Recording In Log File
                //
                RecordingInLogFile(
                        String.Format(
                        "Date Of Beginning: {0} Date Of Ending: {1}", 
                        DateOfStartingModification, DateOfEndingModification));
                //
                // Counts Of Drugstores, Products, Prices, Dates
                //
                foreach (DataTable CurrentData in DataForExporting)
                    switch (CurrentData.TableName)
                    {
                        case "Drugstores":
                            {
                                //
                                // Getting Count Of Drugstores
                                //
                                int
                                    CountOfDrugstores = CurrentData.Rows.Count, 
                                    CountOfChanges = CurrentData.Select("Deleting <> 1").Length, 
                                    CountOfDeleting = CurrentData.Select("Deleting =  1").Length;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(
                                        String.Format(
                                        "Count Of Drugstores: {0} Count Of Changes: {1} Count Of Deleting: {2}", 
                                        CountOfDrugstores, CountOfChanges, CountOfDeleting));
                            }
                            break;
                        case "Products":
                            {
                                //
                                // Getting Count Of Products
                                //
                                int
                                    CountOfProducts = CurrentData.Rows.Count, 
                                    CountOfChanges = CurrentData.Select("Deleting <> 1").Length, 
                                    CountOfDeleting = CurrentData.Select("Deleting =  1").Length;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(
                                        String.Format(
                                        "Count Of Products: {0} Count Of Changes: {1} Count Of Deleting: {2}", 
                                        CountOfProducts, CountOfChanges, CountOfDeleting));
                            }
                            break;
                        case "PriceLists":
                            {
                                //
                                // Getting Count Of Prices
                                //
                                int
                                    CountOfPrices = CurrentData.Rows.Count, 
                                    CountOfChanges = CurrentData.Select("Deleting <> 1").Length, 
                                    CountOfDeleting = CurrentData.Select("Deleting =  1").Length;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(
                                        String.Format(
                                        "Count Of Prices: {0} Count Of Changes: {1} Count Of Deleting: {2} ", 
                                        CountOfPrices, CountOfChanges, CountOfDeleting));
                            }
                            break;
                        case "DatesOfPriceLists":
                            {
                                //
                                // Getting Count Of Dates
                                //
                                int CountOfDates = CurrentData.Rows.Count;
                                //
                                // Recording In Log File
                                //
                                RecordingInLogFile(String.Format("Count Of Full Dates: {0} ", CountOfDates));
                            }
                            break;
                    }
            }
            //
            // Creating Of Executing Of Stored Procedures
            //
            string TextOfStoredProcedures = "";
            //
            if (CorrectResultOfExporting)
                TextOfStoredProcedures = CreatingOfExecutingOfStoredProcedures(DataForExporting);
            //
            // Checking Sanction Of Writing
            //
            bool SanctionOfWriting = false;
            //
            if (CorrectResultOfExporting && (TextOfStoredProcedures.Length > 0))
                SanctionOfWriting = true;
            //
            // Writing Text In File
            //
            if (SanctionOfWriting)
                try
                {
                    //
                    // Creating Streams Of Writing
                    //
                    FileStream FS = new FileStream(PathToFileOfScripts, FileMode.Create, FileAccess.Write);
                    StreamWriter SW = new StreamWriter(FS, Encoding.Default);
                    //
                    // Writing Text Of Stored Procedures
                    //
                    SW.WriteLine(TextOfStoredProcedures);
                    //
                    // Closing Streams
                    //
                    try
                    {
                        SW.Close();
                        FS.Close();
                    }
                    catch (Exception E)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при закрытии потоков файла ({0}): {1}", 
                            PathToFileOfScripts, E.Message));
                    }
                }
                catch (Exception E)
                {
                    //
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при создании потоков файла ({0}): {1}", 
                        PathToFileOfScripts, E.Message));
                    //
                    // Deleting File
                    //
                    try
                    {
                        if (File.Exists(PathToFileOfScripts))
                            File.Delete(PathToFileOfScripts);
                    }
                    catch (Exception E2)
                    {
                        this.RecordingInLogFile(
                            String.Format("ERROR Ошибка при удалении файла ({0}): {1}", 
                            PathToFileOfScripts, E2.Message));
                    }
                }
        }

        // Creating Of Executing Of Stored Procedures (Not Check DataForCreatingScripts)
        public void CreatingOfExecutingOfStoredProcedures(DataTable[] DataForCreatingScripts, string PathToFileOfScripts)
        {
            //
            // Creating Streams Of Writing
            //
            FileStream FS = null;
            StreamWriter SW = null;
            bool StreamsAreCreated = false;
            //
            try
            {
                //
                FS = new FileStream(PathToFileOfScripts, FileMode.Create, FileAccess.Write);
                SW = new StreamWriter(FS, Encoding.Default);
                //
                StreamsAreCreated = true;
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при создании потоков файла ({0}): {1}", 
                    PathToFileOfScripts, E.Message));
                //
                StreamsAreCreated = false;
                //
                // Deleting File
                //
                try
                {
                    if (File.Exists(PathToFileOfScripts))
                        File.Delete(PathToFileOfScripts);
                }
                catch (Exception E2)
                {
                    this.RecordingInLogFile(
                        String.Format("ERROR Ошибка при удалении файла ({0}): {1}", 
                        PathToFileOfScripts, E2.Message));
                }
            }
            //
            // Creating Executing Of Stored Procedures And Writing In File
            //
            if (StreamsAreCreated)
            {
                foreach (DataTable CurrentData in DataForCreatingScripts)
                    switch (CurrentData.TableName)
                    {
                        case "Drugstores":
                            {
                                foreach (DataRow CurrentDrugstore in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingDrugstoresOfSite({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9});", 
                                            CurrentDrugstore["ID"], 
                                            CurrentDrugstore["ID_DI"], 
                                            CurrentDrugstore["Name"], 
                                            CurrentDrugstore["Address"], 
                                            CurrentDrugstore["Phone"], 
                                            CurrentDrugstore["Mail"], 
                                            CurrentDrugstore["Site"], 
                                            CurrentDrugstore["Schedule"], 
                                            CurrentDrugstore["Transport"], 
                                            GettingStringOfBoolean((bool)CurrentDrugstore["Deleting"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingDrugstoresOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                        case "Products":
                            {
                                foreach (DataRow CurrentProduct in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingProductsOfSite({0}, {1}, '{2}', '{3}', '{4}', {5});", 
                                            CurrentProduct["ID"], 
                                            CurrentProduct["ID_PG"], 
                                            GettingStringOfString((string)CurrentProduct["Name"]), 
                                            GettingStringOfString((string)CurrentProduct["Composition"]), 
                                            GettingStringOfDate((DateTime)CurrentProduct["Updating"]), 
                                            GettingStringOfBoolean((bool)CurrentProduct["Deleting"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingProductsOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                        case "PriceLists":
                            {
                                foreach (DataRow CurrentPrice in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingPriceListsOfSite({0}, {1}, '{2}', '{3}', {4}, {5});", 
                                            CurrentPrice["ID_PH"], 
                                            CurrentPrice["ID_PR"], 
                                            GettingStringOfDecimal((decimal)CurrentPrice["Price"]), 
                                            GettingStringOfDate((DateTime)CurrentPrice["Updating"]), 
                                            GettingStringOfBoolean((bool)CurrentPrice["Preferential"]), 
                                            GettingStringOfBoolean((bool)CurrentPrice["Deleting"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingPriceListsOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                        case "DatesOfPriceLists":
                            {
                                foreach (DataRow CurrentDate in CurrentData.Rows)
                                    try
                                    {
                                        SW.WriteLine(
                                            String.Format(
                                            "CALL Pharm66.UpdatingDatesOfPriceListOfSite({0}, '{1}');", 
                                            CurrentDate["ID"], 
                                            GettingStringOfDate((DateTime)CurrentDate["Date"]))
                                            );
                                    }
                                    catch (Exception E)
                                    {
                                        RecordingInLogFile(
                                            String.Format("ERROR Ошибка при формировании SP UpdatingDatesOfPriceListOfSite: {0}", E.Message));
                                    }
                            }
                            break;
                    }
            }
            //
            // Closing Streams
            //
            try
            {
                //
                if (SW != null)
                    SW.Close();
                //
                if (FS != null)
                    FS.Close();
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при закрытии потоков файла ({0}): {1}", 
                    PathToFileOfScripts, E.Message));
            }
        }

        // Creating Of Executing Of Stored Procedures (Not Check DataForCreatingScripts)
        public string CreatingOfExecutingOfStoredProcedures(DataTable[] DataForCreatingScripts)
        {
            //
            StringBuilder StringOfExecutingOfStoredProcedures = new StringBuilder();
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Creating Executing Of Stored Procedures
            //
            foreach (DataTable CurrentData in DataForCreatingScripts)
                switch (CurrentData.TableName)
                {
                    case "Drugstores":
                        {
                            foreach (DataRow CurrentDrugstore in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingDrugstoresOfSite({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9});{10}", 
                                        CurrentDrugstore["ID"], 
                                        CurrentDrugstore["ID_DI"], 
                                        CurrentDrugstore["Name"], 
                                        CurrentDrugstore["Address"], 
                                        CurrentDrugstore["Phone"], 
                                        CurrentDrugstore["Mail"], 
                                        CurrentDrugstore["Site"], 
                                        CurrentDrugstore["Schedule"], 
                                        CurrentDrugstore["Transport"], 
                                        GettingStringOfBoolean((bool)CurrentDrugstore["Deleting"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingDrugstoresOfSite: {0}", E.Message));
                                }
                        }
                        break;
                    case "Products":
                        {
                            foreach (DataRow CurrentProduct in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingProductsOfSite({0}, {1}, '{2}', '{3}', '{4}', {5});{6}", 
                                        CurrentProduct["ID"], 
                                        CurrentProduct["ID_PG"], 
                                        GettingStringOfString((string)CurrentProduct["Name"]), 
                                        GettingStringOfString((string)CurrentProduct["Composition"]), 
                                        GettingStringOfDate((DateTime)CurrentProduct["Updating"]), 
                                        GettingStringOfBoolean((bool)CurrentProduct["Deleting"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingProductsOfSite: {0}", E.Message));
                                }
                        }
                        break;
                    case "PriceLists":
                        {
                            foreach (DataRow CurrentPrice in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingPriceListsOfSite({0}, {1}, '{2}', '{3}', {4}, {5});{6}", 
                                        CurrentPrice["ID_PH"], 
                                        CurrentPrice["ID_PR"], 
                                        GettingStringOfDecimal((decimal)CurrentPrice["Price"]), 
                                        GettingStringOfDate((DateTime)CurrentPrice["Updating"]), 
                                        GettingStringOfBoolean((bool)CurrentPrice["Preferential"]), 
                                        GettingStringOfBoolean((bool)CurrentPrice["Deleting"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingPriceListsOfSite: {0}", E.Message));
                                }
                        }
                        break;
                    case "DatesOfPriceLists":
                        {
                            foreach (DataRow CurrentDate in CurrentData.Rows)
                                try
                                {
                                    StringOfExecutingOfStoredProcedures.Append(
                                        String.Format(
                                        "CALL Pharm66.UpdatingDatesOfPriceListOfSite({0}, '{1}');{2}", 
                                        CurrentDate["ID"], 
                                        GettingStringOfDate((DateTime)CurrentDate["Date"]), 
                                        Paragraph)
                                        );
                                }
                                catch (Exception E)
                                {
                                    RecordingInLogFile(
                                        String.Format("ERROR Ошибка при формировании SP UpdatingDatesOfPriceListOfSite: {0}", E.Message));
                                }
                        }
                        break;
                }
            //
            // Return
            //
            return StringOfExecutingOfStoredProcedures.ToString();
        }

        #endregion


        #region ' Getting Strings Of Types Of Data '

        // Getting String Of Decimal
        private string GettingStringOfDecimal(object Value)
        {
            //
            string StringOfDecimal = "0.000";
            //
            // Checking Of Value
            //
            bool ValueIsCorrect = true;
            //
            if ((Value == null) || (Value is DBNull) || !(Value is Decimal))
                ValueIsCorrect = false;
            //
            // Formating Value
            //
            if (ValueIsCorrect)
            {
                try
                {
                    //
                    Decimal ValueForFormating = Convert.ToDecimal(Value);
                    //
                    StringOfDecimal = String.Format("{0:f3}", ValueForFormating);
                    StringOfDecimal = StringOfDecimal.Replace(",", ".");
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Ошибка при конвертации Decimal: {0}", E.Message)); }
            }
            //
            // Return
            //
            return StringOfDecimal;
        }

        // Getting String Of Date
        private string GettingStringOfDate(DateTime Value)
        {
            //
            string StringOfDate = "";
            //
            // Formating Value
            //
            StringOfDate = 
                String.Format("{0}-{1}-{2} {3}:{4}:{5}", 
                Value.Year, Value.Month, Value.Day, Value.Hour, Value.Minute, Value.Second);
            //
            // Return
            //
            return StringOfDate;
        }

        // Getting String Of Boolean
        private string GettingStringOfBoolean(bool Value)
        {
            //
            string StringOfBoolean = "0";
            //
            // Formating Value
            //
            StringOfBoolean = String.Format("{0}", (Value) ? 1 : 0);
            //
            // Return
            //
            return StringOfBoolean;
        }

        // Getting String Of String
        private string GettingStringOfString(string Value)
        {
            //
            string StringOfString = "";
            string Paragraph = System.Text.Encoding.Default.GetString(new byte[2] { 13, 10 });
            //
            // Formating Value
            //
            StringOfString = Value.Replace(Paragraph, "");
            //
            // Return
            //
            return StringOfString;
        }

        #endregion

        #endregion


        #region ' Getting Modifications '

        // Getting Modifications Of Drugstores
        public DataTable GettingModificationsOfDrugstores(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("Drugstores");

            IPhrmInf.EFPhrmInf.Pharmacies.Where(ph => ph.Date_upd >= DateOfStartingModification && ph.Date_upd <= DateOfEndingModification)
                .Select(ph => new
                {
                    ID = ph.Id_Pharmacy,
                    ID_DI = ph.Id_District,
                    Name = ph.Name_full,
                    Address = ph.Addr,
                    Phone = ph.Phone,
                    Mail = ph.Mail,
                    Site = ph.Web,
                    Schedule = ph.Hours,
                    Transport = ph.Trans,
                    Deleting = ph.Is_deleted
                }).Fill(ref ModifiedData);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of Groups Of Products
        public DataTable GettingModificationsOfGroupsOfProducts(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("GroupsOfProducts");

            IPhrmInf.EFPhrmInf.Product_group.Where(pg => pg.Date_upd >= DateOfStartingModification && pg.Date_upd <= DateOfEndingModification)
                .Select(pg => new
                {
                    ID = pg.Id_product_group,
                    Name = pg.Name_full,
                    DateOfUpdating = pg.Date_upd,
                    Deleting = pg.Is_deleted
                }).Fill(ref ModifiedData);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of Products
        public DataTable GettingModificationsOfProducts(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("Products");

            IPhrmInf.EFPhrmInf.Products.Where(pr => pr.Date_upd >= DateOfStartingModification && pr.Date_upd <= DateOfEndingModification)
                .Select(pr => new
                {
                    ID = pr.Id_Product,
                    ID_PG = pr.Id_product_group,
                    Name = pr.Name_full,
                    Composition = pr.Composition,
                    Description = pr.Description,
                    Updating = pr.Date_upd,
                    Deleting = pr.Is_deleted
                }).Fill(ref ModifiedData);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of PriceLists
        public DataTable GettingModificationsOfPriceLists(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("PriceLists");

            IPhrmInf.EFPhrmInf.price_list.Where(pl => pl.Date_upd >= DateOfStartingModification && pl.Date_upd <= DateOfEndingModification)
                .Select(pl => new
                {
                    ID_PH = pl.Id_Pharmacy,
                    ID_PR = pl.Id_Product,
                    Price = pl.Price,
                    Updating = pl.Date_upd,
                    Preferential = pl.Is_privilege,
                    Deleting = pl.Is_deleted
                }).Fill(ref ModifiedData);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of DatesOfPriceLists
        public DataTable GettingModificationsOfDatesOfPriceLists(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("DatesOfPriceLists");

            IPhrmInf.EFPhrmInf.HistoryOfReceptions.Where(hr => hr.DateOfReception >= DateOfStartingModification && hr.DateOfReception <= DateOfEndingModification)
                .Join(IPhrmInf.EFPhrmInf.ReportsOfImportingOfPriceLists
                .Where(ripl => ripl.FullPriceList && ((ripl.CountNotConfirmed + ripl.CountOfAdditions + ripl.CountOfChanges + ripl.CountOfDeletings) > 0))
                ,hr => hr.ID
                ,ripl => ripl.ID_HR
                ,(hr, ripl) => new { ID = ripl.ID_PH, Date = hr.DateOfReception }).Fill(ref ModifiedData);
            //
            // Return
            //
            return ModifiedData;
        }

        // Getting Modifications Of Announcements
        public DataTable GettingModificationsOfAnnouncements(
            DateTime DateOfStartingModification, DateTime DateOfEndingModification)
        {
            //
            // Getting Modifications Of Data
            //
            DataTable ModifiedData = new DataTable("CountOfExported"); // Announcements

            IPhrmInf.EFPhrmInf.Announcements.Where(a => a.DateOfUpdating >= DateOfStartingModification && a.DateOfUpdating <= DateOfEndingModification)
                .Select(a => new
                {
                    ID_PH = a.ID_PH,
                    ID = a.ID,
                    Caption = a.Caption,
                    Text = a.Text,
                    Published = a.Published,
                    DateOfUpdating = a.DateOfUpdating
                }).Fill(ref ModifiedData);
            //
            // Return
            //
            return ModifiedData;
        }

        #endregion


        #region ' Management Of Dates And Numbers '

        // Getting Current Date Of Storage
        public DateTime GettingCurrentDateOfStorage()
        {
            //
            DateTime CurrentDateOfStorage = new DateTime(1947, 07, 02);
            //
            // Getting Current Date
            //
            try
            {

                CurrentDateOfStorage = LDC.GetSystemDate();
            }
            catch (Exception E)
            {
                CurrentDateOfStorage = new DateTime(1947, 07, 02);
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при получении текущей даты хранилища данных: {0}", E.Message));
            }
            //
            // Return
            //
            return CurrentDateOfStorage;
        }

        // Getting Date
        public DateTime GettingDate(int IDOfDate)
        {
            //
            DateTime ReturnedDate = new DateTime(1947, 07, 02);
            //
            // Getting Date
            //
            try
            {
                ReturnedDate = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == IDOfDate)
                    .Select(s => s.Date_Service).Max();
            }
            catch (Exception E)
            {
                ReturnedDate = new DateTime(1947, 07, 02);
                //
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при получении даты: {0}", E.Message));
            }
            //
            // Return
            //
            return ReturnedDate;
        }

        // Getting Number
        public int GettingNumber(int IDOfNumber)
        {
            //
            int ReturnedNumber = 0;
            //
            // Getting Date
            //
            try
            {
                ReturnedNumber = IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == IDOfNumber)
                    .Select(s => s.Value).Max();
            }
            catch (Exception E)
            {
                ReturnedNumber = 0;
                //
                RecordingInLogFile(
                    String.Format("ERROR Ошибка при получении номера: {0}", E.Message));
            }
            //
            // Return
            //
            return ReturnedNumber;
        }

        // Updating Number
        public void UpdatingNumber(int IDOfNumber, int NewNumber)
        {
            //
            // Updating Date
            //
            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == IDOfNumber)
                    .Update(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Value = NewNumber });
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении номера: {0}", E.Message));
            }
        }

        // Updating Date 
        public void UpdatingDate(int IDOfDate, DateTime NewDate)
        {
            //
            // Updating Date
            //
            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == IDOfDate)
                    .Update(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Date_Service = NewDate });
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при обновлении даты: {0}", E.Message));
            }
        }

        // Increment Of Number
        public void IncrementOfNumber(int IDOfNumber)
        {
            //
            // Increment Number
            //
            try
            {
                IPhrmInf.EFPhrmInf.Services.Where(s => s.Id_Service == IDOfNumber)
                    .Update(s => new ServerOfSystem.PharmaceuticalInformation.Model.Service { Value = s.Value + 1 });
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("ERROR Ошибка при приращении номера: {0}", E.Message));
            }
        }

        // Increment Of Number
        public void IncrementOfNumber(int IDOfNumber, DateTime NewDate)
        {
            //
        }

        #endregion

    }
}