using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_bank")]
    public class BankModel
    {
        [Required(ErrorMessage = "Required")]
        [MaxLength(30, ErrorMessage = "Bank id maximal 30 Number")]
        //[RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        public string bank_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string country_of_origin { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(100, ErrorMessage = "Maximal 100 characters")]
        public string name { get; set; }
        [MaxLength(15, ErrorMessage = "Maximal 15 characters")]
        public string swift_code { get; set; }
        [DataType(DataType.Date)]
        public string created_at { get; set; }
        [DataType(DataType.Date)]
        public string updated_at { get; set; }
    }
}