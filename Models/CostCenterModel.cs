using System;
using System.ComponentModel.DataAnnotations;

namespace tufol.Models
{
    public class CostCenterModel
    {
        [Required(ErrorMessage = "Required")]
        [MaxLength(10, ErrorMessage = "{0} Maximal 10 number")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        public string? cc_id { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(100, ErrorMessage = "Maximal 100 characters")]
        public string? description { get; set; }
        [DataType(DataType.Date)]
        public string created_at { get; set; }
        [DataType(DataType.Date)]
        public string updated_at { get; set; }
    }
}