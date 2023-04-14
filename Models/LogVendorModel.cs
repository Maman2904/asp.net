using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("log_vendor")]
    public class LogVendorModel
    {
        public string log_id { get; set; }
        public string vendor_number { get; set; }
        public string process { get; set; }
        public string note { get; set; }
        public int created_by { get; set; }
        public string created_by_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}