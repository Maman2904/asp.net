using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tb_role")]
    public class RoleModel
    {
        public int role_id { get; set; }
        public string role_name { get; set; }

    }
}