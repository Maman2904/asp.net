using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
  public class SimulateXlsRequest
  {
    public string? ticket_number { get; set; }
    public string? position { get; set; }
    public string? account_type { get; set; }
    public string? g_l { get; set; }
    public string? act_mat_ast_vndr { get; set; }
    public decimal? amount { get; set; }
    public string? currency { get; set; }
    public string? po_number { get; set; }
    public string? po_item_number { get; set; }
    public string? tax_code { get; set; }
    public string? jurisd_code { get; set; }
    public string? tax_date { get; set; }
    public string? bus_area { get; set; }
    public string? cost_center { get; set; }
    public string? co_area { get; set; }
    public string? order_1 { get; set; }
    public string? asset { get; set; }
    public string? sub_number { get; set; }
    public string? asset_val { get; set; }
    public string? material { get; set; }
    public string? quantity { get; set; }
    public string? base_unit { get; set; }
    public string? plant { get; set; }
    public string? part_bus_ar { get; set; }
    public string? cost_object { get; set; }
    public string? profit_segment { get; set; }
    public string? profit_center { get; set; }
    public string? partner_prctr { get; set; }
    public string? wbs_element { get; set; }
    public string? network { get; set; }
    public string? sales_order { get; set; }
    public string? sls_item { get; set; }
    public string? vendor_number { get; set; }
    public string? posting_key { get; set; }
    public string? posting_line_id { get; set; }
    public string? created_auto { get; set; }
    public string? status_rpa { get; set; }
    public string? rpa_status_description { get; set; }
    public int? request_simulate { get; set; }
    public int? verification_status_id { get; set; }


  }
}