using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tr_gl_ticket")]
    public class GlTicketModel
    {
        public string ticket_number { get; set; }
        public string gl_number { get; set; }
        public string gl_debit_or_credit { get; set; }
        [DataType(DataType.Currency)]
        public double gl_amount { get; set; }
        [DataType(DataType.Currency)]
        public double gl_cost_center { get; set; }
        public int gl_ticket_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}