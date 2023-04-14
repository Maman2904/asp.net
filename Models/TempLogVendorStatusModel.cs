using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("temp_log_vendor_status")]
    public class TempLogVendorStatusModel
    {
        public string log_id { get; set; }
        public int vendor_id { get; set; }
        public int verification_status_id { get; set; }
        public string verification_status { get; set; }
        public string? verification_note { get; set; }
        public int? created_by { get; set; }
        public string created_by_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}