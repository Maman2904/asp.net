using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    public class CompanyModel
    {
        public string? purchase_organization_id { get; set; }
        public string? purchase_organization_name { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "{0} must be 4 number")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        public string company_id { get; set; }
        public string name { get; set; }
        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }
    }
}