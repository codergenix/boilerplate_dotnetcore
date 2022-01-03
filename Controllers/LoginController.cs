using DotNetCoreBoilerPlate.Interfaces;
using DotNetCoreBoilerPlate.Models;
using DotNetCoreBoilerPlate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNetCoreBoilerPlate.Controllers
{
    [Route("v1/login")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Authorize/login")]
    public class LoginController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly NetCoreBoilerPlateContext _context;
        private JwtSecurityToken _token;
        private ILogin _user;
        public LoginController(IConfiguration config, NetCoreBoilerPlateContext context, ILogin user)
        {
            _configuration = config;
            _context = context;
            _user = user;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                if (string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password))
                {
                    return Ok(new { success = false, err = "Please fill all information" });
                }
                else
                {
                    //var DataSource = _configuration.GetConnectionString("DataSource");
                    ////var DataSource = "24x7API";
                    //var UserID = _configuration.GetConnectionString("UserName");
                    //var Pass = _configuration.GetConnectionString("PWD");
                    //var Database = _configuration.GetConnectionString("DatabaseName");

                    //string cnstr = "Server=" + DataSource + ";Database=" + Database + ";UID=" + UserID + ";PWD=" + Pass + ";";
                    //_context.ConnectionString = cnstr;

                    var user = _user.doLogin(req.Email, req.Password);

                    if (user != null)
                    {
                        var claims = new[] {
                         new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                         new Claim("Email", req.Email)
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        _token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                        return Ok(new { success = true, result = user, token = "Bearer " + new JwtSecurityTokenHandler().WriteToken(_token) });
                    }
                    else
                    {
                        return Ok(new { success = false, err = "Invalid credentials" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, err = ex.Message });
            }
        }
    }

}
