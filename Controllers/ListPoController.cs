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
    public class ListPoController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private IOpenItemPo _open_item_po;
        private readonly PermissionModel _permission;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly Helpers.AppSettings _appSettings;
        private string _role_id;
        public ListPoController(IMenu menu, IUser user, IOpenItemPo open_item_po, IHttpContextAccessor httpContextAccessor, IStringLocalizer<HomeController> localizer, IOptions<Helpers.AppSettings> appSettings)
        {
            _menu = menu;
            _user = user;
            _open_item_po = open_item_po;
            _appSettings = appSettings.Value;

            _localizer = localizer;
            _role_id = httpContextAccessor.HttpContext.Session.GetString("role_id");
            _list_menu = _menu.GetMenu(_role_id);
            _permission = _menu.GetPermission("ListPo", httpContextAccessor.HttpContext.Session.GetString("role_id"));
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            ViewBag.Menu = _list_menu;
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewBag.role_id = _role_id;
            return View();
        }

        [HttpPost, ActionName("open_item_po_datatable")]
        public IActionResult LoadOpenItemPoData(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(_role_id);
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                List<string> selected_column = new List<string>()
                {
                    "vendor_name", "po_item_number", "doc_date", "po_number", "po_name", "po_quantity", "uom", "currency", "net_price"
                };
                var data = _open_item_po.GetOpenItemPoDatatable(datatable_params, selected_column, condition, vendor_number);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }
    }
}