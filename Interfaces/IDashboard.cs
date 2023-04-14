using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IDashboard
    {
        int CountVendor(string vendor_number = null);
        int CountTempVendor(int position_data, string type, int vendor_type_id = 0, string vendor_number = null);
        int CountTicket(int position_data, Dictionary<string, dynamic> condition = null, string vendor_number = null);
        int CountTicketConfirmToProcurement(string vendor_number = null);
    }
}