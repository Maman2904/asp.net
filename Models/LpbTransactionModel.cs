using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_lpb_transaction")]
    public class LpbTransactionModel
    {
        public string lpb_number { get; set; }
        public string lpb_item_number { get; set; }
        public string vendor_number { get; set; }
        public string po_number { get; set; }
        public string po_item_number { get; set; }
        public decimal qty { get; set; }
        [DataType(DataType.Currency)]
        public double amount { get; set; }
        public string uom { get; set; }
        public string currency { get; set; }
        public string company_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_used { get; set; }
    }
}