using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using tufol.Models;
using tufol.Interfaces;
using System.Threading;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace tufol.Controllers
{
    public class ChangeVendorController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private IVendor _vendor;
        private IMaster _master;
        private IRegistration _registration;
        private String _current_language;
        private readonly Helpers.AppSettings _appSettings;
        private readonly IStringLocalizer<ChangeVendorController> _localizer;
        private IWebHostEnvironment _iHostingEnvironment;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly Helpers.DefaultValue _defaultValue;
        public ChangeVendorController(IMenu menu, IUser user, IVendor vendor, IMaster master, IRegistration registration, IStringLocalizer<ChangeVendorController> localizer, IWebHostEnvironment iHostingEnvironment, IOptions<Helpers.AppSettings> appSettings, IOptions<Helpers.DefaultValue> defaultValue, IHttpContextAccessor httpContextAccessor)
        {
            _localizer = localizer;
            _menu = menu;
            _user = user;
            _master = master;
            _vendor = vendor;
            _registration = registration;
            _appSettings = appSettings.Value;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            _iHostingEnvironment = iHostingEnvironment;
            _defaultValue = defaultValue.Value;
            _list_menu = _menu.GetMenu(httpContextAccessor.HttpContext.Session.GetString("role_id"));
        }
        [HttpGet]
        public IActionResult Index()
        {
            string role_user = HttpContext.Session.GetString("role_id");
            ViewBag.VendorNumber = HttpContext.Session.GetString("vendor_number");
            var DataVendor = _vendor.GetVendorDetail(ViewBag.VendorNumber);
            Console.WriteLine("Data vendor 1 :"+ DataVendor.foreign_currency_id);
            if (string.IsNullOrEmpty(DataVendor.vendor_number))
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("~/Views/Shared/NotFound.cshtml");
            }
            var is_locked = DataVendor.is_locked;
            var LastDataChange = _vendor.GetLastDataChange(ViewBag.VendorNumber);
            ViewBag.last_data_change = _vendor.GetLastDataChange(ViewBag.VendorNumber);
            ViewBag.vendor_id = LastDataChange.vendor_id != null ? LastDataChange.vendor_id : null;
            ViewBag.verification_status = String.IsNullOrEmpty(LastDataChange.status_for_vendor) ? null : LastDataChange.status_for_vendor;
            ViewBag.verification_status_note = String.IsNullOrEmpty(LastDataChange.verification_note) ? "-" : LastDataChange.verification_note;
            bool isReadOnly = false;
            if ( !string.IsNullOrEmpty(LastDataChange.vendor_number) )
            {
                if (Convert.ToInt32(LastDataChange.position_data) != Convert.ToInt32(role_user))
                    isReadOnly = true;

                DataVendor = LastDataChange;
            }
            Console.WriteLine("Data vendor 2 :"+ DataVendor.foreign_currency_id);
            ViewBag.vendor_data =  DataVendor;
            ViewBag.vendor_type_id = DataVendor.vendor_type_id;
            ViewBag.sg_category_id = DataVendor.sg_category_id;
            ViewBag.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(DataVendor.vendor_type_id, DataVendor.country_id);
            ViewBag.error_message = "";
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            ViewBag.language = _current_language;
            ViewBag.Menu = _list_menu;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            if(isReadOnly) {
                return View("Detil");
            }
            return View("Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Index(RegistrationModel model, string action)
        {
            bool result = false;
            var DataVendor = _vendor.GetVendorDetail(model.vendor_number);
            var tempVendor = _vendor.GetLastDataChange(model.vendor_number);

            string role_user = HttpContext.Session.GetString("role_id");
            int is_locked = 1;
            int verification_status_id = action == "Send" ? 16 : 9;
            int position_data = action == "Send" ? 3 : 2;
            int change_request_status = action == "Send" ? 0 : 1; 
            int is_add_new_company = 0;
            int is_updated_orther_data = 0;

            String token = Guid.NewGuid().ToString();
            model.vendor_type_name = DataVendor.vendor_type_name;
            model.position_data = position_data;
            model.token = token;
            model.verification_status_id = verification_status_id;          
            model.status_rpa = DataVendor.status_rpa;        

            String error_message = "";
            // foreach (var modelState in ViewData.ModelState.Values) {
            //     foreach (var error in modelState.Errors) {
            //         Console.WriteLine(error.ErrorMessage);
            //     }
            // }
            if (ModelState.IsValid)
            {
                string upload_basedir = "document/vendor/";
                upload_basedir += DateTime.Now.Year.ToString() + "/";
                if (!Directory.Exists(_iHostingEnvironment.WebRootPath + "/" + upload_basedir))
                {
                    Directory.CreateDirectory(_iHostingEnvironment.WebRootPath + "/" + upload_basedir);
                }
                upload_basedir += DateTime.Now.Month.ToString() + "/";
                if (!Directory.Exists(_iHostingEnvironment.WebRootPath + "/" + upload_basedir))
                {
                    Directory.CreateDirectory(_iHostingEnvironment.WebRootPath + "/" + upload_basedir);
                }
                string upload_directory = _iHostingEnvironment.WebRootPath + "/" + upload_basedir;

                string file_id_card_name = null;
                string file_id_card_path = DataVendor.file_id_card;
                if (model.file_id_card != null && model.file_id_card.Length != 0)
                {
                    file_id_card_name = "idcard-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_id_card = Path.Combine(upload_directory, file_id_card_name);
                    using (var stream = System.IO.File.Create(path_file_id_card))
                    {
                        await model.file_id_card.CopyToAsync(stream);
                        file_id_card_path = upload_basedir + file_id_card_name;
                    }
                }

                string file_npwp_name = null;
                string file_npwp_path = DataVendor.file_npwp;
                if (model.file_npwp != null && model.file_npwp.Length != 0)
                {
                    file_npwp_name = "npwp-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_npwp = Path.Combine(upload_directory, file_npwp_name);
                    using (var stream = System.IO.File.Create(path_file_npwp))
                    {
                        await model.file_npwp.CopyToAsync(stream);
                        file_npwp_path = upload_basedir + file_npwp_name;
                    }
                }

                string file_sppkp_name = null;
                string file_sppkp_path = DataVendor.file_sppkp;
                if (model.file_sppkp != null && model.file_sppkp.Length != 0)
                {
                    file_sppkp_name = "sppkp-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_sppkp = Path.Combine(upload_directory, file_sppkp_name);
                    using (var stream = System.IO.File.Create(path_file_sppkp))
                    {
                        await model.file_sppkp.CopyToAsync(stream);
                        file_sppkp_path = upload_basedir + file_sppkp_name;
                    }
                }

                string file_vendor_statement_name = null;
                string file_vendor_statement_path = DataVendor.file_vendor_statement;
                if (model.file_vendor_statement != null && model.file_vendor_statement.Length != 0)
                {
                    file_vendor_statement_name = "vendor_statement-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_vendor_statement = Path.Combine(upload_directory, file_vendor_statement_name);
                    using (var stream = System.IO.File.Create(path_file_vendor_statement))
                    {
                        await model.file_vendor_statement.CopyToAsync(stream);
                        file_vendor_statement_path = upload_basedir + file_vendor_statement_name;
                    }
                }
                
                bool updateTempVendor = false;

                // check vendor_id
                if(model.vendor_id == 0) {
                    model.type = "Vendor Data Change";

                     // updated data logic
                    var customObject = new {file_id_card_path = file_id_card_path,file_npwp_path = file_npwp_path, file_sppkp_path = file_sppkp_path, file_vendor_statement_path = file_vendor_statement_path};
                    VendorModel dataUpdateMapping = _vendor.mappingRegisToVendor(DataVendor, model, customObject);
                    
                    List<String> diffField = _vendor.getListUpdatedColumn(DataVendor, dataUpdateMapping);
                    String StringJsonDiffField = JsonSerializer.Serialize(diffField);

                    model.updated_data = StringJsonDiffField;
                    Console.WriteLine("Data Diff: " + JsonSerializer.Serialize(diffField));
                    // end of updated data logic

                    int vendor_id = _registration.InsertRegistration(model, file_id_card_path, file_npwp_path, file_sppkp_path, file_vendor_statement_path, token);
                    if ( vendor_id > 0 )
                    {
                        bool vendNum = _vendor.updateVendorNumber(model.vendor_number, vendor_id);
                        _vendor.InsertTempCompany(model, vendor_id);
                        TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
                        {
                            vendor_id = vendor_id,
                            verification_status_id = verification_status_id,
                            verification_note = null,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        _registration.InsertLog(data_insert);

                        //update is_extension
                        var listCompanyOld = _vendor.GetCompanyRelation(model.vendor_number);  
                        var DataLast = _vendor.GetLastDataChange(model.vendor_number);
                        var listCompanyNew = DataLast.company;
                        int is_extension = 0;
                        Console.WriteLine(listCompanyNew.Count()+""+listCompanyOld.Count());

                        if (listCompanyNew.Count() > listCompanyOld.Count()){
                        is_add_new_company = 1;
                        }
                        if (diffField.Count() > 0){
                            is_updated_orther_data = 1;
                        }
                        if(is_add_new_company == 1 && is_updated_orther_data== 0){
                            is_extension=1;
                        }
                        if(is_add_new_company == 0 && is_updated_orther_data== 1){
                            is_extension=2;
                        }
                        if(is_add_new_company == 1 && is_updated_orther_data== 1){
                            is_extension=3;
                        }

                        _vendor.updateIsExtension(vendor_id, is_extension);
                        result = true;
                    }
                    else
                    {
                        error_message = _localizer.GetString("FailedToSave");
                    }
                } else {
                  // updated data logic
                    var customObject = new {file_id_card_path = file_id_card_path,file_npwp_path = file_npwp_path, file_sppkp_path = file_sppkp_path, file_vendor_statement_path = file_vendor_statement_path};
                    var listCompanyOld = _vendor.GetCompanyRelation(model.vendor_number);  
                    var DataLast = _vendor.GetLastDataChange(model.vendor_number);
                    var listCompanyNew = DataLast.company;
                    int is_extension = 0;
                    VendorModel dataUpdateMapping = _vendor.mappingRegisToVendor(DataVendor, model, customObject);
                    
                    List<String> diffField = _vendor.getListUpdatedColumn(DataVendor, dataUpdateMapping);
                    String StringJsonDiffField = JsonSerializer.Serialize(diffField);

                    if (listCompanyNew.Count() > listCompanyOld.Count()){
                        is_add_new_company = 1;
                    }
                    if (diffField.Count() > 0){
                        is_updated_orther_data = 1;
                    }
                    if(is_add_new_company == 1 && is_updated_orther_data== 0){
                        is_extension=1;
                    }
                    if(is_add_new_company == 0 && is_updated_orther_data== 1){
                        is_extension=2;
                    }
                    if(is_add_new_company == 1 && is_updated_orther_data== 1){
                        is_extension=3;
                    }


                    model.updated_data = StringJsonDiffField;
                    Console.WriteLine("Data Diff: " + JsonSerializer.Serialize(diffField));
                    // end of updated data logic
                    model.token = tempVendor.token;
                    updateTempVendor = _vendor.updateTempVendor(model.vendor_id, model, file_id_card_path, file_npwp_path, file_sppkp_path, file_vendor_statement_path);
                    if (updateTempVendor)
                    {
                        //Delete Old File
                        if (!string.IsNullOrEmpty(file_id_card_name) && !string.IsNullOrEmpty(DataVendor.file_id_card) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_id_card))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_id_card);
                        }
                        if (!string.IsNullOrEmpty(file_npwp_name) && !string.IsNullOrEmpty(DataVendor.file_npwp) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_npwp))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_npwp);
                        }
                        if (!string.IsNullOrEmpty(file_sppkp_name) && !string.IsNullOrEmpty(DataVendor.file_sppkp) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_sppkp))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_sppkp);
                        }
                        if (!string.IsNullOrEmpty(file_vendor_statement_name) && !string.IsNullOrEmpty(DataVendor.file_vendor_statement) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_vendor_statement))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_vendor_statement);
                        }
                        //End delete old file

                         //update is_extension
                        _vendor.updateIsExtension(model.vendor_id, is_extension);
                        //Delete Old Company
                        _vendor.deleteTempCompanyByVendorId(model.vendor_id);
                        //Insert input company
                        _vendor.InsertTempCompany(model, model.vendor_id);
                        result = true;
                    }
                    else
                    {
                        error_message = _localizer.GetString("FailedToSave");
                    }
                }
                
                if(action == "Send") 
                {
                    bool saveUpdate = _vendor.updateIsLockedAndChangeRequestStatus(model.vendor_number, is_locked, change_request_status, DataVendor.change_request_note);
                    if(saveUpdate) {
                        //Insert Log
                        LogVendorModel data_insert = new LogVendorModel()
                        {
                            vendor_number = model.vendor_number,
                            process = "Change Vendor",
                            note = null,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        _vendor.InsertLogVendor(data_insert);
                        result = true;
                    }
                }
            }
            

            
            // Console.WriteLine("Diff Field : " + JsonSerializer.Serialize(diffField));

            if (result && action == "Send")
            {
                
                List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/revision_success-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                var pic = _master.GetProcurementGroupByID(model.pic_id);
                registered_email.Add(pic.email);
                Console.WriteLine(JsonSerializer.Serialize(registered_email));
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("vendor_name", model.name);
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    
                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Berhasil" : "Revision Success"), email_content);
                }

                return RedirectToAction("Index", "ChangeVendor");
            }
            else
            {
                if(action == "Draft") {
                    return RedirectToAction("Index", "ChangeVendor");
                } else {
                    ViewBag.Menu = _list_menu;
                    ViewBag.error_message = error_message;
                    ViewBag.vendor_data = model;
                    ViewBag.controller = "ChangeVendor";
                    ViewBag.BaseUrl = _appSettings.BaseUrl;
                    ViewBag.language = _current_language;
                    ViewData["name"] = HttpContext.Session.GetString("name");
                    return View("Form");
                }
            }
        }
    }
}