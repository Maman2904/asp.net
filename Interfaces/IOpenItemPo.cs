using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IOpenItemPo
    {
        int CountTicketData(PostDatatableNetModel param, Dictionary<string, dynamic> condition = null, string vendor_number = null);
        Dictionary<string, dynamic> GetOpenItemPoDatatable(PostDatatableNetModel param, List<string> selected_column, Dictionary<string, dynamic> condition = null, string vendor_number = null);
    }
}