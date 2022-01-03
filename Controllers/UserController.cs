using DotNetCoreBoilerPlate.Interfaces;
using DotNetCoreBoilerPlate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DotNetCoreBoilerPlate.Controllers
{
    [Route("v1/users")]
    [Authorize]
    [ApiController]
    [ApiExplorerSettings(GroupName = "List/users")]
    public class UserController : ControllerBase
    {
        IUser usrRepository;

        public UserController(IUser _usrRepository)
        {
            usrRepository = _usrRepository;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> GetAll([FromBody] DataTableJSONViewModel reqBody)
        {
            try
            {
                string order = reqBody.order;
                string orderby = reqBody.orderBy;

                int skip = reqBody.skip;
                int pagesize = reqBody.pageSize;

                string searchValue = reqBody.search;

                var result = await usrRepository.GetAll(order, orderby, searchValue, skip, pagesize);

                return Ok(new { recordsTotal = result.Item2, recordsFiltered = result.Item1.Count, data = result.Item1, success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, err = ex.Message });
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel userData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await usrRepository.AddUser(userData);
                    if (result != null)
                    {
                        return Ok(new { success = true, result = result });
                    }
                    else
                    {
                        return Ok(new { success = false, err = "User not inserted" });
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new { success = false, err = ex.Message });
                }

            }
            return Ok(new { success = false, err = "Invalid user model" });
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel userData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await usrRepository.UpdateUser(userData);

                    return Ok(new { success = true, result = result });
                }
                catch (Exception ex)
                {
                    if (ex.GetType().FullName == "Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException")
                    {
                        return Ok(new { success = false, err = "User not found" });
                    }

                    return Ok(new { success = false, err = ex.Message });
                }
            }
            return Ok(new { success = false, err = "Invalid user model" });
        }

        [HttpDelete]
        [Route("userID")]
        public async Task<IActionResult> DeleteLessonType(int userID)
        {
            int result = 0;
            try
            {
                result = await usrRepository.DeleteUser(userID);
                if (result == 0)
                {
                    return Ok(new { success = false, err = "User not found" });
                }
                return Ok(new { success = true, result = "User deleted" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, err = ex.Message });
            }
        }
    }
}
