using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tr_po_ticket")]
    public class PoTicketModel
    {
        public int item_id { get; set; }
        public string ticket_number { get; set; }
        public string po_number { get; set; }
        public string? po_name { get; set; }
        public string? po_item_number { get; set; }
        public Decimal gr_qty { get; set; }
        public Decimal qty_billed { get; set; }
        public string uom { get; set; }
        public string currency { get; set; }
        public double gr_amount { get; set; }
        public double amount_invoice { get; set; }
        public double amount_total { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_disabled { get; set; }
        public string? vendor_origin {get; set;}
        public string? document_no {get; set;}
        public string? item_1 {get; set;}
        public string? gl_description {get; set;}
    }
}