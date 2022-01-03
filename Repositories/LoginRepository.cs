using DotNetCoreBoilerPlate.Interfaces;
using DotNetCoreBoilerPlate.Models;
using DotNetCoreBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreBoilerPlate.Repositories
{
    public class LoginRepository : ILogin
    {
        NetCoreBoilerPlateContext _db;
        public LoginRepository(NetCoreBoilerPlateContext db)
        {
            _db = db;
        }
        public LoginViewModel doLogin(string email, string pass)
        {
            if (_db != null)
            {
                LoginViewModel recordList = (from u in _db.BoilerTables

                                             where u.Email.ToLower() == email.ToLower()
                                             select new LoginViewModel
                                             {
                                                 ID = u.ID,
                                                 FirstName = u.FirstName,
                                                 LastName = u.LastName,
                                                 Email = u.Email,
                                                 Password = u.Password,
                                                 Description = u.Description
                                             }).FirstOrDefault();

                if (recordList != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(pass, recordList.Password))
                    {
                        recordList.Password = "";
                        return recordList;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            return null;
        }
    }
}
