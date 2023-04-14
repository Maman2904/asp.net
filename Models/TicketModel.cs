using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tufol.Models
{
	public class TicketModel
	{
        public string ticket_number { get; set; }
        public bool? is_finish { get; set; }
        public string vendor_number { get; set; }
        public string? miro_number { get; set; }
        public string name { get; set; }
        public string npwp { get; set; }
        public string contact_person { get; set; }
        public string email { get; set; }
        public List<String> email_list { get; set; }
        public string pic_id { get; set; }
        public string country_vendor { get; set; }
        public string? additional_street_address { get; set; }
        public string? localidr_bank_id { get; set; }
        public string? localidr_bank_name { get; set; }
        public string? localidr_swift_code { get; set; }
        public string? localidr_account_holder { get; set; }
        public decimal? localidr_bank_account { get; set; }
        public string? localforex_bank_id { get; set; }
        public string? localforex_bank_name { get; set; }
        public string? localforex_swift_code { get; set; }
        public string? localforex_account_holder { get; set; }
        public decimal? localforex_bank_account { get; set; }
        public string? foreign_bank_id { get; set; }
        public string? foreign_bank_name { get; set; }
        public string? foreign_account_holder { get; set; }
        public string? foreign_bank_account { get; set; }
        public int? top_interval { get; set; }
        public int? top_duration { get; set; }
        public string? currency_id { get; set; }
        public int? tax_type_id { get; set; }
        public string? tax_type_name { get; set; }
        public string country_id { get; set; }
        public string mailing_address { get; set; }
        public string district { get; set; }
        public string village { get; set; }
        public string province { get; set; }
        public string phone_number { get; set; }
        public string company_id { get; set; }
        public string? company_name { get; set; }
        public string area_code { get; set; }
        public string? location_name { get; set; }
        public string sap_code { get; set; }
        public string top_id { get; set; }
        public string? baseline_date { get; set; }
        public int tax_status_id { get; set; }
        public string? tax_status_name { get; set; }
        public string? tax_invoice_number { get; set; }
        public DateTime? tax_invoice_date { get; set; }
        public string invoice_number { get; set; }
        public DateTime? invoice_date { get; set; }
        public DateTime? received_date { get; set; }
        public DateTime? submitted_date { get; set; }
        public DateTime? posting_date { get; set; }
        public string? remmitance_number { get; set; }
        public DateTime? remmitance_date { get; set; }
        public string? awb_mail { get; set; }
        public int po_type_group_id { get; set; }
        public int po_type_id { get; set; }
        public string po_type_name { get; set; }
        public string po_number { get; set; }
        public string? gr_number { get; set; }
        public double invoice_amount { get; set; }
        public string invoice_currency { get; set; }
        public IEnumerable<PoTicketModel> table_po { get; set; }
        public IEnumerable<GlTicketModel>? table_gl { get; set; }
        public IEnumerable<MaterialTicketModel>? table_material { get; set; }
        public double dpp_original { get; set; }
        public double dpp { get; set; }
        public double ppn { get; set; }
        public double pph { get; set; }
        public double total_price { get; set; }
        public string file_invoice { get; set; }
        public string file_po { get; set; }
        public string? file_lpb_gr { get; set; }
        public string file_tax_invoice { get; set; }
        public string? file_delivery_note { get; set; }
        public string? file_bast { get; set; }
        public bool? is_paid { get; set; }
        public int verification_status_id { get; set; }
        public string? verification_name { get; set; }
        public string? status_for_intern { get; set; }
        public string? status_for_vendor { get; set; }
        public string? verification_note { get; set; }
        public int position_data { get; set; }
        public bool? is_locked { get; set; }
        public int type_of_payment { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? request_simulate { get; set; }
        public double? tax_amt_fc { get; set; }
        public string? status_rpa { get; set; }
        public string? rpa_status_description {get; set;}
        public IEnumerable<LogTicketModel> log_ticket { get; set; }
    }


    public class UpdateMIRONumberPayloadBody{
        public string? status_rpa { get; set; }
        public string? miro_number { get; set; }
        public DateTime? date_miro_input { get; set; }
        public string? rpa_status_description {get; set;}
    }

}