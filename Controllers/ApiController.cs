using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using tufol.Models;
using tufol.Helpers;
using tufol.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using Microsoft.Extensions.Options;

namespace tufol.Controllers
{
  public class ApiController : Controller
  {
        private IWebHostEnvironment _iHostingEnvironment;
        private String _current_language;
        private readonly Helpers.AppSettings _appSettings;
        private readonly Helpers.DefaultValue _defaultValue;
        private IRegistration _registration;
        private ISetting _setting;
        private IUserAccessApi _userTest;
        private IUser _user;
        private IMaster _master;
        private IVendor _vendor;
        private ITicket _ticket;
        private ITempCompanyRelation _company;
        public ApiController(IRegistration registration, ITempCompanyRelation company, IUser user, IMaster master, IVendor vendor, IUserAccessApi userTest, ISetting setting, ITicket ticket, IWebHostEnvironment iHostingEnvironment, IOptions<Helpers.AppSettings> appSettings, IOptions<Helpers.DefaultValue> defaultValue)
        {
            // this._httpContextAccessor = httpContextAccessor;
            _setting = setting;
            _registration = registration;
            _user = user;
            _master = master;
            _vendor = vendor;
            _ticket = ticket;
            _iHostingEnvironment = iHostingEnvironment;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            _appSettings = appSettings.Value;
            _defaultValue = defaultValue.Value;
            _company = company;
        }

    [HttpGet, ActionName("getApiTaxNumberType")]
    public IActionResult GetApiTaxNumberType(string account_group_id)
    {
      Console.WriteLine("account_id: " + account_group_id);
      var result = _registration.GetApiTaxNumberType(account_group_id);
      return Json(new { status = 200, data = result });
    }


