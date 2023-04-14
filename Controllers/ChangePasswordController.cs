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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace tufol.Controllers
{
    public class ChangePasswordController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly Helpers.AppSettings _appSettings;
        public ChangePasswordController(IMenu menu, IUser user, IHttpContextAccessor httpContextAccessor, IStringLocalizer<HomeController> localizer, IOptions<Helpers.AppSettings> appSettings)
        {
            _menu = menu;
            _user = user;
            _appSettings = appSettings.Value;

            _localizer = localizer;
            _list_menu = _menu.GetMenu(httpContextAccessor.HttpContext.Session.GetString("role_id"));
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Menu = _list_menu;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewData["idRole"] = HttpContext.Session.GetString("idRole");
            return View();
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PreventDuplicateRequest]
    public IActionResult Index(string currentPassword, string NewPassword, string verifyPassword)
    {
      ViewBag.Menu = _list_menu;
      ViewBag.BaseUrl = _appSettings.BaseUrl;
      ViewBag.Menu = _list_menu;
      string user_id = HttpContext.Session.GetString("user_id");
      ViewData["name"] = HttpContext.Session.GetString("name");
      ViewData["idRole"] = HttpContext.Session.GetString("idRole");
      
      var encPass = new Helpers.GlobalFunction().GetMD5(currentPassword);
      var inEncPass = new Helpers.GlobalFunction().GetMD5(NewPassword);
      var dataPassword = _user.GetPassword(encPass,user_id);

      // Console.WriteLine(dataPassword);

      if (encPass != dataPassword)
      {
        // Console.WriteLine("Password not found" + dataPassword);
        ViewBag.error = _localizer.GetString("InvalidPassword");
        return View("~/Views/ChangePassword/Index.cshtml");
      }
      if (NewPassword != verifyPassword)
      {
        ViewBag.error = _localizer.GetString("PasswordMustBeSame");
        return View("~/Views/ChangePassword/Index.cshtml");

      }
      int UpdatedPassword = _user.UpdateByPassword(inEncPass,user_id);

      // Console.WriteLine(UpdatedPassword);
      
      if(UpdatedPassword > 0 ){
        ViewBag.reset_success = "Password has been successfully updated";
        return View("~/Views/ChangePassword/Index.cshtml");
      }else {
        ViewBag.error = "Something went wrong, unable to update the password";
        return View("~/Views/ChangePassword/Index.cshtml");
      }

      //  ViewBag.registration_success = "Password berhasil di ubah";


    }
  }
}