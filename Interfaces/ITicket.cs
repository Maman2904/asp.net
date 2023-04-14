using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface ITicket
    {
        int CountTicketData(int position_data, PostDatatableNetModel param, Dictionary<string, dynamic> condition = null, bool except_role_id = false, int session_role = 0,  string user_id = null);
        List<string> GetUserArea(string user_id);
        Dictionary<string, dynamic> GetTicketDatatable(int position_data, PostDatatableNetModel param, List<string> selected_column, Dictionary<string, dynamic> condition = null, bool except_role_id = false, int session_role = 0, string vendor_number = null, string user_id = null);
        IEnumerable<TaxStatusModel> GetTaxStatus();
        IEnumerable<PoTypeModel> GetPOType(int po_type_group_id);
        IEnumerable<SimulateXlsRequest> GetSimulationResult(string ticket_number);

        IEnumerable<PoTransactionModel> GetPONumber(int po_type_group_id, int po_type, string vendor_number, string company_id = null);
        PoTransactionModel GetPONumberByNumber(string company_id, string po_number, string vendor_number);
        IEnumerable<PoTransactionModel> GetPOTable(int po_type_group_id, string company_id, string po_number, string vendor_number, string gr_number = null);
        IEnumerable<LpbTransactionModel> GetGrNumber(string po_number, int po_type_group_id, int po_type, string vendor_number, string company_id = null);
        IEnumerable<LocationModel> GetLocation();
        LocationModel GetAreaById(string area_code);
        IEnumerable<GlItemModel> GetGlItem();
        IEnumerable<CcItemModel> GetCcItem();
        IEnumerable<MaterialItemModel> GetMaterialItem();
        int CountMaterialItem(string q);
        IEnumerable<SelectModel> GetMaterialItem(string q);
        MaterialItemModel GetMaterialItemById(String material_id);
        string GetMaxTicketNumber(string year, string month, string area_code = null);
        string InsertTicket(CreateTicketModel model, String file_invoice, String file_po, String file_lpb_gr, String file_tax_invoice, String file_delivery_note, String file_bast);
        bool InsertLog(LogTicketModel model);
        bool DeleteTicket(string ticket_number);
        bool updateTicket(string ticket_number, CreateTicketModel model, String file_invoice_name = null, String file_po_name = null, String file_lpb_gr_name = null, String file_tax_invoice_name = null, String file_delivery_note_name = null, String file_bast_name = null);
        bool updateTicketVerificationStatus(string ticket_number, int verification_status_id, string verification_note = null);
        bool updateTicketReceivedDate(string ticket_number, DateTime received_date);
        bool updateTicketPositionData(string ticket_number, int position_data);
        bool InsertPoTicket(CreateTicketModel model, string ticket_number);
        bool InsertGlTicket(CreateTicketModel model, string ticket_number);
        bool InsertMaterialTicket(CreateTicketModel model, string ticket_number);
        bool UpdateIsUsedSvcTransaction(string po_number, string[] po_item_number, string vendor_number, bool is_used);
        bool UpdateIsUsedLpbTransaction(string po_number, string gr_number, string[] po_item_number, string vendor_number, bool is_used);
        bool UpdateIsUsedPoTransaction(CreateTicketModel model, Dictionary<string, dynamic> condition = null);
        bool DeletePoTicket(string ticket_number);
        bool DeleteGlTicket(string ticket_number);
        bool DeleteMaterialTicket(string ticket_number);
        TicketModel GetTicketDetail(String ticket_number);
        TicketModel GetVendorForTicket(String vendor_number);
        IEnumerable<LogTicketModel> GetLogTicket(string ticket_number);
        IEnumerable<CompanyModel> GetCompany(string vendor_number);
        IEnumerable<TicketModel> GetTicketByPositionData(int position_data);
        IEnumerable<PoTicketModel> GetPoTicket(string ticket_number);
        IEnumerable<dynamic> RpaGetTickets(int ticketCase);
        IEnumerable<dynamic> RpaGetRejectedTickets();
        Task<int> RpaPaymentRemittance(List<RemittanceXlsRequest> remittanceXlsRequests);
        Task<int>? RpaTicketReversal(string miro_number, TicketReversalRequest requestBody);
        Task<int>? RpaUpdateMiroNumber(string ticket_number, UpdateMIRONumberPayloadBody payload);
        Task<int>? RpaSimulateResult(List<SimulateXlsRequest> SimulateXlsRequestList, string status_rpa);
        IEnumerable<MaterialItemModel> GetMaterialList(int offset, int limitRow, string search = null);
        IEnumerable<dynamic> GetMaterialListCount();
        string GetEmailCompanyArea (string code);
    }
}