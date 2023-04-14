using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Localization; 
using tufol.Models;
using tufol.Interfaces;
using System.Threading;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Options;

namespace tufol.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PreventDuplicateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Session.GetString("LastProcessedToken");

            if (context.HttpContext.Request.HasFormContentType && context.HttpContext.Request.Form.ContainsKey("__RequestVerificationToken"))
            {
                var currentToken = context.HttpContext.Request.Form["__RequestVerificationToken"].ToString();
                var lastToken = context.HttpContext.Session.GetString("LastProcessedToken");

                if (lastToken == currentToken)
                {
                    context.ModelState.AddModelError(string.Empty, "Looks like you accidentally submitted the same form twice.");
                }
                else
                {
                    context.HttpContext.Session.SetString("LastProcessedToken", currentToken);
                }
            }
        }
    }
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IUser _user;
        private IRegistration _registration;
        private IVendor _vendor;
        private IMaster _master;
        private readonly IStringLocalizer<HomeController> _localizer;
        private IWebHostEnvironment _iHostingEnvironment;
        private String _current_language;
        private readonly Helpers.AppSettings _appSettings;
        private readonly Helpers.DefaultValue _defaultValue;
        private readonly IEnumerable<MenuModel> _list_menu;
        private string _role_id;

        public HomeController(IUser user, IRegistration registration, IMaster master, IStringLocalizer<HomeController> localizer, IWebHostEnvironment iHostingEnvironment, IOptions<Helpers.AppSettings> appSettings, IOptions<Helpers.DefaultValue> defaultValue, IVendor vendor, IMenu menu, IHttpContextAccessor httpContextAccessor)
        {
            // this._httpContextAccessor = httpContextAccessor;
            _user = user;
            _registration = registration;
            _master = master;
            _localizer = localizer;
            _iHostingEnvironment = iHostingEnvironment;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            _appSettings = appSettings.Value;
            _defaultValue = defaultValue.Value;
            _vendor = vendor;
            ViewBag.BaseUrl = "";
            _role_id = httpContextAccessor.HttpContext.Session.GetString("role_id");
            _list_menu = String.IsNullOrEmpty( _role_id) ? new List<MenuModel>() : menu.GetMenu(_role_id);

        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        // 
        [HttpGet, ActionName("index")]
        public IActionResult IndexAsync()
        {
            ViewBag.Menu = _list_menu;
            var landing_page = _master.GetAnnouncementDetail("landing_page");
            var pop_up = _master.GetAnnouncementDetail("pop_up");
            ViewBag.landing_page = landing_page;
            ViewBag.pop_up = pop_up;
            ViewBag.role_id = _role_id;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View();
        }

        [HttpPost, ActionName("index")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Index(string username, string password)
        {
             var passwordEnc = GetMD5(password);
            if(username != "" && passwordEnc != "")
            {
                UserModel createdUser = new UserModel();
                var checkUsername = _user.CheckUsername(username);
                var login = _user.Login(username, passwordEnc);
                if(login.username == null && checkUsername) 
                {
                    ViewBag.error = _localizer.GetString("InvalidPassword");
                    return View();
                }
                else if (login.username == null && !checkUsername)
                {
                    ViewBag.error = _localizer.GetString("InvalidUser");
                    return View("index");
                }
                _user.updateLastLogin(username, passwordEnc);
                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetString("role_id", login.role_id.ToString());
                HttpContext.Session.SetString("user_id", login.user_id.ToString());
                HttpContext.Session.SetString("vendor_number", login.vendor_number.ToString());
                return RedirectToAction("index", "dashboard");
            } 
            else 
            {
                ViewBag.error = _localizer.GetString("InvalidUser");
                return View();
            }
        }

        public IActionResult RegistrationForm()
        {
            RegistrationModel vendor_data = new RegistrationModel();
            vendor_data.vendor_type_id = Convert.ToInt32(_defaultValue.vendor_type_id);
            vendor_data.sg_category_id = Convert.ToInt32(_defaultValue.sg_category_id);
            vendor_data.account_group_id = _defaultValue.account_group_id;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.error_message = "";
            ViewBag.vendor_data = vendor_data;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            ViewBag.language = _current_language;
            ViewBag.Menu = _list_menu;
            return View("RegistrationForm");
        }

        [HttpPost , ActionName("RegistrationForm")] 
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> RegistrationForm(RegistrationModel model) 
        { 
            String error_message = "";
            model.type = "New Registration";
            model.updated_data = null;
            if (ModelState.IsValid) {
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
                if ( model.file_id_card != null && model.file_id_card.Length != 0 )
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
                
                String token = Guid.NewGuid().ToString();
                int vendor_id = _registration.InsertRegistration(model, file_id_card_path, file_npwp_path, file_sppkp_path, file_vendor_statement_path, token);
                if ( vendor_id > 0 )
                {
                    _registration.InsertCompany(model, vendor_id);
                    //Insert Log
                    TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
                    {
                        vendor_id = vendor_id,
                        verification_status_id = 1,
                        verification_note = null,
                        created_by = null
                    };
                    _registration.InsertLog(data_insert);

                    String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/verify_email-" + _current_language + ".html";
                    String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                    List<String> email_list = new Helpers.EmailFunction().GetEmailList(model.email);
                    var pic = _master.GetProcurementGroupByID(model.pic_id);
                    email_list.Add(pic.email);
                    foreach (string email in email_list)
                    {
                        Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                        email_content_param.Add("email", email);
                        email_content_param.Add("year", DateTime.Now.Year.ToString());
                        String encrypted_vendor_id = new Helpers.GlobalFunction().Base64Encode(vendor_id.ToString());
                        email_content_param.Add("verification_link", _appSettings.BaseUrl + _current_language + "/Home/VerifyEmail?vendor=" + encrypted_vendor_id + "&token=" + token + "&email=" + email);

                        String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                        
                        new Helpers.EmailFunction().sending_email(email, (_current_language == "id" ? "Verifikasi Email" : "Verify Email"), email_content);
                    }
                    return RedirectToAction("RegistrationSuccess", "Home");
                } else
                {
                    error_message = _localizer.GetString("FailedToSave");
                }
            }
            model.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(model.vendor_type_id, model.country_id);
            ViewBag.error_message = error_message;
            ViewBag.vendor_data = model;
            ViewBag.Menu = _list_menu;
            return View(model);
        }

        [HttpGet, ActionName("RegistrationSuccess")]
        public IActionResult RegistrationSuccess(string vid)
        {
            string decode_vid = new Helpers.GlobalFunction().Base64Decode(vid);
            int vendor_id = Convert.ToInt32(decode_vid);
            var vendor_data = _vendor.GetPendingVendorDetail(vendor_id);
            ViewBag.vendor_data = vendor_data;
            ViewBag.alert = "RegistrationSuccess";
            ViewBag.Menu = _list_menu;
            return View("Alert");
        }

        [HttpGet, ActionName("RevisionSuccess")]
        public IActionResult RevisionSuccess(string vid)
        {
            string decode_vid = new Helpers.GlobalFunction().Base64Decode(vid);
            int vendor_id = Convert.ToInt32(decode_vid);
            var vendor_data = _vendor.GetPendingVendorDetail(vendor_id);
            ViewBag.vendor_data = vendor_data;
            ViewBag.Menu = _list_menu;
            ViewBag.alert = "RevisionSuccess";
            return View("Alert");
        }

        public IActionResult RevisionForm(string vid, string token)
        {
            string decoded_vendor = new Helpers.GlobalFunction().Base64Decode(vid);
            int vendor_id = Convert.ToInt32(decoded_vendor);
            var vendor_data = _vendor.GetPendingVendorDetail(vendor_id, token);
            vendor_data.vendor_type_id = Convert.ToInt32(_defaultValue.vendor_type_id);
            vendor_data.sg_category_id = Convert.ToInt32(_defaultValue.sg_category_id);
            vendor_data.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(vendor_data.vendor_type_id, vendor_data.country_id);
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.vendor_data = vendor_data;
            ViewBag.controller = "Revision";
            return View("RegistrationForm");
        }

        [HttpPost, ActionName("RevisionForm")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> RevisionForm(RegistrationModel model)
        {
            bool result = false;
            var DataVendor = _vendor.GetPendingVendorDetail(model.vendor_id);
            string role_user = HttpContext.Session.GetString("role_id");
            int verification_status_id = 16;
            int position_data = 3;

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;
            model.token = null;
            model.type = "New Registration";

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
                        verification_note = null,
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

            if (result)
            {
                List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/revision_success-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("vendor_name", model.name);
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Berhasil" : "Revision Success"), email_content);
                }
                return RedirectToAction("RevisionSuccess", "Home");
            }
            else
            {
                ViewBag.error_message = error_message;
                ViewBag.vendor_data = model;
                ViewBag.controller = "Revision";
                ViewBag.Menu = _list_menu;
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = HttpContext.Session.GetString("role_id");
                return View("Form");
            }

        }

        public async Task<IActionResult> VerifyEmailAsync(string vendor, string token, string email)
        {
            string decoded_vendor = new Helpers.GlobalFunction().Base64Decode(vendor);
            int vendor_id = Convert.ToInt32(decoded_vendor);
            string error_message = "";
            bool verification_status = true;
            string error_code = "";
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                verification_status = false;
                ViewBag.alert = "InvalidVerifUrl";
                error_code = "400"; //Error invalid verification url
            }

            TempVendorModel data_vendor = new TempVendorModel();
            if ( verification_status ) { 
                data_vendor = _vendor.GetPendingVendorDetail(vendor_id, token);
                if ( string.IsNullOrEmpty(data_vendor.vendor_id.ToString()) )
                {
                    verification_status = false;
                    ViewBag.alert = "InvalidVerifUrl";
                    error_code = "400"; //Error invalid verification url
                }
            }

            
            if ( verification_status )
            {
                List<String> registered_email = data_vendor.email_list == null ? new List<String>() : data_vendor.email_list;
                List<String> verified_email_list = data_vendor.verified_email_list == null ? new List<String>() : data_vendor.verified_email_list;
                if ( registered_email.Contains(email) )
                {
                    if (!verified_email_list.Contains(email) )
                    {
                        verified_email_list.Add(email);
                        int verification_status_id = 1;
                        int position_data = 2;
                        bool isFinish = false;
                        if (verified_email_list.Count == registered_email.Count )
                        {
                            ViewBag.alert = "VerifEmailFinish";
                            verification_status_id = 16;
                            position_data = 3;
                            isFinish = true;
                        } else
                        {
                            ViewBag.alert = "VerifEmailSuccess";
                        }

                        string join_verified_email = String.Join(";", verified_email_list);
                        _registration.updateVerifyEmail(vendor_id, join_verified_email, verification_status_id, position_data);
                        if ( isFinish )
                        {
                            String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/registration_success-" + _current_language + ".html";
                            String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                            foreach (string recipient in registered_email)
                            {
                                Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                                email_content_param.Add("vendor_name", data_vendor.name);
                                email_content_param.Add("email", recipient);
                                email_content_param.Add("year", DateTime.Now.Year.ToString());

                                String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                                new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Pendaftaran Berhasil" : "Registration Success"), email_content);
                            }
                        }
                    } else
                    {
                        ViewBag.alert = "AlreadyVerifEmail";
                        error_code = "401"; //Error Email already verified
                        verification_status = false;
                    }
                } else
                {
                    ViewBag.alert = "EmailNotRegistered";
                    error_code = "404"; //Error email not registered
                    verification_status = false;
                }
            }
            return View("Alert");
        }

        [HttpPost , ActionName("verifyEmail")] 
        public async Task<IActionResult> verifyEmail(string token, int vendor_id) 
        { 
            
            return RedirectToAction("Index", "home");
        }

        [Route("/{culture}/{controller}/logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("user_id");
            HttpContext.Session.Remove("role_id");
            HttpContext.Session.Remove("vendor_number");
            var culture = "/" + Thread.CurrentThread.CurrentCulture.Name + "/";
            return Redirect(culture);
        }

        [HttpPost, ActionName("LoginForm")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> LoginForm(string username, string password)
        {
          var passwordEnc = GetMD5(password);
          if (username != "" && passwordEnc != "")
          {
            UserModel createdUser = new UserModel();
            var checkUsername = _user.CheckUsername(username);
            var login = _user.Login(username, passwordEnc);
            if (login.username == null && checkUsername)
            {
              ViewBag.error = _localizer.GetString("InvalidPassword");
              return View("LoginForm");
            }
            else if (login.username == null && !checkUsername)
            {
              ViewBag.error = _localizer.GetString("InvalidUser");
              return View("LoginForm");
            }
            _user.updateLastLogin(username, passwordEnc);
            HttpContext.Session.SetString("username", username);
            HttpContext.Session.SetString("role_id", login.role_id.ToString());
            HttpContext.Session.SetString("user_id", login.user_id.ToString());
            HttpContext.Session.SetString("vendor_number", login.vendor_number.ToString());
            return RedirectToAction("index", "dashboard");
          }
          else
          {
            ViewBag.error = _localizer.GetString("InvalidUser");
            return View("LoginForm");
          }
        }

        [HttpGet, ActionName("Login")]
        public IActionResult LoginForm()
        {
            
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            {
                return RedirectToAction("index", "Dashboard");
            }
            
          return View("LoginForm");
        }


        [HttpGet, ActionName("RequestResetPassword")]
        public IActionResult RequestResetPasswordForm()
        {
          return View("RequestResetPasswordForm");
        }

        [HttpPost, ActionName("RequestResetPassword")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> RequestResetPassword(string username)
        {
          var data = _user.GetUsernameOrEmail(username);
          int user_id = data.user_id;
          if (username == data.username || username ==  data.email )
          {           
          String token = Guid.NewGuid().ToString();
          _user.UpdateResetToken(user_id, token);
          Console.WriteLine("token:" +token);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/reset_password-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                List<String> email_list = new Helpers.EmailFunction().GetEmailList(data.email);
                foreach (string email in email_list)
                    {
                        Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                        email_content_param.Add("email", email);
                        email_content_param.Add("year", DateTime.Now.Year.ToString());
                        String encrypted_user_id = new Helpers.GlobalFunction().Base64Encode(data.user_id.ToString());
                        email_content_param.Add("Reset_Password_link", _appSettings.BaseUrl + _current_language + "/Home/EmailResetPassword?uid=" + encrypted_user_id + "&token=" + token + "&email=" + email);

                        String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                        
                        new Helpers.EmailFunction().sending_email(email, (_current_language == "id" ? "Mengatur Ulang Password" : "Reset Password"), email_content);
                    }

                    ViewBag.registration_success = "Permintaan ubah password telah berhasil, silahkan cek email anda";
                    return View("RequestResetPasswordForm");
          }
          ViewBag.error = _localizer.GetString("InvalidUser");
          return View("RequestResetPasswordForm");
        }

        public IActionResult EmailResetPassword(string uid, string token)
        {
            string decoded_user = new Helpers.GlobalFunction().Base64Decode(uid);
            int user_id = Convert.ToInt32(decoded_user);
            var data = _user.GetUserDetail(user_id);
            if (data.user_id == user_id && data.reset_token == token)
            {
                ViewBag.controller = "Home";
                ViewBag.user_id = uid;
                ViewBag.token = token;
                return View("ResetPasswordForm");
            }
            ViewBag.error = _localizer.GetString("InvalidUser");
            return View ("Alert");
        }

        [HttpGet, ActionName("ResetPassword")]
        public IActionResult ResetPasswordForm()
        {
          return View("ResetPasswordForm");
        }

        [HttpPost, ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public IActionResult ResetPassword(string uid, string token, string password, string password_confirm)
        {
        
            var pass_encrypt = GetMD5(password);
            Console.WriteLine("to :"+pass_encrypt);
            string page = HttpContext.Request.Query["page"].ToString();
            int user_id = Convert.ToInt32(new Helpers.GlobalFunction().Base64Decode(uid));
            var data = _user.GetUserDetail(user_id);
            if (user_id ==data.user_id && token == data.reset_token){
                if (password == password_confirm)
                {
                  var UpdatedPass = _user.UpdatePassword(user_id, pass_encrypt,token);
                  // Console.WriteLine("UPDATED PASS: " +UpdatedPass);
                    if(UpdatedPass > 0){
                      ViewBag.registration_success = "Permintaan ubah password telah berhasil";
                      return RedirectToAction("Login", "home");
                    }else {
                      ViewBag.error = "Reset password gagal";
                      return View("ResetPasswordForm");
                    }
                }
            }
            ViewBag.error = _localizer.GetString("PasswordMustBeSame");
            return View("ResetPasswordForm");
        }

    //tes email indesso--
    [HttpGet, ActionName("email")]
    public IActionResult email()
    {
      new Helpers.EmailFunction().sending_email("mamansulaeman2904@gmail.com", "Error RPA", "ok");
      return Json(new { status = 200, data = "sukses" });
    }
    }
}

