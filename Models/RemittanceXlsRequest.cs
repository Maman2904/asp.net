using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    public class RemittanceXlsRequest
    {
        public string? vendor_number { get; set; }
        public string? miro_number { get; set; }
        public string? remmitance_number{ get; set; }
        public DateTime? clearing_date { get; set; }

    }
}