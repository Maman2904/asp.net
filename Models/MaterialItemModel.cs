using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    public class MaterialItemModel
    {
        [Required(ErrorMessage = "Required")]
        [MaxLength(15, ErrorMessage = "{0} Maximal 15 number")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        public string material_id { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "{0} Must be 4 number")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        public string material_plnt { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(200, ErrorMessage = "Maximal 200 characters")]
        public string description { get; set; }
        [DataType(DataType.Date)]
        public string created_at { get; set; }
        [DataType(DataType.Date)]
        public string updated_at { get; set; }
    }
}