using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tufol.Models
{
    [Table("tb_procurement_group")]
    public class PicModel
    {
        public string initial_area { get; set; }
        public string person_number { get; set; }
        public string email { get; set; }
    }
}