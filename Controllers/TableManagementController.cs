using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using tufol.Interfaces;
using tufol.Models;

namespace tufol.Controllers
{
    public class TableManagementController : Controller
    {
        private IMenu _menu;
        private IVendor _vendor;
        private IRegistration _registration;
        private IUser _user;
        private ISetting _setting;
        private String _current_language;
        private readonly Helpers.AppSettings _appSettings;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly PermissionModel _permission;
        private readonly IStringLocalizer<TableManagementController> _localizer;
        private IWebHostEnvironment _iHostingEnvironment; 
        private IMasterTable _masterTable;
        private ITicket _ticket;
        public TableManagementController(ITicket ticket, IVendor vendor, IRegistration registration, IMenu menu, IUser user,ISetting setting, IOptions<Helpers.AppSettings> appSettings, IStringLocalizer<TableManagementController> localizer, IWebHostEnvironment iHostingEnvironment, IHttpContextAccessor httpContextAccessor, IMasterTable masterTable)
        {
            _ticket = ticket;
            _vendor = vendor;
            _registration = registration;
            _menu = menu;
            _user = user;
            _setting = setting;
            _appSettings = appSettings.Value;
            _iHostingEnvironment = iHostingEnvironment;
            _localizer = localizer;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            _list_menu = _menu.GetMenu(httpContextAccessor.HttpContext.Session.GetString("role_id"));
            _permission = _menu.GetPermission("TableManagement", httpContextAccessor.HttpContext.Session.GetString("role_id"));
        }

