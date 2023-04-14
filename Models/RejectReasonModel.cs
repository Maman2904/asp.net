using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_reject_reason")]
    public class RejectReasonModel
    {
        public string reason_code { get; set; }
        public string text_en { get; set; }
        public string text_id { get; set; }
    }
}