using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using tufol.Models;
using tufol.Helpers;
using tufol.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using Microsoft.Extensions.Options;
using System.IO;
using ExcelDataReader;
using System.Text.Json;

namespace tufol.Controllers
{
  public class RpaApiController : Controller
  {
    private IWebHostEnvironment _iHostingEnvironment;
    private String _current_language;
    private readonly Helpers.AppSettings _appSettings;
    private readonly Helpers.DefaultValue _defaultValue;
    private IRegistration _registration;
    private ISetting _setting;
    private IUserAccessApi _userTest;
    private IUser _user;
    private IMaster _master;
    private IVendor _vendor;
    private ITicket _ticket;
    private ITempCompanyRelation _company;
    private IMasterTable _masterTable;
    public RpaApiController(IRegistration registration, ITempCompanyRelation company, IUser user, IMaster master, IVendor vendor, IUserAccessApi userTest, ISetting setting, ITicket ticket, IWebHostEnvironment iHostingEnvironment, IOptions<Helpers.AppSettings> appSettings, IOptions<Helpers.DefaultValue> defaultValue, IMasterTable masterTable)
    {
      // this._httpContextAccessor = httpContextAccessor;
      _setting = setting;
      _registration = registration;
      _user = user;
      _master = master;
      _vendor = vendor;
      _ticket = ticket;
      _iHostingEnvironment = iHostingEnvironment;
      _current_language = Thread.CurrentThread.CurrentCulture.Name;
      _appSettings = appSettings.Value;
      _defaultValue = defaultValue.Value;
      _company = company;
      _masterTable = masterTable;
    }


    [Route("api/rpa/ApprovedVendorRegistration")]
    [HttpGet, ActionName("ApprovedVendorRegistration")]
    public IActionResult ApprovedVendorRegistration(string type)
    {

      var result = _vendor.ApprovedVendorRegistration(type);

      return new ObjectResult(new
      {
        status = 200,
        count = result.Count(),
        data = result,
      })
      { StatusCode = 200 };
    }

    [Route("api/rpa/NewVendorActivation")]
    [HttpPut, ActionName("NewVendorActivation")]
    public async Task<IActionResult> NewVendorActivationAsync(int id, [FromBody] RpaTempVendorModel reqBody)
    {
      try
      {
        Console.WriteLine("HELLLO 000");
        var tempVendor = _vendor.GetPendingVendorDetail(id);
        var tempVendorCompany = _vendor.GetTempCompany(id);
        var error = new BadRequestMessage();
        var status_rpa = reqBody.status_rpa;
        var vendor_number = reqBody.vendor_number;
        string rpa_status_description = reqBody.rpa_status_description;

        Console.WriteLine("vendor_number"+ vendor_number);
        Console.WriteLine("status_rpa"+ status_rpa);
        Console.WriteLine("description"+ reqBody.rpa_status_description);


        if (id != tempVendor.vendor_id)
        {
          Console.WriteLine("HELLLO 2");

          return BadRequest(new { status = 400, message = "ID Vendor not found", error = NotFound() });
        }

        error.items = new List<BadRequesBody>();

        if (status_rpa == null)
        {
          error.items.Add(new BadRequesBody { msg = "status_rpa is required", param = "status_rpa", location = "body" });
        }
        if (vendor_number == null && status_rpa == 1)
        {
          error.items.Add(new BadRequesBody { msg = "vendor_number is required", param = "vendor_number", location = "body" });
        }
        if (error.items.Count > 0)
        {
          return BadRequest(new { status = 400, message = "Bad Request", error = error.items });
        }
        if (status_rpa == 1)
        {
          var bank = _vendor.CekBankExist(tempVendor.foreign_bank_id);
          BankModel update = new BankModel()
          {
            bank_id = tempVendor.foreign_bank_id,
            name = tempVendor.foreign_bank_name,
            country_of_origin = tempVendor.foreign_bank_country_id,
            swift_code = tempVendor.foreign_bank_swift_code
          };
          if (tempVendor.country_id != "ID" && !string.IsNullOrEmpty(tempVendor.foreign_bank_id))
          {
            Console.WriteLine("insert bank");
            if (bank)
            {
              _vendor.updateBank(update);
            }
            else
            {
              _vendor.InsertBank(update);
            }
          }

          Console.WriteLine("insert vendor");
          VendorModel vendorData = new VendorModel()
          {
            vendor_number = reqBody.vendor_number,
            status_rpa = Convert.ToBoolean(reqBody.status_rpa),
            rpa_status_description = reqBody.rpa_status_description,
            vendor_type_id = tempVendor.vendor_type_id,
            account_group_id = tempVendor.account_group_id,
            vendor_type_name = tempVendor.vendor_type_name,
            vendor_type_code = tempVendor.vendor_type_code,
            scheme_group = tempVendor.scheme_group,
            gl = tempVendor.gl,
            sg_category_id = tempVendor.sg_category_id,
            sg_category_name = tempVendor.sg_category_name,
            company = tempVendor.company,
            currency_id = tempVendor.currency_id,
            top_id = tempVendor.top_id,
            top_name = tempVendor.top_name,
            title_id = tempVendor.title_id,
            name = tempVendor.name,
            contact_person = tempVendor.contact_person,
            search_term = tempVendor.search_term,
            street_address = tempVendor.street_address,
            additional_street_address = tempVendor.additional_street_address,
            city = tempVendor.city,
            postal_code = tempVendor.postal_code,
            country_id = tempVendor.country_id,
            country_name = tempVendor.country_name,
            telephone = tempVendor.telephone,
            fax = tempVendor.fax,
            email = tempVendor.email,
            email_list = tempVendor.email_list,
            tax_type_id = tempVendor.tax_type_id,
            tax_type_name = tempVendor.tax_type_name,
            wht_code = tempVendor.wht_code,
            tax_rates = tempVendor.tax_rates,
            tax_number_type_id = tempVendor.tax_number_type_id,
            tax_number_type_code = tempVendor.tax_number_type_code,
            tax_number_type_name = tempVendor.tax_number_type_name,
            npwp = tempVendor.npwp,
            id_card_number = tempVendor.id_card_number,
            sppkp_number = tempVendor.sppkp_number,
            localidr_bank_id = tempVendor.localidr_bank_id,
            localidr_bank_name = tempVendor.localidr_bank_name,
            localidr_swift_code = tempVendor.localidr_swift_code,
            localidr_bank_account = tempVendor.localidr_bank_account,
            localidr_account_holder = tempVendor.localidr_account_holder,
            localforex_bank_id = tempVendor.localforex_bank_id,
            localforex_bank_name = tempVendor.localforex_bank_name,
            localforex_swift_code = tempVendor.localidr_swift_code,
            localforex_bank_account = tempVendor.localforex_bank_account,
            localforex_account_holder = tempVendor.localforex_account_holder,
            localforex_currency_id = tempVendor.localforex_currency_id,
            foreign_bank_country_id = tempVendor.foreign_bank_country_id,
            foreign_bank_country_name = tempVendor.foreign_bank_country_name,
            foreign_bank_id = tempVendor.foreign_bank_id,
            foreign_bank_name = tempVendor.foreign_bank_name,
            foreign_bank_swift_code = tempVendor.foreign_bank_swift_code,
            foreign_bank_account = tempVendor.foreign_bank_account,
            foreign_account_holder = tempVendor.foreign_account_holder,
            foreign_currency_id = tempVendor.foreign_currency_id,
            reference_correspondent = tempVendor.reference_correspondent,
            reference_country_origin = tempVendor.reference_country_origin,
            pic_id = tempVendor.pic_id,
            person_number = tempVendor.person_number,
            email_procurement_group = tempVendor.email_procurement_group,
            file_id_card = tempVendor.file_id_card,
            file_npwp = tempVendor.file_npwp,
            file_sppkp = tempVendor.file_sppkp,
            file_vendor_statement = tempVendor.file_vendor_statement
          };
          Console.WriteLine("foreign+ :" + vendorData.foreign_bank_id);
          LogVendorModel data_insert_log = new LogVendorModel()
          {
            vendor_number = reqBody.vendor_number,
            process = "Insert Vendor",
            note = null,
            created_by = 9,
          };
          bool insertVendor = _vendor.InsertVendor(vendorData);
          Console.WriteLine("insertVendor" + insertVendor);
          if (insertVendor)
          {
            var insertLogVendor = _vendor.InsertLogVendor(data_insert_log);
            Console.WriteLine("insertLogVendor" + insertLogVendor);
            var InsertCompanyRelations = _vendor.InsertCompany(tempVendorCompany, vendor_number);


            _vendor.deleteTempCompanyByVendorId(id);
            _vendor.DeleteTempVendor(id);

            var generatePwd = new Helpers.GlobalFunction().GenerateRandomChar(8);
            Console.WriteLine("insertVendor" + generatePwd);
            String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/activation-" + _current_language + ".html";
            String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
            List<String> email_list = new Helpers.EmailFunction().GetEmailList(vendorData.email);
            foreach (string email in email_list)
            {
              Dictionary<String, String> email_content_param = new Dictionary<string, string>();
              email_content_param.Add("email", email);
              email_content_param.Add("year", DateTime.Now.Year.ToString());
              email_content_param.Add("vendor_name", vendorData.name);
              email_content_param.Add("Username", reqBody.vendor_number);
              email_content_param.Add("Password", generatePwd);

              String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

              new Helpers.EmailFunction().sending_email(email, (_current_language == "id" ? "Aktivasi akun" : "Activation Account"), email_content);
            }
            var encryptPassword = new Helpers.GlobalFunction().GetMD5(generatePwd);
            UserModel insertUser = new UserModel()
            {
              role_id = 2,
              name = vendorData.name,
              username = vendor_number,
              email = vendorData.email,
              initial_area = vendorData.pic_id,
              vendor_number = vendor_number
            };
            _user.AddUser(insertUser, encryptPassword);
          }
        }
        if (status_rpa == 0)
        {
          TempVendorModel update = new TempVendorModel()
          {
            position_data = 5,
            verification_status_id = 17,
            vendor_id = id
          };
          _vendor.UpdateTempVendorRpa(update, status_rpa, reqBody.rpa_status_description);
          TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
          {
            vendor_id = id,
            verification_status_id = 17,
            verification_note = "Eror RPA",
            created_by = 9
          };
          _vendor.InsertLogTempVendor(data_insert);

          var pic = _master.GetProcurementGroupByID(tempVendor.pic_id);

          String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/failed_rpa-" + _current_language + ".html";
          String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

          List<String> recipient_email = new List<String>() {
                            pic.email
                        };

          string email_fc = _setting.GetSettingValue("email_fc_vendor");
          if (!string.IsNullOrEmpty(email_fc))
          {
            List<string> fc_email_list = new Helpers.EmailFunction().GetEmailList(email_fc, ";");
            fc_email_list = fc_email_list == null ? new List<string>() : fc_email_list;
            foreach (string fc_recipient in fc_email_list)
            {
              recipient_email.Add(fc_recipient);
            }
          }
          foreach (string recipient in recipient_email)
          {
            Dictionary<String, String> email_content_param = new Dictionary<string, string>();
            email_content_param.Add("title", (_current_language == "id" ? "Error RPA" : "RPA Error"));
            email_content_param.Add("type", (_current_language == "id" ? "Error RPA" : "RPA Error"));
            email_content_param.Add("email", recipient);
            email_content_param.Add("vendor_name", tempVendor.name);
            email_content_param.Add("year", DateTime.Now.Year.ToString());
            email_content_param.Add("approved_by", "RPA");

            String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
            new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Error RPA" : "RPA Eror"), email_content);
          }
        }

        return Json(new { status = 200, data = new { status_rpa = reqBody.status_rpa, rpa_status_description = reqBody.rpa_status_description, vendor_number = reqBody.vendor_number } });
      }
      catch (Exception ex)
      {
        new Helpers.GlobalFunction().LogError("newActivation", ex.Message);
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return Json(new { status = 500, message = ex.Message });
      }
    }

