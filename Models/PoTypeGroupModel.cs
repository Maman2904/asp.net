using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_po_type_group")]
    public class PoTypeGroupModel
    {
        public int po_type_group_id { get; set; }
        public string name { get; set; }
    }
}