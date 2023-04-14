using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IMaster
    {
        IEnumerable<PurchaseOrganizationModel> GetPurchaseOrganizationGroup(string company_id, int vendor_type_id);
        ProcurementGroupModel GetProcurementGroupByID(string initial_area);
        public Dictionary<string, dynamic> GetAnnouncementList(string culture);
        AnnouncementModel GetAnnouncementDetail(string key_flag);

        bool UpdateAnnouncement(AnnouncementModel model, string key_flag);
        
        CountryTaxModel GetCountryTax(string country_id, string area_code);
        IEnumerable<CompanyAreaModel> GetCompanyArea(string company_id);
        IEnumerable<CompanyAreaModel> GetCompanyAreaCompany();
        string GetBussinessArea(string company_id, string area_code);
        // IEnumerable<RejectReasonModel> GetRejectReasons();
        IEnumerable<PoTypeGroupModel> GetPoTypeGroup();
    }
}