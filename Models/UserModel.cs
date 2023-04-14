using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tb_user")]
    public class UserModel
    {
        public int user_id { get; set; }
        public int role_id { get; set; }
        
        public string role {get; set;}

        public string name {get; set;}
        public string initial_area {get; set;}
        public string procurement_group {get; set;}
        public string username { get; set; }
        public string? password { get; set; }
        public string? repeat_password { get; set; }
        public string email { get; set; }
        public string reset_token { get; set; }
        public IEnumerable<UserAreaModel>? user_area {get; set;}
        
        [DataType(DataType.Date)]
        public DateTime last_login { get; set; }

        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }
        public string? vendor_number { get; set; }
    }
}