using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Interfaces;
using tufol.Helpers;
using tufol.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Dapper;
using DapperQueryBuilder;
using System.Text.Json;

namespace tufol.Services
{
    public class UserService : IUser
    {
        private readonly Helpers.AppSettings _appSettings;
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public UserModel Login(string username, string password)
        {
            UserModel result = new UserModel();
            try
            {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT u.*, r.role_id FROM tb_user u JOIN tb_role r ON u.role_id = r.role_id AND u.username = '" + username + "' AND u.password = '" + password +"'";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            result.user_id = Convert.ToInt32(reader["user_id"].ToString());
                            result.role_id = (byte)(reader["role_id"]);
                            result.username = reader["username"].ToString();
                            result.email = reader["email"].ToString();
                            result.vendor_number = reader["vendor_number"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("Login", ex.Message);
            }
            return result;
        }

        public int CountUserData(string username = null, int role_id = 0, string generalSearch = null){
            int result = 0;
            try {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString)) {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(username)) {
                        string_condition += " AND A.username LIKE @username";
                    }
                    if (!string.IsNullOrEmpty(role_id.ToString()) && role_id != 0) {
                        string_condition += " AND A.role_id = @role_id";
                    }
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch OR A.username LIKE @generalSearch";
                    }
                    var sql = "SELECT COUNT(*) AS total " +
                            "FROM tb_user A " +
                            "WHERE 1=1 " + string_condition ;
                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(role_id.ToString()) && role_id != 0)
                    {
                        command.Parameters.AddWithValue("@role_id", role_id);
                    }