    [HttpGet, ActionName("getApiTaxType")]
    public IActionResult GetApiTaxType(string account_group_id)
    {
      Console.WriteLine("account_id: " + account_group_id);
      var result = _registration.GetApiTaxType(account_group_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("getApiCompany")]
    public IActionResult GetApiCompany(int vendor_type_id)
    {
      Console.WriteLine("vendor type: " + vendor_type_id);
      var result = _registration.GetApiCompany(vendor_type_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("tax_number_type")]
    public IActionResult GetTaxNumberType()
    {
      var result = _registration.GetTaxNumberType();

      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("getApiRowTaxType")]
    public IActionResult GetApiRowTaxType(int tax_type_id)
    {
      var result = _registration.GetApiRowTaxType(tax_type_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("getApiRowBank")]
    public IActionResult GetApiRowBank(string bank_id)
    {
      var result = _registration.GetApiRowBank(bank_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("getApiSgCategory")]
    public IActionResult GetApiSgCategory(int vendor_type_id)
    {
      var result = _registration.GetApiSgCategory(vendor_type_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("currency")]
    public IActionResult GetApiCurrency()
    {
      var result = _registration.GetCurrency();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("title")]
    public IActionResult GetTitle()
    {
      var result = _registration.GetTitle();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("country")]
    public IActionResult GetCountry()
    {
      var result = _registration.GetCountry();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("bank")]
    public IActionResult GetBank(string country_of_origin = null)
    {
      var result = _registration.GetBank(country_of_origin);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("vendor_type")]
    public IActionResult GetVendorType()
    {
      var result = _registration.GetVendorType();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("vendor_type_by_id")]
    public IActionResult GetVendorTypeById(int vendor_type_id)
    {
      var result = _registration.GetVendorTypeById(vendor_type_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("scheme_group")]
    public IActionResult GetSchemeGroup(int vendor_type_id)
    {
      var result = _registration.GetSchemeGroup(vendor_type_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("gl")]
    public IActionResult GetGl(int vendor_type_id)
    {
      var result = _registration.GetGl(vendor_type_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("top")]
    public IActionResult GetTop()
    {
      var result = _registration.GetTop();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("pic")]
    public IActionResult GetPic()
    {
      var result = _registration.GetPic();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("province")]
    public IActionResult GetProvince()
    {
      var result = _registration.GetProvince();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("company")]
    public IActionResult GetCompany()
    {
        var result = _registration.GetCompany();
        return Json(new { status = 200, data = result});
    }

    [HttpGet, ActionName("CompanyByVendorNumber")]
    public IActionResult GetCompanyByVendorNumber(string vendor_number)
    {
        var result = _ticket.GetCompany(vendor_number);
        return Json(new { status = 200, data = result});
    }

    [HttpGet, ActionName("user_role")]
    public IActionResult GetRole()
    {
      var result = _user.GetRole();
      // Console.WriteLine("reuslll:"+result);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("procurement_group")]
    public IActionResult GetProGroup()
    {
      var result = _user.GetProGroup();
      // Console.WriteLine("reuslll:"+result);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("purchase_organization_group")]
    public IActionResult GetPurchaseOrganizationGroup(string company, int vendor_type)
    {
      var result = _master.GetPurchaseOrganizationGroup(company, vendor_type);
      // Console.WriteLine("reuslll:"+result);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("tax_status")]
    public IActionResult GetTaxStatus()
    {
      var result = _ticket.GetTaxStatus();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("po_type_group")]
    public IActionResult GetPOTypeGroup()
    {
      var result = _master.GetPoTypeGroup();
      return Json(new { status = 200, data = result });
    }
    
    [HttpGet, ActionName("po_type")]
    public IActionResult GetPOType(int po_type_group_id)
    {
      var result = _ticket.GetPOType(po_type_group_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("simulation_result")]
    public IActionResult GetSimulationResult(string ticket_number)
    {
      var result = _ticket.GetSimulationResult(ticket_number);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("po_number")]
    public IActionResult GetPONumber(int po_type_group_id, int po_type, string vendor_number, string company_id = null)
    {
      var result = _ticket.GetPONumber(po_type_group_id, po_type, vendor_number, company_id);
      return Json(new { status = 200, data = result });
    }
    [HttpGet, ActionName("po_number_by_number")]
    public IActionResult GetPONumberByNumber(string company_id, string po_number, string vendor_number)
    {
      var result = _ticket.GetPONumberByNumber(company_id, po_number, vendor_number);
      return Json(new { status = 200, data = result });
    }
    [HttpGet, ActionName("table_po")]
    public IActionResult GetPOTable(int po_type_group_id, string company_id, string po_number, string vendor_number, string gr_number = null)
    {
      var result = _ticket.GetPOTable(po_type_group_id, company_id, po_number, vendor_number, gr_number);
      return Json(new { status = 200, data = result });
    }
    [HttpGet, ActionName("ppn")]
    public IActionResult GetPpn(string country_id, string? area_code)
    {
      var result = _master.GetCountryTax(country_id, area_code);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("gr_number")]
    public IActionResult GetGrNumber(string po_number, int po_type_group_id, int po_type, string vendor_number, string company_id = null)
    {
      var result = _ticket.GetGrNumber(po_number, po_type_group_id, po_type, vendor_number, company_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("company_area")]
    public IActionResult GetCompanyArea(string company_id)
    {
      var result = _master.GetCompanyArea(company_id);
      return Json(new { status = 200, data = result });
    }
    [HttpGet, ActionName("bussiness_area")]
    public IActionResult GetBussinessArea(string company_id, string area_code)
    {
      var result = _master.GetBussinessArea(company_id, area_code);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("location")]
    public IActionResult GetLocation()
    {
      var result = _ticket.GetLocation();
      return Json(new { status = 200, data = result });
    }
    
    [HttpGet, ActionName("area_by_id")]
    public IActionResult GetArea(string area_code)
    {
      var result = _ticket.GetAreaById(area_code);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("gl_item")]
    public IActionResult GetGlItem()
    {
      var result = _ticket.GetGlItem();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("cc_item")]
    public IActionResult GetCcItem()
    {
      var result = _ticket.GetCcItem();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("material_itema")]
    public IActionResult GetMaterialIList()
    {
      var result = _ticket.GetMaterialItem();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("material_item")]
    public IActionResult GetMaterialItem(string q, int page)
    {
      var result = _ticket.GetMaterialItem(q);
      var total_count = _ticket.CountMaterialItem(q);
      return Json(new { status = 200, total_count = total_count, items = result });
    }

    [HttpGet, ActionName("material_item_by_id")]
    public IActionResult GetMaterialItemById(string material_id)
    {
      var result = _ticket.GetMaterialItemById(material_id);
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("table_po_ticket")]
    public IActionResult GetTablePo(string ticket_number)
    {
      var result = _ticket.GetPoTicket(ticket_number);
      return Json(new { status = 200, data = result });
    }

    public bool ProtectRoute (string access_token) {

      if (String.IsNullOrEmpty(access_token))
      {
        return false;
      }
      else
      {
        var apiKey = _userTest.GetUserDetail(new Helpers.GlobalFunction().GetMD5(access_token));

        if (String.IsNullOrEmpty(apiKey.access_token))
        {
          return false;
        }
      }

      return true;
    }

    [HttpGet, ActionName("GetBankList")]
    [Route("api/GetBankList")]
    public IActionResult GetBankList(string? page, string? limit)
    {
      var result = _registration.GetBankList();
      return Json(new { status = 200, data = result });
    }
    
    [HttpGet, ActionName("GetMaterialList")]
    [Route("api/GetMaterialList")]
    public IActionResult GetMaterialList(string? page, string? limit, string? search)
    {
      int limitRow = Convert.ToInt32(limit);
      int currPage = Convert.ToInt32(page);
      int offset = (currPage>1) ? (currPage * limitRow) - limitRow : 0;

      var totalRows = _ticket.GetMaterialListCount().Select(t => t.c).FirstOrDefault();

      var result = _ticket.GetMaterialList(offset, limitRow, search);
      return Json(new { status = 200,
      count = totalRows,
      currentPage = currPage,
      data = result 
      });
    }

      [HttpGet, ActionName("bank_list_datatable")]
        public IActionResult LoadBankData(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null)
        {
            // if (!_permission.allow_read)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            try
            {
                var data = _vendor.GetBankListDatatable(pagination, sort, query);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetBankListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

    [HttpGet, ActionName("m_scheme_group")]
    public IActionResult GetMSchemeGroup()
    {
      var result = _registration.GetMSchemeGroup();
      return Json(new { status = 200, data = result });
    }

    [HttpGet, ActionName("m_gl")]
    public IActionResult GetMGl()
    {
      var result = _registration.GetMGl();
      return Json(new { status = 200, data = result });
    }
  }
}