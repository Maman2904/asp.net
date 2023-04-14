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
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace tufol.Controllers
{
    public class UserManagementController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private IMaster _master;
        private ISetting _setting;
        private String _current_language;
        private readonly Helpers.AppSettings _appSettings;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly PermissionModel _permission;
        private readonly IStringLocalizer<UserManagementController> _localizer;
        private IWebHostEnvironment _iHostingEnvironment;
        public UserManagementController(IMenu menu, IMaster master, IUser user,ISetting setting, IOptions<Helpers.AppSettings> appSettings, IStringLocalizer<UserManagementController> localizer, IWebHostEnvironment iHostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _menu = menu;
            _master = master;
            _user = user;
            _setting = setting;
            _appSettings = appSettings.Value;
            _iHostingEnvironment = iHostingEnvironment;
            _localizer = localizer;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            _list_menu = _menu.GetMenu(httpContextAccessor.HttpContext.Session.GetString("role_id"));
            _permission = _menu.GetPermission("UserManagement", httpContextAccessor.HttpContext.Session.GetString("role_id"));
        }
        // GET: /UserManagement/
        public IActionResult Index()
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
            return View();
        }
        // GET: /UserManagement/Detil
        [Route("/{culture}/{controller}/Detil/{user_id}")]
        public IActionResult Detil(int user_id)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataUser = _user.GetUserDetail(user_id);

            ViewBag.DataUser = DataUser;
            Console.WriteLine("Data USer: "+DataUser+" user name:"+DataUser.username);
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            return View("Detil");
        }

        // GET: /UserManagement/all_user_datatable/
        [HttpGet, ActionName("all_user_datatable")]
        public IActionResult LoadData(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null, string username = null, int role_id = 0)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { status = 401, message = "Unauthorized" });
            }
            try
            {
                var data = _user.GetUserListDatatable(pagination, sort, query, username, role_id);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetUserListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        public IActionResult Add()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            UserIndessoModel DataUser = new UserIndessoModel();
            ViewBag.DataUser = DataUser;
            ViewBag.CompanyArea = _master.GetCompanyAreaCompany();
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            ViewBag.action = "Add";
            return View("Add");
        }

        // POST: UserManagement/Add 
        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Add(UserIndessoModel model)
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            String error_message = "";
            String alert_message = "";
            String icon_message = "";
            var Valid = "";
            try {
                if(!Regex.IsMatch(model.password, @"^[a-zA-Z0-9]+$")){
                    Valid = "Must Be contains alpha and numeric";
                }
                if(ModelState.IsValid){
                    var passwordEnc = new Helpers.GlobalFunction().GetMD5(model.password);
                    var result =  _user.AddUserIndesso(model, passwordEnc);
                    if (Convert.ToInt32(result.Result) > 0) {
                        error_message = _localizer.GetString("SuccessToSave");
                        icon_message = "far fa-check-circle";
                        alert_message = "alert-success";
                        return RedirectToAction("Index", "UserManagement");
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
            ViewBag.Valid = Valid;
            Console.WriteLine("valid"+ Valid);
            UserIndessoModel DataUser = new UserIndessoModel();
            ViewBag.DataUser = DataUser;
            ViewBag.CompanyArea = _master.GetCompanyAreaCompany();
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewBag.Menu = _list_menu;
            ViewBag.action = "Add";
            return View("Add");
            
        }

        // GET: /UserManagement/Edit
        [Route("/{culture}/{controller}/Edit/{user_id}")]
        public IActionResult Edit(int user_id)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var DataUser = _user.GetUserDetail(user_id);

            ViewBag.DataUser = DataUser;
            ViewBag.CompanyArea = _master.GetCompanyAreaCompany();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.action = "Edit";
            return View("Edit");
        }

        // Put: UserManagement/Edit
        [HttpPost, ActionName("EditUser")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditUser(UserIndessoModel model)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
                // var pass = "";
                // var DataUser = _user.GetUserDetail(model.user_id);
                int id = model.user_id;
                // if(model.password == DataUser.password){
                //         pass = model.password;
                // }else{
                //         var passwordEnc = new Helpers.GlobalFunction().GetMD5(model.password);
                //         pass = passwordEnc;
                //     }

                // var updateddUser =  _user.UpdateUser(model, id, pass);
                var updateddUser =  _user.UpdateUser(model, id);
            } catch (Exception ex)  {
                Console.WriteLine(ex);
            }
            return RedirectToAction("Index", "UserManagement");
        }

        // DELETE: UserManagement/Delete/5
        [HttpDelete, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!_permission.allow_delete)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
               var deletedUser =  _user.DeleteUser(id);
            } catch (Exception ex)  {
                Console.WriteLine("error : "+ex);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}