using DotNetCoreBoilerPlate.Models;
using DotNetCoreBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreBoilerPlate.Interfaces
{
    public interface IUser
    {
        Task<Tuple<List<UserViewModel>, int>> GetAll(string order, string orderbyval, string searchValue, int skip, int pageSize);

        Task<UserViewModel> AddUser(UserViewModel user);

        Task<UserViewModel> UpdateUser(UserViewModel user);

        Task<int> DeleteUser(int userID);
    }
}
