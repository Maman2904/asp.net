using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tufol.Models
{
    [Table("tb_menu")]
    public class MenuModel
    {
        public int menu_id { get; set; }
        public string label { get; set; }
        public string? endpoint { get; set; }
        public IEnumerable<MenuModel> childs { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}