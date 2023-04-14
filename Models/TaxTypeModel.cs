using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_tax_type")]
    public class TaxTypeModel
    {
        public int tax_type_id { get; set; }
        public string name { get; set; }
        public string wht_code { get; set; }
        public string rates { get; set; }
        public int rate { get; set; }
        [DataType(DataType.Date)]
        public string created_at { get; set; }
        [DataType(DataType.Date)]
        public string updated_at { get; set; }
    }
}