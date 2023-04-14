using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tr_material_ticket")]
    public class MaterialTicketModel
    {
        public string ticket_number { get; set; }
        public string material_id { get; set; }
        public string material_plnt { get; set; }
        public double material_amt { get; set; }
        public int material_qty { get; set; }
        public string material_d_or_c { get; set; }
        public double material_total { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}