    [Route("api/rpa/UpdateVendorChange")]
    [HttpPut, ActionName("UpdateVendorChange")]
    public async Task<IActionResult> UpdateVendorChangeAsync(int id, [FromBody] RpaTempVendorModel reqBody)
    {
      try
      {
        var tempVendor = _vendor.GetPendingVendorDetail(id);
        var tempVendorCompany = _vendor.GetTempCompany(id);
        var error = new BadRequestMessage();
        var status_rpa = reqBody.status_rpa;
        var rpa_status_description = reqBody.rpa_status_description;
        var vendor_number = reqBody.vendor_number;
        Console.WriteLine("vendor_number"+ vendor_number);
        Console.WriteLine("status_rpa"+ status_rpa);
        Console.WriteLine("description"+ reqBody.rpa_status_description);

        if (id != tempVendor.vendor_id)
        {
          return BadRequest(new { status = 400, message = "Bad Request", error = NotFound() });
        }

        error.items = new List<BadRequesBody>();

        if (status_rpa == null)
        {
          error.items.Add(new BadRequesBody { msg = "status_rpa is required", param = "status_rpa", location = "body" });
        }
        if (vendor_number == null)
        {
          error.items.Add(new BadRequesBody { msg = "vendor_number is required", param = "vendor_number", location = "body" });
        }
        if (error.items.Count > 0)
        {
          return BadRequest(new { status = 400, message = "Bad Request", error = error.items });
        }
        if (status_rpa == 1)
        {
          var bank = _vendor.GetBank(tempVendor.foreign_bank_id);
          BankModel update = new BankModel()
          {
            bank_id = tempVendor.foreign_bank_id,
            name = tempVendor.foreign_bank_name,
            country_of_origin = tempVendor.reference_country_origin,
            swift_code = tempVendor.foreign_bank_swift_code
          };
          if (tempVendor.country_id != "ID" && !string.IsNullOrEmpty(tempVendor.foreign_bank_id))
          {
            Console.WriteLine("insert bank");
            if (bank == tempVendor.foreign_bank_id)
            {
              _vendor.updateBank(update);
            }
            else
            {
              _vendor.InsertBank(update);
            }
          }

          Console.WriteLine("update vendor");
          VendorModel vendorData = new VendorModel()
          {
            vendor_number = reqBody.vendor_number,
            status_rpa = Convert.ToBoolean(reqBody.status_rpa),
            rpa_status_description = reqBody.rpa_status_description,
            vendor_type_id = tempVendor.vendor_type_id,
            account_group_id = tempVendor.account_group_id,
            vendor_type_name = tempVendor.vendor_type_name,
            vendor_type_code = tempVendor.vendor_type_code,
            scheme_group = tempVendor.scheme_group,
            gl = tempVendor.gl,
            sg_category_id = tempVendor.sg_category_id,
            sg_category_name = tempVendor.sg_category_name,
            company = tempVendor.company,
            currency_id = tempVendor.currency_id,
            top_id = tempVendor.top_id,
            top_name = tempVendor.top_name,
            title_id = tempVendor.title_id,
            name = tempVendor.name,
            contact_person = tempVendor.contact_person,
            search_term = tempVendor.search_term,
            street_address = tempVendor.street_address,
            additional_street_address = tempVendor.additional_street_address,
            city = tempVendor.city,
            postal_code = tempVendor.postal_code,
            country_id = tempVendor.country_id,
            country_name = tempVendor.country_name,
            telephone = tempVendor.telephone,
            fax = tempVendor.fax,
            email = tempVendor.email,
            email_list = tempVendor.email_list,
            tax_type_id = tempVendor.tax_number_type_id,
            tax_type_name = tempVendor.tax_type_name,
            wht_code = tempVendor.wht_code,
            tax_rates = tempVendor.tax_rates,
            tax_number_type_id = tempVendor.tax_number_type_id,
            tax_number_type_code = tempVendor.tax_number_type_code,
            tax_number_type_name = tempVendor.tax_number_type_name,
            npwp = tempVendor.npwp,
            id_card_number = tempVendor.id_card_number,
            sppkp_number = tempVendor.sppkp_number,
            localidr_bank_id = tempVendor.localidr_bank_id,
            localidr_bank_name = tempVendor.localidr_bank_name,
            localidr_swift_code = tempVendor.localidr_swift_code,
            localidr_bank_account = tempVendor.localidr_bank_account,
            localidr_account_holder = tempVendor.localidr_account_holder,
            localforex_bank_id = tempVendor.localforex_bank_id,
            localforex_bank_name = tempVendor.localforex_bank_name,
            localforex_swift_code = tempVendor.localidr_swift_code,
            localforex_bank_account = tempVendor.localforex_bank_account,
            localforex_account_holder = tempVendor.localforex_account_holder,
            localforex_currency_id = tempVendor.localforex_currency_id,
            foreign_bank_country_id = tempVendor.foreign_bank_country_id,
            foreign_bank_country_name = tempVendor.foreign_bank_country_name,
            foreign_bank_id = tempVendor.foreign_bank_id,
            foreign_bank_name = tempVendor.foreign_bank_name,
            foreign_bank_swift_code = tempVendor.foreign_bank_swift_code,
            foreign_bank_account = tempVendor.foreign_bank_account,
            foreign_account_holder = tempVendor.foreign_account_holder,
            foreign_currency_id = tempVendor.foreign_currency_id,
            reference_correspondent = tempVendor.reference_correspondent,
            reference_country_origin = tempVendor.reference_country_origin,
            pic_id = tempVendor.pic_id,
            person_number = tempVendor.person_number,
            email_procurement_group = tempVendor.email_procurement_group,
            file_id_card = tempVendor.file_id_card,
            file_npwp = tempVendor.file_npwp,
            file_sppkp = tempVendor.file_sppkp,
            file_vendor_statement = tempVendor.file_vendor_statement,
            is_locked = Convert.ToBoolean(0),
            change_request_status = 0,
            token = tempVendor.token
          };
          LogVendorModel data_insert_log = new LogVendorModel()
          {
            vendor_number = reqBody.vendor_number,
            process = "Insert Vendor",
            note = null,
            created_by = 9,
          };
          bool updateVendor = _vendor.updateVendorRpa(vendorData);
          Console.WriteLine("updateVendor" + updateVendor);
          if (updateVendor)
          {
            _vendor.deleteCompanyByVendorNumber(vendor_number);
            _vendor.InsertCompany(tempVendorCompany, vendor_number);
            _vendor.InsertLogVendor(data_insert_log);
            _vendor.deleteTempCompanyByVendorId(id);
            _vendor.DeleteTempVendor(id);

            String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/change_data_success-" + _current_language + ".html";
            String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
            List<String> email_list = new Helpers.EmailFunction().GetEmailList(vendorData.email);
            foreach (string email in email_list)
            {
              Dictionary<String, String> email_content_param = new Dictionary<string, string>();
              email_content_param.Add("email", email);
              email_content_param.Add("year", DateTime.Now.Year.ToString());
              email_content_param.Add("vendor_name", vendorData.name);

              String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

              new Helpers.EmailFunction().sending_email(email, (_current_language == "id" ? "Perubahan Data" : "Change Data"), email_content);
            }
          }

        }
        if (status_rpa == 0)
        {
            if  (rpa_status_description == null) error.items.Add(new BadRequesBody { msg = "rpa_status_description is required", param = "rpa_status_description", location = "body" });
            if (error.items.Count > 0)
            {
              return BadRequest(new { status = 400, message = "Bad Request", error = error.items.ToArray() });
            }
          TempVendorModel update = new TempVendorModel()
          {
            position_data = 5,
            verification_status_id = 17,
            vendor_id = id
          };
          _vendor.UpdateTempVendorRpa(update, status_rpa, reqBody.rpa_status_description);
          TempLogVendorStatusModel data_insert = new TempLogVendorStatusModel()
          {
            vendor_id = id,
            verification_status_id = 17,
            verification_note = "Error RPA",
            created_by = 9
          };
          _vendor.InsertLogTempVendor(data_insert);

          var pic = _master.GetProcurementGroupByID(tempVendor.pic_id);

          String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/failed_rpa-" + _current_language + ".html";
          String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);

          List<String> recipient_email = new List<String>() {
                            pic.email
                        };

          string email_fc = _setting.GetSettingValue("email_fc_vendor");
          if (!string.IsNullOrEmpty(email_fc))
          {
            List<string> fc_email_list = new Helpers.EmailFunction().GetEmailList(email_fc, ";");
            fc_email_list = fc_email_list == null ? new List<string>() : fc_email_list;
            foreach (string fc_recipient in fc_email_list)
            {
              recipient_email.Add(fc_recipient);
            }
          }
          foreach (string recipient in recipient_email)
          {
            Dictionary<String, String> email_content_param = new Dictionary<string, string>();
            email_content_param.Add("title", (_current_language == "id" ? "Error RPA" : "RPA Error"));
            email_content_param.Add("type", (_current_language == "id" ? "Error RPA" : "RPA Error"));
            email_content_param.Add("email", recipient);
            email_content_param.Add("vendor_name", tempVendor.name);
            email_content_param.Add("year", DateTime.Now.Year.ToString());
            email_content_param.Add("approved_by", "RPA");

            String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);
            new Helpers.EmailFunction().sending_email(recipient, (_current_language == "id" ? "Error RPA" : "RPA Eror"), email_content);
          }
        }

