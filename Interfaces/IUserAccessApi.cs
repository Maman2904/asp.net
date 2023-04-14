using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Models;

namespace tufol.Interfaces
{
  public interface IUserAccessApi
  {
    UserAccessApiModel GetUserDetail(string username);
  }
}