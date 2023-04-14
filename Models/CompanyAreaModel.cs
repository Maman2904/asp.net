using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    public class CompanyAreaModel
    {
        public string code { get; set; }
        public string location_id { get; set; }
        public string company_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string? company_area_name {get; set;}
    }
}