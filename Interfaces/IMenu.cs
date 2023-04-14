using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IMenu
    {
        IEnumerable<MenuModel> GetMenu(string role_id = null, int parent_id = 0);
        PermissionModel GetPermission(string endpoint, string role_id = null);
    }
}