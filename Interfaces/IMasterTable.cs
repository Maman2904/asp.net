using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IMasterTable
    {
        Task<int> InsertMasterLPB(List<TempPoLpbModel> model);
        List<PoTransactionModel> GetTempPoTransaction(string company_id);
        List<PoTransactionModel> GetTempPoTransactionService();
        Task<int> InsertMasterPoTransaction(List<PoTransactionModel> model);
        Task<int> InsertMasterPoTransactionService(List<PoTransactionModel> model);
        List<LpbTransactionModel> GetTempLpbTransaction(string company_id);
        Task<int> InsertMasterLpbTransaction(List<LpbTransactionModel> model);
        Task<int> InsertMasterService(List<TempPoServiceModel> model);
        Task<int> InsertMasterPoOpen(List<PoOpenModel> model);
        bool DeleteTempPoLpb(string company_id);
        bool DeletePoTransaction(string company_id);
        bool DeleteLpbTransaction(string company_id);
        bool DeleteTempPoService();
        bool DeletePoTransactionService(string is_service);
        Task<List<PoOpenModel>> UpdateMasterPoOpen(List<PoOpenModel> model);
        Task<List<TempPoLpbModel>> UpdateMasterPoLPB(List<TempPoLpbModel> model);
        Task<List<TempPoServiceModel>> UpdateMasterPoSVC(List<TempPoServiceModel> model);
        Task<List<VendorModelRpa>> UpdateMasterVendor(List<VendorModelRpa> model);
        Task<int> InsertMasterVendor(List<VendorModelRpa> model);
        Task<int> InsertCompanyRelations(List<CompanyRelationModel> model);
        Task<int> DeleteCompanyRelations(List<CompanyRelationModel> vendor_list);
        int VendorTypeId(string sap_code);
        int TaxTypeId(string sap_code);
        int TaxNumberTypeId(string npwp);
        string CekVendor(string model);
        string GetCompanyIdByPurchOrg(string purch_org, string vendor_number);
        Task<int> InsertUserMaster(List<UserModel> model);
        bool Top(string bank_id);
        bool Currency(string bank_id);
        bool CekLpbTransaction(LpbTransactionModel model);
        bool UpdateLpbTransaction(LpbTransactionModel model);
        bool CekIsUsedLpb(LpbTransactionModel model);
        bool CekPoTransaction(PoTransactionModel model);
        bool CekIsUsedPo(PoTransactionModel model);
        bool UpdatePoTransaction(PoTransactionModel model);
        Task<int> InsertMasterLogVendor(List<LogVendorModel> model);
        //Task<int> InsertMasterTempVendor(List<TempVendorModel> model);
        // Task<int> InsertTempCompanyRelations(List<TempCompanyRelationModel> model);
        // bool CekSapTaxType(string sap_code);
        // int CekQuantity();
        //Task<int> InsertMasterBank(List<BankModel> model);
        //bool InsertVendor(VendorModelRpa model);
    }
}