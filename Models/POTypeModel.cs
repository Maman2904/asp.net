using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_po_type")]
    public class PoTypeModel
    {
        public int po_type_id { get; set; }
        public string name { get; set; }
        public int po_type_group_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}