using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_vendor_type")]
    public class VendorTypeModel
    {
        public int vendor_type_id { get; set; }
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Must be 4 characters")]
        public string sap_code { get; set; }
        public string scheme_group_id { get; set; }
        public string gl_id { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }
    }
}