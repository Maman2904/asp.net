using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tufol.Models
{
	public class CreateTicketModel
	{
        [Required(ErrorMessage = "Account number is required")]
        public string account_number { get; set; }
        public string ticket_number { get; set; }
        public string name { get; set; }
        public string? npwp { get; set; }
        public string contact_person { get; set; }
        public string email { get; set; }
        public string? miro_number { get; set; }
        public string vendor_number { get; set; }
        public string? country_vendor { get; set; }
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
        [Required(ErrorMessage = "Country is required")]
        public string country_id { get; set; }
        [Required(ErrorMessage = "Mailing address is required")]
        public string mailing_address { get; set; }
        [Required(ErrorMessage = "District is required")]
        public string district { get; set; }
        [Required(ErrorMessage = "Village is required")]
        public string village { get; set; }
        [Required(ErrorMessage = "Province is required")]
        public string province { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string phone_number { get; set; }
        [Required(ErrorMessage = "Company is required")]
        public string company_id { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public string area_code { get; set; }
        public string? sap_code { get; set; }
        public string top_id { get; set; }
        public string top_duration { get; set; }
        public int? tax_type_id { get; set; }
        [Required(ErrorMessage = "Tax status is required")]
        public int tax_status_id { get; set; }
        public string? tax_invoice_number { get; set; }
        public DateTime? tax_invoice_date { get; set; }
        [Required(ErrorMessage = "Invoice number is required")]
        public string invoice_number { get; set; }
        [Required(ErrorMessage = "Invoice date is required")]
        public DateTime invoice_date { get; set; }
        public DateTime? received_date { get; set; }
        public DateTime? submitted_date { get; set; }
        public DateTime? posting_date { get; set; }
        public string? remmitance_number { get; set; }
        public DateTime? remmitance_date { get; set; }
        public string? awb_mail { get; set; }

        [Required(ErrorMessage = "PO type is required")]
        public int po_type_id { get; set; }
        [Required(ErrorMessage = "PO number is required")]
        public string po_number { get; set; }
        public string? old_po_number { get; set; }
        public string? gr_number { get; set; }
        public string? old_gr_number { get; set; }
        public string invoice_amount { get; set; }
        public string invoice_currency { get; set; }
        // table PO
        public IEnumerable<PoTicketModel> table_po { get; set; }
        public IEnumerable<GlTicketModel>? table_gl { get; set; }
        public IEnumerable<MaterialTicketModel>? table_material { get; set; }
        // table PO
        public string[]? gr_qty { get; set; }
        [Required(ErrorMessage = "QTY Billed is required")]
        public string[] qty_billed { get; set; }
        public string[]? uom { get; set; }
        public string[]? currency { get; set; }
        public string[]? gr_amount { get; set; }
        [Required(ErrorMessage = "Amount to be Invoice is required")]
        public string[] amount_invoice { get; set; }
        public string[]? amount_total { get; set; }
        public string[]? po_name {get; set;}
        public string[]? po_item_number {get; set;}
        public string[]? old_po_item_number {get; set;}
        public string[]? is_disabled {get; set;}
        public string[]? vendor_origin {get; set;}
        public string[]? document_no {get; set;}
        public string[]? item_1 {get; set;}
        public string[]? gl_description {get; set;}
        // table GL
        public string[]? gl_number { get; set; }
        public string[]? gl_debit_or_credit { get; set; }
        public double[]? gl_amount { get; set; }
        public string[]? gl_cost_center { get; set; }
        // table Material
        public string[]? material_id { get; set; }
        public string[]? material_plnt { get; set; }
        public double[]? material_amt { get; set; }
        public int[]? material_qty { get; set; }
        public string[]? material_d_or_c { get; set; }
        public double[]? material_total { get; set; }

        public double dpp_original { get; set; }
        public double dpp { get; set; }
        public double ppn { get; set; }
        public double pph { get; set; }
        public double total_price { get; set; }

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_invoice { get; set; }

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_po { get; set; }

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_lpb_gr { get; set; }

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_tax_invoice { get; set; }

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_delivery_note { get; set; }

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_bast { get; set; }
        public int verification_status_id { get; set; }
        public string? verification_note { get; set; }
        public int position_data { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string? type_of_payment { get; set; }
        public int? request_simulate { get; set; }
        public int? is_finish { get; set; }
        public double? tax_amt_fc { get; set; }
    }
}