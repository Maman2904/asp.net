using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("temp_po_lpb")]
    public class TempPoLpbModel
    {
        public int? temp_po_lpb_id { get; set; }
        public string? po_number { get; set; }
        public string? po_item_number { get; set; }
        public string? vendor_number { get; set; }
        public string? po_name { get; set; }
        public string? currency { get; set; }
        public decimal? po_quantity { get; set; }
        public string? uom { get; set; }
        public string? lpb_number { get; set; }
        public string? lpb_item_number { get; set; }
        public decimal? quantity_lpb { get; set; }
        public string? company_id { get; set; }
        public string? document_no { get; set; }
        public string? item_1 { get; set; }
        public string? posting_date { get; set; }
        public string? doc_header_text { get; set; }
        public string? clearing_date { get; set; }
        public string? clearing_doc { get; set; }
        public string? pk { get; set; }
        public string? acc_ty { get; set; }
        public string? sg { get; set; }
        public string d_c { get; set; }
        public string bus_a { get; set; }
        public string gl { get; set; }
        public string gl_description { get; set; }
        public decimal amount_in_dc { get; set; }
        public decimal amount_in_lc { get; set; }
        public string material_code { get; set; }
        public string order_1 { get; set; }
        public string sales_doc { get; set; }
        public string item_2 { get; set; }
        public string asset { get; set; }
        public string s_no { get; set; }
        public string order_2{ get; set; }
        public string material_number{ get; set; }
        public string desc_vendor_or_credit{ get; set; }
        public string division{ get; set; }
        public string period{ get; set; }
        public string lpb_clearing{ get; set; }
        public string lpb_clear_date{ get; set; }
        public string voucher{ get; set; }
        public string cost_ctr{ get; set; }
        public string account_type{ get; set; }
        public string type{ get; set; }
        public string op_number{ get; set; }
        public string alt_bom{ get; set; }
        public string rcp_group{ get; set; }
        public string tr_ty{ get; set; }
        public string tr_s{ get; set; }
        public string? doc_currency { get; set; }
        public string? mov_type { get; set; }
        public string? ref_doc_migo { get; set; }
        public bool? is_active {get; set;}
        public string count{ get; set; }
        [DataType(DataType.Date)]
        public string created_at { get; set; }
        [DataType(DataType.Date)]
        public string updated_at { get; set; }
    }
}