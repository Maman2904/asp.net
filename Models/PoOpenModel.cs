using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tufol.Models
{
    [Table("m_po_open")]
    public class PoOpenModel
    {
        public string? po_item_number { get; set; }
        public string? type { get; set; }
        public string? cat { get; set; }
        public string? pgr { get; set; }
        public string? poh { get; set; }
        public string? doc_date { get; set; }
        public string? material { get; set; }
        public string? po_name { get; set; }
        public string vendor_name { get; set; }
        public string? vendor_number { get; set; }
        public string? material_group { get; set; }
        public string? d { get; set; }
        public string? i { get; set; }
        public string? a { get; set; }
        public string? plnt { get; set; }
        public string? s_loc { get; set; }
        public decimal? po_quantity { get; set; }
        public string? uom { get; set; }
        public string? quantity_2 { get; set; }
        public string sku { get; set; }
        public string net_price { get; set; }
        public string? currency { get; set; }
        public string per { get; set; }
        public string quantity_3 { get; set; }
        public string open_tgt_qty { get; set; }
        public decimal quantity_to_be_del { get; set; }
        public decimal amount_to_be_del { get; set; }
        public decimal quantity_to_be_inv { get; set; }
        public decimal amount_to_be_inv { get; set; }
        public string number { get; set; }
        public string po_number { get; set; }
        public string p_org { get; set; }
        public string l { get; set; }
        public string tracking_number { get; set; }
        public string agmt { get; set; }
        public string item_2{ get; set; }
        public string targ_val{ get; set; }
        public string tot_open_val{ get; set; }
        public string open_value{ get; set; }
        public string rel_value{ get; set; }
        public string rel_qty{ get; set; }
        public string vp_start{ get; set; }
        public string vper_end{ get; set; }
        public string quot_dd_in{ get; set; }
        public string s{ get; set; }
        public string coll_number{ get; set; }
        public string ctl{ get; set; }
        public string info_rec{ get; set; }
        public string number_2{ get; set; }
        public string grp{ get; set; }
        public string strat{ get; set; }
        public string release{ get; set; }
        public string rel{ get; set; }
        public string ist_loc{ get; set; }
        public string vendor_name_2{ get; set; }
        public string opu{ get; set; }
        public string tx{ get; set; }
        public string tax_jur{ get; set; }
        public string net_value{ get; set; }
        public string i_2{ get; set; }
        public string notified{ get; set; }
        public string stk_seg{ get; set; }
        public string req_seg{ get; set; }
        public string s_2{ get; set; }
        public string conf_item_number{ get; set; }
        public string sort_number{ get; set; }
        public string ext_h_cat{ get; set; }
        public string ru{ get; set; }
        public string reqmt_price{ get; set; }
        public string smart_no{ get; set; }
        public string char1{ get; set; }
        public string char_desc_1{ get; set; }
        public string char2{ get; set; }
        public string char_desc_2{ get; set; }
        public string char3{ get; set; }
        public string char_desc_3{ get; set; }
        [DataType(DataType.Date)]
        public string created_at { get; set; }
        [DataType(DataType.Date)]
        public string updated_at { get; set; }
    }
}