using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
    public interface IUser
    {
        UserModel Login(string username, string password); 
        bool CheckUsername(string username);
        // UserModel CreatedUser(string username, string password);
        UserModel GetRow(string idUser);

        IEnumerable<RoleModel> GetRole();
        IEnumerable<ProcurementGroupModel> GetProGroup();

        Dictionary<string, dynamic> GetUserListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query, string username = null, int role_id = 0);
        UserModel GetUserDetail(int user_id);

        string AddUser(UserModel model, string passEnc);
        Task<int> AddUserIndesso(UserIndessoModel model, string passEnc);
        Task<bool> UpdateUser(UserIndessoModel model, int idUser);

        Task<bool> DeleteUser(string idUser);

        bool UpdateResetToken( int user_id, string token);

        int UpdatePassword( int user_id, string password, string token);

        int UpdateByPassword(string password, string user_id);

        bool updateLastLogin(string username, string password);
        UserModel GetUsernameOrEmail(string usernameORemail);
        string GetPassword(string password, string user_id);
    }
}