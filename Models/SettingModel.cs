using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tb_setting")]
    public class SettingModel
    {
        public string kode { get; set; }
        public string value { get; set; }
    }
}