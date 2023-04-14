using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tb_user_area")]
    public class UserAreaModel
    {
        public int user_id { get; set; }
        public string code {get; set;}
        public string company_area_name {get; set;}
        public string company_id { get; set; }
        public string address { get; set; }

    }
}