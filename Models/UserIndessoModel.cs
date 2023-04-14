using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace tufol.Models
{
    [Table("tb_user")]
    public class UserIndessoModel
    {
        public int user_id { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int role_id { get; set; }
        
        public string role {get; set;}

        [Required(ErrorMessage = "Name is required")]
        public string name {get; set;}
        public string initial_area {get; set;}
        public string procurement_group {get; set;}

        [Required(ErrorMessage = "Username is required")]
        public string username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "{0} Must be 8 Character")]
        //[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "{0} Must Be a Contains Number And Alphabet.")]
        public string password { get; set; }
        public string repeat_password { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }
        public string reset_token { get; set; }

        [Required(ErrorMessage = "User Area is required")]
        public string[]? company_area { get; set; }
        public IEnumerable<UserAreaModel>? user_area {get; set;}
        
        [DataType(DataType.Date)]
        public DateTime last_login { get; set; }

        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }

        [Required(ErrorMessage = "Company area is required")]
        public string area_code {get; set;}
    }
}