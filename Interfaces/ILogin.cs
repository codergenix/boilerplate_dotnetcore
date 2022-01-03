using DotNetCoreBoilerPlate.ViewModels;

namespace DotNetCoreBoilerPlate.Interfaces
{
    public interface ILogin
    {
        LoginViewModel doLogin(string email, string pass);
    }
}
