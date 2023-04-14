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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Localization;
using System.Threading;

namespace tufol.Controllers
{
    public class TicketController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private ITicket _ticket;
        private IVendor _vendor;
        private IMaster _master;
        private ISetting _setting;
        private readonly PermissionModel _permission;
        private readonly IEnumerable<MenuModel> _list_menu;
        private readonly Helpers.AppSettings _appSettings;
        private string _role_id;
        private String _current_language;
        private IWebHostEnvironment _iHostingEnvironment;
        private readonly IStringLocalizer<TicketController> _localizer;
        
        public TicketController(IMenu menu, IUser user, ITicket ticket, IVendor vendor, IMaster master, ISetting setting, IOptions<Helpers.AppSettings> appSettings, IWebHostEnvironment iHostingEnvironment, IHttpContextAccessor httpContextAccessor, IStringLocalizer<TicketController> localizer)
        {
            _menu = menu;
            _user = user;
            _ticket = ticket;
            _vendor = vendor;
            _master = master;
            _setting = setting;
            _iHostingEnvironment = iHostingEnvironment;
            _role_id = httpContextAccessor.HttpContext.Session.GetString("role_id");
            _list_menu = _menu.GetMenu(_role_id);
            _permission = _menu.GetPermission("Ticket", _role_id);
            _localizer = localizer;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            _appSettings = appSettings.Value;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
        }
        // [HttpGet, ActionName("index")]
        public async Task<IActionResult> Index()
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            ViewData["name"] = HttpContext.Session.GetString("name");
            ViewBag.idRole = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            
            return View();
        }
        public async Task<IActionResult> PrintBarcode(string ticket_number)
        {
            // if (!_permission.allow_read)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            var role = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            var DataTicket = _ticket.GetTicketDetail(ticket_number);
            ViewBag.ticket_data = DataTicket;
            return View();
        }
        public async Task<IActionResult> PrintTicket()
        {
            // if (!_permission.allow_read)
            // {
            //     Response.StatusCode = StatusCodes.Status401Unauthorized;
            //     return View("~/Views/Shared/Unauthorized.cshtml");
            // }
            var role = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            var DataTicket = _ticket.GetTicketByPositionData(Convert.ToInt32(role.ToString()));
            ViewBag.ticket_data = DataTicket;
            if(role == "4") {
                foreach (var item in DataTicket)
                {
                    _ticket.updateTicketPositionData(item.ticket_number, 5);
                    if(item.verification_status_id == 10){
                        _ticket.updateTicketReceivedDate(item.ticket_number, DateTime.Now);
                        _ticket.updateTicketVerificationStatus(item.ticket_number, 11);
                        // send email to vendor
                        List<String> registered_email = item.email_list == null ? new List<String>() : item.email_list;
                        String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_received-" + _current_language + ".html";
                        String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                        foreach (string recipient in registered_email)
                        {
                            Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                            email_content_param.Add("vendor_name", item.name);
                            email_content_param.Add("ticket_number", item.ticket_number);
                            email_content_param.Add("email", recipient);
                            email_content_param.Add("year", DateTime.Now.Year.ToString());
                            
                            String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                            new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Tiket Diterima" : "Ticket Received"), email_content);
                        }
                    }
                }
                return View();
            } 
            else return View("Index");
        }
        [HttpPost, ActionName("pending_ticket_datatable")]
        public IActionResult LoadPendingData(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                if ( role_id == 6 )
                {
                    condition.Add("status_rpa !=", 0);
                    condition.Add("request_simulate", 0);
                }
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column , condition, false, role_id, vendor_number, user_id);
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpPost, ActionName("error_ticket_datatable")]
        public IActionResult LoadErrorData(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                condition.Add("status_rpa", 0);
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "miro_number", "posting_date",
                    "remmitance_number", "remmitance_date", "status", "rpa_status_description"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, false, role_id, vendor_number, user_id);
                // var data = "";
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpPost, ActionName("need_reversal_datatable")]
        public IActionResult LoadNeedReversal(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                condition.Add("verification_status_id", 15);
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "miro_number", "posting_date",
                    "remmitance_number", "remmitance_date", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, false, role_id, user_id);
                // var data = "";
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpPost, ActionName( "rejected_ticket_datatable")]
        public IActionResult LoadRejectedTicket(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                condition.Add("label_for_vendor", "Rejected");
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, false, role_id, vendor_number, user_id);
                // var data = "";
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

         [HttpPost, ActionName( "need_revision_ticket_datatable")]
        public IActionResult LoadNeedRevisionTicket(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                condition.Add("verification_status_id", 2);
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, false, role_id, vendor_number, user_id);

                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpPost, ActionName( "draft_ticket_datatable")]
        public IActionResult LoadDraftTicket(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                condition.Add("verification_status_id", 9);
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, false, role_id, vendor_number, user_id);

                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpPost, ActionName( "process_ticket_datatable")]
        public IActionResult LoadProcessTicket(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, true, role_id, vendor_number, user_id);
                // var data = "";
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

        [HttpPost, ActionName( "simulation_ticket_datatable")]
        public IActionResult LoadSimulationTicket(PostDatatableNetModel datatable_params)
        {
            try
            {
                int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                condition.Add("request_simulate", 1);
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "status"
                };
                var data = _ticket.GetTicketDatatable(role_id, datatable_params, selected_column, condition, false, role_id, vendor_number, user_id);
                // var data = "";
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }

         [HttpPost, ActionName( "all_ticket_datatable")]
        public IActionResult LoadAllTicket(PostDatatableNetModel datatable_params)
        {
            try
            {
                // int role_id = Convert.ToInt32(HttpContext.Session.GetString("role_id"));
                string user_id = HttpContext.Session.GetString("user_id");
                string vendor_number = HttpContext.Session.GetString("vendor_number");
                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                List<string> selected_column = new List<string>()
                {
                    "created_at", "received_date", "ticket_number", "vendor_number", "vendor_name", "invoice_number", "invoice_amount", "miro_number", "posting_date",
                    "remmitance_number", "remmitance_date", "status"
                };
                var data = _ticket.GetTicketDatatable(0, datatable_params, selected_column, condition, true, 0, vendor_number, user_id);
                // var data = "";
                return Json(data);
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketListDatatable", ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { status = 500, message = ex.Message });
            }
        }
        [Route("/{culture}/{controller}/Detil/{ticket_number}")]
        public async Task<IActionResult> Detil(string ticket_number, string error_message = null, string alert_message = null, string icon_message = null)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            ViewBag.error_message = error_message;
            ViewBag.alert_message = alert_message;
            ViewBag.icon_message = icon_message;
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            var DataTicket = _ticket.GetTicketDetail(ticket_number);
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.ticket_data = DataTicket;
            ViewBag.simulation_result = _ticket.GetSimulationResult(ticket_number);

            bool isReadOnly = false;
            if((DataTicket.position_data != 2 && _role_id != "5" && _role_id != "6") || (_role_id != DataTicket.position_data.ToString()) || !String.IsNullOrEmpty(DataTicket.remmitance_number) || _role_id == "1") {
                isReadOnly = true;
            }
            if(Convert.ToInt32(_role_id) == 5 && (DataTicket.verification_status_id == 10 || DataTicket.verification_status_id == 11)){
                _ticket.updateTicketReceivedDate(DataTicket.ticket_number, DateTime.Now);
                _ticket.updateTicketVerificationStatus(DataTicket.ticket_number, 6);
                // send email to vendor
                List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_received-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                // email company
                var company_email = _ticket.GetEmailCompanyArea(DataTicket.area_code);
                registered_email.Add(company_email);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("vendor_name", DataTicket.name);
                    email_content_param.Add("ticket_number", DataTicket.ticket_number);
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    
                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Tiket Diterima" : "Ticket Received"), email_content);
                }
            }
            if(isReadOnly) {
                return View();
            } else {
                return View("Form");
            }
        }

        [HttpPost, ActionName("Save")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Save(CreateTicketModel model)
        {
            var vendor_number = HttpContext.Session.GetString("vendor_number");
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);  
            String error_message = "";
            String alert_message = "";
            String icon_message = "";

            model.position_data = 2;
            model.request_simulate = 0;
            model.is_finish = 0;
            model.verification_status_id = 9;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;
            model.verification_note = DataTicket.verification_note == null ? null : DataTicket.verification_note;

            if (ModelState.IsValid)
            {
                string upload_basedir = "document/ticket/";
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

                string file_invoice_name = null;
                string file_invoice_path = null;
                if (model.file_invoice != null && model.file_invoice.Length != 0)
                {
                    file_invoice_name = "invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_invoice = Path.Combine(upload_directory, file_invoice_name);
                    using (var stream = System.IO.File.Create(path_file_invoice))
                    {
                        await model.file_invoice.CopyToAsync(stream);
                        file_invoice_path = upload_basedir + file_invoice_name;
                    }
                }

                string file_po_name = null;
                string file_po_path = null;
                if (model.file_po != null && model.file_po.Length != 0)
                {
                    file_po_name = "po-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_po = Path.Combine(upload_directory, file_po_name);
                    using (var stream = System.IO.File.Create(path_file_po))
                    {
                        await model.file_po.CopyToAsync(stream);
                        file_po_path = upload_basedir + file_po_name;
                    }
                }
                
                string file_lpb_gr_name = null;
                string file_lpb_gr_path = null;
                if (model.file_lpb_gr != null && model.file_lpb_gr.Length != 0)
                {
                    file_lpb_gr_name = "lpb_gr-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_lpb_gr = Path.Combine(upload_directory, file_lpb_gr_name);
                    using (var stream = System.IO.File.Create(path_file_lpb_gr))
                    {
                        await model.file_lpb_gr.CopyToAsync(stream);
                        file_lpb_gr_path = upload_basedir + file_lpb_gr_name;
                    }
                }
                
                string file_tax_invoice_name = null;
                string file_tax_invoice_path = null;
                if (model.file_tax_invoice != null && model.file_tax_invoice.Length != 0)
                {
                    file_tax_invoice_name = "tax_invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_tax_invoice = Path.Combine(upload_directory, file_tax_invoice_name);
                    using (var stream = System.IO.File.Create(path_file_tax_invoice))
                    {
                        await model.file_tax_invoice.CopyToAsync(stream);
                        file_tax_invoice_path = upload_basedir + file_tax_invoice_name;
                    }
                }
                
                string file_delivery_note_name = null;
                string file_delivery_note_path = null;
                if (model.file_delivery_note != null && model.file_delivery_note.Length != 0)
                {
                    file_delivery_note_name = "delivery_note-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_delivery_note = Path.Combine(upload_directory, file_delivery_note_name);
                    using (var stream = System.IO.File.Create(path_file_delivery_note))
                    {
                        await model.file_delivery_note.CopyToAsync(stream);
                        file_delivery_note_path = upload_basedir + file_delivery_note_name;
                    }
                }
                
                string file_bast_name = null;
                string file_bast_path = null;
                if (model.file_bast != null && model.file_bast.Length != 0)
                {
                    file_bast_name = "bast-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_bast = Path.Combine(upload_directory, file_bast_name);
                    using (var stream = System.IO.File.Create(path_file_bast))
                    {
                        await model.file_bast.CopyToAsync(stream);
                        file_bast_path = upload_basedir + file_bast_name;
                    }
                }

                bool update_ticket = _ticket.updateTicket(model.ticket_number, model, file_invoice_path, file_po_path, file_lpb_gr_path, file_tax_invoice_path, file_delivery_note_path, file_bast_path);
                Console.WriteLine(update_ticket);
                if(update_ticket) {
                    //Delete Old File
                    if (!string.IsNullOrEmpty(file_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice);
                    }
                    if (!string.IsNullOrEmpty(file_lpb_gr_name) && !string.IsNullOrEmpty(DataTicket.file_lpb_gr) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr);
                    }
                    if (!string.IsNullOrEmpty(file_tax_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_tax_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice);
                    }
                    if (!string.IsNullOrEmpty(file_delivery_note_name) && !string.IsNullOrEmpty(DataTicket.file_delivery_note) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note);
                    }
                    if (!string.IsNullOrEmpty(file_bast_name) && !string.IsNullOrEmpty(DataTicket.file_bast) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast);
                    }
                    //End delete old file

                    bool delete_po_ticket = _ticket.DeletePoTicket(model.ticket_number);
                    bool insert_po_ticket = _ticket.InsertPoTicket(model, model.ticket_number);
                    bool update_mailing_data = _vendor.updateMailingData(model.vendor_number, model.mailing_address, model.district, model.village, model.province);
                    bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);

                    // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                    // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);

                    // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                    //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                    //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
                    // }
                    error_message = _localizer.GetString("SuccessToSave");
                    icon_message = "far fa-check-circle";
                    alert_message = "alert-success";
                    ViewBag.Menu = _list_menu;
                    ViewBag.role_id = _role_id;
                    ViewBag.ticket_data = DataTicket;
                    ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                    // ViewBag.error_message = error_message;
                    // ViewBag.alert_message = alert_message;
                    // ViewBag.icon_message = icon_message;
                    return RedirectToAction("Index", "Ticket");
                } else {
                    error_message = _localizer.GetString("FailedToSave");
                    icon_message = "flaticon-circle";
                    alert_message = "alert-danger";
                }
            }
            // popup
            var pop_up = _master.GetAnnouncementDetail("popup_ticket");
            ViewBag.pop_up = pop_up;
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            ViewBag.ticket_data = model;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            // ViewBag.error_message = error_message + " ini message";
            // ViewBag.alert_message = alert_message;
            // ViewBag.icon_message = icon_message;
            return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number, error_message = error_message, alert_message = alert_message, icon_message = icon_message });
            // return View("Form");
        }

        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Send(CreateTicketModel model)
        {
            var vendor_number = HttpContext.Session.GetString("vendor_number");
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);  
            String error_message = "";
            String alert_message = "";
            String icon_message = "";

            model.is_finish = 0;
            model.request_simulate = 0;
            model.verification_status_id = 10;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;
            
            if(model.dpp > model.dpp_original || model.dpp != model.dpp_original) {
                model.position_data = 3;
            } else {
                if(model.area_code == "8230" || model.area_code == "8520") {
                    model.position_data = 4;
                } else {
                    model.position_data = 5; //ini jadinya ke FC langsung 5 (procurement skip dl)
                }
            }

            if (ModelState.IsValid)
            {
                string upload_basedir = "document/ticket/";
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

                string file_invoice_name = null;
                string file_invoice_path = null;
                if (model.file_invoice != null && model.file_invoice.Length != 0)
                {
                    file_invoice_name = "invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_invoice = Path.Combine(upload_directory, file_invoice_name);
                    using (var stream = System.IO.File.Create(path_file_invoice))
                    {
                        await model.file_invoice.CopyToAsync(stream);
                        file_invoice_path = upload_basedir + file_invoice_name;
                    }
                }

                string file_po_name = null;
                string file_po_path = null;
                if (model.file_po != null && model.file_po.Length != 0)
                {
                    file_po_name = "po-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_po = Path.Combine(upload_directory, file_po_name);
                    using (var stream = System.IO.File.Create(path_file_po))
                    {
                        await model.file_po.CopyToAsync(stream);
                        file_po_path = upload_basedir + file_po_name;
                    }
                }
                
                string file_lpb_gr_name = null;
                string file_lpb_gr_path = null;
                if (model.file_lpb_gr != null && model.file_lpb_gr.Length != 0)
                {
                    file_lpb_gr_name = "lpb_gr-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_lpb_gr = Path.Combine(upload_directory, file_lpb_gr_name);
                    using (var stream = System.IO.File.Create(path_file_lpb_gr))
                    {
                        await model.file_lpb_gr.CopyToAsync(stream);
                        file_lpb_gr_path = upload_basedir + file_lpb_gr_name;
                    }
                }
                
                string file_tax_invoice_name = null;
                string file_tax_invoice_path = null;
                if (model.file_tax_invoice != null && model.file_tax_invoice.Length != 0)
                {
                    file_tax_invoice_name = "tax_invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_tax_invoice = Path.Combine(upload_directory, file_tax_invoice_name);
                    using (var stream = System.IO.File.Create(path_file_tax_invoice))
                    {
                        await model.file_tax_invoice.CopyToAsync(stream);
                        file_tax_invoice_path = upload_basedir + file_tax_invoice_name;
                    }
                }
                
                string file_delivery_note_name = null;
                string file_delivery_note_path = null;
                if (model.file_delivery_note != null && model.file_delivery_note.Length != 0)
                {
                    file_delivery_note_name = "delivery_note-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_delivery_note = Path.Combine(upload_directory, file_delivery_note_name);
                    using (var stream = System.IO.File.Create(path_file_delivery_note))
                    {
                        await model.file_delivery_note.CopyToAsync(stream);
                        file_delivery_note_path = upload_basedir + file_delivery_note_name;
                    }
                }
                
                string file_bast_name = null;
                string file_bast_path = null;
                if (model.file_bast != null && model.file_bast.Length != 0)
                {
                    file_bast_name = "bast-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_bast = Path.Combine(upload_directory, file_bast_name);
                    using (var stream = System.IO.File.Create(path_file_bast))
                    {
                        await model.file_bast.CopyToAsync(stream);
                        file_bast_path = upload_basedir + file_bast_name;
                    }
                }

                bool update_ticket = _ticket.updateTicket(model.ticket_number, model, file_invoice_path, file_po_path, file_lpb_gr_path, file_tax_invoice_path, file_delivery_note_path, file_bast_path);
                Console.WriteLine(update_ticket);
                if(update_ticket) {
                    //Delete Old File
                    if (!string.IsNullOrEmpty(file_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice);
                    }
                    if (!string.IsNullOrEmpty(file_lpb_gr_name) && !string.IsNullOrEmpty(DataTicket.file_lpb_gr) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr);
                    }
                    if (!string.IsNullOrEmpty(file_tax_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_tax_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice);
                    }
                    if (!string.IsNullOrEmpty(file_delivery_note_name) && !string.IsNullOrEmpty(DataTicket.file_delivery_note) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note);
                    }
                    if (!string.IsNullOrEmpty(file_bast_name) && !string.IsNullOrEmpty(DataTicket.file_bast) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast);
                    }
                    //End delete old file

                    bool delete_po_ticket = _ticket.DeletePoTicket(model.ticket_number);
                    bool insert_po_ticket = _ticket.InsertPoTicket(model, model.ticket_number);
                    bool update_mailing_data = _vendor.updateMailingData(DataTicket.vendor_number, model.mailing_address, model.district, model.village, model.province);
                    bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);
                    
                    // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                    // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);

                    // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                    //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                    //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
                    // }

                    if(delete_po_ticket && insert_po_ticket && update_mailing_data) {
                        //Insert Log
                        LogTicketModel data_insert = new LogTicketModel()
                        {
                            ticket_number = model.ticket_number,
                            process = "Revision Ticket",
                            note = model.verification_note,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        _ticket.InsertLog(data_insert);

                        List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                        String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_revision_success-" + _current_language + ".html";
                        String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                        // email company
                        var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                        registered_email.Add(company_email);
                        foreach (string recipient in registered_email)
                        {
                            Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                            email_content_param.Add("vendor_name", DataTicket.name);
                            email_content_param.Add("ticket_number", model.ticket_number);
                            email_content_param.Add("email", recipient);
                            email_content_param.Add("year", DateTime.Now.Year.ToString());
                            
                            String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                            new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Tiket Berhasil" : "Revision Ticket Success"), email_content);
                        }
                    }
                    error_message = _localizer.GetString("SuccessToSave");
                    icon_message = "far fa-check-circle";
                    alert_message = "alert-success";
                    ViewBag.Menu = _list_menu;
                    ViewBag.role_id = _role_id;
                    ViewBag.ticket_data = DataTicket;
                    ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                    // ViewBag.error_message = error_message;
                    // ViewBag.alert_message = alert_message;
                    // ViewBag.icon_message = icon_message;
                    return RedirectToAction("Index", "Ticket");
                } else {
                    error_message = _localizer.GetString("FailedToSave");
                    icon_message = "flaticon-circle";
                    alert_message = "alert-danger";
                }
            }
            // popup
            var pop_up = _master.GetAnnouncementDetail("popup_ticket");
            ViewBag.pop_up = pop_up;
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            ViewBag.ticket_data = model;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            // ViewBag.error_message = error_message;
            // ViewBag.alert_message = alert_message;
            // ViewBag.icon_message = icon_message;
            return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number, error_message = error_message, alert_message = alert_message, icon_message = icon_message });
            // return View("Form");
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Delete(CreateTicketModel model)
        {
            var vendor_number = HttpContext.Session.GetString("vendor_number");
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);  
            String error_message = "";
            String alert_message = "";
            String icon_message = "";
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;

            if (ModelState.IsValid)
            {
                var delete_po_ticket = _ticket.DeletePoTicket(model.ticket_number);
                var update_mailing_data = _vendor.updateMailingData(DataTicket.vendor_number, model.mailing_address, model.district, model.village, model.province);
                var delete_ticket = _ticket.DeleteTicket(model.ticket_number);
                bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);
                // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, false);

                // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, false);
                // }

                if(delete_po_ticket && update_mailing_data && delete_ticket) {
                    //Delete Old File
                    if (!string.IsNullOrEmpty(DataTicket.file_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice);
                    }
                    if (!string.IsNullOrEmpty(DataTicket.file_lpb_gr) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr);
                    }
                    if (!string.IsNullOrEmpty(DataTicket.file_tax_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice);
                    }
                    if (!string.IsNullOrEmpty(DataTicket.file_delivery_note) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note);
                    }
                    if (!string.IsNullOrEmpty(DataTicket.file_bast) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast);
                    }
                    //End delete old file
                    error_message = _localizer.GetString("SuccessToSave");
                    icon_message = "far fa-check-circle";
                    alert_message = "alert-success";
                    ViewBag.Menu = _list_menu;
                    ViewBag.role_id = _role_id;
                    ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                    // ViewBag.error_message = error_message;
                    // ViewBag.alert_message = alert_message;
                    // ViewBag.icon_message = icon_message;
                    return RedirectToAction("Index", "Ticket");
                } else {
                    error_message = _localizer.GetString("FailedToSave");
                    icon_message = "flaticon-circle";
                    alert_message = "alert-danger";
                }
            }
            // popup
            var pop_up = _master.GetAnnouncementDetail("popup_ticket");
            ViewBag.pop_up = pop_up;
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            ViewBag.ticket_data = DataTicket;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            // ViewBag.error_message = error_message;
            // ViewBag.alert_message = alert_message;
            // ViewBag.icon_message = icon_message;
            return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number, error_message = error_message, alert_message = alert_message, icon_message = icon_message });
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Approve(CreateTicketModel model) {
            if (!_permission.allow_approve)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);
            string role_user = HttpContext.Session.GetString("role_id");

            int verification_status_id = 4;
            int position_data = 5;
            int request_simulate = 0;
            int is_finish = 0;

            // procurement skip dl
            if (role_user == "3")
            {
                if(DataTicket.verification_status_id == 9) {
                    verification_status_id = 4;
                    position_data = 2; //dibalikin ke vendor
                } else {
                    verification_status_id = 10;
                    if(model.area_code == "8230" || model.area_code == "8520") {
                        position_data = 4;
                    } else {
                        position_data = 5; //ini jadinya ke FC langsung 5 (procurement skip dl)
                    }
                }
            }
            else if (role_user == "5")
            {
                verification_status_id = 5;
                position_data = 6;
                request_simulate = 1;
            }
            else if (role_user == "6")
            {
                verification_status_id = 12;
                position_data = 9; //arahin ke rpa dl
            }
            else
            {
                return RedirectToAction("Index", "Ticket");
            }

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;
            model.request_simulate = request_simulate;
            model.is_finish = is_finish;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;

            String error_message = "";
            String icon_message = "";
            String alert_message = "";
            if(role_user == "3") {
                var saveUpdate = _ticket.updateTicketVerificationStatus(DataTicket.ticket_number, verification_status_id, null);
                if(saveUpdate) {
                    _ticket.updateTicketPositionData(DataTicket.ticket_number, position_data);
                    //Insert Log
                    LogTicketModel data_insert = new LogTicketModel()
                    {
                        ticket_number = model.ticket_number,
                        process = "Approve Ticket",
                        note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    _ticket.InsertLog(data_insert);
                    result = true;
                }
            } 
            else {
                if (ModelState.IsValid || String.IsNullOrEmpty(model.gl_amount.ToString()) || !String.IsNullOrEmpty(model.material_amt.ToString()))
                {
                    string upload_basedir = "document/ticket/";
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

                    string file_invoice_name = null;
                    string file_invoice_path = null;
                    if (model.file_invoice != null && model.file_invoice.Length != 0)
                    {
                        file_invoice_name = "invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_invoice = Path.Combine(upload_directory, file_invoice_name);
                        using (var stream = System.IO.File.Create(path_file_invoice))
                        {
                            await model.file_invoice.CopyToAsync(stream);
                            file_invoice_path = upload_basedir + file_invoice_name;
                        }
                    }

                    string file_po_name = null;
                    string file_po_path = null;
                    if (model.file_po != null && model.file_po.Length != 0)
                    {
                        file_po_name = "po-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_po = Path.Combine(upload_directory, file_po_name);
                        using (var stream = System.IO.File.Create(path_file_po))
                        {
                            await model.file_po.CopyToAsync(stream);
                            file_po_path = upload_basedir + file_po_name;
                        }
                    }
                    
                    string file_lpb_gr_name = null;
                    string file_lpb_gr_path = null;
                    if (model.file_lpb_gr != null && model.file_lpb_gr.Length != 0)
                    {
                        file_lpb_gr_name = "lpb_gr-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_lpb_gr = Path.Combine(upload_directory, file_lpb_gr_name);
                        using (var stream = System.IO.File.Create(path_file_lpb_gr))
                        {
                            await model.file_lpb_gr.CopyToAsync(stream);
                            file_lpb_gr_path = upload_basedir + file_lpb_gr_name;
                        }
                    }
                    
                    string file_tax_invoice_name = null;
                    string file_tax_invoice_path = null;
                    if (model.file_tax_invoice != null && model.file_tax_invoice.Length != 0)
                    {
                        file_tax_invoice_name = "tax_invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_tax_invoice = Path.Combine(upload_directory, file_tax_invoice_name);
                        using (var stream = System.IO.File.Create(path_file_tax_invoice))
                        {
                            await model.file_tax_invoice.CopyToAsync(stream);
                            file_tax_invoice_path = upload_basedir + file_tax_invoice_name;
                        }
                    }
                    
                    string file_delivery_note_name = null;
                    string file_delivery_note_path = null;
                    if (model.file_delivery_note != null && model.file_delivery_note.Length != 0)
                    {
                        file_delivery_note_name = "delivery_note-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_delivery_note = Path.Combine(upload_directory, file_delivery_note_name);
                        using (var stream = System.IO.File.Create(path_file_delivery_note))
                        {
                            await model.file_delivery_note.CopyToAsync(stream);
                            file_delivery_note_path = upload_basedir + file_delivery_note_name;
                        }
                    }
                    
                    string file_bast_name = null;
                    string file_bast_path = null;
                    if (model.file_bast != null && model.file_bast.Length != 0)
                    {
                        file_bast_name = "bast-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_bast = Path.Combine(upload_directory, file_bast_name);
                        using (var stream = System.IO.File.Create(path_file_bast))
                        {
                            await model.file_bast.CopyToAsync(stream);
                            file_bast_path = upload_basedir + file_bast_name;
                        }
                    }

                    bool saveUpdate = _ticket.updateTicket(model.ticket_number, model, file_invoice_name, file_po_name, file_lpb_gr_name, file_tax_invoice_name, file_delivery_note_name, file_bast_name);

                    if (saveUpdate)
                    {
                        //Delete Old File
                        if (!string.IsNullOrEmpty(file_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice);
                        }
                        if (!string.IsNullOrEmpty(file_lpb_gr_name) && !string.IsNullOrEmpty(DataTicket.file_lpb_gr) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr);
                        }
                        if (!string.IsNullOrEmpty(file_tax_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_tax_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice);
                        }
                        if (!string.IsNullOrEmpty(file_delivery_note_name) && !string.IsNullOrEmpty(DataTicket.file_delivery_note) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note);
                        }
                        if (!string.IsNullOrEmpty(file_bast_name) && !string.IsNullOrEmpty(DataTicket.file_bast) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast);
                        }
                        //End delete old file

                        if(role_user == "6"){
                            //Delete Old PO
                            _ticket.DeletePoTicket(model.ticket_number);
                            //Insert input PO
                            _ticket.InsertPoTicket(model, model.ticket_number);
                            
                            if(!String.IsNullOrEmpty(model.gl_amount.ToString())) {
                                _ticket.DeleteGlTicket(model.ticket_number);

                                _ticket.InsertGlTicket(model, model.ticket_number);
                            }
                            if(!String.IsNullOrEmpty(model.material_amt.ToString())) {
                                _ticket.DeleteMaterialTicket(model.ticket_number);
                                //Insert input Material
                                _ticket.InsertMaterialTicket(model, model.ticket_number);
                            }
                        }
                        // Update Mailing Data
                        _vendor.updateMailingData(DataTicket.vendor_number, model.mailing_address, model.district, model.village, model.province);
                        bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);
                        
                        // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                        // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);

                        // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                        //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                        //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
                        // }
                        //Insert Log
                        LogTicketModel data_insert = new LogTicketModel()
                        {
                            ticket_number = model.ticket_number,
                            process = "Approve Ticket",
                            note = model.verification_note,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        _ticket.InsertLog(data_insert);
                        result = true;
                    }
                    else
                    {
                        Console.WriteLine("gak bisa di model is valid");
                        error_message = _localizer.GetString("FailedToSave");
                        icon_message = "flaticon-circle";
                        alert_message = "alert-danger";
                        result = false;
                    }
                }else{
                    Console.WriteLine("gak bisa");
                    error_message = _localizer.GetString("FailedToSave");
                    icon_message = "flaticon-circle";
                    alert_message = "alert-danger";
                    result = false;
                }
            }

            if (role_user == "3" && result)
            {
                //Procurement
                var pic = _master.GetProcurementGroupByID(DataTicket.pic_id);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_approval_notification-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() {
                    pic.email
                };
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                recipient_email.Add(company_email);

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
                    
                    email_content_param.Add("message", (_current_language == "id" ? "Ticket dengan nomor " + DataTicket.ticket_number + " disetujui oleh Procurement" : "Ticket number " + DataTicket.ticket_number + " has been approved by Procurement"));
                    email_content_param.Add("title", (_current_language == "id" ? "Buat Tiket" : "Create Ticket"));

                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Persetujuan Tiket" : "Ticket Data Approval"), email_content);
                }
            }
            else if (role_user == "5" && result)
            {
                //FC
                List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_approval_notification-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                registered_email.Add(company_email);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    
                    email_content_param.Add("message", (_current_language == "id" ? "Ticket dengan nomor " + DataTicket.ticket_number + " disetujui oleh FC" : "Ticket number " + DataTicket.ticket_number + " has been approved by FC"));
                    email_content_param.Add("title", (_current_language == "id" ? "Buat Tiket" : "Create Ticket"));

                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Persetujuan Tiket" : "Ticket Data Approval"), email_content);
                }
            }
            else if (role_user == "6" && result)
            {
                //Accounting
                List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_approval_notification-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                registered_email.Add(company_email);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    
                    email_content_param.Add("message", (_current_language == "id" ? "Ticket dengan nomor " + DataTicket.ticket_number + " disetujui untuk pembayaran" : "Ticket number " + DataTicket.ticket_number + " has been approved for payment"));
                    email_content_param.Add("title", (_current_language == "id" ? "Buat Tiket" : "Create Ticket"));

                    email_content_param.Add("email", recipient);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Persetujuan Tiket" : "Ticket Data Approval"), email_content);
                }
            }
            if (result)
            {
                return RedirectToAction("Index", "Ticket");
            }
            else
            {
                // ViewBag.error_message = error_message;
                // ViewBag.alert_message = alert_message;
                // ViewBag.icon_message = icon_message;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = HttpContext.Session.GetString("role_id");
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                ViewBag.ticket_data = model;
                return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number, error_message = error_message, alert_message = alert_message, icon_message = icon_message });
            }
        }

        [HttpPost, ActionName("Revise")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Revise(CreateTicketModel model) {
            if (!_permission.allow_revise)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);
            string role_user = HttpContext.Session.GetString("role_id");

            int verification_status_id = 2;
            int position_data = 2;
            if (role_user == "5")
            {
                verification_status_id = 2;
                position_data = 2; //langsung ke vendor
            } else
            {
                return RedirectToAction("Index", "Ticket");
            }

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;
            model.request_simulate = 0;
            model.is_finish = 0;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;

            String error_message = "";
            String icon_message = "";
            String alert_message = "";
            if (ModelState.IsValid)
            {
                string upload_basedir = "document/ticket/";
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

                string file_invoice_name = null;
                string file_invoice_path = null;
                if (model.file_invoice != null && model.file_invoice.Length != 0)
                {
                    file_invoice_name = "invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_invoice = Path.Combine(upload_directory, file_invoice_name);
                    using (var stream = System.IO.File.Create(path_file_invoice))
                    {
                        await model.file_invoice.CopyToAsync(stream);
                        file_invoice_path = upload_basedir + file_invoice_name;
                    }
                }

                string file_po_name = null;
                string file_po_path = null;
                if (model.file_po != null && model.file_po.Length != 0)
                {
                    file_po_name = "po-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_po = Path.Combine(upload_directory, file_po_name);
                    using (var stream = System.IO.File.Create(path_file_po))
                    {
                        await model.file_po.CopyToAsync(stream);
                        file_po_path = upload_basedir + file_po_name;
                    }
                }
                
                string file_lpb_gr_name = null;
                string file_lpb_gr_path = null;
                if (model.file_lpb_gr != null && model.file_lpb_gr.Length != 0)
                {
                    file_lpb_gr_name = "lpb_gr-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_lpb_gr = Path.Combine(upload_directory, file_lpb_gr_name);
                    using (var stream = System.IO.File.Create(path_file_lpb_gr))
                    {
                        await model.file_lpb_gr.CopyToAsync(stream);
                        file_lpb_gr_path = upload_basedir + file_lpb_gr_name;
                    }
                }
                
                string file_tax_invoice_name = null;
                string file_tax_invoice_path = null;
                if (model.file_tax_invoice != null && model.file_tax_invoice.Length != 0)
                {
                    file_tax_invoice_name = "tax_invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_tax_invoice = Path.Combine(upload_directory, file_tax_invoice_name);
                    using (var stream = System.IO.File.Create(path_file_tax_invoice))
                    {
                        await model.file_tax_invoice.CopyToAsync(stream);
                        file_tax_invoice_path = upload_basedir + file_tax_invoice_name;
                    }
                }
                
                string file_delivery_note_name = null;
                string file_delivery_note_path = null;
                if (model.file_delivery_note != null && model.file_delivery_note.Length != 0)
                {
                    file_delivery_note_name = "delivery_note-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_delivery_note = Path.Combine(upload_directory, file_delivery_note_name);
                    using (var stream = System.IO.File.Create(path_file_delivery_note))
                    {
                        await model.file_delivery_note.CopyToAsync(stream);
                        file_delivery_note_path = upload_basedir + file_delivery_note_name;
                    }
                }
                
                string file_bast_name = null;
                string file_bast_path = null;
                if (model.file_bast != null && model.file_bast.Length != 0)
                {
                    file_bast_name = "bast-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                    var path_file_bast = Path.Combine(upload_directory, file_bast_name);
                    using (var stream = System.IO.File.Create(path_file_bast))
                    {
                        await model.file_bast.CopyToAsync(stream);
                        file_bast_path = upload_basedir + file_bast_name;
                    }
                }

                bool saveUpdate = _ticket.updateTicket(model.ticket_number, model, file_invoice_name, file_po_name, file_lpb_gr_name, file_tax_invoice_name, file_delivery_note_name, file_bast_name);

                if ( saveUpdate )
                {
                    //Delete Old File
                    if (!string.IsNullOrEmpty(file_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice);
                    }
                    if (!string.IsNullOrEmpty(file_lpb_gr_name) && !string.IsNullOrEmpty(DataTicket.file_lpb_gr) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr);
                    }
                    if (!string.IsNullOrEmpty(file_tax_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_tax_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice);
                    }
                    if (!string.IsNullOrEmpty(file_delivery_note_name) && !string.IsNullOrEmpty(DataTicket.file_delivery_note) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note);
                    }
                    if (!string.IsNullOrEmpty(file_bast_name) && !string.IsNullOrEmpty(DataTicket.file_bast) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast))
                    {
                        System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast);
                    }
                    //End delete old file

                    // Update Mailing Data
                    _vendor.updateMailingData(DataTicket.vendor_number, model.mailing_address, model.district, model.village, model.province);
                    bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);
                    // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                    // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);

                    // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                    //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                    //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
                    // }
                    //Insert Log
                    LogTicketModel data_insert = new LogTicketModel()
                    {
                        ticket_number = model.ticket_number,
                        process = "Revise Ticket",
                        note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    _ticket.InsertLog(data_insert);
                    result = true;
                }
                else
                {
                    error_message = _localizer.GetString("FailedToSave");
                    icon_message = "flaticon-circle";
                    alert_message = "alert-danger";
                }
            }

            if (role_user == "5" && result)
            {
                //FC
                var pic = _master.GetProcurementGroupByID(DataTicket.pic_id);

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_revision-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

                List<String> recipient_email = new List<String>() { 
                    pic.email
                };
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                recipient_email.Add(company_email);

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
                    email_content_param.Add("ticket_number", model.ticket_number);
                    email_content_param.Add("note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("revision_link", _appSettings.BaseUrl + _current_language + "/Ticket/Detil/" + model.ticket_number);

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Revisi Data Tiket" : "Ticket Data Revision"), email_content);
                }

            }
            if ( result )
            {
                return RedirectToAction("Index", "Ticket");
            } else
            {
                // ViewBag.error_message = error_message;
                // ViewBag.icon_message = icon_message;
                // ViewBag.alert_message = alert_message;
                ViewBag.ticket_data = model;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = HttpContext.Session.GetString("role_id");
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number, error_message = error_message, alert_message = alert_message, icon_message = icon_message });
            }
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Reject(CreateTicketModel model)
        {
            if (!_permission.allow_reject)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);
            string role_user = HttpContext.Session.GetString("role_id");
            
            int verification_status_id = 15;
            int position_data = 2;
            int is_finish = 0;

            if (role_user == "3")
            {
                verification_status_id = 7;
                position_data = 2;
                is_finish = 1;
            }
            else if (role_user == "5")
            {
                verification_status_id = 8;
                position_data = 2;
                is_finish = 1;
            }
            else if (role_user == "6")
            {
                verification_status_id = 19;
                position_data = 5;
            }
            else if (role_user == "8")
            {
                verification_status_id = 15;
                position_data = 9;
                is_finish = 1;
            }
            else
            {
                return RedirectToAction("Index", "Ticket");
            }

            model.verification_status_id = verification_status_id;
            model.position_data = position_data;
            model.request_simulate = 0;
            model.is_finish = is_finish;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;
            
            if(role_user == "3" || role_user == "8") {
                List<String> qty_billed = new List<String>();
                List<String> vendor_origin = new List<String>();
                List<String> po_item_number = new List<String>();
                List<String> gr_amount = new List<String>();
                List<String> document_no = new List<String>();
                List<String> item_1 = new List<String>();
                foreach (var item in DataTicket.table_po)
                {
                    qty_billed.Add(item.qty_billed.ToString());
                    vendor_origin.Add(item.vendor_origin);
                    po_item_number.Add(item.po_item_number);
                    gr_amount.Add(item.gr_amount.ToString());
                    document_no.Add(item.document_no);
                    item_1.Add(item.item_1);
                }
                model.po_number = DataTicket.po_number;
                model.gr_number = DataTicket.gr_number;
                model.company_id = DataTicket.company_id;
                model.qty_billed = qty_billed.ToArray();
                model.vendor_origin = vendor_origin.ToArray();
                model.document_no = document_no.ToArray();
                model.item_1 = item_1.ToArray();
                model.po_type_id = DataTicket.po_type_id;
                model.po_item_number = po_item_number.ToArray();
                model.gr_amount = gr_amount.ToArray();
            }

            String error_message = "";
            String icon_message = "";
            String alert_message = "";
            bool saveUpdate = false;
            if(role_user == "3") {
                saveUpdate = _ticket.updateTicketVerificationStatus(DataTicket.ticket_number, verification_status_id, null);
                if(saveUpdate) {
                    _ticket.updateTicketPositionData(DataTicket.ticket_number, position_data);
                    Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                    condition.Add("is_used", 0);
                    bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model, condition);
                    //Insert Log
                    LogTicketModel data_insert = new LogTicketModel()
                    {
                        ticket_number = model.ticket_number,
                        process = "Reject Ticket",
                        note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    _ticket.InsertLog(data_insert);
                    result = true;
                }
            }
            else if(role_user == "8") {
                saveUpdate = _ticket.updateTicketVerificationStatus(DataTicket.ticket_number, verification_status_id, model.verification_note);
                if(saveUpdate) {
                    _ticket.updateTicketPositionData(DataTicket.ticket_number, position_data);
                    bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);
                    // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                    // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);

                    // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                    //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                    //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
                    // }
                    //Insert Log
                    LogTicketModel data_insert = new LogTicketModel()
                    {
                        ticket_number = model.ticket_number,
                        process = "Reject Ticket",
                        note = model.verification_note,
                        created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                    };
                    result = true;
                }
            } else {
                if (ModelState.IsValid || String.IsNullOrEmpty(model.gl_amount.ToString()) || !String.IsNullOrEmpty(model.material_amt.ToString()))
                {
                    string upload_basedir = "document/ticket/";
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

                    string file_invoice_name = null;
                    string file_invoice_path = null;
                    if (model.file_invoice != null && model.file_invoice.Length != 0)
                    {
                        file_invoice_name = "invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_invoice = Path.Combine(upload_directory, file_invoice_name);
                        using (var stream = System.IO.File.Create(path_file_invoice))
                        {
                            await model.file_invoice.CopyToAsync(stream);
                            file_invoice_path = upload_basedir + file_invoice_name;
                        }
                    }

                    string file_po_name = null;
                    string file_po_path = null;
                    if (model.file_po != null && model.file_po.Length != 0)
                    {
                        file_po_name = "po-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_po = Path.Combine(upload_directory, file_po_name);
                        using (var stream = System.IO.File.Create(path_file_po))
                        {
                            await model.file_po.CopyToAsync(stream);
                            file_po_path = upload_basedir + file_po_name;
                        }
                    }
                    
                    string file_lpb_gr_name = null;
                    string file_lpb_gr_path = null;
                    if (model.file_lpb_gr != null && model.file_lpb_gr.Length != 0)
                    {
                        file_lpb_gr_name = "lpb_gr-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_lpb_gr = Path.Combine(upload_directory, file_lpb_gr_name);
                        using (var stream = System.IO.File.Create(path_file_lpb_gr))
                        {
                            await model.file_lpb_gr.CopyToAsync(stream);
                            file_lpb_gr_path = upload_basedir + file_lpb_gr_name;
                        }
                    }
                    
                    string file_tax_invoice_name = null;
                    string file_tax_invoice_path = null;
                    if (model.file_tax_invoice != null && model.file_tax_invoice.Length != 0)
                    {
                        file_tax_invoice_name = "tax_invoice-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_tax_invoice = Path.Combine(upload_directory, file_tax_invoice_name);
                        using (var stream = System.IO.File.Create(path_file_tax_invoice))
                        {
                            await model.file_tax_invoice.CopyToAsync(stream);
                            file_tax_invoice_path = upload_basedir + file_tax_invoice_name;
                        }
                    }
                    
                    string file_delivery_note_name = null;
                    string file_delivery_note_path = null;
                    if (model.file_delivery_note != null && model.file_delivery_note.Length != 0)
                    {
                        file_delivery_note_name = "delivery_note-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_delivery_note = Path.Combine(upload_directory, file_delivery_note_name);
                        using (var stream = System.IO.File.Create(path_file_delivery_note))
                        {
                            await model.file_delivery_note.CopyToAsync(stream);
                            file_delivery_note_path = upload_basedir + file_delivery_note_name;
                        }
                    }
                    
                    string file_bast_name = null;
                    string file_bast_path = null;
                    if (model.file_bast != null && model.file_bast.Length != 0)
                    {
                        file_bast_name = "bast-" + Path.GetRandomFileName().Replace(".", string.Empty) + ".pdf";
                        var path_file_bast = Path.Combine(upload_directory, file_bast_name);
                        using (var stream = System.IO.File.Create(path_file_bast))
                        {
                            await model.file_bast.CopyToAsync(stream);
                            file_bast_path = upload_basedir + file_bast_name;
                        }
                    }
                    
                    saveUpdate = _ticket.updateTicket(model.ticket_number, model, file_invoice_path, file_po_path, file_lpb_gr_path, file_tax_invoice_path, file_delivery_note_path, file_bast_path);

                    if ( saveUpdate )
                    {
                        //Delete Old File
                        if (!string.IsNullOrEmpty(file_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_invoice);
                        }
                        if (!string.IsNullOrEmpty(file_lpb_gr_name) && !string.IsNullOrEmpty(DataTicket.file_lpb_gr) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_lpb_gr);
                        }
                        if (!string.IsNullOrEmpty(file_tax_invoice_name) && !string.IsNullOrEmpty(DataTicket.file_tax_invoice) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_tax_invoice);
                        }
                        if (!string.IsNullOrEmpty(file_delivery_note_name) && !string.IsNullOrEmpty(DataTicket.file_delivery_note) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_delivery_note);
                        }
                        if (!string.IsNullOrEmpty(file_bast_name) && !string.IsNullOrEmpty(DataTicket.file_bast) && System.IO.File.Exists(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast))
                        {
                            System.IO.File.Delete(_iHostingEnvironment.WebRootPath + "/" + DataTicket.file_bast);
                        }
                        //End delete old file

                        //Delete Old PO tiket
                        _ticket.DeletePoTicket(model.ticket_number);
                        //Insert input PO tiket
                        _ticket.InsertPoTicket(model, model.ticket_number);
                        if (role_user != "8") {
                            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                            condition.Add("is_used", 0);
                            bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model, condition);
                        }
                        // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
                        // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, false);

                        // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                        //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
                        //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, false);
                        // }
                        //Insert Log
                        LogTicketModel data_insert = new LogTicketModel()
                        {
                            ticket_number = model.ticket_number,
                            process = "Reject Ticket",
                            note = model.verification_note,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        result = true;
                    }
                    else
                    {
                        error_message = _localizer.GetString("FailedToSave");
                        icon_message = "flaticon-circle";
                        alert_message = "alert-danger";
                    }
                }
            }

            if (role_user == "3" && result)
            {
                //Procurement
                List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_rejection-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                registered_email.Add(company_email);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("ticket_number", model.ticket_number);
                    email_content_param.Add("note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("rejected_by", "Procurement");

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Tiket Ditolak" : "Ticket Rejected"), email_content);
                }
            }
            else if (role_user == "5" && result)
            {
                //FC
                var pic = _master.GetProcurementGroupByID(DataTicket.pic_id);
                List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;

                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_rejection-" + _current_language + ".html";
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
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                recipient_email.Add(company_email);

                foreach (string recipient in recipient_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("ticket_number", DataTicket.ticket_number);
                    email_content_param.Add("note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("rejected_by", "FC");

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Tiket Ditolak" : "Ticket Rejected"), email_content);
                }
            }
            else if (role_user == "8" && result)
            {
                //Tax
                List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/ticket_rejection-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                // email company
                var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                registered_email.Add(company_email);
                foreach (string recipient in registered_email)
                {
                    Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                    email_content_param.Add("email", recipient);
                    email_content_param.Add("ticket_number", DataTicket.ticket_number);
                    email_content_param.Add("note", model.verification_note);
                    email_content_param.Add("year", DateTime.Now.Year.ToString());
                    email_content_param.Add("rejected_by", "Tax");

                    String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
                    new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Tiket Ditolak" : "Ticket Rejected"), email_content);
                }
            }
            
            if (result)
            {
                return RedirectToAction("Index", "Ticket");
            }
            else
            {
                // ViewBag.error_message = error_message;
                // ViewBag.icon_message = icon_message;
                // ViewBag.alert_message = alert_message;
                ViewBag.ticket_number = model;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = HttpContext.Session.GetString("role_id");
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                if(_role_id == "8") {
                    ViewBag.ticket_number = DataTicket;
                    return RedirectToAction("Index", "Ticket");
                    // return View("Detil");
                } else {
                    return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number, error_message = error_message, alert_message = alert_message, icon_message = icon_message });
                }
            }
        }

        [HttpPost, ActionName("RequestSimulate")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> RequestSimulate(CreateTicketModel model) {
            if (!_permission.allow_approve)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            bool result = false;
            var DataTicket = _ticket.GetTicketDetail(model.ticket_number);
            string role_user = HttpContext.Session.GetString("role_id");
            
            String error_message = "";

            model.verification_status_id = DataTicket.verification_status_id;
            model.position_data = DataTicket.position_data;
            model.request_simulate = 0;
            model.is_finish = DataTicket.is_finish == false ? 0 : 1;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.request_simulate = 1;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;

            bool saveUpdate = _ticket.updateTicket(model.ticket_number, model);
            //Delete Old PO
            _ticket.DeletePoTicket(model.ticket_number);
            //Insert input PO
            _ticket.InsertPoTicket(model, model.ticket_number);

            _ticket.DeleteGlTicket(model.ticket_number);
            //Insert input GL
            _ticket.InsertGlTicket(model, model.ticket_number);

             _ticket.DeleteMaterialTicket(model.ticket_number);
            //Insert input Material
            _ticket.InsertMaterialTicket(model, model.ticket_number);
            bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);

            // bool update_old_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.old_po_number, model.old_po_item_number, model.vendor_number, false);
            // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);

            // if(!String.IsNullOrEmpty(model.old_gr_number)) {
            //     _ticket.UpdateIsUsedLpbTransaction(model.old_po_number, model.old_gr_number, model.old_po_item_number, model.vendor_number, false);
            //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
            // }

            result = true;

            if (result)
            {
                return RedirectToAction("Index", "Ticket");
            }
            else
            {
                // ViewBag.error_message = error_message;
                ViewBag.ticket_data = DataTicket;
                ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                ViewBag.BaseUrl = _appSettings.BaseUrl;
                ViewBag.language = _current_language;
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
                ViewData["name"] = HttpContext.Session.GetString("name");
                ViewData["idRole"] = HttpContext.Session.GetString("role_id");
                ViewBag.Menu = _list_menu;
                ViewBag.Permission = _permission;
                return RedirectToAction("Detil", "Ticket", new { id = DataTicket.ticket_number });
            }
        }
    }
}