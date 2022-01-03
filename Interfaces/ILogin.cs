using DotNetCoreBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreBoilerPlate.Interfaces
{
    public interface ILogin
    {
        LoginViewModel doLogin(string email, string pass);
    }
}