        // GET: /TableManagement/all_user_datatable/
        [HttpGet, ActionName("all_bank_datatable")]
        public IActionResult LoadDataBank(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                var data = _vendor.GetBankListDatatable(pagination, sort, query);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetbankListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        // GET: /TableManagement/all_company_code_datatable/
        [HttpGet, ActionName("all_company_code_datatable")]
        public IActionResult LoadDataCompanyCode(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                var data = _vendor.GetCompanyCodeListDatatable(pagination, sort, query);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCompanyCodeListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        // GET: /TableManagement/all_material_items_datatable/
        [HttpGet, ActionName("all_material_items_datatable")]
        public IActionResult LoadDataMaterialItems(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                var data = _vendor.GetMaterialListDatatable(pagination, sort, query);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetMaterialistDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

         // GET: /TableManagement/all_cc_items_datatable/
        [HttpGet, ActionName("all_cc_items_datatable")]
        public IActionResult LoadDataCcItems(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                var data = _vendor.GetCcItemsListDatatable(pagination, sort, query);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCItemsListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        // GET: /TableManagement/all_vendor_group_datatable/
        [HttpGet, ActionName("all_vendor_group_datatable")]
        public IActionResult LoadDataVendorGroup(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                var data = _vendor.GetVendorGroupListDatatable(pagination, sort, query);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorGroupListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        // GET: /TableManagement/Bank
         [Route("/{culture}/{controller}/Bank")]
         public IActionResult Bank()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            // passing data from controller to view
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            return View("~/Views/TableManagement/Bank/Index.cshtml");
        }

         // GET: /TableManagement/Bank/Detil
        [Route("/{culture}/{controller}/Bank/Detil/{bank_id}")]
        public IActionResult Detil(string bank_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataBank = _registration.GetBankDetail(bank_id);

            ViewBag.DataBank = DataBank;
            Console.WriteLine("Data Bank: "+DataBank+" name:"+DataBank.name);
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Bank/Detil.cshtml");
        }

         // GET: /TableManagement/Bank/Edit
        [Route("/{culture}/{controller}/Bank/Edit/{bank_id}")]
        public IActionResult Edit(string bank_id)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataBank = _registration.GetBankDetail(bank_id);

            ViewBag.DataBank = DataBank;
            ViewData["name"] = HttpContext.Session.GetString("name");
            // ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["returnUrl"] = "/"+_current_language+"/TableManagement/Bank";
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("~/Views/TableManagement/Bank/Edit.cshtml");
        }

        // Put: TableManagement/Bank/Edit
        [HttpPost, ActionName("EditBank")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditBank(BankModel model)
        {
            //Console.WriteLine("id "+ model.bank_id);
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                if(ModelState.IsValid){
                _vendor.updateBank(model);
                return RedirectToAction("Bank", "TableManagement");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var DataBank = _registration.GetBankDetail(model.bank_id);
            ViewBag.DataBank = DataBank;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Bank/Edit.cshtml");
        }

         public IActionResult AddBank()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            BankModel DataBank = new BankModel();
            ViewBag.DataBank = DataBank;
            // ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["returnUrl"] = "/"+_current_language+"/TableManagement/Bank";
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Bank/Add.cshtml");
        }

        // POST: UserManagement/Add 
        [HttpPost, ActionName("AddBank")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> AddBank(BankModel model)
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
                if(ModelState.IsValid){
                    var cekBank = _vendor.CekBankExist(model.bank_id);
                    if(!cekBank){
                        var result = _vendor.InsertBank(model);
                        if (result) {
                            Console.WriteLine("okee");
                            ViewBag.registration_success = _localizer.GetString("SuccessToSave");
                            return View("~/Views/TableManagement/Bank/Index.cshtml");
                        } 
                        else {
                            ViewBag.error = _localizer.GetString("FailedToSave");
                            return View("~/Views/TableManagement/Bank/Index.cshtml");
                        }
                    }
                    else {
                        Console.WriteLine("sudah ada");
                            ViewBag.error = _localizer.GetString("IdExist");
                            //return View("~/Views/TableManagement/Bank/Add.cshtml");
                    }
                }
            } 
            catch (Exception ex)  {
                Console.WriteLine(ex);
            }
            BankModel DataBank = new BankModel();
            ViewBag.DataBank = DataBank;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Bank/Add.cshtml");
            
        }

        // DELETE: UserManagement/Delete/5
        [HttpDelete, ActionName("DeleteBank")]
        public async Task<IActionResult> DeleteBank(string id)
        {
            // if (!_permission.allow_delete)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            //Console.WriteLine("error : " +id);
            try {
              var data =  _vendor.DeleteBank(id);
            } catch (Exception ex)  {
                Console.WriteLine("error : "+ex);
            }
            return RedirectToAction(nameof(Bank));
        }

        // GET: /TableManagement/Material
         [Route("/{culture}/{controller}/Material")]
         public IActionResult Material()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            // passing data from controller to view
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            return View("~/Views/TableManagement/Material/Index.cshtml");
        }

        [Route("/{culture}/{controller}/Material/Detil/{material_id}")]
        public IActionResult DetilMaterial(string material_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataMaterial = _ticket.GetMaterialItemById(material_id);

            ViewBag.DataMaterial = DataMaterial;
            Console.WriteLine("Data Material: "+DataMaterial+" name:"+DataMaterial.description);
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Bank/Detil.cshtml");
        }

         // GET: /TableManagement/Material/Edit
        [Route("/{culture}/{controller}/Material/Edit/{material_id}")]
        public IActionResult EditMaterialGet(string material_id)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataMaterial = _ticket.GetMaterialItemById(material_id);

            ViewBag.DataMaterial = DataMaterial;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("~/Views/TableManagement/Material/Edit.cshtml");
        }

        // Put: TableManagement/Material/Edit
        [HttpPost, ActionName("EditMaterial")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditMaterial(MaterialItemModel model)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                if(ModelState.IsValid){
                    _vendor.updateMaterial(model);
                    return RedirectToAction("Material", "TableManagement");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var DataMaterial = _ticket.GetMaterialItemById(model.material_id);
            ViewBag.DataMaterial = DataMaterial;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Material/Edit.cshtml");
        }

         public IActionResult AddMaterial()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            MaterialItemModel DataMaterial = new MaterialItemModel();
            ViewBag.DataMaterial = DataMaterial;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Material/Add.cshtml");
        }

        // POST: TableManagement/AddMaterial 
        [HttpPost, ActionName("AddMaterial")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> AddMaterial(MaterialItemModel model)
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
                if(ModelState.IsValid){
                    var cekMaterial = _vendor.CekMaterialExist(model.material_id);
                    if(!cekMaterial){
                    var result = _vendor.InsertMaterial(model);
                        if (result) {
                        ViewBag.registration_success = _localizer.GetString("SuccessToSave");;
                        return View("~/Views/TableManagement/Material/Index.cshtml");
                        }
                         else {
                        ViewBag.error = _localizer.GetString("FailedToSave");
                        return View("~/Views/TableManagement/Material/Index.cshtml");
                        }

                    }
                    else {
                        Console.WriteLine("sudah ada");
                            ViewBag.error = _localizer.GetString("IdExist");
                            //return View("~/Views/TableManagement/Bank/Add.cshtml");
                    }
                }
            } 
            catch (Exception ex)  {
                Console.WriteLine(ex);
            }
            MaterialItemModel DataMaterial = new MaterialItemModel();
            ViewBag.DataMaterial = DataMaterial;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/Material/Add.cshtml");
            
        }

