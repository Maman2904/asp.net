using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("m_country_tax")]
    public class CountryTaxModel
    {
        public int country_tax_id { get; set; }
        public string country_id { get; set; }
        public string? area { get; set; }
        public int tax { get; set; }
    }
}