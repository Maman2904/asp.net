using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IRegistration
    {
        int InsertRegistration(RegistrationModel model, String file_id_card_name, String file_npwp_name, String file_sppkp_name, String file_vendor_statement_name, String token, String type = "Registration");
        bool InsertCompany(RegistrationModel company, int vendor_id);
        bool InsertLog(TempLogVendorStatusModel model);
        bool updateVerifyEmail(int vendor_id, string verified_email, int verification_status, int position_data);
        string GetToken(int vendor_id);
        int GetVerifiedEmail(int vendor_id);
        int GetVerificationStatus(int vendor_id);
        IEnumerable<CompanyModel> GetCompany();
        IEnumerable<CurrencyModel> GetCurrency();
        IEnumerable<TitleModel> GetTitle();
        IEnumerable<CountryModel> GetCountry();
        // IEnumerable<TaxTypeModel> GetTaxType();
        IEnumerable<TaxNumberTypeModel> GetTaxNumberType();
        IEnumerable<BankModel> GetBank(string country_of_origin);
        IEnumerable<VendorTypeModel> GetVendorType();
        VendorTypeModel GetVendorTypeById(int vendor_type_id);
        SchemeGroupModel GetSchemeGroup(int vendor_type_id);
        IEnumerable<TopModel> GetTop();
        IEnumerable<PicModel> GetPic();
        IEnumerable<ProvinceModel> GetProvince();
        GlModel GetGl(int vendor_type_id);

        //for api service
        
        IEnumerable<CompanyModel> GetApiCompany(int vendor_type_id);
        IEnumerable<TaxTypeModel> GetApiTaxType(string account_group_id);
        IEnumerable<TaxNumberTypeModel> GetApiTaxNumberType(string account_group_id);
        TaxTypeModel GetApiRowTaxType(int tax_type_id);
        BankModel GetApiRowBank(string bank_id);
        IEnumerable<SgCategoryModel> GetApiSgCategory(int vendor_type_id);
        IEnumerable<BankModel> GetBankList();
        BankModel GetBankDetail(string bank_id);
        IEnumerable<SchemeGroupModel> GetMSchemeGroup();
        IEnumerable<GlModel> GetMGl();
    }
}