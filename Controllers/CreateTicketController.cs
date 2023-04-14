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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace tufol.Controllers
{
    public class CreateTicketController : Controller
    {
        private IMenu _menu;
        private IUser _user;
        private ITicket _ticket;
        private IVendor _vendor;
        private IMaster _master;
        private readonly IStringLocalizer<CreateTicketController> _localizer;
        private readonly PermissionModel _permission;
        private String _current_language;
        private IWebHostEnvironment _iHostingEnvironment;
        private readonly IEnumerable<MenuModel> _list_menu;
        private string _role_id;
        private readonly Helpers.AppSettings _appSettings;
        public CreateTicketController(IMenu menu, IUser user, ITicket ticket, IVendor vendor, IMaster master, IStringLocalizer<CreateTicketController> localizer, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment iHostingEnvironment, IOptions<Helpers.AppSettings> appSettings)
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
            _list_menu = _menu.GetMenu(httpContextAccessor.HttpContext.Session.GetString("role_id"));
            _permission = _menu.GetPermission("CreateTicket", httpContextAccessor.HttpContext.Session.GetString("role_id"));
            _appSettings = appSettings.Value;
            ViewBag.BaseUrl = _appSettings.BaseUrl;
        }
        [HttpGet]
        public IActionResult Index()
        {
            if (!_permission.allow_create)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            // popup
            var pop_up = _master.GetAnnouncementDetail("popup_ticket");
            ViewBag.pop_up = pop_up;

            ViewBag.Menu = _list_menu;
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            var vendor_number = HttpContext.Session.GetString("vendor_number");
            ViewBag.role_id = _role_id;
            var DataVendor = _ticket.GetVendorForTicket(vendor_number);  
            Console.WriteLine("datavendor locked:" + DataVendor.is_locked);
            ViewBag.ticket_data = DataVendor;
            ViewBag.area_code = "";
            ViewBag.error_message = "";
            ViewBag.alert_message = "";
            ViewBag.icon_message = "";
            var area_mainlaind = "Mainland";
            var area_zanzibar = "Zanzibar";
            if(!String.IsNullOrEmpty(DataVendor.additional_street_address)) {
                if(Regex.IsMatch(DataVendor.additional_street_address, area_mainlaind)){
                    ViewBag.area_code = "Mainland";
                } else if(Regex.IsMatch(DataVendor.additional_street_address, area_zanzibar)) {
                    ViewBag.area_code = "Zanzibar";
                } else {
                    ViewBag.area_code = "";
                }
            } else {
                ViewBag.area_code = "";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> Index(CreateTicketModel model, string action)
        {
            ViewBag.controller = ControllerContext.ActionDescriptor.ControllerName;
            var vendor_number = HttpContext.Session.GetString("vendor_number");
            var DataVendor = _vendor.GetVendorDetail(vendor_number);  
            String error_message = "";
            String alert_message = "";
            String icon_message = "";
            Console.WriteLine(action + " action");

            // Ticket Number
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.ToString("MM");
            string ticket_number_max = _ticket.GetMaxTicketNumber(year, month, model.area_code);
            string[] ticket_number_split = ticket_number_max.Split('-');
            string max_ticket_number = String.IsNullOrEmpty(ticket_number_max.ToString()) ? "0" : ticket_number_split[1].ToString();
            int plus_one = Convert.ToInt32(max_ticket_number) + 1;
            string increment = plus_one.ToString().PadLeft(4, '0');
            bool is_failed = true;
            model.ticket_number = model.area_code + year + month + "-" + increment;

            if(action == "Save") {
                model.position_data = 2;
                model.verification_status_id = 9;
            } else {
                model.verification_status_id = 10;
                if(model.dpp > model.dpp_original || model.dpp != model.dpp_original) {
                    model.position_data = 3;
                } else {
                    if(model.area_code == "8230" || model.area_code == "8520") {
                        model.position_data = 4;
                    } else {
                        model.position_data = 5; //ini jadinya ke FC langsung 5 (procurement skip dl)
                    }
                }
            }

            model.request_simulate = 0;
            model.is_finish = 0;
            model.old_po_number = model.po_number;
            model.old_gr_number = model.gr_number;
            model.old_po_item_number = model.po_item_number;

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

                string ticket_number = _ticket.InsertTicket(model, file_invoice_path, file_po_path, file_lpb_gr_path, file_tax_invoice_path, file_delivery_note_path, file_bast_path);
                if(!String.IsNullOrEmpty(ticket_number)) {
                    bool po_ticket = _ticket.InsertPoTicket(model, ticket_number);
                    bool update_po_transaction = _ticket.UpdateIsUsedPoTransaction(model);
                    // bool update_po_transaction = _ticket.UpdateIsUsedSvcTransaction(model.po_number, model.po_item_number, model.vendor_number, true);
                    bool update_mailing_data = _vendor.updateMailingData(vendor_number, model.mailing_address, model.district, model.village, model.province);
                    
                    // if(!String.IsNullOrEmpty(model.old_gr_number)) {
                    //     Console.WriteLine("update po lpb");
                    //     _ticket.UpdateIsUsedLpbTransaction(model.po_number, model.gr_number, model.po_item_number, model.vendor_number, true);
                    // }

                    // create QR code
                    if (!Directory.Exists(_iHostingEnvironment.WebRootPath + "/document/barcode_ticket/"))
                    {
                        Directory.CreateDirectory(_iHostingEnvironment.WebRootPath + "/document/barcode_ticket/");
                    }
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(ticket_number, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    using(var stream = System.IO.File.Create(Path.Combine(_iHostingEnvironment.WebRootPath + "/document/barcode_ticket/", ticket_number + ".png")))
                    {
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        qrCodeImage.Save(stream, ImageFormat.Png);
                    }

                    if(po_ticket && update_po_transaction && update_mailing_data && action == "Send") {
                        //Insert Log
                        LogTicketModel data_insert = new LogTicketModel()
                        {
                            ticket_number = ticket_number,
                            process = "Insert Ticket",
                            note = null,
                            created_by = Convert.ToInt32(HttpContext.Session.GetString("user_id"))
                        };
                        _ticket.InsertLog(data_insert);

                        List<String> registered_email = DataVendor.email_list == null ? new List<String>() : DataVendor.email_list;
                        String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/create_ticket_success-" + _current_language + ".html";
                        String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                        // email company
                        var company_email = _ticket.GetEmailCompanyArea(model.area_code);
                        registered_email.Add(company_email);
                        foreach (string recipient in registered_email)
                        {
                            Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                            email_content_param.Add("vendor_name", DataVendor.name);
                            email_content_param.Add("ticket_number", ticket_number);
                            email_content_param.Add("email", recipient);
                            email_content_param.Add("year", DateTime.Now.Year.ToString());
                            
                            String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                            new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Buat Tiket Berhasil" : "Create Ticket Success"), email_content);
                        }
                    }
                    is_failed = false;
                    error_message = _localizer.GetString("SuccessToSave");
                    icon_message = "far fa-check-circle";
                    alert_message = "alert-success";
                } 
            } 
            if (is_failed) {
                error_message = _localizer.GetString("FailedToSave");
                icon_message = "flaticon-circle";
                alert_message = "alert-danger";
            }
            var DataTicket = _ticket.GetVendorForTicket(vendor_number);  
            ViewBag.ticket_data = DataTicket;
            // popup
            var pop_up = _master.GetAnnouncementDetail("popup_ticket");
            ViewBag.pop_up = pop_up;
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            ViewBag.error_message = error_message;
            ViewBag.alert_message = alert_message;
            ViewBag.icon_message = icon_message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<IActionResult> SendEmailChat(string name, string email, string phone, string area_code, string message)
        {
            var vendor_number = HttpContext.Session.GetString("vendor_number");
            var DataTicket = _ticket.GetVendorForTicket(vendor_number); 
            var pop_up = _master.GetAnnouncementDetail("popup_ticket");
            Console.WriteLine("areaCode"+ area_code);
            if (!string.IsNullOrEmpty(area_code)){     
                var Email = _ticket.GetEmailCompanyArea(area_code);
                Console.WriteLine("email"+ Email);
                String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/vendor_chat-" + _current_language + ".html";
                String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
            
                        Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                        email_content_param.Add("name", name);
                        email_content_param.Add("email", email);
                        email_content_param.Add("telephone", phone);
                        email_content_param.Add("message", message);
                        email_content_param.Add("year", DateTime.Now.Year.ToString());
                                    
                        String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                        new Helpers.EmailFunction().sending_email(Email, (_current_language == "id" ? "Pesan Vendor" : "Vendor Chat"), email_content);

                ViewBag.ticket_data = DataTicket;
                // popup
                ViewBag.pop_up = pop_up;
                ViewBag.Menu = _list_menu;
                ViewBag.role_id = _role_id;
                ViewBag.send_success = "Sending!";
                return View("~/Views/CreateTicket/Index.cshtml");
            }
            ViewBag.ticket_data = DataTicket;
            // popup
            ViewBag.pop_up = pop_up;
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            ViewBag.error = "not Sending!, Must be Choice Location";
            return View("~/Views/CreateTicket/Index.cshtml");
        }
    }
}
