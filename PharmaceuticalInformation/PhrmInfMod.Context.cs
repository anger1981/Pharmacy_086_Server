﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test_pharm_server.PharmaceuticalInformation
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PhrmInfTESTEntities : DbContext
    {
        public PhrmInfTESTEntities()
            : base("name=PhrmInfTESTEntities")
        {
        }

        public PhrmInfTESTEntities(string ConnectionString)
            : base()
        {
            this.Database.Connection.ConnectionString = ConnectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AbsentProduct> AbsentProducts { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<comparison> comparisons { get; set; }
        public virtual DbSet<DatesOfTransfer> DatesOfTransfers { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<HistoryOfChangesOfPrice> HistoryOfChangesOfPrices { get; set; }
        public virtual DbSet<HistoryOfReception> HistoryOfReceptions { get; set; }
        public virtual DbSet<IDsOfExistingProduct> IDsOfExistingProducts { get; set; }
        public virtual DbSet<InformationOfSetting> InformationOfSettings { get; set; }
        public virtual DbSet<ListOfSetting> ListOfSettings { get; set; }
        public virtual DbSet<ModificationsOfPrice> ModificationsOfPrices { get; set; }
        public virtual DbSet<Pharmacy> Pharmacies { get; set; }
        public virtual DbSet<price_list> price_list { get; set; }
        public virtual DbSet<PrivateImporting> PrivateImportings { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Product_group> Product_group { get; set; }
        public virtual DbSet<RecodingIDsOfDrugstoresOfImporting> RecodingIDsOfDrugstoresOfImportings { get; set; }
        public virtual DbSet<RegistrationOfDrugstore> RegistrationOfDrugstores { get; set; }
        public virtual DbSet<ReportsOfImportingOfPriceList> ReportsOfImportingOfPriceLists { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Terminal> Terminals { get; set; }
        public virtual DbSet<C__HistoryOfSearchRequestsIMP> C__HistoryOfSearchRequestsIMP { get; set; }
        public virtual DbSet<C__ModifiedData> C__ModifiedData { get; set; }
        public virtual DbSet<C__Renaming01> C__Renaming01 { get; set; }
        public virtual DbSet<HistoryOfSearchRequest> HistoryOfSearchRequests { get; set; }
        public virtual DbSet<InhalersComparison> InhalersComparisons { get; set; }
        public virtual DbSet<InhalersPrice> InhalersPrices { get; set; }
        public virtual DbSet<InhalersProduct> InhalersProducts { get; set; }
        public virtual DbSet<LoadedProduct> LoadedProducts { get; set; }
        public virtual DbSet<LogsOfDrugstore> LogsOfDrugstores { get; set; }
        public virtual DbSet<NewInhalersComparison> NewInhalersComparisons { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<PREP_MEDENERGOFARM_COMPARISONS> PREP_MEDENERGOFARM_COMPARISONS { get; set; }
        public virtual DbSet<PREP_ZDOROVO_COMPARISONS> PREP_ZDOROVO_COMPARISONS { get; set; }
        public virtual DbSet<PriceIDsTMP> PriceIDsTMPs { get; set; }
        public virtual DbSet<PriceListTMP> PriceListTMPs { get; set; }
        public virtual DbSet<ProductsFromDL> ProductsFromDLs { get; set; }
    
        public virtual int InsertingDataInAnnouncements(string source, Nullable<int> iD_PH, Nullable<bool> published, string caption, Nullable<bool> autoPublication, string text)
        {
            var sourceParameter = source != null ?
                new ObjectParameter("Source", source) :
                new ObjectParameter("Source", typeof(string));
    
            var iD_PHParameter = iD_PH.HasValue ?
                new ObjectParameter("ID_PH", iD_PH) :
                new ObjectParameter("ID_PH", typeof(int));
    
            var publishedParameter = published.HasValue ?
                new ObjectParameter("Published", published) :
                new ObjectParameter("Published", typeof(bool));
    
            var captionParameter = caption != null ?
                new ObjectParameter("Caption", caption) :
                new ObjectParameter("Caption", typeof(string));
    
            var autoPublicationParameter = autoPublication.HasValue ?
                new ObjectParameter("AutoPublication", autoPublication) :
                new ObjectParameter("AutoPublication", typeof(bool));
    
            var textParameter = text != null ?
                new ObjectParameter("Text", text) :
                new ObjectParameter("Text", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertingDataInAnnouncements", sourceParameter, iD_PHParameter, publishedParameter, captionParameter, autoPublicationParameter, textParameter);
        }
    
        public virtual int InsertingInPriceList(Nullable<int> iDOfDrugstore, Nullable<int> iDOfReception, Nullable<int> p1, Nullable<decimal> p2, Nullable<bool> p3, Nullable<bool> p4)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var iDOfReceptionParameter = iDOfReception.HasValue ?
                new ObjectParameter("IDOfReception", iDOfReception) :
                new ObjectParameter("IDOfReception", typeof(int));
    
            var p1Parameter = p1.HasValue ?
                new ObjectParameter("P1", p1) :
                new ObjectParameter("P1", typeof(int));
    
            var p2Parameter = p2.HasValue ?
                new ObjectParameter("P2", p2) :
                new ObjectParameter("P2", typeof(decimal));
    
            var p3Parameter = p3.HasValue ?
                new ObjectParameter("P3", p3) :
                new ObjectParameter("P3", typeof(bool));
    
            var p4Parameter = p4.HasValue ?
                new ObjectParameter("P4", p4) :
                new ObjectParameter("P4", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertingInPriceList", iDOfDrugstoreParameter, iDOfReceptionParameter, p1Parameter, p2Parameter, p3Parameter, p4Parameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_Clean_Base()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_Clean_Base");
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_CreatingCSVProducts(Nullable<int> iDOfStart, Nullable<int> iDOfEnd, string numberOfFile)
        {
            var iDOfStartParameter = iDOfStart.HasValue ?
                new ObjectParameter("IDOfStart", iDOfStart) :
                new ObjectParameter("IDOfStart", typeof(int));
    
            var iDOfEndParameter = iDOfEnd.HasValue ?
                new ObjectParameter("IDOfEnd", iDOfEnd) :
                new ObjectParameter("IDOfEnd", typeof(int));
    
            var numberOfFileParameter = numberOfFile != null ?
                new ObjectParameter("NumberOfFile", numberOfFile) :
                new ObjectParameter("NumberOfFile", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CreatingCSVProducts", iDOfStartParameter, iDOfEndParameter, numberOfFileParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_Export(string path_export, string sdt, string table)
        {
            var path_exportParameter = path_export != null ?
                new ObjectParameter("Path_export", path_export) :
                new ObjectParameter("Path_export", typeof(string));
    
            var sdtParameter = sdt != null ?
                new ObjectParameter("sdt", sdt) :
                new ObjectParameter("sdt", typeof(string));
    
            var tableParameter = table != null ?
                new ObjectParameter("Table", table) :
                new ObjectParameter("Table", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_Export", path_exportParameter, sdtParameter, tableParameter);
        }
    
        public virtual int sp_ExportPriceListCSV(string fileName, Nullable<System.DateTime> initialDate, Nullable<System.DateTime> finalDate)
        {
            var fileNameParameter = fileName != null ?
                new ObjectParameter("FileName", fileName) :
                new ObjectParameter("FileName", typeof(string));
    
            var initialDateParameter = initialDate.HasValue ?
                new ObjectParameter("InitialDate", initialDate) :
                new ObjectParameter("InitialDate", typeof(System.DateTime));
    
            var finalDateParameter = finalDate.HasValue ?
                new ObjectParameter("FinalDate", finalDate) :
                new ObjectParameter("FinalDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ExportPriceListCSV", fileNameParameter, initialDateParameter, finalDateParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_Price_list_imp(string file_name)
        {
            var file_nameParameter = file_name != null ?
                new ObjectParameter("File_name", file_name) :
                new ObjectParameter("File_name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_Price_list_imp", file_nameParameter);
        }
    
        public virtual int sp_Price_list_update(Nullable<int> id_Pharmacy, Nullable<int> id_Product, Nullable<decimal> price, Nullable<System.DateTime> date_upd)
        {
            var id_PharmacyParameter = id_Pharmacy.HasValue ?
                new ObjectParameter("Id_Pharmacy", id_Pharmacy) :
                new ObjectParameter("Id_Pharmacy", typeof(int));
    
            var id_ProductParameter = id_Product.HasValue ?
                new ObjectParameter("Id_Product", id_Product) :
                new ObjectParameter("Id_Product", typeof(int));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(decimal));
    
            var date_updParameter = date_upd.HasValue ?
                new ObjectParameter("Date_upd", date_upd) :
                new ObjectParameter("Date_upd", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_Price_list_update", id_PharmacyParameter, id_ProductParameter, priceParameter, date_updParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_Service_update(Nullable<int> id_Service, string name_short, string value)
        {
            var id_ServiceParameter = id_Service.HasValue ?
                new ObjectParameter("Id_Service", id_Service) :
                new ObjectParameter("Id_Service", typeof(int));
    
            var name_shortParameter = name_short != null ?
                new ObjectParameter("Name_short", name_short) :
                new ObjectParameter("Name_short", typeof(string));
    
            var valueParameter = value != null ?
                new ObjectParameter("Value", value) :
                new ObjectParameter("Value", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_Service_update", id_ServiceParameter, name_shortParameter, valueParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual int UpdatingAnnouncementsOfDrugstore(Nullable<int> iDOfDrugstore, Nullable<int> iD, string caption, string text, Nullable<bool> published)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var captionParameter = caption != null ?
                new ObjectParameter("Caption", caption) :
                new ObjectParameter("Caption", typeof(string));
    
            var textParameter = text != null ?
                new ObjectParameter("Text", text) :
                new ObjectParameter("Text", typeof(string));
    
            var publishedParameter = published.HasValue ?
                new ObjectParameter("Published", published) :
                new ObjectParameter("Published", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingAnnouncementsOfDrugstore", iDOfDrugstoreParameter, iDParameter, captionParameter, textParameter, publishedParameter);
        }
    
        public virtual int UpdatingDatesOfTransfer(Nullable<int> iDOfDrugstore, Nullable<int> iD, string name, Nullable<int> value, Nullable<System.DateTime> date)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var valueParameter = value.HasValue ?
                new ObjectParameter("Value", value) :
                new ObjectParameter("Value", typeof(int));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingDatesOfTransfer", iDOfDrugstoreParameter, iDParameter, nameParameter, valueParameter, dateParameter);
        }
    
        public virtual int UpdatingInformationOfSettings(Nullable<int> iDOfDrugstore, string key, string value)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var keyParameter = key != null ?
                new ObjectParameter("Key", key) :
                new ObjectParameter("Key", typeof(string));
    
            var valueParameter = value != null ?
                new ObjectParameter("Value", value) :
                new ObjectParameter("Value", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingInformationOfSettings", iDOfDrugstoreParameter, keyParameter, valueParameter);
        }
    
        public virtual int UpdatingListOfSettings(Nullable<int> iDOfDrugstore, string key, string value)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var keyParameter = key != null ?
                new ObjectParameter("Key", key) :
                new ObjectParameter("Key", typeof(string));
    
            var valueParameter = value != null ?
                new ObjectParameter("Value", value) :
                new ObjectParameter("Value", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingListOfSettings", iDOfDrugstoreParameter, keyParameter, valueParameter);
        }
    
        public virtual int UpdatingLogsOfDrugstores(Nullable<int> iDOfDrugstore, string systemLog)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var systemLogParameter = systemLog != null ?
                new ObjectParameter("SystemLog", systemLog) :
                new ObjectParameter("SystemLog", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingLogsOfDrugstores", iDOfDrugstoreParameter, systemLogParameter);
        }
    
        public virtual int UpdatingPriceList(Nullable<int> iDOfDrugstore, Nullable<int> iDOfReception, Nullable<int> p1, Nullable<decimal> p2, Nullable<bool> p3, Nullable<bool> p4)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var iDOfReceptionParameter = iDOfReception.HasValue ?
                new ObjectParameter("IDOfReception", iDOfReception) :
                new ObjectParameter("IDOfReception", typeof(int));
    
            var p1Parameter = p1.HasValue ?
                new ObjectParameter("P1", p1) :
                new ObjectParameter("P1", typeof(int));
    
            var p2Parameter = p2.HasValue ?
                new ObjectParameter("P2", p2) :
                new ObjectParameter("P2", typeof(decimal));
    
            var p3Parameter = p3.HasValue ?
                new ObjectParameter("P3", p3) :
                new ObjectParameter("P3", typeof(bool));
    
            var p4Parameter = p4.HasValue ?
                new ObjectParameter("P4", p4) :
                new ObjectParameter("P4", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingPriceList", iDOfDrugstoreParameter, iDOfReceptionParameter, p1Parameter, p2Parameter, p3Parameter, p4Parameter);
        }
    
        public virtual int UpdatingRegistrationOfDrugstores(Nullable<int> iDOfDrugstore, Nullable<int> iD, string pathToFolderOfPriceLists, string maskOfFullPriceList, string maskOfIncomingPriceList, string maskOfSoldPriceList, Nullable<bool> useOfIDOfPriceList, Nullable<bool> notDeletingPriceList)
        {
            var iDOfDrugstoreParameter = iDOfDrugstore.HasValue ?
                new ObjectParameter("IDOfDrugstore", iDOfDrugstore) :
                new ObjectParameter("IDOfDrugstore", typeof(int));
    
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var pathToFolderOfPriceListsParameter = pathToFolderOfPriceLists != null ?
                new ObjectParameter("PathToFolderOfPriceLists", pathToFolderOfPriceLists) :
                new ObjectParameter("PathToFolderOfPriceLists", typeof(string));
    
            var maskOfFullPriceListParameter = maskOfFullPriceList != null ?
                new ObjectParameter("MaskOfFullPriceList", maskOfFullPriceList) :
                new ObjectParameter("MaskOfFullPriceList", typeof(string));
    
            var maskOfIncomingPriceListParameter = maskOfIncomingPriceList != null ?
                new ObjectParameter("MaskOfIncomingPriceList", maskOfIncomingPriceList) :
                new ObjectParameter("MaskOfIncomingPriceList", typeof(string));
    
            var maskOfSoldPriceListParameter = maskOfSoldPriceList != null ?
                new ObjectParameter("MaskOfSoldPriceList", maskOfSoldPriceList) :
                new ObjectParameter("MaskOfSoldPriceList", typeof(string));
    
            var useOfIDOfPriceListParameter = useOfIDOfPriceList.HasValue ?
                new ObjectParameter("UseOfIDOfPriceList", useOfIDOfPriceList) :
                new ObjectParameter("UseOfIDOfPriceList", typeof(bool));
    
            var notDeletingPriceListParameter = notDeletingPriceList.HasValue ?
                new ObjectParameter("NotDeletingPriceList", notDeletingPriceList) :
                new ObjectParameter("NotDeletingPriceList", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdatingRegistrationOfDrugstores", iDOfDrugstoreParameter, iDParameter, pathToFolderOfPriceListsParameter, maskOfFullPriceListParameter, maskOfIncomingPriceListParameter, maskOfSoldPriceListParameter, useOfIDOfPriceListParameter, notDeletingPriceListParameter);
        }
    }
}
