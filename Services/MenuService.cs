using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tufol.Interfaces;
using tufol.Models;
using tufol.Helpers;
using Microsoft.Data.SqlClient;

namespace tufol.Services
{
    public class MenuService : IMenu
    {
        private readonly Helpers.AppSettings _appSettings;
        public MenuService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IEnumerable<MenuModel> GetMenu(string role_id = null, int parent_id = 0)
        {
            List<MenuModel> result = new List<MenuModel>();
            if (string.IsNullOrEmpty(role_id)) return result;
            try
            {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.* FROM tb_permission A " +
                              "JOIN tb_menu B ON A.menu_id = B.menu_id " +
                              "WHERE A.role_id = @role_id " + (parent_id != 0 ? "AND B.parent_id = @parent_id ": "AND B.parent_id IS NULL ") + 
                              "AND A.allow_read = 1";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@role_id", role_id);
                    if ( parent_id != 0 )
                        command.Parameters.AddWithValue("@parent_id", parent_id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new MenuModel()
                            {
                                menu_id = Convert.ToInt32(reader["menu_id"].ToString()),
                                label = reader["label"].ToString(),
                                endpoint = reader["endpoint"].ToString(),
                                childs = this.GetMenu(role_id, Convert.ToInt32(reader["menu_id"].ToString())),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMenu", ex.Message);
            }
            return result;
        }
        public PermissionModel GetPermission(string endpoint, string role_id = null)
        {
            PermissionModel result = new PermissionModel();
            if (string.IsNullOrEmpty(role_id)) return result;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.* FROM tb_permission A " +
                              "JOIN tb_menu B ON A.menu_id = B.menu_id " +
                              "WHERE A.role_id = @role_id " +
                              "AND B.endpoint = @endpoint";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@role_id", role_id);
                    command.Parameters.AddWithValue("@endpoint", endpoint);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.allow_read = Convert.ToBoolean(reader["allow_read"].ToString());
                            result.allow_create = Convert.ToBoolean(reader["allow_create"].ToString());
                            result.allow_update = Convert.ToBoolean(reader["allow_update"].ToString());
                            result.allow_delete = Convert.ToBoolean(reader["allow_delete"].ToString());
                            result.allow_approve = Convert.ToBoolean(reader["allow_approve"].ToString());
                            result.allow_revise = Convert.ToBoolean(reader["allow_revise"].ToString());
                            result.allow_reject = Convert.ToBoolean(reader["allow_reject"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetPermission", ex.Message);
            }
            return result;
        }
    }
}