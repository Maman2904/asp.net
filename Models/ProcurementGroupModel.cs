using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tb_procurement_group")]
    public class ProcurementGroupModel
    {
        public string initial_area { get; set; }
        public string person_number { get; set; }
        public string email { get; set; }
        public string procurement_group { get; set; }

    }
}