        return Json(new { status = 200, data = new { status_rpa = reqBody.status_rpa, rpa_status_description = reqBody.rpa_status_description, vendor_number = reqBody.vendor_number } });
      }
      catch (Exception ex)
      {
        new Helpers.GlobalFunction().LogError("changeActivation", ex.Message);
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return Json(new { status = 500, message = ex.Message });
      }
    }


    [Route("api/rpa/ApprovedTickets")]
    [HttpGet, ActionName("ApprovedTickets")]
    public IActionResult ApprovedTickets(String vendor_number)

    {
      // APPROVED TICKET
      // WHERE position_data = 9 AND verification_status_id = 12

      var result = _ticket.RpaGetTickets(1);

      return new ObjectResult(new
      {
        status = 200,
        // count = result.Count(),
        data = result,
      })
      { StatusCode = 200 };
    }

    [Route("api/rpa/MasterTable")]
    [HttpPost, ActionName("MasterTable")]
    public async Task<IActionResult> MasterTable(string type, [FromForm] IFormFile xlsFile)
    {

      List<TempPoServiceModel> ListPoService = new List<TempPoServiceModel>();
      List<TempPoLpbModel> ListPoLpb = new List<TempPoLpbModel>();
      List<PoOpenModel> ListPoOpen = new List<PoOpenModel>();
      List<VendorModelRpa> ListVendor = new List<VendorModelRpa>();
      List<CompanyRelationModel> ListCompanyRelation = new List<CompanyRelationModel>();
      List<UserModel> ListUser = new List<UserModel>();
      List<LogVendorModel> ListlogVendor = new List<LogVendorModel>();
      //List<TempVendorModel> ListTempVendor = new List<TempVendorModel>();
      var filename = xlsFile.FileName;
      var file = xlsFile.OpenReadStream();
      var company = _vendor.getCompanyId();
      //var polpb = "PoLpb";
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
      // if (type != polpb){
      //   return new ObjectResult(new
      // {
      //   status = 400,
      //   message = "type is required"
      // })
      // { StatusCode = 400 };
      // }

      // if (xlsFile is null){
      //     return new ObjectResult(new
      // {
      //   status = 400,
      //   message = "not file insert"
      // })
      // { StatusCode = 400 };
      // }
      
      if(type == "PoLpb"){
        string[] kode = filename.Split("_");
        var codeCompany = kode[2];
        Console.WriteLine("po_lpb " +codeCompany);
        foreach(var code in company){
          if (filename.Contains(code)){
            using(var reader = ExcelReaderFactory.CreateReader(file)) {
              while (reader.Read()) {
                var vendor_number = reader.IsDBNull(27);
                var po_number = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString();
                var po_item_number = reader.IsDBNull(4);
                var po_name = reader.IsDBNull(19);
                var currency = reader.IsDBNull(11);
                var po_quantity = reader.IsDBNull(7);
                var uom = reader.IsDBNull(3);
                var lpb_number = reader.IsDBNull(5) ? null : reader.GetValue(5).ToString();
                var lpb_item_number = reader.IsDBNull(6);
                var quantity_lpb = reader.IsDBNull(38);
                var company_id = reader.IsDBNull(0);
                var plant_area = reader.IsDBNull(26);
                if(vendor_number){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "vendor number cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }

                if(po_number != null || lpb_number!= null){
                  if(vendor_number){
                    return new ObjectResult(new
                      {
                        status = 400,
                        message = "vendor number cannot be null in the file Where po number :"+po_number+"lpb number :"+lpb_number,
                      })
                      { StatusCode = 400 };
                  }
                }
                if(po_number == null){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "po number cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(currency){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "currency cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(po_quantity){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "po quantity cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(uom){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "uom cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(lpb_number == null){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "lpb number cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(lpb_item_number){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "lpb item number cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(quantity_lpb){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "quantity lpb cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(company_id){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "po number cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(po_number != null || lpb_number!= null){
                  if(po_name){
                    return new ObjectResult(new
                      {
                        status = 400,
                        message = "po name cannot be null in the file Where po number :"+po_number+"lpb number :"+lpb_number,
                      })
                      { StatusCode = 400 };
                  }
                }
                if(plant_area){
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "bussiness plant area cannot be null in the file"
                    })
                    { StatusCode = 400 };
                }
                if(reader.Depth > 0){
                    string vendor_numberList = reader.IsDBNull(27) ? null : reader.GetValue(27).ToString();
                    if(vendor_numberList != null){

                          // var cekvendor = _masterTable.CekVendor(vendor_numberList);
                          // // if (string.IsNullOrEmpty(cekvendor)){
                          // //   return new ObjectResult(new
                          // //   {
                          // //     status = 400,
                          // //     message = "vendor number is null in the table vendor"
                          // //   })
                          // //   { StatusCode = 400 };
                          // // }

                            //Console.WriteLine("cekvendor"+ cek);
                          //if (!string.IsNullOrEmpty(cekvendor)){
                            bool is_active = true;
                            var cancel_status = reader.IsDBNull(15) ? null : reader.GetValue(15).ToString();
                            if (cancel_status == "X") {
                              is_active = false;
                            }
                            ListPoLpb.Add(new TempPoLpbModel(){
                              company_id = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                              document_no = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                              item_1 = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
                              // item_1 = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                              posting_date = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
                              doc_header_text = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
                              clearing_date = reader.IsDBNull(40) ? null : reader.GetValue(40).ToString(),
                              clearing_doc = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),
                              pk = reader.IsDBNull(22) ? null : reader.GetValue(22).ToString(),
                              acc_ty = reader.IsDBNull(23) ? null : reader.GetValue(23).ToString(),
                              sg = reader.IsDBNull(24) ? null : reader.GetValue(24).ToString(),
                              d_c = reader.IsDBNull(25) ? null : reader.GetValue(25).ToString(),
                              bus_a = reader.IsDBNull(26) ? null : reader.GetValue(26).ToString(),
                              gl = reader.IsDBNull(17) ? null : reader.GetValue(17).ToString(),
                              gl_description = reader.IsDBNull(18) ? "" : reader.GetValue(18).ToString(),
                              amount_in_dc = reader.IsDBNull(10) ? 0 : (-1 * Convert.ToDecimal(reader.GetValue(10))),
                              currency = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString(),
                              amount_in_lc = reader.IsDBNull(12) ? 0 : (-1 * Convert.ToDecimal(reader.GetValue(12))),
                              po_name = reader.IsDBNull(19) ? "" : reader.GetValue(19).ToString(),
                              vendor_number = reader.IsDBNull(27) ? null : reader.GetValue(27).ToString(),
                              material_code = reader.IsDBNull(28) ? null : reader.GetValue(28).ToString(),
                              po_number = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                              po_item_number = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                              // po_item_number = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
                              po_quantity = reader.IsDBNull(7) ? null : (-1 * Convert.ToDecimal(reader.GetValue(7).ToString())),
                              uom = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
                              order_1 = reader.IsDBNull(29) ? null : reader.GetValue(29).ToString(),
                              sales_doc = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString(),
                              item_2 = reader.IsDBNull(31) ? null : reader.GetValue(31).ToString(),
                              asset = reader.IsDBNull(32) ? null : reader.GetValue(32).ToString(),
                              s_no = reader.IsDBNull(33) ? null : reader.GetValue(33).ToString(),
                              order_2 = reader.IsDBNull(34) ? null : reader.GetValue(34).ToString(),
                              material_number = reader.IsDBNull(35) ? null : reader.GetValue(35).ToString(),
                              desc_vendor_or_credit = reader.IsDBNull(36) ? null : reader.GetValue(36).ToString(),
                              division = reader.IsDBNull(37) ? null : reader.GetValue(37).ToString(),
                              quantity_lpb = reader.IsDBNull(38) ? null : (-1 * Convert.ToDecimal(reader.GetValue(38))),
                              period = reader.IsDBNull(39) ? null : reader.GetValue(39).ToString(),
                              lpb_clearing = reader.IsDBNull(51) ? null : reader.GetValue(51).ToString(),
                              lpb_clear_date = reader.IsDBNull(41) ? null : reader.GetValue(41).ToString(),
                              voucher = reader.IsDBNull(42) ? null : reader.GetValue(42).ToString(),
                              cost_ctr = reader.IsDBNull(43) ? null : reader.GetValue(43).ToString(),
                              account_type = reader.IsDBNull(44) ? null : reader.GetValue(44).ToString(),
                              type = reader.IsDBNull(45) ? null : reader.GetValue(45).ToString(),
                              op_number = reader.IsDBNull(46) ? null : reader.GetValue(46).ToString(),
                              alt_bom = reader.IsDBNull(47) ? null : reader.GetValue(47).ToString(),
                              rcp_group = reader.IsDBNull(48) ? null : reader.GetValue(48).ToString(),
                              tr_ty = reader.IsDBNull(49) ? null : reader.GetValue(49).ToString(),
                              tr_s = reader.IsDBNull(50) ? null : reader.GetValue(50).ToString(),
                              // count = reader.IsDBNull(46) ? null : reader.GetValue(46).ToString(),
                              lpb_item_number = reader.IsDBNull(6) ? null : reader.GetValue(6).ToString(),
                              lpb_number = reader.IsDBNull(5) ? null : reader.GetValue(5).ToString(),
                              doc_currency  = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
                              mov_type = reader.IsDBNull(14) ? null : reader.GetValue(14).ToString(),
                              ref_doc_migo = reader.IsDBNull(16) ? null : reader.GetValue(16).ToString(),
                              is_active = is_active
                            });
                    }
                       // }
                }
              }
            }
          }
        }
        if (ListPoLpb.Count() > 0) {
          var SelectInsert = await _masterTable.UpdateMasterPoLPB(ListPoLpb);
          if(SelectInsert.Count() > 0){
            await _masterTable.InsertMasterLPB(SelectInsert);
          }
          
        }
        return new ObjectResult(new
        {
          status = 200,
          // count = result.Count(),
          message = "success",
        })
        { StatusCode = 200 };
                          
      }
      if(type == "PoService"){
        List<dynamic> ErrorPoService = new List<dynamic>();
        Console.WriteLine("po_svc");
                  using(var reader = ExcelReaderFactory.CreateReader(file)) {
                    while (reader.Read()) 
                    {
                      var vendor_number = reader.IsDBNull(29);
                      var po_number = reader.IsDBNull(28);
                      var po_item_number = reader.IsDBNull(0);
                      var po_name = reader.IsDBNull(7);
                      var currency = reader.IsDBNull(19);
                      var po_quantity = reader.IsDBNull(14);
                      var uom = reader.IsDBNull(15);
                      var net_price = reader.IsDBNull(18);
                      var p_org = reader.IsDBNull(30);
                      var plant_area = reader.IsDBNull(12);


                      if(vendor_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "vendor number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_item_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po item number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_name){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po name cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(currency){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "currency cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_quantity){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po quantity cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(uom){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "uom cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(net_price){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "net_price cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(p_org){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "p_org cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(plant_area){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "bussiness plant area cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(reader.Depth > 0){
                        string vendor_numberList = new Helpers.GlobalFunction().SplitSpace(reader.IsDBNull(29) ? null : reader.GetValue(29).ToString());
                        string p_org_id = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString();

                        //Console.WriteLine("ven" + vendor_numberList);

                          if(vendor_numberList != null){
                            var cekvendor = _masterTable.CekVendor(vendor_numberList);
                            var po_number_data = reader.IsDBNull(28) ? null : reader.GetValue(28).ToString();
                            var po_item_number_data = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                            if (string.IsNullOrEmpty(cekvendor)){
                              ErrorPoService.Add( 
                                new {
                                  po_number = po_number_data,
                                  po_item_number = po_item_number_data,
                                  error_message = "vendor: "+ vendor_numberList+" not registered "
                                }
                              );
                              // return new ObjectResult(new
                              // {
                              //   status = 400,
                              //   message = "vendor number: "+vendor_numberList+ " does not exist"
                              // })
                              // { StatusCode = 400 };
                              continue;
                            }
                            var company_id = _masterTable.GetCompanyIdByPurchOrg(p_org_id, vendor_numberList);
                            if (string.IsNullOrEmpty(company_id)){
                              ErrorPoService.Add( 
                                new {
                                  po_number = po_number_data,
                                  po_item_number = po_item_number_data,
                                  error_message = "purchase organization: "+p_org_id+ " of vendor: "+vendor_numberList+" not found in web database"
                                }
                              );
                              continue;
                              // return new ObjectResult(new
                              // {
                              //   status = 400,
                              //   message = "purchase organization: "+p_org_id+ " of vendor: "+vendor_numberList+" not found in web database"
                              // })
                              // { StatusCode = 400 };
                            }
                            //if (!string.IsNullOrEmpty(cekvendor)){
                              ListPoService.Add(new TempPoServiceModel(){
                              po_number = reader.IsDBNull(28) ? null : reader.GetValue(28).ToString(),
                              po_item_number = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                              type = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                              cat = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
                              pgr = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                              poh = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                              doc_date = reader.IsDBNull(5) ? null : reader.GetValue(5).ToString(),
                              material = reader.IsDBNull(6) ? null : reader.GetValue(6).ToString(),
                              po_name = reader.IsDBNull(7) ? null : reader.GetValue(7).ToString(),
                              material_group = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
                              d = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
                              i = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString(),
                              a = reader.IsDBNull(11) ? null : reader. GetValue(11).ToString(),
                              plnt = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString(),
                              s_loc = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
                              po_quantity = reader.IsDBNull(14) ? 0 : Convert.ToDecimal(reader.GetValue(14)),
                              uom = reader.IsDBNull(15) ? null : reader.GetValue(15).ToString(),
                              company_id = company_id,
                              quantity_2 = reader.IsDBNull(16) ? null : reader.GetValue(16).ToString(),
                              sku = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString(),
                              net_price = reader.IsDBNull(18) ? 0 : Convert.ToDecimal(reader.GetValue(18).ToString()),
                              currency = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),
                              per = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
                              quantity_3 = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),
                              open_tgt_qty = reader.IsDBNull(22) ? null : reader.GetValue(22).ToString(),
                              quantity_to_be_del = reader.IsDBNull(23) ? 0 : Convert.ToDecimal(reader.GetValue(23).ToString()),
                              amount_to_be_del = reader.IsDBNull(24) ? 0 : Convert.ToDecimal(reader.GetValue(24).ToString()),
                              quantity_to_be_inv = reader.IsDBNull(25) ? 0 : Convert.ToDecimal(reader.GetValue(25).ToString()),
                              amount_to_be_inv = reader.IsDBNull(26) ? 0 : Convert.ToDecimal(reader.GetValue(26).ToString()),
                              number = reader.IsDBNull(27) ? null : reader.GetValue(27).ToString(),
                              vendor_name = reader.IsDBNull(29) ? null : reader.GetValue(29).ToString(),
                              vendor_number = new Helpers.GlobalFunction().SplitSpace(reader.IsDBNull(29) ? null : reader.GetValue(29).ToString()),
                              p_org = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString(),
                              l = reader.IsDBNull(31) ? null : reader.GetValue(31).ToString(),
                              tracking_number = reader.IsDBNull(32) ? null : reader.GetValue(32).ToString(),
                              agmt = reader.IsDBNull(33) ? null : reader.GetValue(33).ToString(),
                              item_2 = reader.IsDBNull(34) ? null : reader.GetValue(34).ToString(),
                              targ_val = reader.IsDBNull(35) ? null : reader.GetValue(35).ToString(),
                              tot_open_val = reader.IsDBNull(36) ? null : reader.GetValue(36).ToString(),
                              open_value = reader.IsDBNull(37) ? null : reader.GetValue(37).ToString(),
                              rel_value = reader.IsDBNull(38) ? null : reader.GetValue(38).ToString(),
                              rel_qty = reader.IsDBNull(39) ? null : reader.GetValue(39).ToString(),
                              vp_start = reader.IsDBNull(40) ? null : reader.GetValue(40).ToString(),
                              vper_end = reader.IsDBNull(41) ? null : reader.GetValue(41).ToString(),
                              quot_dd_in = reader.IsDBNull(42) ? null : reader.GetValue(42).ToString(),
                              s = reader.IsDBNull(43) ? null : reader.GetValue(43).ToString(),
                              coll_number = reader.IsDBNull(44) ? null : reader.GetValue(44).ToString(),
                              ctl = reader.IsDBNull(45) ? null : reader.GetValue(45).ToString(),
                              info_rec = reader.IsDBNull(46) ? null : reader.GetValue(46).ToString(),
                              number_2 = reader.IsDBNull(47) ? null : reader.GetValue(47).ToString(),
                              grp = reader.IsDBNull(48) ? null : reader.GetValue(48).ToString(),
                              strat = reader.IsDBNull(49) ? null : reader.GetValue(49).ToString(),
                              release = reader.IsDBNull(50) ? null : reader.GetValue(50).ToString(),
                              rel = reader.IsDBNull(51) ? null : reader.GetValue(51).ToString(),
                              ist_loc = reader.IsDBNull(52) ? null : reader.GetValue(52).ToString(),
                              vendor_name_2 = reader.IsDBNull(53) ? null : reader.GetValue(53).ToString(),
                              opu = reader.IsDBNull(54) ? null : reader.GetValue(54).ToString(),
                              tx = reader.IsDBNull(55) ? null : reader.GetValue(55).ToString(),
                              tax_jur = reader.IsDBNull(56) ? null : reader.GetValue(56).ToString(),
                              net_value = reader.IsDBNull(57) ? null : reader.GetValue(57).ToString(),
                              i_2 = reader.IsDBNull(58) ? null : reader.GetValue(58).ToString(),
                              notified = reader.IsDBNull(59) ? null : reader.GetValue(59).ToString(),
                              stk_seg = reader.IsDBNull(60) ? null : reader.GetValue(60).ToString(),
                              req_seg = reader.IsDBNull(61) ? null : reader.GetValue(61).ToString(),
                              s_2 = reader.IsDBNull(62) ? null : reader.GetValue(62).ToString(),
                              conf_item_number = reader.IsDBNull(63) ? null : reader.GetValue(63).ToString(),
                              sort_number = reader.IsDBNull(64) ? null : reader.GetValue(64).ToString(),
                              ext_h_cat = reader.IsDBNull(65) ? null : reader.GetValue(65).ToString(),
                              ru = reader.IsDBNull(66) ? null : reader.GetValue(66).ToString(),
                              reqmt_price = reader.IsDBNull(67) ? null : reader.GetValue(67).ToString(),
                              smart_no = reader.IsDBNull(68) ? null : reader.GetValue(68).ToString(),
                              char1 = reader.IsDBNull(69) ? null : reader.GetValue(69).ToString(),
                              char_desc_1 = reader.IsDBNull(70) ? null : reader.GetValue(70).ToString(),
                              char2 = reader.IsDBNull(71) ? null : reader.GetValue(71).ToString(),
                              char_desc_2 = reader.IsDBNull(72) ? null : reader.GetValue(72).ToString(),
                              char3 = reader.IsDBNull(73) ? null : reader.GetValue(73).ToString(),
                              char_desc_3 = reader.IsDBNull(74) ? null : reader.GetValue(74).ToString()
                              });
                            //}
                            //Console.WriteLine("cekvendor"+ cek);
                          }
                      }
                    }
                    }
                    if (ListPoService.Count() > 0) {
                      var SelectInsert = await _masterTable.UpdateMasterPoSVC(ListPoService);
                      if(SelectInsert.Count() > 0){
                        Console.WriteLine("insert po_ master");
                        await _masterTable.InsertMasterService(SelectInsert);
                      }
                      
                    }
                    if (ErrorPoService.Count > 0) {
                      return new ObjectResult(new
                      {
                        status = 400,
                        message = "Some master po service data is failed to proceed",
                        total_po = ErrorPoService.Count,
                        failed_po_service = ErrorPoService

                      })
                      { StatusCode = 400 };
                    }
                    return new ObjectResult(new
                    {
                      status = 200,
                      // count = result.Count(),
                      message = "success",
                    })
                    { StatusCode = 200 };
        }
      if(type == "PoOpen"){
        Console.WriteLine("po_open");
                  using(var reader = ExcelReaderFactory.CreateReader(file)) {
                    while (reader.Read()) 
                    {
                      var vendor_number = reader.IsDBNull(29);
                      var po_number = reader.IsDBNull(28);
                      var po_item_number = reader.IsDBNull(0);
                      var po_name = reader.IsDBNull(7);
                      var currency = reader.IsDBNull(19);
                      var po_quantity = reader.IsDBNull(14);
                      var uom = reader.IsDBNull(15);

                       if(vendor_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "vendor number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_item_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po item number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_name){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po name cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(currency){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "currency cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(po_quantity){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "po quantity cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(uom){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "uom cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(reader.Depth > 0){
                       // Console.WriteLine("readerr getvalue: ",reader.GetValue(29));
                        ListPoOpen.Add(new PoOpenModel(){
                        po_item_number = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                        type = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                        cat = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
                        pgr = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                        poh = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                        doc_date = reader.IsDBNull(5) ? null : reader.GetValue(5).ToString(),
                        material = reader.IsDBNull(6) ? null : reader.GetValue(6).ToString(),
                        po_name = reader.IsDBNull(7) ? null : reader.GetValue(7).ToString(),
                        material_group = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
                        d = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
                        i = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString(),
                        a = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString(),
                        plnt = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString(),
                        s_loc = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
                        po_quantity = reader.IsDBNull(14) ? 0 : Convert.ToDecimal(reader.GetValue(14)),
                        uom = reader.IsDBNull(15) ? null : reader.GetValue(15).ToString(),
                        quantity_2 = reader.IsDBNull(16) ? null : reader.GetValue(16).ToString(),
                        sku = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString(),
                        net_price = reader.IsDBNull(18) ? null : reader.GetValue(18).ToString(),
                        currency = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),
                        per = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
                        quantity_3 = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),
                        open_tgt_qty = reader.IsDBNull(22) ? null : reader.GetValue(22).ToString(),
                        quantity_to_be_del = reader.IsDBNull(23) ? 0 : Convert.ToDecimal(reader.GetValue(23).ToString()),
                        amount_to_be_del = reader.IsDBNull(24) ? 0 : Convert.ToDecimal(reader.GetValue(24).ToString()),
                        quantity_to_be_inv = reader.IsDBNull(25) ? 0 : Convert.ToDecimal(reader.GetValue(25).ToString()),
                        amount_to_be_inv = reader.IsDBNull(26) ? 0 : Convert.ToDecimal(reader.GetValue(26).ToString()),
                        number = reader.IsDBNull(27) ? null : reader.GetValue(27).ToString(),
                        po_number = reader.IsDBNull(28) ? null : reader.GetValue(28).ToString(),
                        vendor_name = reader.IsDBNull(29) ? null : reader.GetValue(29).ToString(),
                        vendor_number = new Helpers.GlobalFunction().SplitSpace(reader.IsDBNull(29) ? null : reader.GetValue(29).ToString()),
                        p_org = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString(),
                        l = reader.IsDBNull(31) ? null : reader.GetValue(31).ToString(),
                        tracking_number = reader.IsDBNull(32) ? null : reader.GetValue(32).ToString(),
                        agmt = reader.IsDBNull(33) ? null : reader.GetValue(33).ToString(),
                        item_2 = reader.IsDBNull(34) ? null : reader.GetValue(34).ToString(),
                        targ_val = reader.IsDBNull(35) ? null : reader.GetValue(35).ToString(),
                        tot_open_val = reader.IsDBNull(36) ? null : reader.GetValue(36).ToString(),
                        open_value = reader.IsDBNull(37) ? null : reader.GetValue(37).ToString(),
                        rel_value = reader.IsDBNull(38) ? null : reader.GetValue(38).ToString(),
                        rel_qty = reader.IsDBNull(39) ? null : reader.GetValue(39).ToString(),
                        vp_start = reader.IsDBNull(40) ? null : reader.GetValue(40).ToString(),
                        vper_end = reader.IsDBNull(41) ? null : reader.GetValue(41).ToString(),
                        quot_dd_in = reader.IsDBNull(42) ? null : reader.GetValue(42).ToString(),
                        s = reader.IsDBNull(43) ? null : reader.GetValue(43).ToString(),
                        coll_number = reader.IsDBNull(44) ? null : reader.GetValue(44).ToString(),
                        ctl = reader.IsDBNull(45) ? null : reader.GetValue(45).ToString(),
                        info_rec = reader.IsDBNull(46) ? null : reader.GetValue(46).ToString(),
                        number_2 = reader.IsDBNull(47) ? null : reader.GetValue(47).ToString(),
                        grp = reader.IsDBNull(48) ? null : reader.GetValue(48).ToString(),
                        strat = reader.IsDBNull(49) ? null : reader.GetValue(49).ToString(),
                        release = reader.IsDBNull(50) ? null : reader.GetValue(50).ToString().Trim(),
                        rel = reader.IsDBNull(51) ? null : reader.GetValue(51).ToString(),
                        ist_loc = reader.IsDBNull(52) ? null : reader.GetValue(52).ToString(),
                        vendor_name_2 = reader.IsDBNull(53) ? null : reader.GetValue(53).ToString(),
                        opu = reader.IsDBNull(54) ? null : reader.GetValue(54).ToString(),
                        tx = reader.IsDBNull(55) ? null : reader.GetValue(55).ToString(),
                        tax_jur = reader.IsDBNull(56) ? null : reader.GetValue(56).ToString(),
                        net_value = reader.IsDBNull(57) ? null : reader.GetValue(57).ToString(),
                        i_2 = reader.IsDBNull(58) ? null : reader.GetValue(58).ToString(),
                        notified = reader.IsDBNull(59) ? null : reader.GetValue(59).ToString(),
                        stk_seg = reader.IsDBNull(60) ? null : reader.GetValue(60).ToString(),
                        req_seg = reader.IsDBNull(61) ? null : reader.GetValue(61).ToString(),
                        s_2 = reader.IsDBNull(62) ? null : reader.GetValue(62).ToString(),
                        conf_item_number = reader.IsDBNull(63) ? null : reader.GetValue(63).ToString(),
                        sort_number = reader.IsDBNull(64) ? null : reader.GetValue(64).ToString(),
                        ext_h_cat = reader.IsDBNull(65) ? null : reader.GetValue(65).ToString(),
                        ru = reader.IsDBNull(66) ? null : reader.GetValue(66).ToString(),
                        reqmt_price = reader.IsDBNull(67) ? null : reader.GetValue(67).ToString(),
                        smart_no = reader.IsDBNull(68) ? null : reader.GetValue(68).ToString(),
                        char1 = reader.IsDBNull(69) ? null : reader.GetValue(69).ToString(),
                        char_desc_1 = reader.IsDBNull(70) ? null : reader.GetValue(70).ToString(),
                        char2 = reader.IsDBNull(71) ? null : reader.GetValue(71).ToString(),
                        char_desc_2 = reader.IsDBNull(72) ? null : reader.GetValue(72).ToString(),
                        char3 = reader.IsDBNull(73) ? null : reader.GetValue(73).ToString(),
                        char_desc_3 = reader.IsDBNull(74) ? null : reader.GetValue(74).ToString()
                        });
                      }
                    }
              }
                      var SelectInsert = await _masterTable.UpdateMasterPoOpen(ListPoOpen);
                      Console.WriteLine("dict"+ SelectInsert);
                      if(SelectInsert.Count() > 0){
                        await _masterTable.InsertMasterPoOpen(ListPoOpen);
                      }
                      
                      return new ObjectResult(new
                      {
                        status = 200,
                        // count = result.Count(),
                        message = "success",
                      })
                      { StatusCode = 200 };

        }
      if(type == "Vendor"){
                  List<dynamic> NoVendorBankDataList = new List<dynamic>();
                  using(var reader = ExcelReaderFactory.CreateReader(file)) {
                      Console.WriteLine("vendor");
                    while (reader.Read()) 
                    {
                      var vendor_number = reader.IsDBNull(3);
                      var email = reader.IsDBNull(12);
                      var currency_id = reader.IsDBNull(11);
                      var purchase_organization_id = reader.IsDBNull(6);
                      if(vendor_number){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "vendor number cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(email){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "email cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(currency_id){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "currency cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(purchase_organization_id){
                         return new ObjectResult(new
                          {
                            status = 400,
                            message = "purchase_organization_id cannot be null in the file"
                          })
                          { StatusCode = 400 };
                      }
                      if(reader.Depth > 0){
                        bool needRequestChange = false;
                        var officePhone = reader.IsDBNull(24) ? null : reader.GetValue(24).ToString();
                        var mobilePhone = reader.IsDBNull(25) ? null : reader.GetValue(25).ToString();
                        var phone = officePhone + " ; " + mobilePhone;
                        if(string.IsNullOrEmpty(officePhone + mobilePhone)){
                          phone = "-";
                          //Console.WriteLine("phone");
                        }
                        var vendorTypeId = reader.IsDBNull(28) ? null : reader.GetValue(28).ToString();
                        var taxTypeId = reader.IsDBNull(39) ? null : reader.GetValue(39).ToString();
                        var reverseTax = new Helpers.GlobalFunction().Reverse(reader.IsDBNull(39) ? "0W" : reader.GetValue(39).ToString());

                        
                        var taxNumberTypeId = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString();
                        var additional_street_address = reader.IsDBNull(15) ?  "-"  : reader.GetValue(15).ToString();
                        var postal_code = "-";
                        var country = reader.IsDBNull(17) ? null : reader.GetValue(17).ToString();

                        int DataVendorTypeId = _masterTable.VendorTypeId(vendorTypeId);
                        int DataTaxTypeId = _masterTable.TaxTypeId(reverseTax);
                        if (DataTaxTypeId == 69) {
                          DataTaxTypeId = 1;
                        }
                        int DataTaxNumberTypeId = _masterTable.TaxNumberTypeId(taxNumberTypeId);
                        var change_request_note = "";
                        var city = "-";
                        var contact_person = "-";
                        var street_address = "-";
                        var npwp = "-";
                        // var request_note = 
                        if (reader.IsDBNull(16)){
                          change_request_note += "City; ";
                        } else city = reader.GetValue(16).ToString();
                        if (reader.IsDBNull(23)){
                          change_request_note += "Contact Person; ";
                        } else contact_person = reader.GetValue(23).ToString();
                        if (reader.IsDBNull(14)){
                          change_request_note += "Street Address; ";
                        } else street_address = reader.GetValue(14).ToString();
                        if (reader.IsDBNull(18)){
                          if (DataTaxNumberTypeId == 1 || DataTaxNumberTypeId == 2)
                            change_request_note += "VAT Reg Number; ";
                        } else npwp = reader.GetValue(18).ToString();

                        bool isDataBankExist = false;
                        string vendor_numberList = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString();
                        // Console.WriteLine("vendor_number :"+vendor_numberList);
                        string BankKey = reader.IsDBNull(13)? null: reader.GetValue(13).ToString();
                        var currency = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString();
                        var currencyExist = _masterTable.Currency(currency);
                        var num = BankKey;
                        string? bank_id = null;
                        decimal? bank_account = 0;
                        var bank_holder = "";
                        string? fbank_id = null;
                        var fbank_account = "";
                        var fbank_holder = "";
                        
                        if (!string.IsNullOrEmpty(BankKey)) {
                          //if Bank Key not null
                          
                          if(BankKey.Length ==5){
                            num = "00"+BankKey;
                          }
                          if(BankKey.Length ==6){
                            num = "0"+BankKey;
                          }
                          if(BankKey.Length > 30) {
                            NoVendorBankDataList.Add( 
                              new {
                                vendor_number = vendor_numberList,
                                error_message = "Bank Key length is more than 30 character"
                              }
                            );
                            continue;
                          }
                          //Console.WriteLine("num "+ num);
                          var cekBankKey = _vendor.CekBankExist(num);
                          //Console.WriteLine("cekbank "+ cekBankKey);
                          if (cekBankKey == false){
                              if(country != "ID"){
                                BankModel data = new BankModel(){
                                  bank_id =num,
                                  name = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString(),
                                  country_of_origin = country,
                                  swift_code = "-"
                                };
                                _vendor.InsertBank(data);
                              }
                            }
                        }else{
                            NoVendorBankDataList.Add( 
                              new {
                                vendor_number = vendor_numberList,
                                error_message = "Bank Key is Empty"
                              }
                            );
                            continue;
                          }
                            
                          var cekBankkeyAfterInsert = _vendor.GetBank(num);
                          //Console.WriteLine("cek"+ cekBankkeyAfterInsert);
                          if(!string.IsNullOrEmpty(cekBankkeyAfterInsert)){
                              isDataBankExist = true;
                              if (country == "ID"){
                                bank_id = string.IsNullOrEmpty(cekBankkeyAfterInsert)? null: cekBankkeyAfterInsert;
                                bank_account = reader.IsDBNull(8) ? null : Convert.ToDecimal(reader.GetValue(8).ToString().Replace("-","").Replace(" ","").Replace(".",""));
                                bank_holder = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString();
                              }else{
                                fbank_id = string.IsNullOrEmpty(cekBankkeyAfterInsert)? null: cekBankkeyAfterInsert;
                                fbank_account = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString();
                                fbank_holder = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString();
                              }
                          }
                          else{
                            isDataBankExist = false;
                            NoVendorBankDataList.Add( 
                                  new {
                                    vendor_number = vendor_numberList,
                                    error_message = "Bank Key : "+ num + " is not recorded can not insert"
                                  }
                                );
                            continue;
                          }

                        if (DataTaxTypeId == 0) {
                          isDataBankExist = false;
                          Console.WriteLine("tax :"+reverseTax+" ;reverse :"+DataTaxTypeId);
                          NoVendorBankDataList.Add( 
                            new {
                              vendor_number = vendor_numberList,
                              error_message = "tax type  : "+ reverseTax + " is not recorded in web database"
                            }
                          );
                          continue;
                          
                        }
                        if (DataTaxNumberTypeId == 0) {
                          isDataBankExist = false;
                          Console.WriteLine("tax number type:"+taxNumberTypeId+" ;datataxtype :"+DataTaxNumberTypeId);
                          NoVendorBankDataList.Add( 
                            new {
                              vendor_number = vendor_numberList,
                              error_message = "tax number type  : "+ taxNumberTypeId + " is not recorded in web database"
                            }
                          );
                          continue;
                          
                        }
                        if (!currencyExist ) {
                          isDataBankExist = false;
                          Console.WriteLine("currency: "+currency);
                          NoVendorBankDataList.Add( 
                            new {
                              vendor_number = vendor_numberList,
                              error_message = "currency  : "+ currency + " is not recorded in web database"
                            }
                          );
                          continue;
                        }
                        if (isDataBankExist) {

                            string topId =  reader.IsDBNull(26) ? null : reader.GetValue(26).ToString();
                            string CurrencyID = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString();
                            if(vendor_numberList != null){
                              int index = ListVendor.FindIndex(f => f.vendor_number == vendor_numberList);
                              var cekvendor = _masterTable.CekVendor(vendor_numberList);
                              // var cekTopId = _masterTable.Top(topId);
                              var purchaseOrg = reader.IsDBNull(6);
                              //var cekTaxType = _masterTable.CekSapTaxType(reverseTax);

                              // Console.WriteLine("index: "+ cekvendor + vendor_numberList);

                                //Console.WriteLine("masuk cek");

                                  if(change_request_note != "" ){
                                    //Console.WriteLine("undefined");
                                    needRequestChange = true;
                                    change_request_note += " need to be filled";
                                  }

                                  bool is_locked = false;
                                  int change_request_status = 0;

                                    if(needRequestChange){
                                      //Console.WriteLine("need");
                                      is_locked = true;
                                      change_request_status = 1; 
                                    } else change_request_note = null;
                              if (index < 0){
                                  ListVendor.Add(new VendorModelRpa(){
                                  is_locked = is_locked,
                                  change_request_status = change_request_status,
                                  change_request_note = change_request_note,
                                  search_term = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
                                  vendor_number = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                                  currency_id = currency,
                                  top_id = reader.IsDBNull(26) ? null : reader.GetValue(26).ToString(),
                                  name = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                                  street_address = street_address,
                                  additional_street_address = additional_street_address,
                                  city = city,
                                  country_id = country,
                                  telephone = phone,
                                  email = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString(),
                                  tax_type_id = DataTaxTypeId == 0 ? 1 : DataTaxTypeId,
                                  tax_number_type_id = DataTaxNumberTypeId,
                                  npwp = npwp.Replace(".","").Replace("-",""),
                                  id_card_number = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString().Replace(".","").Replace("-",""),
                                  vendor_type_id = DataVendorTypeId,
                                  postal_code = postal_code,
                                  title_id = "Company",
                                  contact_person = contact_person,
                                  localidr_bank_id = string.IsNullOrWhiteSpace(bank_id) ? null : bank_id,
                                  localidr_bank_account = string.IsNullOrEmpty(Convert.ToDecimal(bank_account).ToString()) ? null : bank_account,
                                  localidr_account_holder = string.IsNullOrEmpty(bank_holder) ? null : bank_holder,
                                  foreign_bank_id = string.IsNullOrWhiteSpace(fbank_id) ? null : fbank_id,
                                  foreign_bank_account = string.IsNullOrEmpty(fbank_account) ? null : fbank_account,
                                  foreign_account_holder = string.IsNullOrEmpty(fbank_holder) ? null : fbank_holder,
                                });
                                ListlogVendor.Add(new LogVendorModel(){
                                    vendor_number = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                                    process = "Insert Vendor Master",
                                    note = null,
                                    created_by = 9,
                                });
                                
                              
                              if (string.IsNullOrEmpty(cekvendor)){
                                var GeneratePwd = new Helpers.GlobalFunction().GenerateRandomChar(6);
                                var EmailList = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString();
                                var vendorNumberList = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString();
                                var NameList = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString();
                                var encryptPassword = new Helpers.GlobalFunction().GetMD5(GeneratePwd);

                                  // String path_mail_template = _iHostingEnvironment.ContentRootPath + "/MailTemplates/activation-" + _current_language + ".html";
                                  // String content_template = await new Helpers.EmailFunction().GenerateEmailTemplate(path_mail_template);
                                  // //List<String> email_list = new Helpers.EmailFunction().GetEmailList(ListUser.email);
                                
                                  //   Dictionary<String, String> email_content_param = new Dictionary<string, string>();
                                  //   email_content_param.Add("email", EmailList);
                                  //   email_content_param.Add("year", DateTime.Now.Year.ToString());
                                  //   email_content_param.Add("vendor_name", NameList);
                                  //   email_content_param.Add("Username", vendorNumberList);
                                  //   email_content_param.Add("Password", GeneratePassword);

                                  //   String email_content = new Helpers.EmailFunction().GenerateEmailContentFromTemplate(content_template, email_content_param);

                                  //   new Helpers.EmailFunction().sending_email(EmailList, (_current_language == "id" ? "Aktivasi akun" : "Activation Account"), email_content);
                                //update password atau gimana? 
                                ListUser.Add(new UserModel(){
                                vendor_number = vendor_numberList,
                                name = NameList,
                                email = EmailList,
                                username = vendor_numberList,
                                password = encryptPassword,
                                role_id = 2,
                                });

                                  
                              }
                            }
                            
                            
                              // insert ke dalam tr_company_relation one - many
                                ListCompanyRelation.Add(new CompanyRelationModel(){
                                vendor_number = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                                company_id = reader.IsDBNull(5) ? null : reader.GetValue(5).ToString(),
                                purchase_organization_id = reader.IsDBNull(6) ? null : reader.GetValue(6).ToString(),
                                });
                            }
                          }
                        }
                        // } else {
                        //   return new ObjectResult(new
                        //   {
                        //     status = 200,
                        //     // count = result.Count(),
                        //     message = "success",
                        //   })
                        //   { StatusCode = 200 };
                          
                        // }
                        //Console.WriteLine("tax "+ JsonSerializer.Serialize(NoVendorBankDataList));
                    }
              }
                  //Console.WriteLine(JsonSerializer.Serialize(ListVendor));
                  // Refactor jadi proses transaction
                  Console.WriteLine("vendor cout: "+ListVendor.Count());
                  if (ListVendor.Count() > 0) {
                    Console.WriteLine("update vendor master");
                    var SelectInsert = await _masterTable.UpdateMasterVendor(ListVendor);
                    if(SelectInsert.Count() > 0){
                      Console.WriteLine("insert vendor master");
                      await _masterTable.InsertMasterVendor(SelectInsert);
                    }
                    
                  }
                  // await _masterTable.InsertMasterVendor(ListVendor);
                  await _masterTable.DeleteCompanyRelations(ListCompanyRelation);
                  await _masterTable.InsertCompanyRelations(ListCompanyRelation);
                  await _masterTable.InsertUserMaster(ListUser);
                  await _masterTable.InsertMasterLogVendor(ListlogVendor);
                  if (NoVendorBankDataList.Count > 0 ) {
                    return new ObjectResult(new
                    {
                      status = 400,
                      message = "Some vendor data is failed to proceed",
                      total_row = NoVendorBankDataList.Count,
                      failed_vendor = NoVendorBankDataList

                    })
                    { StatusCode = 400 }; }
                      //await _masterTable.InsertMasterTempVendor(ListTempVendor);
          

        }
        
        return new ObjectResult(new
        {
          status = 200,
          // count = result.Count(),
          data = "success",
        })
        { StatusCode = 200 };
    }

    [Route("api/rpa/InsertMIRONumber")]
    [HttpPost, ActionName("InsertMIRONumber")]
    public IActionResult InsertMIRONumber(String ticket_number, [FromForm] UpdateMIRONumberPayloadBody payloadBody)
    {
      if (payloadBody.status_rpa == null){
        return new ObjectResult(new
        {
          msg = "status_rpa is required", param = "status_rpa", location = "body" 
        })
        { StatusCode = 400 };
      }
      if(payloadBody.status_rpa == "0"){
        //  Console.WriteLine("masuk disini: "+String.IsNullOrEmpty(payloadBody.rpa_status_description));
          if  (String.IsNullOrEmpty(payloadBody.rpa_status_description)) {
              return BadRequest(new { status = 400, message = "Bad Request", error =   "rpa_status_description is required"});
            // error.items.Add(new BadRequesBody { msg = "rpa_status_description is required", param = "rpa_status_description", location = "body" });
            // if (error.items.Count > 0)
            // {
            // }
          } 
      } else {
        payloadBody.date_miro_input = Convert.ToDateTime(payloadBody.date_miro_input);

      }
        Console.WriteLine(payloadBody.date_miro_input + " ini date");
      var result = _ticket.RpaUpdateMiroNumber(ticket_number, payloadBody);
      var status = 404;
      var message = "No Data Updated";
      if (result.Result > 0) {
        status = 200;
        message = "Successfully Updating Data";
      }

      return new ObjectResult(new
      {
        status = status,
        updated_row = result.Result,
        updated_info_detail = result,
        message = message
        // count = result.Count(),
        // data = result,
      })
      { StatusCode = 200 };
    }
    
    [Route("api/rpa/PaymentRemittance")]
    [HttpPost, ActionName("PaymentRemittance")]
    public IActionResult PaymentRemittance([FromForm] IFormFile xlsFile)
    {
      // REMITTANCE
      // is_paid = 1, verification_status_id = 13,remittance_date = from excel, remit_number = excel WHERE miro_number = miro_number and vendor number = vendor number

      RemittanceXlsRequest isidata = new RemittanceXlsRequest();
      List<RemittanceXlsRequest> IsiDataList = new List<RemittanceXlsRequest>();
      var filename = xlsFile.FileName;
      var file = xlsFile.OpenReadStream();
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
      
 
      using(var reader = ExcelReaderFactory.CreateReader(file)) {
        
        // var i = 1;
        while (reader.Read()) 
        {
          
          if(reader.Depth > 0){
            IsiDataList.Add(new RemittanceXlsRequest {
            vendor_number = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
            miro_number = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
            remmitance_number = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
            clearing_date = reader.IsDBNull(3) ? null : Convert.ToDateTime(reader.GetValue(3).ToString().Replace(".", "-"))
            });
          }
        }
      }

      var updateRemittance = _ticket.RpaPaymentRemittance(IsiDataList);


      return new ObjectResult(new
      {
        status = 200,
        updated_row = updateRemittance.Result,
        updated_info_detail = updateRemittance
      })
      { StatusCode = 200 };
    }

    [Route("api/rpa/RejectedTickets")]
    [HttpGet, ActionName("RejectedTickets")]
    public IActionResult RejectedTickets(String vendor_number)

    {
      // Rejected TICKET
      // WHERE position_data = 9 AND verification_status_id = 15

      var result = _ticket.RpaGetRejectedTickets();

      return new ObjectResult(new
      {
        status = 200,
        // count = result.Count(),
        data = result,
      })
      { StatusCode = 200 };
    }

    [Route("api/rpa/TicketReversal")]
    [HttpPost, ActionName("TicketReversal")]
    public IActionResult TicketReversal(string miro_number, [FromBody] TicketReversalRequest requestBody)
    {

      // Console.WriteLine(JsonSerializer.Serialize(requestBody));
      // REVERSAL
      // 1. IF status_rpa 1 => verification_status_id = 18
      // 2. IF status rpa 0 => nothing to do
      if (requestBody.status_rpa == null){
        return new ObjectResult(new
        {
          msg = "status_rpa is required", param = "status_rpa", location = "body" 
        })
        { StatusCode = 400 };
      }


      
      if(requestBody.status_rpa == "0"){
        if (requestBody.rpa_status_description == null){
          return new ObjectResult(new
          {
            msg = "rpa_status_description is required", param = "rpa_status_description", location = "body" 
          })
          { StatusCode = 400 };
        }
        // return new ObjectResult(new
        // {
        //   status = 200,
        //   message = "Nothing to do",
        //   status_rpa = 0
          
        // })
        // { StatusCode = 200 };
      }

      if(String.IsNullOrEmpty(miro_number)){
        return new ObjectResult(new
        {
          status = 400,
          message = "miro_number can't be null or empty"
        })
        { StatusCode = 200 };
      }

      var updateReversal = _ticket.RpaTicketReversal(miro_number, requestBody);


      return new ObjectResult(new
      {
        status = 200,
        updated_row = updateReversal.Result,
        updated_info_detail = updateReversal
      })
      { StatusCode = 200 };
    }

    // SIMULATE 
    // get
    // 1. tr_ticket WHERE request simulate = 1
    [Route("api/rpa/SimulateTickets")]
    [HttpGet, ActionName("SimulateTickets")]
    public IActionResult SimulateTickets(String vendor_number)
    {

      var result = _ticket.RpaGetTickets(2);

      return new ObjectResult(new
      {
        status = 200,
        count = result.Count(),
        result,
      })
      { StatusCode = 200 };
    }

    // put 
    // 1. UPDATE tr_ticket SET request simulate = 0 where ticket_number = ticket_number
    [Route("api/rpa/SimulateResult")]
    [HttpPost, ActionName("SimulateResult")]
    public IActionResult SimulateResult([FromForm] IFormFile xlsFile, [FromForm]  SimulateXlsRequest reqBody)
    {
      SimulateXlsRequest isidata = new SimulateXlsRequest();
      List<SimulateXlsRequest> IsiDataList = new List<SimulateXlsRequest>();
      var error = new BadRequestMessage();
      string ticket_number = "";
      var request_simulate = 0;
      var verification_status_id = 20;
      string status_rpa = reqBody.status_rpa;
      string rpa_status_description = reqBody.rpa_status_description;
      if (status_rpa == null){
        return new ObjectResult(new
        {
          msg = "status_rpa is required", param = "status_rpa", location = "body" 
        })
        { StatusCode = 400 };
      }

      if (status_rpa == "0") {
        ticket_number = reqBody.ticket_number;
        if (ticket_number == null) {
                  return new ObjectResult(new
        {
          msg = "ticket_number is required", param = "ticket_number", location = "body" 
        })
        { StatusCode = 400 };
          //error.items.Add(new BadRequesBody { msg = "ticket_number is required", param = "ticket_number", location = "body" });
        }
        if (rpa_status_description == null){
                            return new ObjectResult(new
        {
          msg = "rpa_status_description is required", param = "rpa_status_description", location = "body" 
        })
        { StatusCode = 400 };
          // error.items.Add(new BadRequesBody { msg = "rpa_status_description is required", param = "rpa_status_description", location = "body" });
        }
        verification_status_id = 21;
        IsiDataList.Add(new SimulateXlsRequest {
          ticket_number = ticket_number,
          verification_status_id = 21,
          rpa_status_description = rpa_status_description,
          request_simulate = request_simulate,
          status_rpa = status_rpa,
        });
      } else {
        verification_status_id = 20;
        if (xlsFile is null){
          return new ObjectResult(new
          {
            status = 400,
            message = "xls file not uploaded"
          })
          { StatusCode = 400 };
        }
        var filename = xlsFile.FileName;
        var file = xlsFile.OpenReadStream();
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using(var reader = ExcelReaderFactory.CreateReader(file)) {
          
          // var i = 1;
          while (reader.Read()) 
          {

            if(reader.Depth == 0 && !reader.IsDBNull(0)){
              ticket_number = reader.GetValue(0).ToString();
            }
            
            if(reader.Depth > 0){
              IsiDataList.Add(new SimulateXlsRequest {
              ticket_number = ticket_number,
              position = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
              account_type= reader.IsDBNull(2) ? null : reader.GetValue(2).ToString(),
              g_l = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
              act_mat_ast_vndr = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
              amount = reader.IsDBNull(5) ? null : Convert.ToDecimal(reader.GetValue(5).ToString()),
              currency = reader.IsDBNull(6) ? null : reader.GetValue(6).ToString(),
              po_number = reader.IsDBNull(7) ? null : reader.GetValue(7).ToString(),
              po_item_number = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
              tax_code = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
              jurisd_code = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString(),
              tax_date = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString(),
              bus_area = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString(),
              cost_center = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
              co_area = reader.IsDBNull(14) ? null : reader.GetValue(14).ToString(),
              order_1 = reader.IsDBNull(15) ? null : reader.GetValue(15).ToString(),
              asset = reader.IsDBNull(16) ? null : reader.GetValue(16).ToString(),
              sub_number = reader.IsDBNull(17) ? null : reader.GetValue(17).ToString(),
              asset_val = reader.IsDBNull(18) ? null : reader.GetValue(18).ToString(),
              material = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),
              quantity = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
              base_unit = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),
              plant = reader.IsDBNull(22) ? null : reader.GetValue(22).ToString(),
              part_bus_ar = reader.IsDBNull(23) ? null : reader.GetValue(23).ToString(),
              cost_object = reader.IsDBNull(24) ? null : reader.GetValue(24).ToString(),
              profit_segment = reader.IsDBNull(25) ? null : reader.GetValue(25).ToString(),
              profit_center = reader.IsDBNull(26) ? null : reader.GetValue(26).ToString(),
              partner_prctr = reader.IsDBNull(27) ? null : reader.GetValue(27).ToString(),
              wbs_element = reader.IsDBNull(28) ? null : reader.GetValue(28).ToString(),
              network = reader.IsDBNull(29) ? null : reader.GetValue(29).ToString(),
              sales_order = reader.IsDBNull(30) ? null : reader.GetValue(30).ToString(),
              sls_item = reader.IsDBNull(31) ? null : reader.GetValue(31).ToString(),
              vendor_number = reader.IsDBNull(32) ? null : reader.GetValue(32).ToString(),
              posting_key = reader.IsDBNull(33) ? null : reader.GetValue(33).ToString(),
              posting_line_id = reader.IsDBNull(34) ? null : reader.GetValue(34).ToString(),
              created_auto = reader.IsDBNull(35) ? null : reader.GetValue(35).ToString(),
              status_rpa = status_rpa, 
              rpa_status_description = null,
              request_simulate = request_simulate,
              verification_status_id = verification_status_id
              });
            }
          }
        }
      }

      var SimulateRes = _ticket.RpaSimulateResult(IsiDataList, status_rpa);

      return new ObjectResult(new
      {
        status = 200,
        // count = result.Count(),
        result = SimulateRes
      })
      { StatusCode = 200 };
    }


  public IActionResult TestExcel() {
  Console.WriteLine("hello excel");
  var fileName = "./wwwroot/m1.xlsx";
  // For .net core, the next line requires the NuGet package, 
  // System.Text.Encoding.CodePages
  System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
  using(var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read)) {
    using(var reader = ExcelReaderFactory.CreateReader(stream)) {

      while (reader.Read()) //Each row of the file
      {
        Console.WriteLine(reader.GetValue(2).ToString());
      }
    }
  }
  return new ObjectResult(new {
    status = 200,
  }) {
    StatusCode = 200
  };
}
  }
}