        // DELETE: UserManagement/Delete/5
        [HttpDelete, ActionName("DeleteMaterial")]
        public async Task<IActionResult> DeleteMaterialConfirmed(string id)
        {
            // if (!_permission.allow_delete)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            try {
              var data =  _vendor.DeleteMaterial(id);
            } catch (Exception ex)  {
                Console.WriteLine("error : "+ex);
            }
            return RedirectToAction(nameof(Material));
        }

         // GET: /TableManagement/CostCenter
         [Route("/{culture}/{controller}/CostCenter")]
         public IActionResult CostCenter()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            // passing data from controller to view
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            return View("~/Views/TableManagement/CostCenter/Index.cshtml");
        }

         // GET: /TableManagement/CostCenter/Detil
        [Route("/{culture}/{controller}/CostCenter/Detil/{cc_id}")]
        public IActionResult DetilCostCenter(string cc_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataCc = _vendor.GetCcItems(cc_id);

            ViewBag.DataCc = DataCc;
            Console.WriteLine("Data CC: "+DataCc+" name:"+DataCc.cc_id);
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/CostCenter/Detil.cshtml");
        }

         // GET: /TableManagement/CostCenter/Edit
        [Route("/{culture}/{controller}/CostCenter/Edit/{cc_id}")]
        public IActionResult EditCostCenterGET(string cc_id)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataCc = _vendor.GetCcItems(cc_id);

            ViewBag.DataCc = DataCc;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("~/Views/TableManagement/CostCenter/Edit.cshtml");
        }

        // Put: TableManagement/CostCenter/Edit
        [HttpPost, ActionName("EditCostCenter")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditCostCenter(CostCenterModel model)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                _vendor.updatecostCenter(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
             return RedirectToAction("CostCenter", "TableManagement");
        }

         public IActionResult AddCostCenter()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            CostCenterModel DataCc = new CostCenterModel();
            ViewBag.DataCc = DataCc;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/CostCenter/Add.cshtml");
        }

        // POST: UserManagement/Add 
        [HttpPost, ActionName("AddCostCenter")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> AddCostCenter(CostCenterModel model)
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
                if(ModelState.IsValid){
                    var CekCC = _vendor.CekCcExist(model.cc_id);
                    if(!CekCC){
                    var result = _vendor.InsertCostCenter(model);
                    if (result) {
                        Console.WriteLine("okee");
                        ViewBag.registration_success = _localizer.GetString("SuccessToSave");
                        return View("~/Views/TableManagement/CostCenter/Index.cshtml");
                    } else {
                        ViewBag.error = _localizer.GetString("FailedToSave");
                        return View("~/Views/TableManagement/CostCenter/Index.cshtml");
                    }

                    }
                     else {
                        Console.WriteLine("sudah ada");
                            ViewBag.error = _localizer.GetString("IdExist");
                            //return View("~/Views/TableManagement/Bank/Add.cshtml");
                    }
                }
            } 
            catch (Exception ex)  {
                Console.WriteLine(ex);
            }
             CostCenterModel DataCc = new CostCenterModel();
            ViewBag.DataCc = DataCc;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/CostCenter/Add.cshtml");
            
        }

        // DELETE: UserManagement/Delete/5
        [HttpDelete, ActionName("DeleteCostCenter")]

        public async Task<IActionResult> DeleteCostCenterConfirmed(string id)
        {
            // if (!_permission.allow_delete)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            try {
              var data =  _vendor.DeleteCostCenter(id);
            } catch (Exception ex)  {
                Console.WriteLine("error : "+ex);
            }
            return RedirectToAction(nameof(CostCenter));
        }

         // GET: /TableManagement/CostCenter
         [Route("/{culture}/{controller}/CompanyCode")]
         public IActionResult CompanyCode()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            // passing data from controller to view
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            return View("~/Views/TableManagement/CompanyCode/Index.cshtml");
        }

         // GET: /TableManagement/CostCenter/Detil
        [Route("/{culture}/{controller}/CompanyCode/Detil/{company_id}")]
        public IActionResult DetilCompanyCode(string company_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataCompany = _vendor.GetCompanyDetail(company_id);

            ViewBag.DataCompany = DataCompany;
            Console.WriteLine("Data CC: "+DataCompany+" name:"+DataCompany.company_id);
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/CompanyCode/Detil.cshtml");
        }

         // GET: /TableManagement/CostCenter/Edit
        [Route("/{culture}/{controller}/CompanyCode/Edit/{company_id}")]
        public IActionResult EditCompanyCode(string company_id)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataCompany = _vendor.GetCompanyDetail(company_id);

            ViewBag.DataCompany = DataCompany;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("~/Views/TableManagement/CompanyCode/Edit.cshtml");
        }

        // Put: TableManagement/CostCenter/Edit
        [HttpPost, ActionName("EditCompany")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditCompany(CompanyModel model)
        {
            Console.WriteLine("return");

            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                Console.WriteLine("per");
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                Console.WriteLine("update");
                _vendor.updateCompany(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("ret");
            return RedirectToAction("CompanyCode", "TableManagement");
        }

         public IActionResult AddCompanyCode()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            CompanyModel DataCompany = new CompanyModel();
            ViewBag.DataCompany = DataCompany;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/CompanyCode/Add.cshtml");
        }

        // POST: UserManagement/Add 
        [HttpPost, ActionName("AddCompanyCode")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> AddCompanyCode(CompanyModel model)
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
                if(ModelState.IsValid){
                     var cekBank = _vendor.CekcompanyCodeExist(model.company_id);
                    if(!cekBank){
                        var result = _vendor.InsertCompany(model);
                        if (result) {
                            Console.WriteLine("okee");
                            ViewBag.registration_success = _localizer.GetString("SuccessToSave");
                            return View("~/Views/TableManagement/CompanyCode/Index.cshtml");
                        } 
                        else {
                            ViewBag.error = _localizer.GetString("FailedToSave");
                            return View("~/Views/TableManagement/CompanyCode/Index.cshtml");
                        }
                    }
                    else {
                        Console.WriteLine("sudah ada");
                            ViewBag.error = _localizer.GetString("IdExist");
                            //return View("~/Views/TableManagement/Bank/Add.cshtml");
                    }
                }
            } 
            catch (Exception ex)  {
                Console.WriteLine(ex);
            }
            CompanyModel DataCompany = new CompanyModel();
            ViewBag.DataCompany = DataCompany;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/CompanyCode/Add.cshtml");
            
        }

        // DELETE: TableManagement/Delete/5
        [HttpDelete, ActionName("DeleteCompanyCode")]

        public async Task<IActionResult> DeleteCompanyCodeConfirmed(string id)
        {
            // if (!_permission.allow_delete)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            try {
              var data =  _vendor.DeleteCompany(id);
            } catch (Exception ex)  {
                Console.WriteLine("error : "+ex);
            }
            return RedirectToAction(nameof(CompanyCode));
        }

         // GET: /TableManagement/VendorGroup
         [Route("/{culture}/{controller}/VendorGroup")]
         public IActionResult VendorGroup()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            // passing data from controller to view
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            return View("~/Views/TableManagement/VendorGroup/Index.cshtml");
        }

         // GET: /TableManagement/CostCenter/Detil
        [Route("/{culture}/{controller}/VendorGroup/Detil/{company_id}")]
        public IActionResult DetilVendorGroup(int vendor_type_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataVendorGroup = _vendor.GetVendorGroupDetail(vendor_type_id);

            ViewBag.DataVendorGroup = DataVendorGroup;
            Console.WriteLine("Data CC: "+DataVendorGroup+" name:"+DataVendorGroup.vendor_type_id);
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/VendorGroup/Detil.cshtml");
        }

         // GET: /TableManagement/CostCenter/Edit
        [Route("/{culture}/{controller}/VendorGroup/Edit/{vendor_type_id}")]
        public IActionResult EditVendorGroupGet(int vendor_type_id)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataVendorGroup = _vendor.GetVendorGroupDetail(vendor_type_id);

            ViewBag.DataVendorGroup = DataVendorGroup;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("~/Views/TableManagement/VendorGroup/Edit.cshtml");
        }

        // Put: TableManagement/CostCenter/Edit
        [HttpPost, ActionName("EditVendorGroup")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditVendorGroup(VendorTypeModel model)
        {
            Console.WriteLine("masuk" + model.vendor_type_id);
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                _vendor.updateVendorGroup(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
             return RedirectToAction("VendorGroup", "TableManagement");
        }

         public IActionResult AddVendorGroup()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            VendorTypeModel DataVendorGroup = new VendorTypeModel();
            ViewBag.DataVendorGroup = DataVendorGroup;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/VendorGroup/Add.cshtml");
        }

        // POST: UserManagement/Add 
        [HttpPost, ActionName("AddVendorGroup")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> AddVendorGroup(VendorTypeModel model)
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            String error_message = "";
            String alert_message = "";
            String icon_message = "";
            try {
                if(ModelState.IsValid){
                    var result = _vendor.InsertVendorGroup(model);
                    if (result) {
                        error_message = _localizer.GetString("SuccessToSave");
                        icon_message = "far fa-check-circle";
                        alert_message = "alert-success";
                        return RedirectToAction("VendorGroup", "TableManagement");
                    } else {
                        error_message = _localizer.GetString("FailedToSave");
                        icon_message = "flaticon-circle";
                        alert_message = "alert-danger";
                    }
                }
            } 
            catch (Exception ex)  {
                Console.WriteLine(ex);
            }
            ViewBag.error_message = error_message;
            ViewBag.alert_message = alert_message;
            ViewBag.icon_message = icon_message;
            VendorTypeModel DataVendorGroup = new VendorTypeModel();
            ViewBag.DataVendorGroup = DataVendorGroup;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            return View("~/Views/TableManagement/VendorGroup/Add.cshtml");
            
        }

        // DELETE: TableManagement/Delete/5
        [HttpDelete, ActionName("DeleteVendorGroup")]

        public async Task<IActionResult> DeleteVendorGroupConfirmed(int id)
        {
            // if (!_permission.allow_delete)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            try {
              var data =  _vendor.DeleteVendorGroup(id);
            } catch (Exception ex)  {
                Console.WriteLine("error : "+ex);
            }
            return RedirectToAction(nameof(VendorGroup));
        }
    }
}