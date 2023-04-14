using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using tufol.Models;
using tufol.Interfaces;
using tufol.Helpers;

namespace tufol.Controllers
{
  // public class DashboardController : BaseController
  public class DashboardController : Controller
  {
        private IMenu _menu;
        private IDashboard _dashboard;
        private IVendor _vendor;
        private readonly IEnumerable<MenuModel> _list_menu;
        private string _role_id;
        public DashboardController(IMenu menu, IDashboard dashboard, IVendor vendor, IHttpContextAccessor httpContextAccessor)
        // public DashboardController(IMenu menu, IUser user, IHttpContextAccessor httpContextAccessor) : base(user, menu, httpContextAccessor)
        {
            _menu = menu;
            _dashboard = dashboard;
            _vendor = vendor;
            _role_id = httpContextAccessor.HttpContext.Session.GetString("role_id");
            _list_menu = _menu.GetMenu(_role_id);
        }

        [Route("{culture}/{controller}")]
        [HttpGet, ActionName("index")]
        public IActionResult Index()
        {

            if (HttpContext.Session.Get("username") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            
            ViewBag.Menu = _list_menu;
            ViewBag.role_id = _role_id;
            ViewBag.VendorNumber = HttpContext.Session.GetString("vendor_number");
            ViewBag.active_vendor = 0;
            ViewBag.pending_vendor = 0;
            ViewBag.revised_vendor = 0;
            ViewBag.rejected_vendor_by_fc = 0;
            ViewBag.draft_ticket = 0;
            ViewBag.sent_ticket = 0;
            ViewBag.rejected_by_tax_ticket = 0;
            ViewBag.received_ticket = 0;
            ViewBag.pending_ticket = 0;
            ViewBag.history_ticket = 0;
            ViewBag.paid_ticket = 0;
            ViewBag.approve_payment_ticket = 0;
            ViewBag.require_reversal_ticket = 0;
            ViewBag.reversal_complete_ticket = 0;
            ViewBag.simulate_ticket = 0;
            ViewBag.error_ticket = 0;
            ViewBag.confirm_proc_ticket = 0;
            ViewBag.reject_ticket = 0;
            // vendor
            if(_role_id == "1" || _role_id == "3" || _role_id == "5") {
                ViewBag.active_vendor = _dashboard.CountVendor(ViewBag.VendorNumber);
                ViewBag.pending_vendor = _dashboard.CountTempVendor(Convert.ToInt32(ViewBag.role_id), "New Registration");
                if(_role_id == "3")
                    ViewBag.rejected_vendor_by_fc = _dashboard.CountTempVendor(2, "New Registration", 8);
                if(_role_id == "5")
                    ViewBag.revised_vendor = _dashboard.CountTempVendor(2, "New Registration", 2);
            }

            if(_role_id == "2") {
                // draft ticket
                Dictionary<string, dynamic> condition_draft = new Dictionary<string, dynamic>();
                condition_draft.Add("verification_status_id", 9);
                ViewBag.draft_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_draft, ViewBag.VendorNumber);

                // confirm to procurement
                ViewBag.confirm_proc_ticket = _dashboard.CountTicketConfirmToProcurement(ViewBag.VendorNumber);

                // reject ticket
                Dictionary<string, dynamic> condition_reject = new Dictionary<string, dynamic>();
                condition_reject.Add("label_for_vendor", "Rejected");
                ViewBag.reject_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_reject, ViewBag.VendorNumber);
            }

            if(_role_id == "1" || _role_id == "2" || _role_id == "6" || _role_id == "7") {
                if(_role_id != "6" || _role_id != "7") {
                    // sent ticket
                    Dictionary<string, dynamic> condition_sent = new Dictionary<string, dynamic>();
                    condition_sent.Add("verification_status_id", 10);
                    ViewBag.sent_ticket = _dashboard.CountTicket(0, condition_sent, ViewBag.VendorNumber);

                    // received ticket
                    Dictionary<string, dynamic> condition_received = new Dictionary<string, dynamic>();
                    condition_received.Add("verification_status_id", 6);
                    condition_received.Add("verification_status", 11);
                    if(_role_id == "1") {
                        ViewBag.received_ticket = _dashboard.CountTicket(0, condition_received, ViewBag.VendorNumber);
                    } else {
                        ViewBag.received_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_received, ViewBag.VendorNumber);
                    }
                }

