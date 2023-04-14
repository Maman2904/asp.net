using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using tufol.Models;
using tufol.Interfaces;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace tufol.Controllers
{
    public class ChangeTicketController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private ITicket _ticket;
        private IVendor _vendor;
        private IMaster _master;
        private readonly PermissionModel _permission;
        private readonly IStringLocalizer<ChangeTicketController> _localizer;
        private String _current_language;
        private IWebHostEnvironment _iHostingEnvironment;
        private readonly IEnumerable<MenuModel> _list_menu;
        private string _role_id;
        // private readonly Helpers.AppSettings _appSettings;        
        public ChangeTicketController(IMenu menu, IUser user, ITicket ticket, IVendor vendor, IMaster master, IStringLocalizer<ChangeTicketController> localizer, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment iHostingEnvironment, IOptions<Helpers.AppSettings> appSettings)
        {
            _menu = menu;
            _user = user;
            _ticket = ticket;
            _vendor = vendor;
            _master = master;
            _localizer = localizer;
            _iHostingEnvironment = iHostingEnvironment;
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
            _role_id = httpContextAccessor.HttpContext.Session.GetString("role_id");
            _list_menu = _menu.GetMenu(_role_id);
            _permission = _menu.GetPermission("Ticket", _role_id);
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

        [Route("/{culture}/{controller}/Detil/{ticket_number}")]
        public async Task<IActionResult> Detil(string ticket_number)
        {
            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            ViewBag.role_id = _role_id;
            ViewBag.error_message = "";
            ViewBag.alert_message = "";
            ViewBag.icon_message = "";
            var DataTicket = _ticket.GetTicketDetail(ticket_number);  
            var DataVendor = _ticket.GetVendorForTicket(DataTicket.vendor_number);  
            ViewBag.ticket_data = DataTicket;
            ViewBag.vendor_data = DataVendor;
            if (string.IsNullOrEmpty(DataTicket.ticket_number))
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("~/Views/Shared/NotFound.cshtml");
            }
            bool isReadOnly = false;
            if(DataTicket.position_data != 2 && !String.IsNullOrEmpty(DataTicket.remmitance_number)) {
                isReadOnly = true;
            }
            if(isReadOnly) {
                return View();
            } else {
                return View("Form");
            }
        }
        public IActionResult PrintBarcode(string ticket_number)
        {
            if (!_permission.allow_read)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var role = HttpContext.Session.GetString("role_id");
            ViewBag.Menu = _list_menu;
            var DataTicket = _ticket.GetTicketDetail(ticket_number);
            ViewBag.ticket_data = DataTicket;
            return View();
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
            // foreach (var modelState in ViewData.ModelState.Values) {
            //     foreach (var error in modelState.Errors) {
            //         Console.WriteLine(error.ErrorMessage);
            //     }
            // }

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
                    return RedirectToAction("Index", "ChangeTicket");
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
            ViewBag.error_message = error_message;
            ViewBag.alert_message = alert_message;
            ViewBag.icon_message = icon_message;
            return RedirectToAction("Detil", "ChangeTicket", new { culture = _current_language, controller = ControllerContext.ActionDescriptor.ControllerName, action = "Detil", id = DataTicket.ticket_number });
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

            model.request_simulate = 0;
            model.is_finish = 0;
            model.verification_status_id = 10;
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
            model.old_po_number = DataTicket.po_number;
            model.old_gr_number = DataTicket.gr_number;
            model.verification_note = DataTicket.verification_note == null ? null : DataTicket.verification_note;

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
                            process = "Insert Ticket",
                            note = null,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        _ticket.InsertLog(data_insert);

                        List<String> registered_email = DataTicket.email_list == null ? new List<String>() : DataTicket.email_list;
                        String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/create_ticket_success-" + _current_language + ".html";
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

                            new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Buat Tiket Berhasil" : "Create Ticket Success"), email_content);
                        }
                    }
                    error_message = _localizer.GetString("SuccessToSave");
                    icon_message = "far fa-check-circle";
                    alert_message = "alert-success";
                    ViewBag.Menu = _list_menu;
                    ViewBag.role_id = _role_id;
                    ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
                    ViewBag.error_message = error_message;
                    ViewBag.alert_message = alert_message;
                    ViewBag.icon_message = icon_message;
                    return RedirectToAction("Index", "ChangeTicket");
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
            ViewBag.error_message = error_message;
            ViewBag.alert_message = alert_message;
            ViewBag.icon_message = icon_message;
            return RedirectToAction("Detil", "ChangeTicket", new { culture = _current_language, controller = ControllerContext.ActionDescriptor.ControllerName, action = "Detil", id = DataTicket.ticket_number });
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
            model.table_po = DataTicket.table_po;
            model.top_interval = DataTicket.top_interval;
            model.tax_type_id = DataTicket.tax_type_id;
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
                    ViewBag.error_message = error_message;
                    ViewBag.alert_message = alert_message;
                    ViewBag.icon_message = icon_message;
                    return RedirectToAction("Index", "ChangeTicket");
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
            ViewBag.error_message = error_message;
            ViewBag.alert_message = alert_message;
            ViewBag.icon_message = icon_message;
            return RedirectToAction("Detil", "ChangeTicket", new { culture = _current_language, controller = ControllerContext.ActionDescriptor.ControllerName, action = "Detil", id = DataTicket.ticket_number });
        }
    }
}