using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IVendor
    {
        Dictionary<string, dynamic> GetVendorListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query, string vendor_number = null, string name = null, int vendor_type_id = 0);
        int CountVendorData(string vendor_number = null, string name = null, int vendor_type_id = 0, string generalSearch = null);
        Dictionary<string, dynamic> GetPendingVendorDatatable(int position_data, string type, Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query, string vendor_number = null, string name = null, int vendor_type_id = 0);
        int CountPendingVendorData(int position_data, string type, string vendor_number = null, string name = null, int vendor_type_id = 0, string generalSearch = null);

        IEnumerable<LogVendorModel> GetLogVendor(string vendor_number);
        IEnumerable<TempLogVendorStatusModel> GetTempLogVendor(int vendor_id);
        IEnumerable<CompanyModel> GetCompany(string vendor_number);
        IEnumerable<CompanyModel> GetTempCompany(int vendor_id);
        VendorModel GetVendorDetail(string vendor_number); 
        TempVendorModel GetPendingVendorDetail(int vendor_id, string token = null);
        bool InsertVendor(VendorModel model);
        bool updateVendor(string vendor_number, RegistrationModel model, int is_locked, int change_request_status, String file_id_card_name = null, String file_npwp_name = null, String file_sppkp_name = null, String file_vendor_statement_name = null);
        bool updateTempVendor(int vendor_id, RegistrationModel model, String file_id_card_name = null, String file_npwp_name = null, String file_sppkp_name = null, String file_vendor_statement_name = null);
        bool updateVendorVerificationStatus(int vendor_id, int verification_status_id, int position_data, string verification_note = null, string token = null);
        bool deleteTempCompanyByVendorId(int vendor_id);
        bool deleteCompanyByVendorNumber(string vendor_number);
        bool InsertLogTempVendor(TempLogVendorStatusModel model);
        bool InsertLogVendor(LogVendorModel model);
        bool InsertTempCompany(RegistrationModel company, int vendor_id);
        bool InsertCompany(IEnumerable<CompanyModel> company, string vendor_number);
        IEnumerable<RpaTempVendorModel> ApprovedVendorRegistration(string type);
        IEnumerable<string> NewVendorActivation(int status_rpa, string rpa_status_description, string vendor_number);
        bool updateVendorNumber(string vendor_number, int vendor_id);
        bool updateIsLockedAndChangeRequestStatus(string vendor_number, int is_locked, int change_request_status, string? change_request_note);
        bool updateMailingData(string vendor_number, string mailing_address, string mailing_district, string mailing_village, string mailing_province);
        string GetIsLocked(string vendor_number);
        TempVendorModel GetLastDataChange(string vendor_number);
        bool DeleteTempVendor(int vendor_id);
        bool UpdateTempVendorRpa(TempVendorModel model, int? status_rpa, string? rpa_status_description);
        bool InsertBank(BankModel model);
        bool updateBank(BankModel model);
        string GetBank(string bank_id);
        bool CekBankExist(string bank_id);
        bool updateVendorRpa(VendorModel model);
        List<String> getListUpdatedColumn(VendorModel data_existing, VendorModel data_update);
        VendorModel mappingRegisToVendor(VendorModel DataVendor, RegistrationModel model, dynamic custom);
        IEnumerable<CompanyModel> GetCompanyRelation(string vendor_number);
        Dictionary<String, String> dictionaryField();
        List<String> getCompanyId();
        int updateIsExtension(int vendor_id, int is_extension);
        Task<bool> DeleteBank(string bank_id);
        bool updateMaterial(MaterialItemModel model);
        bool InsertMaterial(MaterialItemModel model);
        Task<bool> DeleteMaterial(string material_id);
        CostCenterModel GetCcItems(string cc_id);
        bool updatecostCenter(CostCenterModel model);
        bool InsertCostCenter(CostCenterModel model);
        Task<bool> DeleteCostCenter(string cc_id);
        CompanyModel GetCompanyDetail(string company_id);
        bool updateCompany(CompanyModel model);
        bool InsertCompany(CompanyModel model);
        Task<bool> DeleteCompany(string company_id);
        VendorTypeModel GetVendorGroupDetail(int vendor_type_id);
        bool updateVendorGroup(VendorTypeModel model);
        bool InsertVendorGroup(VendorTypeModel model);
        Task<bool> DeleteVendorGroup(int vendor_type_id);
        Dictionary<string, dynamic> GetBankListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query);
        Dictionary<string, dynamic> GetMaterialListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query);
        Dictionary<string, dynamic> GetCompanyCodeListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query);
        Dictionary<string, dynamic> GetCcItemsListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query);
        Dictionary<string, dynamic> GetVendorGroupListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query);
        bool CekMaterialExist(string id);
        bool CekCcExist(string id);
        bool CekcompanyCodeExist(string id);

    }
}