                // ticket payment approve / waiting for payment
                Dictionary<string, dynamic> condition_ticket_approve_payment = new Dictionary<string, dynamic>();
                condition_ticket_approve_payment.Add("remmitance_date", "");
                condition_ticket_approve_payment.Add("miro_number !=", "");
                ViewBag.approve_payment_ticket = _dashboard.CountTicket(0, condition_ticket_approve_payment, ViewBag.VendorNumber);
            }
            
            if(_role_id == "1") {
                // rejected by tax
                Dictionary<string, dynamic> condition_rejected_by_tax = new Dictionary<string, dynamic>();
                condition_rejected_by_tax.Add("verification_status_id", 15);
                ViewBag.rejected_by_tax_ticket = _dashboard.CountTicket(0, condition_rejected_by_tax, ViewBag.VendorNumber);
            }
            
            if(_role_id == "3" || _role_id == "4" || _role_id == "5" || _role_id == "6") {
                // pending ticket
                Dictionary<string, dynamic> condition_pending = new Dictionary<string, dynamic>();
                if(_role_id == "5") {
                    condition_pending.Add("verification_status_id", 6);
                    condition_pending.Add("verification_status", 11);
                } else if ( _role_id == "6" ) {
                    condition_pending.Add("verification_status_id", 5);
                    condition_pending.Add("status_rpa !=", 0);
                    condition_pending.Add("request_simulate", 0);
                }
                ViewBag.pending_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_pending);
                
                if(_role_id != "8") {
                    // ticket history
                    Dictionary<string, dynamic> condition_ticket_history = new Dictionary<string, dynamic>();
                    condition_ticket_history.Add("is_finish", 1);
                    ViewBag.history_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_ticket_history, ViewBag.VendorNumber);
                }
            }

            if(_role_id == "2" || _role_id == "6" || _role_id == "7" || _role_id == "8") {
                // paid ticket
                Dictionary<string, dynamic> condition_ticket_paid = new Dictionary<string, dynamic>();
                condition_ticket_paid.Add("is_paid", 1);
                if(_role_id == "2") 
                    condition_ticket_paid.Add("verification_status_id !=", 15);
                if(_role_id == "8")
                    condition_ticket_paid.Add("remmitance_number !=", "");
                ViewBag.paid_ticket = _dashboard.CountTicket(0, condition_ticket_paid, ViewBag.VendorNumber);
            }
            
            if(_role_id == "6" || _role_id == "8") {
                // require reversal
                Dictionary<string, dynamic> condition_require_reversal = new Dictionary<string, dynamic>();
                condition_require_reversal.Add("verification_status_id", 15);
                ViewBag.require_reversal_ticket = _dashboard.CountTicket(9, condition_require_reversal, ViewBag.VendorNumber);
            }

            if(_role_id == "6") {
                // for_simulate
                Dictionary<string, dynamic> condition_simulate = new Dictionary<string, dynamic>();
                condition_simulate.Add("request_simulate", 1);
                ViewBag.simulate_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_simulate, ViewBag.VendorNumber);
                
                // error ticket
                Dictionary<string, dynamic> condition_error = new Dictionary<string, dynamic>();
                condition_error.Add("status_rpa", 0);
                ViewBag.error_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_error, ViewBag.VendorNumber);
            }
            
            if(_role_id == "8") {
                // reversal complete
                Dictionary<string, dynamic> condition_reversal_complete = new Dictionary<string, dynamic>();
                condition_reversal_complete.Add("verification_status_id", 18);
                ViewBag.reversal_complete_ticket = _dashboard.CountTicket(Convert.ToInt32(ViewBag.role_id), condition_reversal_complete, ViewBag.VendorNumber);
            }
            
            

            if(!String.IsNullOrEmpty(ViewBag.VendorNumber)) {
                var data = _vendor.GetVendorDetail(ViewBag.VendorNumber);
                ViewBag.EditVendor = data.change_request_status == 0 ? false : true;
            }
            return View();
        }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
