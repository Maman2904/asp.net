using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tufol.Models

{
	public class TicketResponse
	{
        public string ticket_number { get; set; }
        public int sg_category_id { get; set; }
        public string vendor_number { get; set; }
        public string company_id { get; set; }
        public string invoice_business_area { get; set; }
        public string invoice_number { get; set; }
        [DataType(DataType.Date)]
        public int invoice_po_type { get; set; }
        public string invoice_po_number { get; set; }
        [DataType(DataType.Currency)]
        public float invoice_amount { get; set; }
        public string invoice_currency { get; set; }
        [DataType(DataType.Currency)]
        public float invoice_wt_tax_amt_base_fc { get; set; }
        [DataType(DataType.Currency)]
        public float invoice_wt_tax_amt_fc { get; set; }
        
        [DataType(DataType.Date)]
        public string invoice_date { get; set; }
        public List<dynamic>? material_list {get;set;}
        public List<dynamic>? item_list {get;set;}
    }

}