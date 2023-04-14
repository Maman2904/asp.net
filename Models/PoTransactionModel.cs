using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_po_transaction")]
    public class PoTransactionModel
    {
        public string po_number { get; set; }
        public string po_item_number { get; set; }
        public string company_id { get; set; }
        public string vendor_number { get; set; }
        public string po_name { get; set; }
        public string currency { get; set; }
        public string? plnt_area { get; set; }
        public double? amount { get; set; }
        public Decimal? qty { get; set; }
        public string uom { get; set; }
        public bool is_service { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_used { get; set; }
        public bool is_active { get; set; }
        public string? vendor_origin {get; set;}
        public string? document_no {get; set;}
        public string? item_1 {get; set;}
        public string? gl_description {get; set;}
    }
}