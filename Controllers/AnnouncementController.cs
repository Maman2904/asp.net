using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using tufol.Models;
using tufol.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Threading;

namespace tufol.Controllers
{
    public class AnnouncementController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private IMaster _master;
        private readonly IEnumerable<MenuModel> _list_menu;
        private ISetting _setting;
        private String _current_language;
        private readonly Helpers.AppSettings _appSettings;
        private readonly PermissionModel _permission;
        private readonly IStringLocalizer<AnnouncementController> _localizer;
        private IWebHostEnvironment _iHostingEnvironment;
        public AnnouncementController(IMenu menu, IUser user,
        ISetting setting, IOptions<Helpers.AppSettings> appSettings, IStringLocalizer<AnnouncementController> localizer, IWebHostEnvironment iHostingEnvironment, 
        IMaster master, IHttpContextAccessor httpContextAccessor)
        {
            _menu = menu;
            _user = user;
            _master = master;
            _setting = setting;
            _appSettings = appSettings.Value;
            _iHostingEnvironment = iHostingEnvironment;
            _localizer = localizer;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
            _list_menu = _menu.GetMenu(httpContextAccessor.HttpContext.Session.GetString("role_id"));
            _permission = _menu.GetPermission("UserManagement", httpContextAccessor.HttpContext.Session.GetString("role_id"));
        }
        // [HttpGet, ActionName("index")]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.Get("username") == null)
            {
            //   return RedirectToAction("Login", "Home");
            }

            ViewBag.Menu = _list_menu;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("idRole");
            return View();
        }

        [HttpGet, ActionName("all_datatable")]
        public IActionResult LoadData(Dictionary<string, string> pagination, Dictionary<string, string> sort = null, Dictionary<string, string> query = null, string username = null, int role_id = 0)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try
            {
                var data = _master.GetAnnouncementList(_current_language);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetAnnouncementListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }


        // UPDATE ENDPOINT: Announcement/Edit/landing_page
        [HttpPost, ActionName("EditAnnouncement")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> EditAnnouncement(AnnouncementModel model)
        {
          
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            try {
                if(ModelState.IsValid){
                string key_flag = model.key_flag;
                Console.WriteLine("Key flaag: "+key_flag);
                var updatedAnnouncement =  _master.UpdateAnnouncement(model, key_flag);
                return RedirectToAction("Index", "Announcement");
                }
            } catch (Exception ex)  {
                Console.WriteLine(ex);
            }
           var Data = _master.GetAnnouncementDetail(model.key_flag);

            ViewBag.DataAnnouncement = Data;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("Edit");

        }
        // GET: /Announcement/Edit
        [Route("/{culture}/{controller}/Edit/{key_flag}")]
        public IActionResult Edit(string key_flag)
        {
            if (!_permission.allow_update)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var Data = _master.GetAnnouncementDetail(key_flag);

            ViewBag.DataAnnouncement = Data;
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            return View("Edit");
        }
    }
}