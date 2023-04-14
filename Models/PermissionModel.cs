using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    [Table("tb_user_permission")]
    public class PermissionModel
    {
        public bool allow_read { get; set; }
        public bool allow_create { get; set; }
        public bool allow_update { get; set; }
        public bool allow_delete { get; set; }
        public bool allow_approve { get; set; }
        public bool allow_revise { get; set; }
        public bool allow_reject { get; set; }
    }
}