                    if (!string.IsNullOrEmpty(username))
                    {
                        command.Parameters.AddWithValue("@username", "%" + username + "%");
                    }
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            } catch (Exception ex) {
                new Helpers.GlobalFunction().LogError("CountUserData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetUserListDatatable(
            Dictionary<string, string> pagination,
            Dictionary<string, string> sort,
            Dictionary<string, string> query,
            string username = null, int role_id = 0
        ) {
            int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "desc" : sort["sort"]: "desc";
            string order = "A.created_at"; 
            if (sort.ContainsKey("field")) {
                if (!string.IsNullOrEmpty(sort["field"])) {
                    var type = sort["field"];
                    switch(type) {
                        case "procurement_group":
                        case "role":
                            order = type;
                            break;
                        default:
                            order = "A." + type;
                            break;
                    }
                }
            }
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;

            string orderby = order;
            Console.WriteLine("page: "+page+ ";perpage: "+perpage+";sorttype: "+sorttype+";orderby: "+orderby+";generalsearch: "+generalSearch);
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString)) {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(username)) {
                        string_condition += " AND A.username LIKE @username";
                    }
                    if (!string.IsNullOrEmpty(role_id.ToString()) && role_id != 0) {
                        string_condition += " AND A.role_id = @role_id";
                    }
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch OR A.username LIKE @generalSearch";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT " +
                            "CONCAT(C.initial_area,' - ', C.person_number) AS procurement_group, "+
                            "A.user_id, B.role_name as role, " +
                            "A.name, A.username, A.email " +
                            "FROM tb_user A " +
                            "JOIN tb_role B ON A.role_id = B.role_id " +
                            "LEFT JOIN tb_procurement_group C ON A.initial_area = C.initial_area " +
                            "WHERE 1=1 "+ string_condition + " " + string_order + " " +
                            "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(role_id.ToString()) && role_id != 0)
                    {
                        command.Parameters.AddWithValue("@role_id", role_id);
                    }

                    if (!string.IsNullOrEmpty(username))
                    {
                        command.Parameters.AddWithValue("@username", "%" + username + "%");
                    }

                     if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }
                    
                    Console.WriteLine("sql :" + sql);
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("user_id", Convert.ToUInt32(reader["user_id"].ToString()));
                            data_row.Add("role", reader["role"].ToString());
                            data_row.Add("name", reader["name"].ToString());
                            data_row.Add("username", reader["username"].ToString());
                            data_row.Add("email", reader["email"].ToString());
                            data_row.Add("procurement_group", reader["procurement_group"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                    total_data = this.CountUserData(username, role_id);
                    total_page = (total_data + perpage - 1) / perpage;
                }
            } 
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetUserListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }
        public IEnumerable<UserAreaModel> GetUserArea(int user_id){
            List<UserAreaModel> result = new List<UserAreaModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.*, B.company_id "+
                              ",CONCAT(UPPER(B.name),' ', UPPER(C.name)) AS company_area_name, B.address "+
                              "FROM tb_user_area A " +
                              "JOIN m_company_area B ON A.area_code = B.code " +
                              "JOIN m_company C on B.company_id = C.company_id "+
                              "WHERE A.user_id = @user_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@user_id", user_id);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new UserAreaModel(){
                                user_id = Convert.ToInt32(reader["user_id"].ToString()),
                                code = reader["area_code"].ToString(),
                                company_id = reader["company_id"].ToString(),
                                company_area_name = reader["company_area_name"].ToString(),
                                address = reader["address"] == DBNull.Value ? null : reader["address"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetUserArea", ex.Message);
                Console.WriteLine("User Area Get :"+ex.Message);
            }
            return result;   
        }

        public UserModel GetUserDetail(int user_id) {
            UserModel result = new UserModel();
            try{
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " +
                              "A.user_id, A.role_id, B.role_name as role, " +
                              "A.name, A.username, A.password, A.email, A.created_at, A.updated_at, A.reset_token " +
                              "FROM tb_user A " +
                              "JOIN tb_role B ON A.role_id = B.role_id " +
                              "WHERE A.user_id= @user_id";
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@user_id", user_id);
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.user_id = Convert.ToInt32(reader["user_id"].ToString());
                            result.role_id = Convert.ToInt32(reader["role_id"].ToString());
                            result.role = reader["role"].ToString();
                            result.name = reader["name"].ToString();
                            result.username = reader["username"].ToString();
                            result.password = reader["password"].ToString();
                            result.email = reader["email"].ToString();
                            result.reset_token = reader["reset_token"].ToString();
                            result.user_area = this.GetUserArea(Convert.ToInt32(reader["user_id"].ToString()));
                            result.created_at = Convert.ToDateTime(reader["created_at"].ToString());
                            result.updated_at = Convert.ToDateTime(reader["updated_at"].ToString());
                        }
                    }
                    cs.Close();
                }

            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetUserDetail", ex.Message);
            }
            return result;
        }

        public IEnumerable<RoleModel> GetRole() {
            List<RoleModel> result = new List<RoleModel>();
            try {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT role_id, role_name FROM tb_role where role_id not in (2,9)";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            result.Add(new RoleModel() 
                            {
                                role_id = Convert.ToInt32(reader["role_id"]),
                                role_name = reader["role_name"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }             
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetRole", ex.Message);
                Console.WriteLine(ex);
            }
            return result;
        }

        public IEnumerable<ProcurementGroupModel> GetProGroup() {
            List<ProcurementGroupModel> result = new List<ProcurementGroupModel>();
            try {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT initial_area, person_number, email FROM tb_procurement_group";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            var init_area = reader["initial_area"].ToString();
                            var person_num = reader["person_number"].ToString();
                            result.Add(new ProcurementGroupModel() 
                            {
                                initial_area = init_area,
                                person_number = person_num,
                                email = reader["email"].ToString(),
                                procurement_group = init_area.Substring(0, 3)
                            });
                        }
                    }
                    cs.Close();
                }
            }             
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetProGroup", ex.Message);
                Console.WriteLine(ex);
            }
            return result;
        }
        public string AddUser(UserModel model, string passEnc) {
            var result = "failed";
            int returnValue = -1;
            try {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "INSERT INTO tb_user (" +
                              "role_id, name, username, email, password, initial_area, vendor_number)" +
                              "VALUES(@role_id, @name, @username, @email, @password,  @initial_area, @vendor_number)";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@role_id", model.role_id);
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@username", model.username);
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@password", passEnc);
                    command.Parameters.AddWithValue("@initial_area", model.initial_area);
                    command.Parameters.AddWithValue("@vendor_number", (string.IsNullOrEmpty(model.vendor_number) ? DBNull.Value : model.vendor_number));

                    cs.Open();

                    // var reader = command.ExecuteReader();
                    returnValue = command.ExecuteNonQuery() == 1 ? 1: -1;
                    cs.Close();
                }

            } catch(Exception ex) {
                new Helpers.GlobalFunction().LogError("AddUser", ex.Message);
                Console.WriteLine(ex);
            }
            return result;
        }
        
        public async Task<int> AddUserIndesso(UserIndessoModel model, string passEnc) {
            int returnValue = 0;
            // var user_id;
            try {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var sql = "INSERT INTO tb_user (" +
                              "role_id, name, username, email, password, initial_area)" +
                              "OUTPUT inserted.user_id " +
                              "VALUES(@role_id, @name, @username, @email, @password,  @initial_area)";
                              
                    var user_id = await cs.ExecuteScalarAsync(@sql, new{
                            role_id = model.role_id, 
                            name = model.name, 
                            username = model.username,
                            email = model.email,
                            password = passEnc,
                            initial_area = model.initial_area
                        }, transaction: trans);
                    var deleteOldArea =  await cs.ExecuteAsync(@"DELETE FROM tb_user_area where user_id =@user_id", new{user_id = user_id}, transaction: trans);
                    Console.WriteLine("deletedarea: "+deleteOldArea);

                    var sqlArea = "INSERT INTO  tb_user_area(area_code, user_id) values ";
                    string[] company_area = model.company_area;
                    for(var i = 0; i < company_area.Length; i ++) {
                        Console.WriteLine("company_area :" +company_area[i]);
                        if (i != company_area.Length - 1 ) sqlArea += "('"+company_area[i].ToString()+"','"+user_id+"'),";
                        else sqlArea += "('"+company_area[i].ToString()+"','"+user_id+"')";
                    }
                    returnValue = await cs.ExecuteAsync(@sqlArea, transaction: trans);
                    trans.Commit();
                    cs.Close();
                }

            } catch(Exception ex) {
                new Helpers.GlobalFunction().LogError("AddUserIndesso", ex.Message);
                Console.WriteLine(ex);
            }
            return returnValue;
        }

        public async Task<bool> UpdateUser(UserIndessoModel model, int idUser) {
            var result = false;
            try {
                using(SqlConnection cs =  new SqlConnection(_appSettings.ConnectionString))
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var sql = "UPDATE tb_user SET role_id=@role_id, " +
                                "name=@name, username=@username, email=@email, initial_area=@initial_area " +
                                "WHERE user_id =@user_id"; 
                    var update = await cs.ExecuteAsync(@sql, new {
                            role_id = model.role_id, 
                            name = model.name, 
                            username = model.username,
                            email = model.email,
                            initial_area = model.initial_area,
                            user_id = idUser
                    }, transaction: trans);
                    var deleteOldArea =  await cs.ExecuteAsync(@"DELETE FROM tb_user_area where user_id =@user_id", new{user_id = idUser}, transaction: trans);
                    var sqlArea = "INSERT INTO  tb_user_area(area_code, user_id) values ";
                    string[] company_area = model.company_area;
                    for(var i = 0; i < company_area.Length; i ++) {
                        Console.WriteLine("company_area :" +company_area[i]);
                        if (i != company_area.Length - 1 ) sqlArea += "('"+company_area[i].ToString()+"','"+idUser+"'),";
                        else sqlArea += "('"+company_area[i].ToString()+"','"+idUser+"')";
                    }
                    var insertNewArea = await cs.ExecuteAsync(@sqlArea, transaction: trans);
                    if (update > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("UpdateUser", ex.Message);
            }
            return result;
        }

        public async Task<bool> DeleteUser(string idUser) {
            var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var deleteUserArea= await cs.ExecuteAsync(@"DELETE FROM tb_user_area WHERE user_id =@user_id", new {
                        user_id = idUser    
                    }, transaction: trans);
                    var deleteUser = await cs.ExecuteAsync(@"DELETE FROM tb_user WHERE user_id =@user_id", new {
                        user_id = idUser
                    }, transaction: trans);
                    if (deleteUser > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("DeleteUser", ex.Message);
            }
            return result;
        }

        public bool CheckUsername(string username)
        {
            var result = false;
            try
            {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT user_id FROM tb_user WHERE username = '" + username + "'";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        result = true;
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CheckUsername", ex.Message);
            }
            return result;
        }
        
        public UserModel GetRow(string idUser)
        {
            UserModel result = new UserModel();
            try
            {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT user_id FROM tb_user WHERE user_id = '" + idUser + "'";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        result.user_id = Convert.ToInt32(reader["user_id"].ToString());
                        result.role_id = (byte)(reader["role_id"]);
                        result.username = reader["username"].ToString();
                        result.email = reader["email"].ToString();
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                result = null;
                new Helpers.GlobalFunction().LogError("GetRow", ex.Message);
            }
            return result;
        }


        public bool updateLastLogin(string username, string password) {
            var result = false;
            Console.WriteLine(" name: "+username+" psswd: "+password);
            
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "UPDATE tb_user SET last_login=@last_login " +
                                "WHERE username =@username and password=@password"; 

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@last_login", DateTime.Now);
                    cs.Open();

                    var reader = command.ExecuteReader();

                    if(reader.HasRows)
                    {
                        result = true;
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("UpdateLastLogin", ex.Message);
            }
            return result;
        }

        public UserModel GetUsernameOrEmail(string username)
        {
            UserModel result = new UserModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM tb_user WHERE username = @username OR email = @username";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@username", username);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.user_id = Convert.ToInt32(reader["user_id"].ToString());
                            result.username = reader["username"].ToString();
                            result.email = reader["email"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetUsernameOrEmail", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public bool UpdateResetToken(int user_id, string token)
        {
            var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "UPDATE tb_user SET reset_token = @reset_token WHERE user_id = @user_id"; 

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@reset_token", token);
                    command.Parameters.AddWithValue("@user_id", user_id);
                    cs.Open();

                    var reader = command.ExecuteReader();

                    if(reader.HasRows)
                    {
                        result = true;
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("UpdateResetToken", ex.Message);
            }
            return result;
        }
        public int UpdatePassword(int user_id, string password, string token)
        {
            int result = 0;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "UPDATE tb_user SET password = @password, reset_token = NULL WHERE user_id = @user_id"; 
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@reset_token", token);
                    cs.Open();
                    Console.WriteLine(sql);

                    result = command.ExecuteNonQuery();

                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("UpdatePassword", ex.Message);
            }
            Console.WriteLine(result);
            return result;
        }

        public int UpdateByPassword(string password, string user_id)
        {
            var result = 0;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "UPDATE tb_user SET password = @password WHERE user_id = @user_id"; 

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@user_id", user_id);
                    cs.Open();

                    Console.WriteLine(sql);

                    var reader = command.ExecuteNonQuery();
                    Console.WriteLine(reader);

                    result = reader;
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("UpdateByPassword", ex.Message);
            }
            return result;
        }

        public string GetPassword(string password, string user_id)
        {
            var result = "";
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT password FROM tb_user WHERE password = @password AND user_id = @user_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@user_id", user_id);

                    cs.Open();

                    Console.WriteLine(sql);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["password"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("getPassword", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}