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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace tufol.Controllers
{
    public class VendorManagementController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private IVendor _vendor;
        private IRegistration _registration;
        private IMaster _master;
        private ISetting _setting;
        private String _current_language;
        private String _role_id;
        private readonly Helpers.AppSettings _appSettings;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly PermissionModel _permission;
        private readonly IStringLocalizer<VendorManagementController> _localizer;
        private IWebHostEnvironment _iHostingEnvironment;

        // public DashboardController(IMenu menu, IUser user) : base(user, menu)
        public VendorManagementController(IMenu menu, IUser user, IVendor vendor, IRegistration registration, IMaster master, ISetting setting, IOptions<Helpers.AppSettings> appSettings, IStringLocalizer<VendorManagementController> localizer, IWebHostEnvironment iHostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _menu = menu;
            _user = user;
            _vendor = vendor;
            _registration = registration;
            _master = master;
            _setting = setting;
            _appSettings = appSettings.Value;
            _iHostingEnvironment = iHostingEnvironment;
            _localizer = localizer;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            _role_id = httpContextAccessor.HttpContext.Session.GetString("role_id");
            _list_menu = _menu.GetMenu(_role_id);
            _permission = _menu.GetPermission("VendorManagement", _role_id);
            
        }
        public IActionResult Index()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = _role_id;
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            return View();
        }
        [Route("/{culture}/{controller}/PendingVendor/{vendor_id}")]
        public IActionResult DetilPendingVendor(int vendor_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataVendor = _vendor.GetPendingVendorDetail(vendor_id);
            ViewBag.vendor_data = DataVendor;
            dynamic updated_data = !String.IsNullOrEmpty(DataVendor.updated_data) ? (JArray) JsonConvert.DeserializeObject(DataVendor.updated_data) : new List<String>(){};
            ViewBag.updated_data = updated_data;
            ViewBag.dict_field = _vendor.dictionaryField();
            Console.WriteLine("UPDATED DATA 000: " + System.Text.Json.JsonSerializer.Serialize(DataVendor.updated_data));
            Console.WriteLine("UPDATED DATA: " + System.Text.Json.JsonSerializer.Serialize(updated_data));
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.actionName = "PendingVendor";
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            ViewBag.error_message = "";
            ViewBag.language = _current_language;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewBag.role_id = _role_id;
            ViewBag.Menu = _list_menu;
            ViewBag.Permission = _permission;
            if(_role_id == "1")
                return View("Detil");
            else
                return View("Form");
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Approve(RegistrationModel model) {
            if (!_permission.allow_approve)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataVendor = _vendor.GetPendingVendorDetail(model.vendor_id);
            string role_user = _role_id;
            String encrypted_vendor_id = new Helpers.GlobalFunction().Base64Encode(model.vendor_id.ToString());
            

            string token = Guid.NewGuid().ToString();
            int verification_status_id = 4;
            int position_data = 5;

            dynamic updated_data = !String.IsNullOrEmpty(DataVendor.updated_data) ? JsonConvert.DeserializeObject(DataVendor.updated_data) : new List<String>(){};
            ViewBag.updated_data = updated_data;
            ViewBag.dict_field = _vendor.dictionaryField();

            if (role_user == "3")
            {
                verification_status_id = 4;
                position_data = 5;
            }
            else if (role_user == "5")
            {
                verification_status_id = 5;
                position_data = 9;
            }
            else
            {
                return RedirectToAction("Index", "VendorManagement");
            }

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;

            String error_message = "";
                 var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();
            
            for (var j = 0; j < errors.Length ; j++) {
                Console.WriteLine("error: "+errors[j]);
            }
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
                string file_id_card_path = null;
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
                string file_npwp_path = null;
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
                string file_sppkp_path = null;
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
                string file_vendor_statement_path = null;
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

                bool saveUpdate = _vendor.updateTempVendor(model.vendor_id, model, file_id_card_name, file_npwp_name, file_sppkp_name, file_vendor_statement_name);

                if (saveUpdate)
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

                    //Delete Old Company
                    _vendor.deleteTempCompanyByVendorId(model.vendor_id);
                    //Insert input company
                    _vendor.InsertTempCompany(model, model.vendor_id);
                    //Insert Log
                    TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
                    {
                        vendor_id = model.vendor_id,
                        verification_status_id = verification_status_id,
                        verification_note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    _vendor.InsertLogTempVendor(data_insert);
                    result = true;
                }
                else
                {
                    error_message = _localizer.GetString("FailedToSave");
                    result = false;
                }
            }else{
                error_message = _localizer.GetString("FailedToSave");
                result = false;
            }

            if (role_user == "3" && result)
            {
                //FC
                var pic = _master.GetProcurementGroupByID(model.pic_id);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_approval_notification-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() {
                    pic.email
                };

                string email_fc = _setting.GetSettingValue("email_fc_vendor");
                if (!string.IsNullOrEmpty(email_fc))
                {
                    List<string> fc_email_list = new Helpers.EmailFunction().GetEmailList(email_fc, ";");
                    fc_email_list = fc_email_list == null ? new List<string>() : fc_email_list;
                    foreach (string fc_recipient in fc_email_list)
                    {
                        recipient_email.Add(fc_recipient);
                    }
                }
                foreach (string recipient in recipient_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    
                    if ( DataVendor.type == "Vendor Data Change" ) {
                        email_content_param.Add("message", (_current_language == "id" ? "Perubahan data vendor " + DataVendor.name + " disetujui oleh Procurement" : "Changes to " + DataVendor.name + " data approved by Procurement"));
                        email_content_param.Add("title", (_current_language == "id" ? "Perubahan Data Vendor" : "Vendor Change"));
                    }
                    else
                    {
                        email_content_param.Add("message", (_current_language == "id" ? "Pendaftaran vendor " + DataVendor.name + " disetujui oleh Procurement" : DataVendor.name + " registration approved by Procurement"));
                        email_content_param.Add("title", (_current_language == "id" ? "Pendaftaran Vendor" : "Vendor Registration"));
                    }

                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Persetujuan Data Vendor" : "Vendor Data Approval"), email_content);
                }
            }
            else if (role_user == "5" && result)
            {
                //FC
                var pic = _master.GetProcurementGroupByID(model.pic_id);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_approval_notification-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() {
                    pic.email
                };

                
                foreach (string recipient in recipient_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();

                    if (DataVendor.type == "Vendor Data Change")
                    {
                        email_content_param.Add("message", (_current_language == "id" ? "Perubahan data vendor " + DataVendor.name + " disetujui oleh FC" : "Changes to " + DataVendor.name + " data approved by FC"));
                        email_content_param.Add("title", (_current_language == "id" ? "Perubahan Data Vendor" : "Vendor Change"));
                    }
                    else
                    {
                        email_content_param.Add("message", (_current_language == "id" ? "Pendaftaran vendor " + DataVendor.name + " disetujui oleh FC" : DataVendor.name + " registration approved by FC"));
                        email_content_param.Add("title", (_current_language == "id" ? "Pendaftaran Vendor" : "Vendor Registration"));
                    }

                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Persetujuan data Vendor" : "Vendor Data Approval"), email_content);
                }

            }
            if (result)
            {
                return RedirectToAction("Index", "VendorManagement");
            }
            else
            {
                ViewBag.error_message = error_message;
                ViewBag.vendor_data = model;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.actionName = "";
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = _role_id;
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                ViewBag.vendor_data = model;
                Console.WriteLine("error"+ error_message);
                // return RedirectToAction("Detil", "VendorManagement", new { id = DataVendor.vendor_id, error_message = error_message });
                return View("Form");
            }
        }

        [HttpPost, ActionName("Revise")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Revise(RegistrationModel model) {
            if (!_permission.allow_revise)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataVendor = _vendor.GetPendingVendorDetail(model.vendor_id);
            string role_user = _role_id;
            String encrypted_vendor_id = new Helpers.GlobalFunction().Base64Encode(model.vendor_id.ToString());

            string token = Guid.NewGuid().ToString();
            int verification_status_id = 3;
            int position_data = 2;

            dynamic updated_data = !String.IsNullOrEmpty(DataVendor.updated_data) ? JsonConvert.DeserializeObject(DataVendor.updated_data) : new List<String>(){};
            ViewBag.updated_data = updated_data;
            ViewBag.dict_field = _vendor.dictionaryField();

            if (role_user == "3")
            { 
                verification_status_id = 3;
                position_data = 2;
                model.token = token;
            }
            else if (role_user == "5")
            {
                verification_status_id = 2;
                position_data = 3;
            } else
            {
                return RedirectToAction("Index", "VendorManagement");
            }

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;

            String error_message = "";
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
                string file_id_card_path = null;
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
                string file_npwp_path = null;
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
                string file_sppkp_path = null;
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
                string file_vendor_statement_path = null;
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

                bool saveUpdate = _vendor.updateTempVendor(model.vendor_id, model, file_id_card_name, file_npwp_name, file_sppkp_name, file_vendor_statement_name);

                if ( saveUpdate )
                {
                    //Delete Old File
                    if ( !string.IsNullOrEmpty(file_id_card_name) && !string.IsNullOrEmpty(DataVendor.file_id_card) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataVendor.file_id_card) )
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

                    //Delete Old Company
                    _vendor.deleteTempCompanyByVendorId(model.vendor_id);
                    //Insert input company
                    _vendor.InsertTempCompany(model, model.vendor_id);
                    //Insert Log
                    TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
                    {
                        vendor_id = model.vendor_id,
                        verification_status_id = verification_status_id,
                        verification_note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    _vendor.InsertLogTempVendor(data_insert);
                    result = true;
                }
                else
                {
                    error_message = _localizer.GetString("FailedToSave");
                }
            }

            if (role_user == "3" && result)
            {
                //Procurement
                List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;
                Console.WriteLine(_iHostingEnvironment.ContentRootPath);
                Console.WriteLine(_current_language);
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_revision-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("vendor_name", model.name);
                    email_content_param.Add("Note", model.verification_note);
                    email_content_param.Add("revision_link", _appSettings.BaseUrl + _current_language + "/Home/RevisionForm?vid="+ encrypted_vendor_id+"&token="+token);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Data Vendor" : "Vendor Data Revision"), email_content);
                }
            }
            else if (role_user == "5" && result)
            {
                //FC
                var pic = _master.GetProcurementGroupByID(model.pic_id);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_revision-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() { 
                    pic.email
                };

                string email_fc = _setting.GetSettingValue("email_fc_vendor");
                if (!string.IsNullOrEmpty(email_fc))
                {
                    List<string> fc_email_list = new Helpers.EmailFunction().GetEmailList(email_fc, ";");
                    fc_email_list = fc_email_list == null ? new List<string>() : fc_email_list;
                    foreach (string fc_recipient in fc_email_list)
                    {
                        recipient_email.Add(fc_recipient);
                    }
                }
                foreach (string recipient in recipient_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("vendor_name", model.name);
                    email_content_param.Add("note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("revision_link", _appSettings.BaseUrl + _current_language + "/VendorManagement/PendingVendor/" + model.vendor_id);

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Data Vendor" : "Vendor Data Revision"), email_content);
                }

            }
            if ( result )
            {
                return RedirectToAction("Index", "VendorManagement");
            } else
            {
                ViewBag.error_message = error_message;
                ViewBag.vendor_data = model;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.actionName = "";
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = _role_id;
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                return View("Form");
            }
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Reject(RegistrationModel model)
        {
            if (!_permission.allow_reject)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataVendor = _vendor.GetPendingVendorDetail(model.vendor_id);
            string role_user = _role_id;
            String encrypted_vendor_id = new Helpers.GlobalFunction().Base64Encode(model.vendor_id.ToString());

            string token = Guid.NewGuid().ToString();
            int verification_status_id = 7;
            int position_data = 2;

            dynamic updated_data = !String.IsNullOrEmpty(DataVendor.updated_data) ? JsonConvert.DeserializeObject<List<String>>(DataVendor.updated_data).ToList() : new List<String>(){};
            ViewBag.updated_data = updated_data;
            ViewBag.dict_field = _vendor.dictionaryField();
            
            if (role_user == "3")
            {
                verification_status_id = 7;
                position_data = 2;
            }
            else if (role_user == "5")
            {
                verification_status_id = 8;
                position_data = 2;
            }
            else
            {
                return RedirectToAction("Index", "VendorManagement");
            }

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;

            String error_message = "";
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
                string file_id_card_path = null;
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
                string file_npwp_path = null;
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
                string file_sppkp_path = null;
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
                string file_vendor_statement_path = null;
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

                bool saveUpdate = _vendor.updateTempVendor(model.vendor_id, model, file_id_card_name, file_npwp_name, file_sppkp_name, file_vendor_statement_name);

                if (saveUpdate)
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

                    //Delete Old Company
                    _vendor.deleteTempCompanyByVendorId(model.vendor_id);
                    //Insert input company
                    _vendor.InsertTempCompany(model, model.vendor_id);
                    //Insert Log
                    TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
                    {
                        vendor_id = model.vendor_id,
                        verification_status_id = verification_status_id,
                        verification_note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    _vendor.InsertLogTempVendor(data_insert);
                    LogVendorModel data = new LogVendorModel()
                        {
                            vendor_number = model.vendor_number,
                            process = "Change Vendor Rejected",
                            note = model.verification_note,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                    _vendor.DeleteTempVendor(DataVendor.vendor_id);
                    if (DataVendor.type == "Vendor Data Change" && !string.IsNullOrEmpty(DataVendor.vendor_number))
                    {
                        _vendor.InsertLogVendor(data);
                    }
                    result = true;

                }
                else
                {
                    error_message = _localizer.GetString("FailedToSave");
                }
            }

            if (role_user == "3" && result)
            {
                //Procurement
                List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;
                
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_rejection-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("vendor_name", model.name);
                    email_content_param.Add("Note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("rejected_by", "Procurement");

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Pendaftaran Vendor Ditolak" : "Vendor Registration Rejected"), email_content);
                }
            }
            else if (role_user == "5" && result)
            {
                //FC
                var pic = _master.GetProcurementGroupByID(model.pic_id);
                List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_rejection-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() {
                    pic.email
                };

                string email_fc = _setting.GetSettingValue("email_fc_vendor");
                if (!string.IsNullOrEmpty(email_fc))
                {
                    List<string> fc_email_list = new Helpers.EmailFunction().GetEmailList(email_fc, ";");
                    fc_email_list = fc_email_list == null ? new List<string>() : fc_email_list;
                    foreach (string fc_recipient in fc_email_list)
                    {
                        recipient_email.Add(fc_recipient);
                    }
                }
                foreach (string vendor_email in registered_email)
                {
                    recipient_email.Add(vendor_email);
                }


                foreach (string recipient in recipient_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("vendor_name", model.name);
                    email_content_param.Add("Note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("rejected_by", "FC");

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Pendaftaran Vendor Ditolak" : "Vendor Registration Rejected"), email_content);
                }

            }
            if (result)
            {
                return RedirectToAction("Index", "VendorManagement");
            }
            else
            {
                ViewBag.error_message = error_message;
                ViewBag.vendor_data = model;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.actionName = "";
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = _role_id;
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                ViewBag.vendor_data = model;
                return View("Form");
            }
        }

        [HttpGet, ActionName("active_vendor_datatable")]
        public IActionResult LoadActiveData(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null, string vendor_number = null, string name = null, int vendor_type_id = 0)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                var data = _vendor.GetVendorListDatatable(pagination, sort, query, vendor_number, name, vendor_type_id);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpGet, ActionName("data_change_request_datatable")]
        public IActionResult LoadChangeRequestData(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null, string vendor_number = null, string name = null, int vendor_type_id = 0)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            ViewBag.VendorNumber = HttpContext.Session.GetString("vendor_number");

            Console.WriteLine("vendor number"+ ViewBag.VendorNumber); 
            try
            {   
                var data = _vendor.GetVendorListDatatable(pagination, sort, query, vendor_number, name, vendor_type_id);
                Console.WriteLine("vendor number :"+ data.ToArray());              
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpGet, ActionName("pending_vendor_datatable")]
        public IActionResult LoadPendingData(string type, Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null, string vendor_number = null, string name = null, int vendor_type_id = 0)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                var data = _vendor.GetPendingVendorDatatable(role_id, type, pagination, sort, query, vendor_number, name, vendor_type_id);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }
        [Route("{culture}/{controllers}/edit/{vendor_id}")]
        public IActionResult Edit(int vendor_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataVendor = _vendor.GetPendingVendorDetail(vendor_id);

            dynamic updated_data = !String.IsNullOrEmpty(DataVendor.updated_data) ? JsonConvert.DeserializeObject(DataVendor.updated_data) : new List<String>(){};
            ViewBag.updated_data = updated_data;
            ViewBag.dict_field = _vendor.dictionaryField();

            ViewBag.vendor_data = DataVendor;
            ViewBag.language = _current_language;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.actionName = "";
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = _role_id;
            ViewBag.Menu = _list_menu;
            return View("Form");
        }
        [Route("{culture}/{controllers}/ActiveVendor/{vendor_number}")]
        public IActionResult ActiveVendor(string vendor_number)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataVendor = _vendor.GetVendorDetail(vendor_number);
            // ViewBag.log_vendor = _vendor.GetLogVendor(vendor_number);

            ViewBag.vendor_data = DataVendor;
            var LastDataChange = _vendor.GetLastDataChange(vendor_number);
            ViewBag.last_data_change = LastDataChange;
            Console.WriteLine(DataVendor.is_locked + " ini yaa");
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.actionName = "";
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewBag.role_id = _role_id;
            ViewBag.Menu = _list_menu;
            ViewBag.Culture = _current_language;
            return View("Detil");
        }

        [HttpPost, ActionName("RequestData")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> RequestData(VendorModel model) {
            if (!_permission.allow_approve)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataVendor = _vendor.GetVendorDetail(model.vendor_number);
            var tempVendor = _vendor.GetLastDataChange(model.vendor_number);
            string role_user = _role_id;

            int is_locked = 0;
            int change_request_status = 1;
            string change_request_note = model.change_request_note;
            int verification_status_id = 2;
            if (role_user == "3")
            {
                verification_status_id = 3;
            }
            else if (role_user == "5")
            {
                verification_status_id = 2;
            }
            else
            {
                return RedirectToAction("Index", "VendorManagement");
            }

            String error_message = "";
            bool saveUpdate = _vendor.updateIsLockedAndChangeRequestStatus(model.vendor_number, is_locked, change_request_status, change_request_note);
            if(tempVendor.vendor_id > 0) {
                bool updateTempVendor = _vendor.updateVendorVerificationStatus(tempVendor.vendor_id, verification_status_id, 2, model.change_request_note, tempVendor.token);
                TempLogVendorStatusModel log_insert = new TempLogVendorStatusModel()
                {
                    vendor_id = tempVendor.vendor_id,
                    verification_status_id = 1,
                    verification_note = null,
                    created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                };
                _vendor.InsertLogTempVendor(log_insert);
            }

            //Insert Log
            LogVendorModel data_insert = new LogVendorModel()
            {
                vendor_number = model.vendor_number,
                process = "Revision Request",
                note = change_request_note,
                created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
            };
            _vendor.InsertLogVendor(data_insert);
            result = true;

            if (role_user == "3" && result)
            {
                List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;
                Console.WriteLine(_iHostingEnvironment.ContentRootPath);
                Console.WriteLine(_current_language);
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_revision-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("vendor_name", DataVendor.name);
                    email_content_param.Add("note", change_request_note);
                    email_content_param.Add("revision_link", _appSettings.BaseUrl + _current_language + "/ChangeVendor/");
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Data Vendor" : "Vendor Data Revision"), email_content);
                }
            }
            else if (role_user == "5" && result)
            {
                var pic = _master.GetProcurementGroupByID(DataVendor.pic_id);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_revision-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() { 
                    pic.email
                };

                string email_fc = _setting.GetSettingValue("email_fc_vendor");
                if (!string.IsNullOrEmpty(email_fc))
                {
                    List<string> fc_email_list = new Helpers.EmailFunction().GetEmailList(email_fc, ";");
                    fc_email_list = fc_email_list == null ? new List<string>() : fc_email_list;
                    foreach (string fc_recipient in fc_email_list)
                    {
                        recipient_email.Add(fc_recipient);
                    }
                }
                foreach (string recipient in recipient_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("vendor_name", DataVendor.name);
                    email_content_param.Add("note", change_request_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("revision_link", _appSettings.BaseUrl + _current_language + "/ChangeVendor/");

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Data Vendor" : "Vendor Data Revision"), email_content);
                }
            }
            if (result)
            {
                return RedirectToAction("Index", "VendorManagement");
            }
            else
            {
                ViewBag.error_message = error_message;
                ViewBag.vendor_data = DataVendor;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.actionName = "";
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = _role_id;
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                return View("Form");
            }
        }

        [Route("{culture}/{controllers}/DataChangeRequest/{vendor_number}")]
        public IActionResult DataChangeRequest(string vendor_number)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataVendor = _vendor.GetVendorDetail(vendor_number);
            ViewBag.vendor_data = DataVendor;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.actionName = "";
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = _role_id;
            ViewBag.Menu = _list_menu;
            return View("Form");
        }
    }
}